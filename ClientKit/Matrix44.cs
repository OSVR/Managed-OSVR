/// Managed-OSVR binding
///
/// <copyright>
/// Copyright 2014, 2015 Sensics, Inc. and contributors
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

﻿using System;
using System.Runtime.InteropServices;


namespace OSVR.ClientKit
{
    internal static class Matrix44Native
    {
#if MANAGED_OSVR_INTERNAL_PINVOKE
        // On iOS and Xbox 360, plugins are statically linked into
        // the executable, so we have to use __Internal as the
        // library name.
        private const string OSVRCoreDll = "__Internal";
#else
        private const string OSVRCoreDll = "osvrClientKit";
#endif

        /// <summary>
        /// Set a matrix to identity
        /// </summary>
        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void osvrMatrix44SetIdentity(out Matrix44 matrix);
    }

    /// <summary>
    /// A structure defining a 4x4 matrix. Not to be used for general pose
    /// data, that can be more descriptively described with Pose3.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix44
    {
        double M11;
        double M12;
        double M13;
        double M14;

        double M21;
        double M22;
        double M23;
        double M24;

        double M31;
        double M32;
        double M33;
        double M34;

        double M41;
        double M42;
        double M43;
        double M44;

        static Matrix44 Identity
        {
            get
            {
                Matrix44 ret;
                Matrix44Native.osvrMatrix44SetIdentity(out ret);
                return ret;
            }
        }
    }
}
