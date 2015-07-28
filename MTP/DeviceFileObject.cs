using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PortableDeviceApiLib;
using PortableDeviceConstants;

namespace Mtp
{
    public abstract class DeviceFileObject
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DeviceFileObject Parent { get; set; }

        public abstract bool IsContainer { get; }
        public abstract IEnumerable<DeviceFileObject> Contents { get; }

        public int Depth
        {
            get 
            {
                if (Parent == null)
                {
                    return 0;
                }

                return Parent.Depth + 1;
            }
        }

        public string FullPath
        {
            get { return PathToRoot(Parent) + Path.DirectorySeparatorChar + Name; }
        }

        
        public DeviceFolder FindDeepestMatch(string[] pathLevels)
        {
            if (pathLevels == null || pathLevels.Length == 0 || string.Compare(Name, pathLevels[0], true) != 0)
            {
                throw new ArgumentException("could not match beginning of path");
            }

            return FindDeepestMatchRecursive(pathLevels.Skip(1).ToArray());
        }

        private DeviceFolder FindDeepestMatchRecursive(string[] pathLevels)
        {
            if (pathLevels == null || pathLevels.Length == 0 || Contents == null)
            {
                return this as DeviceFolder;
            }

            var deeperMatch = Contents.SingleOrDefault(x => x.IsContainer && string.Compare(x.Name, pathLevels[0], true) == 0);

            if (deeperMatch == null)
            {
                return this as DeviceFolder;
            }

            return deeperMatch.FindDeepestMatchRecursive(pathLevels.Skip(1).ToArray());
        }

        private static string PathToRoot(DeviceFileObject fileObject)
        {
            if (fileObject == null)
            {
                return string.Empty;
            }

            return PathToRoot(fileObject.Parent) + Path.DirectorySeparatorChar + fileObject.Name;
        }

        public static DeviceFileObject FromObjectId(string objectId, IPortableDeviceProperties properties, DeviceFolder parent)
        {
            IPortableDeviceKeyCollection keys;
            properties.GetSupportedProperties(objectId, out keys);

            IPortableDeviceValues values;
            properties.GetValues(objectId, keys, out values);

            string name;

            Guid objectType;
            values.GetGuidValue(PortableDevicePKeys.WPD_OBJECT_CONTENT_TYPE, out objectType);

            if (objectType == PortableDeviceGuids.WPD_CONTENT_TYPE_FOLDER || objectType == PortableDeviceGuids.WPD_CONTENT_TYPE_FUNCTIONAL_OBJECT)
            {
                values.GetStringValue(PortableDevicePKeys.WPD_OBJECT_NAME, out name);
                return new DeviceFolder { Id = objectId, Name = name, Parent = parent };
            }

            values.GetStringValue(PortableDevicePKeys.WPD_OBJECT_ORIGINAL_FILE_NAME, out name);
            return new DeviceFile { Id = objectId, Name = name, Parent = parent };
        }
    }
}