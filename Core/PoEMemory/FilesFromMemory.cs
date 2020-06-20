using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Interfaces;
using GameOffsets;
using GameOffsets.Native;
using MoreLinq;

namespace ExileCore.PoEMemory
{
    public struct FileInformation
    {
        public FileInformation(long ptr, int changeCount)
        {
            Ptr = ptr;
            ChangeCount = changeCount;
        }

        public long Ptr { get; }
        public int ChangeCount { get; }
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

            Parallel.For(0, 256, (i) =>
            {
                var readAddress = fileRoot + i * 0x40;
                var fileChunkStruct = mem.Read<FilesOffsets>(readAddress);

                ReadDictionary(fileChunkStruct.ListPtr, files);
            });
            
            return files.ToDictionary();
        }

        public void ReadDictionary(long head, ConcurrentDictionary<string, FileInformation> dictionary)
        {
            var node = mem.Read<FileNode>(head);

            var sw = Stopwatch.StartNew();
            var headLong = head;

            while (headLong != node.Next)
            {
                if (sw.ElapsedMilliseconds > 2000)
                {
                    Core.Logger.Error($"ReadDictionary error. Elapsed: {sw.ElapsedMilliseconds}");
                    return;
                }

                var key = mem.ReadStringU(node.Key);

                if (dictionary.ContainsKey(key))
                    Core.Logger.Error($"ReadDictionary error. Already contains key: {key}. Value: {node.Value:X}");
                else
                {
                    var changeCount = mem.Read<int>(node.Value + 0x38);
                    dictionary[key] = new FileInformation(node.Value, changeCount);
                }

                node = mem.Read<FileNode>(node.Next);
            }
        }

        public Dictionary<string, FileInformation> GetAllFilesSync()
        {
            var files = new Dictionary<string, FileInformation>();
            throw new NotImplementedException();
            //var fileRoot = mem.AddressOfProcess + mem.BaseOffsets[OffsetsName.FileRoot];
            //var start = mem.Read<long>(fileRoot + 0x8);
            //var filesPointer = mem.ReadListPointer(new IntPtr(start));

            //foreach (var p in filesPointer)
            //{
            //    var filesOffsets = mem.Read<FilesOffsets>(p);
            //    var advancedInformation = mem.Read<GameOffsets.FileNode>(filesOffsets.MoreInformation);
            //    if (advancedInformation.String.buf == 0) continue;

            //    var str = RemoteMemoryObject.Cache.StringCache.Read(
            //        $"{nameof(FilesFromMemory)}{advancedInformation.String.buf}",
            //        () => advancedInformation.String.ToString(mem));

            //    if (str.Length <= 0) continue;

            //    files[str] = new FileInformation(filesOffsets.MoreInformation, advancedInformation.AreaCount,
            //        advancedInformation.Test1,
            //        advancedInformation.Test2);
            //}

            return files;
        }
    }
}