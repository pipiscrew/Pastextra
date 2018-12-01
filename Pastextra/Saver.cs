using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Pastextra
{
    class Saver
    {

        public Saver(string path, string val)
        {
            string ext = ".txt";

            if (val.ToLower().StartsWith("http") && val.IndexOf("\r") == -1 && val.IndexOf(" ") == -1)
            {
                if ((Control.ModifierKeys == Keys.Shift || Control.ModifierKeys == Keys.Control) && File.Exists(Application.StartupPath + "\\wkhtmltopdf.exe"))
                {
                    Process msbProcess = new Process();
                    msbProcess.StartInfo.FileName = Application.StartupPath + "\\wkhtmltopdf.exe";
                    msbProcess.StartInfo.Arguments = "--title \"" + val + "\" \"" + val + "\" \"" + path  + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf\"";
                    msbProcess.StartInfo.UseShellExecute = false;
                    msbProcess.StartInfo.CreateNoWindow = (Control.ModifierKeys == Keys.Shift);

                    msbProcess.Start();
                    //msbProcess.WaitForExit();
                    return;
                }
                else
                {
                    //http://www.sorrowman.org/c-sharp-programmer/url-link-to-desktop.html
                    //https://social.msdn.microsoft.com/Forums/vstudio/en-US/af82ce78-6aa7-43cc-8a10-cdacd9b93728/find-url-from-a-text-file-in-c?forum=csharpgeneral
                    ext = ".url";
                    val = "[InternetShortcut]\r\nURL=" + val;
                }
            }


            File.WriteAllText(path + DateTime.Now.ToString("yyyyMMddHHmmss") + ext, val, new UTF8Encoding(false));
        }

        public Saver(string path, Image val)
        {
            val.Save(path + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png", ImageFormat.Png);
        }



    }
}
