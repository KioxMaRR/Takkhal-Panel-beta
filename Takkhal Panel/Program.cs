using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Takkhal_Panel
{
    static class Program
    {
    
        static Mutex mutex = null;

        [STAThread]
        static void Main()
        {
            const string appName = "TakkhalLauncher";
            bool createdNew;

            mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                MessageBox.Show("The Launcher is already running.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1()); 
        }
    }
}
