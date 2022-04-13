using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.Json;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using static FileManagerEmpty.RegexHelp;
using Data;
using System.Runtime.InteropServices;

namespace FileManagerEmpty
{

    //Функции и требования
    //Просмотр файловой структуры+
    //Поддержка копирование файлов, каталогов+
    //Поддержка удаление файлов, каталогов+
    //В конфигурационном файле должна быть настройка вывода количества элементов на страницу+
    //При выходе должно сохраняться, последнее состояние+
    //Должны быть комментарии в коде+
    //Должна быть документация к проекту в формате md+
    //Приложение должно обрабатывать непредвиденные ситуации (не падать)+
    //При успешном выполнение предыдущих пунктов – реализовать сохранение ошибки в текстовом файле в каталоге errors/random_name_exception.txt
    //При успешном выполнение предыдущих пунктов – реализовать движение по истории команд (стрелочки вверх, вниз)
    internal partial class Program
    {
        public static Dictionary<string, string> collectionHelp = new Dictionary<string, string>();
        static readonly string stringa = new String('=', 30);
        public delegate void OnKey(ConsoleKeyInfo key);
        public event OnKey KeyPr;
        static string SelectFolder = "C:/";
        public static int Paging = 0;
        static ILogger logger;
        static Commads commads =new Commads();
        //}

        /// <summary>
        /// Требуется создать консольный файловый менеджер начального уровня, который покрывает минимальный набор функционала по работе с файлами.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {

            ////Expression<Func<int, int, int>> divExpr =(a, b) => a / b;
            ////var y =divExpr.Compile();
            ////var tg = y(100, 4);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Title = "FILE Manager " + Environment.Version.ToString();
            logger = new Logger();
            var set = new Setting(logger);
            var GetSettin = set.GetSettingConfig();
                
            if (set is not null)
                SelectFolder = GetSettin.Folder.Length >0 ? GetSettin.Folder : SelectFolder; //грузим последнюю папку            
            logger.InitLogFile();
            DrawInterface();

        }



