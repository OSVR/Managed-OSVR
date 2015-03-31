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

﻿using System;
using System.Runtime.InteropServices;
using System.Text;

#if !MANAGED_OSVR_INTERNAL_PINVOKE

using System.IO;

#endif

namespace OSVR
{
    namespace ClientKit
    {
        /// @brief Client context object: Create and keep one in your application.
        /// Handles lifetime management and provides access to ClientKit
        /// functionality.
        /// @ingroup ClientKitCPP
        public class ClientContext : IDisposable
        {
            #region ClientKit C functions

            // Should be defined if used with Unity and UNITY_IOS or UNITY_XBOX360 are defined
#if MANAGED_OSVR_INTERNAL_PINVOKE
            // On iOS and Xbox 360, plugins are statically linked into
            // the executable, so we have to use __Internal as the
            // library name.
            private const string OSVRCoreDll = "__Internal";
#else
            private const string OSVRCoreDll = "osvrClientKit";
#endif

            public static Byte OSVR_RETURN_SUCCESS = 0x0;
            public static Byte OSVR_RETURN_FAILURE = 0x1;

            [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
            public extern static IntPtr /*OSVR_ClientContext*/ osvrClientInit([MarshalAs(UnmanagedType.LPStr)] string applicationIdentifier, [MarshalAs(UnmanagedType.U4)] uint flags);

            [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrClientUpdate(IntPtr /*OSVR_ClientContext*/ ctx);

            [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrClientShutdown(IntPtr /*OSVR_ClientContext*/ ctx);

            [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrClientGetStringParameterLength(IntPtr /*OSVR_ClientContext*/ ctx, string path, out UIntPtr len);

            [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrClientGetStringParameter(IntPtr /*OSVR_ClientContext*/ ctx, string path, StringBuilder buf, UIntPtr len);

#if !MANAGED_OSVR_INTERNAL_PINVOKE

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool SetDllDirectory(string lpPathName);

#endif

            #endregion ClientKit C functions

            /// <summary>
            /// Static constructor - Try finding the right path for the p/invoked DLL before p/invoke tries to.
            /// </summary>
            static ClientContext()
            {
#if !MANAGED_OSVR_INTERNAL_PINVOKE
                var assembly = System.Uri.UnescapeDataString((new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath);
                var path = Path.GetDirectoryName(assembly);
                System.Diagnostics.Debug.WriteLine("OSVR.ClientKit assembly directory: " + path);

                if (IntPtr.Size == 8)
                {
                    var pathx64 = Path.Combine(path, "x64");
                    if (Directory.Exists(pathx64))
                    {
                        path = pathx64;
                    }
                }

                System.Diagnostics.Debug.WriteLine("OSVR.ClientKit DLL directory: " + path);
                // @todo cross-platform this: can we change the name of the pinvoked native module or something? Right now we just catch and ignore the p/invoke exception here.
                try
                {
                    SetDllDirectory(path);
                }
                catch (DllNotFoundException e)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Failed to set DLL directory: {0}", e));
                }
#endif
            }

            /// @brief Initialize the library.
            /// @param applicationIdentifier A string identifying your application.
            /// Reverse DNS format strongly suggested.
            /// @param flags initialization options (reserved) - pass 0 for now.
            public ClientContext(string applicationIdentifier, uint flags)
            {
                this.m_context = osvrClientInit(applicationIdentifier, flags);
            }

            public ClientContext(string applicationIdentifier)
            {
                this.m_context = osvrClientInit(applicationIdentifier, 0);
            }

            ~ClientContext()
            {
                Dispose(false);
            }

            public void shutdown()
            {
                if (this.m_context != IntPtr.Zero)
                {
                    System.Diagnostics.Debug.WriteLine("Client shutdown");
                    osvrClientShutdown(this.m_context);
                    this.m_context = IntPtr.Zero;
                }
            }

            /// @brief Destructor: Shutdown the library.
            public void Dispose()
            {
                System.Diagnostics.Debug.WriteLine("In Dispose()");
                Dispose(true);
                // No need to call the finalizer since we've now cleaned
                // up the unmanaged memory.
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("In Dispose({0})", disposing));
                shutdown();
            }

            /// @brief Updates the state of the context - call regularly in your
            /// mainloop.
            public void update()
            {
                Byte ret = osvrClientUpdate(this.m_context);
                if (OSVR_RETURN_SUCCESS != ret)
                {
                    throw new ApplicationException("Error updating context.");
                }
            }

            /// @brief Get the interface associated with the given path.
            /// @param path A resource path.
            /// @returns The interface object.
            public Interface getInterface(string path)
            {
                IntPtr /*OSVR_ClientInterface*/ iface = IntPtr.Zero;
                Byte ret = Interface.osvrClientGetInterface(this.m_context, path, ref iface);
                if (OSVR_RETURN_SUCCESS != ret)
                {
                    throw new ArgumentException("Couldn't create interface because the path was invalid.");
                }

                return new Interface(iface);
            }

            /// @brief Get a string parameter value from the given path.
            /// @param path A resource path.
            /// @returns parameter value, or empty string if parameter does not
            /// exist or is not a string.
            public string getStringParameter(string path)
            {
                UIntPtr length = UIntPtr.Zero;
                Byte ret = osvrClientGetStringParameterLength(m_context, path, out length);
                if (OSVR_RETURN_SUCCESS != ret)
                {
                    throw new ArgumentException("Invalid context or null reference to length variable.");
                }

                if (UIntPtr.Zero == length)
                {
                    return "";
                }

                StringBuilder buf = new StringBuilder((int)(length.ToUInt32()));
                ret = osvrClientGetStringParameter(m_context, path, buf, length);
                if (OSVR_RETURN_SUCCESS != ret)
                {
                    throw new ApplicationException("Invalid context, null reference to buffer, or buffer is too small.");
                }

                return buf.ToString();
            }

            private IntPtr /*OSVR_ClientContext*/ m_context;
        }
    } // end namespace ClientKit
} // end namespace OSVR
