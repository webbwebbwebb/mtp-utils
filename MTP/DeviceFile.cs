using System.Collections.Generic;

namespace Mtp
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