namespace ConsoleApplication1
{
    public class Configuration : IConfiguration
    {
        // exe 0    1        2
        // mtp copy <source> <destination>
        public Configuration(string[] args)
        {
            Action = args[0].ToUpper();
            SourceFilePath = args[1].ParseFilePath();
            SearchPattern = args[1].ParseSearchPattern();
            Destination = args[2];
            DeviceName = Destination.ParseDeviceName();
        }

        public string Action { get; private set; }
        public string SourceFilePath { get; private set; }
        public string SearchPattern { get; private set; }
        public string Destination { get; private set; }
        public string DeviceName { get; private set; }
    }
}