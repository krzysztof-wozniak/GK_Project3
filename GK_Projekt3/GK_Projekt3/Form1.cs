using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GK_Projekt3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void sourcePictureBox_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files(*.BMP; *.JPG; *.PNG; *.GIF)| *.BMP; *.JPG; *.PNG; *.GIF | All files(*.*) | *.*";
                openFileDialog.RestoreDirectory = true;
                Image image = null;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Image oldImage = sourcePictureBox.Image;
                    image = Image.FromFile(openFileDialog.FileName);
                    sourcePictureBox.Image = image;
                    if (oldImage != null)
                        oldImage.Dispose();
                }
            }
            UpdatePictures();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            modelComboBox.SelectedIndex = 0;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void UpdatePictures()
        {
            Bitmap bitmap1 = new Bitmap(sourcePictureBox.Image);
            Bitmap bitmap2 = new Bitmap(sourcePictureBox.Image);
            Bitmap bitmap3 = new Bitmap(sourcePictureBox.Image);
            Image oldimage1 = pictureBox1.Image;
            Image oldimage2 = pictureBox2.Image;
            Image oldimage3 = pictureBox3.Image;
            if (modelComboBox.SelectedIndex == 0)//YCbCr
            {
                for(int i = 0; i < bitmap.Width; i++)
                {
                    for(int j = 0; j < bitmap.Height; j++)
                    {
                        Color Y = bitmap1.GetPixel(i, j);
                        double y = (double)Y.R * 0.299 + (double)Y.G * 0.587 + (double)Y.B * 0.114;
                        int c = (int)Math.Round(y);
                        if (c > 255)
                            c = 255;
                        if (c < 0)
                            c = 0;
                        bitmap1.SetPixel(i, j, Color.FromArgb(c, c, c));
                    }
                }
            }
            pictureBox1.Image = bitmap1;
            oldimage1.Dispose();
            oldimage2.Dispose();
            oldimage3.Dispose();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
