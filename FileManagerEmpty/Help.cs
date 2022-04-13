namespace FileManagerEmpty
{

    internal partial class Program
    {
        public static class Help
        {
            public static void ConsoleHelp()
            {
                //
                Console.WriteLine();
                if (!collectionHelp.ContainsKey("ls"))
                {
                    collectionHelp.Add("ls", "Вывод дерева файловой системы с условием “пейджинга”\nПараметр -p, пример ls C:\\Source -p 2");
                    collectionHelp.Add("cp", "Копирование каталога,пример C:\\Source D:\\Target или C:\\source.txt D:\\target.txt");
                    collectionHelp.Add("rm", "Удаление каталога рекурсивно/Файла, пример rm C:\\Source или rm C:\\source.txt");
                    collectionHelp.Add("file", "Вывод информации");
                    collectionHelp.Add("Help", "Все доступные команды,пример file C:\\source.txt");
                }
                foreach (var item in collectionHelp)
                {
                    Console.WriteLine(item);
                }

            }
        }





    }
}
