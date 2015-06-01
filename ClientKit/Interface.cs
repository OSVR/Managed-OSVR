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
using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace OSVR
{

    namespace ClientKit
    {
        public sealed class SafeClientInterfaceHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public SafeClientInterfaceHandle() : base(true) { }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            override protected bool ReleaseHandle()
            {
                System.Diagnostics.Debug.WriteLine("Interface shutdown");
                return Interface.osvrClientFreeInterface(handle) == OSVR.ClientKit.ClientContext.OSVR_RETURN_SUCCESS;
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void PositionCallback(IntPtr /*void*/ userdata, ref TimeValue timestamp, ref PositionReport report);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void PoseCallback(IntPtr /*void*/ userdata, ref TimeValue timestamp, ref PoseReport report);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void OrientationCallback(IntPtr /*void*/ userdata, ref TimeValue timestamp, ref OrientationReport report);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ButtonCallback(IntPtr /*void*/ userdata, ref TimeValue timestamp, ref ButtonReport report);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void AnalogCallback(IntPtr /*void*/ userdata, ref TimeValue timestamp, ref AnalogReport report);

        /// @brief Interface handle object. Typically acquired from a ClientContext.
        /// @ingroup ClientKitCPP
        public class Interface : IDisposable
        {

            #region ClientKit C functions
#if UNITY_IPHONE || UNITY_XBOX360
			// On iOS and Xbox 360, plugins are statically linked into
			// the executable, so we have to use __Internal as the
			// library name.
			const string OSVR_CORE_DLL = "__Internal";
#else
            const string OSVR_CORE_DLL = "osvrClientKit";
#endif

            //typedef struct OSVR_ClientContextObject *OSVR_ClientContext;
            //typedef struct OSVR_ClientInterfaceObject *OSVR_ClientInterface;
            //typedef char OSVR_ReturnCode; (0 == OSVR_RETURN_SUCCESS; 1 == OSVR_RETURN_FAILURE)


            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrRegisterPositionCallback(SafeClientInterfaceHandle iface, [MarshalAs(UnmanagedType.FunctionPtr)] PositionCallback cb, IntPtr /*void**/ userdata);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrRegisterPoseCallback(SafeClientInterfaceHandle iface, PoseCallback cb, IntPtr /*void**/ userdata);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrRegisterOrientationCallback(SafeClientInterfaceHandle iface, [MarshalAs(UnmanagedType.FunctionPtr)] OrientationCallback cb, IntPtr /*void**/ userdata);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrRegisterButtonCallback(SafeClientInterfaceHandle iface, [MarshalAs(UnmanagedType.FunctionPtr)] ButtonCallback cb, IntPtr /*void**/ userdata);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrRegisterAnalogCallback(SafeClientInterfaceHandle iface, [MarshalAs(UnmanagedType.FunctionPtr)] AnalogCallback cb, IntPtr /*void**/ userdata);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrClientGetInterface(SafeClientContextHandle ctx, string path, ref SafeClientInterfaceHandle iface);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrGetPoseState(SafeClientInterfaceHandle iface, ref TimeValue timestamp, ref Pose3 state);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrGetPositionState(SafeClientInterfaceHandle iface, ref TimeValue timestamp, ref Vec3 state);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrGetOrientationState(SafeClientInterfaceHandle iface, ref TimeValue timestamp, ref Quaternion state);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrGetButtonState(SafeClientInterfaceHandle iface, ref TimeValue timestamp, ref Byte state);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrGetAnalogState(SafeClientInterfaceHandle iface, ref TimeValue timestamp, ref Double state);

            [DllImport(OSVR_CORE_DLL, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrClientFreeInterface(IntPtr iface);

            #endregion

            /// @brief Constructs an Interface object from an OSVR_ClientInterface
            /// object.
            public Interface(SafeClientInterfaceHandle iface)
            {
                this.m_interface = iface;
            }

            internal SafeClientInterfaceHandle Handle
            {
                get { return m_interface; }
            }

            /// @brief Register a callback for some report type.
            public void registerCallback(PoseCallback cb, IntPtr /*void*/ userdata)
            {
                osvrRegisterPoseCallback(m_interface, cb, userdata);
            }

            public void registerCallback(PositionCallback cb, IntPtr /*void*/ userdata)
            {
                osvrRegisterPositionCallback(m_interface, cb, userdata);
            }

            public void registerCallback(OrientationCallback cb, IntPtr /*void*/ userdata)
            {
                osvrRegisterOrientationCallback(m_interface, cb, userdata);
            }

            public void registerCallback(ButtonCallback cb, IntPtr /*void*/ userdata)
            {
                osvrRegisterButtonCallback(m_interface, cb, userdata);
            }

            public void registerCallback(AnalogCallback cb, IntPtr /*void*/ userdata)
            {
                osvrRegisterAnalogCallback(m_interface, cb, userdata);
            }

            private SafeClientInterfaceHandle m_interface;

            public void Dispose()
            {
                System.Diagnostics.Debug.WriteLine("In Interface.Dispose()");
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("In Interface.Dispose({0})", disposing));
                if (disposing)
                {
                    if (m_interface != null && !m_interface.IsInvalid)
                    {
                        m_interface.Dispose();
                        m_interface = null;
                    }
                }
            }
        }

    } // end namespace ClientKit

} // end namespace OSVR
