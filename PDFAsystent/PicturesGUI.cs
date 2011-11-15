﻿//PDF Asystent, program wspierający działanie systemu Elibri
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
using System.Windows.Forms;
using System.Drawing;

namespace PDFAsystent
{
    public class PicturesGUI
    {        
        public static List<PictureBox> CreatePictureBoxes(int pageCount, 
            int width, int height) 
        {
            List<PictureBox> pictureBoxes = new List<PictureBox>();
            int x = 5;

            for (int i = 0; i < pageCount; i++)
            {

                pictureBoxes.Add(new PictureBox());
                pictureBoxes[i].Location = new System.Drawing.Point(x, 5);
                x += width + 5;
                pictureBoxes[i].SizeMode = PictureBoxSizeMode.CenterImage;
                pictureBoxes[i].Height = height;
                pictureBoxes[i].Width = width;
                pictureBoxes[i].Image = new Bitmap(Properties.Resources.load_test);
            }
            return pictureBoxes;
        }

        public static List<Button> CreateMagnifingButtons(int pageCount, int spaceBetween)
        {
            List<Button> magnifingButtons = new List<Button>();
            int x = (spaceBetween -15);

            for (int i = 0; i < pageCount; i++)
            {
                    magnifingButtons.Add(new Button());
                    magnifingButtons[i].Location = new System.Drawing.Point(x, 2);
                    x += spaceBetween + 5;
                    magnifingButtons[i].Image = PDFAsystent.Properties.Resources.magnifier;
                    magnifingButtons[i].Height = 22;
                    magnifingButtons[i].Width = 22;              
            }
            return magnifingButtons;
        }

        public static List<CheckBox> CreateCheckBoxes(int pageCount,
           int x, int y, int width)
        {
            List<CheckBox> checkBoxes = new List<CheckBox>();

            for (int i = 0; i < pageCount; i++)
            {
                checkBoxes.Add(new CheckBox());
                checkBoxes[i].Location = new System.Drawing.Point(x, y);
                x += width + 5;
            }

            return checkBoxes;
        }
    }
}
