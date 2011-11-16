using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace PDFAsystent
{
    public static class ErrorSender
    {
        public static string[] GetData()
        {
            string local_string = "localstr";

            if (File.Exists("Data.dat"))
            {
                string strFileData = "";
                using (FileStream inputStream = new FileStream("Data.dat", FileMode.Open, FileAccess.Read))
                {
                    DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();
                    cryptic.Key = ASCIIEncoding.ASCII.GetBytes(local_string);
                    cryptic.IV = ASCIIEncoding.ASCII.GetBytes(local_string);
                    CryptoStream crStream = new CryptoStream(inputStream, cryptic.CreateDecryptor(), CryptoStreamMode.Read);
                    StreamReader reader = new StreamReader(crStream);
                    strFileData = reader.ReadToEnd();
                    reader.Close();
                    inputStream.Close();
                }

                string[] _data = strFileData.Split(' ');

                if (_data.Length == 4)
                {
                    return _data;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static bool SendMail(string messageBody, string subject)
        {
            string[] _data = GetData();
            if (_data == null)
            {
                System.Windows.Forms.MessageBox.Show("Plik 'Data.dat' uszkodzony.", "PDF Asystent",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }
            else
            {
                string user = "brak";
                Microsoft.Win32.RegistryKey RK = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\PDFAsystent\\Directories");
                if (RK != null)
                {
                    user = RK.GetValue("Email", "brak").ToString();
                }

                messageBody += "\r\nProgram zarejestrowany na: " + user;
                SmtpClient client = new SmtpClient("smtp.gmail.com");
                MailAddress from = new MailAddress(_data[0],
                   "PDF Asystent");
                MailAddress to = new MailAddress(_data[1]);
                MailAddress to2 = new MailAddress(_data[2]);
                MailMessage message = new MailMessage(from, to);
                message.To.Add(to2);
                message.Body = messageBody;
                message.Subject = subject;
                client.Credentials = new NetworkCredential(_data[0], _data[3]);
                client.Port = 587;
                client.EnableSsl = true;
                try
                {
                    client.Send(message);
                }
                catch
                {
                    message.Dispose();
                    return false;
                }
                message.Dispose();
                return true;
            }
        }
    }
}
