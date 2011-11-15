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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;


namespace PDFAsystent
{
    public partial class UploadForm : Form
    {
        string outputTempPDF, filePDF, adress;
        List<int> pageList;
        PDFStripper pdfStripper;
        List<PDFFile> pdfList;

        public UploadForm(List<PDFFile> pdfList, string adress, string outputTempPDF, string filePDF, List<int> pageList, PDFStripper pdfStripper)
        {
            InitializeComponent();
            this.outputTempPDF = outputTempPDF;
            this.filePDF = filePDF;
            this.pageList = pageList;
            this.pdfStripper = pdfStripper;
            this.adress = adress;
            this.pdfList = pdfList;
        }

        public void UploadFile(string url, string filePath)
        {
            WebClient webClient = new WebClient();
            Uri siteUri = new Uri(url);
            webClient.UploadProgressChanged += WebClientUploadProgressChanged;
            webClient.UploadFileCompleted += WebClientUploadCompleted;            
            webClient.UploadFileAsync(siteUri, "POST", filePath);
            
        }

        void WebClientUploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            progressBar1.Maximum = Convert.ToInt32(e.TotalBytesToSend);
            progressBar1.Value = Convert.ToInt32(e.BytesSent);
            label3.Text = e.BytesSent + " B";
            label5.Text = e.TotalBytesToSend + " B";
        }

        void WebClientUploadCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            string response = System.Text.Encoding.UTF8.GetString(e.Result);
            string[] messageFromServer = response.Split(' ');
            if (messageFromServer.Count() == 1)
            {
                MessageBox.Show("Operacja zakończona sukcesem.", "PDF Asystent",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else if (messageFromServer[0] == "OK")
            {
                string temp = "";
                for (int i = 1; i < messageFromServer.Count(); i++)
                {
                    temp += messageFromServer[i] + " ";
                }
                MessageBox.Show("Operacja zakończona sukcesem.\r\n" + temp, "PDF Asystent",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else if (messageFromServer[0] == "ERROR")
            {
                string temp = "";
                for (int i = 1; i < messageFromServer.Count(); i++)
                {
                    temp += messageFromServer[i] + " ";
                }
                MessageBox.Show("Wystąpił błąd podczas wysyłania pliku na serwer Elibri.\r\n" + temp, "PDF Asystent",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        private void UploadForm_Load(object sender, EventArgs e)
        {
            Application.Idle += new EventHandler(OnLoaded);
        }
              
        private void OnLoaded(object sender, EventArgs args)
        {
            Application.Idle -= new EventHandler(OnLoaded);

            label1.Text = "Przygotowywanie stron do wysłania";

            string[] pdfFilesToMerge = new string[pdfList.Count];
            for (int i = 0; i < pdfList.Count; i++)
            {
                pdfFilesToMerge[i] = pdfList[i].FileDirectory
                    + @"\" + pdfList[i].FileName;
            }
            pdfStripper.MergeFiles(outputTempPDF,
                pdfFilesToMerge);

            pdfStripper.ExtractPages(outputTempPDF, filePDF,
                                pageList, progressBar1);
            label1.Text = "Trwa wysyłanie pliku na serwer Elibri";
            UploadFile(adress, filePDF);
        }
    }
}
