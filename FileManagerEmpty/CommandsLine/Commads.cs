using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileManagerEmpty
{
    public class Commads
    {
        public  bool CommandsUser()
        {
            string comm = null;
            while (true)
            {
                (bool flag, string char1) = ManagementKey();
                if (flag)
                {
                    break;
                    return true;
                }
                if (!flag)
                {

                }
                comm = Console.ReadLine();
                comm = char1 + comm;
            }

            var Splitt = comm?.Split(' ');
            if (comm?.Length > 0 && Splitt?.Length > 0)
            {
                string res = ParseCommandLine(Splitt[0]);//опред только команду

                if (res is null)
                {
                    Console.WriteLine("Команда не найдена");
                    Program.Render();
                }
                else
                {
                    Console.WriteLine($"Выбрана работа с командой {res}");
                    if (Splitt.Length > 0)
                    {
                        bool IsExit = ParseArgumentLine(ref res, ref comm);
                        if (!IsExit)//выход из цикла если выбрана помощь на весь экран.. кто знает  может  там 1000 настроек  будет 
                        {
                            return false;
                        }
                        else
                        {
                            Program.Render();

                            return true; //перерисовываем
                        }
                    }
                    return true;
                }

            }
            return true;
        }

        public  int SelectItem = 0;
        public  (bool, string) ManagementKey()
        {
            bool Flag = Program.Pagination is not null;
            if (!Flag)
            {
                Program.Console_CursorPos(0, 3);
                return (true, "");
            }
            var CountMax = Program.Pagination?[Program.Paging].Count;
            while (true)
            {
                var char1 = Console.ReadKey(true).Key;
                switch (char1)
                {
                    case ConsoleKey.UpArrow:
                        {
                            if (SelectItem > 0)
                            {
                                SelectItem--;
                            }
                            return (Program.Menu(), "");
                            //Console.WriteLine("Вверх");
                        }
                    //case ConsoleKey.RightArrow:
                    //    {
                    //        break;  // Console.WriteLine("Right");
                    //    }
                    case ConsoleKey.DownArrow:
                        {
                            if (SelectItem > -1 && SelectItem < CountMax + 1)
                            {
                                SelectItem++;
                            }
                            return (Program.Menu(), "");
                            //Console.WriteLine("Вниз");

                        }
                    default:
                        {
                            var t = char1.ToString();
                            if (Regex.Match(t, "[a-zA-Z]").Success)
                            {
                                Program.Console_CursorPos(3, 23);
                                Console.Write(char1);
                                return (false, t);//если чел. пытается писать ,передаем его перевый символ в набор 
                            }
                            return (true, "");
                        }


                }
            }
        }
        /// <summary>
        /// Определяем тип команды поданной на вход
        /// </summary>
        /// <param name="comm"></param>
        /// <returns></returns>
        private  string ParseCommandLine(string comm)
        {
            foreach (var com in Program.collectionHelp)
            {
                var minimalC = comm.ToLower();
                if (minimalC.StartsWith(com.Key.ToLower()))//ищем команду 
                {
                    return com.Key;
                }
            }
            return null;
        }
        /// <summary>
        /// Оброботчик Команд
        /// </summary>
        /// <param name="res"></param>
        /// <param name="comm"></param>
        /// <returns></returns>
        private  bool ParseArgumentLine(ref string res, ref string comm)
        {

            (string PathInput, string PathOutput, int pag) = CheckFolderAndFiles(ref res, ref comm);

            switch (res)
            {
                case "ls": return Program.Directory_Output(ref PathInput, pag);
                case "cp": return Program.CopyDirectoryOrFiles(ref PathInput, ref PathOutput);
                case "rm": Program.DeleteDirectoryOrFile(ref PathInput); break;
                case "file": return Program.OutFile(ref PathInput, ref comm);
                case "Help": Program.HelpInfo(true); return false;
                default: Console.WriteLine("BagCommand"); break;
            }
            return true;
        }
        /// <summary>
        /// Проверка регуляркой папки, китайский не поодерживается но можно легко добавить...
        /// </summary>
        /// <param name="res"></param>
        /// <param name="comm"></param>
        /// <returns></returns>
        private (string, string, int) CheckFolderAndFiles(ref string res, ref string comm)
        {
            //универ.метод определения есть ли папка или файл простая проверка перед работой позволит  понять , были ли пробелы в названиях папки
            string puthInput = null;
            string puthOup = null;
            int pag = 0;
            bool ListorAll = res == "ls";
            //два вида регулярки в продакшен их можно поместить в статический класс и скопилировать сразу,
            //тут это делать не буду , чтобы была видна логика 
            Regex pattern;
            if (ListorAll)
            {
                pattern = RegexHelp.PatternPaggingComp;
            }
            else
            {
                pattern = RegexHelp.PatternAllComand;
            }
            var regex = pattern;
            var mathess = regex.Matches(comm);
            if (ListorAll && mathess != null && mathess.Count == 0)//user has not entered pagination 
            {
                regex = RegexHelp.NoPagging;
                mathess = regex.Matches(comm);
                foreach (Match item in mathess)
                {
                    var type = item.Value;
                    if (Directory.Exists(type))// пример ls C:\\Source -p 2"
                    {
                        puthInput = type;//она же дир.  изспользуется не по назначению 
                        return (puthInput, puthOup, pag);
                    }
                }
            }
            foreach (Match mathes in mathess)
            {
                var type = mathes.Value;
                if (ListorAll)
                {
                    int num;
                    var SplitDirandPagin = type.Split(" -p");
                    int.TryParse(SplitDirandPagin[1], out num);
                    if (num != 0)
                    {
                        pag = num;
                    }
                    if (Directory.Exists(SplitDirandPagin[0]))// пример ls C:\\Source -p 2"
                    {
                        puthInput = SplitDirandPagin[0];//она же дир  изспользуется не по назначению 
                        break;
                    }
                }
                else
                {
                    var SplitDirandPagin = type.TrimEnd().Split(" ");
                    if (SplitDirandPagin.Count() == 2)//хороший вариант деления 2исходный файла не содержат  пробелы и запрещающ. символы
                    {
                        puthInput = SplitDirandPagin[0];
                        puthOup = SplitDirandPagin[1];
                    }
                    else
                    {//плохой вариант  имя файла содержит  пробелы,проходим по символьно ищем делитель 
                        StringBuilder stringBuilder = new StringBuilder();
                        var t = ""; bool Dobor = false; string tempSymbol = null;
                        for (int i = 0; i < type.Length; i++)
                        {
                            if (type[i] != ' ' && !Dobor)//
                            {
                                if (tempSymbol != null)
                                {
                                    stringBuilder.Append(tempSymbol);
                                    tempSymbol = null;
                                }
                                stringBuilder.Append(type[i]);
                            }
                            else
                            {//"D:\Users\\De
                                if (Dobor && type[i] != ':')
                                {
                                    stringBuilder.Append(type[i]);
                                }
                                else
                                {
                                    if (type[i] == ':')
                                    {
                                        var pt = stringBuilder.ToString();
                                        if (puthInput is null)
                                        {
                                            tempSymbol = pt.Substring(pt.Length - 1);//запоминаем до след. итерации
                                            puthInput = pt.Remove(pt.Length - 1);
                                            stringBuilder.Clear();
                                            Dobor = false;
                                            continue;
                                        }
                                        else if (puthOup is null)
                                        {
                                            tempSymbol = pt.Substring(pt.Length - 1);
                                            puthOup = pt.Remove(pt.Length - 1);
                                            stringBuilder.Clear();
                                            Dobor = false;
                                            continue;
                                        }
                                    }
                                    stringBuilder.Append(' ');//добиваем пробел 
                                    Dobor = true;
                                }
                                //запоминаем позицию  последнего пробела 
                            }
                        }
                        if (stringBuilder.Length > 0 && puthOup is null)//после таких манипяляций остаются остатки 
                        {
                            var pt = stringBuilder.ToString();
                            tempSymbol = pt.Substring(pt.Length - 1);
                            puthOup = pt.Remove(pt.Length - 1);
                            stringBuilder.Clear();
                            Dobor = false;
                            continue;
                        }
                    }
                }
            }
            return (puthInput, puthOup, pag);
        }
    }
}
