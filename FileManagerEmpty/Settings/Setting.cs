using FileManagerEmpty;
using System.Text.Json;

namespace Data
{
    public  class JsonSerWrite//можно любые параметры добавить ,в задании кол-во управлен.страницей требуется
    {
        public  int PageLines { get; set; } = 8;//  макс 8 из за обсобенности работы кривого интрефейса без поддержки контейнера
        public string Folder { get; set; }
    }

    public class Setting : BaseSetting, ISetting 
    {
       
        public readonly string Config = "Config.json";
        public readonly string path;
        ILogger logger;
        public Setting(ILogger logger)
        {
            this.logger = logger;            
        }
        public JsonSerWrite GetSettingConfig()
        {

            if (File.Exists(path))
            {
                try
                {
                    string jsonSettings = File.ReadAllText(path);
                    return JsonSerializer.Deserialize<JsonSerWrite>(jsonSettings)!;

                }
                catch (Exception e)
                {
                    Console.Write($"Ошибка при чтении настроек!");
                    this.logger.WriteLog(ref e);
                }
            }
            return null;
        }
        public void SaveSettingsFile(JsonSerWrite js)
        {
            var path = Path.Combine(base.GetCurrentDirectory, Config);
            string jsonSettings = JsonSerializer.Serialize(js);
            try
            {
                File.WriteAllText(path, jsonSettings);
            }
            catch (Exception e)
            {
                Console.Write("Ошибка при записи файла настроек, смотрите лог!");
                this.logger.WriteLog(ref e);
            }
        }
    }
}
