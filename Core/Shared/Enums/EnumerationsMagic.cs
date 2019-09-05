using System;

namespace Shared.Enums
{
    [Flags]
    public enum ProcessAccessRights
    {
        DELETE = 0x00010000,
        READ_CONTROL = 0x00020000,
        WRITE_DAC = 0x00040000,
        WRITE_OWNER = 0x00080000,
        SYNCHRONIZE = 0x00100000,
        STANDARD_RIGHTS_ALL = 0x000F0000,
        PROCESS_TERMINATE = 0x0001,
        PROCESS_CREATE_THREAD = 0x0002,
        PROCESS_VM_OPERATION = 0x0008,
        PROCESS_VM_READ = 0x0010,
        PROCESS_VM_WRITE = 0x0020,
        PROCESS_DUP_HANDLE = 0x0040,
        PROCESS_CREATE_PROCESS = 0x0080,
        PROCESS_SET_QUOTA = 0x0100,
        PROCESS_SET_INFORMATION = 0x0200,
        PROCESS_QUERY_INFORMATION = 0x0400,
        PROCESS_SUSPEND_RESUME = 0x0800,
        PROCESS_QUERY_LIMITED_INFORMATION = 0x1000,
        PROCESS_ALL_ACCESS = STANDARD_RIGHTS_ALL | SYNCHRONIZE | 0xFFF,
    }

    [Flags]
    public enum ThreadAccessRights
    {
        DELETE = 0x00010000,
        READ_CONTROL = 0x00020000,
        WRITE_DAC = 0x00040000,
        WRITE_OWNER = 0x00080000,
        SYNCHRONIZE = 0x00100000,
        STANDARD_RIGHTS_ALL = 0x000F0000,
        THREAD_TERMINATE = 0x0001,
        THREAD_SUSPEND_RESUME = 0x0002,
        THREAD_GET_CONTEXT = 0x0008,
        THREAD_SET_CONTEXT = 0x0010,
        THREAD_SET_INFORMATION = 0x0020,
        THREAD_QUERY_INFORMATION = 0x0040,
        THREAD_SET_THREAD_TOKEN = 0x0080,
        THREAD_IMPERSONATE = 0x0100,
        THREAD_DIRECT_IMPERSONATION = 0x0200,
        THREAD_ALL_ACCESS = STANDARD_RIGHTS_ALL | SYNCHRONIZE | 0x3FF,
    }

    public enum MemoryProtectionType
    {
        PAGE_NOACCESS = 0x01,
        PAGE_READONLY = 0x02,
        PAGE_READWRITE = 0x04,
        PAGE_WRITECOPY = 0x08,
        PAGE_EXECUTE = 0x10,
        PAGE_EXECUTE_READ = 0x20,
        PAGE_EXECUTE_READWRITE = 0x40,
        PAGE_EXECUTE_WRITECOPY = 0x80,
        PAGE_GUARD = 0x100,
        PAGE_NOCACHE = 0x200,
        PAGE_WRITECOMBINE = 0x400,
    }

    public enum MemoryAllocationState
    {
        MEM_COMMIT = 0x00001000,
        MEM_RESERVE = 0x00002000,
        MEM_RESET = 0x00080000,
        MEM_PHYSICAL = 0x00400000,
        MEM_TOP_DOWN = 0x00100000,
        MEM_LARGE_PAGES = 0x20000000,
    }

    public enum MemoryAllocationType
    {
        MEM_PRIVATE = 0x0020000,
        MEM_MAPPED = 0x0040000,
        MEM_IMAGE = 0x1000000,
    }

    [Flags]
    public enum MemoryFreeType
    {
        MEM_DECOMMIT = 0x4000,
        MEM_RELEASE = 0x8000,
    }
}