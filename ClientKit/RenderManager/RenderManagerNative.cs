/// Managed-OSVR binding
///
/// <copyright>
/// Copyright 2016 Sensics, Inc. and contributors
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
using OSVR_CBool = System.Byte;
using OSVR_RenderInfoCollection = System.IntPtr;
using OSVR_RenderInfoCount = System.Int32;
using OSVR_RenderManager = System.IntPtr;
using OSVR_RenderManagerPresentState = System.IntPtr;
using OSVR_RenderManagerRegisterBufferState = System.IntPtr;

namespace OSVR.RenderManager
{
    internal static class RenderManagerNative
    {
        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrDestroyRenderManager(OSVR_RenderManager renderManager);

        ///// <summary>
        ///// [OBSOLETE - DO NOT USE]
        ///// </summary>
        //[DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        //public extern static Byte osvrRenderManagerGetNumRenderInfo(
        //    OSVR_RenderManager renderManager, RenderParams renderParams,
        //    out OSVR_RenderInfoCount numRenderInfoOut);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrRenderManagerGetDoingOkay(
            OSVR_RenderManager renderManager);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrRenderManagerGetDefaultRenderParams(
            out RenderParams renderParamsOut);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrRenderManagerStartPresentRenderBuffers(
            out OSVR_RenderManagerPresentState presentStateOut);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrRenderManagerFinishPresentRenderBuffers(
            OSVR_RenderManager renderManager,
            OSVR_RenderManagerPresentState presentState, RenderParams renderParams,
            OSVR_CBool shouldFlipY); // @TODO: we need to marshall this bool correctly

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrRenderManagerStartRegisterRenderBuffers(
            out OSVR_RenderManagerRegisterBufferState registerBufferStateOut);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrRenderManagerFinishRegisterRenderBuffers(
            OSVR_RenderManager renderManager,
            OSVR_RenderManagerRegisterBufferState registerBufferState,
            OSVR_CBool appWillNotOverwriteBeforeNewPresent);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrRenderManagerPresentSolidColorf(
            OSVR_RenderManager renderManager,
            RGBFloat rgb);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrRenderManagerGetRenderInfoCollection(
            OSVR_RenderManager renderManager,
            RenderParams renderParams,
            out OSVR_RenderInfoCollection renderInfoCollectionOut);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrRenderManagerReleaseRenderInfoCollection(
            OSVR_RenderInfoCollection renderInfoCollection);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrRenderManagerGetNumRenderInfoInCollection(
            OSVR_RenderInfoCollection renderInfoCollection,
            out OSVR_RenderInfoCount countOut);
    }
}
