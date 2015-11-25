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


## Integration samples

### ИЗМЕНЕНИЕ ИЗОБРАЖЕНИЯ

#### Пример 1

Запишем в image2 находится image1, отмасштабируемый до размера 100 (ширина) и 200 (высота)

```c#
        image2 = image1.Scale(100, 200);
```

#### Пример 2

Запишем в image2 image1, уменьшиный в 2 раза

```c#
        image2 = image1.Scale(image1.Width / 2, image1.Height / 2);
```

#### Пример 3

Попробуем задать отрицательные значения размеров

```c#
        try
        {
              image2 = image1.Scale(-1, -3); //отрицательные размеры породят исключние
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(ex.Message, "Scale exeption");
        }
```

В случае неуверенности в значениях размеров, заключайте вызов функции ы блок try-catch!

#### Пример 4

Добавим к image1 аддитивный шум в соотношении шум/полезный сигнал 1 к 9. Запишем результвт в image2

```c#
        image2 = image1.Noize(1f / (1 + 9));
```

#### Пример 5

Добавим к image1 аддитивный шум в соотношении шум/полезный сигнал 1 к 2. Запишем результвт в image2,
а шум в три отдельные матрицы по одной на каждый цветовой канал.

```c#
        byte[,] rNoize = null;
        byte[,] gNoize = null;
        byte[,] bNoize = null;
        image2 = image1.Noize(1f / (1 + 2), ref rNoize, ref gNoize, ref bNoize);
```

фунуция SingleNoize работает так же, как Noize, только накладывет на все каналы один и тот же шум


#### Пример 6

Интуитивно продлим image1 на 10 пикселей со всех сторон. Запишем результвт в image2

```c#
        image2 = image1.Expand(10);
```

### РАБОТА С КАНАЛАМИ

#### Пример 1

Переведём image1 в чёрнобелый и запишем его в image2

```c#
        image2 = image1.ToBlackAndWhite();
```
#### Пример 2

Запишем значение синего канала image1 во все каналы image2

```c#
        image2 = image1.OnlyBlue();
```

Аналогично работает с красным и зелёным каналами.

### ЗАГРУЗКА ИЗОБРАЖЕНИЯ

#### Пример 1

Загрузить данные в image2 из массива байт, где каждый пиксел задан четырмя значениями каналов
Размер результирующего изображения - 3 по ширине на 5 по высоте

```c#
        byte[] array  = new byte [3*5*4]                            //инициализируем массив
        {
            0,255,255,255, 255,0,255,255, 255,255,0,255, 
            0,255,255,255, 255,0,255,255, 255,255,0, 255,
            0,255,255,255, 255,0,255,255, 255,255,0, 255,
            0,255,255,255, 255,0,255,255, 255,255,0, 255,
            0,255,255,255, 255,0,255,255, 255,255,0, 255,
        };
        image2 = Converter.ToImage(array, 3);                //загружаем его. 
```

#### Пример 2

Загрузить данные в image2 из массива байт, где каждый значение задаёт яркость данного пиксела
Размер результирующего изображения - 3 по ширине на 8 по высоте

```c#
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
```

### ПЕРЕВОД В МАССИВЫ

#### Пример 1

Загрузить данные в image2 из массива байт, полученного из image1

```c#
        array = Converter.ToByteArray(image1);
        image2 = Converter.ToImage(array, image1.Width);
        //или одной строкой
        image2 = Converter.ToImage(Converter.ToByteArray(image1), image1.Width);
```

#### Пример 2

Преобразовать изображение image1 в маску яркости.

```c#
        array = Converter.ToByteArray(image1, true);
```

#### Пример 3

Преобразовать яркость изображение image1 в матрицу комплексных чисел.

```c#
        Complex[,] complex = Converter.ToComplexMatrix(image1);
```

Так же класс Converter умеет преобразовывать комплексные и байтовые массивы и матрицы, (а так же изображения) друг в друга.


### СВЁРТКА

Предустановленные фильтры находятся в классе Filters.
При свёртке изображение становится меньше, но можно предварительно увеличить изображение (по умолчанию). 
Чтобы задать нужный режим используйте enum ConvolutionMode.
Так же свёртку можно сделать не со всеми каналами, а выборочно.
Для этого используется класс Channel

#### Пример 1

Сделаем свёртку image1 с фильтром MotionBlur. Запишем результвт в image2

```c#
        image2 = image1.Convolution(Filters.MotionBlurFilter);
```

Второй вариант:

```
image2 = Filters.MotionBlurFilter.Convolution(image1);
```

#### Пример 2

Сделаем свёртку image1 с произвольным фильтром, свернём только красный и зелёный каналы. 
Не увеличиваем предваительно изображение. Запишем результвт в image2

```c#
ConvolutionFilter filter = new ConvolutionFilter("custom filter",
    new double[3, 3] { { 0.1d, 0.1d, 0.1d }, { 0.2d, 0.4d, 0.2d }, { 0.1d, 0.1d, 0.1d } }, 1d / 1.2d, 0);
image2 = image1.Convolution(filter, ConvolutionFilter.ConvolutionMode.collapse, Channel.RED | Channel.GREEN);
```

## Examples (примеры работы)



## Contacts

Develoment is over, but if you have interesting ideas, or think, that some methods work bad or just slow - let me know it!

* mail: kovnir.alik@gmail.com
* vk: http://vk.com/akovnir
