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
using System.Linq;
using System.Text;
using System.Drawing;

namespace PDFAsystent
{
    public static class ImageResizer
    {
        public static Bitmap ResizeMagnifiedImage(Bitmap picture, int width, int height)
        {
            int temp_width = width, temp_height = height;
            float multiplier;            
            multiplier =  (float)temp_height / (float)picture.Height;
            height = Convert.ToInt32(picture.Height * multiplier);
            width = Convert.ToInt32(picture.Width * multiplier);

            if (width > temp_width)
            {
                multiplier = (float)temp_width / (float)width;
                height = Convert.ToInt32(height * multiplier);
                width = Convert.ToInt32(width * multiplier);
            }

            Bitmap resizedPicture = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(resizedPicture))
            {
                
                    graphics.DrawImage(picture, 0, 0, width, height);
                
            }
            return resizedPicture;
        }
    }
}
