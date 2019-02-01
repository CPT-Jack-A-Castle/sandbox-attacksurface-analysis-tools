﻿//  Copyright 2019 Google Inc. All Rights Reserved.
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

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace NtApiDotNet
{
#pragma warning disable 1591
    public enum KtmObjectType
    {
        Transaction,
        TransactionManager,
        ResourceManager,
        Enlistment,
        Invalid
    }

    [StructLayout(LayoutKind.Sequential), DataStart("ObjectIds")]
    public struct KtmObjectCursor
    {
        public Guid LastQuery;
        public int ObjectIdCount;
        public Guid ObjectIds;
    }

    public static partial class NtSystemCalls
    {
        [DllImport("ntdll.dll")]
        public static extern NtStatus NtEnumerateTransactionObject(
          SafeKernelObjectHandle RootObjectHandle,
          KtmObjectType QueryType,
          ref KtmObjectCursor ObjectCursor,
          int ObjectCursorLength,
          out int ReturnLength
        );
    }

#pragma warning restore 1591

    /// <summary>
    /// General utilities for the kernel transaction manager.
    /// </summary>
    public static class NtTransactionManagerUtils
    {
        #region Static Methods

        /// <summary>
        /// Enumerate transaction objects of a specific type from a root handle.
        /// </summary>
        /// <param name="root_object_handle">The root handle to enumearate from.</param>
        /// <param name="query_type">The type of object to query.</param>
        /// <returns>The list of enumerated transaction object GUIDs.</returns>
        public static IEnumerable<Guid> EnumerateTransactionObjects(SafeKernelObjectHandle root_object_handle, KtmObjectType query_type)
        {
            KtmObjectCursor cursor = new KtmObjectCursor();
            int size = Marshal.SizeOf(cursor);
            NtStatus status = NtSystemCalls.NtEnumerateTransactionObject(root_object_handle, query_type, ref cursor, size, out int return_length);
            while (status != NtStatus.STATUS_NO_MORE_ENTRIES)
            {
                yield return cursor.ObjectIds;
                status = NtSystemCalls.NtEnumerateTransactionObject(root_object_handle, query_type, ref cursor, size, out return_length);
            }
        }

        /// <summary>
        /// Enumerate all transaction objects of a specific type.
        /// </summary>
        /// <param name="query_type">The type of object to query.</param>
        /// <returns>The list of enumerated transaction object GUIDs.</returns>
        public static IEnumerable<Guid> EnumerateTransactionObjects(KtmObjectType query_type)
        {
            return EnumerateTransactionObjects(SafeKernelObjectHandle.Null, query_type);
        }
        #endregion
    }
}