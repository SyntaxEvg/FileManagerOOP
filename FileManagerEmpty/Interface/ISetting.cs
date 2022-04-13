namespace Data
{
    public interface ISetting
    {
        JsonSerWrite GetSettingConfig();
        void SaveSettingsFile(JsonSerWrite js);
    }
}