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
using ChannelCount = System.Int32;

namespace OSVR.ClientKit
{
    /// <summary>
    /// Defines the joints that are available a skeleton per the OSVR Skeleton Spec.
    /// </summary>
    public enum SkeletonJoints:uint
    {
        Pelvis,
        Spine0,
        Spine1,
        Spine2,
        Spine3,
        Neck,
        Head,
        ClavicleLeft,
        ArmUpperLeft,
        ArmLowerLeft,
        HandLowerLeft,

        // left hand
        HandLeft,
        ThumbProximalLeft,
        ThumbMedialLeft,
        ThumbDistalLeft,
        
        IndexProximalLeft,
        IndexMedialLeft,
        IndexDistalLeft,

        MiddleProximalLeft,
        MiddleMedialLeft,
        MiddleDistalLeft,

        RingProximalLeft,
        RingMedialLeft,
        RingDistalLeft,

        PinkyProximalLeft,
        PinkyMedialLeft,
        PinkyDistalLeft,
        // end left hand

        ClavicleRight,
        ArmUpperRight,
        ArmLowerRight,
        HandLowerRight,

        // right hand
        HandRight,
        ThumbProximalRight,
        ThumbMedialRight,
        ThumbDistalRight,

        IndexProximalRight,
        IndexMedialRight,
        IndexDistalRight,

        MiddleProximalRight,
        MiddleMedialRight,
        MiddleDistalRight,

        RingProximalRight,
        RingMedialRight,
        RingDistalRight,

        PinkyProximalRight,
        PinkyMedialRight,
        PinkyDistalRight,
        // end right hand

        LegUpperLeft,
        LegLowerLeft,
        FootLeft,
        ToesLeft,
        LegUpperRight,
        LegLowerRight,
        FootRight,
        ToesRight,
    }

    ///// <summary>
    ///// There are various types of skeleton reports that allow to get
    ///// different skeleton joints/bones. Note, Each report can include information
    ///// from one skeleton sensor due to connectedness of skeleton. Refer to the
    ///// definition of each report below for a complete description of which bones are
    ///// included
    ///// </summary>
    //public enum SkeletonReportSizes
    //{
    //    Head = 2,
    //    Arm = 19,
    //    Leg = 4,
    //    Foot = 4,
    //    Hand = 16,
    //    LOA1 = 21,
    //    LOA2 = 55,
    //}

    /// <summary>
    /// A state of a single skeleton joint (joint/bone)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonJointState
    {
        /// <summary>
        /// A skeleton joint ID that specifies which bone/joint it is.
        /// </summary>
        public SkeletonJoints joint;

        /// <summary>
        /// A tracker pose state
        /// </summary>
        public Pose3 pose;
    }

    /// <summary>
    /// A state of a single skeleton joint (joint/bone).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonJointReport
    {
        /// <summary>
        /// A tracker sensor that corresponds to current joint.
        /// </summary>
        public ChannelCount sensor;
        public SkeletonJointState state;
    }

    /// <summary>
    /// A type of skeleton state Low level of Articulation LOA1.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonTrimmedState
    {
        public SkeletonJointReport[] joints;
    }

    /// <summary>
    /// Report for Skeleton Level of Articulation 1 (LOA1) As defined in
    /// H-Anim Low Level of Articulation provides a scaled down version of skeleton
    /// joints and includes the following: Head, Neck, Clavicle Left/Right
    /// (Shoulders), Arm Upper Left/Right (Elbows), Arm Lower Left/Right (Elbows)
    /// Hand Left/Right (Wrists), Spine 0/1/2/3 (Center spine), Pelvis, Leg Upper
    /// Left/Right (Hips), Leg Lower Left/Right (Knees), Foot Left/Right
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonTrimmedReport
    {
        public ChannelCount sensor;
        public SkeletonTrimmedState state;
    }

    /// <summary>
    /// A type of skeleton state Hgh level of Articulation LOA2.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonWholeState
    {
        public SkeletonJointReport[] joints;
    }

    /// <summary>
    /// Report for Skeleton Level of Articulation 2 (LOA2) : H-Anim defines a
    /// humanoid figure with 72 joints to have high Level of articulation however
    /// OSVR Skeleton Interface defines a total of 55 joints/bones and it includes
    /// all joints described above: Pelvis, Spine 0/1/2/3, Neck, Head, Clavicle
    /// Left/Right (Shoulders), Arm Upper Left/Right (Elbows), Arm Lower
    /// Left/Right(forearms), Hand Left/Right (Wrists), Left/Right Thumb
    /// Proximal/Medial/Distal, Left/Right Index Proximal/Medial/Distal, Left/Right
    /// Middle Proximal/Medial/Distal, Left/Right Ring Proximal/Medial/Distal,
    /// Left/Right Pinky Proximal/Medial/Distal, Leg Upper Left/Right (Hips), Leg
    /// Lower Left/Right (Knees), Foot Left/Right, Toes Left/Right
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonWholeReport
    {
        public ChannelCount sensor;
        public SkeletonWholeState state;
    }

