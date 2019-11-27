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

        private double greenX = (double)0.3000;
        private double greenY = (double)0.6000;

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
                openFileDialog.InitialDirectory = System.IO.Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName;
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
            colorProfileComboBox.SelectedIndex = 0;
            illuminantComboBox.SelectedIndex = 0;
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
                double redZ = 1 - redX - redY;
                double blueZ = 1 - blueX - blueY;
                double greenZ = 1 - greenX - greenY;
                double whiteZ = 1.0 - whiteX - whiteY;
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
                        double YYR = Y / YR; 
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

        private void button1_Click(object sender, EventArgs e)
        {
            UpdatePictures();
        }

        private void colorProfileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (colorProfileComboBox.SelectedIndex)
            {
                case 0://sRGB
                    UpdateLabSettings(0.64, 0.33, 0.3, 0.6, 0.15, 0.06, 0.3127, 0.3290, 2.2);
                    break;
                case 1://Adobe
                    UpdateLabSettings(0.64, 0.33, 0.21, 0.71, 0.15, 0.06, 0.3127, 0.329, 2.2);
                    break;
                case 2://Apple
                    UpdateLabSettings(0.6250, 0.3400, 0.2800, 0.595, 0.155, 0.07, 0.3127, 0.329, 1.8);
                    break;
                case 3://CIE
                    UpdateLabSettings(0.735, 0.265, 0.274, 0.717, 0.167, 0.009, 0.3333, 0.3333, 2.2);
                    break;
                case 4://Wide
                    UpdateLabSettings(0.7347, 0.2653, 0.1152, 0.8264, 0.1566, 0.0177, 0.3457, 0.3585, 1.2);
                    break;

            }
        }

        private void UpdateLabSettings(double redX, double redY, double greenX, double greenY, double blueX, double blueY, double whiteX, double whiteY, double gamma)
        {
            redXNumeric.Value = (decimal)redX;
            redYNumeric.Value = (decimal)redY;
            greenXNumeric.Value = (decimal)greenX;
            greenYNumeric.Value = (decimal)greenY;
            blueXNumeric.Value = (decimal)blueX;
            blueYNumeric.Value = (decimal)blueY;
            whiteXNumeric.Value = (decimal)whiteX;
            whiteYNumeric.Value = (decimal)whiteY;
            gammeNumeric.Value = (decimal)gamma;
        }

        private void illuminantComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (illuminantComboBox.SelectedIndex)
            {
                case 0://A
                    whiteXNumeric.Value = (decimal)0.44757;
                    whiteYNumeric.Value = (decimal)0.40744;
                    break;
                case 1://B
                    whiteXNumeric.Value = (decimal)0.34840;
                    whiteYNumeric.Value = (decimal)0.35160;
                    break;
                case 2://C
                    whiteXNumeric.Value = (decimal)0.31006;
                    whiteYNumeric.Value = (decimal)0.31615;
                    break;
                case 3://D50
                    whiteXNumeric.Value = (decimal)0.34567;
                    whiteYNumeric.Value = (decimal)0.35850;
                    break;
                case 4://D75
                    whiteXNumeric.Value = (decimal)0.29902;
                    whiteYNumeric.Value = (decimal)0.31485;
                    break;
                case 5://E
                    whiteXNumeric.Value = (decimal)0.33333;
                    whiteYNumeric.Value = (decimal)0.33333;
                    break;
                case 6://F11
                    whiteXNumeric.Value = (decimal)0.38054;
                    whiteYNumeric.Value = (decimal)0.37691;
                    break;
            }
        }
    }
}
