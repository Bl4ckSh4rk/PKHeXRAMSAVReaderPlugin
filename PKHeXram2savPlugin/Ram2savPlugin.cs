using System;
using System.IO;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeXRAM2SAVPlugin
{
    public class RAM2SAVPlugin : IPlugin
    {
        public string Name => "ram2sav Plugin";
        public int Priority => 1; // Loading order, lowest is first.
        public ISaveFileProvider SaveFileEditor { get; private set; }
        public IPKMView PKMEditor { get; private set; }

        public void Initialize(params object[] args)
        {
            Console.WriteLine($"Loading {Name}...");
            if (args == null)
                return;
            SaveFileEditor = (ISaveFileProvider)Array.Find(args, z => z is ISaveFileProvider);
            PKMEditor = (IPKMView)Array.Find(args, z => z is IPKMView);
            var menu = (ToolStrip)Array.Find(args, z => z is ToolStrip);
            LoadMenuStrip(menu);
        }

        private void LoadMenuStrip(ToolStrip menuStrip)
        {
            var items = menuStrip.Items;
            var tools = items.Find("Menu_Tools", false)[0] as ToolStripDropDownItem;
            AddPluginControl(tools);
        }

        private void AddPluginControl(ToolStripDropDownItem tools)
        {
            var ctrl = new ToolStripMenuItem(Name);
            tools.DropDownItems.Add(ctrl);

            var c2 = new ToolStripMenuItem($"Export PK6 files");
            c2.Click += new EventHandler(ExportPKM);
            var c3 = new ToolStripMenuItem($"Export pcdata.bin");
            c3.Click += new EventHandler(ExportPcdata);

            ctrl.DropDownItems.Add(c2);
            ctrl.DropDownItems.Add(c3);
        }

        private void ExportPcdata(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Open ramsav.bin";
                ofd.Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*";

                if (ofd.ShowDialog() != DialogResult.OK) return;
                byte[] input = File.ReadAllBytes(ofd.FileName);

                if (Path.GetFileName(ofd.FileName).Contains("ramsav"))
                {
                    SAV6 sav = null;

                    if (input.Length == 0x70000)
                        sav = new SAV6XY(RAM2SAV.GetMAIN(input));
                    else if (input.Length == 0x80000)
                        sav = new SAV6AO(RAM2SAV.GetMAIN(input));

                    if (sav != null)
                    {
                        using (SaveFileDialog sfd = new SaveFileDialog())
                        {
                            sfd.Title = "Save pcdata.bin";
                            sfd.Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*";
                            sfd.FileName = "ramsav_pcdata.bin";

                            if (sfd.ShowDialog() != DialogResult.OK) return;
                            File.WriteAllBytes(sfd.FileName,sav.GetPCBinary());
                        }
                    }
                }
            }
        }

        private void ExportPKM(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Open ramsav.bin";
                ofd.Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*";

                if (ofd.ShowDialog() != DialogResult.OK) return;
                byte[] input = File.ReadAllBytes(ofd.FileName);

                if (Path.GetFileName(ofd.FileName).Contains("ramsav"))
                {
                    SAV6 sav = null;

                    if (input.Length == 0x70000)
                        sav = new SAV6XY(RAM2SAV.GetMAIN(input));
                    else if (input.Length == 0x80000)
                        sav = new SAV6AO(RAM2SAV.GetMAIN(input));

                    if (sav != null)
                    {
                        using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                        {
                            if (fbd.ShowDialog() != DialogResult.OK) return;
                            sav.DumpBoxes(fbd.SelectedPath);

                            for(int i = 0; i < sav.PartyCount; i++)
                            {
                                PKM pkm = sav.GetPartySlotAtIndex(i);
                                string filename = $"{fbd.SelectedPath}\\{pkm.FileName}";

                                File.WriteAllBytes(filename,pkm.Data);
                            }
                        }
                    }
                }
            }
        }

        public void NotifySaveLoaded()
        {
            Console.WriteLine($"{Name} was notified that a Save File was just loaded.");
        }
        public bool TryLoadFile(string filePath)
        {
            Console.WriteLine($"{Name} was provided with the file path, but chose to do nothing with it.");
            return false; // no action taken
        }
    }
}
