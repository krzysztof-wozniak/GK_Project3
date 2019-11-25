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
        private int currentIndex = 0;


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
                for(int i = 0; i < bitmap1.Width; i++)
                {
                    for(int j = 0; j < bitmap1.Height; j++)
                    {
                        Color pixel = bitmap1.GetPixel(i, j);
                        float y = (float)pixel.R * (float)0.299 + (float)pixel.G * (float)0.587 + (float)pixel.B * (float)0.114;
                        float yScaled = y / (float)255;
                        float bScaled = (float)pixel.B / (float)255;
                        float rScaled = (float)pixel.R / (float)255;

                        float cbScaled = (bScaled - yScaled) / (float)1.772 + (float)0.5;
                        float crScaled = (rScaled - yScaled) / (float)1.402 + (float)0.5;
                        int cb = (int)Math.Round(cbScaled * 255);
                        int cr = (int)Math.Round(crScaled * 255);

                        int c = (int)Math.Round(y);
                        if (c > 255)
                            c = 255;
                        if (c < 0)
                            c = 0;
                        bitmap1.SetPixel(i, j, Color.FromArgb(c, c, c));
                        bitmap2.SetPixel(i, j, Color.FromArgb(127, 255 - cb, cb));
                        bitmap3.SetPixel(i, j, Color.FromArgb(cr, 255 - cr, 127));

                    }
                }
            }
            else if(modelComboBox.SelectedIndex == 1) //HSV
            {
                for (int i = 0; i < bitmap1.Width; i++)
                {
                    for (int j = 0; j < bitmap1.Height; j++)
                    {
                        Color pixel = bitmap1.GetPixel(i, j);

                        float rScaled = (float)pixel.R / (float)255;
                        float gScaled = (float)pixel.G / (float)255;
                        float bScaled = (float)pixel.B / (float)255;

                        float max = Math.Max(Math.Max(rScaled, gScaled), bScaled);
                        float min = Math.Min(Math.Min(rScaled, gScaled), bScaled);
                        float h = 0, s = 0, v = max;
                        //H:
                        if(max == min)
                        {
                            h = 0;
                        }
                        else
                        {
                            if(max == rScaled)
                            {
                                h = (float)60 * (gScaled - bScaled) / (max - min);
                            }
                            else if(max == gScaled)
                            {
                                h = (float)60 * ((float)2 + (bScaled - rScaled) / (max - min));
                            }
                            else
                            {
                                h = (float)60 * ((float)4 + (rScaled - gScaled) / (max - min));

                            }
                        }
                        if (h < 0)
                            h += (float)360;

                        //S:
                        if(max == 0)
                        {
                            s = 0;
                        }
                        else
                        {
                            s = (max - min) / max;
                        }
                        h = h / (float)360 * (float)255;
                        s = s * (float)255;
                        v = v * (float)255;

                        int H = (int)Math.Round(h);
                        int S = (int)Math.Round(s);
                        int V = (int)Math.Round(v);

                        bitmap1.SetPixel(i, j, Color.FromArgb(H, H, H));
                        bitmap2.SetPixel(i, j, Color.FromArgb(S, S, S));
                        bitmap3.SetPixel(i, j, Color.FromArgb(V, V, V));

                    }
                }
            }
            pictureBox1.Image = bitmap1;
            pictureBox2.Image = bitmap2;
            pictureBox3.Image = bitmap3;
            oldimage1.Dispose();
            oldimage2.Dispose();
            oldimage3.Dispose();
        }

    
        private void modelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (modelComboBox.SelectedIndex == currentIndex)
                return;
            switch(modelComboBox.SelectedIndex)
            {
                case 0:
                    descriptionLabel1.Text = "Y";
                    descriptionLabel2.Text = "Cb";
                    descriptionLabel3.Text = "Cr";
                    break;
                case 1:
                    descriptionLabel1.Text = "H";
                    descriptionLabel2.Text = "S";
                    descriptionLabel3.Text = "V";
                    break;
                case 2:
                    descriptionLabel1.Text = "L";
                    descriptionLabel2.Text = "a";
                    descriptionLabel3.Text = "b";
                    break;

            }
            UpdatePictures();
            currentIndex = modelComboBox.SelectedIndex;
        }
    }
}
