using PKHeX.Core;

namespace PKHeXRAMSAVReaderPlugin
{
    public class RAMSAVReaderPlugin : IPlugin
    {
        public string Name => "RAMSAV Reader";
        public int Priority => 1000;

        public ISaveFileProvider SaveFileEditor => null;

        public void Initialize(params object[] args)
        {
            SaveUtil.CustomSaveReaders.Add(new SaveReaderRAMSAV());
        }

        public void NotifySaveLoaded() { }
        public bool TryLoadFile(string filePath) { return false; }
    }
}
