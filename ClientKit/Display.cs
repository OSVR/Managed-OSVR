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
#if !NETCORE_1_1
using System.Runtime.ConstrainedExecution;
#endif

using ViewportDimension = System.Int32;
using ViewerCount = System.UInt32;
using EyeCount = System.Byte;
using SurfaceCount = System.UInt32;
using DistortionPriority = System.Int32;
using DisplayInputCount = System.Byte;
using DisplayDimension = System.Int32;

namespace OSVR.ClientKit
{
    public sealed class SafeDisplayConfigHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeDisplayConfigHandle() : base(true) { }

#if !NETCORE_1_1
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
#endif
        protected override bool ReleaseHandle()
        {
            return DisplayConfigNative.osvrClientFreeDisplay(handle) == OSVR.ClientKit.ClientContext.OSVR_RETURN_SUCCESS;
        }
    }

    /// <summary>
    /// Parameters for a per-color-component radial distortion shader
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RadialDistortionParameters
    {
        /// <summary>
        /// Vector of K1 coefficients for the R, G, B channels
        /// </summary>
        public Vec3 k1;

        /// <summary>
        /// Center of projection for the radial distortion, relative to the
        /// bounds of this surface.
        /// </summary>
        public Vec2 centerOfProjection;

        public override string ToString()
        {
            return String.Format(
                "k1: (r: {0}, g: {1}, b: {2}), centerOfProjection: (x: {3}, y: {4})",
                k1.x, k1.y, k1.z, centerOfProjection.x, centerOfProjection.y);
        }
    }

    /// <summary>
    /// Dimensions for a display input.
    /// </summary>
    public struct DisplayDimensions
    {
        public DisplayDimension Width { get; set; }
        public DisplayDimension Height { get; set; }
    }

    /// <summary>
    /// Projection clipping planes for a given viewer-eye-surface locus.
    /// </summary>
    public struct ProjectionClippingPlanes
    {
        public double Left { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }
        public double Top { get; set; }
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
        public extern static Byte osvrClientCheckDisplayStartup(SafeDisplayConfigHandle context);

        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrClientGetNumDisplayInputs(SafeDisplayConfigHandle display, out DisplayInputCount numDisplayInputs);

        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrClientGetDisplayDimensions(SafeDisplayConfigHandle display,
            DisplayInputCount displayInputIndex, out DisplayDimension width, out DisplayDimension height);

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
        public extern static Byte osvrClientGetViewerEyeSurfaceDisplayInputIndex(SafeDisplayConfigHandle display,
            ViewerCount viewer, EyeCount eye, SurfaceCount surface, out DisplayInputCount displayInput);

        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrClientGetViewerEyeSurfaceProjectionMatrixd(SafeDisplayConfigHandle display,
            ViewerCount viewer, EyeCount eye, SurfaceCount surface, double near, double far,
            MatrixConventionsFlags flags, out Matrix44d matrix);

        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrClientGetViewerEyeSurfaceProjectionMatrixf(SafeDisplayConfigHandle display,
            ViewerCount viewer, EyeCount eye, SurfaceCount surface, float near, float far,
            MatrixConventionsFlags flags, out Matrix44f matrix);

        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrClientGetViewerEyeSurfaceProjectionClippingPlanes(SafeDisplayConfigHandle display,
            ViewerCount viewer, EyeCount eye, SurfaceCount surface,
            out double left, out double right, out double bottom, out double top);

        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrClientDoesViewerEyeSurfaceWantDistortion(SafeDisplayConfigHandle display,
            ViewerCount viewer, EyeCount eye, SurfaceCount surface, [MarshalAs(UnmanagedType.I1)]out bool distortionRequested);

        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrClientGetViewerEyeSurfaceRadialDistortionPriority(SafeDisplayConfigHandle display,
            ViewerCount viewer, EyeCount eye, SurfaceCount surface, out DistortionPriority priority);

        [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrClientGetViewerEyeSurfaceRadialDistortion(SafeDisplayConfigHandle display,
            ViewerCount viewer, EyeCount eye, SurfaceCount surface, out RadialDistortionParameters distortionParams);
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
        /// Checks to see if a display is fully configured and ready, including
        /// having received its first pose update.
        /// 
        /// Once this first succeeds, it will continue to succeed for the lifetime of
        /// the display config object, so it is not necessary to keep calling once you
        /// get a successful result.
        /// </summary>
        /// <returns>true, if the display config is ready and received its first pose report,
        /// false otherwise.</returns>
        public bool CheckDisplayStartup()
        {
            return DisplayConfigNative.osvrClientCheckDisplayStartup(mHandle) == ClientContext.OSVR_RETURN_SUCCESS;
        }

        /// <summary>
        /// A display config can have one or more display inputs to pass pixels
        /// over (HDMI/DVI connections, etc): retrieve the number of display inputs in
        /// the current configuration.
        /// </summary>
        /// <returns>Number of display inputs in the logical display
        /// topology, **constant** throughout the active, valid lifetime of a display
        /// config object.</returns>
        public DisplayInputCount GetNumDisplayInputs()
        {
            DisplayInputCount ret;
            CheckSuccess(
                DisplayConfigNative.osvrClientGetNumDisplayInputs(mHandle, out ret),
                "[OSVR] DisplayCOnfig.GetNumDisplayInputs(): native osvrClientGetNumDisplayInputs call failed.");
            return ret;
        }

        /// <summary>
        /// Retrieve the pixel dimensions of a given display input for a display config
        /// </summary>
        /// <param name="displayInputIndex">The zero-based index of the display input.</param>
        /// <returns>The height and width of the display input specified. These dimensions are
        /// **constant** throughout the active, valid lifetime of a display config object.</returns>
        public DisplayDimensions GetDisplayDimensions(DisplayInputCount displayInputIndex)
        {
            DisplayDimension width, height;
            CheckSuccess(
                DisplayConfigNative.osvrClientGetDisplayDimensions(mHandle, displayInputIndex,
                    out width, out height),
                    "[OSVR] DisplayConfig.GetDisplayDimensions(): native osvrClientGetDisplayDimensions call failed.");
            return new DisplayDimensions { Width = width, Height = height };
        }

        /// <summary>
        /// A display config can have one (or theoretically more) viewers
        /// retrieve the viewer count.
        /// </summary>
        /// <returns>
        /// Number of viewers in the logical display topology,
        /// constant throughout the active, valid lifetime of a display config
        /// object.
        /// </returns>
        public ViewerCount GetNumViewers()
        {
            ViewerCount ret;
            CheckSuccess(
                DisplayConfigNative.osvrClientGetNumViewers(mHandle, out ret),
                "[OSVR] DisplayConfig.GetNumViewers(): native osvrClientGetNumViewers call failed.");
            return ret;
        }

        /// <summary>
        /// Get the pose of a viewer in a display config.
        /// 
        /// Note that there may not necessarily be any surfaces rendered from this pose
        /// (it's the unused "center" eye in a stereo configuration, for instance) so
        /// only use this if it makes integration into your engine or existing
        /// applications (not originally designed for stereo) easier.
        /// 
        /// Will only succeed if osvrClientCheckDisplayStartup() succeeds.
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
        /// have a substantially similar pose: get the count.
        /// </summary>
        /// <param name="viewer">Viewer ID</param>
        /// <returns>
        /// Number of eyes for this viewer in the logical display
        /// topology, constant throughout the active, valid lifetime of a display
        /// config object
        /// </returns>
        public EyeCount GetNumEyesForViewer(ViewerCount viewer)
        {
            EyeCount ret;
            CheckSuccess(
                DisplayConfigNative.osvrClientGetNumEyesForViewer(mHandle, viewer, out ret),
                "[OSVR] DisplayConfig.GetNumEyesForViewer(): native osvrClientGetNumEyesForViewer call failed.");
            return ret;
        }

        /// <summary>
        /// Get the "viewpoint" for the given eye of a viewer in a display
        /// config. Will only succeed if CheckDisplayStartup() succeeds.
        /// </summary>
        /// <param name="viewer">Viewer ID</param>
        /// <param name="eye">Eye ID</param>
        /// <returns>
        /// Room-space pose (not relative to pose of the viewer)
        /// </returns>
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
        /// Will only succeed if CheckDisplayStartup() succeeds.
        /// </summary>
        /// <param name="viewer">Viewer ID</param>
        /// <param name="eye">Eye ID</param>
        /// <param name="flags">Bitwise OR of matrix convention flags (see OSVR_MatrixFlags)</param>
        /// <returns>
        /// the transformation matrix from room space to eye space (not relative to pose of the viewer)
        /// </returns>
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
        /// Will only succeed if CheckDisplayStartup() returns true.
        /// </summary>
        /// <param name="viewer">Viewer ID</param>
        /// <param name="eye">Eye ID</param>
        /// <param name="flags">Bitwise OR of matrix convention flags (see OSVR_MatrixFlags)</param>
        /// <returns>
        /// the transformation matrix from room space to eye space (not relative to pose of the viewer)
        /// </returns>
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
        /// <returns>
        /// Number of surfaces (numbered [0, surfaces - 1]) for the
        /// given viewer and eye. Constant throughout the active, valid lifetime of
        /// a display config object.
        /// </returns>
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
        ///     input** for a surface seen by an eye of a viewer in a display config. (This
        ///     does not include other video inputs that may be on a single virtual desktop,
        ///     etc. or explicitly account for display configurations that use multiple
        ///     video inputs. It does not necessarily indicate that a viewport in the sense
        ///     of glViewport must be created with these parameters, though the parameter
        ///     order matches for convenience.)
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
        /// Get the index of the display input for a surface seen by an eye of a
        /// viewer in a display config.
        /// 
        /// This is the OSVR-assigned display input: it may not (and in practice,
        /// usually will not) match any platform-specific display indices. This function
        /// exists to associate surfaces with video inputs as enumerated by
        /// GetNumDisplayInputs().
        /// </summary>
        /// <param name="viewer">Viewer ID</param>
        /// <param name="eye">Eye ID</param>
        /// <param name="surface">Surface ID</param>
        /// <returns>Zero-based index of the display input pixels for
        ///  this surface are tranmitted over.
        ///  This association is **constant** throughout the active, valid lifetime of a
        ///  display config object.</returns>
        public DisplayInputCount GetViewerEyeSurfaceDisplayInputIndex(ViewerCount viewer, EyeCount eye, SurfaceCount surface)
        {
            DisplayInputCount ret;
            CheckSuccess(DisplayConfigNative.osvrClientGetViewerEyeSurfaceDisplayInputIndex(mHandle,
                viewer, eye, surface, out ret),
                "[OSVR] DisplayConfig.GetViewerEyeSurfaceDisplayInputIndex(): native osvrClientGetViewerEyeSurfaceDisplayInputIndex call failed");
            return ret;
        }

        /// <summary>
        /// Get the projection matrix for a surface seen by an eye of a viewer
        /// in a display config. (double version)
        /// </summary>
        /// <param name="viewer">Viewer ID</param>
        /// <param name="eye">Eye ID</param>
        /// <param name="surface">Surface ID</param>
        /// <param name="near">Distance from viewpoint to near clipping plane - must be positive..</param>
        /// <param name="far">Distance from viewpoint to far clipping plane - must be positive
        /// and not equal to near, typically greater than near.</param>
        /// <param name="flags">Bitwise OR of matrix convention flags (see MatrixConventionFlags)</param>
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
        /// <param name="near">Distance from viewpoint to near clipping plane - must be positive..</param>
        /// <param name="far">Distance from viewpoint to far clipping plane - must be positive
        /// and not equal to near, typically greater than near.</param>
        /// <param name="flags">Bitwise OR of matrix convention flags (see OSVR_MatrixFlags)</param>
        public Matrix44f GetProjectionMatrixForViewerEyeSurfacef(ViewerCount viewer, EyeCount eye, SurfaceCount surface, float near, float far, MatrixConventionsFlags flags)
        {
            Matrix44f ret;
            CheckSuccess(
                DisplayConfigNative.osvrClientGetViewerEyeSurfaceProjectionMatrixf(mHandle, viewer, eye, surface, near, far, flags, out ret),
                "[OSVR] DisplayConfig.GetProjectionForViewerEyeSurface(): native osvrClientGetProjectionForViewerEyeSurface call failed.");
            return ret;
        }

        /// <summary>
        /// Get the clipping planes (positions at unit distance) for a surface
        /// seen by an eye of a viewer
        /// in a display config.
        ///
        /// This is only for use in integrations that cannot accept a fully-formulated
        /// projection matrix as returned by
        /// osvrClientGetViewerEyeSurfaceProjectionMatrixf() or
        /// osvrClientGetViewerEyeSurfaceProjectionMatrixd(), and may not necessarily
        /// provide the same optimizations.
        /// 
        /// As all the planes are given at unit (1) distance, before passing these
        /// planes to a consuming function in your application/engine, you will typically
        /// divide them by your near clipping plane distance.
        /// </summary>
        /// <param name="viewer">Viewer ID</param>
        /// <param name="eye">Eye ID</param>
        /// <param name="surface">Surface ID</param>
        public ProjectionClippingPlanes GetViewerEyeSurfaceProjectionClippingPlanes(ViewerCount viewer, EyeCount eye, SurfaceCount surface)
        {
            double left, right, top, bottom;
            CheckSuccess(
                DisplayConfigNative.osvrClientGetViewerEyeSurfaceProjectionClippingPlanes(mHandle, viewer, eye, surface, out left, out right, out bottom, out top),
                "[OSVR] DisplayConfig.GetViewerEyeSurfaceProjectionClippingPlanes(): native osvrClientGetViewerEyeSurfaceProjectionClippingPlanes call failed.");
            return new ProjectionClippingPlanes
            {
                Left = left,
                Right = right,
                Bottom = bottom,
                Top = top
            };
        }

        /// <summary>
        /// Determines if a surface seen by an eye of a viewer in a display
        /// config requests some distortion to be performed.
        /// 
        /// This simply reports true or false, and does not specify which kind of
        /// distortion implementations have been parameterized for this display. For
        /// each distortion implementation your application supports, you'll want to
        /// call the corresponding priority function to find out if it is available.
        /// </summary>
        /// <param name="viewer">Viewer ID</param>
        /// <param name="eye">Eye ID</param>
        /// <param name="surface">Surface ID</param>
        /// <returns>
        /// whether distortion is requested. Constant throughout the active, valid
        /// lifetime of a display config object.
        /// </returns>
        public bool DoesViewerEyeSurfaceWantDistortion(ViewerCount viewer, EyeCount eye, SurfaceCount surface)
        {
            bool ret;
            CheckSuccess(
                DisplayConfigNative.osvrClientDoesViewerEyeSurfaceWantDistortion(mHandle, viewer, eye, surface, out ret),
                "[OSVR] DisplayConfig.DoesViewerEyeSurfaceWantDistortion(): native osvrClientDoesViewerEyeSurfaceWantDistortion call failed.");
            return ret;
        }

        /// <summary>
        /// Returns the priority/availability of radial distortion parameters for
        /// a surface seen by an eye of a viewer in a display config.
        /// 
        /// If osvrClientDoesViewerEyeSurfaceWantDistortion() reports false, then the
        /// display does not request distortion of any sort, and thus neither this nor
        /// any other distortion strategy priority function will report an "available"
        /// priority.
        /// </summary>
        /// <param name="viewer">Viewer ID</param>
        /// <param name="eye">Eye ID</param>
        /// <param name="surface">Surface ID</param>
        /// <returns>
        /// the priority level. Negative values indicate this technique
        /// not available. Higher values indicate higher preference for the given
        /// technique based on the device's description. Constant throughout the
        /// active, valid lifetime of a display config object.
        /// </returns>
        public DistortionPriority GetViewerEyeSurfaceRadialDistortionPriority(ViewerCount viewer, EyeCount eye, SurfaceCount surface)
        {
            DistortionPriority ret;
            CheckSuccess(
                DisplayConfigNative.osvrClientGetViewerEyeSurfaceRadialDistortionPriority(mHandle, viewer, eye, surface, out ret),
                "[OSVR] DisplayConfig.GetViewerEyeSurfaceWantDistortion(): native osvrClientGetViewerEyeSurfaceRadialDistortionPriority call failed.");
            return ret;
        }

        /// <summary>
        /// Returns the radial distortion parameters, if known/requested, for a
        /// surface seen by an eye of a viewer in a display config.
        /// 
        /// Will only succeed if GetViewerEyeSurfaceRadialDistortionPriority()
        /// reports a non-negative priority.
        /// </summary>
        /// <param name="viewer">Viewer ID</param>
        /// <param name="eye">Eye ID</param>
        /// <param name="surface">Surface ID</param>
        /// <returns>the parameters for radial distortion</returns>
        public RadialDistortionParameters GetViewerEyeSurfaceRadialDistortion(ViewerCount viewer, EyeCount eye, SurfaceCount surface)
        {
            RadialDistortionParameters ret;
            CheckSuccess(
                DisplayConfigNative.osvrClientGetViewerEyeSurfaceRadialDistortion(mHandle, viewer, eye, surface, out ret),
                "[OSVR] DisplayConfig.GetViewerEyeSurfaceRadialDistortion(): native osvrClientGetViewerEyeSurfaceRadialDistortion call failed.");
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
