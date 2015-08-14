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

using System;
using OSVR.ClientKit;

namespace DisplayParameter
{
    class MatrixConventionsTest
    {
        static void Main(string[] args)
        {
            ClientContext.PreloadNativeLibraries();
            Pose3 pose = new Pose3
            {
                rotation = new Quaternion { x = 0f, y = 0f, z = 1f, w = 0f },
                translation = new Vec3 { x = 2.0f, y = 3.0f, z = 4.0f },
            };

#if !NET20
            Matrix44d matrixRH = pose.ToMatrixd(MatrixConventionsFlags.RHInput);
            Matrix44d matrixLH = pose.ToMatrixd(MatrixConventionsFlags.LHInput);
            Matrix44f matrixRM = pose.ToMatrixf(MatrixConventionsFlags.RowMajor);
#else
            Matrix44d matrixRH = MatrixConventions.ToMatrixd(pose, MatrixConventionsFlags.RHInput);
            Matrix44d matrixLH = MatrixConventions.ToMatrixd(pose, MatrixConventionsFlags.LHInput);
            Matrix44f matrixRM = MatrixConventions.ToMatrixf(pose, MatrixConventionsFlags.RowMajor);
#endif
            Console.WriteLine("pose as double matrix using right-handed coordinates: {0}", matrixRH.ToString());
            Console.WriteLine("pose as double matrix using left-handed coordinates: {0}", matrixLH.ToString());
            Console.WriteLine("pose as float matrix as row-major: {0}", matrixRM);
        }
    }
}
