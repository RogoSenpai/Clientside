using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class MainChat : Form
    {
        public TcpClient socketClient;
        public NetworkStream stream = default(NetworkStream);
        string citireDate = null;
        string nume = null;
        Thread threadClient;
        List<string> chatActual = new List<string>();
        List<string> chat = new List<string>();

        public MainChat()
        {
            InitializeComponent();
        }

        private void MainChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogres = MessageBox.Show("Do you really want to leave?", "Exit", MessageBoxButtons.YesNo);
            if (dialogres == DialogResult.Yes)
            {
                try
                {
                    threadClient.Abort();
                    socketClient.Close();
                }
                catch (Exception error) { }

                Application.ExitThread();
            }
            else if(dialogres == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!textBox3.Text.Equals(""))
                {
                    chat.Add("chat");
                    chat.Add(textBox3.Text);
                    byte[] outs = ObiectToByte(chat);
                    stream.Write(outs, 0, outs.Length);
                    stream.Flush();
                    textBox3.Text = "";
                    chat.Clear();
                    
                }
            }
            catch(Exception error)
            {
                button1.Enabled = true;
            }
        }

        public byte[] ObiectToByte(object _obiect)
        {
            using(var stream = new MemoryStream())
            {
                var formatare = new BinaryFormatter();
                formatare.Serialize(stream, _obiect);
                return stream.ToArray();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            socketClient = new TcpClient();
            try
            {
                socketClient.Connect("127.0.0.1", 8080);
                citireDate = "Connected!";
                mesaj();
                stream = socketClient.GetStream();
                byte[] outs = Encoding.ASCII.GetBytes(nume + "$");
                stream.Write(outs, 0, outs.Length);
                stream.Flush();
                button1.Enabled = false;
                threadClient = new Thread(getMesaj);
                threadClient.Start();
            }
            catch(Exception error)
            {
                MessageBox.Show("Server has not started yet.");
            }
        }
        
        private void getMesaj()
        {
            try
            {
                while (true)
                {
                    stream = socketClient.GetStream();
                    byte[] ins = new byte[10025];
                    stream.Read(ins, 0, ins.Length);
                    List<string> parti = null;

                    if (!socketConectat(socketClient))
                    {
                        MessageBox.Show("You have been disconnected!");
                        threadClient.Abort();
                        socketClient.Close();
                        button1.Enabled = true;
                    }

                    parti = (List<string>)ByteToObject(ins);
                    switch (parti[0])
                    {
                        case "listaUsers":
                            getUseri(parti);
                            break;
                        case "chat":
                            citireDate = "" + parti[1];
                            mesaj();
                            break;
                    }

                    if (citireDate[0].Equals('\0'))
                    {
                        citireDate = "Reconnect.";
                        mesaj();
                        this.Invoke((MethodInvoker)delegate
                        {
                            button1.Enabled = true;
                        });
                        threadClient.Abort();
                        socketClient.Close();
                        break;
                    }
                    chat.Clear();
                }
            }
            catch(Exception error)
            {
                threadClient.Abort();
                socketClient.Close();
                button1.Enabled = true;
                Console.WriteLine(error);
            }
        }

        public void getUseri(List<string> l)
        {
            this.Invoke((MethodInvoker)delegate
            {
                listBox1.Items.Clear();
                for (int i = 1; i < l.Count; i++)
                {
                    listBox1.Items.Add(l[i]);
                }
            });
        }

        public Object ByteToObject(byte[] arr)
        {
            using (var stream = new MemoryStream())
            {
                var formatare = new BinaryFormatter();
                stream.Write(arr, 0, arr.Length);
                stream.Seek(0, SeekOrigin.Begin);
                var obiect = formatare.Deserialize(stream);
                return obiect;
            }
        }
        bool socketConectat(TcpClient tc)
        {
            bool switcher = false;
            try
            {
                bool partea1 = tc.Client.Poll(10, SelectMode.SelectRead);
                bool partea2 = (tc.Available == 0);
                if (partea1 && partea2)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        button1.Enabled = true;
                    });
                    switcher = false;
                }
                else
                {
                    switcher = true;
                }
            }
            catch(Exception error)
            {
                Console.WriteLine(error);
            }
            return switcher;
        }
        public void seteazaNume(String s)
        {
            nume = s;
        }

        private void mesaj()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(mesaj));
            }
            else
            {
                textBox1.Text = textBox1.Text + Environment.NewLine + citireDate;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.TextLength;
            textBox1.ScrollToCaret();
        }

        private void MainChat_Load(object sender, EventArgs e)
        {

        }
    }
}
