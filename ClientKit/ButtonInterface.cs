/// Managed-OSVR binding
///
/// <copyright>
/// Copyright 2014 Sensics, Inc.
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
    public static class ButtonInterfaceExtensions
    {
        public static ButtonInterface GetButtonInterface(this ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new ButtonInterface(iface);
        }
    }
#endif

    /// <summary>
    /// Interface representing a button from an input device (either a
    /// physical button or a simulated button invoked by a gesture, etc...)
    /// </summary>
    public class ButtonInterface: InterfaceBase<Byte>
    {
        public const Byte Pressed = 1;
        public const Byte Released = 0;

#if NET45
        [Obsolete("Use the GetButtonInterface extension method on ClientContext instead.")]
#endif
        public static ButtonInterface GetInterface(ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new ButtonInterface(iface);
        }

        private ButtonCallback cb;
        public ButtonInterface(Interface iface) :
            base(iface, Interface.osvrGetButtonState) { }

        public override void Start()
        {
            cb = new ButtonCallback(this.InterfaceCallback);
            Interface.osvrRegisterButtonCallback(iface.Handle, cb, IntPtr.Zero);
        }

        protected void InterfaceCallback(IntPtr userdata, ref TimeValue timestamp, ref ButtonReport report)
        {
            OnStateChanged(timestamp, report.sensor, report.state);
        }
    }
}
