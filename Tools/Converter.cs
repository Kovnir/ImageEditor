using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Numerics;

/// <summary>
/// Lightweight library for image manipulation.
/// Development is here:
/// https://github.com/Kovnir/ImageEditor
/// </summary>
namespace ImageEditor
{
    /// <summary>
    /// Класс Converter предназначен для преобразования комплексных и байтовых массивов и матриц
    /// и изображений друг в друга. 
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Переводит массив байт в массив комплексных чисел.
        /// </summary>
        /// <param name="b">Входной массив</param>
        /// <returns>Выходной массив</returns>
        public static Complex[] ToComplexArray(byte[] data)
        {
            Complex[] complex = new Complex[data.Length];
            for (int i = 0; i < data.Length; i++)
                complex[i] = data[i];
            return complex;
        }
        /// <summary>
        /// Переводит массив типа double в массив комплексных чисел.
        /// </summary>
        /// <param name="d">Входной массив</param>
        /// <returns>Выходной массив</returns>
        public static Complex[] ToComplexArray(double[] data)
        {
            Complex[] complex = new Complex[data.Length];
            for (int i = 0; i < data.Length; i++)
                complex[i] = data[i];
            return complex;
        }
        /// <summary>
        /// Переводит изображение в массив комплексных чисел.
        /// </summary>
        /// <param name="image">Входное изображение</param>
        /// <param name="blackAndWhite">true, если каждый пиксел кодируется одним байтов, false, если четырмя</param>
        /// <returns>Выходной массив</returns>
        public static Complex[] ToComplexArray(Image image, bool blackAndWhite = false)
        {
            return ToComplexArray(ToByteArray(image, blackAndWhite));
        }

        /// <summary>
        /// Переводит массив комплексных чисел в массив байт.
        /// </summary>
        /// <param name="data">Входной массив</param>
        /// <returns>Выходной массив</returns>
        public static byte[] ToByteArray(Complex[] data)
        {
            byte[] b = new byte[data.Length];
            int index = 0;

            foreach (Complex c in data)
            {
                if (c.Real > byte.MaxValue)
                    b[index] = byte.MaxValue;
                else if (c.Real < byte.MinValue)
                    b[index] = byte.MinValue;
                else
                    b[index] = (byte)Math.Round(c.Real);
                index++;
            }
            return b;
        }
        /// <summary>
        /// Переводит массив типа double в массив массив байт.
        /// </summary>
        /// <param name="data">Входной массив</param>
        /// <returns>Выходной массив</returns>
        public static byte[] ToByteArray(double[] data)
        {
            byte[] b = new byte[data.Length];
            int index = 0;

            foreach (double d in data)
            {
                if (d > byte.MaxValue)
                    b[index] = byte.MaxValue;
                else if (d < byte.MinValue)
                    b[index] = byte.MinValue;
                else
                    b[index] = (byte)Math.Round(d);
                index++;
            }
            return b;
        }
        /// <summary>
        /// Переводит изображение в массив байт.
        /// </summary>
        /// <param name="image">Входное изображение</param>
        /// <param name="blackAndWhite">true, если каждый пиксел кодируется одним байтов, false, если четырмя</param>
        /// <returns>Выходной массив</returns>
        public static byte[] ToByteArray(Image source, bool blackAndWhite = false)
        {
            if (source == null) return null;
            if (!blackAndWhite)
            {
                Bitmap bmp = new Bitmap(source);                                        //инициализируем битмап
                PixelFormat pxf = PixelFormat.Format32bppRgb;                           //задаём формат пиксел
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);            //получаем данные картинки
                BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);  //блокируем набор данных изображения в памяти
                IntPtr ptr = bmpData.Scan0;                                             //получаем адрес первой линии

