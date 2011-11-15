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
using System.IO;
using System.Threading;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace PDFAsystent
{
    public partial class Form1 : Form
    {
        SplashForm _splash;
        public string ISBN, tempISBN;
        ConfigForm config;
        ISBNForm isbnForm;
        int percentage = 10;
        int pagesBelow = 0;
        Thread miniLoading;
        public bool searching = false;
        public List<PDFFile> pdfList;
        int howMuchLeft;
        PDFStripper pdfStripper;
        List<Button> buttonList;
        int pictureBoxWidth, pictureBoxHight;
        MagnifiedImageForm maxImageForm;
        bool firstTime = true;
        bool startThread = true;
        Thread searchISBN;
        List<PictureBox> pictureBoxes;
        List<CheckBox> checkBoxes;
        List<Button> magnifingButtons;
        int counter = 1, maxCounter = 1;
        PDFFileMerger pdfMerger;
        int pagesToWrite;
        bool threadAlive = true;
        public bool saveFile = false;
        int timesStarted;
        List<Button> pageRange;
        bool lastButton = false;
        public bool ISBNApproved;

        public int miniaturesDisabled = 0;
        string outputFile = System.IO.Path.GetTempPath()
            + @"PDFAsystent_tempfile.jpeg";
        string outputTempPDF = System.IO.Path.GetTempPath()
            + @"PDFAsystent_tempfile.pdf";
        public string outputTempPDF2 = System.IO.Path.GetTempPath()
            + @"PDFAsystent_tempfileview.pdf";
        string filePDF = System.IO.Path.GetTempPath();
        
        public Form1(SplashForm splash)
        {
            _splash = splash;

            InitializeComponent();
            splitContainer1.Panel2Collapsed = true;
            splitContainer6.Panel2Collapsed = true;
            this.Size = new Size(624, 293);

            config = new ConfigForm(this);
            isbnForm = new ISBNForm(this);
            miniaturesDisabled = config.MiniaturesDisabled;

            pdfStripper = new PDFStripper();
            pdfList = new List<PDFFile>();
            pageRange = new List<Button>();

            buttonList = new List<Button>(7) { buttonDelete1,
                buttonDelete2, buttonDelete3, buttonDelete4,
                buttonDelete5, buttonDelete6, buttonDelete7};

            int heightButtons = panel2.Height / 7;
            int locationY = panel2.Location.Y;
            foreach (Button but in buttonList)
            {
                but.Location = new Point(320, locationY);
                but.Height = heightButtons + 1;
                locationY += heightButtons;
            }

            RegistryKey RK = Registry.CurrentUser.OpenSubKey("Software\\PDFAsystent\\Directories");
            if (RK != null)
            {                
                openFileDialog1.InitialDirectory = RK.GetValue("Open_Directory", Environment.SpecialFolder.MyDocuments).ToString();
                timesStarted = Convert.ToInt32(RK.GetValue("TimesUsed", "0"));
            }
            
        }

        public void CheckPagesToWrite()
        {
            percentage = config.Percentage;
            pagesBelow = config.PagesBelow;

            if (config.AutomaticCheck == 1)
            {
                if (config.AutomaticCheckBelow == 1)
                    pagesBelow = config.PagesBelow;

                if (pagesBelow < 0)
                    pagesBelow = 30;

                pdfMerger.CheckPercentagePages(percentage, pagesBelow);
                pagesToWrite = pdfMerger.PageCount * percentage / 100;
                if (pagesToWrite > pagesBelow)
                    pagesToWrite = pagesBelow;
                if (pdfMerger.PageCount < 400)
                    label15.Text = labelCounting.Text = pagesToWrite +
                    @"/" + pdfMerger.PageCount;
                else
                    label15.Text = labelCounting.Text = pagesToWrite +
                    @"/" + 400;
            }
            else
            {
                pdfMerger.CheckPercentagePages(0, 0);
                if (pdfMerger.PageCount < 400)
                    label15.Text = labelCounting.Text = "0/" + pdfMerger.PageCount;
                else
                    label15.Text = labelCounting.Text = "0/" + 400;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (startThread)
            {
                ISBNApproved = false;
                lastButton = false;
                maxCounter = 1;
                counter = 1;
                pdfMerger = new PDFFileMerger(Convert.ToInt32(labelPagesCount.Text));
                
                if ((pdfMerger.PageCount > 100) && (maxCounter == 1))
                {
                    maxCounter = pdfMerger.PageCount / 100;
                    if (maxCounter * 100 < pdfMerger.PageCount)
                        maxCounter++;
                }

                splitContainer5.Panel2.Controls.Clear();
                splitContainer5.Panel2.Controls.Add(button3);
                splitContainer5.Panel2.Controls.Add(button2);

                if (maxCounter == 1)
                    pageRange.Clear();
                else
                {
                    splitContainer5.Panel2.Controls.Add(label9);
                    pageRange.Clear();
                    if (maxCounter <= 4)
                    {
                        CreateRangeButtonList(maxCounter);
                    }
                    else if (maxCounter > 4)
                    {
                        CreateRangeButtonList(4);
                        ReplaceLastButton();
                    }
                }
                CheckPagesToWrite();                
            }
            splitContainer1.Panel1Collapsed = true;
            this.Size = new Size(705, 436);
            if (firstTime)
            {
                pictureBoxWidth = panelImages.Width / 4 - 5;
                pictureBoxHight = panelImages.Height * 3 / 4;
                maxImageForm = new MagnifiedImageForm();
                panelImages.HorizontalScroll.SmallChange = (((panelImages.Width / 4) + 5) * 2);
                panelImages.HorizontalScroll.LargeChange = (((panelImages.Width / 4) + 5) * 3);
                
                pictureBoxes = PicturesGUI.CreatePictureBoxes(100,
                    pictureBoxWidth, pictureBoxHight);
                checkBoxes = PicturesGUI.CreateCheckBoxes(100,
                    (panelImages.Width / 11 - 5), (panelImages.Height * 8 / 10),
                    (panelImages.Width / 4 - 5));
                magnifingButtons = PicturesGUI.CreateMagnifingButtons(100, pictureBoxWidth);                
            }
              
            InsertTempPictures();
            CreateImagePanel();
            
            this.timer1.Enabled = true;
        }

        private void ReplaceLastButton()
        {
            pageRange[3].Text = "Strony " + (pdfMerger.PageCount - 99) + " - "
                + pdfMerger.PageCount;
            pageRange[3].Click -= pageButton_Click;
            pageRange[3].Click += new EventHandler(pageLastButton_Click);
        }

        private void CreateRangeButtonList(int count)
        {
            for (int i = 0; i < count; i++)                
                pageRange.Add(new Button());

            int locx = button3.Location.X;
            int buttonWidth = (locx - 125) / 4;
            int startingX = 0;

            if (count == 2)
                startingX = 50 + buttonWidth;
            else if (count == 3)
                startingX = 25 + (buttonWidth / 2);
            else if (count == 4)
                startingX = 25;

            foreach (Button but in pageRange)
            {
                but.Width = buttonWidth;
                but.Location = new Point(startingX, 25);
                startingX += 30 + buttonWidth;
                splitContainer5.Panel2.Controls.Add(but);
            }

            for (int i = 0; i < count; i++)
            {
                if (i == (count - 1))
                    pageRange[i].Text = "Strony " + ((i * 100) + 1) + " - "
                        + pdfMerger.PageCount;
                else
                    pageRange[i].Text = "Strony " + ((i * 100) + 1) + " - "
                        + ((i + 1) * 100);

                pageRange[i].Name = "Butt" + (i + 1);

                pageRange[i].Click += new EventHandler(pageButton_Click);
            }
        }

        private void pageLastButton_Click(object sender, EventArgs e)
        {

            int pagesCount = Convert.ToInt32(labelPagesCount.Text);
            panelImages.AutoScrollPosition = new Point(0, 0);
            int checkBoxCount;
            checkBoxCount = pdfMerger.checkBoxTextList.Count;

            panelImages.Controls.Clear();
            for (int i = 0; i < 100; i++)
            {
                this.panelImages.Controls.Add(pictureBoxes[i]);
                this.panelImages.Controls.Add(checkBoxes[i]);
                this.panelImages.Controls.Add(magnifingButtons[i]);
                magnifingButtons[i].BringToFront();
                magnifingButtons[i].Name = "Button" + pdfMerger.checkBoxTextList[checkBoxCount - 100 + i];
                checkBoxes[i].Text = pdfMerger.checkBoxTextList[checkBoxCount - 100 + i];
            }

            for (int i = 0; i < checkBoxes.Count; i++)
            {
                checkBoxes[i].CheckedChanged -= checkBox_CheckedChanged;
                for (int j = 0; j < pdfMerger.checkBoxTextList.Count; j++)
                {
                    if (checkBoxes[i].Text == pdfMerger.checkBoxTextList[j])
                    {
                        checkBoxes[i].Checked = pdfMerger.pagesToWrite[j];
                        break;
                    }
                }
                checkBoxes[i].CheckedChanged += new EventHandler(checkBox_CheckedChanged);
                if (checkBoxes[i].Checked)
                    pictureBoxes[i].BorderStyle = BorderStyle.Fixed3D;
                else
                    pictureBoxes[i].BorderStyle = BorderStyle.None;
            }
            panelImages.Invalidate();
            
            if (!threadAlive)
            {
                for (int i = 0; i < 100; i++)
                {
                    pictureBoxes[i].Image = pdfMerger.imageList[pdfMerger.PageCount - 100 + i];
                    pictureBoxes[i].Invalidate();
                }
            }

            lastButton = true;
        }

        private void pageButton_Click(object sender, EventArgs e)
        {
            Button butt = sender as Button;
            for (int i = 0; i < pageRange.Count; i++)
            {
                if (butt.Name == pageRange[i].Name)
                {
                    counter = (i + 1);
                    CreateImagePanel();
                    panelImages.Invalidate();     
                    break;
                }
            }
            lastButton = false;
        }

        private void CreateImagePanel()
        {
            int tempValue;
            int pagesCount = Convert.ToInt32(labelPagesCount.Text);

            panelImages.AutoScrollPosition = new Point(0, 0);

            if ((maxCounter > 1) && (counter != maxCounter))
            {
                tempValue = 100;
            }
            else if ((maxCounter > 1) && (counter == maxCounter))
            {
                tempValue = pagesCount - ((maxCounter - 1) * 100);
            }
            else
            {
                tempValue = pagesCount;
            }

            if (firstTime)
            {
                for (int i = 0; i < tempValue; i++)
                {
                    this.panelImages.Controls.Add(pictureBoxes[i]);
                    this.panelImages.Controls.Add(checkBoxes[i]);
                    this.panelImages.Controls.Add(magnifingButtons[i]);
                    magnifingButtons[i].BringToFront();
                    magnifingButtons[i].Name = "Button" + pdfMerger.checkBoxTextList[(i + ((counter - 1) * 100))];
                    checkBoxes[i].Text = pdfMerger.checkBoxTextList[(i + ((counter - 1) * 100))];

                    if (firstTime)
                    {
                        checkBoxes[i].CheckedChanged += new EventHandler(checkBox_CheckedChanged);
                        checkBoxes[i].Click += new EventHandler(checkBox_CheckedChanged);
                        pictureBoxes[i].Click += new EventHandler(pictureBox_Click);
                        magnifingButtons[i].Click += new EventHandler(button_Click);
                    }
                    panelImages.Invalidate();
                }

                ArrangeCheckOnLoad();
            }
            else if (pagesCount > 100)
            {
                if ((tempValue * 3) < panelImages.Controls.Count)
                {
                    int _tempValue = pictureBoxes.Count - tempValue;
                    for (int z = 0; z < _tempValue; z++)
                    {
                        PictureBox pic = pictureBoxes[pictureBoxes.Count - z - 1];
                        panelImages.Controls.Remove(pic);
                        Button but = magnifingButtons[magnifingButtons.Count - z - 1];
                        panelImages.Controls.Remove(but);
                        CheckBox check = checkBoxes[checkBoxes.Count - z - 1];
                        panelImages.Controls.Remove(check);
                    }
                }
                else
                {
                    int _tempValue = ((tempValue * 3) - panelImages.Controls.Count) / 3;
                    for (int z = 0; z <= _tempValue; z++)
                    {
                        PictureBox pic = pictureBoxes[pictureBoxes.Count - _tempValue + z - 1];
                        panelImages.Controls.Add(pic);
                        Button but = magnifingButtons[magnifingButtons.Count - _tempValue + z - 1];
                        panelImages.Controls.Add(but);
                        CheckBox check = checkBoxes[checkBoxes.Count - _tempValue + z - 1];
                        panelImages.Controls.Add(check);

                    }
                }
                for (int i = 0; i < tempValue; i++)
                {
                    magnifingButtons[i].BringToFront();
                    magnifingButtons[i].Name = "Button" + pdfMerger.checkBoxTextList[(i + ((counter - 1) * 100))];
                    checkBoxes[i].Text = pdfMerger.checkBoxTextList[(i + ((counter - 1) * 100))];
                }
                for (int i = 0; i < checkBoxes.Count; i++)
                {
                    checkBoxes[i].CheckedChanged -= checkBox_CheckedChanged;
                    for (int j = 0; j < pdfMerger.checkBoxTextList.Count; j++)
                    {
                        if (checkBoxes[i].Text == pdfMerger.checkBoxTextList[j])
                        {
                            checkBoxes[i].Checked = pdfMerger.pagesToWrite[j];
                            break;
                        }
                    }
                    checkBoxes[i].CheckedChanged += new EventHandler(checkBox_CheckedChanged);
                    if (checkBoxes[i].Checked)
                        pictureBoxes[i].BorderStyle = BorderStyle.Fixed3D;
                    else
                        pictureBoxes[i].BorderStyle = BorderStyle.None;
                }
                panelImages.Invalidate();
                //}
            }
            else
            {
                panelImages.Controls.Clear();
                for (int i = 0; i < tempValue; i++)
                {
                    this.panelImages.Controls.Add(pictureBoxes[i]);
                    this.panelImages.Controls.Add(checkBoxes[i]);
                    this.panelImages.Controls.Add(magnifingButtons[i]);
                    magnifingButtons[i].BringToFront();
                    magnifingButtons[i].Name = "Button" + pdfMerger.checkBoxTextList[(i + ((counter - 1) * 100))];
                    checkBoxes[i].Text = pdfMerger.checkBoxTextList[(i + ((counter - 1) * 100))];
               
                }
                for (int i = 0; i < checkBoxes.Count; i++)
                {
                    checkBoxes[i].CheckedChanged -= checkBox_CheckedChanged;
                    for (int j = 0; j < pdfMerger.checkBoxTextList.Count; j++)
                    {
                        if (checkBoxes[i].Text == pdfMerger.checkBoxTextList[j])
                        {
                            checkBoxes[i].Checked = pdfMerger.pagesToWrite[j];
                            break;
                        }
                    }
                    checkBoxes[i].CheckedChanged += new EventHandler(checkBox_CheckedChanged);
                    if (checkBoxes[i].Checked)
                        pictureBoxes[i].BorderStyle = BorderStyle.Fixed3D;
                    else
                        pictureBoxes[i].BorderStyle = BorderStyle.None;
                }
                panelImages.Invalidate();
            }

            if (startThread)
            {
                if (miniaturesDisabled == 0)
                    threadAlive = true;
                else
                    threadAlive = false;
                miniLoading = new Thread(ShowMiniatures);
                if (miniaturesDisabled == 1)
                    threadAlive = false;
                miniLoading.Start();
                groupBoxImages.Text = "Podgląd pliku: trwa ładowanie podglądu";
                startThread = false;
            }
            firstTime = false;

            if (!threadAlive)
            {
                for (int i = 0; i < tempValue; i++)
                {
                    pictureBoxes[i].Image = pdfMerger.imageList[i + ((counter - 1) * 100)];
                    pictureBoxes[i].Invalidate();
                }
            }
        }

        public void ArrangeCheckOnLoad()
        {
            for (int i = 0; i < checkBoxes.Count; i++)
            {
                checkBoxes[i].CheckedChanged -= checkBox_CheckedChanged;
                for (int j = 0; j < pdfMerger.checkBoxTextList.Count; j++)
                {
                    if (checkBoxes[i].Text == pdfMerger.checkBoxTextList[j])
                    {
                        checkBoxes[i].Checked = pdfMerger.pagesToWrite[j];
                        break;
                    }
                }
                checkBoxes[i].CheckedChanged += new EventHandler(checkBox_CheckedChanged);
                if (checkBoxes[i].Checked)
                    pictureBoxes[i].BorderStyle = BorderStyle.Fixed3D;
                else
                    pictureBoxes[i].BorderStyle = BorderStyle.None;
            }
        }

        private int GetPageNr(int page_nr, int fileIndex)
        {
            int temp, page = page_nr;
            for (int i = 0; i < fileIndex; i++)
            {
                temp = 0;
                for (int j = 0; j <= i; j++)
                    temp += pdfList[j].PageCount;
                if ((page_nr - temp) <= pdfList[i + 1].PageCount)
                {
                    page = page_nr - temp;
                    break;
                }
            }
            return page;
        }

        private void InsertTempPictures()
        {
            if (miniaturesDisabled == 0)
                for (int i = 0; i < pdfMerger.PageCount; i++)
                    pdfMerger.imageList.Add(Properties.Resources.load_test); 
            else
                for (int i = 0; i < pdfMerger.PageCount; i++)
                    pdfMerger.imageList.Add(Properties.Resources.no_miniature); 
        }

        private void ShowMiniatures()
        {
            int fileIndex, pageToShow;
            howMuchLeft = 0;
            for (int i = 0; i < pdfMerger.PageCount; i++)            
            {
                if ((i < 300) || (i >= (pdfMerger.PageCount - 100)))
                {
                    if (threadAlive)
                    {
                        fileIndex = GetIndexForFile(i + 1);
                        if (fileIndex > 0)
                            pageToShow = GetPageNr((i + 1), fileIndex);
                        else
                            pageToShow = i + 1;
                    }
                    else
                        break;

                    if (threadAlive)
                    {
                        pdfMerger.imageList.Insert(i, ImageResizer.ResizeMagnifiedImage(PDFViewer.GetImageFromPage(
                            (pdfList[fileIndex].FileDirectory + @"\" + pdfList[fileIndex].FileName),
                            outputFile, pageToShow, false), pictureBoxWidth, pictureBoxHight));
                        howMuchLeft++;
                    }
                    else
                        break;
                }
            }
            threadAlive = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = true;
            this.Size = new Size(624, 293);
        }

        private void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            listView1.DoDragDrop(listView1.SelectedItems, DragDropEffects.Move);
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            int len = e.Data.GetFormats().Length - 1;
            int i;
            for (i = 0; i <= len; i++)
            {
                if (e.Data.GetFormats()[i].Equals("System.Windows.Forms.ListView+SelectedListViewItemCollection"))
                {
                    e.Effect = DragDropEffects.Move;
                }
            }
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                return;
            }
            Point cp = listView1.PointToClient(new Point(e.X, e.Y));
            ListViewItem dragToItem = listView1.GetItemAt(cp.X, cp.Y);
            if (dragToItem == null)
            {
                return;
            }
            int dragIndex = dragToItem.Index;

            int newIndex = dragIndex;
            int oldIndex = listView1.SelectedIndices[0];
            PDFFile tempPdfFile;

            ListViewItem[] sel = new ListViewItem[listView1.SelectedItems.Count];
            for (int i = 0; i <= listView1.SelectedItems.Count - 1; i++)
            {
                sel[i] = listView1.SelectedItems[i];
            }
            for (int i = 0; i < sel.GetLength(0); i++)
            {
                ListViewItem dragItem = sel[i];
                int itemIndex = dragIndex;
                if (itemIndex == dragItem.Index)
                {
                    return;
                }
                if (dragItem.Index < itemIndex)
                    itemIndex++;
                else
                    itemIndex = dragIndex + i;
                
                ListViewItem insertItem = (ListViewItem)dragItem.Clone();
                listView1.Items.Insert(itemIndex, insertItem);            
                listView1.Items.Remove(dragItem);
            }
            if (newIndex > oldIndex)
            {
                int indexDiff= newIndex - oldIndex;
                tempPdfFile = pdfList[oldIndex];
                for (int i = 0; i < (newIndex - oldIndex); i++)
                {
                    pdfList[newIndex - indexDiff] = pdfList[newIndex - indexDiff + 1];
                    indexDiff--;
                }                
                pdfList[newIndex] = tempPdfFile;                
            }
            else if (newIndex < oldIndex)
            {
                int indexDiff = oldIndex - newIndex;
                tempPdfFile = pdfList[oldIndex];
                for (int i = 0; i < (oldIndex - newIndex); i++)
                {
                    pdfList[newIndex + indexDiff] = pdfList[newIndex + indexDiff - 1];
                    indexDiff--;
                }
                pdfList[newIndex] = tempPdfFile;
            }
        }

        private void addToListButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (threadAlive)                
                    threadAlive = false;
                startThread = true;
                
                string fileName = "";
                string fileDirectory = "";
                int pageCount;
                fileName = Path.GetFileName(openFileDialog1.FileName);
                fileDirectory = Path.GetDirectoryName(openFileDialog1.FileName);
                pageCount =
                    pdfStripper.GETPDFPageCount(openFileDialog1.FileName);
                pdfList.Add(new PDFFile(pageCount, fileName, fileDirectory));

                button1.Enabled = true;
                RefreshListFiles();
            }
        }

        private void RefreshListFiles()
        {
            listView1.Items.Clear();
            foreach (Button button in buttonList)
                button.Visible = false;

            foreach (PDFFile pdfFile in pdfList)
            {
                ListViewItem item = new ListViewItem(new[] { pdfFile.FileName, pdfFile.PageCount.ToString() });                
                listView1.Items.Add(item);                
            }

            for (int i = 0; i < pdfList.Count; i++)
                buttonList[i].Visible = true;
     
            labelFilesCount.Text = pdfList.Count.ToString();
            int fullPageCount = 0;
            foreach (PDFFile pdfFile in pdfList)
                fullPageCount += pdfFile.PageCount;
            labelPagesCount.Text = fullPageCount.ToString();
        }

        #region ListView1 buttons

        private void RemoveFromList(int index)
        {
            DialogResult result;
            result = MessageBox.Show("Czy na pewno chcesz usunąć plik " + pdfList[index].FileName + " z listy?",
                "PDF Asystent", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                pdfList.RemoveAt(index);
                RefreshListFiles();
                if (threadAlive)                
                    threadAlive = false;                
                startThread = true;
                if (listView1.Items.Count == 0)
                    button1.Enabled = false;
              
            }
        }

        private void buttonDelete1_Click(object sender, EventArgs e)
        {
            RemoveFromList(0);
        }

        private void buttonDelete2_Click(object sender, EventArgs e)
        {
            RemoveFromList(1);
        }

        private void buttonDelete3_Click(object sender, EventArgs e)
        {
            RemoveFromList(2);
        }

        private void buttonDelete4_Click(object sender, EventArgs e)
        {
            RemoveFromList(3);
        }

        private void buttonDelete5_Click(object sender, EventArgs e)
        {
            RemoveFromList(4);
        }

        private void buttonDelete6_Click(object sender, EventArgs e)
        {
            RemoveFromList(5);
        }

        private void buttonDelete7_Click(object sender, EventArgs e)
        {
            RemoveFromList(6);
        }