        /// <summary>
        /// вывод названия файлов или папок 
        /// </summary>
        /// <returns></returns>
        public static bool Menu()
        {
            Console_CursorPos(0, 3);
            var ViewModelFile = Pagination[Paging];
            int SelectItemIndex = 0; 
            foreach (var item in ViewModelFile)
            {
                int ind = item.Key;
                if (ind == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Back...");
                }
                if (ind == commads.SelectItem)
                {
                    //Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(item.Value);//выводим название файлов или папок 
                    
                }
                else
                {
                    //Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(item.Value);//выводим название файлов или папок 
                }
                SelectItemIndex++;
                //Console.WriteLine(item.Value);//выводим название файлов или папок 
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console_CursorPos(0, 15);
            Console.WriteLine($"Page {Paging} of {Pagination.Count - 1}");
            Console.WriteLine(stringa);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console_CursorPos(3, 23);
            return true;
        }



        /// <summary>
        /// Перерисовка интерфейса после ввода команд
        /// </summary>
        private static void DrawInterface()
        {
            while (true)
            {
                //Task.Run(() => ManagementKey());

                Console.Clear();
                //рисовка и заполнение
                Console_CursorPos(0, 0);
                Console.WriteLine(stringa);
                //тут  метод для показа файлов и папок
                DirectoryAndFolders();
                Console.WriteLine("Информация о выбранном");
                Console_CursorPos(0, 18);
                Console.WriteLine(stringa);
                DefoultInfo();//по дефолту
                Console.WriteLine(stringa);
                Console.WriteLine("Ввведите нужную команду");
                Console_CursorPos(0, 23);//коор команды ввода
                InputCommand();              
                Console.WriteLine(stringa);
                Console.WriteLine("Поддерживаемые команды");
                Console.WriteLine(stringa);
                Help.ConsoleHelp();
                Console.WriteLine(stringa);
                //сюда сдвиг курсора  для ввода 
                Console_CursorPos(3, 23);

                while (true)//ожидаем вводы от юзера
                {
                    if (!commads.CommandsUser())
                    {
                        break;
                    }

                }
            }

        }
        /// <summary>
        /// позиция курсора для ввода инфы
        /// </summary>
        public static void Render()
        {
            Console_CursorPos(0, 23);
            Console.Write(">> ");
            Console_CursorPos(3, 23);
        }




        /// <summary>
        /// Вывод помощи на весь экран
        /// </summary>
        /// <param name="flag"></param>
        public static void HelpInfo(bool flag = false)
        {
            Console.Clear();
            Help.ConsoleHelp();
            if (flag)
            {
                Console.WriteLine("Для закрытие помощи,нажмите любую клавищу");
                Console.ReadKey(true);
            }
        }

        /// <summary>
        /// Выводим информацию о файла
        /// </summary>
        /// <param name="pathInput"></param>
        /// <param name="comm"></param>
        /// <returns></returns>
        public static bool OutFile(ref string pathInput, ref string comm)
        {
            if (pathInput is null)
            {
                var pathN = comm.Replace("file ", "", StringComparison.OrdinalIgnoreCase);
                if (!Path.HasExtension(pathN))// папка 
                {
                    OutInform(ref pathN, true);//true -РАБ  С ПАПКОЙ
                }
                else
                {
                    OutInform(ref pathN, false);
                }

            }
            var result = Path.HasExtension(pathInput);
            if (!result && pathInput != null) // папка 
            {
                OutInform(ref pathInput, true);//true -РАБ С ПАПКОЙ
            }
            else if (pathInput != null)//файл 
            {
                OutInform(ref pathInput, false);
            }
            return true;//означ.  что не пер. интрерфейс
        }
        /// <summary>
        /// Вывод инфо о File or Folder
        /// </summary>
        /// <param name="pathInput"></param>
        /// <param name="v"></param>
        private static void OutInform(ref string pathInput, bool v)
        {
            if (v)//work folder
            {
                GetDirectoryInfo(ref pathInput);
                return;
            }
            
            GetFileInfo(ref pathInput);

        }
        /// <summary>
        /// Удаление File or Folder
        /// </summary>
        /// <param name="pathInput"></param>
        public static void DeleteDirectoryOrFile(ref string pathInput)
        {
            var result = Path.HasExtension(pathInput);
            if (!result && pathInput != null) // папка 
            {
                if (Directory.Exists(pathInput))
                {
                    DeleteDirectory(pathInput);
                }
                else
                {
                    Console.Write($"{pathInput} не существует Нажмите любую клавишу");
                    Console.ReadKey();
                    Console.Clear();
                    return;
                }
            }
            else if (pathInput != null)//файл
            {
                if (File.Exists(pathInput))
                {
                    DeleteFile(pathInput);
                }
            }
            Console.Write($"Null, не существует Нажмите любую клавишу");
            Console.ReadKey();
            Console.Clear();




        }


       
        /// <summary>
        /// Создать копию папки или файла
        /// </summary>
        /// <param name="pathInput"></param>
        /// <param name="pathOutput"></param>
        /// <returns></returns>
        public static bool CopyDirectoryOrFiles(ref string pathInput, ref string pathOutput)
        {
            var result = Path.HasExtension(pathInput);
            if (!result) // папка 
            {
                if (!Directory.Exists(pathOutput))
                {
                    Directory.CreateDirectory(pathOutput);
                }
                DirectoryInfo dir = new DirectoryInfo(pathInput);
                DirectoryInfo[] dirs = dir.GetDirectories();
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string tempPath = Path.Combine(pathInput, file.Name);
                    try
                    {
                        file.CopyTo(tempPath, false);
                    }
                    catch (Exception ex)
                    {
                        Console.Write($"Ошибка при копировании файла {file.Name} (Нажмите любую клавишу)");
                        logger.WriteLog(ref ex);
                        Console.ReadKey();
                    }
                }
            }
            else//file
            {
                File_Copy(pathInput, pathOutput);
            }
            return true;
            
        }

        /// <summary>
        /// Скопировать Файл 
        /// </summary>
        /// <param name="pathInput"></param>
        /// <param name="pathOutput"></param>
        private static void File_Copy(string pathInput, string pathOutput)
        {
            try
            {
                if (File.Exists(pathInput))
                {
                    File.Copy(pathInput, pathOutput);
                    Console.Write("Копирование успешно");
                }
            }
            catch (Exception e)
            {
                Console.Write($"При копировании произошла ошибка");
                logger.WriteLog(ref e);
                Console.ReadKey();
            }
        }
        /// <summary>
        /// Вывод инфо о Директории 
        /// </summary>
        /// <param name="selectFolder"></param>
        /// <param name="pagg"></param>
        /// <returns></returns>
        public static bool Directory_Output(ref string selectFolder, int pagg)
        {
            if (selectFolder != null && Directory.Exists(selectFolder))//еще одна провера папки, вдруг юзер удалил ее =)
            {
                SelectFolder = selectFolder;
                Paging = pagg > -1 && pagg < int.MaxValue ? pagg : 0;
                //
               
                return true;
            }
            return false;
        }


       

       
        /// <summary>
        /// Ввод команды 
        /// </summary>
        private static void InputCommand()
        {
            Console.Write(">> ");
            //потом найти эту позицию  и поставить сюда курсор  для ввода мышки
            // Console.WriteLine();
            Console_CursorPos(0, 23);//коор команды ввода 4 строки достаточно
            Console_CursorPos(0, 27);
        }

