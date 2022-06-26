using System;
using PKHeX.Core;

namespace PKHeXRAMSAVReaderPlugin
{
    public class RAMSAVReaderPlugin : IPlugin
    {
        public string Name => "RAMSAV Reader";
        public int Priority => 1000;

        public ISaveFileProvider SaveFileEditor => throw new NotImplementedException();

        public void Initialize(params object[] args)
        {
            Console.WriteLine($"Loading {Name}...");

            SaveUtil.CustomSaveReaders.Add(new SaveReaderRAMSAV());
        }

        public void NotifySaveLoaded()
        {
            Console.WriteLine($"{Name} was notified that a Save File was just loaded.");
        }

        public bool TryLoadFile(string filePath)
        {
            Console.WriteLine($"{Name} was provided with the file path, but chose to do nothing with it.");
            return false;
        }
    }
}
