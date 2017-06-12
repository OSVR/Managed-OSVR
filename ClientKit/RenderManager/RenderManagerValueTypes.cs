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
using OSVR.ClientKit;
using System;
using System.Runtime.InteropServices;
using OSVR_CBool = System.Boolean;

namespace OSVR.RenderManager
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RenderParams
    {
        /// <summary>
        /// Room space to insert
        /// </summary>
        public Pose3 WorldFromRoomAppend;

        /// <summary>
        /// Overrides head space
        /// </summary>
        public Pose3 RoomFromHeadReplace;

        public double NearClipDistanceMeters;
        public double FarClipDistanceMeters;

        public static RenderParams Default
        {
            get 
            {
                RenderParams ret;
                RenderManagerNative.osvrRenderManagerGetDefaultRenderParams(out ret);
                return ret;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ProjectionMatrix
    {
        public double Left;
        public double Right;
        public double Top;
        public double Bottom;
        public double NearClip;
        public double FarClip;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ViewportDescription
    {
        public double Left;
        public double Lower;
        public double Width;
        public double Height;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RGBFloat
    {
        public float R;
        public float G;
        public float B;
    }

    public enum OpenStatus
    {
        Failure = 0,
        Partial = 1,
        Complete = 2,
    }

    #region OpenGL

    [StructLayout(LayoutKind.Sequential)]
    public struct OpenGLContextParams
    {
        public string WindowTitle;
        [MarshalAs(UnmanagedType.I1)]
        public OSVR_CBool FullScreen;
        public int Width;
        public int Height;
        public int XPos;
        public int YPos;
        public int BitsPerPixel;
        public uint NumBuffers;
        [MarshalAs(UnmanagedType.I1)]
        public OSVR_CBool visible;
    }

    internal static class OpenGLToolkitFunctionsDelegatesNative
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Create(IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Destroy(IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public delegate OSVR_CBool AddOpenGLContext(IntPtr data, ref OpenGLContextParams p);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public delegate OSVR_CBool RemoveOpenGLContext(IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public delegate OSVR_CBool MakeCurrent(IntPtr data, UIntPtr display);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public delegate OSVR_CBool SwapBuffers(IntPtr data, UIntPtr display);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public delegate OSVR_CBool SetVerticalSync(IntPtr data, OSVR_CBool verticalSync);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public delegate OSVR_CBool HandleEvents(IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public delegate OSVR_CBool GetDisplayFrameBuffer(IntPtr data, UIntPtr display, out uint frameBufferOut);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public delegate OSVR_CBool GetDisplaySizeOverride(IntPtr data, UIntPtr display, out int width, out int height);
    }


    [StructLayout(LayoutKind.Sequential)]
    internal struct OSVR_OpenGLToolkitFunctions
    {
        public UIntPtr Size;
        public IntPtr Data;

        // @todo: do we need the UnmanagedFunctionPointer here or in the delegate declarations above?
        public OpenGLToolkitFunctionsDelegatesNative.Create Create;
        public OpenGLToolkitFunctionsDelegatesNative.Destroy Destroy;
        public OpenGLToolkitFunctionsDelegatesNative.AddOpenGLContext AddOpenGLContext;
        public OpenGLToolkitFunctionsDelegatesNative.RemoveOpenGLContext RemoveOpenGLContext;
        public OpenGLToolkitFunctionsDelegatesNative.MakeCurrent MakeCurrent;
        public OpenGLToolkitFunctionsDelegatesNative.SwapBuffers SwapBuffers;
        public OpenGLToolkitFunctionsDelegatesNative.SetVerticalSync SetVerticalSync;
        public OpenGLToolkitFunctionsDelegatesNative.HandleEvents HandleEvents;
        public OpenGLToolkitFunctionsDelegatesNative.GetDisplayFrameBuffer GetDisplayFrameBuffer;
        public OpenGLToolkitFunctionsDelegatesNative.GetDisplaySizeOverride GetDisplaySizeOverride;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct OSVR_GraphicsLibraryOpenGL
    {
        /// <summary>
        /// Represents a native OSVR_OpenGLToolkitFunctions*
        /// </summary>
        public IntPtr Toolkit;
    }

    public class OpenGLToolkitFunctions
    {
        /// <summary>
        /// Represents a native OSVR_OpenGLToolkitFunctions*
        /// </summary>
        private IntPtr mNativePtr;

        /// <summary>
        /// Allocates and returns a native OSVR_OpenGLToolkitFunctions* that can be passed to native code.
        /// </summary>
        internal IntPtr ToNative()
        {
            OSVR_OpenGLToolkitFunctions ret = new OSVR_OpenGLToolkitFunctions();
            ret.Size = (UIntPtr)Marshal.SizeOf(ret); // @todo make sure this lines up with the native code size
            ret.Data = IntPtr.Zero;

            // Fill in the native callbacks
            ret.Create = CreateNative;
            ret.Destroy = DestroyNative;
            ret.AddOpenGLContext = AddOpenGLContextNative;
            ret.RemoveOpenGLContext = RemoveOpenGLContextNative;
            ret.MakeCurrent = MakeCurrentNative;
            ret.SwapBuffers = SwapBuffersNative;
            ret.HandleEvents = HandleEventsNative;
            ret.GetDisplayFrameBuffer = GetDisplayFrameBufferNative;
            ret.GetDisplaySizeOverride = GetDisplaySizeOverrideNative;

            // Allocate a native struct on the heap and pin it.
            // @todo does RenderManager take ownership of the lifetime of this object?
            // If so, will that work with Marshall.AllocHGlobal allocated memory?
            mNativePtr = Marshal.AllocHGlobal(Marshal.SizeOf(ret));

            // the third argument should be false the first time this is called,
            // and true for each subsequent time it's called. Otherwise this will
            // leak
            Marshal.StructureToPtr(ret, mNativePtr, mNativePtr != IntPtr.Zero);

            return mNativePtr;
        }

        #region Managed Callbacks

        protected virtual void Create()
        {

        }

        protected virtual void Destroy()
        {

        }

        protected virtual OSVR_CBool AddOpenGLContext(ref OpenGLContextParams p)
        {
            return false;
        }

        protected virtual OSVR_CBool RemoveOpenGLContext()
        {
            return false;
        }

        protected virtual OSVR_CBool MakeCurrent(UIntPtr display)
        {
            return false;
        }

        protected virtual OSVR_CBool SwapBuffers(UIntPtr display)
        {
            return false;
        }

        protected virtual OSVR_CBool SetVerticalSync(OSVR_CBool verticalSync)
        {
            return false;
        }

        protected virtual OSVR_CBool HandleEvents()
        {
            return false;
        }

        protected virtual OSVR_CBool GetDisplayFrameBuffer(UIntPtr display, out uint frameBufferOut)
        {
            frameBufferOut = 0;
            return false;
        }

        protected virtual OSVR_CBool GetDisplaySizeOverride(UIntPtr display, out int width, out int height)
        {
            width = height = 0;
            return false;
        }

        #endregion

        #region Native Callbacks

        private void CreateNative(IntPtr data)
        {
            Create();
        }

        private void DestroyNative(IntPtr data)
        {
            Destroy();
        }

        private OSVR_CBool AddOpenGLContextNative(IntPtr data, ref OpenGLContextParams p)
        {
            return AddOpenGLContext(ref p);
        }

        private OSVR_CBool RemoveOpenGLContextNative(IntPtr data)
        {
            return RemoveOpenGLContext();
        }

        private OSVR_CBool MakeCurrentNative(IntPtr data, UIntPtr display)
        {
            return MakeCurrent(display);
        }

        private OSVR_CBool SwapBuffersNative(IntPtr data, UIntPtr display)
        {
            return SwapBuffers(display);
        }

        private OSVR_CBool SetVerticalSyncNative(IntPtr data, OSVR_CBool verticalSync)
        {
            return SetVerticalSync(verticalSync);
        }

        private OSVR_CBool HandleEventsNative(IntPtr data)
        {
            return HandleEvents();
        }

        private OSVR_CBool GetDisplayFrameBufferNative(IntPtr data, UIntPtr display, out uint frameBufferOut)
        {
            return GetDisplayFrameBuffer(display, out frameBufferOut);
        }

        private OSVR_CBool GetDisplaySizeOverrideNative(IntPtr data, UIntPtr display, out int width, out int height)
        {
            return GetDisplaySizeOverride(display, out width, out height);
        }

        #endregion

    }

    public struct GraphicsLibraryOpenGL
    {
        public OpenGLToolkitFunctions Toolkit;

        internal OSVR_GraphicsLibraryOpenGL ToNative()
        {
            OSVR_GraphicsLibraryOpenGL ret = new OSVR_GraphicsLibraryOpenGL();
            ret.Toolkit = Toolkit != null ? Toolkit.ToNative() : IntPtr.Zero;
            return ret;
        }
    }

    public struct RenderBufferOpenGL
    {
        public uint/*GLuint*/ ColorBufferName;
        public uint/*GLuint*/ DepthStencilBufferName;
    }

    internal struct OSVR_RenderInfoOpenGL
    {
        public OSVR_GraphicsLibraryOpenGL Library;
        public ViewportDescription Viewport;
        public Pose3 Pose;
        public ProjectionMatrix Projection;
    }

    public struct RenderInfoOpenGL
    {
        public ViewportDescription Viewport;
        public Pose3 Pose;
        public ProjectionMatrix Projection;

        internal static RenderInfoOpenGL FromNative(OSVR_RenderInfoOpenGL renderInfoNative)
        {
            RenderInfoOpenGL ret = new RenderInfoOpenGL();
            ret.Pose = renderInfoNative.Pose;
            ret.Projection = renderInfoNative.Projection;
            ret.Viewport = renderInfoNative.Viewport;
            return ret;
        }

        internal OSVR_RenderInfoOpenGL ToNative()
        {
            OSVR_RenderInfoOpenGL ret = new OSVR_RenderInfoOpenGL();

            // @todo do we need to retain this from the native OSVR_RenderInfoOpenGL?
            ret.Library = new OSVR_GraphicsLibraryOpenGL();
            ret.Library.Toolkit = IntPtr.Zero;

            ret.Pose = Pose;
            ret.Projection = Projection;
            ret.Viewport = Viewport;
            return ret;
        }
    }

    internal struct OSVR_OpenResultsOpenGL
    {
        public OpenStatus Status;
        public OSVR_GraphicsLibraryOpenGL Library;
        public RenderBufferOpenGL Buffers; // @todo: why is this plural?
    }

    public struct OpenResultsOpenGL
    {
        public OpenStatus Status;
        public RenderBufferOpenGL Buffers; // @todo: why is this plural?

        internal static OpenResultsOpenGL FromNative(OSVR_OpenResultsOpenGL resultsNative)
        {
            OpenResultsOpenGL ret = new OpenResultsOpenGL();
            ret.Status = resultsNative.Status;
            ret.Buffers = resultsNative.Buffers;
            return ret;
        }
    }


    #endregion

}
