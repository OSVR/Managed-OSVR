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
    /// <summary>
    /// Bit flags for specifying matrix options. Only one option may be
    /// specified per pair, with all the specified options combined with bitwise-or
    /// `|`.
    ///
    /// Most methods here that take matrix flags only obey the first grouping - the
    /// memory ordering flags. The remaining flags are primarily for use with
    /// projection matrix generation.
    /// </summary>
    [Flags]
    public enum MatrixConventionsFlags : ushort
    {
        /// <summary>
        /// Default options - Column major, column vectors, right handed, signed z
        /// </summary>
        Default = 0x0,

        /// <summary>
        /// Memory order - column major (default)
        /// </summary>
        ColMajor = 0x0,

        /// <summary>
        /// Memory order - row major
        /// </summary>
        RowMajor = 0x1,

        /// <summary>
        /// Matrix transforms column vectors (default)
        /// </summary>
        ColVectors = 0x0,

        /// <summary>
        /// Matrix transforms row vectors
        /// </summary>
        RowVectors = 0x2,

        /// <summary>
        /// Right-handed coordinate system (default)
        /// </summary>
        RHInput = 0x0,

        /// <summary>
        /// Left-handed coordinate system
        /// </summary>
        LHInput = 0x4,

        /// <summary>
        /// Projection matrix outputs the near and far planes
        /// mapped to Z values in range [-1,1] (default)
        /// </summary>
        SignedZ = 0x0,

        /// <summary>
        /// Projection matrix outputs the near and far planes
        /// mapped to Z values in range [0,1]
        /// </summary>
        UnsignedZ = 0x8,
    }

    /// <summary>
    /// A structure defining a 4x4 matrix (double). Not to be used for general pose
    /// data, that can be more descriptively described with Pose3.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix44d
    {
        double M0;
        double M1;
        double M2;
        double M3;

        double M4;
        double M5;
        double M6;
        double M7;

        double M8;
        double M9;
        double M10;
        double M11;

        double M12;
        double M13;
        double M14;
        double M15;

        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}",
                M0, M1, M2, M3, M4, M5, M6, M7, M8, M9, M10, M11, M12, M13, M14, M15);
        }
    }

    /// <summary>
    /// A structure defining a 4x4 matrix (float). Not to be used for general pose
    /// data, that can be more descriptively described with Pose3.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix44f
    {
        float M0;
        float M1;
        float M2;
        float M3;
        float M4;
        float M5;
        float M6;
        float M7;
        float M8;
        float M9;
        float M10;
        float M11;
        float M12;
        float M13;
        float M14;
        float M15;

        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}",
                M0, M1, M2, M3, M4, M5, M6, M7, M8, M9, M10, M11, M12, M13, M14, M15);
        }
    }

    internal static class MatrixConventionsNative
    {
        #if MANAGED_OSVR_INTERNAL_PINVOKE
        // On iOS and Xbox 360, plugins are statically linked into
        // the executable, so we have to use __Internal as the
        // library name.
        private const string OSVRUtilDll = "__Internal";
#else
        private const string OSVRUtilDll = "osvrUtil";
#endif

        [DllImport(OSVRUtilDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Byte osvrPose3ToMatrixd(ref Pose3 pose, MatrixConventionsFlags flags, out Matrix44d mat);

        [DllImport(OSVRUtilDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern Byte osvrPose3ToMatrixf(ref Pose3 pose, MatrixConventionsFlags flags, out Matrix44f mat);
    }

    public static class MatrixConventions
    {
        /// <summary>
        /// Convert a Pose3 to a Matrix44d, using the given convention flags
        /// </summary>
#if !NET20
        public static Matrix44d ToMatrixd(this Pose3 pose, MatrixConventionsFlags flags)
#else
        public static Matrix44d ToMatrixd(Pose3 pose, MatrixConventionsFlags flags)
#endif
        {
            Matrix44d ret;
            MatrixConventionsNative.osvrPose3ToMatrixd(ref pose, flags, out ret);
            return ret;
        }

        /// <summary>
        /// Convert a Pose3 to a Matrix44d, using the given convention flags
        /// </summary>
#if !NET20
        public static void ToMatrixd(this Pose3 pose, MatrixConventionsFlags flags, out Matrix44d matrix)
#else
        public static void ToMatrixd(Pose3 pose, MatrixConventionsFlags flags, out Matrix44d matrix)
#endif
        {
            MatrixConventionsNative.osvrPose3ToMatrixd(ref pose, flags, out matrix);
        }

        /// <summary>
        /// Convert a Pose3 to a Matrix44f, using the given convention flags
        /// </summary>
#if !NET20
        public static Matrix44f ToMatrixf(this Pose3 pose, MatrixConventionsFlags flags)
#else
        public static Matrix44f ToMatrixf(Pose3 pose, MatrixConventionsFlags flags)
#endif
        {
            Matrix44f ret;
            MatrixConventionsNative.osvrPose3ToMatrixf(ref pose, flags, out ret);
            return ret;
        }

        /// <summary>
        /// Convert a Pose3 to a Matrix44f, using the given convention flags
        /// </summary>
#if !NET20
        public static void ToMatrixf(this Pose3 pose, MatrixConventionsFlags flags, out Matrix44f matrix)
#else
        public static void ToMatrixf(Pose3 pose, MatrixConventionsFlags flags, out Matrix44f matrix)
#endif
        {
            MatrixConventionsNative.osvrPose3ToMatrixf(ref pose, flags, out matrix);
        }
    }
}
