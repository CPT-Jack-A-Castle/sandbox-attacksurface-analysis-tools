﻿//  Copyright 2016, 2017 Google Inc. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using NtApiDotNet.Win32.SafeHandles;
using NtApiDotNet.Win32.Security.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace NtApiDotNet.Win32
{
#pragma warning disable 1591
    /// <summary>
    /// Service trigger type.
    /// </summary>
    public enum ServiceTriggerType
    {
        DeviceInterfaceArrival = 1,
        IPAddressAvailability = 2,
        DomainJoin = 3,
        FirewallPortEvent = 4,
        GroupPolicy = 5,
        NetworkEndpoint = 6,
        CustomSystemStateChange = 7,
        Custom = 20,
        Aggregate = 30,
    }

    public enum ServiceTriggerDataType
    {
        Binary = 1,
        String = 2,
        Level = 3,
        KeywordAny = 4,
        KeywordAll = 5,
    }

    public enum ServiceTriggerAction
    {
        Start = 1,
        Stop = 2
    }

    public enum ServiceStatus
    {
        Stopped = 1,
        StartPending = 2,
        StopPending = 3,
        Running = 4,
        ContinuePending = 5,
        PausePending = 6,
        Paused = 7,
    }

    [Flags]
    public enum ServiceControlsAccepted
    {
        None = 0,
        Stop = 1,
        PauseContinue = 2,
        Shutdown = 4,
        ParamChange = 8,
        NetBindChange = 0x10,
        HardwareProfileChange = 0x20,
        PowerEvent = 0x40,
        SessionChange = 0x80,
        PreShutdown = 0x100,
        Timechange = 0x200,
        TriggerEvent = 0x400,
        UserLogoff = 0x800,
        Internal = 0x1000,
        LowResources = 0x2000,
        SystemLowResources = 0x4000
    }

    [Flags]
    public enum ServiceFlags
    {
        None = 0,
        RunsInSystemProcess
    }

    [Flags]
    public enum ServiceControlManagerAccessRights : uint
    {
        CreateService = 0x0002,
        Connect = 0x0001,
        EnumerateService = 0x0004,
        Lock = 0x0008,
        ModifyBootConfig = 0x0020,
        QueryLockStatus = 0x0010,
        All = CreateService | Connect | EnumerateService
            | Lock | ModifyBootConfig | QueryLockStatus | ReadControl
            | Delete | WriteDac | WriteOwner,
        GenericRead = GenericAccessRights.GenericRead,
        GenericWrite = GenericAccessRights.GenericWrite,
        GenericExecute = GenericAccessRights.GenericExecute,
        GenericAll = GenericAccessRights.GenericAll,
        Delete = GenericAccessRights.Delete,
        ReadControl = GenericAccessRights.ReadControl,
        WriteDac = GenericAccessRights.WriteDac,
        WriteOwner = GenericAccessRights.WriteOwner,
        Synchronize = GenericAccessRights.Synchronize,
        MaximumAllowed = GenericAccessRights.MaximumAllowed,
        AccessSystemSecurity = GenericAccessRights.AccessSystemSecurity
    }

    [Flags]
    public enum ServiceAccessRights : uint
    {
        ChangeConfig = 0x0002,
        EnumerateDependents = 0x0008,
        Interrogate = 0x0080,
        PauseContinue = 0x0040,
        QueryConfig = 0x0001,
        QueryStatus = 0x0004,
        Start = 0x0010,
        Stop = 0x0020,
        UserDefinedControl = 0x0100,
        SetStatus = 0x8000,
        All = ChangeConfig | EnumerateDependents | Interrogate | PauseContinue
            | QueryStatus | QueryConfig | Start | Stop | UserDefinedControl | ReadControl
            | Delete | WriteDac | WriteOwner,
        GenericRead = GenericAccessRights.GenericRead,
        GenericWrite = GenericAccessRights.GenericWrite,
        GenericExecute = GenericAccessRights.GenericExecute,
        GenericAll = GenericAccessRights.GenericAll,
        Delete = GenericAccessRights.Delete,
        ReadControl = GenericAccessRights.ReadControl,
        WriteDac = GenericAccessRights.WriteDac,
        WriteOwner = GenericAccessRights.WriteOwner,
        Synchronize = GenericAccessRights.Synchronize,
        MaximumAllowed = GenericAccessRights.MaximumAllowed,
        AccessSystemSecurity = GenericAccessRights.AccessSystemSecurity
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SERVICE_STATUS_PROCESS
    {
        public ServiceType dwServiceType;
        public ServiceStatus dwCurrentState;
        public ServiceControlsAccepted dwControlsAccepted;
        public Win32Error dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
        public int dwProcessId;
        public ServiceFlags dwServiceFlags;
    }

    internal enum SC_ENUM_TYPE
    {
        SC_ENUM_PROCESS_INFO = 0
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ENUM_SERVICE_STATUS_PROCESS
    {
        public IntPtr lpServiceName;
        public IntPtr lpDisplayName;
        public SERVICE_STATUS_PROCESS ServiceStatusProcess;
    }

    [Flags]
    public enum ServiceType
    {
        KernelDriver = 0x00000001,
        FileSystemDriver = 0x00000002,
        Adapter = 0x00000004,
        RecognizerDriver = 0x00000008,
        Driver = KernelDriver | FileSystemDriver | Adapter | RecognizerDriver,
        Win32OwnProcess = 0x00000010,
        Win32ShareProcess = 0x00000020,
        Win32 = Win32OwnProcess | Win32ShareProcess,
        UserService = 0x00000040,
        UserServiceInstance = 0x00000080,
        InteractiveProcess = 0x00000100
    }

    public enum ServiceState
    {
        All,
        Active,
        InActive
    }

    internal enum SC_STATUS_TYPE
    {
        SC_STATUS_PROCESS_INFO = 0
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SERVICE_TRIGGER_INFO
    {
        public int cTriggers;
        public IntPtr pTriggers;
        public IntPtr pReserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SERVICE_TRIGGER
    {
        public ServiceTriggerType dwTriggerType;
        public ServiceTriggerAction dwAction;
        public IntPtr pTriggerSubtype;
        public int cDataItems;
        public IntPtr pDataItems;

        public Guid GetSubType()
        {
            if (pTriggerSubtype != IntPtr.Zero)
            {
                return (Guid)Marshal.PtrToStructure(pTriggerSubtype, typeof(Guid));
            }
            return Guid.Empty;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SERVICE_TRIGGER_SPECIFIC_DATA_ITEM
    {
        public ServiceTriggerDataType dwDataType;
        public int cbData;
        public IntPtr pData;
    }

    public enum ServiceSidType
    {
        None = 0,
        Unrestricted = 1,
        Restricted = 3
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SERVICE_SID_INFO
    {
        public ServiceSidType dwServiceSidType;
    }

    public enum ServiceLaunchProtectedType
    {
        None = 0,
        Windows = 1,
        WindowsLight = 2,
        AntimalwareLight = 3,
        AppLight = 4,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SERVICE_LAUNCH_PROTECTED_INFO
    {
        public ServiceLaunchProtectedType dwLaunchProtected;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SERVICE_DELAYED_AUTO_START_INFO
    {
        [MarshalAs(UnmanagedType.Bool)]
        public bool fDelayedAutostart;
    }

    public enum ServiceStartType
    {
        Boot = 0,
        System = 1,
        Auto = 2,
        Demand = 3,
        Disabled = 4,
    }

    public enum ServiceErrorControl
    {
        Ignore = 0,
        Normal = 1,
        Severe = 2,
        Critical = 3
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct QUERY_SERVICE_CONFIG
    {
        public ServiceType dwServiceType;
        public ServiceStartType dwStartType;
        public ServiceErrorControl dwErrorControl;
        public IntPtr lpBinaryPathName;
        public IntPtr lpLoadOrderGroup;
        public int dwTagId;
        public IntPtr lpDependencies;
        public IntPtr lpServiceStartName;
        public IntPtr lpDisplayName;
    }

    internal class ServiceFakeTypeFactory : NtFakeTypeFactory
    {
        public override IEnumerable<NtType> CreateTypes()
        {
            return new NtType[] {
                new NtType(ServiceUtils.SERVICE_NT_TYPE_NAME, ServiceUtils.GetServiceGenericMapping(),
                        typeof(ServiceAccessRights), typeof(ServiceAccessRights),
                        MandatoryLabelPolicy.NoWriteUp),
                new NtType(ServiceUtils.SCM_NT_TYPE_NAME, ServiceUtils.GetScmGenericMapping(),
                        typeof(ServiceControlManagerAccessRights), typeof(ServiceControlManagerAccessRights),
                        MandatoryLabelPolicy.NoWriteUp)
            };
        }
    }

#pragma warning restore
    /// <summary>
    /// Utilities for accessing services.
    /// </summary>
    public static class ServiceUtils
    {
        #region Private Members
        private const int SERVICE_CONFIG_TRIGGER_INFO = 8;
        private const int SERVICE_CONFIG_SERVICE_SID_INFO = 5;
        private const int SERVICE_CONFIG_REQUIRED_PRIVILEGES_INFO = 6;
        private const int SERVICE_CONFIG_LAUNCH_PROTECTED = 12;
        private const int SERVICE_CONFIG_DELAYED_AUTO_START_INFO = 3;

        internal static string GetString(this IntPtr ptr)
        {
            if (ptr != IntPtr.Zero)
                return Marshal.PtrToStringUni(ptr);
            return string.Empty;
        }

        internal static IEnumerable<string> GetMultiString(this IntPtr ptr)
        {
            List<string> ss = new List<string>();
            if (ptr == IntPtr.Zero)
                return new string[0];
            string s = ptr.GetString();
            while (s.Length > 0)
            {
                ss.Add(s);
                ptr += (s.Length + 1) * 2;
                s = ptr.GetString();
            }
            return ss.AsReadOnly();
        }

        internal static string ToMultiString(this IEnumerable<string> ss)
        {
            if (ss == null || !ss.Any())
                return null;

            StringBuilder builder = new StringBuilder();

            foreach (var s in ss)
            {
                builder.Append(s);
                builder.Append('\0');
            }
            builder.Append('\0');
            return builder.ToString();
        }

        private static SecurityInformation DEFAULT_SECURITY_INFORMATION = SecurityInformation.Dacl
                | SecurityInformation.Owner
                | SecurityInformation.Label
                | SecurityInformation.Group;

        private static NtResult<SecurityDescriptor> GetServiceSecurityDescriptor(
            SafeServiceHandle handle, string type_name, SecurityInformation security_information,
            bool throw_on_error)
        {
            if (handle == null || handle.IsInvalid)
                return NtStatus.STATUS_INVALID_HANDLE.CreateResultFromError<SecurityDescriptor>(throw_on_error);
            byte[] sd = new byte[8192];
            return Win32NativeMethods.QueryServiceObjectSecurity(handle, security_information,
                sd, sd.Length, out _).CreateWin32Result(throw_on_error, 
                () => new SecurityDescriptor(sd, NtType.GetTypeByName(type_name)));
        }

        private static NtStatus SetServiceSecurityDescriptor(SafeServiceHandle handle,
            SecurityInformation security_information, SecurityDescriptor security_descriptor, bool throw_on_error)
        {
            if (handle is null)
            {
                throw new ArgumentNullException(nameof(handle));
            }

            if (security_descriptor is null)
            {
                throw new ArgumentNullException(nameof(security_descriptor));
            }

            if (!Win32NativeMethods.SetServiceObjectSecurity(handle, security_information, security_descriptor.ToByteArray()))
            {
                return Win32Utils.GetLastWin32Error().ToNtException(throw_on_error);
            }

            return NtStatus.STATUS_SUCCESS;
        }

        private static IEnumerable<ServiceTriggerInformation> GetTriggersForService(SafeServiceHandle service)
        {
            List<ServiceTriggerInformation> triggers = new List<ServiceTriggerInformation>();
            using (var buf = new SafeStructureInOutBuffer<SERVICE_TRIGGER_INFO>(8192, false))
            {
                if (!Win32NativeMethods.QueryServiceConfig2(service, SERVICE_CONFIG_TRIGGER_INFO,
                    buf, buf.Length, out int required))
                {
                    return triggers.AsReadOnly();
                }

                SERVICE_TRIGGER_INFO trigger_info = buf.Result;
                if (trigger_info.cTriggers == 0)
                {
                    return triggers.AsReadOnly();
                }

                SERVICE_TRIGGER[] trigger_arr;
                using (SafeHGlobalBuffer trigger_buffer = new SafeHGlobalBuffer(trigger_info.pTriggers,
                    trigger_info.cTriggers * Marshal.SizeOf(typeof(SERVICE_TRIGGER)), false))
                {
                    trigger_arr = new SERVICE_TRIGGER[trigger_info.cTriggers];
                    trigger_buffer.ReadArray(0, trigger_arr, 0, trigger_arr.Length);
                }

                for (int i = 0; i < trigger_arr.Length; ++i)
                {
                    triggers.Add(ServiceTriggerInformation.GetTriggerInformation(trigger_arr[i]));
                }

                return triggers.AsReadOnly();
            }
        }

        private static IEnumerable<string> GetServiceRequiredPrivileges(SafeServiceHandle service)
        {
            using (var buf = new SafeHGlobalBuffer(8192))
            {
                if (!Win32NativeMethods.QueryServiceConfig2(service, SERVICE_CONFIG_REQUIRED_PRIVILEGES_INFO,
                        buf, buf.Length, out int needed))
                {
                    return new string[0];
                }

                return buf.Read<IntPtr>(0).GetMultiString();
            }
        }

        private static ServiceSidType GetServiceSidType(SafeServiceHandle service)
        {
            using (var buf = new SafeStructureInOutBuffer<SERVICE_SID_INFO>())
            {
                if (!Win32NativeMethods.QueryServiceConfig2(service, SERVICE_CONFIG_SERVICE_SID_INFO,
                        buf, buf.Length, out int needed))
                {
                    return ServiceSidType.None;
                }
                return buf.Result.dwServiceSidType;
            }
        }

        private static ServiceLaunchProtectedType GetServiceLaunchProtectedType(SafeServiceHandle service)
        {
            using (var buf = new SafeStructureInOutBuffer<SERVICE_LAUNCH_PROTECTED_INFO>())
            {
                if (!Win32NativeMethods.QueryServiceConfig2(service, SERVICE_CONFIG_LAUNCH_PROTECTED,
                        buf, buf.Length, out int needed))
                {
                    return ServiceLaunchProtectedType.None;
                }
                return buf.Result.dwLaunchProtected;
            }
        }

        private static bool GetDelayedStart(SafeServiceHandle service)
        {
            using (var buf = new SafeStructureInOutBuffer<SERVICE_DELAYED_AUTO_START_INFO>())
            {
                if (!Win32NativeMethods.QueryServiceConfig2(service, SERVICE_CONFIG_DELAYED_AUTO_START_INFO,
                        buf, buf.Length, out int needed))
                {
                    return false;
                }
                return buf.Result.fDelayedAutostart;
            }
        }

        private static NtResult<ServiceInformation> GetServiceSecurityInformation(SafeServiceHandle scm, string name,
            SecurityInformation security_information, bool throw_on_error)
        {
            using (var service_result = OpenService(scm, name, ServiceAccessRights.QueryConfig, throw_on_error))
            {
                if (!service_result.IsSuccess)
                {
                    return service_result.Cast<ServiceInformation>();
                }

                var service = service_result.Result;
                using (var service_sec = OpenService(scm, name, ServiceAccessRights.ReadControl, false))
                {
                    using (var config = QueryConfig(service, false).GetResultOrDefault())
                    {
                        return new ServiceInformation(name,
                            GetServiceSecurityDescriptor(service_sec.GetResultOrDefault(), SERVICE_NT_TYPE_NAME, security_information, false).GetResultOrDefault(),
                            GetTriggersForService(service), GetServiceSidType(service),
                            GetServiceLaunchProtectedType(service), GetServiceRequiredPrivileges(service),
                            config, GetDelayedStart(service)).CreateResult();
                    }
                }
            }
        }

        private static NtResult<SafeStructureInOutBuffer<QUERY_SERVICE_CONFIG>> QueryConfig(SafeServiceHandle service, bool throw_on_error)
        {
            using (var buf = new SafeStructureInOutBuffer<QUERY_SERVICE_CONFIG>(8192, false))
            {
                return Win32NativeMethods.QueryServiceConfig(service, buf, buf.Length,
                    out int required).CreateWin32Result(throw_on_error, () => buf.Detach());
            }
        }

        private static string GetServiceDisplayName(SafeServiceHandle service)
        {
            using (var buf = QueryConfig(service, false))
            {
                if (!buf.IsSuccess)
                    return string.Empty;

                return buf.Result.Result.lpDisplayName.GetString();
            }
        }

        private static SERVICE_STATUS_PROCESS QueryStatus(SafeServiceHandle service)
        {
            using (var buffer = new SafeStructureInOutBuffer<SERVICE_STATUS_PROCESS>())
            {
                if (!Win32NativeMethods.QueryServiceStatusEx(service, SC_STATUS_TYPE.SC_STATUS_PROCESS_INFO,
                    buffer, buffer.Length, out int length))
                {
                    throw new SafeWin32Exception();
                }
                return buffer.Result;
            }
        }

        private static int GetServiceProcessId(SafeServiceHandle scm, string name)
        {
            using (SafeServiceHandle service = Win32NativeMethods.OpenService(scm, name, ServiceAccessRights.QueryStatus))
            {
                if (service.IsInvalid)
                {
                    throw new SafeWin32Exception();
                }

                return QueryStatus(service).dwProcessId;
            }
        }

        private static IEnumerable<Win32Service> GetServices(SERVICE_STATE service_state, ServiceType service_types)
        {
            using (SafeServiceHandle scm = Win32NativeMethods.OpenSCManager(null, null,
                            ServiceControlManagerAccessRights.Connect | ServiceControlManagerAccessRights.EnumerateService))
            {
                if (scm.IsInvalid)
                {
                    throw new SafeWin32Exception();
                }

                const int Length = 32 * 1024;
                using (var buffer = new SafeHGlobalBuffer(Length))
                {
                    int resume_handle = 0;
                    while (true)
                    {
                        bool ret = Win32NativeMethods.EnumServicesStatusEx(scm, SC_ENUM_TYPE.SC_ENUM_PROCESS_INFO,
                            service_types, service_state, buffer,
                            buffer.Length, out int bytes_needed, out int services_returned, ref resume_handle, null);
                        Win32Error error = Win32Utils.GetLastWin32Error();
                        if (!ret && error != Win32Error.ERROR_MORE_DATA)
                        {
                            throw new SafeWin32Exception(error);
                        }

                        ENUM_SERVICE_STATUS_PROCESS[] services = new ENUM_SERVICE_STATUS_PROCESS[services_returned];
                        buffer.ReadArray(0, services, 0, services_returned);
                        foreach (var service in services)
                        {
                            yield return new Win32Service(service);
                        }

                        if (ret)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private static NtResult<SafeServiceHandle> OpenService(SafeServiceHandle scm, string name, ServiceAccessRights desired_access, bool throw_on_error)
        {
            using (var service = Win32NativeMethods.OpenService(scm, name, desired_access))
            {
                if (service.IsInvalid)
                {
                    return Win32Utils.CreateResultFromDosError<SafeServiceHandle>(throw_on_error);
                }
                return service.Detach().CreateResult();
            }
        }

        private static NtResult<SafeServiceHandle> OpenService(string name, ServiceAccessRights desired_access, bool throw_on_error)
        {
            using (SafeServiceHandle scm = Win32NativeMethods.OpenSCManager(null, null,
                            ServiceControlManagerAccessRights.Connect))
            {
                if (scm.IsInvalid)
                {
                    return Win32Utils.CreateResultFromDosError<SafeServiceHandle>(throw_on_error);
                }
                return OpenService(scm, name, desired_access, throw_on_error);
            }
        }

        #endregion

        #region Static Properties
        /// <summary>
        /// The name of the fake NT type for a service.
        /// </summary>
        public const string SERVICE_NT_TYPE_NAME = "Service";
        /// <summary>
        /// The name of the fake NT type for the SCM.
        /// </summary>
        public const string SCM_NT_TYPE_NAME = "SCM";
        #endregion

        #region Static Methods
        /// <summary>
        /// Get the generic mapping for the SCM.
        /// </summary>
        /// <returns>The SCM generic mapping.</returns>
        public static GenericMapping GetScmGenericMapping()
        {
            GenericMapping mapping = new GenericMapping
            {
                GenericRead = ServiceControlManagerAccessRights.ReadControl | ServiceControlManagerAccessRights.EnumerateService | ServiceControlManagerAccessRights.QueryLockStatus,
                GenericWrite = ServiceControlManagerAccessRights.ReadControl | ServiceControlManagerAccessRights.CreateService | ServiceControlManagerAccessRights.ModifyBootConfig,
                GenericExecute = ServiceControlManagerAccessRights.ReadControl | ServiceControlManagerAccessRights.Connect | ServiceControlManagerAccessRights.Lock,
                GenericAll = ServiceControlManagerAccessRights.All
            };
            return mapping;
        }

        /// <summary>
        /// Get the generic mapping for a service.
        /// </summary>
        /// <returns>The service generic mapping.</returns>
        public static GenericMapping GetServiceGenericMapping()
        {
            GenericMapping mapping = new GenericMapping
            {
                GenericRead = ServiceAccessRights.ReadControl | ServiceAccessRights.QueryConfig
                | ServiceAccessRights.QueryStatus | ServiceAccessRights.Interrogate | ServiceAccessRights.EnumerateDependents,
                GenericWrite = ServiceAccessRights.ReadControl | ServiceAccessRights.ChangeConfig,
                GenericExecute = ServiceAccessRights.ReadControl | ServiceAccessRights.Start
                | ServiceAccessRights.Stop | ServiceAccessRights.PauseContinue | ServiceAccessRights.UserDefinedControl,
                GenericAll = ServiceAccessRights.All
            };
            return mapping;
        }

        /// <summary>
        /// Get the security descriptor of the SCM.
        /// </summary>
        /// <returns>The SCM security descriptor.</returns>
        public static SecurityDescriptor GetScmSecurityDescriptor()
        {
            return GetScmSecurityDescriptor(DEFAULT_SECURITY_INFORMATION);
        }

        /// <summary>
        /// Get the security descriptor of the SCM.
        /// </summary>
        /// <param name="security_information">Parts of the security descriptor to return.</param>
        /// <param name="throw_on_error">True to throw on error.</param>
        /// <returns>The SCM security descriptor.</returns>
        public static NtResult<SecurityDescriptor> GetScmSecurityDescriptor(SecurityInformation security_information, bool throw_on_error)
        {
            var desired_access = NtSecurity.QuerySecurityAccessMask(security_information).ToSpecificAccess<ServiceControlManagerAccessRights>();

            using (SafeServiceHandle scm = Win32NativeMethods.OpenSCManager(null, null,
                            ServiceControlManagerAccessRights.Connect | desired_access))
            {
                if (scm.IsInvalid)
                    return Win32Utils.CreateResultFromDosError<SecurityDescriptor>(throw_on_error);
                return GetServiceSecurityDescriptor(scm, SCM_NT_TYPE_NAME, security_information, throw_on_error);
            }
        }

        /// <summary>
        /// Get the security descriptor of the SCM.
        /// </summary>
        /// <param name="security_information">Parts of the security descriptor to return.</param>
        /// <returns>The SCM security descriptor.</returns>
        public static SecurityDescriptor GetScmSecurityDescriptor(SecurityInformation security_information)
        {
            return GetScmSecurityDescriptor(security_information, true).Result;
        }

        /// <summary>
        /// Get the security descriptor for a service.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <param name="security_information">Parts of the security descriptor to return.</param>
        /// <param name="throw_on_error">True to throw on error.</param>
        /// <returns>The security descriptor.</returns>
        public static NtResult<SecurityDescriptor> GetServiceSecurityDescriptor(string name, 
            SecurityInformation security_information, bool throw_on_error)
        {
            var desired_access = NtSecurity.QuerySecurityAccessMask(security_information).ToSpecificAccess<ServiceAccessRights>();

            using (var service = OpenService(name, desired_access, throw_on_error))
            {
                if (!service.IsSuccess)
                    return service.Cast<SecurityDescriptor>();
                return GetServiceSecurityDescriptor(service.Result, SERVICE_NT_TYPE_NAME, security_information, throw_on_error);
            }
        }

        /// <summary>
        /// Get the security descriptor for a service.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <param name="security_information">Parts of the security descriptor to return.</param>
        /// <returns>The security descriptor.</returns>
        public static SecurityDescriptor GetServiceSecurityDescriptor(string name,
            SecurityInformation security_information)
        {
            return GetServiceSecurityDescriptor(name, security_information, true).Result;
        }

        /// <summary>
        /// Set the SCM security descriptor.
        /// </summary>
        /// <param name="security_descriptor">The security descriptor to set.</param>
        /// <param name="security_information">The parts of the security descriptor to set.</param>
        /// <param name="throw_on_error">True to throw on error.</param>
        /// <returns>The NT status code.</returns>
        public static NtStatus SetScmSecurityDescriptor(SecurityDescriptor security_descriptor, 
            SecurityInformation security_information, bool throw_on_error)
        {
            var desired_access = NtSecurity.SetSecurityAccessMask(security_information).ToSpecificAccess<ServiceControlManagerAccessRights>();

            using (SafeServiceHandle scm = Win32NativeMethods.OpenSCManager(null, null,
                            ServiceControlManagerAccessRights.Connect | desired_access))
            {
                if (scm.IsInvalid)
                    return Win32Utils.GetLastWin32Error().ToNtException(throw_on_error);
                return SetServiceSecurityDescriptor(scm, security_information, security_descriptor, throw_on_error);
            }
        }

        /// <summary>
        /// Set the SCM security descriptor.
        /// </summary>
        /// <param name="security_descriptor">The security descriptor to set.</param>
        /// <param name="security_information">The parts of the security descriptor to set.</param>
        public static void SetScmSecurityDescriptor(SecurityDescriptor security_descriptor,
            SecurityInformation security_information)
        {
            SetScmSecurityDescriptor(security_descriptor, security_information, true);
        }

        /// <summary>
        /// Get the information about a service.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <param name="throw_on_error">True to throw on error.</param>
        /// <returns>The service information.</returns>
        public static NtResult<ServiceInformation> GetServiceInformation(string name, bool throw_on_error)
        {
            using (SafeServiceHandle scm = Win32NativeMethods.OpenSCManager(null, null,
                            ServiceControlManagerAccessRights.Connect))
            {
                if (scm.IsInvalid)
                {
                    return Win32Utils.CreateResultFromDosError<ServiceInformation>(throw_on_error);
                }

                return GetServiceSecurityInformation(scm, name, DEFAULT_SECURITY_INFORMATION, throw_on_error);
            }
        }

        /// <summary>
        /// Set the security descriptor for a service.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <param name="security_descriptor">The security descriptor to set.</param>
        /// <param name="security_information">The security information to set.</param>
        /// <param name="throw_on_error">True to throw on error.</param>
        /// <returns>The NT status.</returns>
        public static NtStatus SetServiceSecurityDescriptor(string name, SecurityDescriptor security_descriptor, 
            SecurityInformation security_information, bool throw_on_error)
        {
            var desired_access = NtSecurity.SetSecurityAccessMask(security_information).ToSpecificAccess<ServiceAccessRights>();
            using (var service = OpenService(name, desired_access, throw_on_error))
            {
                if (!service.IsSuccess)
                    return service.Status;
                return SetServiceSecurityDescriptor(service.Result, security_information, security_descriptor, throw_on_error);
            }
        }

        /// <summary>
        /// Set the security descriptor for a service.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <param name="security_descriptor">The security descriptor to set.</param>
        /// <param name="security_information">The security information to set.</param>
        public static void SetServiceSecurityDescriptor(string name, SecurityDescriptor security_descriptor,
            SecurityInformation security_information)
        {
            SetServiceSecurityDescriptor(name, security_descriptor, security_information, true);
        }

        /// <summary>
        /// Get the information about a service.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <returns>The service information.</returns>
        public static ServiceInformation GetServiceInformation(string name)
        {
            return GetServiceInformation(name, true).Result;
        }

        /// <summary>
        /// Get the information about all services.
        /// </summary>
        /// <param name="service_types">The types of services to return.</param>
        /// <returns>The list of service information.</returns>
        public static IEnumerable<ServiceInformation> GetServiceInformation(ServiceType service_types)
        {
            return GetServices(ServiceState.All, service_types).Select(s => GetServiceInformation(s.Name, 
                false).GetResultOrDefault()).Where(s => s != null && s.ServiceType.HasFlagSet(service_types)).ToArray();
        }

        /// <summary>
        /// Get the PID of a running service.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <returns>Returns the PID of the running service, or 0 if not running.</returns>
        /// <exception cref="SafeWin32Exception">Thrown on error.</exception>
        public static int GetServiceProcessId(string name)
        {
            using (SafeServiceHandle scm = Win32NativeMethods.OpenSCManager(null, null,
                            ServiceControlManagerAccessRights.Connect))
            {
                if (scm.IsInvalid)
                {
                    throw new SafeWin32Exception();
                }

                return GetServiceProcessId(scm, name);
            }
        }

        /// <summary>
        /// Get the PIDs of a list of running service.
        /// </summary>
        /// <param name="names">The names of the services.</param>
        /// <returns>Returns the PID of the running service, or 0 if not running.</returns>
        /// <exception cref="SafeWin32Exception">Thrown on error.</exception>
        public static IDictionary<string, int> GetServiceProcessIds(IEnumerable<string> names)
        {
            Dictionary<string, int> result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            using (SafeServiceHandle scm = Win32NativeMethods.OpenSCManager(null, null,
                            ServiceControlManagerAccessRights.Connect))
            {
                if (scm.IsInvalid)
                {
                    throw new SafeWin32Exception();
                }

                foreach (var name in names)
                {
                    if (!result.ContainsKey(name))
                    {
                        result[name] = GetServiceProcessId(scm, name);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Get a running service by name.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <returns>The running service.</returns>
        /// <remarks>This will return active and non-active services as well as drivers.</remarks>
        public static Win32Service GetService(string name)
        {
            using (SafeServiceHandle scm = Win32NativeMethods.OpenSCManager(null, null,
                            ServiceControlManagerAccessRights.Connect))
            {
                if (scm.IsInvalid)
                {
                    throw new SafeWin32Exception();
                }

                using (var service = Win32NativeMethods.OpenService(scm, name,
                    ServiceAccessRights.QueryConfig | ServiceAccessRights.QueryStatus))
                {
                    if (service.IsInvalid)
                    {
                        throw new SafeWin32Exception();
                    }
                    return new Win32Service(name, GetServiceDisplayName(service), QueryStatus(service));
                }
            }
        }

        /// <summary>
        /// Get a list of all registered services.
        /// </summary>
        /// <param name="state">Specify state of services to get.</param>
        /// <param name="service_types">Specify the type filter for services.</param>
        /// <returns>A list of registered services.</returns>
        public static IEnumerable<Win32Service> GetServices(ServiceState state, ServiceType service_types)
        {
            SERVICE_STATE state_flags;
            switch (state)
            {
                case ServiceState.All:
                    state_flags = SERVICE_STATE.SERVICE_STATE_ALL;
                    break;
                case ServiceState.Active:
                    state_flags = SERVICE_STATE.SERVICE_ACTIVE;
                    break;
                case ServiceState.InActive:
                    state_flags = SERVICE_STATE.SERVICE_INACTIVE;
                    break;
                default:
                    throw new ArgumentException("Invalid state.", nameof(state));
            }
            return GetServices(state_flags, service_types);
        }

        /// <summary>
        /// Get flags for all user service types.
        /// </summary>
        /// <returns>The flags for user service types.</returns>
        public static ServiceType GetServiceTypes()
        {
            ServiceType service_types = ServiceType.Win32OwnProcess | ServiceType.Win32ShareProcess;
            if (!NtObjectUtils.IsWindows81OrLess)
            {
                service_types |= ServiceType.UserService;
            }
            return service_types;
        }

        /// <summary>
        /// Get flags for all kernel driver types.
        /// </summary>
        /// <returns>The flags for kernel driver types.</returns>
        public static ServiceType GetDriverTypes()
        {
            return ServiceType.Driver;
        }

        /// <summary>
        /// Get a list of all registered services.
        /// </summary>
        /// <returns>A list of registered services.</returns>
        public static IEnumerable<Win32Service> GetServices()
        {
            return GetServices(SERVICE_STATE.SERVICE_STATE_ALL, GetServiceTypes());
        }

        /// <summary>
        /// Get a list of all active running services with their process IDs.
        /// </summary>
        /// <returns>A list of all active running services with process IDs.</returns>
        public static IEnumerable<Win32Service> GetRunningServicesWithProcessIds()
        {
            return GetServices(SERVICE_STATE.SERVICE_ACTIVE, GetServiceTypes());
        }

        /// <summary>
        /// Get a list of all drivers.
        /// </summary>
        /// <returns>A list of all drivers.</returns>
        public static IEnumerable<Win32Service> GetDrivers()
        {
            return GetServices(SERVICE_STATE.SERVICE_STATE_ALL, GetDriverTypes());
        }

        /// <summary>
        /// Get a list of all active running drivers.
        /// </summary>
        /// <returns>A list of all active running drivers.</returns>
        public static IEnumerable<Win32Service> GetRunningDrivers()
        {
            return GetServices(SERVICE_STATE.SERVICE_ACTIVE, GetDriverTypes());
        }

        /// <summary>
        /// Get a list of all services and drivers.
        /// </summary>
        /// <returns>A list of all services and drivers.</returns>
        public static IEnumerable<Win32Service> GetServicesAndDrivers()
        {
            return GetServices(SERVICE_STATE.SERVICE_STATE_ALL,
                GetDriverTypes() | GetServiceTypes());
        }

        /// <summary>
        /// Get a list of all services and drivers.
        /// </summary>
        /// <returns>A list of all services and drivers.</returns>
        public static IEnumerable<Win32Service> GetRunningServicesAndDrivers()
        {
            return GetServices(SERVICE_STATE.SERVICE_ACTIVE,
                GetDriverTypes() | GetServiceTypes());
        }

        /// <summary>
        /// Get a fake NtType for a service.
        /// </summary>
        /// <param name="type_name">Service returns the service type, SCM returns SCM type.</param>
        /// <returns>The fake service NtType. Returns null if not a recognized type.</returns>
        [Obsolete("Use NtType.GetTypeByName with SERVICE_NT_TYPE_NAME or SCM_NT_TYPE_NAME")]
        public static NtType GetServiceNtType(string type_name)
        {
            if (!type_name.Equals(SERVICE_NT_TYPE_NAME, StringComparison.OrdinalIgnoreCase)
                && !type_name.Equals(SCM_NT_TYPE_NAME, StringComparison.OrdinalIgnoreCase))
                return null;
            return NtType.GetTypeByName(type_name);
        }

        /// <summary>
        /// Create a new service.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <param name="display_name">The display name for the service.</param>
        /// <param name="service_type">The service type.</param>
        /// <param name="start_type">The service start type.</param>
        /// <param name="error_control">Error control.</param>
        /// <param name="binary_path_name">Path to the service executable.</param>
        /// <param name="load_order_group">Load group order.</param>
        /// <param name="dependencies">List of service dependencies.</param>
        /// <param name="service_start_name">The username for the service.</param>
        /// <param name="password">Password for the username if needed.</param>
        /// <param name="throw_on_error">True to throw on error.</param>
        /// <returns>The registered service information.</returns>
        public static NtResult<Win32Service> CreateService(
            string name,
            string display_name,
            ServiceType service_type,
            ServiceStartType start_type,
            ServiceErrorControl error_control,
            string binary_path_name,
            string load_order_group,
            IEnumerable<string> dependencies,
            string service_start_name,
            SecureString password,
            bool throw_on_error)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty", nameof(name));
            }

            if (string.IsNullOrEmpty(binary_path_name))
            {
                throw new ArgumentException($"'{nameof(binary_path_name)}' cannot be null or empty", nameof(binary_path_name));
            }

            using (var scm = Win32NativeMethods.OpenSCManager(null, null,
                ServiceControlManagerAccessRights.Connect | ServiceControlManagerAccessRights.CreateService))
            {
                if (scm.IsInvalid)
                    return Win32Utils.CreateResultFromDosError<Win32Service>(throw_on_error);

                using (var pwd = new SecureStringMarshalBuffer(password))
                {
                    using (var service = Win32NativeMethods.CreateService(scm, name, display_name, ServiceAccessRights.MaximumAllowed,
                            service_type, start_type, error_control, binary_path_name, load_order_group, null, dependencies.ToMultiString(),
                            string.IsNullOrEmpty(service_start_name) ? null : service_start_name, pwd))
                    {
                        if (service.IsInvalid)
                            return Win32Utils.CreateResultFromDosError<Win32Service>(throw_on_error);
                        return new Win32Service(name, display_name ?? string.Empty, QueryStatus(service)).CreateResult();
                    }
                }
            }
        }

        /// <summary>
        /// Create a new service.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <param name="display_name">The display name for the service.</param>
        /// <param name="service_type">The service type.</param>
        /// <param name="start_type">The service start type.</param>
        /// <param name="error_control">Error control.</param>
        /// <param name="binary_path_name">Path to the service executable.</param>
        /// <param name="load_order_group">Load group order.</param>
        /// <param name="dependencies">List of service dependencies.</param>
        /// <param name="service_start_name">The username for the service.</param>
        /// <param name="password">Password for the username if needed.</param>
        /// <returns>The registered service information.</returns>
        public static Win32Service CreateService(
            string name,
            string display_name,
            ServiceType service_type,
            ServiceStartType start_type,
            ServiceErrorControl error_control,
            string binary_path_name,
            string load_order_group,
            IEnumerable<string> dependencies,
            string service_start_name,
            SecureString password)
        {
            return CreateService(name, display_name, service_type,
                start_type, error_control, binary_path_name, load_order_group,
                dependencies, service_start_name, password, true).Result;
        }

        /// <summary>
        /// Delete a service.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <param name="throw_on_error">True to throw on error.</param>
        /// <returns>The NT status.</returns>
        public static NtStatus DeleteService(string name, bool throw_on_error)
        {
            using (var service = OpenService(name, ServiceAccessRights.Delete, throw_on_error))
            {
                if (!service.IsSuccess)
                    return service.Status;
                return Win32NativeMethods.DeleteService(service.Result).ToNtException(throw_on_error);
            }
        }
        /// <summary>
        /// Delete a service.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        public static void DeleteService(string name)
        {
            DeleteService(name, true);
        }

        /// <summary>
        /// Change service configuration.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <param name="display_name">The display name for the service.</param>
        /// <param name="service_type">The service type.</param>
        /// <param name="start_type">The service start type.</param>
        /// <param name="error_control">Error control.</param>
        /// <param name="binary_path_name">Path to the service executable.</param>
        /// <param name="load_order_group">Load group order.</param>
        /// <param name="dependencies">List of service dependencies.</param>
        /// <param name="service_start_name">The username for the service.</param>
        /// <param name="password">Password for the username if needed.</param>
        /// <param name="throw_on_error">True to throw on error.</param>
        /// <returns>The NT status code.</returns>
        public static NtStatus ChangeServiceConfig(
            string name,
            string display_name,
            ServiceType? service_type,
            ServiceStartType? start_type,
            ServiceErrorControl? error_control,
            string binary_path_name,
            string load_order_group,
            IEnumerable<string> dependencies,
            string service_start_name,
            SecureString password,
            bool throw_on_error)
        {
            using (var service = OpenService(name, ServiceAccessRights.ChangeConfig, throw_on_error))
            {
                if (!service.IsSuccess)
                    return service.Status;
                IntPtr pwd = password != null ? Marshal.SecureStringToBSTR(password) : IntPtr.Zero;
                try
                {
                    return Win32NativeMethods.ChangeServiceConfig(service.Result,
                        service_type ?? (ServiceType)(-1), start_type ?? (ServiceStartType)(-1),
                        error_control ?? (ServiceErrorControl)(-1), binary_path_name, load_order_group,
                        null, dependencies.ToMultiString(), service_start_name, pwd, display_name).ToNtException(throw_on_error);
                }
                finally
                {
                    if (pwd != IntPtr.Zero)
                        Marshal.FreeBSTR(pwd);
                }
            }
        }

        /// <summary>
        /// Start a service by name.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <param name="args">Optional arguments to pass to the service.</param>
        /// <param name="throw_on_error">True to throw on error.</param>
        /// <returns>The status code for the service.</returns>
        public static NtStatus StartService(string name, string[] args, bool throw_on_error)
        {
            using (var service = OpenService(name, ServiceAccessRights.Start, throw_on_error))
            {
                if (!service.IsSuccess)
                    return service.Status;
                return Win32NativeMethods.StartService(service.Result, 
                    args?.Length ?? 0, args).ToNtException(throw_on_error);
            }
        }

        /// <summary>
        /// Start a service by name.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        /// <param name="args">Optional arguments to pass to the service.</param>
        /// <returns>The status code for the service.</returns>
        public static void StartService(string name, string[] args)
        {
            StartService(name, args, true);
        }

        #endregion
    }
}
