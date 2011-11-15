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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PDFAsystent
{
    public class PDFFile
    {
        private int pageCount;
        public int PageCount
        {
            get
            {
                return pageCount;
            }
            set { pageCount = value; }
        }

        private string fileName;
        public string FileName
        {
            get { return fileName; }
            private set { fileName = value; }
        }

        private string fileDirectory;
        public string FileDirectory
        {
            get { return fileDirectory; }
            set
            {
                fileDirectory = value;              
            }
        }

        public List<Image> pageList;
        public List<string> checkBoxTextList;
        public List<bool> pagesToWrite;

        public PDFFile(int pageCount, string fileName, string fileDirectory)
        {            
            pageList = new List<Image>();
            checkBoxTextList = new List<string>();
            pagesToWrite = new List<bool>();
            PageCount = pageCount;
            FileName = fileName;
            this.FileDirectory = fileDirectory;
            PopulateLists();
        }

        private void PopulateLists()
        {
            for (int i = 0; i < PageCount; i++)
            {              
                    checkBoxTextList.Add(("Strona " + (i + 1)));
                    pagesToWrite.Add(false);                    
            }
        }
    }
}
