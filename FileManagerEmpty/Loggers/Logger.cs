using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FileManagerEmpty
{
    public class Logger :  BaseSetting, ILogger
    {
        readonly string PuthFolderErrorLog;
        readonly string PuthLogger;
        static object Lock = new object();
        public readonly string ErrorsLogFile = "random_name_exception.txt";

        public bool LoggerWrite { get;  set; } =true;

        public Logger()
        {
            this.PuthFolderErrorLog= base.GetCurrentDirectory + "/errors/";
            this.PuthLogger = Path.Combine(PuthFolderErrorLog, ErrorsLogFile);
        }


        /// <summary>
        /// Инициалиця логера
        /// </summary>
        public  void InitLogFile()
        {

            try
            {
                if (!Directory.Exists(PuthFolderErrorLog))
                {
                    Directory.CreateDirectory(PuthFolderErrorLog);
                }
                if (!File.Exists(PuthLogger))
                {
                    File.Create(PuthLogger);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex} Логер не будет вести журнал");
                LoggerWrite = false;
            }
        }

        public  void WriteLog(ref Exception ex)
        {
            if (!LoggerWrite)
            {
                return; //логер сломан и записи не ведет
            }
            if (File.Exists(PuthLogger))
            {
                lock (Lock)
                {
                    try
                    {
                        var jsonString = JsonSerializer.Serialize(ex.Message);
                        File.WriteAllText(Path.Combine(PuthLogger, PuthLogger), jsonString + Environment.NewLine);
                    }
                    catch
                    {
                        Console.Write($"Ошибка записи в файл {PuthLogger}");
                    }
                }
            }
        }
    }
}
