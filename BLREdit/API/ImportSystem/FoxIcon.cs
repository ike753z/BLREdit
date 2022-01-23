﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BLREdit
{
    public class FoxIcon
    {
        private const int WideImageWidth = 256;
        private const int WideImageHeight = 128;

        private const int LargeSquareImageWidth = 128;

        private const int SmallSquareImageWidth = 64;

        public string Name { get; set; } = "";
        public Uri Icon { get; set; } = null;

        private static readonly BitmapImage WideEmpty = CreateEmptyBitmap(WideImageWidth, WideImageHeight);
        private static readonly BitmapImage LargeSquareEmpty = CreateEmptyBitmap(LargeSquareImageWidth, LargeSquareImageWidth);
        private static readonly BitmapImage SmallSquareEmpty = CreateEmptyBitmap(SmallSquareImageWidth, SmallSquareImageWidth);

        static FoxIcon()
        { 
            WideEmpty.Freeze();
            LargeSquareEmpty.Freeze();
            SmallSquareEmpty.Freeze();
        }

        public FoxIcon(string file)
        {
            string[] fileparts = file.Split('\\');
            string[] iconparts = fileparts[fileparts.Length - 1].Split('.');
            string iconname = "";
            if (iconparts.Length > 2)
            {
                for (int i = 0; i < iconparts.Length - 1; i++)
                {
                    if (i != 0)
                    {
                        iconname += ".";
                    }
                    iconname += iconparts[i];
                }
            }
            else
            {
                iconname = iconparts[0];
            }
            Name = iconname;
            Icon = new Uri(AppDomain.CurrentDomain.BaseDirectory + file, UriKind.Absolute);
        }

        public BitmapSource GetWideImage()
        {

            DrawingGroup group = new DrawingGroup();

            ImageDrawing baseImage = new ImageDrawing
            {
                Rect = new Rect(0, 0, WideImageWidth, WideImageHeight),
                ImageSource = WideEmpty.Clone()
            };
            group.Children.Add(baseImage);

            var tmp = GetImage();
            ImageDrawing actualImage = new ImageDrawing();
            int offsetX = (WideImageWidth - tmp.PixelWidth) / 2;
            if (offsetX < 0)
            { offsetX = 0; }

            actualImage.Rect = new Rect(offsetX, 0, tmp.PixelWidth, tmp.PixelHeight);
            actualImage.ImageSource = tmp;
            group.Children.Add(actualImage);

            var finished = new DrawingImage(group);
            return ToBitmapSource(finished);
        }

        public BitmapSource GetLargeSquareImage()
        {
            return GetSquareImage(LargeSquareImageWidth, LargeSquareEmpty.Clone());
        }

        public BitmapSource GetSmallSquareImage()
        {
            return GetSquareImage(SmallSquareImageWidth, SmallSquareEmpty.Clone());
        }

        public BitmapSource GetSquareImage(int square, BitmapImage empty)
        {

            DrawingGroup group = new DrawingGroup();
          
            ImageDrawing baseImage = new ImageDrawing
            {
                Rect = new Rect(0, 0, square, square),
                ImageSource = empty
            };
            group.Children.Add(baseImage);

            var tmp = GetImage();
            ImageDrawing actualImage = new ImageDrawing();
            int offsetY = (square - tmp.PixelWidth);
            if (offsetY < 0)
            {
                actualImage.Rect = new Rect(0, 0, square, tmp.PixelHeight / 2);
            }
            else
            {
                actualImage.Rect = new Rect(0, 0, square, square);
            }
            actualImage.ImageSource = tmp;
            group.Children.Add(actualImage);

            var finished = new DrawingImage(group);
            return ToBitmapSource(finished);
        }

        private BitmapSource GetImage()
        {
            return new BitmapImage(this.Icon);
        }

        public static BitmapSource ToBitmapSource(DrawingImage source)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawImage(source, new Rect(new Point(0, 0), new Size(source.Width, source.Height)));
            drawingContext.Close();

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)source.Width, (int)source.Height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);
            return bmp;
        }

        public static BitmapImage CreateEmptyBitmap(int width, int height)
        {
            // Define parameters used to create the BitmapSource.
            PixelFormat pf = PixelFormats.Bgra32;
          
            int rawStride = (width * pf.BitsPerPixel)/ 8;
            byte[] rawImage = new byte[rawStride * height];

            // Initialize the image with data.
            for (int i = 0; i < rawImage.Length; i+=4)
            {
                rawImage[i] = 0;    //Blue
                rawImage[i+1] = 0;  //Green
                rawImage[i+2] = 0;  //Red
                rawImage[i+3] = 30; //Alpha
            }

            // Create a BitmapSource.
            BitmapSource bitmap = BitmapSource.Create(width, height,
                96, 96, pf, null,
                rawImage, rawStride);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            MemoryStream stream = new MemoryStream();
            BitmapImage bitmapImage = new BitmapImage();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(stream);

            stream.Position = 0;
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(stream.ToArray());
            bitmapImage.EndInit();

            stream.Close();

            return bitmapImage;
        }

        public override string ToString()
        {
            return LoggingSystem.ObjectToTextWall(this);
        }
    }
}