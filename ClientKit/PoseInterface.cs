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
using System.Diagnostics;

namespace OSVR.ClientKit
{
#if NET45
    public static class PoseInterfaceExtensions
    {
        public static PoseInterface GetPoseInterface(this ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new PoseInterface(iface);
        }
    }
#endif

    /// <summary>
    /// Interface representing a combined position and orientation sensor.
    /// </summary>
    public class PoseInterface: InterfaceBase<Pose3>
    {
#if NET45
        [Obsolete("Use the GetPoseInterface method on ClientContext instead.")]
#endif
        public static PoseInterface GetInterface(ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new PoseInterface(iface);
        }

        private PoseCallback cb;
        public PoseInterface(Interface iface) :
            base(iface, Interface.osvrGetPoseState) { }

        protected override void Start()
        {
            cb = new PoseCallback(this.InterfaceCallback);
            Interface.osvrRegisterPoseCallback(iface.Handle, cb, IntPtr.Zero);
        }

        protected void InterfaceCallback(IntPtr userdata, ref TimeValue timestamp, ref PoseReport report)
        {
            if(this == null)
            {
                #if DEBUG
                Debug.WriteLine("[Managed-OSVR]: PoseInterface.InterfaceCallback called with null this ptr. Perhaps this object was not Diposed properly?");
                #endif
                return;
            }
            OnStateChanged(timestamp, report.sensor, report.pose);
        }
    }
}
