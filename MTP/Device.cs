using System.IO;
using System.Linq;
using PortableDeviceApiLib;
using PortableDeviceConstants;
using PortableDeviceTypesLib;
using IPortableDeviceValues = PortableDeviceApiLib.IPortableDeviceValues;

namespace Mtp
{
    public class Device
    {
        private readonly PortableDeviceClass _portableDeviceClass;

        public string Id { get; set; }
        public string Name { get; set; }

        public Device(PortableDeviceClass portableDeviceClass)
        {
            _portableDeviceClass = portableDeviceClass;
        }

        public void Connect()
        {
            var pValues = (IPortableDeviceValues)new PortableDeviceValuesClass();
            pValues.SetStringValue(ref PortableDevicePKeys.WPD_CLIENT_NAME, "mtp.exe");
            pValues.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_CLIENT_MAJOR_VERSION, 1);
            pValues.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_CLIENT_MINOR_VERSION, 0);
            pValues.SetUnsignedIntegerValue(ref PortableDevicePKeys.WPD_CLIENT_REVISION, 2);

            _portableDeviceClass.Open(Id, pValues);
        }

        public void Copy(string path, string searchPattern, string destinationPath)
        {
            IPortableDeviceContent content;
            _portableDeviceClass.Content(out content);

            var filePaths = Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
            if (filePaths.Length > 0)
            {
                var deviceFileStructure = new DeviceFileStructure(ref content);
                DeviceFolder destinationFolder = deviceFileStructure.GetOrCreateFolderObject(destinationPath);

                foreach (var filePath in filePaths)
                {
                    var fileToCopy = new FileInfo(filePath);
                    if (destinationFolder.Contents.All(c => c.Name != fileToCopy.Name))
                    {
                        deviceFileStructure.Copy(fileToCopy, destinationFolder);     
                    }
                }
            }
        }

        public void Disconnect()
        {
            _portableDeviceClass.Close();
        }
    }
}