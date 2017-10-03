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
    public static class EyeTracker2DInterfaceExtensions
    {
        public static EyeTracker2DInterface GetEyeTracker2DInterface(this ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new EyeTracker2DInterface(iface);
        }
    }
#endif

    /// <summary>
    /// Interface representing the intersection point on a 2D plane of the user's gaze.
    /// </summary>
    public class EyeTracker2DInterface : InterfaceBase<Vec2>
    {
#if NET45
        [Obsolete("Use the GetEyeTracker2DInterface extension method on ClientContext instead.")]
#endif
        public static EyeTracker2DInterface GetInterface(ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new EyeTracker2DInterface(iface);
        }

        private EyeTracker2DCallback cb;
        public EyeTracker2DInterface(Interface iface) :
            base(iface, Interface.osvrGetEyeTracker2DState) { }

        protected override void Start()
        {
            cb = new EyeTracker2DCallback(this.InterfaceCallback);
            Interface.osvrRegisterEyeTracker2DCallback(iface.Handle, cb, IntPtr.Zero);
        }

        protected void InterfaceCallback(IntPtr userdata, ref TimeValue timestamp, ref EyeTracker2DReport report)
        {
            if(this == null)
            {
                #if DEBUG
                Debug.WriteLine("[Managed-OSVR]: EyeTracker2D.InterfaceCallback called with null this ptr. Perhaps this object was not Diposed properly?");
                #endif
                return;
            }
            OnStateChanged(timestamp, report.sensor, report.state);
        }
    }
}
