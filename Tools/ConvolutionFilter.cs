using System;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Drawing;
using ImageEditor.Exceptions;
using System.Numerics;


namespace ImageEditor
{
    /// <summary>
    /// Класс фильтра для свёртки
    /// </summary>
    public class ConvolutionFilter
    {
        public string filterName;       //имя фильтра
        public double factor;           //коэфициент для того, что сумма = 1 была необязательной
        public double bias;             //смещенре для некоторых фильтров
        private double[,] _filterMatrix;//сама матрица
        public double[,] filterMatrix   //её геттер/сеттер
        {
            get { return _filterMatrix; }
            set
            {
                if (value == null)
                {
                    _filterMatrix = null;
                    return;
                }
                if (value.GetLength(0) != value.GetLength(1))
                    throw new InvalidKernelException("Wrong Kernel Size");
                if (value.GetLength(0) %2 == 0)
                    throw new InvalidKernelException("Kernel Size is parity");
                _filterMatrix = value;
            }
        }

        public double[,] normalizedFilterMatrix   //нормализированное ядно
        {
            get
            {
                double[,] complex = new double[filterMatrix.GetLength(0), filterMatrix.GetLength(1)];
                for (int i = 0; i < filterMatrix.GetLength(0); i++)
                    for (int j = 0; j < filterMatrix.GetLength(1); j++)
                        complex[i, j] = filterMatrix[i, j] * factor;
                return complex;
            }
        }

        public Complex[,] complexMatrix //возвращает матрицу в комплекном виде
        {
            get
            {
                Complex[,] complex = new Complex[filterMatrix.GetLength(0), filterMatrix.GetLength(1)];
                for (int i = 0; i < filterMatrix.GetLength(0); i++)
                    for (int j = 0; j < filterMatrix.GetLength(1); j++)
                        complex[i, j] = new Complex(filterMatrix[i, j] * factor, 0);
                return complex;
            }
        }


        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="name">Имя</param>
        /// <param name="kernel">Маска</param>
        /// <param name="factor">Коэфициент умножения</param>
        /// <param name="bias">Сдвиг</param>
        public ConvolutionFilter(string name = "Custom Filter", double[,] kernel = null , double factor = 1, double bias = 0)
        {
            filterName = name;
            filterMatrix = kernel;
            this.factor = factor;
            this.bias = bias;
        }

        public enum ConvolutionMode {collapse, expand};


