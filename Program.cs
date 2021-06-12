using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ChatEnter chatE = new ChatEnter();
            Boolean switcher = true;

            if(chatE.ShowDialog() == DialogResult.OK)
            {
                if(chatE.numeText() != "")
                {
                    switcher = false;
                    MainChat mainC = new MainChat();
                    mainC.seteazaNume(chatE.numeText());
                    Application.Run(mainC);
                }
                else
                {
                    MessageBox.Show("No nickname detected, exiting application!");
                }
            }
            else
            {
                Application.Exit();
            }
        }
    }
}
