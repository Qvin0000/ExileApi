using System.Collections.Generic;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory
{
    public class Offsets
    {
        public static Offsets Regular = new Offsets {IgsOffset = 0, IgsDelta = 0, ExeName = "PathOfExile_x64"};
        public static Offsets Korean = new Offsets {IgsOffset = 0, IgsDelta = 0, ExeName = "Pathofexile_x64_KG"};

        public static Offsets Steam = new Offsets {IgsOffset = 0x28, IgsDelta = 0, ExeName = "PathOfExile_x64Steam"};
        /*
        00007FF7006C7891  | 90                                 | nop                                        |
        00007FF7006C7892  | 48 8B 1D EF 93 06 01               | mov rbx,qword ptr ds:[7FF701730C88]        |
        00007FF7006C7899  | 48 89 05 E8 93 06 01               | mov qword ptr ds:[7FF701730C88],rax        |
        00007FF7006C78A0  | 48 85 DB                           | test rbx,rbx                               |
        00007FF7006C78A3  | 74 15                              | je pathofexile_x64.7FF7006C78BA            |
        00007FF7006C78A5  | 48 8B CB                           | mov rcx,rbx                                |
        00007FF7006C78A8  | E8 53 D7 00 00                     | call pathofexile_x64.7FF7006D5000          |
        */
        //    90 48 8B 1D ?? ?? ?? ?? 48 89 05 ?? ?? ?? ?? 48 85 DB 74 15 48 8B CB E8

        private static readonly Pattern basePtrPattern = new Pattern(
            new byte[]
            {
                0x90, 0x48, 0x03, 0xD8, 0x48, 0x8B, 0x03, 0x48, 0x85, 0xC0, 0x75, 0x00, 0x48, 0x8B, 0x1D, 0x00, 0x00, 0x00, 0x00, 0x48,
                0x8B, 0x05, 0x00, 0x00, 0x00, 0x00, 0x48, 0x85, 0xC0, 0X74, 0x00, 0x66, 0x90
            }, "xxxxxxxxxxx?xxx????xxx????xxxx?xx", "BasePtr");

        /* FileRoot Pointer
        00007FF6C47EED01  | 48 8D 0D A8 23 7F 00               | lea rcx,qword ptr ds:[7FF6C4FE10B0]        | <--FileRootPtr
        00007FF6C47EED08  | E8 E3 5C 56 FF                     | call pathofexile_x64.7FF6C3D549F0          |
        00007FF6C47EED0D  | 48 8B 3D A4 23 7F 00               | mov rdi,qword ptr ds:[7FF6C4FE10B8]        |
        00007FF6C47EED14  | 48 8B 1F                           | mov rbx,qword ptr ds:[rdi]                 |
        00007FF6C47EED17  | 48 3B DF                           | cmp rbx,rdi                                |
        00007FF6C47EED1A  | 0F 84 26 01 00 00                  | je pathofexile_x64.7FF6C47EEE46            |
        */
        // 3.3.x
        //    48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8B 3D ?? ?? ?? ?? 48 8B 1F

        private static readonly Pattern fileRootPattern =
            new Pattern(new byte[]
                {
                    0x48, 0x8D, 0x2D , 0x00 , 0x00, 0x00, 0x00 , 0x41 , 0x8B , 0x00 , 0x00 , 0x39
				}, "xxx????xx??x", "File Root",
                000000);

        /* Area Change
        00007FF63317CE40 | 48 83 EC 58                    | sub rsp,58                                      |
        00007FF63317CE44 | 4C 8B C1                       | mov r8,rcx                                      |
        00007FF63317CE47 | 41 B9 01 00 00 00              | mov r9d,1                                       |
        00007FF63317CE4D | 48 8B 49 10                    | mov rcx,qword ptr ds:[rcx+10]                   |
        00007FF63317CE51 | 48 89 4C 24 30                 | mov qword ptr ss:[rsp+30],rcx                   |
        00007FF63317CE56 | 48 85 C9                       | test rcx,rcx                                    |
        00007FF63317CE59 | 74 11                          | je pathofexile_x64 - alpha 2.5.7FF63317CE6C     |
        00007FF63317CE5B | 41 8B C1                       | mov eax,r9d                                     |
        00007FF63317CE5E | F0 0F C1 41 54                 | lock xadd dword ptr ds:[rcx+54],eax             |
        00007FF63317CE63 | 8B 05 7B 09 F0 00              | mov eax,dword ptr ds:[<AreaChangeCount>]        |
        00007FF63317CE69 | 89 41 50                       | mov dword ptr ds:[rcx+50],eax                   |
        00007FF63317CE6C | 49 8B 08                       | mov rcx,qword ptr ds:[r8]                       |
        00007FF63317CE6F | 49 8B 40 18                    | mov rax,qword ptr ds:[r8+18]                    |
        */
        // 3.0.3b
        //     48 83 EC 58 4C 8B C1 41 B9 01 00 00 00 48 8B 49 10

        private static readonly Pattern areaChangePattern =
            new Pattern(
                new byte[]
                {
                    0xE8, 0x00, 0x00, 0x00, 0x00, 0xE8, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x05
				}, "x????x????xx", "Area change", 9430000);

        /*
        PathOfExile_x64.exe+853E28 - 48 89 05 E9ABC400     - mov [PathOfExile_x64.exe+149EA18],rax { [00000000] }
        PathOfExile_x64.exe+853E2F - 48 8B 44 24 40        - mov rax,[rsp+40]
        PathOfExile_x64.exe+853E34 - 48 89 06              - mov [rsi],rax
        PathOfExile_x64.exe+853E37 - 48 8B C6              - mov rax,rsi
        PathOfExile_x64.exe+853E3A - 48 83 C4 20           - add rsp,20 { 32 }
        PathOfExile_x64.exe+853E3E - 5E                    - pop rsi
        PathOfExile_x64.exe+853E3F - C3                    - ret 
        */

        private static readonly Pattern isLoadingScreenPattern =
            new Pattern(
                new byte[] {0x48, 0x89, 0x05, 0x00, 0x00, 0x00, 0x00, 0x48, 0x8B, 0x00, 0x00, 0x00, 0x48, 0x89, 0x00, 0x48, 0x8B, 0xC6},
                "xxx????xx???xx?xxx", "Loading");

        private static readonly Pattern GameStatePattern = new Pattern(
            new byte[]
            {
                0x48, 0x83, 0xec, 0x50, 0x48, 0xc7, 0x44, 0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x48, 0x89, 0x9c, 0x24, 0x00, 0x00, 0x00, 0x00, 0x48,
                0x8b, 0xf9, 0x33, 0xed, 0x48, 0x39
            }, "xxxxxxxx?????xxxx????xxxxxxx", "Game State", 1240000);

        /*
        PathOfExile_x64.exe+118FD9 - 4C 8B 35 48255B01     - mov r14,[PathOfExile_x64.exe+16CB528] { [C6151734A0] }<<here
        PathOfExile_x64.exe+118FE0 - 4D 85 F6              - test r14,r14
        PathOfExile_x64.exe+118FE3 - 0F94 C0               - sete al
        PathOfExile_x64.exe+118FE6 - 84 C0                 - test al,al
        */
        public long AreaChangeCount { get; private set; }
        public long Base { get; private set; }
        public string ExeName { get; private set; }
        public long FileRoot { get; private set; }
        public int IgsDelta { get; private set; }
        public int IgsOffset { get; private set; }
        public int IgsOffsetDelta => IgsOffset + IgsDelta;
        public long isLoadingScreenOffset { get; private set; }
        public long GameStateOffset { get; private set; }

        public Dictionary<OffsetsName, long> DoPatternScans(IMemory m)
        {
            var array = m.FindPatterns( /*basePtrPattern,*/
                fileRootPattern, areaChangePattern, /* isLoadingScreenPattern,*/ GameStatePattern);

            var result = new Dictionary<OffsetsName, long>();

            //  System.Console.WriteLine("Base Pattern: " + (m.AddressOfProcess + array[0]).ToString("x8"));

            var index = 0;
            var baseAddress = m.Process.MainModule.BaseAddress.ToInt64();

            //  Base = m.Read<int>(baseAddress + array[index] + 0xF) + array[index] + 0x13; index++;
            //  System.Console.WriteLine("Base Address: " + (Base + m.AddressOfProcess).ToString("x8"));

            //  long InGameState = m.Read<long>(Base + BaseAddress, 0x8, 0xF8, 0x38);
            //  System.Console.WriteLine("InGameState: " + InGameState.ToString("x8"));

            FileRoot = m.Read<int>(baseAddress + array[index] + 0x3) + array[index] + 0x7;
            index++;

            //   System.Console.WriteLine("FileRoot Pointer: " + (FileRoot + m.AddressOfProcess).ToString("x8"));

            AreaChangeCount = m.Read<int>(baseAddress + array[index] + 0xC) + array[index] + 0x10;
            index++;

            // System.Console.WriteLine("AreaChangeCount: " + m.ReadInt(AreaChangeCount + m.AddressOfProcess).ToString());

            //    isLoadingScreenOffset = m.Read<int>(baseAddress + array[index] + 0x03) + array[index] + 0x07;
            //index++;
            // System.Console.WriteLine("Is Loading Screen Offset:" + (isLoadingScreenOffset + m.AddressOfProcess).ToString("x8"));

            GameStateOffset = m.Read<int>(baseAddress + array[index] + 29) + array[index] + 33;

            //  System.Console.WriteLine("Game State Offset:" + (GameStateOffset + m.AddressOfProcess).ToString("x8"));

            //  result.Add(OffsetsName.Base,Base);
            result.Add(OffsetsName.FileRoot, FileRoot);
            result.Add(OffsetsName.AreaChangeCount, AreaChangeCount);

            //   result.Add(OffsetsName.IsLoadingScreenOffset,isLoadingScreenOffset);
            result.Add(OffsetsName.GameStateOffset, GameStateOffset);
            return result;
        }
    }
}
