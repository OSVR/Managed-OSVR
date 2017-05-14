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
using System.Text;
using Microsoft.Win32.SafeHandles;
#if !WINDOWS_UWP
using System.Runtime.ConstrainedExecution;
#endif

#if !MANAGED_OSVR_INTERNAL_PINVOKE

using System.IO;

#endif

namespace OSVR
{
    namespace ClientKit
    {
        public sealed class SafeClientContextHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public SafeClientContextHandle() : base(true) { }

#if !WINDOWS_UWP
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
#endif
            protected override bool ReleaseHandle()
            {
#if !WINDOWS_UWP
                System.Diagnostics.Debug.WriteLine("[OSVR] ClientContext shutdown");
                return ClientContext.osvrClientShutdown(handle) == OSVR.ClientKit.ClientContext.OSVR_RETURN_SUCCESS;
#else
                return true;
#endif
            }
        }

        /// <summary>
        /// Instantiate this class in order to attempt to auto-start the server. Call Dispose() to free up
        /// resources related to auto-starting the server (this may stop the server), if any for the current platform. The implementation
        /// for this is platform specific. It is recommended that you instantiate at most one of these
        /// per application, and its lifetime should be longer than any client context.
        /// </summary>
        public class ServerAutoStarter : IDisposable
        {
#if MANAGED_OSVR_INTERNAL_PINVOKE
            // On iOS and Xbox 360, plugins are statically linked into
            // the executable, so we have to use __Internal as the
            // library name.
            private const string OSVRCoreDll = "__Internal";
#else
            private const string OSVRCoreDll = "osvrClientKit";
#endif

            [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
            internal extern static void osvrClientAttemptServerAutoStart();

            [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
            internal extern static void osvrClientReleaseAutoStartedServer();

            public ServerAutoStarter()
            {
                osvrClientAttemptServerAutoStart();
            }

            ~ServerAutoStarter()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                System.Diagnostics.Debug.WriteLine("[OSVR] In ServerAutoStarter.Dispose()");
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// We don't strictly need this overload here, but it helps to keep the same
            /// IDisposable pattern when reviewing the code later to make sure we're implementing
            /// it correctly.
            /// </summary>
            /// <param name="disposing"></param>
            protected virtual void Dispose(bool disposing)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("[OSVR] In ServerAutoStarter.Dispose({0})", disposing));
                osvrClientReleaseAutoStartedServer(); // don't check disposing here, because this is an unmanaged resource
            }
        }

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
            public extern static SafeClientContextHandle osvrClientInit([MarshalAs(UnmanagedType.LPStr)] string applicationIdentifier, [MarshalAs(UnmanagedType.U4)] uint flags);

