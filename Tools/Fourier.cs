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
                dist[n] = value/N;
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

            Complex[,] outC = new Complex[M,N];

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

    }
}