        /// <summary>
        /// Функция свёртки
        /// </summary>
        /// <param name="sourceImage">Исходное изображение</param>
        /// <param name="mode">Режим свёртки</param>
        /// <param name="channels">Каналы, над которыми проводится операция свёртки</param>
        /// <returns>
        /// Результирующее изображение
        /// </returns>
        public Image Convolution(Image sourceImage, 
            ConvolutionMode mode = ConvolutionMode.collapse, 
            int channels = (Channel.BLUE | Channel.RED | Channel.GREEN))
        {
            Bitmap source = new Bitmap(sourceImage);

            //размеры фильтра
            int filterWidth = filterMatrix.GetLength(1);
            int filterHeight = filterMatrix.GetLength(0);
            int filterOffset = (filterWidth - 1) / 2;           //смещение от центра

            
            if (mode == ConvolutionMode.expand)
            {
                source = (Bitmap) source.Expand(filterOffset);
            }

            //Для того, чтобы получить доступ к основные ARGB значения из Bitmap объекта мы сначала должны заблокировать Bitmap в памяти
            //Блокировка Bitmap в памяти предотвращает Garbage Collector от перемещения Bitmap объект на новое место в памяти.
            //При вызове Bitmap.LockBits метод исходный код создает экземпляр BitmapData объект из возвращаемого значения. 
            BitmapData sourceData = source.LockBits(new Rectangle(0, 0,
                                        source.Width, source.Height),
                                        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);


            //BitmapData.Stride свойство представляет ряд байтов в одном растрового подряд пикселей. 
            //В этом сценарии BitmapData.Stride должна быть равна ширине Bitmap в пикселях, умноженных на четыре,
            //так как каждый пиксель состоит из четырех байтов: Альфа, Красный, Зеленый и Синий.


            byte[] pixelBuffer = new byte[sourceData.Width * 4 * sourceData.Height];
            int resWidth = source.Width - filterOffset*2;
            int resHeight = source.Height - filterOffset*2;

            byte[] resultBuffer = new byte[resWidth * 4 * resHeight];
            
            //свойствл BitmapData.Scan0 BitmapData типа IntPtr представляет собой адрес памяти первого байта Bitmap 
            //Использовуя Marshal.Copy метод, мы укажем адрес памяти отправной точки, 
            //откуда начнём копирование байт Bitmap.

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            //Если растр был заблокирован в памяти обеспечим снятие блокировки с помощью вызова метода Bitmap.UnlockBits.
            source.UnlockBits(sourceData);

            double blue = 0.0;
            double green = 0.0;
            double red = 0.0;
            
            int calcOffset = 0;

            int byteOffset = 0;

            int byteOffset1 = 0;

            int width = source.Width;
            int height = source.Height;


                //проходим по массиву так, чтобы маска вписалась внутрь
                for (int offsetY = filterOffset; offsetY < height - filterOffset; offsetY++)
                {
                    for (int offsetX = filterOffset; offsetX < width - filterOffset; offsetX++)
                    {
                        blue = 0;
                        green = 0;
                        red = 0;

                        byteOffset = offsetY *
                                        width * 4 +
                                        offsetX * 4;
                        byteOffset1 = (offsetY - filterOffset) * (width-2*filterOffset) * 4 +
                (offsetX - filterOffset) * 4;

                        //Инициирование цикла с отрицательными значениями упрощает реализацию концепции соседних пикселей.
                        for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                        {
                            for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                            {

                                //вычисляет индекс соседнего пиксела в отношении текущего пикселя.
                                calcOffset = byteOffset +
                                             (filterX * 4) +
                                             (filterY * width * 4);

                                //матрица значение применяется в качестве фактора индивидуальных цветовых компонентов соответствующих соседним пикселям.
                                //Результаты будут добавлены в итоговые переменные голубых, зеленых и красных каналов.
                                blue += (double)(pixelBuffer[calcOffset]) *
                                         filterMatrix[filterY + filterOffset,
                                         filterX + filterOffset];


                                green += (double)(pixelBuffer[calcOffset + 1]) *
                                          filterMatrix[filterY + filterOffset,
                                          filterX + filterOffset];


                                red += (double)(pixelBuffer[calcOffset + 2]) *
                                        filterMatrix[filterY + filterOffset,
                                        filterX + filterOffset];
                            }
                        }

                        //применяем коэффициент и добавить смещения, заданного параметром фильтра.
                        blue = factor * blue + bias;
                        green = factor * green + bias;
                        red = factor * red + bias;

                        //Цветовые компоненты могут содержать только значение в диапазоне от 0 до 255 включительно.
                        //Прежде, чем мы присвоить рассчитанное значение цветового компонента, мы гарантируем, 
                        //что значение находится в пределах требуемого диапазона. 
                        //Значения, которые превышают 255 установим на 255, а менее 0 устанавливаются в 0. 
                        if (blue > 255)
                        { blue = 255; }
                        else if (blue < 0)
                        { blue = 0; }

                        if (green > 255)
                        { green = 255; }
                        else if (green < 0)
                        { green = 0; }

                        if (red > 255)
                        { red = 255; }
                        else if (red < 0)
                        { red = 0; }

                        if ((channels & Channel.BLUE) == Channel.BLUE)
                            resultBuffer[byteOffset1] = (byte)Math.Round(blue);
                        else
                            resultBuffer[byteOffset1] = pixelBuffer[byteOffset];
                        if ((channels & Channel.GREEN) == Channel.GREEN)
                            resultBuffer[byteOffset1 + 1] = (byte)Math.Round(green);
                        else
                            resultBuffer[byteOffset1 + 1] = pixelBuffer[byteOffset + 1];
                        if ((channels & Channel.RED) == Channel.RED)
                            resultBuffer[byteOffset1 + 2] = (byte)Math.Round(red);
                        else
                            resultBuffer[byteOffset1 + 2] = pixelBuffer[byteOffset + 2];
                        resultBuffer[byteOffset1 + 3] = 255;
                    }
                }

            //создание нового растрового экземпляр объекта и копирование результат расчета буфера
            Bitmap resultBitmap = new Bitmap(resWidth, resHeight);


            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                    resWidth, resHeight),
                                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }


