using System;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace Shared.SomeMagic
{
    [HostProtection(MayLeakOnAbort = true)]
    [SuppressUnmanagedCodeSecurity]
    public sealed class SafeMemoryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeMemoryHandle() : base(true) { }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        public SafeMemoryHandle(IntPtr handle) : base(true) => SetHandle(handle);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle() => handle != IntPtr.Zero && Imports.CloseHandle(handle);
    }
}