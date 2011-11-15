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
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace PDFAsystent
{
    public class PDFStripper
    {
        public int GETPDFPageCount(string filepath)
        {
            int pageCount = 0;
            PdfReader pdfFile = new PdfReader(filepath);
            pageCount = pdfFile.NumberOfPages;
            pdfFile.Close();
            return pageCount;
        }

        public void ExtractPages(string inputFile, string outputFile,
            List<int> extractPages, System.Windows.Forms.ProgressBar progres)
        {
            if (inputFile == outputFile)
            {
                System.Windows.Forms.MessageBox.Show("Nie możesz użyć pliku wejściowego jako wyjściowego do zapisu.");
            }

            PdfReader inputPDF = new PdfReader(inputFile);

            Document doc = new Document();
            PdfReader reader = new PdfReader(inputFile);
            progres.Maximum = reader.NumberOfPages;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(doc, memoryStream);
                doc.Open();
                doc.AddDocListener(writer);
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    progres.Value = i;
                    if (extractPages.FindIndex(s => s == i) == -1) continue;
                    doc.SetPageSize(reader.GetPageSize(i));
                    doc.NewPage();
                    PdfContentByte cb = writer.DirectContent;
                    PdfImportedPage pageImport = writer.GetImportedPage(reader, i);
                    int rot = reader.GetPageRotation(i);
                    if (rot == 90 || rot == 270)
                    {
                        cb.AddTemplate(pageImport, 0, -1.0F, 1.0F, 0, 0, reader.GetPageSizeWithRotation(i).Height);
                    }
                    else
                    {
                        cb.AddTemplate(pageImport, 1.0F, 0, 0, 1.0F, 0, 0);
                    }
                }
                reader.Close();
                doc.Close();
                try
                {
                    File.WriteAllBytes(outputFile, memoryStream.ToArray());
                }
                catch 
                {                    
                    throw new Exception("Błąd przy próbie zapisu do pliku. Upewnij się iż żaden inny proces obecnie go nie używa.");
                }
            }
        }

        public void MergeFiles(string destinationFile, string[] sourceFiles)
        {

            int f = 0;
            PdfReader reader = new PdfReader(sourceFiles[f]);
            int n = reader.NumberOfPages;
            Document document = new Document(reader.GetPageSizeWithRotation(1));
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(destinationFile, FileMode.Create));
            document.Open();
            PdfContentByte cb = writer.DirectContent;
            PdfImportedPage page;
            int rotation;
            while (f < sourceFiles.Length)
            {
                int i = 0;
                while (i < n)
                {
                    i++;
                    document.SetPageSize(reader.GetPageSizeWithRotation(i));
                    document.NewPage();
                    page = writer.GetImportedPage(reader, i);
                    rotation = reader.GetPageRotation(i);
                    if (rotation == 90 || rotation == 270)
                    {
                        cb.AddTemplate(page, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(i).Height);
                    }
                    else
                    {
                        cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                    }
                }
                f++;
                if (f < sourceFiles.Length)
                {
                    reader = new PdfReader(sourceFiles[f]);
                    n = reader.NumberOfPages;
                }
            }
            document.Close();
        }

        public string SearchForISBNOnPage(int pageNumber, string inputFile)
        {
            string text;
            string pattern = @"(97[89][- ]){0,1}[0-9]{1,5}[- ][0-9]{1,7}[- ][0-9]{1,6}[- ][0-9X]";
            string ISBN;

            PdfReader inputPDF = new PdfReader(inputFile);
            
            Document doc = new Document();
            PdfReader reader = new PdfReader(inputFile);
            text = PdfTextExtractor.GetTextFromPage(reader, pageNumber);

            if (Regex.IsMatch(text, pattern))
            {
                Match m = Regex.Match(text, pattern);
                ISBN = m.Value.Replace(" ", "-");
            }
            else
                ISBN = "";

            return ISBN;
        }
    }
}
