using System.Collections.Generic;

namespace ConsoleApplication1
{
    public class DeviceFile : DeviceFileObject
    {
        public override bool IsContainer
        {
            get { return false; }
        }

        public override IEnumerable<DeviceFileObject> Contents
        {
            get
            {
                return null;
            }
        }
    }
}