    /// <summary>
    /// A type of skeleton hand state.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonHandState
    {
        public SkeletonJointReport[] joints;
    }

    /// <summary>
    /// Report for One Hand (Left or Right)
    /// Each hand report includes : Hand (Wrist), Thumb/Index/Middle/Ring/Pinky
    /// Proximal/Medial/Distal
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonHandReport
    {
        public ChannelCount sensor;
        public SkeletonHandState state;
    }

    /// <summary>
    /// A type of skeleton arm state
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonArmState
    {
        public SkeletonJointReport[] joints;
    }

    /// <summary>
    /// Report for a single Arm (Left or Right)
    /// Each hand report includes : Clavicle, Arm Upper, Arm Lower, Hand,
    /// Thumb/Index/Middle/Ring/Pinky Proximal/Medial/Distal
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonArmReport
    {
        /// <summary>
        /// A skeleton interface sensor ID
        /// </summary>
        public ChannelCount sensor;

        /// <summary>
        /// A collection of skeleton joint reports that only contains the
        /// joints for an arm.
        /// </summary>
        public SkeletonArmState state;
    }

    /// <summary>
    /// A type of skeleton foot state
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonFootState
    {
        public SkeletonJointReport[] joints;
    }

    /// <summary>
    /// Report for a single foot
    /// The report includes the following joints: Foot, Toes
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonFootReport
    {
        /// <summary>
        /// A skeleton interface sensor ID
        /// </summary>
        public ChannelCount sensor;

        /// <summary>
        /// A collection of skeleton joint reports that only contains the
        /// joints for a foot.
        /// </summary>
        public SkeletonFootState state;
    }

    /// <summary>
    /// A type of skeleton leg state
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonLegState
    {
        public SkeletonJointReport[] joints;
    }

    /// <summary>
    /// Report for a single leg
    /// The report includes the following joints: Leg Lower, Leg Upper, Foot, Toes
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonLegReport
    {
        /// <summary>
        /// A skeleton interface sensor ID
        /// </summary>
        public ChannelCount sensor;

        /// <summary>
        /// A collection of skeleton joint reports that only contains the
        /// above joints
        /// </summary>
        public SkeletonLegState state;
    }

#if NET45
    public static class SkeletonInterfaceExtensions
    {
        public static SkeletonJointInterface GetSkeletonJointInterface(this ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new SkeletonJointInterface(iface);
        }

        public static SkeletonTrimmedInterface GetSkeletonJointInterface(this ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new SkeletonTrimmedInterface(iface);
        }

        public static SkeletonWholeInterface GetSkeletonJointInterface(this ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new SkeletonWholeInterface(iface);
        }

        public static SkeletonHandInterface GetSkeletonJointInterface(this ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new SkeletonHandInterface(iface);
        }

        public static SkeletonArmInterface GetSkeletonJointInterface(this ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new SkeletonArmInterface(iface);
        }

        public static SkeletonFootInterface GetSkeletonJointInterface(this ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new SkeletonFootInterface(iface);
        }

        public static SkeletonLegInterface GetSkeletonJointInterface(this ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new SkeletonLegInterface(iface);
        }
    }
#endif

    /// <summary>
    /// Interface for SkeletonJoint reports.
    /// </summary>
    public class SkeletonJointInterface : InterfaceBase<SkeletonJointState>
    {
#if NET45
        [Obsolete("Use the GetSkeletonJointInterface extension method on ClientContext instead.")]
#endif
        public static SkeletonJointInterface GetInterface(ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new SkeletonJointInterface(iface);
        }

        private SkeletonJointCallback cb;
        public SkeletonJointInterface(Interface iface) :
            base(iface, Interface.osvrGetSkeletonJointState) { }

        protected override void Start()
        {
            cb = new SkeletonJointCallback(this.InterfaceCallback);
            Interface.osvrRegisterSkeletonJointCallback(iface.Handle, cb, IntPtr.Zero);
        }

        protected void InterfaceCallback(IntPtr userdata, ref TimeValue timestamp, ref SkeletonJointReport report)
        {
            OnStateChanged(timestamp, report.sensor, report.state);
        }
    }

    /// <summary>
    /// Interface for SkeletonTrimmed reports.
    /// </summary>
    public class SkeletonTrimmedInterface : InterfaceBase<SkeletonTrimmedState>
    {
#if NET45
        [Obsolete("Use the GetSkeletonTrimmedInterface extension method on ClientContext instead.")]
#endif
        public static SkeletonTrimmedInterface GetInterface(ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new SkeletonTrimmedInterface(iface);
        }

        private SkeletonTrimmedCallback cb;
        public SkeletonTrimmedInterface(Interface iface) :
            base(iface, Interface.osvrGetSkeletonTrimmedState) { }

        protected override void Start()
        {
            cb = new SkeletonTrimmedCallback(this.InterfaceCallback);
            Interface.osvrRegisterSkeletonTrimmedCallback(iface.Handle, cb, IntPtr.Zero);
        }

