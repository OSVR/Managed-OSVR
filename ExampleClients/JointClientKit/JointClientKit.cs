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
using OSVR.ClientKit;

namespace JointClientKit
{
    class JointClientKit
    {
        static void Main(string[] args)
        {
            ClientContext.PreloadNativeLibraries(true);
            var jointClientOptions = new JointClientOptions();

            //jointClientOptions.AddString("/display", "displays/OSVR_HDK_1_1.json");

            // not presently working in the native code.
            //jointClientOptions.LoadPlugin("com_osvr_example_EyeTracker");

            // This is the default behavior when you doin't specify a JointClientOptions
            jointClientOptions.AutoloadPlugins();
            jointClientOptions.TriggerHardwareDetect();
            using(var context = JointClientOptions.InitContext(ref jointClientOptions, "com.osvr.Examples.JointClientKit"))
            {
                if (context == null)
                {
                    Console.WriteLine("Failed to create a joint client context.");
                    return;
                }
                context.update();
            }
        }
    }
}
