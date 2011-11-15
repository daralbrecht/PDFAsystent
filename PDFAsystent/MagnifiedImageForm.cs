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
using System.Drawing;
using System.Windows.Forms;

namespace PDFAsystent
{
    public partial class MagnifiedImageForm : Form
    {
        string outputFile = System.IO.Path.GetTempPath()
            + @"PDFAsystent_tempfile2.jpeg";

        public MagnifiedImageForm()
        {
            InitializeComponent();
        }

        public void ShowImage(int page_nr, PDFFile pdfFile)
        {
            pictureBox1.Image = ImageResizer.ResizeMagnifiedImage(PDFViewer.GetImageFromPage(
                (pdfFile.FileDirectory + @"\" + pdfFile.FileName),
                outputFile, page_nr, true), pictureBox1.Width, pictureBox1.Height);
        }
    }
}
