
namespace FileManagerEmpty
{
    public interface ILogger
    { 
        void InitLogFile();
        void WriteLog(ref Exception ex);
    }
}