using System.IO;
using VerteX.Compiling;
using VerteX.Lexing;

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
        /// Флаг для сохранения сборки в файл.
        /// </summary>
        public bool save = false;

        /// <summary>
        /// Флаг для запуска сборки после компиляции.
        /// </summary>
        public bool run = true;

        /// <summary>
        /// Флаг, отвечающий за вывод дебаг-сообщений в консоль.
        /// </summary>
        public bool debug = false;

        /// <summary>
        /// Флаг, отвечающий за вывод лог-сообщений в консоль.
        /// </summary>
        public bool logs = true;

        /// <summary>
        /// Путь к исходному файлу с кодом.
        /// </summary>
        public string filePath = "";

        /// <summary>
        /// Обрабатывает параметры.
        /// </summary>
        /// <param name="args">Входные параметры, сырые.</param>
        public Arguments(string[] args)
        {
            if (args.Length == 0)
            {
                throw new System.Exception("Вызов компилятора без параметров невозможен.");
            }

            foreach (string param in args) {
                switch (param)
                {
                    case "compile":
                        runMode = RunMode.Compile;
                        save = true;
                        run = false;
                        break;
                    case "runTests":
                        runMode = RunMode.Test;
                        break;
                    case "-save":
                        save = true;
                        break;
                    case "-norun":
                        run = false;
                        break;
                    case "-debug":
                        debug = true;
                        break;
                    case "-nologs":
                        logs = false;
                        break;
                    default:
                        if (File.Exists(param))
                        {
                            filePath = param;
                        }
                        else
                        {
                            string name = param.Split('=')[0];
                            string value = param.Split('=')[1];

                            if (name == "lang")
                            {
                                CodeManager.Lang = value.ToLower();
                            }
                        }
                        break;
                }
            }
            if (filePath == "" && runMode != RunMode.Test)
            {
                throw new System.Exception("Запуск компилятора без файла невозможен.");
            }
        }
    }
}
