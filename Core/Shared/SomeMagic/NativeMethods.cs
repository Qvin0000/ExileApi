using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using ExileCore.Shared.Enums;

namespace ExileCore.Shared.SomeMagic
{
    public static unsafe class NativeMethods
    {
        public static bool LogError = false;

        public static SafeMemoryHandle OpenProcess(int pId, ProcessAccessRights accessRights = ProcessAccessRights.PROCESS_ALL_ACCESS)
        {
            var processHandle = Imports.OpenProcess(accessRights, false, pId);

            if (processHandle == null || processHandle.IsInvalid || processHandle.IsClosed)
            {
                throw new Win32Exception(
                    $"[Error Code: {Marshal.GetLastWin32Error()}] Unable to open process {pId} with access {accessRights:X}");
            }

            return processHandle;
        }

        public static int GetProcessId(SafeMemoryHandle processHandle)
        {
            var pId = Imports.GetProcessId(processHandle);

            if (pId == 0)
            {
                throw new Win32Exception(
                    $"[Error Code: {Marshal.GetLastWin32Error()}] Unable to get Id from process handle 0x{processHandle.DangerousGetHandle().ToString("X")}");
            }

            return pId;
        }

        public static bool Is64BitProcess(SafeMemoryHandle processHandle)
        {
            if (!Imports.IsWow64Process(processHandle, out var Is64BitProcess))
            {
                throw new Win32Exception(
                    $"[Error Code: {Marshal.GetLastWin32Error()}] Unable to determine if process handle 0x{processHandle.DangerousGetHandle().ToString("X")} is 64 bit");
            }

            return !Is64BitProcess;
        }

        public static string GetClassName(IntPtr windowHandle)
        {
            var stringBuilder = new StringBuilder(char.MaxValue);

            if (Imports.GetClassName(windowHandle, stringBuilder, stringBuilder.Capacity) == 0)
            {
                throw new Win32Exception(
                    $"[Error Code: {Marshal.GetLastWin32Error()}] Unable to get class name from window handle 0x{windowHandle.ToString("X")}");
            }

            return stringBuilder.ToString();
        }

        public static bool CloseHandle(IntPtr handle)
        {
            if (!Imports.CloseHandle(handle))
                throw new Win32Exception($"[Error Code: {Marshal.GetLastWin32Error()}] Unable to close handle 0x{handle.ToString("X")}");

            return true;
        }

        public static int ReadProcessMemory(SafeMemoryHandle processHandle, IntPtr address, [Out] byte[] buffer, int size)
        {
            if (!Imports.ReadProcessMemory(processHandle, address, buffer, size, out var bytesRead))
            {
                if (LogError)
                {
                    var finalError = new StringBuilder();

                    finalError.AppendLine(
                        $"[Error Code: {Marshal.GetLastWin32Error()}] Unable to read memory from 0x{address.ToString($"X{IntPtr.Size}")}[Size: {size}]");

                    var frames = new StackTrace(true).GetFrames();

                    if (frames != null)
                    {
                        for (var i = 1; i < Math.Min(frames.Length, 10); i++)
                        {
                            var stackFrame = frames[i];
                            finalError.Append(stackFrame);
                        }
                    }

                    Core.Logger.Error(finalError.ToString());
                }
            }

            return bytesRead;
        }

        public static int WriteProcessMemory(SafeMemoryHandle processHandle, IntPtr address, [Out] byte[] buffer, int size)
        {
            var bytesWritten = 0;

            if (!Imports.WriteProcessMemory(processHandle, address, buffer, size, out bytesWritten))
            {
                if (LogError)
                {
                    var finalError = new StringBuilder();

                    finalError.AppendLine(
                        $"[Error Code: {Marshal.GetLastWin32Error()}] Unable to write memory at 0x{address.ToString($"X{IntPtr.Size}")}[Size: {size}]");

                    var frames = new StackTrace(true).GetFrames();

                    for (var i = 1; i < Math.Min(frames.Length, 10); i++)
                    {
                        var stackFrame = frames[i];

                        finalError.AppendLine(
                            $"{stackFrame.GetFileName()} -> {stackFrame.GetMethod().Name}, line: {stackFrame.GetFileLineNumber()}");
                    }

                    Core.Logger.Error(finalError.ToString());
                }
            }

            return bytesWritten;
        }

        public static IntPtr Allocate([Optional] IntPtr address, int size,
            MemoryProtectionType protect = MemoryProtectionType.PAGE_EXECUTE_READWRITE)
        {
            var ret = Imports.VirtualAlloc(address, size, MemoryAllocationState.MEM_COMMIT, protect);

            if (ret.Equals(0))
            {
                throw new Win32Exception(string.Format("[Error Code: {0}] Unable to allocate memory at 0x{1}[Size: {2}]",
                    Marshal.GetLastWin32Error(), address.ToString($"X{IntPtr.Size}"), size));
            }

            return ret;
        }

