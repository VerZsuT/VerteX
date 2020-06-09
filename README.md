# Язык программирования VerteX

_Находится в стадии разработки._

## Немного о языке

Это компилируемый язык с динамической типизацией и С-подобным синтаксисом.

### Планируемые фишки

Основное отличие от остальных ЯП - отсутствие основного языка.
Можно писать код как на английском, так и на русском, китайском и других поддерживаемых языках.

Есть возможность импортировать методы из других сборок(dll) и языков программирования.

Для удобства использования библиотек, написанных на другом человеческом языке, можно связать названия методов и переменных
Например "Вывод => Print", в таком случае при компиляции вместо 'Вывод' будет подставляться 'Print', что обеспечит правильную работу программы.

### Что реализовано на данных момент?

Сейчас в языке есть функции, переменные, ввод и вывод, а также конструкция IF, но без Else.
Уже в разработке остальные основные конструкции языка, такие как FOR, WHILE.

## Пример использования

Исходный файл(.vertex):
```
функция выводНаЭкран(сообщение) {
	печать(сообщение);
}
автор = "Александр";
выводНаЭкран(автор);
```

Cобираем проект в Visual Studio, либо качаем сборку с Google Drive по [ссылке](https://drive.google.com/file/d/1aJ9sKcrCgp_z6tBu24dat97-UJpCkVgo/view?usp=sharing).

Переносим в папку со сборкой файл с кодом (чтобы не прописывать полный путь).

Работаем в консоли:
1. Переходим в папку со сборкой. 
2. Выполняем в консоли **`vertex <FileName>.vertex`**, ждём компиляции(1-2сек).

Вывод в консоли:
```
Александр
```

### Дополнительные флаги и команды

Есть две основные команды:
* **`vertex <FileUrl>`** - компилирует в памяти и исполняет.
* **`vertex compile <FileUrl>`** - компилирует и сохраняет исполняемый файл в рабочей директории под именем "accembly".

Также есть пару полезных флагов (писать после FileUrl):
* **`-save`** - сохраняет .exe файл.
* **`-norun`** - не исполняет скомпилированный код.
* **`-nologs`** - не выводит Log-сообщения в консоль.
* **`-debug`** - включает 'debugMode', в нём в консоль выводятся ошибки компиляции, а также сгенерированный код (в этих целях он генерируется максимально читабельным). В общем режим отладки.

Флаги являются более приоритетными.
