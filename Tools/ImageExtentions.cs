using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Numerics;


namespace ImageEditor
{
    public static partial class ImageExtentions
    {

        /* -------------------------------- Изменение изображения ---------------------------------- */

        /// <summary>
        /// Масштабирует изображение
        /// </summary>
        /// <param name="source">вызывается исходным изображением</param>
        /// <param name="width">ширина нового изображения</param>
        /// <param name="height">высота нового изображения</param>
        /// <returns>отмасштабированные изображение</returns>
        public static Image Scale(this Image source, int width, int height)
        {
            if (source == null) return null;
            Image dest = new Bitmap(width, height);                                 //инициализируем возвращаемый елемент
            Graphics gr = Graphics.FromImage(dest);                                 //инициализируем холст
            gr.FillRectangle(Brushes.White, 0, 0, width, height);                   //очищаем экран
            gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            float srcwidth = source.Width;                                          //начальные размеры
            float srcheight = source.Height;
            float dstwidth = width;                                                 //целевые размеры
            float dstheight = height;

            if (srcwidth <= dstwidth && srcheight <= dstheight)                     //исходное изображение меньше целевого
            {
                int left = (width - source.Width) / 2;
                int top = (height - source.Height) / 2;
                gr.DrawImage(source, left, top, source.Width, source.Height);
            }
            else if (srcwidth / srcheight > dstwidth / dstheight)                   //пропорции исходного изображения более широкие
            {
                float cy = srcheight / srcwidth * dstwidth;
                float top = ((float)dstheight - cy) / 2.0f;
                if (top < 1.0f) top = 0;
                gr.DrawImage(source, 0, top, dstwidth, cy);
            }
            else                                                                    //пропорции исходного изображения более узкие
            {
                float cx = srcwidth / srcheight * dstheight;
                float left = ((float)dstwidth - cx) / 2.0f;
                if (left < 1.0f) left = 0;
                gr.DrawImage(source, left, 0, cx, dstheight);
            }
            return dest;                                                            //вернём результирующее изображение
        }

        /// <summary>
        /// Добавляет аддитивный шум к изображению, одинаковый ко всем каналам
        /// </summary>
        /// <param name="source">исходное изображение</param>
        /// <param name="factor">коэфициент шума: 0 - нет шума, 1 - только шум</param>
        /// <returns>изображение с шумом</returns>
        public static Image SingleNoize(this Image source, double factor)
        {
            byte[,] noize = null;
            return SingleNoize(source, factor, ref noize);
        }

        /// <summary>
        /// Добавляет аддитивный шум к изображению, одинаковый ко всем каналам
        /// </summary>
        /// <param name="source">исходное изображение</param>
        /// <param name="factor">коэфициент шума: 0 - нет шума, 1 - только шум</param>
        /// <returns>изображение с шумом</returns>
        public static Image SingleNoize(this Image source, double factor, ref byte[,] noize)
        {
            if (source == null) return null;

            Bitmap bmp = new Bitmap(source);
            PixelFormat pxf = PixelFormat.Format24bppRgb;                           //задаём формат пиксела
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);            //получаем данные картинки
            //блокируем набор данных изображения в памяти
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);
            IntPtr ptr = bmpData.Scan0;                                             //получаем адрес первой линии

            // Задаём массив из Byte и помещаем в него набор данных.
            // int numBytes = bmp.Width * bmp.Height * 3; 
            //На 3 умножаем - поскольку RGB цвет кодируется 3-мя байтами
            //Либо используем вместо Width - Stride
            int numBytes = bmpData.Stride * bmp.Height;
            int widthBytes = bmpData.Stride;
            byte[] rgbValues = new byte[numBytes];
            Marshal.Copy(ptr, rgbValues, 0, numBytes);                              //копируем значения в массив
            Random random = new Random();
            double oneMinusFactor = 1 - factor;