        public static IntPtr Allocate(SafeMemoryHandle processHandle, [Optional] IntPtr address, int size,
            MemoryProtectionType protect = MemoryProtectionType.PAGE_EXECUTE_READWRITE)
        {
            var ret = Imports.VirtualAllocEx(processHandle, address, size, MemoryAllocationState.MEM_COMMIT, protect);

            if (ret.Equals(0))
            {
                throw new Win32Exception(string.Format(
                    "[Error Code: {0}] Unable to allocate memory to process handle 0x{1} at 0x{2}[Size: {3}]",
                    Marshal.GetLastWin32Error(), processHandle.DangerousGetHandle().ToString("X"),
                    address.ToString($"X{IntPtr.Size}"), size));
            }

            return ret;
        }

        public static bool Free(IntPtr address, int size = 0, MemoryFreeType free = MemoryFreeType.MEM_RELEASE)
        {
            if (!Imports.VirtualFree(address, size, free))
            {
                throw new Win32Exception(string.Format("[Error Code: {0}] Unable to free memory at 0x{1}[Size: {2}]",
                    Marshal.GetLastWin32Error(), address.ToString($"X{IntPtr.Size}"), size));
            }

            return true;
        }

        public static bool Free(SafeMemoryHandle processHandle, IntPtr address, int size = 0,
            MemoryFreeType free = MemoryFreeType.MEM_RELEASE)
        {
            if (!Imports.VirtualFreeEx(processHandle, address, size, free))
            {
                throw new Win32Exception(string.Format(
                    "[Error Code: {0}] Unable to free memory from process handle 0x{1} at 0x{2}[Size: {3}]",
                    Marshal.GetLastWin32Error(), processHandle.DangerousGetHandle().ToString("X"),
                    address.ToString($"X{IntPtr.Size}"), size));
            }

            return true;
        }

        public static void Copy(void* destination, void* source, int size)
        {
            try
            {
                Imports.MoveMemory(destination, source, size);
            }
            catch
            {
                throw new Win32Exception(string.Format("[Error Code: {0}] Unable to copy memory to {0} from {1}[Size: {2}]",
                    Marshal.GetLastWin32Error(), (*(ulong*) destination).ToString($"X{IntPtr.Size}"),
                    (*(ulong*) source).ToString($"X{IntPtr.Size} ({size})")));
            }
        }

        public static MemoryProtectionType ChangeMemoryProtection(IntPtr address, int size,
            MemoryProtectionType newProtect =
                MemoryProtectionType.PAGE_EXECUTE_READWRITE)
        {
            MemoryProtectionType oldProtect;

            if (!Imports.VirtualProtect(address, size, newProtect, out oldProtect))
            {
                throw new Win32Exception(
                    $"[Error Code: {Marshal.GetLastWin32Error()}] Unable to change memory protection at 0x{address.ToString($"X{IntPtr.Size}")}[Size: {size}] to {newProtect.ToString("X")}");
            }

            return oldProtect;
        }

        public static MemoryProtectionType ChangeMemoryProtection(SafeMemoryHandle processHandle, IntPtr address, int size,
            MemoryProtectionType newProtect =
                MemoryProtectionType.PAGE_EXECUTE_READWRITE)
        {
            MemoryProtectionType oldProtect;

            if (!Imports.VirtualProtectEx(processHandle, address, size, newProtect, out oldProtect))
            {
                throw new Win32Exception(
                    $"[Error Code: {Marshal.GetLastWin32Error()}] Unable to change memory protection of process handle 0x{processHandle.DangerousGetHandle().ToString("X")} at 0x{address.ToString($"X{IntPtr.Size}")}[Size: {size}] to {newProtect.ToString("X")}");
            }

            return oldProtect;
        }

        public static MemoryBasicInformation Query(IntPtr address, int size)
        {
            if (Imports.VirtualQuery(address, out var memInfo, size) == 0)
            {
                throw new Win32Exception(
                    $"[Error Code: {Marshal.GetLastWin32Error()}] Unable to retrieve memory information from 0x{address.ToString($"X{IntPtr.Size}")}[Size: {size}]");
            }

            return memInfo;
        }

        public static MemoryBasicInformation Query(SafeMemoryHandle processHandle, IntPtr address, int size)
        {
            if (Imports.VirtualQueryEx(processHandle, address, out var memInfo, size) == 0)
            {
                throw new Win32Exception(
                    $"[Error Code: {Marshal.GetLastWin32Error()}] Unable to retrieve memory information of process handle 0x{processHandle.DangerousGetHandle().ToString("X")} from 0x{address.ToString($"X{IntPtr.Size}")}[Size: {size}]");
            }

            return memInfo;
        }
    }
}
