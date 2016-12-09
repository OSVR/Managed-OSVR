/// Managed-OSVR binding
///
/// <copyright>
/// Copyright 2016 Sensics, Inc. and contributors
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///     http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// </copyright>

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
    /// <summary>
    /// based on this internal implementation here:
    /// https://github.com/dotnet/corefx/blob/master/src/Common/src/Microsoft/Win32/SafeHandles/SafeHandleZeroOrMinusOneIsInvalid.cs
    /// LICENSE:
    /// Licensed to the .NET Foundation under one or more agreements.
    /// The .NET Foundation licenses this file to you under the MIT license.
    /// See the LICENSE file in the project root for more information.
    /// </summary>
    public abstract class SafeHandleZeroOrMinusOneIsInvalid : SafeHandle
    {
        protected SafeHandleZeroOrMinusOneIsInvalid(bool ownsHandle)
            : base(IntPtr.Zero, ownsHandle)
        { }

        public override bool IsInvalid
        {
            get
            {
                return handle == IntPtr.Zero || handle == (IntPtr)(-1);
            }
        }
    }
}