        /// <summary>
        /// Конволюция (свёртка) в частотной области
        /// </summary>
        /// <param name="sourceImage">исходное изображение</param>
        /// <returns>искаженное изображение</returns>
        public Image FastConvolution(Image sourceImage)
        {
            int height = sourceImage.Height;
            int width = sourceImage.Width;
            int filterSize = this.filterMatrix.GetLength(0);       //размер PSF
            int filterHalfSize = (filterSize - 1) / 2 + 1;                //центр PSF
            Image expandedImage = sourceImage.Expand(filterHalfSize);
            int expHeight = expandedImage.Height;
            int expWidth = expandedImage.Width;
            double[] image = Converter.ToDoubleArray(expandedImage);
            double[,] red = new double[expHeight, expWidth];
            double[,] green = new double[expHeight, expWidth];
            double[,] blue = new double[expHeight, expWidth];
            int index = 0;
            for (int i = 0; i < expHeight; i++)
                for (int j = 0; j < expWidth; j++)
                {
                    blue[i, j] = image[index];
                    green[i, j] = image[index + 1];
                    red[i, j] = image[index + 2];
                    index += 4;
                }

            Complex[,] redFourier = Fourier.FastTransform(Converter.ToComplexMatrix(red));
            Complex[,] greenFourier = Fourier.FastTransform(Converter.ToComplexMatrix(green));
            Complex[,] blueFourier = Fourier.FastTransform(Converter.ToComplexMatrix(blue));
            int newSize = redFourier.GetLength(0);

            double[,] kernel = this.ExpendedByZero(newSize);
            Complex[,] kernelFourier = Fourier.FastTransform(Converter.ToComplexMatrix(kernel));
            for (int u = 0; u < newSize; u++)
                for (int v = 0; v < newSize; v++)
                {
                    redFourier[u, v] *= kernelFourier[u, v];
                    greenFourier[u, v] *= kernelFourier[u, v];
                    blueFourier[u, v] *= kernelFourier[u, v];
                }
            Complex[,] newRed = Fourier.IFastTransform(redFourier, expHeight, expWidth);
            Complex[,] newGreen = Fourier.IFastTransform(greenFourier, expHeight, expWidth);
            Complex[,] newBlue = Fourier.IFastTransform(blueFourier, expHeight, expWidth);

            Complex[,] resRed = new Complex[height, width];
            Complex[,] resGreen = new Complex[height, width];
            Complex[,] resBlue = new Complex[height, width];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    resRed[i, j] = Math.Round(newRed[i + filterHalfSize + 1, j + filterHalfSize + 1].Real);
                    resGreen[i, j] = Math.Round(newGreen[i + filterHalfSize + 1, j + filterHalfSize + 1].Real);
                    resBlue[i, j] = Math.Round(newBlue[i + filterHalfSize + 1, j + filterHalfSize + 1].Real);
                }
            Image result = Converter.ToImage(resRed, resGreen, resBlue);
            return result;
        }

        /// <summary>
        /// Расширение матрицы до квадратной нового размер, путём добавления нулей.
        /// </summary>
        /// <param name="newSize">новая размерность</param>
        /// <returns>Результирующее изображение</returns>
        public double[,] ExpendedByZero(int newSize)
        {
            double[,] data = this.normalizedFilterMatrix;
            double[,] newData = new double[newSize, newSize];
            for (int i = 0; i < newSize; i++)
                for (int j = 0; j < newSize; j++)
                    newData[i, j] = 0;

            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                    newData[i, j] = data[i, j];
            return newData;
        }

    }

    public static class Channel
    {
        public const byte RED = 4;//Convert.ToInt32("100", 2);
        public const byte GREEN = 2; //Convert.ToInt32("010", 2);
        public const byte BLUE = 1;//Convert.ToInt32("001", 2);
    }

    public static partial class ImageExtentions
    {
        /// <summary>
        /// Функция свёртки
        /// </summary>
        /// <param name="source">Исходное изображение</param>
        /// <param name="filter">Фильтр</param>
        /// <param name="mode">Режим свёртки</param>
        /// <param name="channels">Каналы, над которыми проводится операция свёртки</param>
        /// <returns>
        /// Результирующее изображение
        /// </returns>
        public static Image Convolution(this Image source, ConvolutionFilter filter,
            ConvolutionFilter.ConvolutionMode mode = ConvolutionFilter.ConvolutionMode.collapse, 
            int channels = (Channel.BLUE | Channel.RED | Channel.GREEN))
        {
            return filter.Convolution(new Bitmap(source), mode, channels);
        }

        /// <summary>
        /// Конволюция (свёртка) в частотной области
        /// </summary>
        /// <param name="source">Исходное изображение.</param>
        /// <param name="filter">Фильтр.</param>
        /// <returns></returns>
        public static Image FastConvolution(this Image source, ConvolutionFilter filter)
        {
            return filter.FastConvolution(source);
        }
    }

}
