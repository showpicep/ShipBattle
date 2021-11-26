using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShipBattle
{
    class AnimationForHit
    {
        public List<Image> frame = new List<Image>();
        string path = @"C:\Users\Acer\Desktop\myproj2113\ShipBattle\ShipBattle\Properties\PicturesForAnimation\";

        public void GettingImages()
        {
            frame.Clear();
            for (int i = 1; i < 5; i++)
            {
                string imgPath = path + i.ToString() + ".png";
                FileStream fs = new FileStream(imgPath, FileMode.Open);
                Image img = Image.FromStream(fs);
                //MessageBox.Show(img.ToString());
                frame.Add(img);
                fs.Close();
            }
        }
    }
}
