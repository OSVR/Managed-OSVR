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
        public OSVR_CBool FullScreen;
        public int Width;
        public int Height;
        public int XPos;
        public int YPos;
        public int BitsPerPixel;
        public uint NumBuffers;
        public OSVR_CBool visible;
    }

    internal static class OpenGLToolkitFunctionsDelegatesNative
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Create(IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Destroy(IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate OSVR_CBool AddOpenGLContext(IntPtr data, ref OpenGLContextParams p);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate OSVR_CBool RemoveOpenGLContext(IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate OSVR_CBool MakeCurrent(IntPtr data, UIntPtr display);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate OSVR_CBool SwapBuffers(IntPtr data, UIntPtr display);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate OSVR_CBool SetVerticalSync(IntPtr data, OSVR_CBool verticalSync);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate OSVR_CBool HandleEvents(IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate OSVR_CBool GetDisplayFrameBuffer(IntPtr data, UIntPtr display, out UIntPtr frameBufferOut);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate OSVR_CBool GetDisplaySizeOverride(IntPtr data, UIntPtr display, out int width, out int height);
    }


    [StructLayout(LayoutKind.Sequential)]
    internal struct OSVR_OpenGLToolkitFunctions
    {
        public UIntPtr Size;
        public IntPtr Data;

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
        public OSVR_OpenGLToolkitFunctions? Toolkit;
    }

    public static class OpenGLToolkitFunctionsDelegates
    {
        public delegate void Create();
        public delegate void Destroy();
        public delegate OSVR_CBool AddOpenGLContext(ref OpenGLContextParams p);
        public delegate OSVR_CBool RemoveOpenGLContext();
        public delegate OSVR_CBool MakeCurrent(UIntPtr display);
        public delegate OSVR_CBool SwapBuffers(UIntPtr display);
        public delegate OSVR_CBool SetVerticalSync(OSVR_CBool verticalSync);
        public delegate OSVR_CBool HandleEvents();
        public delegate OSVR_CBool GetDisplayFrameBuffer(UIntPtr display, out UIntPtr frameBufferOut);
        public delegate OSVR_CBool GetDisplaySizeOverride(UIntPtr display, out int width, out int height);
    }

    public struct OpenGLToolkitFunctions
    {
        public UIntPtr Size;
        public IntPtr Data;

        public OpenGLToolkitFunctionsDelegates.Create Create;
        public OpenGLToolkitFunctionsDelegates.Destroy Destroy;
        public OpenGLToolkitFunctionsDelegates.AddOpenGLContext AddOpenGLContext;
        public OpenGLToolkitFunctionsDelegates.RemoveOpenGLContext RemoveOpenGLContext;
        public OpenGLToolkitFunctionsDelegates.MakeCurrent MakeCurrent;
        public OpenGLToolkitFunctionsDelegates.SwapBuffers SwapBuffers;
        public OpenGLToolkitFunctionsDelegates.SetVerticalSync SetVerticalSync;
        public OpenGLToolkitFunctionsDelegates.HandleEvents HandleEvents;
        public OpenGLToolkitFunctionsDelegates.GetDisplayFrameBuffer GetDisplayFrameBuffer;
        public OpenGLToolkitFunctionsDelegates.GetDisplaySizeOverride GetDisplaySizeOverride;
    }

    public struct GraphicsLibraryOpenGL
    {
        public OpenGLToolkitFunctions? Toolkit;
    }


    #endregion

}
