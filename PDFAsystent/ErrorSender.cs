using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.ComponentModel;

namespace PDFAsystent
{
    public static class ErrorSender
    {       
        public static bool SendMail(string messageBody, string subject)
        {
            string user = "brak";
            Microsoft.Win32.RegistryKey RK = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\PDFAsystent\\Directories");
            if (RK != null)
            {
                user = RK.GetValue("Email", "brak").ToString();                
            }

            messageBody += "\r\nProgram zarejestrowany na: " + user;
            SmtpClient client = new SmtpClient("smtp.gmail.com");
            MailAddress from = new MailAddress("email1@gmail.com",
               "PDF Asystent");            
            MailAddress to = new MailAddress("email2@gmail.com");
            MailAddress to2 = new MailAddress("email3@gmail.com");            
            MailMessage message = new MailMessage(from, to);
            message.To.Add(to2);
            message.Body = messageBody;
            message.Subject = subject;
            client.Credentials = new NetworkCredential("email1@gmail.com", "pass");
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
