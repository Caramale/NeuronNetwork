using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NeuralNetwork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int NetSide = 10;
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Button 'Clear'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.Strokes.Clear();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)inkCanvas.ActualWidth, (int)inkCanvas.ActualHeight, 96d, 96d, PixelFormats.Default);
            rtb.Render(inkCanvas);
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            using (FileStream fs = File.Open(NeuralNetwork.Properties.Resources.DrawingFile, FileMode.Create))
            {
                encoder.Save(fs);
            }
            Bitmap source1 = new Bitmap(Properties.Resources.DrawingFile);
            Bitmap bmp = CropImage(source1, new Rectangle(new System.Drawing.Point(10, 10), new System.Drawing.Size((int)inkCanvas.ActualWidth-10, (int)inkCanvas.ActualHeight-10)));
            Bitmap bm = CalculateRectangle(bmp);
            bm.Save(Properties.Resources.CropedImage);
            GetPixels(bm);
        }

        public double CheckArea(int x, int y, int HeightStep, int WidthStep, Bitmap bmp)
        {
            int SumOfPaintedPixel = 0;
            int Width = x + WidthStep;
            int Height = y + HeightStep;
            for(int i =x; i< Width && i<bmp.Width; i++)
            {
                for(int j=y; j< Height && j<bmp.Height; j++)
                {
                    System.Drawing.Color color = bmp.GetPixel(i, j);
                    if (!IsWhitePixel(color)) SumOfPaintedPixel++;
                }
            }
              return ((double)SumOfPaintedPixel/(HeightStep*WidthStep));
        }

        public void DivideOnCells(int Height, int Width, ref int NewHeight, ref int NewWidth)
        {
            NewHeight = (int) (Height / NetSide);
            NewWidth = (int) (Width / NetSide);
        }

        private void GetPixels(Bitmap bmp)
        {
            int Height = bmp.Height;
            int Width = bmp.Width;
            int NewHeight = 0;
            int NewWidth = 0;

            int Square = NetSide * NetSide;
            double[] result = new double[Square];
            DivideOnCells(Height, Width, ref NewHeight, ref NewWidth);
            for (int x = 0; x < Width; x+=NewWidth)
            {
                int index = index = (x / NewWidth) * NetSide;
                for (int y = 0; y < Height && index < Square; y+=NewHeight)
                {                   
                    result[index]  = CheckArea(x, y, NewHeight, NewWidth, bmp);
                    index = (x / NewWidth) * NetSide + (y+ NewHeight) / NewHeight;
                }
            }

            System.IO.File.WriteAllLines(Properties.Resources.PixelMapOfAnImage, result.Select(d => d.ToString()));         
            
        }

        /// <summary>
        /// Crops an image in rectangle
        /// </summary>
        /// <param name="source"></param>
        /// <param name="section"></param>
        /// <returns>New  image</returns>
        private Bitmap CropImage(Bitmap source, Rectangle section)
        {
            // An empty bitmap which will hold the cropped image
            Bitmap bmp = new Bitmap(section.Width, section.Height);

            Graphics g = Graphics.FromImage(bmp);

            // Draw the given area (section) of the source image
            // at location 0,0 on the empty bitmap (bmp)
            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

            return bmp;
        }

        /// <summary>
        /// Select an painted area of canvas by finding the top, the bottom, the right border and the left border of letter.
        /// </summary>
        /// <param name="p"></param>
        /// <returns>An croped image with no white borders.</returns>
        public Bitmap CalculateRectangle(Bitmap p)
        {
            int TopMost = 0;
            int RightBorder = 0;
            int BotomMost = 0;
            int LeftBorder = 0;
            //maxH
            bool flag = false;
            for (int i = 0; i < p.Height; i++)
            {
                for (int j = 0; j < p.Width; j++)
                {
                    if (!IsWhitePixel(p.GetPixel(j, i)))
                    {
                        TopMost = i;
                        flag = true;
                        break;
                    }
                }
                if (flag) break;
            }
            //minH
            flag = false;
            for (int i = p.Height - 1; i >= 0; i--)
            {
                for (int j = 0; j < p.Width; j++)
                {
                    if (!IsWhitePixel(p.GetPixel(j, i)))
                    {
                        BotomMost = i;
                        flag = true;
                        break;
                    }
                }
                if (flag) break;
            }
            //left
            flag = false;
            for (int j = 0; j < p.Width; j++)
            {
                for (int i = TopMost; i <= BotomMost; i++)
                {
                    if (!IsWhitePixel(p.GetPixel(j, i)))
                    {
                        LeftBorder = j;
                        flag = true;
                        break;
                    }
                }
                if (flag) break;
            }
            //right
            flag = false;
            for (int j = p.Width - 1; j >= 0; j--)
            {
                for (int i = TopMost; i <= BotomMost; i++)
                {
                    if (!IsWhitePixel(p.GetPixel(j, i)))
                    {
                        RightBorder = j;
                        flag = true;
                        break;
                    }
                }
                if (flag) break;
            }
            return CropImage(p, new Rectangle(new System.Drawing.Point(LeftBorder, TopMost), new System.Drawing.Size(RightBorder - LeftBorder+1, BotomMost - TopMost+1)));
        }

        /// <summary>
        /// Checks if pixel is white or not.    
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns><c>true</c> if pixel is white, <c>false</c> otherwise</returns>
        public bool IsWhitePixel(System.Drawing.Color pixel)
        {
            return pixel.Name.Equals("ffffffff") || pixel.Name.Equals("0");
        }
    }
}
