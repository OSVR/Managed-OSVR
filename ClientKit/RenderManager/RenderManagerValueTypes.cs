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
using OSVR.ClientKit;
using System.Runtime.InteropServices;

namespace OSVR.RenderManager
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RenderParams
    {
        /// <summary>
        /// Room space to insert
        /// </summary>
        public Pose3 WorldFromRoomAppend;

        /// <summary>
        /// Overrides head space
        /// </summary>
        public Pose3 RoomFromHeadReplace;

        public double NearClipDistanceMeters;
        public double FarClipDistanceMeters;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProjectionMatrix
    {
        public double Left;
        public double Right;
        public double Top;
        public double Bottom;
        public double NearClip;
        public double FarClip;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ViewportDescription
    {
        public double Left;
        public double Lower;
        public double Width;
        public double Height;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RGBFloat
    {
        public float R;
        public float G;
        public float B;
    }

    public enum OpenStatus
    {
        Failure = 0,
        Partial = 1,
        Complete = 2,
    }
}
