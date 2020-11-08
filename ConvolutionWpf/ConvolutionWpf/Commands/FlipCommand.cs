using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Catel.MVVM;

namespace ConvolutionWpf.Commands
{
    public class FlipCommand : Command
    {
        private readonly Func<WriteableBitmap> _imageFactory;

        public event Action<WriteableBitmap> OnImageChanged;

        public FlipCommand(Func<WriteableBitmap> imageFactory)
            : base(() => { })
        {
            _imageFactory = imageFactory;
        }

        public void ExecuteCommand()
        {
            var image = _imageFactory();
            //var newImage = _imageFactory();
            if (image == null)
                return;

            //todo
            //public WriteableBitmap(int pixelWidth,int pixelHeight,double dpiX,double dpiY,PixelFormat pixelFormat,BitmapPalette palette)
            //инициализация нового WriteableBitmap

            var newImage = new WriteableBitmap(image.PixelWidth, image.PixelHeight, image.DpiX, image.DpiY, image.Format, image.Palette);
            //инициализация newImage с двойным размером по ширине image
            var pixels = new byte[image.PixelHeight * image.BackBufferStride];
            //инициализация массива pixels для считывания image
            image.CopyPixels(pixels, image.BackBufferStride, 0);
            //копируем массив точек в pixels
            var newPixels = new byte[image.PixelHeight * image.BackBufferStride];
            var pixelsNormal = new byte[image.BackBufferStride];
            var pixelsReverse = new byte[image.BackBufferStride];
            for (int i = 0; i < image.PixelHeight; i++)
            {
                
                for (int j = 0; j < image.PixelWidth; j++)
                {
                    int index = i * image.BackBufferStride + 4 * j;
                    for (int c = 0; c < 4; ++c)
                    {
                        pixelsReverse[image.BackBufferStride - 4 * (j+1) + c] = pixels[index + c];
                    }
                }
                pixelsReverse.CopyTo(newPixels,i*image.PixelHeight);
            }
            newImage.WritePixels(new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight), newPixels, image.BackBufferStride, 0);
            


            OnImageChanged?.Invoke(newImage); //отправляем на отрисовку картинку
        }

        protected override void Execute(object parameter, bool ignoreCanExecuteCheck)
        {
            ExecuteCommand();
        }
    }
}