            //перебираем пикселы по 3 байта на каждый и меняем значения
            for (int i = 0; i < bmp.Height; i++)
                for (int j = 0; j < bmp.Width; j++)
                {
                    byte bNoize = (byte)Math.Round((random.NextDouble() * 255 * factor));
                    byte gNoize = (byte)Math.Round((random.NextDouble() * 255 * factor));
                    byte rNoize = (byte)Math.Round((random.NextDouble() * 255 * factor));
                    if (noize != null)
                        noize[i, j] = rNoize;

                    int counter = (j + (bmp.Width * i)) * 3;
                    rgbValues[counter] = (byte)Math.Round((rgbValues[counter] * oneMinusFactor + bNoize));
                    rgbValues[counter + 1] = (byte)Math.Round((rgbValues[counter + 1] * oneMinusFactor + gNoize));
                    rgbValues[counter + 2] = (byte)Math.Round((rgbValues[counter + 2] * oneMinusFactor + rNoize));
                }
            Marshal.Copy(rgbValues, 0, ptr, numBytes);                              //копируем набор данных обратно в изображение
            bmp.UnlockBits(bmpData);                                                //разблокируем набор данных изображения в памяти
            return bmp;
        }
        /// <summary>
        /// Добавляет аддитивный шум к изображению
        /// </summary>
        /// <param name="source">исходное изображение</param>
        /// <param name="factor">коэфициент шума: 0 - нет шума, 1 - только шум</param>
        /// <returns>изображение с шумом</returns>
        public static Image Noize(this Image source, double factor)
        {
            byte[,] r = null;
            byte[,] g = null;
            byte[,] b = null;
            return Noize(source, factor, ref r, ref g, ref b);
        }

        /// <summary>
        /// Добавляет аддитивный шум к изображению
        /// </summary>
        /// <param name="source">исходное изображение</param>
        /// <param name="factor">коэфициент шума: 0 - нет шума, 1 - только шум</param>
        /// <returns>изображение с шумом</returns>
        public static Image Noize(this Image source, double factor, ref byte[,] prNoize, ref byte[,] pgNoize, ref byte[,] pbNoize)
        {
            if (source == null) return null;

            Bitmap bmp = new Bitmap(source);
            PixelFormat pxf = PixelFormat.Format24bppRgb;                           //задаём формат пиксела
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);            //получаем данные картинки
            //блокируем набор данных изображения в памяти
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);
            IntPtr ptr = bmpData.Scan0;                                             //получаем адрес первой линии

            // Задаём массив из Byte и помещаем в него набор данных.
            // int numBytes = bmp.Width * bmp.Height * 3; 
            //На 3 умножаем - поскольку RGB цвет кодируется 3-мя байтами
            //Либо используем вместо Width - Stride
            int numBytes = bmpData.Stride * bmp.Height;
            int widthBytes = bmpData.Stride;
            byte[] rgbValues = new byte[numBytes];
            Marshal.Copy(ptr, rgbValues, 0, numBytes);                              //копируем значения в массив
            Random random = new Random();
            double oneMinusFactor = 1 - factor;

            //перебираем пикселы по 3 байта на каждый и меняем значения
            for (int i = 0; i < bmp.Height; i++)
                for (int j = 0; j < bmp.Width; j++)
                {
                    byte bNoize = (byte)Math.Round((random.NextDouble() * 255 * factor));
                    byte gNoize = (byte)Math.Round((random.NextDouble() * 255 * factor));
                    byte rNoize = (byte)Math.Round((random.NextDouble() * 255 * factor));
                    if (prNoize != null)
                        prNoize[i, j] = rNoize;
                    if (pgNoize != null)
                        pgNoize[i, j] = gNoize;
                    if (pbNoize != null)
                        pbNoize[i, j] = bNoize;

                    int counter = (j + (bmp.Width * i)) * 3;
                    rgbValues[counter] = (byte)Math.Round(rgbValues[counter] * oneMinusFactor + bNoize);
                    rgbValues[counter + 1] = (byte)Math.Round((rgbValues[counter + 1] * oneMinusFactor + gNoize));
                    rgbValues[counter + 2] = (byte)Math.Round((rgbValues[counter + 2] * oneMinusFactor + rNoize));
                }
            Marshal.Copy(rgbValues, 0, ptr, numBytes);                              //копируем набор данных обратно в изображение
            bmp.UnlockBits(bmpData);                                                //разблокируем набор данных изображения в памяти
            return bmp;
        }

        /* -------------------------------- Работа с каналами ---------------------------------- */
        
        /// <summary>
        /// Переводит изображение в чёрнобелый вид
        /// </summary>
        /// <param name="source">Исходное изображение</param>
        /// <returns>результирующее изображение</returns>
        public static Image ToBlackAndWhite(this Image source)
        {
            if (source == null) return null;

            //коэфициенты влияния канала на результирующее чёнобелое изображение
            float RED_FACTOR = 0.11403f;
            float GREEN_FACTOR = 0.58703f;
            float BLUE_FACTOR = 0.29893f;

            Bitmap bmp = new Bitmap(source);                                        //инициализируем выходное узображение
            PixelFormat pxf = PixelFormat.Format32bppArgb;                           //задаём формат Пикселя.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);            //получаем данные картинки.
            //Блокируем набор данных изображения в памяти
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);
            IntPtr ptr = bmpData.Scan0;                                             //получаем адрес первой линии.

            // Задаём массив из Byte и помещаем в него набор данных.
            // int numBytes = bmp.Width * bmp.Height * 4; 
            //На 4 умножаем - поскольку RGB цвет кодируется 4-мя байтами
            //Либо используем вместо Width - Stride
            int numBytes = bmpData.Stride * bmp.Height;
            int widthBytes = bmpData.Stride;
            byte[] rgbValues = new byte[numBytes];

            Marshal.Copy(ptr, rgbValues, 0, numBytes);                              //копируем значения в массив.

            // Перебираем пикселы по 3 байта на каждый и меняем значения
            for (int i = 0; i < bmp.Height; i++)
                for (int j = 0; j < bmp.Width; j++)
                {
                    int counter = (j + (bmp.Width * i)) * 4;                        //вычисляем индекс
                    int value = (int)(BLUE_FACTOR * rgbValues[counter + 2] + GREEN_FACTOR * rgbValues[counter + 1] + RED_FACTOR * rgbValues[counter]);
                    byte color_b = Convert.ToByte(value);                           //переводим новое значение в байт

                    rgbValues[counter] = color_b;                                   //записываем его во все каналы
                    rgbValues[counter + 1] = color_b;
                    rgbValues[counter + 2] = color_b;
                }
            Marshal.Copy(rgbValues, 0, ptr, numBytes);                              //копируем набор данных обратно в изображение
            bmp.UnlockBits(bmpData);                                                //разблокируем набор данных изображения в памяти.
            return bmp;                                                             //возвращаем изображение
        }

        /// <summary>
        /// Заменяет значеия всех каналов на значение крассного канала.
        /// </summary>
        /// <param name="source">Исходное изображение</param>
        /// <returns>результирующее изображение</returns>
        public static Image OnlyRed(this Image source)
        {
            if (source == null) return null;

            Bitmap bmp = new Bitmap(source);                                        //инициализируем выходное узображение
            PixelFormat pxf = PixelFormat.Format32bppArgb;                           //задаём формат Пикселя.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);            //получаем данные картинки.
            //Блокируем набор данных изображения в памяти
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);
            IntPtr ptr = bmpData.Scan0;                                             //получаем адрес первой линии.

            // Задаём массив из Byte и помещаем в него набор данных.
            // int numBytes = bmp.Width * bmp.Height * 3; 
            //На 3 умножаем - поскольку RGB цвет кодируется 3-мя байтами
            //Либо используем вместо Width - Stride
            int numBytes = bmpData.Stride * bmp.Height;
            int widthBytes = bmpData.Stride;
            byte[] rgbValues = new byte[numBytes];

            Marshal.Copy(ptr, rgbValues, 0, numBytes);                              //копируем значения в массив.

            // Перебираем пикселы по 3 байта на каждый и меняем значения
            for (int i = 0; i < bmp.Height; i++)
                for (int j = 0; j < bmp.Width; j++)
                {
                    int counter = (j + (bmp.Width * i)) * 4;

                    rgbValues[counter] = rgbValues[counter + 2];
                    rgbValues[counter + 1] = rgbValues[counter + 2];
                    rgbValues[counter + 2] = rgbValues[counter + 2];

                }
            Marshal.Copy(rgbValues, 0, ptr, numBytes);                              //копируем набор данных обратно в изображение
            bmp.UnlockBits(bmpData);                                                //разблокируем набор данных изображения в памяти.
            return bmp;                                                             //возвращаем изображение
        }

        /// <summary>
        /// Заменяет значеия всех каналов на значение зелёного канала.
        /// </summary>
        /// <param name="source">Исходное изображение</param>
        /// <returns>результирующее изображение</returns>
        public static Image OnlyGreen(this Image source)
        {
            if (source == null) return null;

            Bitmap bmp = new Bitmap(source);                                        //инициализируем выходное узображение
            PixelFormat pxf = PixelFormat.Format32bppArgb;                           //задаём формат Пикселя.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);            //получаем данные картинки.
            //Блокируем набор данных изображения в памяти
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);
            IntPtr ptr = bmpData.Scan0;                                             //получаем адрес первой линии.

            // Задаём массив из Byte и помещаем в него набор данных.
            // int numBytes = bmp.Width * bmp.Height * 3; 
            //На 3 умножаем - поскольку RGB цвет кодируется 3-мя байтами
            //Либо используем вместо Width - Stride
            int numBytes = bmpData.Stride * bmp.Height;
            int widthBytes = bmpData.Stride;
            byte[] rgbValues = new byte[numBytes];

            Marshal.Copy(ptr, rgbValues, 0, numBytes);                              //копируем значения в массив.

            // Перебираем пикселы по 3 байта на каждый и меняем значения
            for (int i = 0; i < bmp.Height; i++)
                for (int j = 0; j < bmp.Width; j++)
                {
                    int counter = (j + (bmp.Width * i)) * 4;

                    rgbValues[counter] = rgbValues[counter + 1];
                    rgbValues[counter + 1] = rgbValues[counter + 1];
                    rgbValues[counter + 2] = rgbValues[counter + 1];

                }
            Marshal.Copy(rgbValues, 0, ptr, numBytes);                              //копируем набор данных обратно в изображение
            bmp.UnlockBits(bmpData);                                                //разблокируем набор данных изображения в памяти.
            return bmp;                                                             //возвращаем изображение
        }

        /// <summary>
        /// Заменяет значеия всех каналов на значение синего канала.
        /// </summary>
        /// <param name="source">Исходное изображение</param>
        /// <returns>результирующее изображение</returns>
        public static Image OnlyBlue(this Image source)
        {
            if (source == null) return null;

            Bitmap bmp = new Bitmap(source);                                        //инициализируем выходное узображение
            PixelFormat pxf = PixelFormat.Format32bppArgb;                           //задаём формат Пикселя.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);            //получаем данные картинки.
            //Блокируем набор данных изображения в памяти
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);
            IntPtr ptr = bmpData.Scan0;                                             //получаем адрес первой линии.

            // Задаём массив из Byte и помещаем в него набор данных.
            // int numBytes = bmp.Width * bmp.Height * 3; 
            //На 3 умножаем - поскольку RGB цвет кодируется 3-мя байтами
            //Либо используем вместо Width - Stride
            int numBytes = bmpData.Stride * bmp.Height;
            int widthBytes = bmpData.Stride;
            byte[] rgbValues = new byte[numBytes];

            Marshal.Copy(ptr, rgbValues, 0, numBytes);                              //копируем значения в массив.

            // Перебираем пикселы по 3 байта на каждый и меняем значения
            for (int i = 0; i < bmp.Height; i++)
                for (int j = 0; j < bmp.Width; j++)
                {
                    int counter = (j + (bmp.Width * i)) * 4;

                    rgbValues[counter] = rgbValues[counter];
                    rgbValues[counter + 1] = rgbValues[counter];
                    rgbValues[counter + 2] = rgbValues[counter];

                }
            Marshal.Copy(rgbValues, 0, ptr, numBytes);                              //копируем набор данных обратно в изображение
            bmp.UnlockBits(bmpData);                                                //разблокируем набор данных изображения в памяти.
            return bmp;                                                             //возвращаем изображение
        }

        /// <summary>
        /// Получает спектр изображения в виде изображения.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static Image GetSpectrum(this Image source)
        {
            Complex[,] fBlue = Fourier.Transform(Converter.ToComplexMatrix(source.OnlyBlue()));
            Complex[,] fGreen = Fourier.Transform(Converter.ToComplexMatrix(source.OnlyGreen()));
            Complex[,] fRed = Fourier.Transform(Converter.ToComplexMatrix(source.OnlyRed()));
            return Converter.ToImage(fRed, fGreen, fBlue);
        }

        /// <summary>
        /// Зеркально расширяет изображение на несколько пикселей с каждой стороны
        /// </summary>
        /// <param name="source">Исходное изображение.</param>
        /// <param name="offset">на сколько пикселей расширяем.</param>
        /// <returns></returns>
        public static Image Expand(this Image source, int offset)
        {
            if (source == null) return null;

            Bitmap bmp = new Bitmap(source);                                        //инициализируем выходное узображение
            PixelFormat pxf = PixelFormat.Format32bppArgb;                          //задаём формат Пикселя.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);            //получаем данные картинки.
            //Блокируем набор данных изображения в памяти
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);
            IntPtr ptr = bmpData.Scan0;                                             //получаем адрес первой линии.

            // Задаём массив из Byte и помещаем в него набор данных.
            // int numBytes = bmp.Width * bmp.Height * 3; 
            //На 3 умножаем - поскольку RGB цвет кодируется 3-мя байтами
            //Либо используем вместо Width - Stride
            int numBytes = bmpData.Stride * bmp.Height;
            byte[] startValues = new byte[numBytes];
            Marshal.Copy(ptr, startValues, 0, numBytes);                              //копируем значения в массив.


            int startWidth = bmp.Width;
            int startHeight = bmp.Height;

            int width = startWidth + offset * 2;
            int height = startHeight + offset * 2;

            byte[] finishValues = new byte[width * height*4];

            //копируем середину
            for (int i = 0; i < startHeight; i++)
                for (int j = 0; j < startWidth; j++)
                {
                    int index1 = ((i * startWidth)  + j) * 4;
                    int index2 = (((i+offset) * width) + j+ offset) * 4;
                    finishValues[index2] = startValues[index1];
                    finishValues[index2 + 1] = startValues[index1 + 1];
                    finishValues[index2 + 2] = startValues[index1 + 2];
                    finishValues[index2 + 3] = startValues[index1 + 3];
                }

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    int index1 = ((i * width) + j) * 4;
                    int index2 = 0;
                    if (i >= offset && i < height - offset)
                    {
                        if (j < offset)
                            index2 = ((offset + (offset - j)) + i * width) * 4;                             //слева
                        if (j >= width - offset)
                            index2 = (((width - offset) - (j - (width - offset))) -2 + i * width) * 4;      //справа
                    }
                    if (j >= offset && j < width - offset)
                    {
                        if (i < offset)
                            index2 = (j + (offset + (offset - i)) * width) * 4;                             //сверху
                        if (i >= height - offset)
                           index2 = ((((height - offset) - (i - (height - offset))) - 2)* width + j) * 4;   //снизу
                    }
                    if (i < offset && j < offset)                                               //левый верхний
                        index2 = (((offset-i)+offset) * width + (offset-j)+offset) * 4;
                    if (i < offset && j >= width - offset)                                      //правый верхний
                        index2 = (((offset-i) + offset) * width + ((width - offset) - (j - (width - offset))) - 2) * 4;
                    if (i >= height - offset && j >= width - offset)                            //правый нижний
                        index2 = (((height - offset) - (i - (height - offset))-2) * width + ((width - offset) - (j - (width - offset))) - 2) * 4;
                    if (i >= height - offset && j < offset)                                     //левый нижний
                        index2 = (((height - offset) - (i - (height - offset))-2) * width + ((offset - j) + offset)) * 4;

                    if (index2 == 0)
                        continue;
                    finishValues[index1] = finishValues[index2];
                    finishValues[index1 + 1] = finishValues[index2 + 1];
                    finishValues[index1 + 2] = finishValues[index2 + 2];
                    finishValues[index1 + 3] = 255;
                }
           

            Bitmap resBmp = new Bitmap(width,height);                                        //инициализируем выходное узображение
            rect = new Rectangle(0, 0, width, height);            //получаем данные картинки.
            //Блокируем набор данных изображения в памяти
            bmpData = resBmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);
            ptr = bmpData.Scan0;                                             //получаем адрес первой линии.

            Marshal.Copy(finishValues, 0, ptr, width*height*4);                              //копируем набор данных обратно в изображение
            resBmp.UnlockBits(bmpData);                                                //разблокируем набор данных изображения в памяти.
            return resBmp;                                                             //возвращаем изображение
 //           */
//            return Converter.ToImage(finishValues, width);
        }



    }
}
