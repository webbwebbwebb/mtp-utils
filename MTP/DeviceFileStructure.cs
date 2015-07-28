using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using PortableDeviceApiLib;
using PortableDeviceConstants;
using PortableDeviceTypesLib;
using IPortableDeviceValues = PortableDeviceApiLib.IPortableDeviceValues;

namespace Mtp
{
    public class DeviceFileStructure
    {
        private IPortableDeviceContent _deviceContent;

        public DeviceFileStructure(ref IPortableDeviceContent deviceContent)
        {
            _deviceContent = deviceContent;
        }

        public DeviceFolder Root { get; private set; }

        public DeviceFolder GetOrCreateFolderObject(string folderPath)
        {
            // traverse down folder structure until we can't match any further
            string[] pathLevels = folderPath.Split(Path.DirectorySeparatorChar);
            if (Root == null)
            {
                Root = (DeviceFolder)Enumerate(ref _deviceContent, pathLevels, "DEVICE", null);
            }
            DeviceFolder deepestMatch = Root.FindDeepestMatch(pathLevels);

            var currentFolder = deepestMatch;
            // create any missing levels
            for (var i = deepestMatch.Depth + 1; i < pathLevels.Length; i++)
            {
                DeviceFolder newFolder = CreateNewFolder(pathLevels[i], currentFolder);
                currentFolder = newFolder;
            }
            
            return currentFolder;
        }

        public void Copy(FileInfo file, DeviceFolder destinationFolder)
        {
            Console.WriteLine(file.Name);
            IPortableDeviceValues values = GetRequiredPropertiesForContentType(file, destinationFolder.Id);

            PortableDeviceApiLib.IStream tempStream;
            uint optimalTransferSizeBytes = 0;
            _deviceContent.CreateObjectWithPropertiesAndData(values, out tempStream, ref optimalTransferSizeBytes, null);

            var targetStream = (System.Runtime.InteropServices.ComTypes.IStream)tempStream;

            try
            {
                using (var sourceStream = file.OpenRead())
                {
                    var buffer = new byte[optimalTransferSizeBytes];
                    int bytesRead;
                    do
                    {
                        bytesRead = sourceStream.Read(buffer, 0, (int)optimalTransferSizeBytes);
                        IntPtr pcbWritten = IntPtr.Zero;
                        targetStream.Write(buffer, bytesRead, pcbWritten);
                    } while (bytesRead > 0);
                }
                targetStream.Commit(0);
            }
            finally
            {
                Marshal.ReleaseComObject(tempStream);
            }
        }

        private DeviceFolder CreateNewFolder(string folderName, DeviceFolder parentFolder)
        {
            IPortableDeviceProperties properties;
            _deviceContent.Properties(out properties);

            if (parentFolder == null)
            {
                throw new ArgumentException("Cannot create folder at root level.");
            }

            var createFolderValues = new PortableDeviceValues() as IPortableDeviceValues;
            createFolderValues.SetStringValue(PortableDevicePKeys.WPD_OBJECT_PARENT_ID, parentFolder.Id);
            createFolderValues.SetStringValue(PortableDevicePKeys.WPD_OBJECT_NAME, folderName);
            createFolderValues.SetStringValue(PortableDevicePKeys.WPD_OBJECT_ORIGINAL_FILE_NAME, folderName);
            createFolderValues.SetGuidValue(PortableDevicePKeys.WPD_OBJECT_CONTENT_TYPE, PortableDeviceGuids.WPD_CONTENT_TYPE_FOLDER);
            createFolderValues.SetGuidValue(PortableDevicePKeys.WPD_OBJECT_FORMAT, PortableDeviceGuids.WPD_OBJECT_FORMAT_PROPERTIES_ONLY);

            string newFolderId = "";
            _deviceContent.CreateObjectWithPropertiesOnly(createFolderValues, ref newFolderId);

            var newFolder = new DeviceFolder
            {
                Id = newFolderId,
                Name = folderName,
                Parent = parentFolder
            };
            parentFolder.AddChild(newFolder);

            return newFolder;
        }

        private static DeviceFileObject Enumerate(ref IPortableDeviceContent deviceContent, string[] pathLevels, string currentObjectId, DeviceFolder parent)
        {
            IPortableDeviceProperties properties;
            deviceContent.Properties(out properties);

            var currentFileObject = DeviceFileObject.FromObjectId(currentObjectId, properties, parent);

            if (currentFileObject is DeviceFolder && pathLevels.Length > 0 && pathLevels[0].ToUpper() == currentFileObject.Name.ToUpper())
            {
                var currentFolder = currentFileObject as DeviceFolder;
                IEnumPortableDeviceObjectIDs pEnum;
                deviceContent.EnumObjects(0, currentObjectId, null, out pEnum);

                uint fetched = 0;
                do
                {
                    string childObjectId;
                    pEnum.Next(1, out childObjectId, ref fetched);

                    if (fetched > 0)
                    {
                        currentFolder.AddChild(Enumerate(ref deviceContent, pathLevels.Skip(1).ToArray(), childObjectId, currentFolder));
                    }
                } while (fetched > 0);
            }

            return currentFileObject;
        }

        private IPortableDeviceValues GetRequiredPropertiesForContentType(FileInfo file, string parentObjectId)
        {
            var values = (IPortableDeviceValues)new PortableDeviceValues();

            values.SetStringValue(ref PortableDevicePKeys.WPD_OBJECT_PARENT_ID, parentObjectId);

            values.SetUnsignedLargeIntegerValue(PortableDevicePKeys.WPD_OBJECT_SIZE, (ulong)file.Length);
            values.SetStringValue(PortableDevicePKeys.WPD_OBJECT_ORIGINAL_FILE_NAME, Path.GetFileName(file.Name));
            values.SetStringValue(PortableDevicePKeys.WPD_OBJECT_NAME, Path.GetFileName(file.Name));
            values.SetBoolValue(PortableDevicePKeys.WPD_OBJECT_CAN_DELETE, 1);

            return values;
        }
    }
}