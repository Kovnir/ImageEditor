using System;
using System.Numerics;

namespace ImageEditor
{
    /// <summary>
    /// Содержит методы расширения класса ConvolutionFilter для преобразования фильтра 
    /// в форму частотно-контрастной характеристики и обратно.
    /// </summary>
    public class OpticalTransferFunction
    {
        /// <summary>
        /// Возведение в квадрат модуля косплексного числа
        /// </summary>
        /// <param name="val">Комплексное число</param>
        /// <returns></returns>
        public static Complex ModPow(Complex val)
        {
            return new Complex(Math.Pow(val.Real, 2), Math.Pow(val.Imaginary, 2));
        }

        /// <summary>
        /// Перевод оператора искажения из пространственной в частотную область. Размерность не меняется. 
        /// </summary>
        /// <param name="filter">Оператор искажения PSF (Point Spread Function)</param>
        /// <returns>OTF (Optical Transfer Function)</returns>
        public static Complex[,] Psf2otf(ConvolutionFilter filter)
        {
            double[,] filterMatrix = filter.normalizedFilterMatrix;
            int FilterSize = filterMatrix.GetLength(0);
            int halfSize = (FilterSize - 1) / 2;
            int ost = FilterSize - halfSize;
            double[,] newFilter = new double[FilterSize, FilterSize];
            //+ + -
            //+ + -
            //- - -
            for (int i = 0; i < ost; i++)
                for (int j = 0; j < ost; j++)
                    newFilter[i, j] = filterMatrix[i + halfSize, j + halfSize];
            //- - +
            //- - +
            //- - -
            for (int i = 0; i < ost; i++)
                for (int j = ost; j < FilterSize; j++)
                    newFilter[i, j] = filterMatrix[i + halfSize, j - ost];
            //- - -
            //- - -
            //+ + -
            for (int i = ost; i < FilterSize; i++)
                for (int j = 0; j < ost; j++)
                    newFilter[i, j] = filterMatrix[i - ost, j + halfSize];
            //- - -
            //- - -
            //- - +
            for (int i = ost; i < FilterSize; i++)
                for (int j = ost; j < FilterSize; j++)
                    newFilter[i, j] = filterMatrix[i - ost, j - ost];

            return Fourier.Transform(Converter.ToComplexMatrix(newFilter));
        }

        /// <summary>
        /// Перевод оператора искажения из пространственной в частотную область с новой (бОльшей) размерностью.  
        /// </summary>
        /// <param name="filter">оператор искажения исходного размера</param>
        /// <param name="newSize">новая размерность</param>
        /// <returns></returns>
        public static Complex[,] Psf2otf(ConvolutionFilter filter, int newSize)
        {
            double[,] filterMatrix = filter.normalizedFilterMatrix;
            int sourceFilterSize = filterMatrix.GetLength(0);
            int halfSize = (filter.filterMatrix.GetLength(0) - 1) / 2;
            if (newSize < sourceFilterSize)
                return null;
            double[,] extendedFilter = new double[newSize, newSize];
            //0 0 0
            //0 0 0
            //0 0 0
            for (int i = 0; i < newSize; i++)
                for (int j = 0; j < newSize; j++)
                {
                    extendedFilter[i, j] = 0;
                }
            //- - -
            //- + +
            //- + +
            for (int i = 0; i < halfSize + 1; i++)
                for (int j = 0; j < halfSize + 1; j++)
                    extendedFilter[i, j] = filterMatrix[i + halfSize, j + halfSize];
            //- - -
            //+ - -
            //+ - -
            for (int i = 0; i < halfSize + 1; i++)
                for (int j = newSize - halfSize; j < newSize; j++)
                    extendedFilter[i, j] = filterMatrix[i + halfSize, j - (newSize - halfSize)];
            //- + +
            //- - -
            //- - -
            for (int i = newSize - halfSize; i < newSize; i++)
                for (int j = 0; j < halfSize + 1; j++)
                    extendedFilter[i, j] = filterMatrix[i - (newSize - halfSize), j + halfSize];
            //+ - -
            //- - -
            //- - -
            for (int i = newSize - halfSize; i < newSize; i++)
                for (int j = newSize - halfSize; j < newSize; j++)
                    extendedFilter[i, j] = filterMatrix[i - (newSize - halfSize), j - (newSize - halfSize)];

            return Fourier.Transform(Converter.ToComplexMatrix(extendedFilter));
        }

        /// <summary>
        /// Перевод оператора искажения из частотной в пространственную область. Размерность не меняется.
        /// </summary>
        /// <param name="otf">оператор искажения в частотной области (OTF - Optical Transfer Function)</param>
        /// <returns>PSF - Point Spread Function</returns>
        public static ConvolutionFilter Otf2psf(Complex[,] otf)
        {
            Complex[,] psf = Fourier.ITransform(otf);
            int FilterSize = psf.GetLength(0);
            int halfSize = (FilterSize - 1) / 2;
            int ost = FilterSize - halfSize;
            Complex[,] returnPSF = new Complex[FilterSize, FilterSize];
            //+ - -
            //- - -
            //- - -
            for (int i = 0; i < halfSize; i++)
                for (int j = 0; j < halfSize; j++)
                    returnPSF[i, j] = psf[i + ost, j + ost];
            //- + +
            //- - -
            //- - -
            for (int i = 0; i < halfSize; i++)
                for (int j = halfSize; j < FilterSize; j++)
                    returnPSF[i, j] = psf[i + ost, j - halfSize];
            //- - -
            //+ - -
            //+ - -
            for (int i = halfSize; i < FilterSize; i++)
                for (int j = 0; j < halfSize; j++)
                    returnPSF[i, j] = psf[i - halfSize, j + ost];
            //- - -
            //- + +
            //- + +
            for (int i = halfSize; i < FilterSize; i++)
                for (int j = halfSize; j < FilterSize; j++)
                    returnPSF[i, j] = psf[i - halfSize, j - halfSize];

            ConvolutionFilter cf = new ConvolutionFilter("Recovery Fiter", Converter.ToDoubleMatrix(returnPSF));
            return cf;
        }
    }
}
