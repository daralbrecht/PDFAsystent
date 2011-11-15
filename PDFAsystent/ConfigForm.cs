using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace PDFAsystent
{
    public partial class ConfigForm : Form
    {
        //percentage of pages for automatic check
        private int percentage;
        public int Percentage
        {
            get 
            {
                RegistryKey RK = Registry.CurrentUser.OpenSubKey("Software\\PDFAsystent\\Directories");
                if (RK != null)
                {
                    percentage = Convert.ToInt32(RK.GetValue("PercentageP", "10"));
                }
                else
                    percentage = 20;
                return percentage; 
            }
            set 
            {
                RegistryKey regKey = Registry.CurrentUser;
                regKey.OpenSubKey("Software\\PDFAsystent\\Directories");
                if (regKey == null)
                {
                    regKey = regKey.CreateSubKey("Software\\PDFAsystent\\Directories");
                }
                if (value != percentage)
                {
                    percentage = value;
                    if ((percentage <= 100) && (percentage > 0))
                        regKey.SetValue("PercentageP", percentage);
                }
            }
        }

        private int pagesBelow;
        public int PagesBelow
        {
            get
            {
                RegistryKey RK = Registry.CurrentUser.OpenSubKey("Software\\PDFAsystent\\Directories");
                if (RK != null)
                {
                    pagesBelow = Convert.ToInt32(RK.GetValue("PagesBel", "30"));
                }
                else
                    pagesBelow = 30;
                return pagesBelow;
            }
            set
            {
                if (value != pagesBelow)
                {
                    RegistryKey regKey = Registry.CurrentUser;
                    regKey.OpenSubKey("Software\\PDFAsystent\\Directories");
                    if (regKey == null)
                    {
                        regKey = regKey.CreateSubKey("Software\\PDFAsystent\\Directories");
                    }

                    pagesBelow = value;
                    if (pagesBelow > 0)
                        regKey.SetValue("PagesBel", pagesBelow);
                }            
            }
        }
        
        //if "0" disabled, if "1" enabled
        int automaticCheck;
        public int AutomaticCheck
        {
            get { return automaticCheck; }
            set { automaticCheck = value; }
        }

        //if "0" disabled, if "1" enabled
        int miniaturesDisabled;
        public int MiniaturesDisabled
        {
            get
            {
                RegistryKey RK = Registry.CurrentUser.OpenSubKey("Software\\PDFAsystent\\Directories");
                if (RK != null)
                {
                    miniaturesDisabled = Convert.ToInt32(RK.GetValue("MinDis", "0"));
                }
                else
                    miniaturesDisabled = 0;
                return miniaturesDisabled;
            }
            set { miniaturesDisabled = value; }
        }

        int automaticCheckBelow;
        public int AutomaticCheckBelow
        {
            get { return automaticCheckBelow; }
            set { automaticCheckBelow = value; }
        }

        Form1 form;
        public int optionsUsed;
        decimal startPercentage, startPages;
        bool startingCheck1, startingCheck2;        
               
        public ConfigForm(Form1 form)
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;
            this.form = form;

            RegistryKey RK = Registry.CurrentUser.OpenSubKey("Software\\PDFAsystent\\Directories");
            if (RK != null)
            {
                AutomaticCheck = Convert.ToInt32(RK.GetValue("autoCheck", "0"));
                AutomaticCheckBelow = Convert.ToInt32(RK.GetValue("autoBelow", "0"));
                percentage = Convert.ToInt32(RK.GetValue("PercentageP", "10"));
                miniaturesDisabled = Convert.ToInt32(RK.GetValue("MinDis", "0"));
                pagesBelow = Convert.ToInt32(RK.GetValue("PagesBel", "30"));
                optionsUsed = Convert.ToInt32(RK.GetValue("OptU", "0"));     
            }            

            if (AutomaticCheck == 1)
                checkBox1.Checked = true;
            else
                checkBox1.Checked = false;

            if (AutomaticCheckBelow == 1)
                checkBox3.Checked = true;
            else
                checkBox3.Checked = false;

            if (miniaturesDisabled == 1)
                checkBox2.Checked = true;
            else
                checkBox2.Checked = false;

            numericUpDown1.Value = Percentage;
            numericUpDown2.Value = PagesBelow;
                        
            startPercentage = numericUpDown1.Value;
            startPages = numericUpDown2.Value;
            startingCheck1 = checkBox1.Checked;
            startingCheck2 = checkBox2.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                label1.Enabled = true;
                numericUpDown1.Enabled = true;
                checkBox3.Enabled = true;
                label2.Enabled = true;
                if (checkBox3.Checked)
                {
                    label3.Enabled = true;
                    label4.Enabled = true;
                    numericUpDown2.Enabled = true;
                }
            }
            else
            {
                label1.Enabled = false;
                numericUpDown1.Enabled = false;
                label2.Enabled = false;
                label3.Enabled = false;
                label4.Enabled = false;
                numericUpDown2.Enabled = false;
                checkBox3.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = startPercentage;
            numericUpDown2.Value = startPages;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                AutomaticCheck = 1;
            else
                AutomaticCheck = 0;

            if (checkBox3.Checked)
                AutomaticCheckBelow = 1;
            else
                AutomaticCheckBelow = 0;

            if (checkBox2.Checked)
                miniaturesDisabled = 1;
            else
                miniaturesDisabled = 0;

            //pagesBelow = Convert.ToInt32(numericUpDown2.Value);

            RegistryKey regKey = Registry.CurrentUser;
            regKey = regKey.CreateSubKey("Software\\PDFAsystent\\Directories");
            
            if ((AutomaticCheck == 0) || (AutomaticCheck == 1))
                regKey.SetValue("autoCheck", AutomaticCheck);
            if ((percentage <=100 ) && (percentage >0))
                regKey.SetValue("PercentageP", percentage);
            if ((miniaturesDisabled == 0) || (miniaturesDisabled == 1))
                regKey.SetValue("MinDis", miniaturesDisabled);
            if (pagesBelow > 0)
                regKey.SetValue("PagesBel", pagesBelow);
            if ((automaticCheckBelow == 0) || (automaticCheckBelow == 1))
                regKey.SetValue("autoBelow", automaticCheckBelow);
            if (optionsUsed == 0)
                regKey.SetValue("OptU", "1");
            
            //check for new settings
            if ((form.pdfList.Count > 0) &&
                ((((startPercentage != numericUpDown1.Value) && (checkBox1.Checked))
                || (startingCheck1 != checkBox1.Checked)
                || (startingCheck2 != checkBox2.Checked)) && (form.pdfList.Count > 0)
                || (startPages == numericUpDown2.Value) && (checkBox3.Checked)))
            {
                form.CheckPagesToWrite();
                form.ArrangeCheckOnLoad();
            }

            this.Close();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            percentage = Convert.ToInt32(numericUpDown1.Value);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
                miniaturesDisabled = 1;
            else
                miniaturesDisabled = 0;
        }

        private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (checkBox2.Checked)
            {
                form.miniaturesDisabled = this.MiniaturesDisabled;
            }
            else
                form.miniaturesDisabled = this.MiniaturesDisabled;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                label3.Enabled = true;
                label4.Enabled = true;
                numericUpDown2.Enabled = true;
            }
            else
            {
                label3.Enabled = false;
                label4.Enabled = false;
                numericUpDown2.Enabled = false;
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            pagesBelow = Convert.ToInt32(numericUpDown2.Value);
        }
    }
}
