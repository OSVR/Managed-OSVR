using Microsoft.Win32.SafeHandles;
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
using System.Collections.Generic;
#if !WINDOWS_UWP
using System.Runtime.ConstrainedExecution;
#endif
using System.Runtime.InteropServices;
using System.Text;

namespace OSVR.ClientKit
{
    //public sealed class SafeJointClientOptionsHandle : SafeHandleZeroOrMinusOneIsInvalid
    //{
    //    public SafeJointClientOptionsHandle() : base(true) { }

    //    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
    //    protected override bool ReleaseHandle()
    //    {
    //        System.Diagnostics.Debug.WriteLine("[OSVR] ClientContext shutdown");
    //        //return ClientContext.osvrClientShutdown(handle) == OSVR.ClientKit.ClientContext.OSVR_RETURN_SUCCESS;
    //        return false;
    //    }
    //}

    internal static class JointClientKitNative
    {
#if MANAGED_OSVR_INTERNAL_PINVOKE
            // On iOS and Xbox 360, plugins are statically linked into
            // the executable, so we have to use __Internal as the
            // library name.
            private const string OSVRCoreDll = "__Internal";
#else
        private const string JointClientKitDll = "osvrJointClientKit";
#endif

#if WINDOWS_UWP
        public static IntPtr osvrJointClientCreateOptions() { return IntPtr.Zero; }
        public static Byte osvrJointClientOptionsAutoloadPlugins(IntPtr options) { return 0; }
        public static Byte osvrJointClientOptionsLoadPlugin(IntPtr options, string pluginName) { return 0; }
        public static Byte osvrJointClientOptionsInstantiateDriver(IntPtr options, string pluginName, string driverName, string parameters) { return 0; }
        public static Byte osvrJointClientOptionsAddAlias(IntPtr options, string path, string source) { return 0; }
        public static Byte osvrJointClientOptionsAddAliases(IntPtr options, string aliases) { return 0; }
        public static Byte osvrJointClientOptionsAddString(IntPtr options, string path, string s) { return 0; }
        public static Byte osvrJointClientOptionsTriggerHardwareDetect(IntPtr options) { return 0; }
        public static SafeClientContextHandle osvrJointClientInit(string applicationIdentifier, IntPtr options) { return null; }
#else
        [DllImport(JointClientKitDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr /*SafeJointClientOptionsHandle*/ osvrJointClientCreateOptions();

        [DllImport(JointClientKitDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrJointClientOptionsAutoloadPlugins(IntPtr /*SafeJointClientOptionsHandle*/ options);

        [DllImport(JointClientKitDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrJointClientOptionsLoadPlugin(IntPtr /*SafeJointClientOptionsHandle*/ options,
            string pluginName);

        [DllImport(JointClientKitDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrJointClientOptionsInstantiateDriver(IntPtr /*SafeJointClientOptionsHandle*/ options,
            string pluginName, string driverName, string parameters);

        [DllImport(JointClientKitDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrJointClientOptionsAddAlias(IntPtr /*SafeJointClientOptionsHandle*/ options,
            string path, string source);

        [DllImport(JointClientKitDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrJointClientOptionsAddAliases(IntPtr /*SafeJointClientOptionsHandle*/ options,
            string aliases);

        [DllImport(JointClientKitDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrJointClientOptionsAddString(IntPtr /*SafeJointClientOptionsHandle*/ options,
            string path, string s);

        [DllImport(JointClientKitDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static Byte osvrJointClientOptionsTriggerHardwareDetect(IntPtr /*SafeJointClientOptionsHandle*/ options);

        [DllImport(JointClientKitDll, CallingConvention = CallingConvention.Cdecl)]
        public extern static SafeClientContextHandle osvrJointClientInit(
            string applicationIdentifier, IntPtr /*SafeJointClientOptionsHandle*/ options);
#endif
    }

    /// <summary>
    /// A limited-purpose library for creating client contexts that operate
    /// the server in the same thread. **See cautions in OSVR-Core documentation**
    /// 
    /// Methods of this class queue up actions to perform on the server when the joint
    /// client/server is later created in InitContext(). These methods do
    /// not perform the actions directly or immediately, and only perform some basic
    /// error checking on their inputs: failure in the actions specified may instead
    /// be reported later in a failure to create the context.
    /// </summary>
    public class JointClientOptions
    {
        IntPtr /*SafeJointClientOptionsHandle*/ mHandle;

        /// <summary>
        /// Creates an empty OSVR_JointClientOpts.
        /// </summary>
        public JointClientOptions()
        {
            mHandle = JointClientKitNative.osvrJointClientCreateOptions();
            if(mHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException(
                    "[OSVR] JointClientOptions.JointClientOptions(): native method osvrJointClientCreateOptions() failed");
            }
        }

        private void ThrowIfError(Byte returnCode, string message)
        {
            if(returnCode == ClientContext.OSVR_RETURN_FAILURE)
            {
                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Queues up the autoloading of plugins. May only be called once per
        /// options object.
        /// </summary>
        public void AutoloadPlugins()
        {
            ThrowIfError(JointClientKitNative.osvrJointClientOptionsAutoloadPlugins(mHandle),
                "[OSVR] JointClientOptions.AutoloadPlugins(): native method osvrJointClientOptionsAutoloadPlugins failed");
        }

        /// <summary>
        /// Queues up the manual load of a plugin by name.
        /// </summary>
        public void LoadPlugin(string pluginName)
        {
            ThrowIfError(JointClientKitNative.osvrJointClientOptionsLoadPlugin(mHandle, pluginName),
                "[OSVR] JointClientOptions.LoadPlugin(): native method osvrJointClientOptionsLoadPlugin failed");
        }

        /// <summary>
        /// Queues up the manual instantiation of a plugin/driver by name with
        /// optional parameters (JSON).
        /// </summary>
        public void InstantiateDriver(string pluginName, string driverName, string parameters)
        {
            ThrowIfError(JointClientKitNative.osvrJointClientOptionsInstantiateDriver(mHandle, pluginName, driverName, parameters),
                "[OSVR] JointClientOptions.InstantiateDriver(): native method osvrJointClientOptionsInstantiateDriver failed");
        }

        /// <summary>
        /// Queues up the manual addition of an alias to the path tree.
        /// </summary>
        public void AddAlias(string path, string source)
        {
            ThrowIfError(JointClientKitNative.osvrJointClientOptionsAddAlias(mHandle, path, source),
                "[OSVR] JointClientOptions.AddAlias(): native method osvrJointClientOptionsAddAlias failed");
        }

        /// <summary>
        /// Queues up the manual addition of aliases specified in JSON to the
        /// path tree.
        /// </summary>
        public void AddAliases(string aliases)
        {
            ThrowIfError(JointClientKitNative.osvrJointClientOptionsAddAliases(mHandle, aliases),
                "[OSVR] JointClientOptions.AddAliases(): native method osvrJointClientOptionsAddAliases failed");
        }

        /// <summary>
        /// Queues up the manual addition of a string element to the path tree.
        /// </summary>
        public void AddString(string path, string value)
        {
            ThrowIfError(JointClientKitNative.osvrJointClientOptionsAddString(mHandle, path, value),
                "[OSVR] JointClientOptions.AddString(): native method osvrJointClientOptionsAddString failed");
        }

        /// <summary>
        /// Queues up a trigger for hardware detection.
        /// </summary>
        public void TriggerHardwareDetect()
        {
            ThrowIfError(JointClientKitNative.osvrJointClientOptionsTriggerHardwareDetect(mHandle),
                "[OSVR] JointClientOptions.TriggerHardwareDetect(): native method osvrJointClientOptionsTriggerHardwareDetect failed");
        }
        
        /// <summary>
        /// Initialize the library, starting up a "joint" context that also
        /// contains a server. This version of InitContext performs the default
        /// server operation: loading of all autoload-enabled plugins, and a hardware
        /// detection.
        /// </summary>
        /// <param name="applicationIdentifier">A string identifying your
        /// application. Reverse DNS format strongly suggested.</param>
        /// <returns>null, if initialization failed, else a ClientContext.</returns>
        public static ClientContext InitContext(string applicationIdentifier)
        {
            var clientContextHandle = JointClientKitNative.osvrJointClientInit(applicationIdentifier, IntPtr.Zero);
            if (clientContextHandle.IsInvalid)
            {
                return null;
            }

            return new ClientContext(clientContextHandle);
        }

        /// <summary>
        /// Initialize the library, starting up a "joint" context that also
        /// contains a server. This version of InitContext performs the default
        /// server operation: loading of all autoload-enabled plugins, and a hardware
        /// detection.
        /// </summary>
        /// <param name="options">The configuration options object for starting the joint server
        /// operations. Pass null for default operation: loading of all
        /// autoload-enabled plugins, and a hardware detection. If a non-null pointer is
        /// passed, the enqueued operations will be performed in-order (the default
        /// operations will not be performed). Any exceptions thrown will cause the
        /// initialization to fail, returning a null context.</param>
        /// <param name="applicationIdentifier">A string identifying your
        /// application. Reverse DNS format strongly suggested.</param>
        /// <returns>null, if initialization failed, else a ClientContext.</returns>
        public static ClientContext InitContext(ref JointClientOptions options, string applicationIdentifier)
        {
            if(options == null)
            {
                return InitContext(applicationIdentifier);
            }

            var clientContextHandle = JointClientKitNative.osvrJointClientInit(applicationIdentifier, options.mHandle);
            options = null; // options can't be re-used after Init is called.
            if(clientContextHandle.IsInvalid)
            {
                return null;
            }

            return new ClientContext(clientContextHandle);
        }
    }
}
