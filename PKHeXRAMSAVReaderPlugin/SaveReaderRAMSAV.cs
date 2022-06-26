﻿using System;
using static System.Buffers.Binary.BinaryPrimitives;
using System.IO;
using PKHeX.Core;

namespace PKHeXRAMSAVReaderPlugin
{
    /// <summary>
    /// Logic for recognizing ramsav.bin files dumped via old ram2sav exploits (Gen 6).
    /// </summary>
    public class SaveReaderRAMSAV : ISaveReader
    {
        private const int SIZEXY = 0x70000;
        private const int SIZEAO = 0x80000;

        private const string FILENAME = "ramsav.bin";

        public bool IsRecognized(int size) => size is SIZEXY or SIZEAO;

        public SaveHandlerSplitResult TrySplit(byte[] input)
        {
            return new SaveHandlerSplitResult(GetSAV(input), Array.Empty<byte>(), Array.Empty<byte>());
        }

        public SaveFile ReadSaveFile(byte[] data, string path = null)
        {
            if (path is null || Path.GetFileName(path) is FILENAME)
            {
                return data.Length is SIZEXY ? new SAV6XY(GetSAV(data)) : data.Length is SIZEAO ? new SAV6AO(GetSAV(data)) : null;
            }

            return null;
        }

        // https://github.com/kwsch/PKHeX/blob/6ab7ba4bc1f9cb45f3617ce3202e63a0cf9928db/Misc/ram2sav.cs
        private static byte[] GetSAV(ReadOnlySpan<byte> ramsav)
        {
            byte[] main;
            uint[] offsets;
            uint[] lens;
            uint[] magics;
            uint[] skips;
            uint[] instances;
            uint val = ReadUInt32LittleEndian(ramsav);
            uint spos = 0;

            if (ramsav.Length == SIZEAO) // ORAS
            {
                offsets = new uint[] { 0x00005400, 0x00005800, 0x00006400, 0x00006600, 0x00006800, 0x00006A00, 0x00006C00, 0x00006E00, 0x00007000, 0x00007200, 0x00007400, 0x00009600, 0x00009800, 0x00009E00, 0x0000A400, 0x0000F400, 0x00014400, 0x00019400, 0x00019600, 0x00019E00, 0x0001A400, 0x0001B600, 0x0001BE00, 0x0001C000, 0x0001C200, 0x0001C800, 0x0001CA00, 0x0001CE00, 0x0001D600, 0x0001D800, 0x0001DA00, 0x0001DC00, 0x0001DE00, 0x0001E000, 0x0001E800, 0x0001EE00, 0x0001F200, 0x00020E00, 0x00021000, 0x00021400, 0x00021800, 0x00022000, 0x00023C00, 0x00024000, 0x00024800, 0x00024C00, 0x00025600, 0x00025A00, 0x00026200, 0x00027000, 0x00027200, 0x00027400, 0x00028200, 0x00028A00, 0x00028E00, 0x00030A00, 0x00038400, 0x0006D000, 0x0007B200 };
                lens = new uint[] { 0x000002C8, 0x00000B90, 0x0000002C, 0x00000038, 0x00000150, 0x00000004, 0x00000008, 0x000001C0, 0x000000BE, 0x00000024, 0x00002100, 0x00000130, 0x00000440, 0x00000574, 0x00004E28, 0x00004E28, 0x00004E28, 0x00000170, 0x0000061C, 0x00000504, 0x000011CC, 0x00000644, 0x00000104, 0x00000004, 0x00000420, 0x00000064, 0x000003F0, 0x0000070C, 0x00000180, 0x00000004, 0x0000000C, 0x00000048, 0x00000054, 0x00000644, 0x000005C8, 0x000002F8, 0x00001B40, 0x000001F4, 0x000003E0, 0x00000216, 0x00000640, 0x00001A90, 0x00000400, 0x00000618, 0x0000025C, 0x00000834, 0x00000318, 0x000007D0, 0x00000C48, 0x00000078, 0x00000200, 0x00000C84, 0x00000628, 0x00000400, 0x00007AD0, 0x000078B0, 0x00034AD0, 0x0000E058, 0x00000200 };
                skips = new uint[] { 0x00000000, 0x00000000, 0x00000000, 0x00000004, 0x00000000, 0x00000000, 0x00000000, 0x00000004, 0x00000000, 0x00000000, 0x00000000, 0x00000004, 0x00000000, 0x00000000, 0x00000004, 0x00000004, 0x00000004, 0x00000004, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000004, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000004, 0x00000000, 0x00000000, 0x00000004, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000004, 0x00000004, 0x00000000, 0x00000004, 0x00000004 };
                magics = new uint[] { 0x005E0BB8, 0x005E09C4, 0x005E0904, 0x005E0AA4, 0x005E0BF8, 0x005E05CC, 0x005E0B04, 0x005E0A44, 0x005E0AC4, 0x005E05AC, 0x005E064C, 0x005E0944, 0x005E0924, 0x005E0B78, 0x005E0884, 0x005E0884, 0x005E0884, 0x005E0AE4, 0x005E068C, 0x005DF40C, 0x005E0C18, 0x005E072C, 0x005E05EC, 0x005E0964, 0x005E06AC, 0x005E06EC, 0x005E062C, 0x005E0BD8, 0x005E0A64, 0x005E0B98, 0x005E07CC, 0x005E0A24, 0x005E058C, 0x005E04EC, 0x005E056C, 0x005E082C, 0x005E0984, 0x005E070C, 0x005E0B58, 0x005E054C, 0x005E050C, 0x005E080C, 0x005E076C, 0x005E07AC, 0x005E09E4, 0x005E08A4, 0x005E078C, 0x005E074C, 0x005E060C, 0x005E06CC, 0x96696996, 0x005E0864, 0x005E07EC, 0x005E0A04, 0x005E052C, 0x005E08C4, 0x005E04CC, 0x005E066C, 0x005E09A4 };
                instances = new uint[] { 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000 };
            }
            else // XY
            {
                offsets = new uint[] { 0x00005400, 0x00005800, 0x00006400, 0x00006600, 0x00006800, 0x00006A00, 0x00006C00, 0x00006E00, 0x00007000, 0x00007200, 0x00007400, 0x00009600, 0x00009800, 0x00009E00, 0x0000A400, 0x0000F400, 0x00014400, 0x00019400, 0x00019600, 0x00019E00, 0x0001A400, 0x0001AC00, 0x0001B400, 0x0001B600, 0x0001B800, 0x0001BE00, 0x0001C000, 0x0001C400, 0x0001CC00, 0x0001CE00, 0x0001D000, 0x0001D200, 0x0001D400, 0x0001D600, 0x0001DE00, 0x0001E400, 0x0001E800, 0x00020400, 0x00020600, 0x00020800, 0x00020C00, 0x00021000, 0x00022C00, 0x00023000, 0x00023800, 0x00023C00, 0x00024600, 0x00024A00, 0x00025200, 0x00026000, 0x00026200, 0x00026400, 0x00027200, 0x00027A00, 0x0005C600, 0x0006A800 };
                lens = new uint[] { 0x000002C8, 0x00000B88, 0x0000002C, 0x00000038, 0x00000150, 0x00000004, 0x00000008, 0x000001C0, 0x000000BE, 0x00000024, 0x00002100, 0x00000140, 0x00000440, 0x00000574, 0x00004E28, 0x00004E28, 0x00004E28, 0x00000170, 0x0000061C, 0x00000504, 0x000006A0, 0x00000644, 0x00000104, 0x00000004, 0x00000420, 0x00000064, 0x000003F0, 0x0000070C, 0x00000180, 0x00000004, 0x0000000C, 0x00000048, 0x00000054, 0x00000644, 0x000005C8, 0x000002F8, 0x00001B40, 0x000001F4, 0x000001F0, 0x00000216, 0x00000390, 0x00001A90, 0x00000308, 0x00000618, 0x0000025C, 0x00000834, 0x00000318, 0x000007D0, 0x00000C48, 0x00000078, 0x00000200, 0x00000C84, 0x00000628, 0x00034AD0, 0x0000E058, 0x00000200 };
                skips = new uint[] { 0x00000000, 0x00000000, 0x00000000, 0x00000004, 0x00000000, 0x00000000, 0x00000000, 0x00000004, 0x00000000, 0x00000000, 0x00000000, 0x00000004, 0x00000000, 0x00000000, 0x00000004, 0x00000004, 0x00000004, 0x00000004, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000004, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000004, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000004, 0x00000004 };
                magics = new uint[] { 0x0059AE94, 0x0059ACC0, 0x0059AC00, 0x0059AD80, 0x0059AED4, 0x0059A8E8, 0x0059ADE0, 0x0059AD20, 0x0059ADA0, 0x0059A8C8, 0x0059A968, 0x0059AC40, 0x0059AC20, 0x0059AE54, 0x0059ABA0, 0x0059ABA0, 0x0059ABA0, 0x0059ADC0, 0x0059A9A8, 0x00599734, 0x0059AEF4, 0x0059AA48, 0x0059A908, 0x0059AC60, 0x0059A9C8, 0x0059AA08, 0x0059A948, 0x0059AEB4, 0x0059AD40, 0x0059AE74, 0x0059AAE8, 0x0059AD00, 0x0059A8A8, 0x0059A800, 0x0059A888, 0x0059AB70, 0x0059AC80, 0x0059AA28, 0x0059AE34, 0x0059A868, 0x0059A820, 0x0059AB28, 0x0059AA88, 0x0059AAC8, 0x0059ACE0, 0x0059ABC0, 0x0059AAA8, 0x0059AA68, 0x0059A928, 0x0059A9E8, 0x0059AD60, 0x0059AB80, 0x0059AB08, 0x0059A7E0, 0x0059A988, 0x0059ACA0 };
                instances = new uint[] { 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000 };
                
                switch (val)
                {
                    case 0x0059E418:
                        magics = new uint[] { 0x0059E418, 0x0059E244, 0x0059E184, 0x0059E304, 0x0059E458, 0x0059DE6C, 0x0059E364, 0x0059E2A4, 0x0059E324, 0x0059DE4C, 0x0059DEEC, 0x0059E1C4, 0x0059E1A4, 0x0059E3D8, 0x0059E124, 0x0059E124, 0x0059E124, 0x0059E344, 0x0059DF2C, 0x0059CCB8, 0x0059E478, 0x0059DFCC, 0x0059DE8C, 0x0059E1E4, 0x0059DF4C, 0x0059DF8C, 0x0059DECC, 0x0059E438, 0x0059E2C4, 0x0059E3F8, 0x0059E06C, 0x0059E284, 0x0059DE2C, 0x0059DD84, 0x0059DE0C, 0x0059E0CC, 0x0059E204, 0x0059DFAC, 0x0059E3B8, 0x0059DDEC, 0x0059DDA4, 0x0059E0AC, 0x0059E00C, 0x0059E04C, 0x0059E264, 0x0059E144, 0x0059E02C, 0x0059DFEC, 0x0059DEAC, 0x0059DF6C, 0x0059E2E4, 0x0059E104, 0x0059E08C, 0x0059DD64, 0x0059DF0C, 0x0059E224 };
                        break;
                    case 0x0059E408:
                        magics = new uint[] { 0x0059E408, 0x0059E234, 0x0059E174, 0x0059E2F4, 0x0059E448, 0x0059DE5C, 0x0059E354, 0x0059E294, 0x0059E314, 0x0059DE3C, 0x0059DEDC, 0x0059E1B4, 0x0059E194, 0x0059E3C8, 0x0059E114, 0x0059E114, 0x0059E114, 0x0059E334, 0x0059DF1C, 0x0059CCA8, 0x0059E468, 0x0059DFBC, 0x0059DE7C, 0x0059E1D4, 0x0059DF3C, 0x0059DF7C, 0x0059DEBC, 0x0059E428, 0x0059E2B4, 0x0059E3E8, 0x0059E05C, 0x0059E274, 0x0059DE1C, 0x0059DD74, 0x0059DDFC, 0x0059E0BC, 0x0059E1F4, 0x0059DF9C, 0x0059E3A8, 0x0059DDDC, 0x0059DD94, 0x0059E09C, 0x0059DFFC, 0x0059E03C, 0x0059E254, 0x0059E134, 0x0059E01C, 0x0059DFDC, 0x0059DE9C, 0x0059DF5C, 0x0059E2D4, 0x0059E0F4, 0x0059E07C, 0x0059DD54, 0x0059DEFC, 0x0059E214 };
                        break;
                    case 0x0059BEC4:
                        magics = new uint[] { 0x0059BEC4, 0x0059BCF0, 0x0059BC30, 0x0059BDB0, 0x0059BF04, 0x0059B918, 0x0059BE10, 0x0059BD50, 0x0059BDD0, 0x0059B8F8, 0x0059B998, 0x0059BC70, 0x0059BC50, 0x0059BE84, 0x0059BBD0, 0x0059BBD0, 0x0059BBD0, 0x0059BDF0, 0x0059B9D8, 0x0059A764, 0x0059BF24, 0x0059BA78, 0x0059B938, 0x0059BC90, 0x0059B9F8, 0x0059BA38, 0x0059B978, 0x0059BEE4, 0x0059BD70, 0x0059BEA4, 0x0059BB18, 0x0059BD30, 0x0059B8D8, 0x0059B830, 0x0059B8B8, 0x0059BBA0, 0x0059BCB0, 0x0059BA58, 0x0059BE64, 0x0059B898, 0x0059B850, 0x0059BB58, 0x0059BAB8, 0x0059BAF8, 0x0059BD10, 0x0059BBF0, 0x0059BAD8, 0x0059BA98, 0x0059B958, 0x0059BA18, 0x0059BD90, 0x0059BBB0, 0x0059BB38, 0x0059B810, 0x0059B9B8, 0x0059BCD0 };
                        break;
                }
            }

            using (MemoryStream ms = new())
            {
                for (int i = 0; i < lens.Length; i++)
                {
                    int ofs = FindIndex(ramsav, magics[i], instances[i], spos);
                    _ = ms.Seek(offsets[i] - 0x5400, SeekOrigin.Begin);
                    spos += lens[i];

                    if (ofs > 0)
                    {
                        int start = ofs + (int)skips[i];
                        int end = start + (int)lens[i];
                        new MemoryStream(ramsav[start..end].ToArray()).CopyTo(ms);
                        spos += 4;
                    }
                    else
                    {
                        new MemoryStream(new byte[(int)lens[i]]).CopyTo(ms);
                    }
                }

                if (ms.Length % 0x200 != 0)
                    new MemoryStream(new byte[0x200 - ms.Length % 0x200]).CopyTo(ms);

                main = ms.ToArray();
            }
            return main;
        }

        private static int FindIndex(ReadOnlySpan<byte> data, uint val, uint instances, uint start)
        {
            if (val == 0x96696996)
                return -1;

            int ofs = (int)start;
            int times = 0;
            uint v = ReadUInt32LittleEndian(data[ofs..]);

            while ((v != val || times != instances) && ofs + 4 < data.Length)
            {
                ofs++;
                if (v == val)
                    times++;
                v = ReadUInt32LittleEndian(data[ofs..]);
            }

            if (ofs + 4 != data.Length)
                return ofs + 4;

            Console.WriteLine("Failed to find " + val.ToString("X8"));
            return -1;
        }
    }
}