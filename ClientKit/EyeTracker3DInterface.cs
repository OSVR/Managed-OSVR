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
    public static class EyeTracker3DInterfaceExtensions
    {
        public static EyeTracker3DInterface GetEyeTracker3DInterface(this ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new EyeTracker3DInterface(iface);
        }
    }
#endif

    /// <summary>
    /// Interface representing a position sensor.
    /// </summary>
    public class EyeTracker3DInterface : InterfaceBase<EyeTracker3DState>
    {
#if NET45
        [Obsolete("Use the GetEyeTracker3DInterface extension method on ClientContext instead.")]
#endif
        public static EyeTracker3DInterface GetInterface(ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new EyeTracker3DInterface(iface);
        }

        private EyeTracker3DCallback cb;
        public EyeTracker3DInterface(Interface iface) :
            base(iface, Interface.osvrGetEyeTracker3DState) { }

        protected override void Start()
        {

            cb = new EyeTracker3DCallback(this.InterfaceCallback);
            Interface.osvrRegisterEyeTracker3DCallback(iface.Handle, cb, IntPtr.Zero);
        }

        protected void InterfaceCallback(IntPtr userdata, ref TimeValue timestamp, ref EyeTracker3DReport report)
        {
            if(this == null)
            {
                #if DEBUG
                Debug.WriteLine("[Managed-OSVR]: EyeTracker3DInterface.InterfaceCallback called with null this ptr. Perhaps this object was not Diposed properly?");
                #endif
                return;
            }
            OnStateChanged(timestamp, report.sensor, report.state);
        }
    }
}
