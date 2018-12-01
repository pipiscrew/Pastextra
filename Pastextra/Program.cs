using Microsoft.Win32;
using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Pastextra
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            
            if (args.Length > 0)
            {
                string path = args[0];

                if (path.EndsWith("\"")) //when in root drive adds a quote >> wtf??
                  path=  path.Substring(0, path.Length - 1);

                path = path.EndsWith("\\") ? path : path + "\\";
                
                if (Clipboard.ContainsText())
                {
                    string c = Clipboard.GetText(); //.Trim();

                    if (string.IsNullOrWhiteSpace(c))
                        MessageBox.Show("Empty text detected.\r\n\r\nOperation aborted!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else 
                        new Saver(path, c);
                }
                else if (Clipboard.ContainsImage()) //identify image type - http://stackoverflow.com/a/5209528
                    new Saver(path, Clipboard.GetImage());
                else
                    MessageBox.Show("Unknown clipboard content.\r\n\r\nOperation aborted!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                if (Registry.GetValue(@"HKEY_CLASSES_ROOT\Directory\Background\shell\Pastextra\command", "", null) == null)
                    RegisterApp();
                else
                    UnRegisterApp();
            }

        }

        private static void UnRegisterApp()
        {
            if (MessageBox.Show("This action *will remove* the app explorer context item. \r\n\r\nDo you want to continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                return;

            var key = Registry.ClassesRoot.OpenSubKey("Directory").OpenSubKey("Background").OpenSubKey("shell", true);
            key.DeleteSubKeyTree("Pastextra");

            key = Registry.ClassesRoot.OpenSubKey("Directory").OpenSubKey("shell", true);
            key.DeleteSubKeyTree("Pastextra");

            MessageBox.Show("Application has been removed from your system", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void RegisterApp()
        {
            if (MessageBox.Show("This action *will add* the app explorer context item. \r\n\r\nDo you want to continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
                return;

            var key = Registry.ClassesRoot.OpenSubKey("Directory").OpenSubKey("Background").OpenSubKey("shell", true).CreateSubKey("Pastextra");
            key.SetValue("Icon", Application.StartupPath + "\\pastextra.ico");
            key = key.CreateSubKey("command");
            key.SetValue("", Application.ExecutablePath + " \"%v\"");

            key = Registry.ClassesRoot.OpenSubKey("Directory").OpenSubKey("shell", true).CreateSubKey("Pastextra");
            key.SetValue("Icon", Application.StartupPath + "\\pastextra.ico");
            key = key.CreateSubKey("command");
            key.SetValue("", Application.ExecutablePath + " \"%1\"");
            MessageBox.Show("Application has been added to your system", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Unhandled Thread Exception",  MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            Environment.Exit(-1);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show((e.ExceptionObject as Exception).Message, "Unhandled UI Exception", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            Environment.Exit(-1);
        }

    }
}
