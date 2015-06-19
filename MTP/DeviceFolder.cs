using System.Collections.Generic;

namespace ConsoleApplication1
{
    public class DeviceFolder : DeviceFileObject
    {
        private readonly List<DeviceFileObject> _contents;

        public DeviceFolder()
        {
            _contents = new List<DeviceFileObject>();
        }

        public override bool IsContainer
        {
            get { return true; }
        }

        public override IEnumerable<DeviceFileObject> Contents
        {
            get { return _contents; }
        }

        public void AddChild(DeviceFileObject child)
        {
            if (child != null)
            {
                _contents.Add(child);
                child.Parent = this;
            }
        }
    }
}