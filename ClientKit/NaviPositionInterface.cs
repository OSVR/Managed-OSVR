﻿/// Managed-OSVR binding
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
    public static class NaviPositionInterfaceExtensions
    {
        public static NaviPositionInterface GetNaviPositionInterface(this ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new NaviPositionInterface(iface);
        }
    }
#endif

    /// <summary>
    /// Interface representing a navigation velocity tracker.
    /// </summary>
    public class NaviPositionInterface : InterfaceBase<Vec2>
    {
#if NET45
        [Obsolete("Use the GetNaviPosition method on ClientContext instead.")]
#endif
        public static NaviPositionInterface GetInterface(ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new NaviPositionInterface(iface);
        }

        private NaviPositionCallback cb;
        public NaviPositionInterface(Interface iface) :
            base(iface, Interface.osvrGetNaviPositionState) { }

        protected override void Start()
        {
            cb = new NaviPositionCallback(this.InterfaceCallback);
            Interface.osvrRegisterNaviPositionCallback(iface.Handle, cb, IntPtr.Zero);
        }

        protected void InterfaceCallback(IntPtr userdata, ref TimeValue timestamp, ref NaviPositionReport report)
        {
            OnStateChanged(timestamp, report.sensor, report.state);
        }
    }
}
