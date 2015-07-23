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
            public byte state;
        }
    }

}
