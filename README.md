# ImageEditor
Lightweight library for image manipulation on .NET

## Description

Library was developed as part of diploma. That include three more parts:
[Delbluring libruary](github.com/xsimbvx/ImageRecovery), [Integration sample](github.com/xsimbvx/IRIntegration) and [Modification of delbluring algorithms](github.com/xsimbvx/DeblurModification).
Project was builded in Microsoft Visual Studio 2015.

Библиотека по большей части представляет собой реализацию функций пакета прикладных программ MATLAB для работы с изображениями, который в дальнейшем используются в библиотеке востановления изображений. Библиотека включает в себя несколько файлов.


## Classes

Class | Description
------------ | -------------
***ConvolutionFilter.cs*** | Содержит публичный класс *ConvolutionFilter* представляющий из себя ядро искажающего оператора. Включает в себя функцию свёртки (convolution), с возможностью увеличения и уменьшения размера отноительно исходного изображения. Так же имеет публичный класс Channel для обозначения того, с какими каналами изображения должны проводиться манипуляции, и класс-расширение для базового класса Image, добавляющий в класс Image функцию свёрки.
***Filters.cs*** | Содержит публичный класс *Filters* с предописанными частоиспользуемыми фильтрами, среди которых маска копирования, восемь масок размытия, пять масок резкости, пять масок обнаружения границ и пять масок тиснения.
***Converter.cs*** | Содержит публичный статический класс *Converter*, содержащий множество методов преобразования изображений, линейных и двумерных массивов байт, комплексных чисел и чисел с плавающей запятой друг в друга.
***Fourier.cs*** | Содержит публичный статический класс *Fourier*, содержаций методы линейного и двумерного прямого и обратного преобразования Фурье. 
***OpticalTransferFunction.cs*** | Содержит методы расширения класса *ConvolutionFilter* для преобразования фильтра в форму частотно-контрастной характеристики и обратно.
***ImageExtentions.cs*** | Содержит класс *ImageExtentions* - реализующий методы-расширения класса Image, которые удобны для редактирования изображений. В том числе: масштабирование, несколького методов добавления гауссового шума, перевод изображения в чернобелое по алгоритму MATLAB, вычленение конкретного канала, зеркальное расширение изображения и пр.


## Integration

Integration sample you can find in Sample.cs

```с#
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

```



## Contacts

Develoment is over, but if you have interesting ideas, or think, that some methods work bad or just slow - let me know it!

* mail: kovnir.alik@gmail.com
* vk: http://vk.com/akovnir
