using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BLREdit.API.Utils
{
    public static class ImgCache
    {
        private static bool AddFolderLine = AppDomain.CurrentDomain.BaseDirectory.EndsWith("\\");

        private const int WideImageWidth = 256;
        private const int WideImageHeight = 128;
        private const int LargeSquareImageWidth = 128;
        private const int SmallSquareImageWidth = 64;


        public static readonly BitmapImage WideEmpty = CreateEmptyBitmap(WideImageWidth, WideImageHeight);
        private static readonly BitmapImage Preview = CreateEmptyBitmap(1280, 720);
        private static readonly BitmapImage LargeSquareEmpty = CreateEmptyBitmap(LargeSquareImageWidth, LargeSquareImageWidth);
        private static readonly BitmapImage SmallSquareEmpty = CreateEmptyBitmap(SmallSquareImageWidth, SmallSquareImageWidth);

        static ImgCache()
        {
            WideEmpty.Freeze();
            LargeSquareEmpty.Freeze();
            SmallSquareEmpty.Freeze();
            Preview.Freeze();
        }

        private static string GetFemaleIconName(ImportItem item)
        {
            string[] parts = item.icon.Split('_');
            string female = "";
            for (int i = 0; i < parts.Length; i++)
            {
                if (i == parts.Length - 1)
                {
                    female += "_Female";
                }
                if (i == 0)
                {
                    female += parts[i];
                }
                else
                {
                    female += "_" + parts[i];
                }
            }
            return female;
        }

        public static void CreateImageChacheForItem(ImportItem item)
        {
            Uri Icon;
            Uri Female;
            Uri Scope;

            if (!AddFolderLine)
            {
                Icon = new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\" + "Assets\\textures\\" + item.icon + ".png", UriKind.Absolute);
                Female = new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\" + "Assets\\textures\\" + GetFemaleIconName(item) + ".png", UriKind.Absolute);
                Scope = new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\" + "Assets\\crosshairs\\" + item.name + ".png", UriKind.Absolute);
            }
            else
            {
                Icon = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Assets\\textures\\" + item.icon + ".png", UriKind.Absolute);
                Female = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Assets\\textures\\" + GetFemaleIconName(item) + ".png", UriKind.Absolute);
                Scope = new Uri(AppDomain.CurrentDomain.BaseDirectory + "Assets\\crosshairs\\" + item.name + ".png", UriKind.Absolute);
            }

            if (!File.Exists(Icon.LocalPath)) { return; }

            CreateCacheImage(Icon, item.WideImage, WideEmpty.Clone());
            CreateCacheImage(Icon, item.SmallSquareImage, SmallSquareEmpty.Clone());
            //CreateCacheImage(Icon, item.LargeSquareImage, LargeSquareEmpty.Clone());

            if (item.Category == "upperBody" || item.Category == "lowerBody")
            {
                CreateCacheImage(Female, item.FemaleWide, WideEmpty.Clone());
                CreateCacheImage(Female, item.FemaleSmall, SmallSquareEmpty.Clone());
            }

            if (item.Category == "scope")
            {
                CreateCacheImage(Scope, item.Scope, Preview.Clone());
                CreateCacheImage(Scope, item.MiniScope, SmallSquareEmpty.Clone());
            }
        }

        private static void CreateCacheImage(Uri source, Uri target, BitmapImage background)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(CombineImage(source, background)));

            using (var fileStream = new FileStream(target.LocalPath, FileMode.Create))
            {
                    encoder.Save(fileStream);
                    fileStream.Close();
            }
        }


        private static BitmapSource CombineImage(Uri source, BitmapImage empty, bool Uniform = true)
        {
            DrawingGroup group = new DrawingGroup();


            ImageDrawing baseImage = new ImageDrawing //background for Image
            {
                Rect = new Rect(0, 0, empty.Width, empty.Height),
                ImageSource = empty
            };
            group.Children.Add(baseImage);

            var tmp = new BitmapImage(source); //Load the actual image we want to draw

            ImageDrawing actualImage = new ImageDrawing
            {
                Rect = CreateRectForImage(empty.Width, empty.Height, tmp.PixelWidth, tmp.PixelHeight, Uniform),
                ImageSource = tmp
            };
            group.Children.Add(actualImage);

            var finished = new DrawingImage(group);
            return ToBitmapSource(finished);
        }

        public static Rect CreateRectForImage(double BaseWidth, double BaseHeight, double InsertWidth, double InsertHeight, bool Uniform)
        {
            double offsetX = (BaseWidth - InsertWidth);
            double offsetY = (BaseHeight - InsertHeight);

            double scaleX = BaseWidth / InsertWidth;
            double scaleY = BaseHeight / InsertHeight;

            if (Uniform)
            {
                if (scaleX <= scaleY)
                {
                    double finalOffset = 0;
                    if (offsetX != offsetY)
                    {
                        double scaledOffset = BaseHeight - (InsertHeight * scaleX);
                        finalOffset = scaledOffset - (scaledOffset / 2.0);
                    }
                    return new Rect(0, finalOffset, InsertWidth * scaleX, InsertHeight * scaleX);
                }
                else
                {
                    return new Rect((offsetX * scaleY) / 2.0, 0, InsertWidth * scaleY, InsertHeight * scaleY);
                }
            }
            else
            {
                return new Rect(0, 0, InsertWidth * scaleX, InsertHeight * scaleY);
            }
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

            int rawStride = (width * pf.BitsPerPixel) / 8;
            byte[] rawImage = new byte[rawStride * height];

            // Initialize the image with data.
            for (int i = 0; i < rawImage.Length; i += 4)
            {
                rawImage[i] = 0;    //Blue
                rawImage[i + 1] = 0;  //Green
                rawImage[i + 2] = 0;  //Red
                rawImage[i + 3] = 0; //Alpha
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
    }
}
