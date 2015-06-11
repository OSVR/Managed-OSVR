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
    public static class AnalogInterfaceExtensions
    {
        public static AnalogInterface GetAnalogInterface(this ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new AnalogInterface(iface);
        }
    }
#endif

    /// <summary>
    /// An interface representing an analog input device,
    /// for example an analog trigger or one of the axes of
    /// a joystick.
    /// </summary>
    public class AnalogInterface: InterfaceBase<Double>
    {
#if NET45
        [Obsolete("Use the GetAnalogInterface extension method on ClientContext instead.")]
#endif
        public static AnalogInterface GetInterface(ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new AnalogInterface(iface);
        }

        private AnalogCallback cb;
        public AnalogInterface(Interface iface) :
            base(iface, Interface.osvrGetAnalogState) { }

		protected override void Start()
        {
            cb = new AnalogCallback(this.InterfaceCallback);
            Interface.osvrRegisterAnalogCallback(iface.Handle, cb, IntPtr.Zero);
        }

        protected void InterfaceCallback(IntPtr userdata, ref TimeValue timestamp, ref AnalogReport report)
        {
            OnStateChanged(timestamp, report.sensor, report.state);
        }
    }
}
