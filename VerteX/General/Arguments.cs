using System;
using System.IO;
using VerteX.Exceptions;

namespace VerteX.General
{
    /// <summary>
    /// Предоставляет удобную работу с параметрами запуска.
    /// </summary>
    public class Arguments
    {
        /// <summary>
        /// Режим запуска компилятора.
        /// </summary>
        public RunMode runMode = RunMode.Default;

        /// <summary>
        /// Сохранять ли файл после компиляции.
        /// </summary>
        public bool save = false;

        /// <summary>
        /// Запускать ли сборку после компиляции.
        /// </summary>
        public bool run = true;

        /// <summary>
        /// Флаг для создания 
        /// </summary>
        public bool executable = true;

        /// <summary>
        /// Выводить ли сообщения отладки.
        /// </summary>
        public bool debug = false;

        /// <summary>
        /// Выводить ли лог-сообщения.
        /// </summary>
        public bool logs = true;

        /// <summary>
        /// Путь к исходному файлу с кодом.
        /// </summary>
        public string filePath = "";

        /// <summary>
        /// Обрабатывает параметры.
        /// </summary>
        /// <param name="args">Сырые входные параметры.</param>
        public Arguments(string[] args)
        {
            if (args.Length == 0)
                throw new RunException("Вызов компилятора без параметров невозможен");

            foreach (string param in args)
            {
                if (param == "compile" && runMode == RunMode.Default)
                {
                    runMode = RunMode.Compile;
                    save = true;
                    run = false;
                }
                else if (param == "link" && runMode == RunMode.Default)
                {
                    runMode = RunMode.Link;
                }
                else if (param == "clearLinks" && runMode == RunMode.Default)
                {
                    string path = GlobalParams.linksPath;

                    runMode = RunMode.ClearLinks;

                    if (File.Exists(path))
                        File.Delete(path);
                }
                else if (param == "-S" || param == "--save")
                    save = true;
                else if (param == "-NR" || param == "--norun")
                    run = false;
                else if (param == "-D" || param == "--debug")
                    debug = true;
                else if (param == "-NL" || param == "--nologs")
                    logs = false;
                else if (param == "-L" || param == "--library")
                    executable = false;
                else
                {
                    if (runMode == RunMode.Default || runMode == RunMode.Compile)
                    {
                        if (File.Exists(param))
                        {
                            filePath = param;
                            GlobalParams.fileName = Path.GetFileNameWithoutExtension(param);
                        }
                        else
                        {
                            throw new RunException("Файл по указанному пути не найден");
                        }

                    }
                    else if (runMode == RunMode.Link)
                    {
                        string[] prs = param.Split('=');

                        if (prs.Length == 2)
                            File.AppendAllText(GlobalParams.linksPath, $"{prs[0]} = {prs[1]} \n");

                        Console.WriteLine("VerteX[Лог]: Ссылка успешно добавлена.");
                    }
                }
            }

            if (filePath == "" && (runMode == RunMode.Default && runMode == RunMode.Compile))
                throw new RunException("Запуск компилятора без файла невозможен");
        }
    }
}