        private static void DefoultInfo()
        {
            var currentDirectory = SelectFolder;
            GetDirectoryInfo(ref currentDirectory);
        }
        /// <summary>
        /// Задать координаты кастомным методом )))
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        public static void Console_CursorPos(int v1, int v2)
        {
            Console.CursorLeft = v1;//по x строке
            Console.CursorTop = v2;//по y столбцу
        }

        public static Dictionary<int, List<KeyValuePair<int, string>>> Pagination = new();
        /// <summary>
        ///Вывод содержимого 
        /// </summary>
        private static void DirectoryAndFolders()
        {         
            var path = SelectFolder;//выбранная папка
            if (!Directory.Exists(path))//так как мы грузим эту папку из  текстового файла,проверим ее на валидность 
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"{path} Folder no Found");
                return;     
                         
            }
            var PageMaxSetting = Paging; //выбранная страница
            var set =new Setting(logger);
            var checkFild =set.GetSettingConfig();//грузим из  файла кол-во разрешен вывод страниц
            var pageLines = 8;//дефолт
            if (checkFild != null && checkFild.PageLines > 0)
            {
                pageLines = checkFild.PageLines;// кол-во выводимых файлов на страницу
            }
            //сохраним настройку в файл
            var saveSet = new Data.JsonSerWrite() { PageLines = pageLines, Folder =path };
            set.SaveSettingsFile(saveSet);
            //выводим инфу постранично
            Console.Write("Select [");
            Console.WriteLine(path + "]");
            Console.WriteLine(stringa);
            //создаем словарь с индексом элемента, для того чтобы потом по нему перемещаться 
            Dictionary<int, string> directoriesOrFiles = new();

            int indexFile = 0;
            var resultFolderAndFile = Directory.GetDirectories(path).
                Select(e => new DirectoryInfo(e)).
                OrderBy(ent => ent.CreationTime).
                Select(dir => new
                {
                    Inform = dir.Name
                   // Inform = dir.Name +"\t\t" + dir.CreationTime
                }).Union(Directory.GetFiles(path).
               Select(e => new FileInfo(e)).
               OrderBy(ent => ent.CreationTime).
               Select(dir => new
               {
                   Inform = dir.Name
                 //  Inform = dir.Name + "\t\t"  + dir.CreationTime
                  // Inform = dir.Name+ "\t" + dir.Attributes + "\t" + dir.CreationTime
               })).AsParallel().AsOrdered();

            foreach (var item in resultFolderAndFile)
            {
                directoriesOrFiles.Add(indexFile, item.Inform);
                indexFile++;
            }
            ////тоже самое делаем с файлами         
            //провер. допустимое заданное число  выводимых файлов на экран
            if (directoriesOrFiles.Count() > 0)
            {
                Pagination = new();
                //распределения файлов по разным страницам 
                int indexPathPagin = 0;
                int index = 0;
                foreach (var pathId in directoriesOrFiles)
                {
                    if (indexPathPagin < pageLines) //0<8
                    {
                        var t = KeyValuePair.Create(pathId.Key, pathId.Value);//Определяет пару "ключ-значение",                        
                        if (Pagination.ContainsKey(index))
                        {
                            Pagination[index].Add(t);
                        }
                        else
                        {
                            var t1 = KeyValuePair.Create(pathId.Key, pathId.Value);//Определяет пару "ключ-значение",
                            Pagination.Add(index, new List<KeyValuePair<int, string>>() { t1 });
                        }
                    }
                    else
                    {
                        index++;
                        var t = KeyValuePair.Create(pathId.Key, pathId.Value);//Определяет пару "ключ-значение",
                        Pagination.Add(index, new List<KeyValuePair<int, string>>() { t });
                        indexPathPagin = 0;
                    }
                    indexPathPagin++;
                }
                //выводим на экран нужную  заданную пагинацию человеком - p 5...
                if (Pagination.ContainsKey(Paging))
                {
                    var ViewModelFile = Pagination[Paging];

                    foreach (var item in ViewModelFile)
                    {
                        if (item.Key==0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Back...");
                        }
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(item.Value);//выводим название файлов или папок 
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console_CursorPos(0, 15);
                    Console.WriteLine($"Page {Paging} of {Pagination.Count - 1}");
                    Console.WriteLine(stringa);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Пагинация не доступна");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }


            }


        }


        


        /// <summary>
        /// Вывод информации о каталоге
        /// </summary>
        /// <param name="path"></param>
        static void GetDirectoryInfo(ref string path)
        {
            if (Directory.Exists(path))//еще раз  проверим вдруг удалена.. 
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                var directories = Directory.GetDirectories(path);
                var files = Directory.GetFiles(path);//делаем еще раз провер,  если вдруг  файл был удален в другом месте 
                Console_CursorPos(0, 18);
                Console.WriteLine($"Последний доступ к текущему : {directoryInfo.LastAccessTime} / ");
                Console.WriteLine($"Время последней операции записи: {directoryInfo.LastWriteTime}");
                Console.WriteLine($"Кол-во: {directories.Length} Folder and {files.Length} Files");
               
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Folder not Found");
            }
            Console.ForegroundColor = ConsoleColor.Yellow;//возращаем цвет

        }
        static void GetFileInfo(ref string path)
        {
            if (File.Exists(path))//еще раз  проверим вдруг удалена.. 
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                FileInfo fileInfo = new FileInfo(path);
                Console.Write($"File: {path}");
                Console.Write($"Last Access: {fileInfo.LastAccessTime}");
                Console.Write($"Last Write: {fileInfo.LastWriteTime}");
                Console.Write($"Length: {fileInfo.Length} / ");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("File not Found");
            }
            Console.ForegroundColor = ConsoleColor.Yellow;//возращаем цвет

        }


        /// <summary>
        /// получение размера файла или каталога
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static long GetSize(string path)
        {
            if (Directory.Exists(path))
            {
                string[] subDirectories;
                subDirectories = Directory.GetDirectories(path);
                var subFiles = Directory.GetFiles(path);
                long size = 0;

                if (subDirectories.Length != 0)
                {
                    foreach (var file in subFiles)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        size += fileInfo.Length;
                    }
                    foreach (var directory in subDirectories)
                    {
                        size += GetSize(directory);
                    }
                }
                else
                {
                    foreach (var file in subFiles)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        size += fileInfo.Length;
                    }
                }
                return size;
            }
            else if (File.Exists(path))
            {
                FileInfo fileInfo = new FileInfo(path);
                return fileInfo.Length;
            }
            else
            {
                return 0;
            }

        }

         /// <summary>
         /// Удалить папку
         /// </summary>
         /// <param name="path"></param>
        static void DeleteDirectory(string path)
        {
            try
            {
                Directory.Delete(path, true);
                Console.Write("Удаление успешно");
            }
            catch (Exception e)
            {
                Console.Write($"Ошибка при удалении каталога: {path}");
                logger.WriteLog(ref e);
                Console.ReadKey();
            }
        }


        /// <summary>
        /// Удалить файл
        /// </summary>
        /// <param name="path"></param>
        static void DeleteFile(string path)
        {
            var tempName = new FileInfo(path).Name;
            try
            {

                File.Delete(path);
                Console.Write($"[{tempName}] Удален");
            }
            catch (Exception e)
            {
                Console.Write($"Ошибка при удалении файла: {tempName}");
                logger.WriteLog(ref e);
                Console.ReadKey();
            }
        }


        /// <summary>
        /// Скопировать папку
        /// </summary>
        /// <param name="pathFrom"></param>
        /// <param name="pathTo"></param>
        static void CopyDirectory(string pathFrom, string pathTo)
        {
            DirectoryInfo dir = new DirectoryInfo(pathFrom);
            DirectoryInfo[] dirs = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();
            Directory.CreateDirectory(pathTo);
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(pathTo, file.Name);
                try
                {
                    file.CopyTo(tempPath, false);
                }
                catch (Exception e)
                {
                    Console.Write($"Ошибка при копировании файла {file.Name} (Нажмите любую клавишу)");
                    logger.WriteLog(ref e);
                    Console.ReadKey();
                }
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(pathTo, subdir.Name);
                try
                {
                    CopyDirectory(subdir.FullName, tempPath);
                }
                catch (Exception e)
                {
                    Console.Write($"Ошибка при копировании директории {subdir.FullName} (Нажмите любую клавишу)");
                    logger.WriteLog(ref e);
                    Console.ReadKey();
                }
            }
        }
    }
}
