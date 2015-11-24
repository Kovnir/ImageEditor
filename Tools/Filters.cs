using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor
{
    public static class Filters
    {
        /* --------------------------------- МАСКА КОПИРОВАНИЯ --------------------------------- */
        public static ConvolutionFilter CopyFilter
        {
            get
            {
                return new ConvolutionFilter("CopyFilter",
                    new double[,] { { 0, 0, 0, },  
                        { 0, 1, 0, },  
                        { 0, 0, 0, }, });
            }
        }        /* --------------------------------- BLURING --------------------------------- */
        public static ConvolutionFilter Blur3x3Filter
        {
            get
            {
                return new ConvolutionFilter("Blur3x3Filter",
                    new double[,] { { 0.0, 0.2, 0.0, },  
                        { 0.2, 0.2, 0.2, },  
                        { 0.0, 0.2, 0.2, }, });
            }
        }
        public static ConvolutionFilter Blur5x5Filter
        {
            get
            {
                return new ConvolutionFilter("Blur5x5Filter",
                    new double[,] { { 0, 0, 1, 0, 0, }, 
                        { 0, 1, 1, 1, 0, }, 
                        { 1, 1, 1, 1, 1, }, 
                        { 0, 1, 1, 1, 0, }, 
                        { 0, 0, 1, 0, 0, }, }, 1.0/13.0);
            }
        }
        public static ConvolutionFilter Gaussian3x3BlurFilter
        {
            get
            {
                return new ConvolutionFilter("Gaussian3x3BlurFilter",
                    new double[,] { { 1, 2, 1, },  
                        { 2, 4, 2, },  
                        { 1, 2, 1, }, }, 1.0 / 16.0);
            }
        }
        public static ConvolutionFilter Gaussian5x5BlurFilter
        {
            get
            {
                return new ConvolutionFilter("Gaussian5x5BlurFilter",
                    new double[,] { { 2, 04, 05, 04, 2, }, 
                        { 4, 09, 12, 09, 4, }, 
                        { 5, 12, 15, 12, 5, }, 
                        { 4, 09, 12, 09, 4, }, 
                        { 2, 04, 05, 04, 2, }, }, 1.0 / 159.0);
            }
        }
        public static ConvolutionFilter MotionBlurFilter
        {
            get
            {
                return new ConvolutionFilter("MotionBlurFilter",
                    new double[,] { { 1, 0, 0, 0, 0, 0, 0, 0, 1, }, 
                        { 0, 1, 0, 0, 0, 0, 0, 1, 0, },
                        { 0, 0, 1, 0, 0, 0, 1, 0, 0, },
                        { 0, 0, 0, 1, 0, 1, 0, 0, 0, },
                        { 0, 0, 0, 0, 1, 0, 0, 0, 0, }, 
                        { 0, 0, 0, 1, 0, 1, 0, 0, 0, },
                        { 0, 0, 1, 0, 0, 0, 1, 0, 0, },
                        { 0, 1, 0, 0, 0, 0, 0, 1, 0, },
                        { 1, 0, 0, 0, 0, 0, 0, 0, 1, }, }, 1.0 / 18.0);
            }
        }
        public static ConvolutionFilter MotionBlurLeftToRightFilter
        {
            get
            {
                return new ConvolutionFilter("MotionBlurLeftToRightFilter",
                    new double[,] { { 1, 0, 0, 0, 0, 0, 0, 0, 0, }, 
                        { 0, 1, 0, 0, 0, 0, 0, 0, 0, },
                        { 0, 0, 1, 0, 0, 0, 0, 0, 0, },
                        { 0, 0, 0, 1, 0, 0, 0, 0, 0, },
                        { 0, 0, 0, 0, 1, 0, 0, 0, 0, }, 
                        { 0, 0, 0, 0, 0, 1, 0, 0, 0, },
                        { 0, 0, 0, 0, 0, 0, 1, 0, 0, },
                        { 0, 0, 0, 0, 0, 0, 0, 1, 0, },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 1, },}, 1.0 / 9.0);
            }
        }
        public static ConvolutionFilter MotionBlurRightToLeftFilter
        {
            get
            {
                return new ConvolutionFilter("MotionBlurRightToLeftFilter",
                    new double[,] {{ 0, 0, 0, 0, 0, 0, 0, 0, 1, }, 
                        { 0, 0, 0, 0, 0, 0, 0, 1, 0, },
                        { 0, 0, 0, 0, 0, 0, 1, 0, 0, },
                        { 0, 0, 0, 0, 0, 1, 0, 0, 0, },
                        { 0, 0, 0, 0, 1, 0, 0, 0, 0, }, 
                        { 0, 0, 0, 1, 0, 0, 0, 0, 0, },
                        { 0, 0, 1, 0, 0, 0, 0, 0, 0, },
                        { 0, 1, 0, 0, 0, 0, 0, 0, 0, },
                        { 1, 0, 0, 0, 0, 0, 0, 0, 0, }, }, 1.0 / 9.0);
            }
        }
        public static ConvolutionFilter SoftenFilter
        {
            get
            {
                return new ConvolutionFilter("SoftenFilter",
                    new double[,] {{ 1, 1, 1, },  
                        { 1, 1, 1, },  
                        { 1, 1, 1, }, }, 1.0 / 8.0);
            }
        }

        /* --------------------------------- SHARPNG --------------------------------- */
        public static ConvolutionFilter SharpenFilter
        {
            get
            {
                return new ConvolutionFilter("SharpenFilter",
                    new double[,] {{ -1, -1, -1, },  
                        { -1,  9, -1, },  
                        { -1, -1, -1, }, });
            }
        }
        public static ConvolutionFilter Sharpen3x3Filter
        {
            get
            {
                return new ConvolutionFilter("Sharpen3x3Filter",
                    new double[,] {{  0, -2,  0, },  
                        { -2,  11, -2, },  
                        {  0, -2,  0, }, }, 1.0 / 3.0);
            }
        }
        public static ConvolutionFilter Sharpen5x5Filter
        {
            get
            {
                return new ConvolutionFilter("Sharpen5x5Filter",
                    new double[,] {{ -1, -1, -1, -1, -1, }, 
                        { -1,  2,  2,  2, -1, },
                        { -1,  2,  8,  2,  1, },
                        { -1,  2,  2,  2, -1, },
                        { -1, -1, -1, -1, -1, }, }, 1.0 / 8.0);
            }
        }
        public static ConvolutionFilter Sharpen3x3FactorFilter
        {
            get
            {
                return new ConvolutionFilter("Sharpen3x3FactorFilter",
                    new double[,] {{  0, -2,  0, },  
                        { -2, 11, -2, },  
                        {  0, -2,  0, }, }, 1.0 / 3.0);
            }
        }
        public static ConvolutionFilter IntenseSharpenFilter
        {
            get
            {
                return new ConvolutionFilter("IntenseSharpenFilter",
                    new double[,] { { 1,  1, 1, },  
                        { 1, -7, 1, },  
                        { 1,  1, 1, }, });
            }
        }
        /* --------------------------------- EDGE DETECTIONING --------------------------------- */
        public static ConvolutionFilter EdgeDetectionFilter
        {
            get
            {
                return new ConvolutionFilter("EdgeDetectionFilter",
                    new double[,] { { -1, -1, -1, },  
                        { -1,  8, -1, },  
                        { -1, -1, -1, },});
            }
        }
        public static ConvolutionFilter EdgeDetection45DegreeFilter
        {
            get
            {
                return new ConvolutionFilter("EdgeDetection45DegreeFilter",
                    new double[,] { { -1,  0,  0,  0,  0, }, 
                        {  0, -2,  0,  0,  0, },
                        {  0,  0,  6,  0,  0, },
                        {  0,  0,  0, -2,  0, },
                        {  0,  0,  0,  0, -1, }, });
            }
        }
        public static ConvolutionFilter HorizontalEdgeDetectionFilter
        {
            get
            {
                return new ConvolutionFilter("HorizontalEdgeDetectionFilter",
                    new double[,] { {  0,  0,  0,  0,  0, }, 
                        {  0,  0,  0,  0,  0, },
                        { -1, -1,  2,  0,  0, },
                        {  0,  0,  0,  0,  0, },
                        {  0,  0,  0,  0,  0, },});
            }
        }
        public static ConvolutionFilter VerticalEdgeDetectionFilter
        {
            get
            {
                return new ConvolutionFilter("VerticalEdgeDetectionFilter",
                    new double[,] { { 0,  0, -1,  0,  0, }, 
                        { 0,  0, -1,  0,  0, },
                        { 0,  0,  4,  0,  0, },
                        { 0,  0, -1,  0,  0, },
                        { 0,  0, -1,  0,  0, }, });
            }
        }
        public static ConvolutionFilter EdgeDetectionTopLeftBottomRightFilter
        {
            get
            {
                return new ConvolutionFilter("EdgeDetectionTopLeftBottomRightFilter",
                    new double[,] {  { -5,  0,  0, },  
                        {  0,  0,  0, },  
                        {  0,  0,  5, }, });
            }
        }
        /* --------------------------------- EMBOSING --------------------------------- */
        public static ConvolutionFilter EmbossFilter
        {
            get
            {
                return new ConvolutionFilter("EmbossFilter",
                    new double[,] { { 2,  0,  0, },  
                        { 0, -1,  0, },  
                        { 0,  0, -1, }, }, 1.0, 128);
            }
        }
        public static ConvolutionFilter Emboss45DegreeFilter
        {
            get
            {
                return new ConvolutionFilter("Emboss45DegreeFilter",
                    new double[,] { { -1, -1,  0, },  
                        { -1,  0,  1, },  
                        {  0,  1,  1, }, }, 1.0, 128);
            }
        }
        public static ConvolutionFilter EmbossTopLeftBottomRightFilter
        {
            get
            {
                return new ConvolutionFilter("EmbossTopLeftBottomRightFilter",
                    new double[,] { { -1, 0, 0, },  
                        {  0, 0, 0, },  
                        {  0, 0, 1, }, }, 1.0, 128);
            }
        }
        public static ConvolutionFilter IntenseEmbossFilter
        {
            get
            {
                return new ConvolutionFilter("IntenseEmbossFilter",
                    new double[,] { { -1, -1, -1, -1,  0, }, 
                        { -1, -1, -1,  0,  1, },
                        { -1, -1,  0,  1,  1, },
                        { -1,  0,  1,  1,  1, },
                        {  0,  1,  1,  1,  1, }, }, 1.0, 128);
            }
        }
        public static ConvolutionFilter HighPass3x3Filter
        {
            get
            {
                return new ConvolutionFilter("HighPass3x3Filter",
                    new double[,] { { -1, -2, -1, },  
                        { -2, 12, -2, },  
                        { -1, -2, -1, } }, 1.0 / 16.0, 128.0);
            }
        }
    }
}
