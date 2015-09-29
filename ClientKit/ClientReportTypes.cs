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
using OSVR.ClientKit;

using SensorCount = System.Int32;

using EyeTracker2DState = OSVR.ClientKit.Vec2;
using Location2DState = OSVR.ClientKit.Vec2;

using DirectionState = OSVR.ClientKit.Vec3;
using EyeTracker3DState = OSVR.ClientKit.Vec3;
using PositionState = OSVR.ClientKit.Vec3;

using ImageDimension = System.UInt32;
using ImageChannels = System.Byte;
using ImageDepth = System.Byte;
using ImageBufferElement = System.Byte;

namespace OSVR
{
    namespace ClientKit
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PositionReport
        {
            public SensorCount sensor;
            public PositionState xyz;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OrientationReport
        {
            public SensorCount sensor;
            public Quaternion rotation;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PoseReport
        {
            public SensorCount sensor;
            public Pose3 pose;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ButtonReport
        {
            public SensorCount sensor;
            public Byte state;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct AnalogReport
        {
            public SensorCount sensor;
            public Double state;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Location2DReport
        {
            public SensorCount sensor;
            public Location2DState xy;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DirectionReport
        {
            public SensorCount sensor;
            public DirectionState direction;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EyeTracker3DState
        {
            public bool directionValid;
            public DirectionState direction;
            public bool basePointValid;
            public PositionState basePoint;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EyeTracker3DReport
        {
            public SensorCount sensor;
            public EyeTracker3DState state;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EyeTracker2DReport
        {
            public SensorCount sensor;
            public EyeTracker2DState state;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EyeTrackerBlinkReport
        {
            public SensorCount sensor;
            [MarshalAs(UnmanagedType.I1)]
            public bool state;
        }

        public enum ImagingValueType
        {
            UnsignedInt = 0,
            SignedInt = 1,
            FloatingPoint = 2,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ImagingMetadata
        {
            public ImageDimension height;
            public ImageDimension width;
            public ImageChannels channels;
            public ImageDepth depth;
            public ImagingValueType type;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ImagingState
        {
            public ImagingMetadata metadata;
            public IntPtr data;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ImagingReport
        {
            public SensorCount sensor;
            public ImagingState state;
        }

        /// <summary>
        /// Report type for an navigation velocity callback on a tracker
        /// interface
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct NaviVelocityReport
        {
            public SensorCount sensor;
            /// <summary>
            /// The 2D vector in world coordinate system, in meters/second
            /// </summary>
            public Vec2 state;
        }

        /// <summary>
        /// Report type for an navigation position callback on a tracker
        /// interface
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct NaviPositionReport
        {
            public SensorCount sensor;
            /// <summary>
            /// The 2D vector in world coordinate system, in meters, relative to
            /// starting position 
            /// </summary>
            public Vec2 state;
        }
    }

}
