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
using System.Runtime.InteropServices;

namespace OSVR.ClientKit
{
#if NET45
    public static class ImagingInterfaceExtensions
    {
        public static ImagingInterface GetImagingInterface(this ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new ImagingInterface(context, iface);
        }
    }
#endif

    internal static class ImagingInterfaceNative
    {
        [DllImport(OSVRLibNames.ClientKit, CallingConvention = CallingConvention.Cdecl)]
        public static extern Byte osvrClientFreeImage(SafeClientContextHandle ctx, IntPtr imageData);
    }

    /// <summary>
    /// Interface representing the intersection point on a 2D plane of the user's gaze.
    /// </summary>
    public class ImagingInterface : InterfaceBase<ImagingState>
    {

#if NET45
        [Obsolete("Use the GetImagingInterface extension method on ClientContext instead.")]
#endif
        public static ImagingInterface GetInterface(ClientContext context, string path)
        {
            var iface = context.getInterface(path);
            return new ImagingInterface(context, iface);
        }

        private ImagingCallback cb;
        private ClientContext context;
        
        public ImagingInterface(ClientContext context, Interface iface) :
            base(iface, null)
        {
            this.context = context;
        }

        protected override void Start()
        {
            cb = new ImagingCallback(this.InterfaceCallback);
            Interface.osvrRegisterImagingCallback(iface.Handle, cb, IntPtr.Zero);
        }

        protected void InterfaceCallback(IntPtr userdata, ref TimeValue timestamp, ref ImagingReport report)
        {
            OnStateChanged(timestamp, report.sensor, report.state);
        }

        protected override void OnStateChanged(TimeValue timestamp, int sensor, ImagingState report)
        {
            base.OnStateChanged(timestamp, sensor, report);
            ImagingInterfaceNative.osvrClientFreeImage(context.ContextHandle, report.data);
        }
    }
}
