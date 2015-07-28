namespace Mtp
{
    public interface IConfiguration
    {
        string Action { get; }
        string SourceFilePath { get; }
        string SearchPattern { get; }
        string Destination { get; }
        string DeviceName { get; }
    }
}