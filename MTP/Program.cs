using System;
using PortableDeviceApiLib;

namespace Mtp
{
    public class Program
    {
        static int Main(string[] args)
        {
            int errorCode = 0;
            try
            {
                IConfiguration config = new Configuration(args);

                IDeviceManager deviceManager = new DeviceManager(new PortableDeviceManagerClass());

                var device = deviceManager.GetDeviceByName(config.DeviceName);
                if (device == null)
                {
                    throw new Exception(string.Format("Could not connect to {0}", config.DeviceName));
                }

                if (config.Action == "COPY")
                {
                    try
                    {
                        device.Connect();
                        device.Copy(config.SourceFilePath, config.SearchPattern, config.Destination);
                    }
                    finally
                    {
                        device.Disconnect();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                errorCode = -1;
            }

            return errorCode;
        }
    }
}
