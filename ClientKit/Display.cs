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

﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;

using ViewportDimension = System.Int32;
using ViewerCount = System.UInt32;
using EyeCount = System.Byte;
using SurfaceCount = System.UInt32;

namespace OSVR.ClientKit
{
    public sealed class SafeDisplayConfigHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeDisplayConfigHandle() : base(true) { }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            return DisplayConfigNative.osvrClientFreeDisplay(handle) == OSVR.ClientKit.ClientContext.OSVR_RETURN_SUCCESS;
        }
    }

    internal static class DisplayConfigNative {
#if MANAGED_OSVR_INTERNAL_PINVOKE
            // On iOS and Xbox 360, plugins are statically linked into
            // the executable, so we have to use __Internal as the
            // library name.
            private const string OSVRCoreDll = "__Internal";
#else
            private const string OSVRCoreDll = "osvrClientKit";
#endif

        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrClientGetDisplay(SafeClientContextHandle context, out SafeDisplayConfigHandle display);

        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrClientFreeDisplay(IntPtr display);

        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrClientGetNumViewers(SafeDisplayConfigHandle display, out ViewerCount viewers);

        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrClientGetViewerPose(SafeDisplayConfigHandle display,
            ViewerCount viewer, out Pose3 pose);

        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrClientGetNumEyesForViewer(SafeDisplayConfigHandle display,
            ViewerCount viewer, out EyeCount eyes);

        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrClientGetViewerEyePose(SafeDisplayConfigHandle display,
            ViewerCount viewer, EyeCount eye, out Pose3 pose);

        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrClientGetViewerEyeViewMatrixd(SafeDisplayConfigHandle display,
            ViewerCount viewer, EyeCount eye, MatrixConventionsFlags flags, out Matrix44d mat);

        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrClientGetViewerEyeViewMatrixf(SafeDisplayConfigHandle display,
            ViewerCount viewer, EyeCount eye, MatrixConventionsFlags flags, out Matrix44f mat);

        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrClientGetNumSurfacesForViewerEye(SafeDisplayConfigHandle display,
            ViewerCount viewer, EyeCount eye, out SurfaceCount surfaces);

        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrClientGetRelativeViewportForViewerEyeSurface(SafeDisplayConfigHandle display,
            ViewerCount viewer, EyeCount eye, SurfaceCount surface,
            out ViewportDimension left, out ViewportDimension bottom, out ViewportDimension width, out ViewportDimension height);

        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrClientGetViewerEyeSurfaceProjectionMatrixd(SafeDisplayConfigHandle display,
            ViewerCount viewer, EyeCount eye, SurfaceCount surface, double near, double far,
            MatrixConventionsFlags flags, out Matrix44d matrix);

        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrClientGetViewerEyeSurfaceProjectionMatrixf(SafeDisplayConfigHandle display,
            ViewerCount viewer, EyeCount eye, SurfaceCount surface, double near, double far,
            MatrixConventionsFlags flags, out Matrix44f matrix);
    }

    public struct Viewport
    {
        /// <summary>
        /// Distance in pixels from the left of the video input to the left of the viewport.
        /// </summary>
        public ViewportDimension Left { get; set; }

        /// <summary>
        /// Distance in pixels from the bottom of the video input to the bottom of the viewport.
        /// </summary>
        public ViewportDimension Bottom { get; set; }

        /// <summary>
        ///  Width of viewport in pixels.
        /// </summary>
        public ViewportDimension Width { get; set; }

        /// <summary>
        /// Height of viewport in pixels.
        /// </summary>
        public ViewportDimension Height { get; set; }

        public override string ToString()
        {
            return String.Format("(left: {0}, bottom: {1}, width: {2}, height: {3})",
                Left, Bottom, Width, Height);
        }
    }

    /// <summary>
    /// OSVR display configuration object. Allows you to query viewer,eye,surface matrices.
    /// </summary>
    public class DisplayConfig : IDisposable
    {
        private SafeDisplayConfigHandle mHandle;
        internal DisplayConfig(SafeDisplayConfigHandle handle)
        {
            mHandle = handle;
        }

        private void CheckSuccess(Byte returnCode, string message)
        {
            if(returnCode != OSVR.ClientKit.ClientContext.OSVR_RETURN_SUCCESS)
            {
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// A display config can have one (or theoretically more) viewers
        /// </summary>
        public ViewerCount GetNumViewers()
        {
            ViewerCount ret;
            CheckSuccess(
                DisplayConfigNative.osvrClientGetNumViewers(mHandle, out ret),
                "[OSVR] DisplayConfig.GetNumViewers(): native osvrClientGetNumViewers call failed.");
            return ret;
        }

        /// <summary>
        /// Get the center of projection/"eye point" for a viewer in a display config.
        /// Note that there may not necessarily be any surfaces rendered from this pose
        /// (it's the unused "center" eye in a stereo configuration) so only use this if
        /// it makes integration into your engine or existing applications (not
        /// originally designed for stereo) easier.
        /// </summary>
        /// <param name="viewer">Viewer ID</param>
        public Pose3 GetViewerPose(ViewerCount viewer)
        {
            Pose3 ret;
            CheckSuccess(
                DisplayConfigNative.osvrClientGetViewerPose(mHandle, viewer, out ret),
                "[OSVR] DisplayConfig.GetViewerPose(): native osvrClientGetViewerPose call failed.");

            return ret;
        }

        /// <summary>
        /// Each viewer in a display config can have one or more "eyes" which
        /// have a substantially similar pose.
        /// </summary>
        /// <param name="viewer">Viewer ID</param>
        public EyeCount GetNumEyesForViewer(ViewerCount viewer)
        {
            EyeCount ret;
            CheckSuccess(
                DisplayConfigNative.osvrClientGetNumEyesForViewer(mHandle, viewer, out ret),
                "[OSVR] DisplayConfig.GetNumEyesForViewer(): native osvrClientGetNumEyesForViewer call failed.");
            return ret;
        }

        /// <summary>
        /// Get the center of projection/"eye point" for the given eye of a
        /// viewer in a display config
        /// </summary>
        /// <param name="viewer">Viewer ID</param>
        /// <param name="eye">Eye ID</param>
        public Pose3 GetViewerEyePose(ViewerCount viewer, EyeCount eye)
        {
            Pose3 ret;
            CheckSuccess(
                DisplayConfigNative.osvrClientGetViewerEyePose(mHandle, viewer, eye, out ret),
                "[OSVR] DisplayConfig.GetViewerEyePose(): native osvrClientGetViewerPose call failed.");
            return ret;
        }

        /// <summary>
        /// Get the view matrix (inverse of pose) for the given eye of a
        /// viewer in a display config - matrix of doubles.
        /// </summary>
        /// <param name="viewer">Viewer ID</param>
        /// <param name="eye">Eye ID</param>
        /// <param name="flags">Bitwise OR of matrix convention flags (see OSVR_MatrixFlags)</param>
        public Matrix44d GetViewerEyeViewMatrixd(ViewerCount viewer, EyeCount eye, MatrixConventionsFlags flags)
        {
            Matrix44d ret;
            CheckSuccess(
                DisplayConfigNative.osvrClientGetViewerEyeViewMatrixd(mHandle, viewer, eye, flags, out ret),
                "[OSVR] DisplayConfig.GetViewerEyeViewMatrixd(): native osvrClientGetViewerEyeViewMatrixd call failed.");
            return ret;
        }

        /// <summary>
        /// Get the view matrix (inverse of pose) for the given eye of a
        /// viewer in a display config - matrix of floats.
        /// </summary>
        /// <param name="viewer">Viewer ID</param>
        /// <param name="eye">Eye ID</param>
        /// <param name="flags">Bitwise OR of matrix convention flags (see OSVR_MatrixFlags)</param>
        public Matrix44f GetViewerEyeViewMatrixf(ViewerCount viewer, EyeCount eye, MatrixConventionsFlags flags)
        {
            Matrix44f ret;
            CheckSuccess(
                DisplayConfigNative.osvrClientGetViewerEyeViewMatrixf(mHandle, viewer, eye, flags, out ret),
                "[OSVR] DisplayConfig.GetViewerEyeViewMatrixf(): native osvrClientGetViewerEyeViewMatrixf call failed.");
            return ret;
        }

        /// <summary>
        /// Each eye of each viewer in a display config has one or more surfaces
        /// (aka "screens") on which content should be rendered.
        /// </summary>
        /// <param name="viewer">Viewer ID</param>
        /// <param name="eye">Eye ID</param>
        public SurfaceCount GetNumSurfacesForViewerEye(ViewerCount viewer, EyeCount eye)
        {
            SurfaceCount ret;
            CheckSuccess(
                DisplayConfigNative.osvrClientGetNumSurfacesForViewerEye(mHandle, viewer, eye, out ret),
                "[OSVR] DisplayConfig.GetNumSurfacesForViewerEye(): native osvrClientGetNumSurfacesForViewerEye call failed.");
            return ret;
        }

        /// <summary>
        /// Get the dimensions/location of the viewport **within the display
        /// input** for a surface seen by an eye of a viewer
        /// in a display config. (This does not include other video inputs that may be
        /// on a single virtual desktop, etc. and does not necessarily indicate that a
        /// viewport in the sense of glViewport must be created with these parameters,
        /// though the output order matches for convenience.)
        /// </summary>
        /// <param name="viewer">Viewer ID</param>
        /// <param name="eye">Eye ID</param>
        /// <param name="surface">Surface ID</param>
        public Viewport GetRelativeViewportForViewerEyeSurface(ViewerCount viewer, EyeCount eye, SurfaceCount surface)
        {
            ViewportDimension left, bottom, width, height;
            CheckSuccess(
                DisplayConfigNative.osvrClientGetRelativeViewportForViewerEyeSurface(mHandle, viewer, eye, surface,
                out left, out bottom, out width, out height),
                "[OSVR] DisplayConfig.GetRelativeViewportForViewerEyeSurface(): native osvrClientGetRelativeViewportForViewerEyeSurface call failed.");

            return new Viewport
            {
                Left = left,
                Bottom = bottom,
                Width = width,
                Height = height,
            };
        }

        /// <summary>
        /// Get the projection matrix for a surface seen by an eye of a viewer
        /// in a display config. (double version)
        /// </summary>
        /// <param name="viewer">Viewer ID</param>
        /// <param name="eye">Eye ID</param>
        /// <param name="surface">Surface ID</param>
        /// <param name="near">Distance to near clipping plane - must be nonzero, typically positive.</param>
        /// <param name="far">Distance to far clipping plane - must be nonzero, typically
        ///     positive and greater than near.</param>
        /// <param name="flags">Bitwise OR of matrix convention flags (see OSVR_MatrixFlags)</param>
        public Matrix44d GetProjectionMatrixForViewerEyeSurfaced(ViewerCount viewer, EyeCount eye, SurfaceCount surface, double near, double far, MatrixConventionsFlags flags)
        {
            Matrix44d ret;
            CheckSuccess(
                DisplayConfigNative.osvrClientGetViewerEyeSurfaceProjectionMatrixd(mHandle, viewer, eye, surface, near, far, flags, out ret),
                "[OSVR] DisplayConfig.GetProjectionForViewerEyeSurface(): native osvrClientGetProjectionForViewerEyeSurface call failed.");
            return ret;
        }

        /// <summary>
        /// Get the projection matrix for a surface seen by an eye of a viewer
        /// in a display config. (float version)
        /// </summary>
        /// <param name="viewer">Viewer ID</param>
        /// <param name="eye">Eye ID</param>
        /// <param name="surface">Surface ID</param>
        /// <param name="near">Distance to near clipping plane - must be nonzero, typically positive.</param>
        /// <param name="far">Distance to far clipping plane - must be nonzero, typically
        ///     positive and greater than near.</param>
        /// <param name="flags">Bitwise OR of matrix convention flags (see OSVR_MatrixFlags)</param>
        public Matrix44f GetProjectionMatrixForViewerEyeSurfacef(ViewerCount viewer, EyeCount eye, SurfaceCount surface, double near, double far, MatrixConventionsFlags flags)
        {
            Matrix44f ret;
            CheckSuccess(
                DisplayConfigNative.osvrClientGetViewerEyeSurfaceProjectionMatrixf(mHandle, viewer, eye, surface, near, far, flags, out ret),
                "[OSVR] DisplayConfig.GetProjectionForViewerEyeSurface(): native osvrClientGetProjectionForViewerEyeSurface call failed.");
            return ret;
        }

        public void Dispose()
        {
            System.Diagnostics.Debug.WriteLine("[OSVR] In Display.Dispose()");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("[OSVR] In Interface.Dispose({0})", disposing));
            if (disposing)
            {
                if (mHandle != null && !mHandle.IsInvalid)
                {
                    mHandle.Dispose();
                    mHandle = null;
                }
            }
        }
    }
}
