# Язык программирования VerteX

_Находится в стадии разработки._

## Немного о языке

Это компилируемый язык с динамической типизацией и Си-подобным синтаксисом.

### Планируемые фишки

Мультиязычность: можно использовать любой реальный язык для написания кода (включая ключевые слова и методы сторонних библиотек).
Возможность импользования библиотек, написанных на других языках программирования и комбинирования их в одном коде на данном ЯП.
Совмещение компилируемых и интерпретируемых, а также строго и динамически типизируемых ЯП.

### Что реализовано на данных момент?

В языке есть функции, переменные, ввод и вывод, а также конструкции IF-ELSE, WHILE, DO-WHILE, TRY-CATCH, SWITCH.
Сейчас в разработке конструкция FOR, а также реализация ООП.
После этого займусь отличительными особенностями языка.

## Пример использования

Исходный файл(_.vertex_ / _.vrtx_):
```
функция выводНаЭкран(сообщение)
{
    печать(сообщение);
}
автор = "Александр";
выводНаЭкран(автор);
```

Cобираем проект в Visual Studio, либо качаем готовую сборку из Google Drive по [данной ссылке](https://drive.google.com/file/d/1fFu9MiruAaWgxwA-FVeleO1dR7YOlr0E/view?usp=sharing).

Переносим в папку со сборкой файл с кодом (чтобы не прописывать полный путь), либо используем уже готовый пробный файл "example.vrtx".

Работаем в консоли:
1. Переходим в папку со сборкой. 
2. Выполняем в консоли **`vertex <FileName>.vrtx`**, ждём компиляции.

Вывод в консоли:
```
Александр
```

P.s: Расширение файла не играет роли, но для понятности лучше использовать расширение _.vertex_ либо _.vrtx_.

### Флаги, команды и параметры

Есть две основные команды:
* **`vertex <FilePath>`** - компилирует в памяти и исполняет.
* **`vertex compile <FilePath>`** - компилирует и сохраняет исполняемый файл в рабочей директории под именем "assembly.exe".

Один парметр:
* **`lang=<lang_name>`** - реальный язык, на котором вы хотите писать код, на данный момент есть два поддерживаемых языка (_en_ и _ru_).

Также есть пару полезных флагов:
* **`-save`** - сохраняет сборку в файл.
* **`-norun`** - не исполняет скомпилированный код.
* **`-nologs`** - не выводит лог-сообщения в консоль.
* **`-debug`** - включает режим отладки, в нём в консоль выводятся ошибки компиляции, определённые токены, а также сгенерированный код (в этих целях он генерируется максимально читабельным).

### Помощь проекту

Буду рад любой помощи, начиная от сообщения об ошибке и до совместной работы над проектом.