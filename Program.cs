using System;
using System.Windows.Forms;

namespace SerialPortEnhancedGUI
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Enable visual styles and run the enhanced serial communication form
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new EnhancedSerialForm());
        }
    }
}
