﻿//  Copyright 2021 Google Inc. All Rights Reserved.
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

namespace NtApiDotNet.Win32.Security.Policy
{
    /// <summary>
    /// Utilities for an LSA policy.
    /// </summary>
    internal static class LsaPolicyUtils
    {
        #region Static Methods
        /// <summary>
        /// The name of the fake NT type for a LSA policy.
        /// </summary>
        public const string LSA_POLICY_NT_TYPE_NAME = "LsaPolicy";

        /// <summary>
        /// The name of the fake NT type for a LSA secret.
        /// </summary>
        public const string LSA_SECRET_NT_TYPE_NAME = "LsaSecret";

        /// <summary>
        /// The name of the fake NT type for a LSA secret.
        /// </summary>
        public const string LSA_ACCOUNT_NT_TYPE_NAME = "LsaAccount";

        /// <summary>
        /// Generic generic mapping for LSA policy security.
        /// </summary>
        /// <returns>The generic mapping for the LSA policy.</returns>
        public static GenericMapping GetLsaPolicyGenericMapping()
        {
            return new GenericMapping()
            {
                GenericRead = LsaPolicyAccessRights.ReadControl | LsaPolicyAccessRights.ViewAuditInformation | LsaPolicyAccessRights.GetPrivateInformation,
                GenericWrite = LsaPolicyAccessRights.ReadControl | LsaPolicyAccessRights.TrustAdmin | LsaPolicyAccessRights.CreateAccount | LsaPolicyAccessRights.CreateSecret |
                    LsaPolicyAccessRights.CreatePrivilege | LsaPolicyAccessRights.SetDefaultQuotaLimits | LsaPolicyAccessRights.SetAuditRequirements | LsaPolicyAccessRights.AuditLogAdmin |
                    LsaPolicyAccessRights.ServerAdmin,
                GenericExecute = LsaPolicyAccessRights.ReadControl | LsaPolicyAccessRights.ViewLocalInformation | LsaPolicyAccessRights.LookupNames,
                GenericAll = LsaPolicyAccessRights.ReadControl | LsaPolicyAccessRights.WriteDac | LsaPolicyAccessRights.WriteOwner | LsaPolicyAccessRights.Delete |
                    LsaPolicyAccessRights.ViewAuditInformation | LsaPolicyAccessRights.GetPrivateInformation | LsaPolicyAccessRights.TrustAdmin | LsaPolicyAccessRights.CreateAccount | LsaPolicyAccessRights.CreateSecret |
                    LsaPolicyAccessRights.CreatePrivilege | LsaPolicyAccessRights.SetDefaultQuotaLimits | LsaPolicyAccessRights.SetAuditRequirements | LsaPolicyAccessRights.AuditLogAdmin |
                    LsaPolicyAccessRights.ServerAdmin | LsaPolicyAccessRights.ViewLocalInformation | LsaPolicyAccessRights.LookupNames | LsaPolicyAccessRights.Notification
            };
        }

        /// <summary>
        /// Generic generic mapping for LSA secret security.
        /// </summary>
        /// <returns>The generic mapping for the LSA secret.</returns>
        public static GenericMapping GetLsaSecretGenericMapping()
        {
            return new GenericMapping()
            {
                GenericRead = LsaSecretAccessRights.ReadControl | LsaSecretAccessRights.QueryValue,
                GenericWrite = LsaSecretAccessRights.ReadControl | LsaSecretAccessRights.SetValue,
                GenericExecute = LsaPolicyAccessRights.ReadControl,
                GenericAll = LsaSecretAccessRights.ReadControl | LsaSecretAccessRights.WriteDac | LsaSecretAccessRights.WriteOwner | LsaSecretAccessRights.Delete |
                    LsaSecretAccessRights.QueryValue | LsaSecretAccessRights.SetValue
            };
        }

        /// <summary>
        /// Generic generic mapping for LSA account security.
        /// </summary>
        /// <returns>The generic mapping for the LSA account.</returns>
        public static GenericMapping GetLsaAccountGenericMapping()
        {
            return new GenericMapping()
            {
                GenericRead = LsaAccountAccessRights.ReadControl | LsaAccountAccessRights.View,
                GenericWrite = LsaAccountAccessRights.ReadControl | LsaAccountAccessRights.AdjustPrivileges | LsaAccountAccessRights.AdjustQuotas | LsaAccountAccessRights.AdjustSystemAccess,
                GenericExecute = LsaAccountAccessRights.ReadControl,
                GenericAll = LsaAccountAccessRights.ReadControl | LsaAccountAccessRights.WriteDac | LsaAccountAccessRights.WriteOwner | LsaAccountAccessRights.Delete |
                    LsaAccountAccessRights.View | LsaAccountAccessRights.AdjustPrivileges | LsaAccountAccessRights.AdjustQuotas | LsaAccountAccessRights.AdjustSystemAccess
            };
        }

        public static NtType LsaPolicyNtType => NtType.GetTypeByName(LSA_POLICY_NT_TYPE_NAME);
        public static NtType LsaSecretNtType => NtType.GetTypeByName(LSA_SECRET_NT_TYPE_NAME);
        public static NtType LsaAccountNtType => NtType.GetTypeByName(LSA_ACCOUNT_NT_TYPE_NAME);

        #endregion
    }
}