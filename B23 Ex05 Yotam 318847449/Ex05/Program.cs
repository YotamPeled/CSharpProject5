using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ex05
{
    internal class Program
    {
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FormGameSettings formGameSettings = new FormGameSettings();

            while (true)
            {
                formGameSettings.ShowDialog();

                if (formGameSettings.DialogResult == DialogResult.Retry)
                {
                    continue;
                }
                else if (formGameSettings.DialogResult == DialogResult.Abort)
                {
                    formGameSettings = new FormGameSettings();
                }
                else
                {
                    break;
                }
            }
        }
    }
}
