﻿using System;                   //пространство имён фундоментальных и базовых классов
using System.Windows.Forms;     //для стандартных диалогов
using System.Drawing;           //для дрсиупа к классу Image
using ImageEditor;              //для доступа к функциям библиотеки
using System.Numerics;          //для доступа к кномплексным числам

class Tutorial
{

    public static void Test()
    {
        Image image1 = LoadImage();         //загружаем тестовое изображения
        if (image1 == null) return;         //уходим в случае неудачи
        Image image2 = null;                //инициализируем второе изображение, которое понадобится нам в примерах

        /* ----------------------------------------------------------------------------------------- */
        /* ------------------------------- НИЖЕ ПРИВЕДЕНЫ ПРИМЕРЫ ---------------------------------- */
        /* ----------------------------- ИСПОЛЬЗОВАНИЯ ВСЕХ ФУНКЦИЙ -------------------------------- */
        /* ---------------------------------- ДАННОЙ БИБЛИОТЕКИ ------------------------------------ */
        /* ----------------------------------------------------------------------------------------- */


        /* ----------------------------- 1. ИЗМЕНЕНИЕ ИЗОБРАЖЕНИЯ ---------------------------------- */

        //Пример 1
        //Запишем в image2 находится image1, отмасштабируемый до размера 100 (ширина) и 200 (высота)
        image2 = image1.Scale(100, 200);

        //Пример 2
        //Запишем в image2 image1, уменьшиный в 2 раза
        image2 = image1.Scale(image1.Width / 2, image1.Height / 2);

        //Пример 3
        //Попробуем задать отрицательные значения размеров
        try
        {
        //      image2 = image1.Scale(-1, -3);                      //отрицательные размеры породят исключние
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Scale exeption");
            return;
        }
        //В случае неуверенности в значениях размеров, заключайте вызов функции ы блок try-catch!

        //Пример 4
        //Добавим к image1 аддитивный шум в соотношении шум/полезный сигнал 1 к 9. Запишем результвт в image2

        image2 = image1.Noize(1f / (1 + 9));

        //Пример 5
        //Добавим к image1 аддитивный шум в соотношении шум/полезный сигнал 1 к 2. Запишем результвт в image2,
        //а шум в три отдельные матрицы по одной на каждый цветовой канал.

        byte[,] rNoize = null;
        byte[,] gNoize = null;
        byte[,] bNoize = null;
        image2 = image1.Noize(1f / (1 + 2), ref rNoize, ref gNoize, ref bNoize);

        //фунуция SingleNoize работает так же, как Noize, только накладывет на все каналы один и тот же шум

        //Пример 6
        //Интуитивно продлим image1 на 10 пикселей со всех сторон. Запишем результвт в image2

        image2 = image1.Expand(10);

        /* ------------------------------- 2. РАБОТА С КАНАЛАМИ ------------------------------------ */

        //Пример 1
        //Переведём image1 в чёрнобелый и запишем его в image2
        image2 = image1.ToBlackAndWhite();

        //Пример 2
        //Запишем значение синего канала image1 во все каналы image2
        image2 = image1.OnlyBlue();
        //Аналогично с красным и зелёным каналами


        /* ----------------------------- 3. ЗАГРУЗКА ИЗОБРАЖЕНИЯ ----------------------------------- */

        //Пример 1
        //Загрузить данные в image2 из массива байт, где каждый пиксел задан четырмя значениями каналов
        //Размер результирующего изображения - 3 по ширине на 5 по высоте

        byte[] array  = new byte [3*5*4]                            //инициализируем массив
        {
            0,255,255,255, 255,0,255,255, 255,255,0,255, 
            0,255,255,255, 255,0,255,255, 255,255,0, 255,
            0,255,255,255, 255,0,255,255, 255,255,0, 255,
            0,255,255,255, 255,0,255,255, 255,255,0, 255,
            0,255,255,255, 255,0,255,255, 255,255,0, 255,
        };
        image2 = Converter.ToImage(array, 3);                //загружаем его. 
        //Согласен, не самый удобный и логичный вариант вызова, но как сделать иначе - я ещё не придумал

        //Пример 2
        //Загрузить данные в image2 из массива байт, где каждый значение задаёт яркость данного пиксела
        //Размер результирующего изображения - 3 по ширине на 8 по высоте

        array = new byte[8 * 3]                                     //инициализируем массив
        {
            0,  0,  0, 
            10, 20, 20,
            40, 40, 40,
            60, 60, 60,
            80, 80, 80,
            100, 100, 100,
            120, 120, 120,
            140, 140, 140,
        };
        image2 = Converter.ToImage(array, 3, true);          //загружаем его.


        /* ------------------------------- 4. ПЕРЕВОД В МАССИВЫ ------------------------------------ */

        //Пример 1
        //Загрузить данные в image2 из массива байт, полученного из image1

        array = Converter.ToByteArray(image1);
        image2 = Converter.ToImage(array, image1.Width);
        //или одной строкой
        image2 = Converter.ToImage(Converter.ToByteArray(image1), image1.Width);

        //Пример 2
        //Преобразовать изображение image1 в маску яркости.

        array = Converter.ToByteArray(image1, true);

        //Пример 3
        //Преобразовать яркость изображение image1 в матрицу комплексных чисел.

        Complex[,] complex = Converter.ToComplexMatrix(image1);

        //Так же класс Converter умеет преобразовывать комплексные и байтовые массивы и матрицы
        //(а так же изображения) друг в друга.


        /* ------------------------------------ 5. СВЁРТКА ----------------------------------------- */

        //Предустановленные фильтры находятся в классе Filters.
        //При свёртке изображение становится меньше, но можно предварительно увеличить изображение (по умолчанию). 
        //Чтобы задать нужный режим используйте enum ConvolutionMode.
        //Так же свёртку можно сделать не со всеми каналами, а выборочно.
        //Для этого используется класс Channel

        //Пример 1
        //Сделаем свёртку image1 с фильтром MotionBlur. Запишем результвт в image2
        image2 = image1.Convolution(Filters.MotionBlurFilter);
        //или
        image2 = Filters.MotionBlurFilter.Convolution(image1);

        //Пример 2
        //Сделаем свёртку image1 с произвольным фильтром, свернём только красный и зелёный каналы. 
        //Не увеличиваем предваительно изображение. Запишем результвт в image2

        ConvolutionFilter filter = new ConvolutionFilter("custom filter",
            new double[3, 3] { { 0.1d, 0.1d, 0.1d }, { 0.2d, 0.4d, 0.2d }, { 0.1d, 0.1d, 0.1d } }, 1d / 1.2d, 0);
        image2 = image1.Convolution(filter, ConvolutionFilter.ConvolutionMode.collapse, Channel.RED | Channel.GREEN);

    }


    public static Image LoadImage()
    {
        Image image;
        OpenFileDialog ofd = new OpenFileDialog();          //попросим указать месторасположения файла
        if (ofd.ShowDialog() != DialogResult.OK)            //если не указано
            return null;                                         //уходим
        try
        {
            image = Image.FromFile(ofd.FileName);          //иначе пытаемся грузить картинку
        }
        catch                                               //если не удалось - сообщим об этом
        {
            MessageBox.Show("Не удалось загрузить изображение", "Loading exeption");
            return null;
        }
        return image;                                       //вернём изображение
    }
}
