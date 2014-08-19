using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DesktopOrganizer.Utils
{
    public class Advapi32
    {
        private const uint STANDARD_RIGHTS_READ = 0x00020000;
        private const uint TOKEN_QUERY = 0x0008;
        private const uint TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);

        public static bool IsRunningAsAdministrator(Process process)
        {
            IntPtr token_handle;
            if (!OpenProcessToken(process.Handle, TOKEN_READ, out token_handle))
                throw new ApplicationException("Could not get process token.  Win32 Error Code: " + Marshal.GetLastWin32Error());

            var elevation_result = TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault;
            var elevation_result_size = Marshal.SizeOf((int)elevation_result);
            var elevation_type_ptr = Marshal.AllocHGlobal(elevation_result_size);
            uint returned_size;

            var success = GetTokenInformation(token_handle, TOKEN_INFORMATION_CLASS.TokenElevationType, elevation_type_ptr, (uint)elevation_result_size, out returned_size);
            if (!success)
                throw new ApplicationException("Unable to determine the current elevation.");

            elevation_result = (TOKEN_ELEVATION_TYPE)Marshal.ReadInt32(elevation_type_ptr);
            var is_process_admin = elevation_result == TOKEN_ELEVATION_TYPE.TokenElevationTypeFull;
            return is_process_admin;
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool GetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, IntPtr TokenInformation, uint TokenInformationLength, out uint ReturnLength);

        private enum TOKEN_INFORMATION_CLASS
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin,
            TokenElevationType,
            TokenLinkedToken,
            TokenElevation,
            TokenHasRestrictions,
            TokenAccessInformation,
            TokenVirtualizationAllowed,
            TokenVirtualizationEnabled,
            TokenIntegrityLevel,
            TokenUIAccess,
            TokenMandatoryPolicy,
            TokenLogonSid,
            MaxTokenInfoClass
        }

        private enum TOKEN_ELEVATION_TYPE
        {
            TokenElevationTypeDefault = 1,
            TokenElevationTypeFull,
            TokenElevationTypeLimited
        }
    }
}
