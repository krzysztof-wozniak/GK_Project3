using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;

namespace GK_Projekt3
{
    public partial class Form1 : Form
    {
        private int currentIndex = 0;
        private const double XR = (double)94.81;
        private const double YR = (double)100.0;
        private const double ZR = (double)107.3;

        private double redX = (double)0.6400;
        private double redY = (double)0.3300;

        private double greenX = (double)0.2100;
        private double greenY = (double)0.7100;

        private double blueX = (double)0.1500;
        private double blueY = (double)0.0600;

        private double whiteX = (double)0.3127;
        private double whiteY = (double)0.3290;

        private double gamma = (double)2.20;

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
            redXNumeric.Value = (decimal)redX;
            redYNumeric.Value = (decimal)redY;
            greenXNumeric.Value = (decimal)greenX;
            greenYNumeric.Value = (decimal)greenY;
            blueXNumeric.Value = (decimal)blueX;
            blueYNumeric.Value = (decimal)blueY;
            whiteXNumeric.Value = (decimal)whiteX;
            whiteYNumeric.Value = (decimal)whiteY;
            gammeNumeric.Value = (decimal)gamma;
            UpdatePictures();
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
            else //Lab
            {
                double maxL = 0;
                double minL = 0;
                double maxA = 0;
                double minA = 0;
                double maxB = 0;
                double minB = 0;
                double redZ = 1 - redX - redY;
                double blueZ = 1 - blueX - blueY;
                double greenZ = 1 - greenX - greenY;
                double whiteZ = 1.0 - whiteX - whiteY;
                //whiteX /= whiteY;
                //whiteY /= whiteY;
                //whiteZ /= whiteY;
                Vector<double> whiteVector = Vector<double>.Build.DenseOfArray(new double[] { whiteX, whiteY, whiteZ });
                Matrix<double> matrix = Matrix<double>.Build.DenseOfArray(new double[,] { { redX, greenX, blueX }, { redY, greenY, blueY }, { redZ, greenZ, blueZ } });
                Matrix<double> inversedMatrix = matrix.Inverse();
                Vector<double> sRGB = inversedMatrix.Multiply(whiteVector);
                for (int row = 0; row < matrix.RowCount; row++)
                {
                    for (int col = 0; col < matrix.ColumnCount; col++)
                    {
                        matrix[row, col] = matrix[row, col] * sRGB[col];
                    }
                }

                for (int i = 0; i < bitmap1.Width; i++)
                {
                    for (int j = 0; j < bitmap1.Height; j++)
                    {
                        Color pixel = bitmap1.GetPixel(i, j);
                        double rScaled = (double)pixel.R / (double)255;
                        double gScaled = (double)pixel.G / (double)255;
                        double bScaled = (double)pixel.B / (double)255;
                        rScaled = (double)Math.Pow(rScaled, gamma);
                        gScaled = (double)Math.Pow(gScaled, gamma);
                        bScaled = (double)Math.Pow(bScaled, gamma);
                        Vector<double> RGB = Vector<double>.Build.DenseOfArray(new double[] { rScaled, gScaled, bScaled });
                        Vector<double> XYZ = matrix.Multiply(RGB);
                        double X = XYZ[0];
                        double Y = XYZ[1];
                        double Z = XYZ[2];
                        double YYR = Y / YR; //Y / Yr
                        double cubicRootYYR = (double)Math.Pow(YYR, 1.0 / 3.0);
                        double L = 0, a = 0, b = 0;
                        if(YYR > 0.008856)
                        {
                            L = (double)116 * cubicRootYYR - 16;
                        }
                        else
                        {
                            L = 903.3 * YYR;
                        }
                        a = (double)500 * (double)((double)Math.Pow(X / XR, 1.0 / 3.0) - cubicRootYYR);
                        b = (double)200 * (double)(cubicRootYYR - (double)(Math.Pow(Z / ZR, 1.0 / 3.0)));
                        if(i == 0 && j == 0)
                        {
                            maxL = L;
                            minL = L;
                            maxA = a;
                            maxB = b;
                            minA = a;
                            minB = b;
                        }
                        else
                        {
                            if (maxL < L)
                                maxL = L;
                            if (minL > L)
                                minL = L;
                            if (maxA < a)
                                maxA = a;
                            if (minA > a)
                                minA = a;
                            if (maxB < b)
                                maxB = b;
                            if (minB > b)
                                minB = b;
                        }
                        //?
                        L *= 85;
                        a *= 8;
                        b *= 8;
                        int L1 = (int)Math.Round(L);
                        int a1 = (int)Math.Round(a);
                        int b1 = (int)Math.Round(b);
                        if (L1 > 255)
                            L1 = 255;
                        if (L1 < 0)
                            L1 = 0;
                        a1 += 127;
                        b1 += 127;
                        if (a1 > 255)
                            a1 = 255;
                        if (a1 < 0)
                            a1 = 0;
                        if (b1 > 255)
                            b1 = 255;
                        if (b1 < 0)
                            b1 = 0;
                        bitmap1.SetPixel(i, j, Color.FromArgb(L1, L1, L1));
                        bitmap2.SetPixel(i, j, Color.FromArgb(a1, 255 - a1, 127));
                        bitmap3.SetPixel(i, j, Color.FromArgb(b1, 127, 255 - b1));
                    }
                }
                descriptionLabel1.Text = $"minL = {minL.ToString("0.##")} maxL = {maxL.ToString("0.##")}";
                descriptionLabel2.Text = $"minA = {minA.ToString("0.##")} maxA = {maxA.ToString("0.##")}";
                descriptionLabel3.Text = $"minB = {minB.ToString("0.##")} maxB = {maxB.ToString("0.##")}";

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

        private void redXNumeric_ValueChanged(object sender, EventArgs e)
        {
            redX = (double)redXNumeric.Value;
        }

        private void redYNumeric_ValueChanged(object sender, EventArgs e)
        {
            redY = (double)redYNumeric.Value;
        }

        private void greenXNumeric_ValueChanged(object sender, EventArgs e)
        {
            greenX = (double)greenXNumeric.Value;
        }

        private void greenYNumeric_ValueChanged(object sender, EventArgs e)
        {
            greenY = (double)greenYNumeric.Value;
        }

        private void blueXNumeric_ValueChanged(object sender, EventArgs e)
        {
            blueX = (double)blueXNumeric.Value;
        }

        private void blueYNumeric_ValueChanged(object sender, EventArgs e)
        {
            blueY = (double)blueYNumeric.Value;
        }

        private void whiteXNumeric_ValueChanged(object sender, EventArgs e)
        {
            whiteX = (double)whiteXNumeric.Value;
        }

        private void whiteYNumeric_ValueChanged(object sender, EventArgs e)
        {
            whiteY = (double)whiteYNumeric.Value;
        }

        private void gammeNumeric_ValueChanged(object sender, EventArgs e)
        {
            gamma = (double)gammeNumeric.Value;
        }

        private void labTableLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdatePictures();
        }
    }
}
