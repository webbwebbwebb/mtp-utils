namespace ConsoleApplication1
{
    public interface IDeviceManager
    {
        Device GetDeviceByName(string friendlyName);
    }
}