namespace Mtp
{
    public interface IDeviceManager
    {
        Device GetDeviceByName(string friendlyName);
    }
}