#endregion

        #region Events On Images Click

        private void pictureBox_Click(object sender, EventArgs e)
        {
            PictureBox picture = sender as PictureBox;
            if (picture.BorderStyle == BorderStyle.Fixed3D)
                picture.BorderStyle = BorderStyle.None;
            else
                picture.BorderStyle = BorderStyle.Fixed3D;

            for (int i = 0; i < pictureBoxes.Count; i++)
            {
                if (pictureBoxes[i].BorderStyle == BorderStyle.Fixed3D)
                    checkBoxes[i].Checked = true;
                else
                    checkBoxes[i].Checked = false;
            }
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            pagesToWrite = 0;
            for (int i = 0; i < checkBoxes.Count; i++)
            {
                if (checkBoxes[i].Checked == true)
                {
                    pictureBoxes[i].BorderStyle = BorderStyle.Fixed3D;
                    for (int j = 0; j < pdfMerger.checkBoxTextList.Count; j++)
                    {
                        if (checkBoxes[i].Text == pdfMerger.checkBoxTextList[j])
                        {
                            pdfMerger.pagesToWrite[j] = true;
                            break;
                        }
                    }
                }
                else
                {
                    pictureBoxes[i].BorderStyle = BorderStyle.None;
                    for (int j = 0; j < pdfMerger.checkBoxTextList.Count; j++)
                    {
                        if (checkBoxes[i].Text == pdfMerger.checkBoxTextList[j])
                        {
                            pdfMerger.pagesToWrite[j] = false;
                            break;
                        }
                    }
                }
            }
            foreach (bool ifTrue in pdfMerger.pagesToWrite)
                if (ifTrue == true)
                    pagesToWrite++;
            if (pdfMerger.PageCount < 400)
                label15.Text = labelCounting.Text = pagesToWrite + "/" + pdfMerger.PageCount;
            else
                label15.Text = labelCounting.Text = pagesToWrite + "/" + 400;
        }

        private void button_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                int page_nr = 1;
                Button button = sender as Button;
                string[] numbers = Regex.Split(button.Name, @"\D+");
                foreach (string value in numbers)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        page_nr = int.Parse(value);
                    }
                }

                if (maxImageForm.Visible == false)
                {
                    maxImageForm = new MagnifiedImageForm();
                    maxImageForm.Show(this);
                }

                int fileIndex, pageIndex;
                fileIndex = GetIndexForFile(page_nr);
                pageIndex = GetPageNr(page_nr, fileIndex);

                maxImageForm.ShowImage(pageIndex, pdfList[fileIndex]);
            }
        }
        #endregion

        private int GetIndexForFile(int page_nr)
        {
            int fileIndex = 0, addPages;
            for (int i = 0; i < pdfList.Count; i++)
            {
                addPages = 0;

                for (int j = 0; j <= i; j++)
                {
                    addPages += pdfList[j].PageCount;
                }

                if (page_nr <= addPages)
                {
                    fileIndex = i;
                    break;
                }
            }
            return fileIndex;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int tempValue;
            bool stopTimer = false;
            int howMuchToRead;

            if (pdfMerger.PageCount > 400)
                howMuchToRead = 400;
            else
                howMuchToRead = pdfMerger.PageCount;

            if (lastButton)
            {
                for (int i = 0; i < 100; i++)
                {
                    pictureBoxes[i].Image = pdfMerger.imageList[pdfMerger.PageCount - 100 + i];
                    pictureBoxes[i].BackgroundImage = Properties.Resources.background;
                    pictureBoxes[i].Invalidate();
                }
                groupBoxImages.Text = "Podgląd pliku: trwa ładowanie podglądu (załadowanych stron: " + howMuchLeft
                    + " z " + howMuchToRead + ")";
            }
            else
            {
                if ((maxCounter > 1) && (counter != maxCounter))
                {
                    tempValue = 100;
                }
                else if ((maxCounter > 1) && (counter == maxCounter))
                {
                    tempValue = pdfMerger.PageCount - ((maxCounter - 1) * 100);
                }
                else
                {
                    tempValue = pdfMerger.PageCount;
                }

                if (!threadAlive)
                {                   
                    stopTimer = true;
                }

                for (int i = 0; i < tempValue; i++)
                {                    
                    pictureBoxes[i].Image = pdfMerger.imageList[i + ((counter - 1) * 100)];                   
                    if (pictureBoxes[i].BackgroundImage == null)
                        pictureBoxes[i].BackgroundImage = Properties.Resources.background;
                    pictureBoxes[i].Invalidate();
                }

                groupBoxImages.Text = "Podgląd pliku: trwa ładowanie podglądu (załadowanych stron: " + howMuchLeft
                    + " z " + howMuchToRead + ")";

                if (stopTimer)
                {
                    groupBoxImages.Text = "Podgląd pliku";
                    timer1.Stop();
                    timer1.Enabled = false;
                }
            }            
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            threadAlive = false;

            string openPath = "";
            if (openFileDialog1.FileName != "")
                openPath = Path.GetDirectoryName(openFileDialog1.FileName);     
            RegistryKey regKey = Registry.CurrentUser;
            regKey = regKey.CreateSubKey("Software\\PDFAsystent\\Directories");
            if (openPath != "")
                regKey.SetValue("Open_Directory", openPath);
            if (timesStarted >= 0)
                regKey.SetValue("TimesUsed", timesStarted);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _splash.Close();
            Application.OpenForms["Form1"].BringToFront();
        }

        private void dodajPlikToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addToListButton_Click(sender, e);
        }

        private void zakończToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ustawieniaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            config.StartPosition = FormStartPosition.CenterParent;
            config.ShowDialog(this);
        }

   

        private void button2_Click(object sender, EventArgs e)
        {
            if (pagesToWrite == 0)
                MessageBox.Show("Wskaż strony, które chcesz wykorzystać", "PDF Asystent",
                     MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
            {
                splitContainer6.Panel1Collapsed = true;
                this.Size = new Size(543, 176);
                searching = true;
                searchISBN = new Thread(FindISBN);
                searchISBN.Start();
            }
        }

        private void FindISBN()
        {
            int z = 0;
            foreach (PDFFile pdf in pdfList)
            {
                for (int i = 0; i < pdf.PageCount; i ++)
                {
                    tempISBN = pdfStripper.SearchForISBNOnPage(i + 1, pdf.FileDirectory + @"\" + pdf.FileName);
                    z++;
                    if ((!String.IsNullOrEmpty(tempISBN)) || (z == 5))
                        break;
                }

                if (!String.IsNullOrEmpty(tempISBN))
                {
                    ISBN = "ISBN_" + tempISBN + ".pdf";                    
                    searching = false;
                    ISBNApproved = true;
                    break;
                }

                if (z == 5)
                {
                    ISBN = "none";
                    searching = false;
                    break;
                }
            }
        }

        private void StartISBNSearch()
        {
            isbnForm.StartPosition = FormStartPosition.CenterParent;
            isbnForm.ShowDialog(this);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            splitContainer6.Panel2Collapsed = true;
            this.Size = new Size(705, 436);           
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<int> pageList;
            string token = "brak", adress;

            while (searching)
                Thread.Sleep(100);
            //if (searching)
            //{
            //    ISBNWaitForm waitForm = new ISBNWaitForm(this);
            //    waitForm.StartPosition = FormStartPosition.CenterParent;
            //    waitForm.ShowDialog(this);
            //}

            if (ISBN == "none")
                StartISBNSearch();

            if (ISBNApproved)
            {
                RegistryKey RK = Registry.CurrentUser.OpenSubKey("Software\\PDFAsystent\\Directories");
                if (RK != null)
                {
                    token = RK.GetValue("Token", "brak").ToString();
                }

                if (token == "brak")
                {
                    if (File.Exists(@"token.txt"))
                    {
                        string tokentxt, email = "";

                        System.IO.StreamReader sr =
                            new System.IO.StreamReader(@"token.txt");
                        tokentxt = sr.ReadLine();
                        sr.Close();

                        int index = tokentxt.IndexOf("=");
                        if (index > 0)
                            token = tokentxt.Substring(0, index);
                        email = tokentxt.Substring(63);
                        email = email.Replace(" ", "");

                        Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.CurrentUser;
                        regKey = regKey.CreateSubKey("Software\\PDFAsystent\\Directories");
                        if (!String.IsNullOrEmpty(token))
                            regKey.SetValue("Token", token);
                        if (!String.IsNullOrEmpty(email))
                            regKey.SetValue("Email", email);
                    }
                }
                if ((token == "brak") || (token.Length < 60))
                {
                    ObtainTokenForm tokenForm = new ObtainTokenForm(this);
                    tokenForm.StartPosition = FormStartPosition.CenterParent;
                    tokenForm.ShowDialog();
                }
                else
                {
                    string base_url = GetParameters.GetParam("--url_base");
                    if (String.IsNullOrEmpty(base_url))
                        base_url = "https://www.elibri.com.pl";

                    //old adress = @"https://www.elibri.com.pl/upload_book_preview?token="
                    //             + token + @"&isbn=" + tempISBN;                  
                    adress = base_url + @"/product_previews?token="
                        + token + @"&isbn=" + tempISBN;

                    pageList = new List<int>();
                    filePDF = System.IO.Path.GetTempPath();
                    filePDF += ISBN;

                    for (int i = 0; i < pdfMerger.pagesToWrite.Count; i++)
                    {
                        if (pdfMerger.pagesToWrite[i] == true)
                            pageList.Add(i + 1);
                    }

                    UploadForm uploadForm = new UploadForm(pdfList, adress, outputTempPDF, filePDF, pageList, pdfStripper);

                    uploadForm.StartPosition = FormStartPosition.CenterParent;
                    uploadForm.ShowDialog(this);
                    File.Delete(outputTempPDF);
                    
                    timesStarted++;
                    if ((config.optionsUsed == 0) && ((timesStarted == 1)
                        || (timesStarted == 5)
                        || (timesStarted == 10)
                        || (timesStarted == 20)))
                        MessageBox.Show("Jeśli chcesz by program w przyszłości automatycznie zaznaczał część stron, skorzystaj z opcji dostępnych w menu głównym programu (Menu->Opcje->Ustawienia)",
                                "PDF Asystent", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MessageForm mForm = new MessageForm(this);
                    mForm.StartPosition = FormStartPosition.CenterParent;
                    mForm.ShowDialog(this);
                }
            }
    }

        private void CreateTempFile()
        {
            string[] pdfFilesToMerge = new string[pdfList.Count];
            for (int i = 0; i < pdfList.Count; i++)
            {
                pdfFilesToMerge[i] = pdfList[i].FileDirectory
                    + @"\" + pdfList[i].FileName;
            }
            pdfStripper.MergeFiles(outputTempPDF,
                pdfFilesToMerge);
        }

        public void StartFromBeginning()
        {      
            button4.Enabled = true;
            button5.Enabled = true;
            ISBN = "";

            pdfList.Clear();

            RefreshListFiles();
            if (threadAlive)
                threadAlive = false;
            startThread = true;
            if (listView1.Items.Count == 0)
                button1.Enabled = false;

            splitContainer1.Panel2Collapsed = true;
            splitContainer6.Panel2Collapsed = true;
            this.Size = new Size(624, 293);
            searching = false;
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.Size == new Size(543, 176))
            {
                splitContainer8.SplitterDistance = 61;
                splitContainer9.SplitterDistance = 351;
            }
            else if (this.Size == new Size(705, 436))
            {
                splitContainer4.SplitterDistance = 66;
                splitContainer5.SplitterDistance = 262;
            }
            else if (this.Size == new Size(624, 293))
            {
                splitContainer2.SplitterDistance = 57;
                splitContainer3.SplitterDistance = 56;
                splitContainer7.SplitterDistance = 25;
            }
        }

        private void wyczyśćListęToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("Czy na pewno chcesz wyczyścić listę plików?",
                "PDF Asystent", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                pdfList.Clear();
                if (threadAlive)
                    threadAlive = false;
                startThread = true;
                if (listView1.Items.Count == 0)
                    button1.Enabled = false;
                RefreshListFiles();
                button1.Enabled = false;
            }
        }

        public void button7_Click(object sender, EventArgs e)
        {
            File.Delete(outputTempPDF2);
            List<int> pageList = new List<int>();

            CreateTempFile();

            for (int i = 0; i < pdfMerger.pagesToWrite.Count; i++)
            {
                if (pdfMerger.PageCount > 400)
                {
                    if ((i < 300) || (i >= (pdfMerger.PageCount - 100)))
                    {
                        if (pdfMerger.pagesToWrite[i] == true)
                            pageList.Add(i + 1);
                    }
                }
                else
                {
                    if (pdfMerger.pagesToWrite[i] == true)
                    pageList.Add(i + 1);
                }
            }

            pdfStripper.ExtractPages(outputTempPDF, outputTempPDF2,
                pageList, progressBar1);

            if (!saveFile)
                System.Diagnostics.Process.Start(outputTempPDF2);
            else
            {
                saveFile = false;
            }
        }

        private void oProgramieToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.StartPosition = FormStartPosition.CenterParent;
            about.ShowDialog(this);
        }

        private void wczytajTokenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.FileName = "token.txt";
            openDialog.Filter = "Pliki tekstowe|*.txt";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                if (Path.GetFileName(openDialog.FileName) == "token.txt")
                {
                    string tokentxt, token = "", email = "";

                    System.IO.StreamReader sr =
                        new System.IO.StreamReader(openDialog.FileName);
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
                }
                else
                {
                    MessageBox.Show("Wskazano nieprawidłowy plik", "Wystąpił błąd",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                openDialog.Dispose();
            }
        }
    }
}
