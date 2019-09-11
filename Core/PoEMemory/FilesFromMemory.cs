using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Interfaces;
using GameOffsets;
using MoreLinq;

namespace ExileCore.PoEMemory
{
    public struct FileInformation
    {
        public FileInformation(long ptr, int changeCount, int test1, int test2)
        {
            Ptr = ptr;
            ChangeCount = changeCount;
            Test1 = test1;
            Test2 = test2;
        }

        public long Ptr { get; }
        public int ChangeCount { get; }
        public int Test1 { get; }
        public int Test2 { get; }
    }

    public class FilesFromMemory
    {
        private readonly IMemory mem;

        public FilesFromMemory(IMemory memory)
        {
            mem = memory;
        }

        public Dictionary<string, FileInformation> GetAllFiles()
        {
            var files = new ConcurrentDictionary<string, FileInformation>();
            var fileRoot = mem.AddressOfProcess + mem.BaseOffsets[OffsetsName.FileRoot];
            var start = mem.Read<long>(fileRoot + 0x8);

            var filesPointer = mem.ReadListPointer(new IntPtr(start));

            Parallel.ForEach(filesPointer, p =>
            {
                var filesOffsets = mem.Read<FilesOffsets>(p);
                var advancedInformation = mem.Read<GameOffsets.FileInformation>(filesOffsets.MoreInformation);
                if (advancedInformation.String.buf == 0) return;
                /*var str = RemoteMemoryObject.Cache.StringCache.Read($"{nameof(FilesFromMemory)}{advancedInformation.String.buf}",
                                                                    () => advancedInformation.String.ToString(mem));*/
                var str = advancedInformation.String.ToString(mem);

                if (str.Length <= 0) return;

                files.TryAdd(
                    str,
                    new FileInformation(filesOffsets.MoreInformation, advancedInformation.AreaCount, advancedInformation.Test1,
                        advancedInformation.Test2));
            });

            return files.ToDictionary();
        }

        public Dictionary<string, FileInformation> GetAllFilesSync()
        {
            var files = new Dictionary<string, FileInformation>();
            var fileRoot = mem.AddressOfProcess + mem.BaseOffsets[OffsetsName.FileRoot];
            var start = mem.Read<long>(fileRoot + 0x8);
            var filesPointer = mem.ReadListPointer(new IntPtr(start));

            foreach (var p in filesPointer)
            {
                var filesOffsets = mem.Read<FilesOffsets>(p);
                var advancedInformation = mem.Read<GameOffsets.FileInformation>(filesOffsets.MoreInformation);
                if (advancedInformation.String.buf == 0) continue;

                var str = RemoteMemoryObject.Cache.StringCache.Read($"{nameof(FilesFromMemory)}{advancedInformation.String.buf}",
                    () => advancedInformation.String.ToString(mem));

                if (str.Length <= 0) continue;

                files[str] = new FileInformation(filesOffsets.MoreInformation, advancedInformation.AreaCount, advancedInformation.Test1,
                    advancedInformation.Test2);
            }

            return files;
        }
    }
}
