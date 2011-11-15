//PDF Asystent, program wspierający działanie systemu Elibri
//Ten plik jest częścią PDF Asystent
//Copyright (C) 2011 Albrecht Dariusz

//PDF Asystent jest wolnym oprogramowaniem; możesz go 
//rozprowadzać dalej i/lub modyfikować na warunkach Powszechnej
//Licencji Publicznej GNU, wydanej przez Fundację Wolnego
//Oprogramowania - według wersji 2-giej tej Licencji lub którejś
//z późniejszych wersji. 

//Niniejszy program rozpowszechniany jest z nadzieją, iż będzie on 
//użyteczny - jednak BEZ JAKIEJKOLWIEK GWARANCJI, nawet domyślnej 
//gwarancji PRZYDATNOŚCI HANDLOWEJ albo PRZYDATNOŚCI DO OKREŚLONYCH 
//ZASTOSOWAŃ. W celu uzyskania bliższych informacji - Powszechna 
//Licencja Publiczna GNU. 

//Z pewnością wraz z niniejszym programem otrzymałeś też egzemplarz
//Powszechnej Licencji Publicznej GNU (GNU General Public License);
//jeśli nie - napisz do Free Software Foundation, Inc., 59 Temple
//Place, Fifth Floor, Boston, MA  02110-1301  USA

//Aby skontaktować się z twórcą programu napisz mail na adres:
//dariusz.albrecht@gmail.com

using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace PDFAsystent
{
    public class PDFViewer
    {
        [DllImport("gsdll32.dll", EntryPoint = "gsapi_new_instance")]
        private static extern int CreateAPI(out IntPtr pinstance,
            IntPtr caller_handle);

        [DllImport("gsdll32.dll", EntryPoint = "gsapi_init_with_args")]
        private static extern int InitAPI(IntPtr instance, int argc, string[] argv);

        [DllImport("gsdll32.dll", EntryPoint = "gsapi_exit")]
        private static extern int ExitAPI(IntPtr instance);

        [DllImport("gsdll32.dll", EntryPoint = "gsapi_delete_instance")]
        private static extern void DeleteAPI(IntPtr instance);
                
        private static readonly string[] ArgsForAPI = new string[]
        {
            "-q",
            "-dQUIET",
            "-dSAFER",
            "-dBATCH",
            "-dNOPAUSE",
            "-dNOPROMPT",
            "-dNumRenderingThreads=4",
            "-sDEVICE=jpeg",    
            "-dPDFFitPage"            
        };

        private static readonly string[] ArgsForAPI2 = new string[]
        {
            "-q",
            "-dQUIET",
            "-dSAFER",
            "-dBATCH",
            "-dNOPAUSE",
            "-dNOPROMPT",
            "-dMaxBitmap=500000000",
            "-dNumRenderingThreads=4",
            "-dAlignToPixels=0",
            "-dGridFitTT=0",
            "-dTextAlphaBits=4",
            "-dGraphicsAlphaBits=4",
            "-sDEVICE=jpeg", 
            "-density 200x200",
            "-quality 100",       
        };
                       
        public static void CreateImageFromPage(string inputFile, string outputFile,
            int pageNumber, bool ifNice)
        {            
            ConvertPage(GetArgsForAPI(inputFile, outputFile, pageNumber, ifNice));
        }
                
        public static System.Drawing.Bitmap GetImageFromPage(string inputFile,
            string outputFile, int pageNumber, bool ifNice)
        {
            System.Drawing.Bitmap image = null;
            try
            {
                ConvertPage(GetArgsForAPI(inputFile, outputFile, pageNumber, ifNice));
                using (FileStream stream = new FileStream(outputFile, FileMode.Open,
                    FileAccess.Read))
                {
                    image = new System.Drawing.Bitmap(stream);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                SendMailForm sendMail = new SendMailForm(ex);
                sendMail.ShowDialog();
            }
            finally
            {
                try
                {
                    File.Delete(outputFile);
                }
                catch (IOException ex)
                {
                    System.Windows.Forms.MessageBox.Show("Błąd przy próbie usunięcia zasobów tymczasowych.\r\nTreść: "
                        + ex.Message, "Błąd", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);                   
                }                
            }
            return image;
        }
                
        private static string[] GetArgsForAPI(string inputFile, string outputFile,
            int pageNumber, bool ifNice)
        {
            System.Collections.ArrayList argsToSend;
            if (!ifNice)
                argsToSend = new System.Collections.ArrayList(ArgsForAPI);
            else
                argsToSend = new System.Collections.ArrayList(ArgsForAPI2);

            if (pageNumber <= 0)
                throw new ArgumentException("Podany numer strony jest nieprawidłowy.", "GetARGSForAPI.pageNumber");

            argsToSend.Add(String.Format("-dFirstPage={0}", pageNumber));
            argsToSend.Add(String.Format("-dLastPage={0}", pageNumber));
            
            argsToSend.Add(String.Format("-sOutputFile={0}", outputFile));
            argsToSend.Add(inputFile);

            return (string[])argsToSend.ToArray(typeof(string));
        }

        private static void ConvertPage (string[] argsToAPI)
        {
            IntPtr instance;
            lock (padlock)
            {
                CreateAPI(out instance, IntPtr.Zero);
                try
                {
                    int result = InitAPI(instance, argsToAPI.Length, argsToAPI);

                    if (result < 0)
                    {
                        throw new ExternalException("Błąd konwersji.", result);
                    }
                }
                catch (ExternalException ex)
                {
                    SendMailForm sendMail = new SendMailForm(ex);
                    sendMail.ShowDialog();
                }
                finally
                {
                    Clear(instance);
                }
            }
        }        

        private static object padlock = new object();

        private static void Clear(IntPtr instance)
        {
            ExitAPI(instance);
            DeleteAPI(instance);
        }
    }
}
