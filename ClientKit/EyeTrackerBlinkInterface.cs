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

namespace OSVR.ClientKit
{
#if NET45
    public static class EyeTrackerBlinkInterfaceExtensions
    {
        public static EyeTrackerBlinkInterface GetEyeTrackerBlinkInterface(this ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new EyeTrackerBlinkInterface(iface);
        }
    }
#endif

    /// <summary>
    /// Interface representing a position sensor.
    /// </summary>
    public class EyeTrackerBlinkInterface : InterfaceBase<Byte>
    {
#if NET45
        [Obsolete("Use the GetEyeTrackerBlinkInterface extension method on ClientContext instead.")]
#endif
        public static EyeTrackerBlinkInterface GetInterface(ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new EyeTrackerBlinkInterface(iface);
        }

        private EyeTrackerBlinkCallback cb;
        public EyeTrackerBlinkInterface(Interface iface) :
            base(iface, Interface.osvrGetEyeTrackerBlinkState) { }

        protected override void Start()
        {

            cb = new EyeTrackerBlinkCallback(this.InterfaceCallback);
            Interface.osvrRegisterEyeTrackerBlinkCallback(iface.Handle, cb, IntPtr.Zero);
        }

        protected void InterfaceCallback(IntPtr userdata, ref TimeValue timestamp, ref EyeTrackerBlinkReport report)
        {
            OnStateChanged(timestamp, report.sensor, report.state);
        }
    }
}