            [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrClientUpdate(SafeClientContextHandle ctx);

            [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrClientShutdown(IntPtr /*OSVR_ClientContext*/ ctx);

            [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrClientGetStringParameterLength(SafeClientContextHandle ctx, string path, out UIntPtr len);

            [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
            public extern static Byte osvrClientGetStringParameter(SafeClientContextHandle ctx, string path, StringBuilder buf, UIntPtr len);

            [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
            internal extern static Byte osvrClientCheckStatus(SafeClientContextHandle ctx);

            [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
            internal extern static Byte osvrClientSetRoomRotationUsingHead(SafeClientContextHandle ctx);

            [DllImport(OSVRCoreDll, CallingConvention = CallingConvention.Cdecl)]
            internal extern static Byte osvrClientClearRoomToWorldTransform(SafeClientContextHandle ctx);

            #endregion ClientKit C functions

            #region Support for locating native libraries

            /// <summary>
            /// Try finding the right path for the p/invoked DLLs before p/invoke tries to.
            /// </summary>
            static public void PreloadNativeLibraries()
            {
                PreloadNativeLibraries(false);
            }

            /// <summary>
            /// Try finding the right path for the p/invoked DLLs before p/invoke tries to.
            /// <param name="loadJointClientKitDlls">True, if you wish to load joint client kit DLLs as well.</param>
            /// </summary>
            static public void PreloadNativeLibraries(bool loadJointClientKitDlls)
            {
#if !MANAGED_OSVR_INTERNAL_PINVOKE && !WINDOWS_UWP

                // This line based on http://stackoverflow.com/a/864497/265522
                var assembly = System.Uri.UnescapeDataString((new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath);
                var assemblyPath = Path.GetDirectoryName(assembly);
                System.Diagnostics.Debug.WriteLine("[OSVR] ClientKit assembly directory: " + assemblyPath);
                LibraryPathAttempter attempt;
                if (IntPtr.Size == 8)
                {
                    attempt = new LibraryPathAttempter(assemblyPath, loadJointClientKitDlls).Attempt("x86_64").Attempt("x64").Attempt("64");
                }
                else
                {
                    attempt = new LibraryPathAttempter(assemblyPath, loadJointClientKitDlls).Attempt("x86").Attempt("32");
                }
                if (attempt.Success)
                {
                    if (attempt.Dir.Length > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("[OSVR] Loaded ClientKit native libraries from directory: " + attempt.Dir);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("[OSVR] Loaded ClientKit native libraries from default search path.");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[OSVR] Could not preload ClientKit native libraries");
                }
#endif
            }

#if !MANAGED_OSVR_INTERNAL_PINVOKE && !WINDOWS_UWP

            private class LibraryPathAttempter
            {
                [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
                private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

                /// <summary>
                /// Constructor - includes an "attempt" using the
                /// default search path and the assembly path
                /// </summary>
                /// <param name="dir">Directory containing this assembly.</param>
                /// <param name="loadJointClientKitDlls">True if you want to load the joint client kit dlls,
                /// false if you just want the client kit dlls.</param>
                public LibraryPathAttempter(string dir, bool loadJointClientKitDlls)
                {
                    AssemblyDir = dir;
                    AttemptDirectory("");
                    AttemptDirectory(dir);
                    mLoadJointClientKitDlls = loadJointClientKitDlls;
                }

                /// <summary>
                /// Chained method call to provide a subdirectory of the
                /// assembly directory to try to load the native
                /// libraries from
                /// </summary>
                /// <param name="pathSuffix">
                /// Name of subdirectory of the assembly directory.
                /// </param>
                /// <returns>
                /// This object for additional chained calls
                /// </returns>
                public LibraryPathAttempter Attempt(string pathSuffix)
                {
                    if (!KeepTrying)
                    {
                        return this;
                    }
                    AttemptDirectory(Path.Combine(AssemblyDir, pathSuffix));
                    return this;
                }

                /// <summary>
                /// The directory that we successfully loaded native
                /// libraries from. An empty string may mean we failed
                /// to load the libraries or we found them on the
                /// default search path, see Success to disambiguate.
                /// </summary>
                public string Dir
                {
                    get
                    {
                        return DllDir;
                    }
                }

                /// <summary>
                /// Indicates whether or not we were able to pre-load the native libraries.
                /// </summary>
                public bool Success
                {
                    get
                    {
                        return LoadedSuccess;
                    }
                }

                /// <summary>
                /// Attempts to load native libraries from the given
                /// directory. If successful, sets Result to dir.
                /// </summary>
                /// <param name="dir">
                /// Base directory to try loading from - empty string
                /// implies "default search path"
                /// </param>
                private void AttemptDirectory(string dir)
                {
                    if (!KeepTrying)
                    {
                        return;
                    }
                    if (AttemptDirectory(dir, "", ".dll") || AttemptDirectory(dir, "lib", ".so"))
                    {
                        DllDir = dir;
                        LoadedSuccess = true;
                        KeepTrying = false;
                    }
                }

                /// <summary>
                /// Attempts to load native libraries from the given
                /// directory with the provided formula (prefix and
                /// suffix), effectively dir + prefix + libname + suffix
                /// </summary>
                /// <param name="dir">
                /// Base directory to try loading from - empty string
                /// implies "default search path"
                /// </param>
                /// <param name="prefix">
                /// The string to prepend to the library's canonical
                /// name to get the filename
                /// </param>
                /// <param name="suffix">
                /// The string to append to the end of the library's
                /// canonical name to get the filename - essentially the extension.
                /// </param>
                /// <returns>
                /// true if all libraries were successfully loaded.
                /// </returns>
                private bool AttemptDirectory(string dir, string prefix, string suffix)
                {
                    if (!KeepTrying)
                    {
                        return false;
                    }
                    bool success = true;
                    // @todo cross-platform this: can we change the name
                    // of the pinvoked native module or something? Right
                    // now we just catch and ignore the p/invoke
                    // exception here.
                    try
                    {
                        foreach (var lib in NativeLibs)
                        {
                            var libName = ProduceLibPath(dir, prefix, suffix, lib);
                            System.Diagnostics.Debug.WriteLine(String.Format("[OSVR] Trying to LoadLibrary: {0}", libName));
                            var result = LoadLibrary(libName);
                            if (result == IntPtr.Zero)
                            {
                                // Later ones depend on earlier ones, so if one fails, this was not the right approach.
                                success = false;
                                break;
                            }
                        }
                    }
                    catch (DllNotFoundException e)
                    {
                        System.Diagnostics.Debug.WriteLine(String.Format("[OSVR] Caught exception trying to LoadLibrary: {0}", e));
                        success = false;
                        KeepTrying = false;
                    }
                    return success;
                }

                /// <summary>
                /// Combine a base directory, prefix, suffix, and
                /// library canonical name into a path to pass to
                /// LoadLibrary, specially handling the case of empty
                /// base directory for default search path (no path
                /// delimiter or extension/suffix)
                /// </summary>
                /// <param name="dir">
                /// Base directory to try loading from - empty string
                /// implies "default search path"
                /// </param>
                /// <param name="prefix">
                /// The string to prepend to the library's canonical
                /// name to get the filename
                /// </param>
                /// <param name="suffix">
                /// The string to append to the end of the library's
                /// canonical name to get the filename - essentially the extension.
                /// </param>
                /// <param name="lib">The library's canonical name</param>
                /// <returns>A library name or path to pass to LoadLibrary</returns>
                private static String ProduceLibPath(string dir, string prefix, string suffix, string lib)
                {
                    return dir.Length > 0 ? Path.Combine(dir, prefix + lib + suffix) : prefix + lib;
                }

                /// <summary>
                /// Native library names (without lib, .dll, or .so), in
                /// order of increasing dependency - that is, each
                /// library may only depend on libraries earlier in the list.
                /// This list contains only the client kit dlls and not the joint client kit dlls.
                /// </summary>
                private String[] NativeLibsClientOnly = { "osvrUtil", "osvrCommon", "osvrClient", "osvrClientKit" };

                /// <summary>
                /// Native library names (without lib, .dll, or .so), in
                /// order of increasing dependency - that is, each
                /// library may only depend on libraries earlier in the list.
                /// This list contains all the dlls required for running a joint client/server application.
                /// </summary>
                private String[] NativeLibsJointClientKit = { "osvrUtil", "osvrCommon", "osvrClient", "osvrClientKit", "osvrConnection.dll", "osvrPluginHost.dll", "osvrPluginKit.dll", "osvrUSBSerial.dll", "osvrVRPNServer.dll", "osvrJointClientKit.dll" };

                /// <summary>
                /// The correct native libs set - either the client only or joint client list, depending on
                /// the passed in configuration.
                /// </summary>
                private String[] NativeLibs
                {
                    get
                    {
                        return mLoadJointClientKitDlls ? NativeLibsJointClientKit : NativeLibsClientOnly;
                    }
                }

                private bool mLoadJointClientKitDlls;
                private string AssemblyDir;
                private string DllDir;
                private bool LoadedSuccess = false;
                private bool KeepTrying = true;
            }

#endif

            #endregion Support for locating native libraries

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
                // Since this is likely the first p/invoke call, may throw
                // System.BadImageFormatException if our DLL-path magic didn't
                // put the right bitness of DLL in the path
                this.m_context = osvrClientInit(applicationIdentifier, 0);
            }

            internal ClientContext(SafeClientContextHandle handle)
            {
                this.m_context = handle;
            }

            ~ClientContext()
            {
                Dispose(false);
            }

            /// @brief Destructor: Shutdown the library.
            public void Dispose()
            {
                System.Diagnostics.Debug.WriteLine("[OSVR] In ClientContext.Dispose()");
                Dispose(true);
                // No need to call the finalizer since we've now cleaned
                // up the unmanaged memory.
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("[OSVR] In ClientContext.Dispose({0})", disposing));
                if (disposing)
                {
                    foreach (var childDisposable in childDisposables)
                    {
                        childDisposable.Dispose();
                    }

                    if (this.m_context != null && !this.m_context.IsInvalid)
                    {
                        this.m_context.Dispose();
                        this.m_context = null;
                    }
                }
            }

            /// <summary>
            /// Checks to see if the client context is fully started up and connected
            /// properly to a server.
            ///
            /// If this reports that the client context is not OK, there may not be a server
            /// running, or you may just have to call osvrClientUpdate() a few times to
            /// permit startup to finish. The return value of this call will not change from
            /// failure to success without calling osvrClientUpdate().
            /// </summary>
            /// <returns>false if not yet fully connected/initialized, or if
            /// some other error (null context) occurs.</returns>
            public bool CheckStatus()
            {
                return osvrClientCheckStatus(this.m_context) == OSVR_RETURN_SUCCESS;
            }

            /// <summary>
            /// The native OSVR client context handle.
            /// </summary>
            public SafeClientContextHandle ContextHandle
            {
                get { return m_context; }
            }

            /// <summary>
            /// In the native client kit, some objects are owned by the client context,
            /// and are cleaned up when the context is cleaned up. We need to track these,
            /// so that we don't accidently attempt to free an object that is already
            /// freed when the context is destroyed. The child's Dispose implementation must
            /// be idempotent, per convention.
            /// </summary>
            internal void AddChildDisposable(IDisposable childDisposable)
            {
                if (null != childDisposable)
                {
                    this.childDisposables.Add(childDisposable);
                }
            }

            /// <summary>
            /// Get a parsed display configuration. This lets you query eyes, surfaces, and
            /// viewers.
            /// </summary>
            public DisplayConfig GetDisplayConfig()
            {
                SafeDisplayConfigHandle handle;
                if (DisplayConfigNative.osvrClientGetDisplay(this.m_context, out handle) != OSVR_RETURN_SUCCESS)
                {
                    return null;
                }
                this.AddChildDisposable(handle);
                return new DisplayConfig(handle);
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
                SafeClientInterfaceHandle iface = new SafeClientInterfaceHandle();
                Byte ret = Interface.osvrClientGetInterface(this.m_context, path, ref iface);
                if (OSVR_RETURN_SUCCESS != ret)
                {
                    throw new ArgumentException("Couldn't create interface because the path was invalid.");
                }
                this.AddChildDisposable(iface);
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


            /// <summary>
            /// Updates the internal "room to world" transformation (applied to all
            /// tracker data for this client context instance) based on the user's head
            /// orientation, so that the direction the user is facing becomes -Z to your
            /// application. Only rotates about the Y axis (yaw).
            /// 
            /// Note that this method internally calls osvrClientUpdate() to get a head pose
            /// so your callbacks may be called during its execution!
            /// </summary>
            public void SetRoomRotationUsingHead()
            {
                Byte success = osvrClientSetRoomRotationUsingHead(m_context);
                if (OSVR_RETURN_SUCCESS != success)
                {
                    throw new ApplicationException("OSVR::SetRoomRotationUsingHead() - native osvrClientSetRoomRotationUsingHead call failed.");
                }
            }


            /// <summary>
            /// Clears/resets the internal "room to world" transformation back to an
            /// identity transformation - that is, clears the effect of any other
            /// manipulation of the room to world transform.
            /// </summary>
            public void ClearRoomToWorldTransform()
            {
                Byte success = osvrClientClearRoomToWorldTransform(m_context);
                if (OSVR_RETURN_SUCCESS != success)
                {
                    throw new ApplicationException("OSVR::ClearRoomToWorldTransform() - native osvrClientClearRoomToWorldTransform call failed.");
                }
            }

            private SafeClientContextHandle m_context;
            private readonly System.Collections.Generic.List<IDisposable> childDisposables
                = new System.Collections.Generic.List<IDisposable>();

        }
    } // end namespace ClientKit
} // end namespace OSVR
