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
    public static class OrientationInterfaceExtensions
    {
        public static OrientationInterface GetOrientationInterface(this ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new OrientationInterface(iface);
        }
    }
#endif

    /// <summary>
    /// Interface representing an orientation sensor.
    /// </summary>
    public class OrientationInterface: InterfaceBase<OrientationReport, Quaternion>
    {
#if NET45
        [Obsolete("Use the GetOrientationInterface extension method on ClientContext instead.")]
#endif
        public static OrientationInterface GetInterface(ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new OrientationInterface(iface);
        }

        private OrientationCallback cb;
        public OrientationInterface(Interface iface)
            : base(iface, Interface.osvrGetOrientationState) { }

        public override void Start()
        {
            cb = new OrientationCallback(this.InterfaceCallback);
            Interface.osvrRegisterOrientationCallback(iface.Handle, cb, IntPtr.Zero);
        }

        protected void InterfaceCallback(IntPtr userdata, ref TimeValue timestamp, ref OrientationReport report)
        {
            OnStateChanged(timestamp, report);
        }
    }
}