                // Задаём массив из Byte и помещаем в него набор данных.
                // int numBytes = bmp.Width * bmp.Height * 4; 
                //На 3 умножаем - поскольку RGB цвет кодируется 4-мя байтами
                //Либо используем вместо Width - Stride
                int numBytes = bmpData.Stride * bmp.Height;
                byte[] rgbValues = new byte[numBytes];
                Marshal.Copy(ptr, rgbValues, 0, numBytes);                              //копируем значения в массив
                return rgbValues;
            }
            else
            {
                Bitmap bmp = new Bitmap(source.ToBlackAndWhite());                      //инициализируем битмап
                PixelFormat pxf = PixelFormat.Format32bppArgb;                          //задаём формат пиксел
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);            //получаем данные картинки
                BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);  //блокируем набор данных изображения в памяти
                IntPtr ptr = bmpData.Scan0;                                             //получаем адрес первой линии

                // Задаём массив из Byte и помещаем в него набор данных
                // int numBytes = bmp.Width * bmp.Height * 4; 
                //На 4 умножаем - поскольку RGB цвет кодируется 4-мя байтами
                //Либо используем вместо Width - Stride
                int numBytes = bmpData.Stride * bmp.Height;
              //  int widthBytes = bmpData.Stride;
                byte[] rgbValues = new byte[numBytes];

                Marshal.Copy(ptr, rgbValues, 0, numBytes);                              //копируем значения в массив.
                Byte[] array = new Byte[bmp.Width * bmp.Height];
                //перебираем пикселы по 4 байта на каждый и меняем значения