        protected void InterfaceCallback(IntPtr userdata, ref TimeValue timestamp, ref SkeletonTrimmedReport report)
        {
            OnStateChanged(timestamp, report.sensor, report.state);
        }
    }

    /// <summary>
    /// Interface for SkeletonWhole reports.
    /// </summary>
    public class SkeletonWholeInterface : InterfaceBase<SkeletonWholeState>
    {
#if NET45
        [Obsolete("Use the GetSkeletonWholeInterface extension method on ClientContext instead.")]
#endif
        public static SkeletonWholeInterface GetInterface(ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new SkeletonWholeInterface(iface);
        }

        private SkeletonWholeCallback cb;
        public SkeletonWholeInterface(Interface iface) :
            base(iface, Interface.osvrGetSkeletonWholeState) { }

        protected override void Start()
        {
            cb = new SkeletonWholeCallback(this.InterfaceCallback);
            Interface.osvrRegisterSkeletonWholeCallback(iface.Handle, cb, IntPtr.Zero);
        }

        protected void InterfaceCallback(IntPtr userdata, ref TimeValue timestamp, ref SkeletonWholeReport report)
        {
            OnStateChanged(timestamp, report.sensor, report.state);
        }
    }

    /// <summary>
    /// Interface for SkeletonHand reports.
    /// </summary>
    public class SkeletonHandInterface : InterfaceBase<SkeletonHandState>
    {
#if NET45
        [Obsolete("Use the GetSkeletonHandInterface extension method on ClientContext instead.")]
#endif
        public static SkeletonHandInterface GetInterface(ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new SkeletonHandInterface(iface);
        }

        private SkeletonHandCallback cb;
        public SkeletonHandInterface(Interface iface) :
            base(iface, Interface.osvrGetSkeletonHandState) { }

        protected override void Start()
        {
            cb = new SkeletonHandCallback(this.InterfaceCallback);
            Interface.osvrRegisterSkeletonHandCallback(iface.Handle, cb, IntPtr.Zero);
        }

        protected void InterfaceCallback(IntPtr userdata, ref TimeValue timestamp, ref SkeletonHandReport report)
        {
            OnStateChanged(timestamp, report.sensor, report.state);
        }
    }

    /// <summary>
    /// Interface for SkeletonArm reports.
    /// </summary>
    public class SkeletonArmInterface : InterfaceBase<SkeletonArmState>
    {
#if NET45
        [Obsolete("Use the GetSkeletonArmInterface extension method on ClientContext instead.")]
#endif
        public static SkeletonArmInterface GetInterface(ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new SkeletonArmInterface(iface);
        }

        private SkeletonArmCallback cb;
        public SkeletonArmInterface(Interface iface) :
            base(iface, Interface.osvrGetSkeletonArmState) { }

        protected override void Start()
        {
            cb = new SkeletonArmCallback(this.InterfaceCallback);
            Interface.osvrRegisterSkeletonArmCallback(iface.Handle, cb, IntPtr.Zero);
        }

        protected void InterfaceCallback(IntPtr userdata, ref TimeValue timestamp, ref SkeletonArmReport report)
        {
            OnStateChanged(timestamp, report.sensor, report.state);
        }
    }

    /// <summary>
    /// Interface for SkeletonFoot reports.
    /// </summary>
    public class SkeletonFootInterface : InterfaceBase<SkeletonFootState>
    {
#if NET45
        [Obsolete("Use the GetSkeletonFootInterface extension method on ClientContext instead.")]
#endif
        public static SkeletonFootInterface GetInterface(ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new SkeletonFootInterface(iface);
        }

        private SkeletonFootCallback cb;
        public SkeletonFootInterface(Interface iface) :
            base(iface, Interface.osvrGetSkeletonFootState) { }

        protected override void Start()
        {
            cb = new SkeletonFootCallback(this.InterfaceCallback);
            Interface.osvrRegisterSkeletonFootCallback(iface.Handle, cb, IntPtr.Zero);
        }

        protected void InterfaceCallback(IntPtr userdata, ref TimeValue timestamp, ref SkeletonFootReport report)
        {
            OnStateChanged(timestamp, report.sensor, report.state);
        }
    }

    /// <summary>
    /// Interface for SkeletonLeg reports.
    /// </summary>
    public class SkeletonLegInterface : InterfaceBase<SkeletonLegState>
    {
#if NET45
        [Obsolete("Use the GetSkeletonLegInterface extension method on ClientContext instead.")]
#endif
        public static SkeletonLegInterface GetInterface(ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new SkeletonLegInterface(iface);
        }

        private SkeletonLegCallback cb;
        public SkeletonLegInterface(Interface iface) :
            base(iface, Interface.osvrGetSkeletonLegState) { }

        protected override void Start()
        {
            cb = new SkeletonLegCallback(this.InterfaceCallback);
            Interface.osvrRegisterSkeletonLegCallback(iface.Handle, cb, IntPtr.Zero);
        }

        protected void InterfaceCallback(IntPtr userdata, ref TimeValue timestamp, ref SkeletonLegReport report)
        {
            OnStateChanged(timestamp, report.sensor, report.state);
        }
    }
}
