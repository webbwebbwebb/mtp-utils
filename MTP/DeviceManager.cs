using System;
using System.Collections.Generic;
using System.Linq;
using PortableDeviceApiLib;

namespace ConsoleApplication1
{
    public class DeviceManager : IDeviceManager
    {
        private readonly PortableDeviceManagerClass _portableDeviceManagerClass;

        public DeviceManager(PortableDeviceManagerClass portableDeviceManagerClass)
        {
            _portableDeviceManagerClass = portableDeviceManagerClass;
        }

        public Device GetDeviceByName(string friendlyName)
        {
            var allDevices = GetAllDevices();
            return allDevices.SingleOrDefault(x => String.Compare(x.Name, friendlyName, System.StringComparison.OrdinalIgnoreCase) == 0);
        }

        private IList<Device> GetAllDevices()
        {
            _portableDeviceManagerClass.RefreshDeviceList();
            
            uint numberOfDevices = 0;
            _portableDeviceManagerClass.GetDevices(null, ref numberOfDevices);

            if (numberOfDevices == 0)
            {
                return new Device[0];
            }

            var deviceIds = new string[numberOfDevices];

            _portableDeviceManagerClass.GetDevices(deviceIds, ref numberOfDevices);
            var output = new List<Device>();
            foreach (var deviceId in deviceIds)
            {
                uint deviceFriendlyNameLength = 0;
                _portableDeviceManagerClass.GetDeviceFriendlyName(deviceId, null, ref deviceFriendlyNameLength);
                var pDeviceFriendlyName = new ushort[deviceFriendlyNameLength];
                _portableDeviceManagerClass.GetDeviceFriendlyName(deviceId, pDeviceFriendlyName, ref deviceFriendlyNameLength);

                output.Add(new Device(new PortableDeviceClass())
                {
                    Id = deviceId,
                    Name = pDeviceFriendlyName.ConvertToString().TrimNullTermination()
                });
            }

            return output;
        }
    }
}