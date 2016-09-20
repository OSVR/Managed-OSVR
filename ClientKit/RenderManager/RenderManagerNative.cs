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
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using OSVR.ClientKit;
using OSVR_CBool = System.Byte;
using OSVR_RenderInfoCollection = System.IntPtr;
using OSVR_RenderInfoCount = System.Int32;
using OSVR_RenderManagerPresentState = System.IntPtr;
using OSVR_RenderManagerRegisterBufferState = System.IntPtr;
using OSVR_RenderManagerOpenGL = System.IntPtr;
using System.Collections;
using System.Collections.Generic;

namespace OSVR.RenderManager
{
    public sealed class SafeRenderManagerHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeRenderManagerHandle() : base(true) { }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            System.Diagnostics.Debug.WriteLine("[OSVR] ClientContext shutdown");
            return RenderManagerNative.osvrDestroyRenderManager(handle) == OSVR.ClientKit.ClientContext.OSVR_RETURN_SUCCESS;
        }
    }

    internal static class RenderManagerNative
    {
        // we don't use SafeRenderManagerHandle here because we need to call this from
        // SafeRenderManagerHandle.ReleaseHandle
        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrDestroyRenderManager(IntPtr /*OSVR_RenderManager*/ renderManager);

        ///// <summary>
        ///// [OBSOLETE - DO NOT USE]
        ///// </summary>
        //[DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        //public extern static Byte osvrRenderManagerGetNumRenderInfo(
        //    SafeRenderManagerHandle renderManager, RenderParams renderParams,
        //    out OSVR_RenderInfoCount numRenderInfoOut);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrRenderManagerGetDoingOkay(
            SafeRenderManagerHandle renderManager);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrRenderManagerGetDefaultRenderParams(
            out RenderParams renderParamsOut);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrRenderManagerStartPresentRenderBuffers(
            out OSVR_RenderManagerPresentState presentStateOut);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrRenderManagerFinishPresentRenderBuffers(
            SafeRenderManagerHandle renderManager,
            OSVR_RenderManagerPresentState presentState, RenderParams renderParams,
            [MarshalAs(UnmanagedType.I1)]OSVR_CBool shouldFlipY); // @TODO: we need to marshall this bool correctly

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrRenderManagerStartRegisterRenderBuffers(
            out OSVR_RenderManagerRegisterBufferState registerBufferStateOut);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrRenderManagerFinishRegisterRenderBuffers(
            SafeRenderManagerHandle renderManager,
            OSVR_RenderManagerRegisterBufferState registerBufferState,
            [MarshalAs(UnmanagedType.I1)]OSVR_CBool appWillNotOverwriteBeforeNewPresent);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrRenderManagerPresentSolidColorf(
            SafeRenderManagerHandle renderManager,
            RGBFloat rgb);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrRenderManagerGetRenderInfoCollection(
            SafeRenderManagerHandle renderManager,
            RenderParams renderParams,
            out OSVR_RenderInfoCollection renderInfoCollectionOut);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrRenderManagerReleaseRenderInfoCollection(
            OSVR_RenderInfoCollection renderInfoCollection);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrRenderManagerGetNumRenderInfoInCollection(
            OSVR_RenderInfoCollection renderInfoCollection,
            out OSVR_RenderInfoCount countOut);

        #region Utilities

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte OSVR_PoseState_to_OpenGL(
            [In, Out] double[] opengl_out, Pose3 state_in);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte OSVR_PoseState_to_D3D(
            [In, Out] float[] d3d_out, Pose3 state_in);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte OSVR_PoseState_to_Unity(
            out Pose3 state_out, Pose3 state_in);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte OSVR_Projection_to_OpenGL(
            [In, Out] double[] OpenGL_out, ProjectionMatrix projection_in);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte OSVR_Projection_to_D3D(
            [In, Out] float[] d3d_out, ProjectionMatrix projection_in);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte OSVR_Projection_to_Unreal(
            [In, Out] float[] unreal_out, ProjectionMatrix projection_in);

        #endregion

        #region OpenGL

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static byte osvrCreateRenderManagerOpenGL(
            SafeClientContextHandle clientContext,
            [MarshalAs(UnmanagedType.LPStr)] string graphicsLibraryName,
            OSVR_GraphicsLibraryOpenGL graphicsLibrary,
            out SafeRenderManagerHandle renderManager,
            out OSVR_RenderManagerOpenGL renderManagerOpenGL);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static byte osvrRenderManagerOpenDisplayOpenGL(
            OSVR_RenderManagerOpenGL renderManager,
            out OSVR_OpenResultsOpenGL openResultsOut);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static byte osvrRenderManagerPresentRenderBufferOpenGL(
            OSVR_RenderManagerPresentState presentState,
            RenderBufferOpenGL buffer,
            OSVR_RenderInfoOpenGL renderInfoUsed,
            ViewportDescription normalizedCroppingViewport);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static byte osvrRenderManagerRegisterRenderBufferOpenGL(
            OSVR_RenderManagerRegisterBufferState registerBufferState,
            RenderBufferOpenGL renderBuffer);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static byte osvrRenderManagerGetRenderInfoFromCollectionOpenGL(
            OSVR_RenderInfoCollection renderInfoCollection,
            OSVR_RenderInfoCount index,
            out OSVR_RenderInfoOpenGL renderInfoOut);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static byte osvrRenderManagerCreateColorBufferOpenGL(
            uint/*GLsizei*/ width, uint/*GLsizei*/ height, uint/*GLenum*/ format,
            out uint/*GLuint*/ colorBufferNameOut);

        [DllImport(OSVRLibNames.RenderManager, CallingConvention = CallingConvention.Cdecl)]
        public extern static byte osvrRenderManagerCreateDepthBufferOpenGL(
            uint/*GLsizei*/ width, uint/*GLsizei*/ height,
            out uint/*GLuint*/ depthBufferNameOut);

        #endregion
    }

    public static class RenderManagerUtils
    {
        private static void CheckArray(double[] array)
        {
            if(array == null || array.Length != 16)
            {
                throw new ArgumentException("Array must be an array of size 16");
            }
        }

        private static void CheckArray(float[] array)
        {
            if (array == null || array.Length != 16)
            {
                throw new ArgumentException("Array must be an array of size 16");
            }
        }

        private static void CheckReturnCode(byte returnCode)
        {
            if(returnCode != ClientContext.OSVR_RETURN_SUCCESS)
            {
                throw new InvalidOperationException("Native OSVR call failed.");
            }
        }

        /// <summary>
        /// Produce an OpenGL ModelView matrix from an OSVR_PoseState.
        /// Assumes that the world is described in a right-handed fashion and
        /// that we're going to use a right-handed projection matrix.
        /// </summary>
        /// <param name="state">Input state from RenderManager.</param>
        /// <param name="array_out">16-element double array that has
        ///        been allocated by the caller.</param>
        public static void PoseStateToOpenGL(Pose3 state, double[] array_out)
        {
            CheckArray(array_out);
            byte rc = RenderManagerNative.OSVR_PoseState_to_OpenGL(array_out, state);
            CheckReturnCode(rc);
        }

        /// <summary>
        /// Produce a D3D ModelView matrix from an OSVR_PoseState.
        /// Handles transitioning from the right-handed OSVR coordinate
        /// system to the left-handed projection matrix that is typical
        /// for D3D applications.
        /// </summary>
        /// <param name="state">Input state from RenderManager.</param>
        /// <param name="array_out">16-element double array that has
        ///        been allocated by the caller.</param>
        public static void PoseStateToD3D(Pose3 state, float[] array_out)
        {
            CheckArray(array_out);
            byte rc = RenderManagerNative.OSVR_PoseState_to_D3D(array_out, state);
            CheckReturnCode(rc);
        }

        /// <summary>
        /// Modify the OSVR_PoseState from OSVR to be appropriate for use
        /// in a Unity application.  OSVR's world is right handed, and Unity's
        /// is left handed.
        /// </summary>
        /// <param name="state">Input state from RenderManager.</param>
        /// <returns>Ouput state for use by Unity</returns>
        public static Pose3 PoseStateToUnity(Pose3 state)
        {
            Pose3 ret;
            byte rc = RenderManagerNative.OSVR_PoseState_to_Unity(out ret, state);
            CheckReturnCode(rc);
            return ret;
        }

        /// <summary>
        /// Produce an OpenGL Projection matrix from an OSVR_ProjectionMatrix.
        /// Assumes that the world is described in a right-handed fashion and
        /// that we're going to use a right-handed projection matrix.
        /// </summary>
        /// <param name="projection">Input projection description from RenderManager.</param>
        /// <param name="array_out">16-element double array that has
        ///        been allocated by the caller.</param>
        public static void ProjectionToOpenGL(ProjectionMatrix projection, double[] array_out)
        {
            CheckArray(array_out);
            byte rc = RenderManagerNative.OSVR_Projection_to_OpenGL(array_out, projection);
            CheckReturnCode(rc);
        }

        /// <summary>
        /// Produce a D3D Projection matrix from an OSVR_ProjectionMatrix.
        /// Produces a left-handed projection matrix as is typical
        /// for D3D applications.
        /// </summary>
        /// <param name="projection">Input projection description from RenderManager.</param>
        /// <param name="array_out">Pointer to 16-element float array that has
        ///        been allocated by the caller.</param>
        public static void ProjectionToD3D(ProjectionMatrix projection, float[] array_out)
        {
            CheckArray(array_out);
            byte rc = RenderManagerNative.OSVR_Projection_to_D3D(array_out, projection);
            CheckReturnCode(rc);
        }

        /// <summary>
        /// Produce an Unreal Projection matrix from a ProjectionMatrix.
        /// Produces a left-handed projection matrix whose Z values are
        /// in the opposite order, with Z=0 at the far clipping plane and
        /// Z=1 at the near clipping plane.  If there is not a far clipping
        /// plane defined, then set it to be the same as the near
        /// clipping plane before calling this function.  If there is not a
        /// near clipping plane set, then set it to 1 before calling this
        /// function.
        /// To put the result into an Unreal FMatrix, you would do the following (in native code):
        ///   float p[16];
        ///   OSVR_Projection_to_D3D(p, projection_in);
        ///   FPlane row1(p[0], p[1], p[2], p[3]);
        ///   FPlane row2(p[4], p[5], p[6], p[7]);
        ///   FPlane row3(p[8], p[9], p[10], p[11]);
        ///   FPlane row4(p[12], p[13], p[14], p[15]);
        ///   FMatrix ret = FMatrix(row1, row2, row3, row4);
        /// </summary>
        /// <param name="projection">Input projection description from RenderManager.</param>
        /// <param name="array_out">16-element float array that has
        ///        been allocated by the caller.</param>
        public static void ProjectionToUnreal(ProjectionMatrix projection, float[] array_out)
        {
            CheckArray(array_out);
            byte rc = RenderManagerNative.OSVR_Projection_to_Unreal(array_out, projection);
            CheckReturnCode(rc);
        }
    }

    public class RenderManager : IDisposable
    {
        protected SafeRenderManagerHandle mRenderManager;
        protected RenderManager() { }

        ~RenderManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            System.Diagnostics.Debug.WriteLine("[OSVR] In RenderManager.Dispose()");
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("[OSVR] In RenderManager.Dispose({0})", disposing));
            if(disposing)
            {
                if(mRenderManager != null && !mRenderManager.IsInvalid)
                {
                    mRenderManager.Dispose();
                    mRenderManager = null;
                }
            }
        }

        internal SafeRenderManagerHandle Handle
        {
            get { return mRenderManager; }
        }

        public bool DoingOkay
        {
            get
            {
                byte rc = RenderManagerNative.osvrRenderManagerGetDoingOkay(mRenderManager);
                return rc == ClientContext.OSVR_RETURN_SUCCESS;
            }
        }

        public void PresentSolidColor(RGBFloat rgb)
        {
            byte rc = RenderManagerNative.osvrRenderManagerPresentSolidColorf(mRenderManager, rgb);
            if(rc != ClientContext.OSVR_RETURN_SUCCESS)
            {
                throw new InvalidOperationException("native osvrRenderManagerPresentSolidColorf call failed.");
            }
        }
    }

    public class RenderManagerOpenGL : RenderManager
    {
        private OSVR_RenderManagerOpenGL mRenderManagerOpenGL;

        public RenderManagerOpenGL(ClientContext clientContext,
            string graphicsLibraryName,
            GraphicsLibraryOpenGL graphicsLibrary) : base()
        {
            RenderManagerNative.osvrCreateRenderManagerOpenGL(
                clientContext.ContextHandle, graphicsLibraryName,
                graphicsLibrary.ToNative(), out mRenderManager, out mRenderManagerOpenGL);
        }

        public OpenResultsOpenGL OpenDisplay()
        {
            OSVR_OpenResultsOpenGL resultsNative;
            Byte rc = RenderManagerNative.osvrRenderManagerOpenDisplayOpenGL(mRenderManagerOpenGL, out resultsNative);
            if(rc != ClientContext.OSVR_RETURN_SUCCESS)
            {
                throw new InvalidOperationException("osvrRenderManagerOpenDisplayOpenGL call failed.");
            }
            var ret = OpenResultsOpenGL.FromNative(resultsNative);
            return ret;
        }

        public IList<RenderInfoOpenGL> GetRenderInfo(RenderParams renderParams)
        {
            byte rc = 0;
            var ret = new List<RenderInfoOpenGL>();
            OSVR_RenderInfoCollection renderInfoCollection;
            rc = RenderManagerNative.osvrRenderManagerGetRenderInfoCollection(mRenderManager, renderParams, out renderInfoCollection);
            if(rc == ClientContext.OSVR_RETURN_FAILURE)
            {
                throw new InvalidOperationException("osvrRenderManagerGetRenderInfoCollection call failed.");
            }

            int size;
            rc = RenderManagerNative.osvrRenderManagerGetNumRenderInfoInCollection(renderInfoCollection, out size);
            if(rc == ClientContext.OSVR_RETURN_FAILURE)
            {
                throw new InvalidOperationException("osvrRenderManagerGetNumRenderInfoInCollection call failed.");
            }

            try
            {
                for (int i = 0; i < size; i++)
                {
                    OSVR_RenderInfoOpenGL renderInfo;
                    rc = RenderManagerNative.osvrRenderManagerGetRenderInfoFromCollectionOpenGL(renderInfoCollection, i, out renderInfo);
                    if (rc == ClientContext.OSVR_RETURN_FAILURE)
                    {
                        throw new InvalidOperationException("osvrRenderManagerGetRenderInfoInCollectionOpenGL call failed.");
                    }
                    ret.Add(RenderInfoOpenGL.FromNative(renderInfo));
                }

            }
            finally
            {
                rc = RenderManagerNative.osvrRenderManagerReleaseRenderInfoCollection(renderInfoCollection);
                if(rc == ClientContext.OSVR_RETURN_FAILURE)
                {
                    throw new InvalidOperationException("osvrRenderManagerReleaseRenderInfoCollection call failed.");
                }
            }
            return ret;
        }
    }
}
