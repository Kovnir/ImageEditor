using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace ImageEditor
{
    public static class Fourier
    {

        /// <summary>
        /// Линейное прямое преобразование фурье
        /// </summary>
        /// <param name="source">входной массив</param>
        /// <returns>Выходной ряд</returns>
        public static Complex[] Transform(Complex[] source)
        {
            int N = source.Length;
            Complex[] dist = new Complex[N];

            for (int k = 0; k < N; k++)
            {
                if (k % 500 == 0)
                    Console.WriteLine(k);

                Complex value = 0;
                for (int n = 0; n < N; n++)
                {
                    double phi = -2 * Math.PI * k * n / N;

                    double Real = source[n].Real * Math.Cos(phi) - source[n].Imaginary * Math.Sin(phi);
                    double Imaginary = source[n].Real * Math.Sin(phi) + source[n].Imaginary * Math.Cos(phi);

                    value += new Complex(Real, Imaginary);
                }
                dist[k] = value;
            }
            return dist;
        }

        /// <summary>
        /// Линейное обратное преобразование фурье
        /// </summary>
        /// <param name="source">вВходной ряж</param>
        /// <returns>Выходной массив</returns>
        public static Complex[] ITransform(Complex[] source)
        {
            int N = source.Length;
            Complex[] dist = new Complex[N];
            for (int n = 0; n < N; n++)
            {
                Complex value = 0;
                for (int k = 0; k < N; k++)
                {
                    double phi = 2 * Math.PI * k * n / N;

                    double Real = source[k].Real * Math.Cos(phi) - source[k].Imaginary * Math.Sin(phi);
                    double Imaginary = source[k].Real * Math.Sin(phi) + source[k].Imaginary * Math.Cos(phi);

                    value += new Complex(Real, Imaginary);
                }
                dist[n] = value / N;
            }

            return dist;
        }

        /// <summary>
        /// Двумерное прямое преобразование фурье
        /// </summary>
        /// <param name="source">входная матрица</param>
        /// <returns>Выходной ряд</returns>
        public static Complex[,] Transform(Complex[,] source)
        {
            int M = source.GetLength(0);    //строк
            int N = source.GetLength(1);    //столбцов

            Complex[,] outC = new Complex[M, N];

            //преобразуем строки
            for (int i = 0; i < M; i++) //проходим по строкам
            {
                Complex[] str = new Complex[N];
                for (int j = 0; j < N; j++) //проходим внутри строки, собирая её
                    str[j] = source[i, j];
                str = Fourier.Transform(str);   //получаем фурье образ строки
                for (int j = 0; j < N; j++) //записываем его в массив
                    outC[i, j] = str[j];
            }

            //преобразуем столбцы
            for (int i = 0; i < N; i++) //проходим по столбцам
            {
                Complex[] raw = new Complex[M];
                for (int j = 0; j < M; j++) //проходим внутри столбца, собирая его
                    raw[j] = outC[j, i];
                raw = Fourier.Transform(raw);   //получаем фурье образ столбца
                for (int j = 0; j < M; j++) //записываем его в массив
                    outC[j, i] = raw[j];
            }

            return outC;
        }

        /// <summary>
        /// Двумерное обратное преобразование фурье
        /// </summary>
        /// <param name="source">входная матрица</param>
        /// <returns>Выходной ряд</returns>
        public static Complex[,] ITransform(Complex[,] source)
        {
            int M = source.GetLength(0);    //строк
            int N = source.GetLength(1);    //столбцов

            Complex[,] outC = new Complex[M, N];

            //преобразуем столбцы
            for (int i = 0; i < N; i++) //проходим по столбцам
            {
                Complex[] raw = new Complex[M];
                for (int j = 0; j < M; j++) //проходим внутри столбца, собирая его
                    raw[j] = source[j, i];
                raw = Fourier.ITransform(raw);   //получаем фурье образ столбца
                for (int j = 0; j < M; j++) //записываем его в массив
                    outC[j, i] = raw[j];
            }

            //преобразуем строки
            for (int i = 0; i < M; i++) //проходим по строкам
            {
                Complex[] str = new Complex[N];
                for (int j = 0; j < N; j++) //проходим внутри строки, собирая её
                    str[j] = outC[i, j];
                str = Fourier.ITransform(str);   //получаем фурье образ строки
                for (int j = 0; j < N; j++) //записываем его в массив
                    outC[i, j] = str[j];
            }

            return outC;
        }


        /// <summary>
        /// Вычисление поворачивающего модуля e^(-i*2*PI*k/N)
        /// </summary>
        /// <param name="k"></param>
        /// <param name="N"></param>
        /// <returns></returns>
        private static Complex Module(int k, int N)
        {
            //            if (k % N == 0) return 1;
            double arg = -2 * Math.PI * k / N;
            return new Complex(Math.Cos(arg), Math.Sin(arg));
        }

        /// <summary>
        /// Возвращает спектр сигнала, вычесленное по быстрому алгоритму фурье
        /// </summary>
        /// <param name="input">Массив значений сигнала</param>
        /// <returns>Массив со значениями спектра сигнала</returns>
        public static Complex[] FastTransform(Complex[] input)
        {
            double log = Math.Log(input.Length, 2);
            Complex[] x;

            if (log - Math.Round(log) != 0)
            {
                x = new Complex[(int) Math.Pow(2,(int)log + 1)];
                input.CopyTo(x, 0);
            }
            else
            {
                x = (Complex[]) input.Clone();
            }


            Complex[] X;
            int N = x.Length;
            if (N == 2)
            {
                X = new Complex[2];
                X[0] = x[0] + x[1];
                X[1] = x[0] - x[1];
            }
            else
            {
                Complex[] x_even = new Complex[N / 2];
                Complex[] x_odd = new Complex[N / 2];
                for (int i = 0; i < N / 2; i++)
                {
                    x_even[i] = x[2 * i];
                    x_odd[i] = x[2 * i + 1];
                }
                Complex[] X_even = UnsafeFastTransform(x_even);
                Complex[] X_odd = UnsafeFastTransform(x_odd);
                X = new Complex[N];
                for (int i = 0; i < N / 2; i++)
                {
                    X[i] = X_even[i] + Module(i, N) * X_odd[i];
                    X[i + N / 2] = X_even[i] - Module(i, N) * X_odd[i];
                }
            }

            return X;
        }


        /// <summary>
        /// Возвращает спектр сигнала, вычесленное по быстрому алгоритму фурье. 
        /// Не содержит проверки корректности входных значений.
        /// </summary>
        /// <param name="input">Массив значений сигнала</param>
        /// <returns>Массив со значениями спектра сигнала</returns>
        private static Complex[] UnsafeFastTransform(Complex[] input)
        {

            Complex[] X;
            int N = input.Length;
            if (N == 2)
            {
                X = new Complex[2];
                X[0] = input[0] + input[1];
                X[1] = input[0] - input[1];
            }
            else
            {
                Complex[] x_even = new Complex[N / 2];
                Complex[] x_odd = new Complex[N / 2];
                for (int i = 0; i < N / 2; i++)
                {
                    x_even[i] = input[2 * i];
                    x_odd[i] = input[2 * i + 1];
                }
                Complex[] X_even = UnsafeFastTransform(x_even);
                Complex[] X_odd = UnsafeFastTransform(x_odd);
                X = new Complex[N];
                for (int i = 0; i < N / 2; i++)
                {
                    X[i] = X_even[i] + Module(i, N) * X_odd[i];
                    X[i + N / 2] = X_even[i] - Module(i, N) * X_odd[i];
                }
            }

            return X;
        }


        /// <summary>
        /// Обратное быстрое преобразование фурье
        /// </summary>
        /// <param name="x">Массив значений сигнала</param>
        /// <returns>Массив со значениями спектра сигнала</returns>
        public static Complex[] IFastTransform(Complex[] x, int length = 0)
        {
            Complex[] iTransform = PartIFastTransform(x);
            Complex[] ret;
            if (length != 0)
            {
                ret = new Complex[length];
                for (int i = 0; i < length; i++)
                    ret[i] = iTransform[i];
            }
            else
            {
                ret = iTransform;
            }
            for (int i = 0; i < ret.Length; i++)
                ret[i] /= iTransform.Length;
            return ret;
        }

        /// <summary>
        /// Обратное быстрое преобразование фурье
        /// </summary>
        /// <param name="x">Массив значений сигнала</param>
        /// <returns>Массив со значениями спектра сигнала</returns>
        private static Complex[] PartIFastTransform(Complex[] x)
        {
            double log = Math.Log(x.Length, 2);
            if (log - Math.Round(log) != 0) return null;
            Complex[] X;
            int N = x.Length;
            if (N == 2)
            {
                X = new Complex[2];
                X[0] = x[0] + x[1];
                X[1] = x[0] - x[1];
            }
            else
            {
                Complex[] x_even = new Complex[N / 2];
                Complex[] x_odd = new Complex[N / 2];
                for (int i = 0; i < N / 2; i++)
                {
                    x_even[i] = x[2 * i];
                    x_odd[i] = x[2 * i + 1];
                }
                Complex[] X_even = UnsafePartIFastTransform(x_even);
                Complex[] X_odd = UnsafePartIFastTransform(x_odd);
                X = new Complex[N];
                for (int i = 0; i < N / 2; i++)
                {
                    X[i] = X_even[i] + Module(-i, N) * X_odd[i];
                    X[i + N / 2] = X_even[i] - Module(-i, N) * X_odd[i];
                }
            }

            return X;
        }


        /// <summary>
        /// Обратное быстрое преобразование фурье.
        /// Возвращает спектр сигнала, вычесленное по быстрому алгоритму фурье. 
        /// Не содержит проверки корректности входных значений.
        /// </summary>
        /// <param name="x">Массив значений сигнала</param>
        /// <returns>Массив со значениями спектра сигнала</returns>
        private static Complex[] UnsafePartIFastTransform(Complex[] x)
        {
            Complex[] X;
            int N = x.Length;
            if (N == 2)
            {
                X = new Complex[2];
                X[0] = x[0] + x[1];
                X[1] = x[0] - x[1];
            }
            else
            {
                Complex[] x_even = new Complex[N / 2];
                Complex[] x_odd = new Complex[N / 2];
                for (int i = 0; i < N / 2; i++)
                {
                    x_even[i] = x[2 * i];
                    x_odd[i] = x[2 * i + 1];
                }
                Complex[] X_even = UnsafePartIFastTransform(x_even);
                Complex[] X_odd = UnsafePartIFastTransform(x_odd);
                X = new Complex[N];
                for (int i = 0; i < N / 2; i++)
                {
                    X[i] = X_even[i] + Module(-i, N) * X_odd[i];
                    X[i + N / 2] = X_even[i] - Module(-i, N) * X_odd[i];
                }
            }

            return X;
        }

        /// <summary>
        /// Двумерное прямое быстрое преобразование фурье
        /// </summary>
        /// <param name="source">входная матрица</param>
        /// <returns>Выходной ряд</returns>
        public static Complex[,] FastTransform(Complex[,] source)
        {
            int m = source.GetLength(0);    //строк
            int n = source.GetLength(1);    //столбцов

            int size = n;
            if (m > n)
                size = m;

            
            double log = Math.Log(size, 2);
            Complex[,] x;

            if (log - Math.Round(log) != 0)
            {
                size = (int)Math.Pow(2, (int)log + 1);
                x = new Complex[size, size];
                for (int i = 0; i < m; i++)
                    for (int j = 0; j < n; j++)
                        x[i,j] = source[i,j];
            }
            else
            {
                x = (Complex[,])source.Clone();
            }
            
            Complex[,] outC = new Complex[size, size];

            //преобразуем строки
            for (int i = 0; i < size; i++) //проходим по строкам
            {
                Complex[] str = new Complex[size];
                for (int j = 0; j < size; j++) //проходим внутри строки, собирая её
                    str[j] = x[i, j];
                str = Fourier.UnsafeFastTransform(str);   //получаем фурье образ строки
                for (int j = 0; j < size; j++) //записываем его в массив
                    outC[i, j] = str[j];
            }

            //преобразуем столбцы
            for (int i = 0; i < size; i++) //проходим по столбцам
            {
                Complex[] raw = new Complex[size];
                for (int j = 0; j < size; j++) //проходим внутри столбца, собирая его
                    raw[j] = outC[j, i];
                raw = Fourier.UnsafeFastTransform(raw);   //получаем фурье образ столбца
                for (int j = 0; j < size; j++) //записываем его в массив
                    outC[j, i] = raw[j];
            }

            return outC;
        }

 
        /// <summary>
        /// Двумерное обратное быстрое преобразование фурье
        /// </summary>
        /// <param name="source">входная матрица</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <returns>
        /// Выходной ряд
        /// </returns>
        public static Complex[,] IFastTransform(Complex[,] source, int height, int width)
        {

            if (source.GetLength(0) != source.GetLength(1)) return null;
            int size = source.GetLength(0);


            Complex[,] outC = new Complex[size, size];

            //преобразуем столбцы
            for (int i = 0; i < size; i++) //проходим по столбцам
            {
                Complex[] raw = new Complex[size];
                for (int j = 0; j < size; j++) //проходим внутри столбца, собирая его
                    raw[j] = source[j, i];
                raw = Fourier.IFastTransform(raw);   //получаем фурье образ столбца
                for (int j = 0; j < size; j++) //записываем его в массив
                    outC[j, i] = raw[j];
            }

            //преобразуем строки
            for (int i = 0; i < size; i++) //проходим по строкам
            {
                Complex[] str = new Complex[size];
                for (int j = 0; j < size; j++) //проходим внутри строки, собирая её
                    str[j] = outC[i, j];
                str = Fourier.IFastTransform(str);   //получаем фурье образ строки
                for (int j = 0; j < size; j++) //записываем его в массив
                    outC[i, j] = str[j];
            }

            Complex[,] value = new Complex[height, width];

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    value[i, j] = outC[i, j];
            return value;
        }
    }
}