//                for (int i = 0; i < bmp.Height; i++)
  //                  for (int j = 0; j < bmp.Width; j++)
    //                0 4 8 
                for (int i = 0; i < numBytes; i+=4)
                    array[i / 4] = rgbValues[i];
      //              }
                bmp.UnlockBits(bmpData);                                                //разблокируем набор данных изображения в памяти.
                return array;
            }
        }

        /// <summary>
        /// Переводит массив байт в массив вещественных чисел двойной точности.
        /// </summary>
        /// <param name="data">Входной массив</param>
        /// <returns>Выходной массив</returns>
        public static double[] ToDoubleArray(byte[] data)
        {
            double[] d = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
                d[i] = data[i];
            return d;
        }
        /// <summary>
        /// Переводит массив комплексных чисел в массив вещественных чисел двойной точности.
        /// </summary>
        /// <param name="data">Входной массив</param>
        /// <returns>Выходной массив</returns>
        public static double[] ToDoubleArray(Complex[] data)
        {
            double[] d = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
                d[i] = data[i].Real;
            return d;
        }
        /// <summary>
        /// Переводит изображение в массив вещественныъ чисел двойной точности.
        /// </summary>
        /// <param name="image">Входное изображение</param>
        /// <param name="blackAndWhite">true, если каждый пиксел кодируется одним байтов, false, если четырмя</param>
        /// <returns>Выходной массив</returns>
        public static double[] ToDoubleArray(Image image, bool blackAndWhite = false)
        {
            return ToDoubleArray(ToByteArray(image, blackAndWhite));
        }

        /*-------------------------------------------------------------------------------*/

        /// <summary>
        /// Переводит матрицу комплексных чисел в матрицу байт
        /// </summary>
        /// <param name="data">Входная матрица</param>
        /// <returns>Результирующая матрица</returns>
        public static Complex[,] ToComplexMatrix(byte[,] data)
        {
            Complex[,] c = new Complex[data.GetLength(0), data.GetLength(1)];
            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                    c[i, j] = data[i, j];
            return c;
        }
        /// <summary>
        /// Переводит матрицу комплексных чисел в матрицу вещественных чисел двойной точности
        /// </summary>
        /// <param name="data">Входная матрица</param>
        /// <returns>Результирующая матрица</returns>
        public static Complex[,] ToComplexMatrix(double[,] data)
        {
            Complex[,] c = new Complex[data.GetLength(0), data.GetLength(1)];
            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                    c[i, j] = data[i, j];
            return c;
        }
        /// <summary>
        /// Представляет значение яркости в матрицу комплексных чисел
        /// </summary>
        /// <param name="source">входное изображение</param>
        /// <returns>выходной массив</returns>
        public static Complex[,] ToComplexMatrix(Image source)
        {
            if (source == null) return null;
            Bitmap bmp = new Bitmap(source.ToBlackAndWhite());                      //чёрнобелый вид исходного изображения
            PixelFormat pxf = PixelFormat.Format32bppArgb;                           //задаём формат пиксела

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);  //блокируем набор данных изображения в памяти

            IntPtr ptr = bmpData.Scan0;                                             //получаем адрес первой линии.

            // Задаём массив из Byte и помещаем в него набор данных.
            // int numBytes = bmp.Width * bmp.Height * 4; 
            //На 4 умножаем - поскольку RGB цвет кодируется 4-мя байтами
            //Либо используем вместо Width - Stride
            int numBytes = bmpData.Stride * bmp.Height;
            int widthBytes = bmpData.Stride;
            byte[] rgbValues = new byte[numBytes];
            Marshal.Copy(ptr, rgbValues, 0, numBytes);                              //копируем значения в массив.

            Complex[,] complex = new Complex[bmp.Height, bmp.Width];
            //перебираем пикселы по 4 байта на каждый и меняем значения
            for (int i = 0; i < bmp.Height; i++)
                for (int j = 0; j < bmp.Width; j++)
                {
                    int counter = (j + (bmp.Width * i)) * 4;
                    complex[i, j] = (Complex)rgbValues[counter];
                }
            bmp.UnlockBits(bmpData);                                                //разблокируем набор данных изображения в памяти.
            return complex;                                                         //возвращаем массив
        }

        /// <summary>
        /// Переводит матрицу комплексных чисел в матрицу байт.
        /// </summary>
        /// <param name="data">Входная матрица</param>
        /// <returns>Результирующая матрица</returns>
        public static byte[,] ToByteMatrix(Complex[,] data)
        {
            byte[,] b = new byte[data.GetLength(0), data.GetLength(1)];
            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                    b[i, j] = (byte)Math.Round(data[i, j].Real);
            return b;
        }
        /// <summary>
        /// Переводит матрицу double в матрицу байт.
        /// </summary>
        /// <param name="data">Входная матрица</param>
        /// <returns>Результирующая матрица</returns>
        public static byte[,] ToByteMatrix(double[,] data)
        {
            byte[,] b = new byte[data.GetLength(0), data.GetLength(1)];
            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                    b[i, j] = (byte)Math.Round(data[i, j]);
            return b;
        }
        /// <summary>
        /// Представляет значение яркости в матрицу байт
        /// </summary>
        /// <param name="source">входное изображение</param>
        /// <returns>выходной массив</returns>
        public static byte[,] ToByteMatrix(Image source)
        {
            if (source == null) return null;
            Bitmap bmp = new Bitmap(source.ToBlackAndWhite());                      //чёрнобелый вид исходного изображения
            PixelFormat pxf = PixelFormat.Format32bppArgb;                           //задаём формат пиксела

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);  //блокируем набор данных изображения в памяти

            IntPtr ptr = bmpData.Scan0;                                             //получаем адрес первой линии.

            // Задаём массив из Byte и помещаем в него набор данных.
            // int numBytes = bmp.Width * bmp.Height * 4; 
            //На 4 умножаем - поскольку RGB цвет кодируется 4-мя байтами
            //Либо используем вместо Width - Stride
            int numBytes = bmpData.Stride * bmp.Height;
            int widthBytes = bmpData.Stride;
            byte[] rgbValues = new byte[numBytes];
            Marshal.Copy(ptr, rgbValues, 0, numBytes);                              //копируем значения в массив.

            byte[,] byteArray = new byte[bmp.Height, bmp.Width];
            //перебираем пикселы по 4 байта на каждый и меняем значения
            for (int i = 0; i < bmp.Height; i++)
                for (int j = 0; j < bmp.Width; j++)
                {
                    int counter = (j + (bmp.Width * i)) * 4;
                    byteArray[i, j] = rgbValues[counter];
                }
            bmp.UnlockBits(bmpData);                                                //разблокируем набор данных изображения в памяти.
            return byteArray;                                                         //возвращаем массив
        }


        /// <summary>
        /// Переводит матрицу комплексных чисел в матрицу double.
        /// </summary>
        /// <param name="data">Входная матрица</param>
        /// <returns>Результирующая матрица</returns>
        public static double[,] ToDoubleMatrix(Complex[,] data)
        {
            double[,] d = new double[data.GetLength(0), data.GetLength(1)];
            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                    d[i, j] = data[i, j].Real;
            return d;
        }
        /// <summary>
        /// Переводит матрицу байт в матрицу double.
        /// </summary>
        /// <param name="data">Входная матрица</param>
        /// <returns>Результирующая матрица</returns>
        public static double[,] ToDoubleMatrix(byte[,] data)
        {
            double[,] d = new double[data.GetLength(0), data.GetLength(1)];
            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                    d[i, j] = data[i, j];
            return d;
        }
        /// <summary>
        /// Представляет значение яркости в матрицу вещественных чисел двойной точности
        /// </summary>
        /// <param name="source">входное изображение</param>
        /// <returns>выходной массив</returns>
        public static double[,] ToDoubleMatrix(Image source)
        {
            if (source == null) return null;
            Bitmap bmp = new Bitmap(source.ToBlackAndWhite());                      //чёрнобелый вид исходного изображения
            PixelFormat pxf = PixelFormat.Format32bppArgb;                           //задаём формат пиксела

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);  //блокируем набор данных изображения в памяти

            IntPtr ptr = bmpData.Scan0;                                             //получаем адрес первой линии.

            // Задаём массив из Byte и помещаем в него набор данных.
            // int numBytes = bmp.Width * bmp.Height * 4; 
            //На 4 умножаем - поскольку RGB цвет кодируется 4-мя байтами
            //Либо используем вместо Width - Stride
            int numBytes = bmpData.Stride * bmp.Height;
            int widthBytes = bmpData.Stride;
            byte[] rgbValues = new byte[numBytes];
            Marshal.Copy(ptr, rgbValues, 0, numBytes);                              //копируем значения в массив.

            double[,] byteArray = new double[bmp.Height, bmp.Width];
            //перебираем пикселы по 4 байта на каждый и меняем значения
            for (int i = 0; i < bmp.Height; i++)
                for (int j = 0; j < bmp.Width; j++)
                {
                    int counter = (j + (bmp.Width * i)) * 4;
                    byteArray[i, j] = (double)rgbValues[counter];
                }
            bmp.UnlockBits(bmpData);                                                //разблокируем набор данных изображения в памяти.
            return byteArray;                                                         //возвращаем массив
        }
        
        /*-------------------------------------------------------------------------------*/

        /// <summary>
        /// Загружает изображение из массива байт.
        /// </summary>
        /// <param name="data">входной массив</param>
        /// <param name="width">ширина изображения</param>
        /// <param name="blackAndWhite">true, если каждый пиксел кодируется одним байтов, false, если четырмя</param>
        /// <returns>полученное изображение</returns>
        public static Image ToImage(byte[] data, int width, bool blackAndWhite = false)
        {
            byte[] data1;
            if (blackAndWhite)
            {
                data1 = new byte[data.Length * 4];
                for (int i = 0; i < data.Length; i++)
                {
                    data1[i * 4] = data[i];
                    data1[i * 4 + 1] = data[i];
                    data1[i * 4 + 2] = data[i];
                    data1[i * 4 + 3] = 255;
                }
            }
            else
                data1 = data;

            PixelFormat pxf = PixelFormat.Format32bppArgb;                           //задаём формат пиксела
            Bitmap bmp = new Bitmap(width, data1.Length / 4 / width);                //создаём картинку
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);  //блокируем набор данных изображения в памяти
            IntPtr ptr = bmpData.Scan0;                                             //получаем адрес первой линии.
            Marshal.Copy(data1, 0, ptr, data1.Length);                                //копируем набор данных обратно в изображение
            bmp.UnlockBits(bmpData);                                                //разблокируем набор данных изображения в памяти.
            return bmp;                                                             //возвращаем загруженное изображение

        }
        /// <summary>
        /// Загружает изображение из массива вещественных чисел двойной точности.
        /// </summary>
        /// <param name="data">входной массив</param>
        /// <param name="width">ширина изображения</param>
        /// <param name="blackAndWhite">true, если каждый пиксел кодируется одним байтов, false, если четырмя</param>
        /// <returns>полученное изображение</returns>
        public static Image ToImage(double[] data, int width, bool blackAndWhite = false)
        {
            return ToImage(ToByteArray(data),width, blackAndWhite);
        }
        /// <summary>
        /// Загружает изображение из массива комплексных чисел.
        /// </summary>
        /// <param name="data">входной массив</param>
        /// <param name="width">ширина изображения</param>
        /// <param name="blackAndWhite">true, если каждый пиксел кодируется одним байтов, false, если четырмя</param>
        /// <returns>полученное изображение</returns>
        public static Image ToImage(Complex[] data, int width, bool blackAndWhite = false)
        {
            return ToImage(ToByteArray(data), width, blackAndWhite);
        }

        /// <summary>
        /// Загружает изображение из матрицы вещественных чисел двойной точности.
        /// </summary>
        /// <param name="data">входная матрица</param>
        /// <returns>полученное изображение</returns>
        public static Image ToImage(double[,] data)
        {
            byte[] data1;
            data1 = new byte[data.Length * 4];
            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    int index = (i * data.GetLength(1) + j) * 4;
                    data1[index] = (byte)Math.Round(data[i, j]);
                    data1[index+ 1] = (byte)Math.Round(data[i, j]);
                    data1[index+ 2] = (byte)Math.Round(data[i, j]);
                    data1[index+ 3] = 255;
                }

            PixelFormat pxf = PixelFormat.Format32bppArgb;                           //задаём формат пиксела
            Bitmap bmp = new Bitmap(data.GetLength(1), data.GetLength(0));                //создаём картинку
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);  //блокируем набор данных изображения в памяти
            IntPtr ptr = bmpData.Scan0;                                             //получаем адрес первой линии.
            Marshal.Copy(data1, 0, ptr, data1.Length);                                //копируем набор данных обратно в изображение
            bmp.UnlockBits(bmpData);                                                //разблокируем набор данных изображения в памяти.
            return bmp;                                                             //возвращаем загруженное изображение
        }
        /// <summary>
        /// Загружает изображение из матрицы комплексных чисел.
        /// </summary>
        /// <param name="data">входная матрица</param>
        /// <returns>полученное изображение</returns>
        public static Image ToImage(Complex[,] data)
        {
            byte[] data1;
            data1 = new byte[data.GetLength(0)*data.GetLength(1) * 4];
            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    int index = (i * data.GetLength(1) + j) * 4;
                    data1[index] = (byte)Math.Round(data[i, j].Real);
                    data1[index + 1] = (byte)Math.Round(data[i, j].Real);
                    data1[index + 2] = (byte)Math.Round(data[i, j].Real);
                    data1[index + 3] = 255;
                }

            PixelFormat pxf = PixelFormat.Format32bppArgb;                           //задаём формат пиксела
            Bitmap bmp = new Bitmap(data.GetLength(1), data.GetLength(0));                //создаём картинку
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);  //блокируем набор данных изображения в памяти
            IntPtr ptr = bmpData.Scan0;                                             //получаем адрес первой линии.
            Marshal.Copy(data1, 0, ptr, data1.Length);                                //копируем набор данных обратно в изображение
            bmp.UnlockBits(bmpData);                                                //разблокируем набор данных изображения в памяти.
            return bmp;                                                             //возвращаем загруженное изображение
        }
        /// <summary>
        /// Загружает изображение из матрицы байт.
        /// </summary>
        /// <param name="data">входная матрица</param>
        /// <returns>полученное изображение</returns>
        public static Image ToImage(byte[,] data)
        {
            byte[] data1;
            data1 = new byte[data.Length * 4];
            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    int index = (i * data.GetLength(1) + j) * 4;
                    data1[index] = data[i, j];
                    data1[index + 1] = data[i, j];
                    data1[index + 2] = data[i, j];
                    data1[index + 3] = 255;
                }

            PixelFormat pxf = PixelFormat.Format32bppRgb;                           //задаём формат пиксела
            Bitmap bmp = new Bitmap(data.GetLength(1), data.GetLength(0));                //создаём картинку
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);  //блокируем набор данных изображения в памяти
            IntPtr ptr = bmpData.Scan0;                                             //получаем адрес первой линии.
            Marshal.Copy(data1, 0, ptr, data1.Length);                                //копируем набор данных обратно в изображение
            bmp.UnlockBits(bmpData);                                                //разблокируем набор данных изображения в памяти.
            return bmp;                                                             //возвращаем загруженное изображение
        }
    }
}
