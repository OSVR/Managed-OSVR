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
            jointClientOptions.LoadPlugin("com_osvr_example_EyeTracker");
            jointClientOptions.TriggerHardwareDetect();
            using(var context = JointClientOptions.InitContext(ref jointClientOptions, "com.osvr.Examples.JointClientKit"))
            {
                if (context == null)
                {
                    Console.WriteLine("Failed to create a joint client context.");
                    return;
                }

                context.update();
                var leftEye = EyeTracker2DInterface.GetInterface(context, "/me/eyes/left");
                leftEye.StateChanged += leftEye_StateChanged;
                for (var i = 0; i < 10000; i++)
                {
                    context.update();
                }
            }
        }

        static void leftEye_StateChanged(object sender, TimeValue timestamp, int sensor, Vec2 report)
        {
            Console.WriteLine("Got a left eye state changed event! Something must be working...");
        }
    }
}
