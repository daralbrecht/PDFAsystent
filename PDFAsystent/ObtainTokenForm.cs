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
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PDFAsystent
{
    public partial class ObtainTokenForm : Form
    {        
        Form1 form;

        public ObtainTokenForm(Form1 form)
        {
            InitializeComponent();            
            this.form = form;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (Path.GetFileName(openFileDialog1.FileName) == "token.txt")
                {
                    string tokentxt, token = "", email = "";

                    System.IO.StreamReader sr =
                        new System.IO.StreamReader(openFileDialog1.FileName);
                    tokentxt = sr.ReadLine();
                    sr.Close();

                    int index = tokentxt.IndexOf("=");
                    if (index > 0)
                    token = tokentxt.Substring(0, index);
                    email = tokentxt.Substring(63);

                    Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.CurrentUser;                    
                    regKey = regKey.CreateSubKey("Software\\PDFAsystent\\Directories");
                    if (!String.IsNullOrEmpty(token))
                        regKey.SetValue("Token", token);
                    if (!String.IsNullOrEmpty(email))
                        regKey.SetValue("Email", email);

                    if (String.IsNullOrEmpty(token))
                        MessageBox.Show("Błąd odczytu danych z pliku 'token.txt'. Upewnij się, że plik jest prawidłowy",
                            "Wystąpił błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    MessageBox.Show("Token dodany do programu.", "PDF Asystent",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Wskazano nieprawidłowy plik", "Wystąpił błąd",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                form.saveFile = true;
                form.button7_Click(sender, e);
                File.Copy(form.outputTempPDF2, saveFileDialog1.FileName + ".pdf", true);
                MessageBox.Show("Plik zapisany.", "PDF Asystent",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
