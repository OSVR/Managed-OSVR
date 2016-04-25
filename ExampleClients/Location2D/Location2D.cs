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

namespace Location2D
{
    class Location2D
    {
        static string FormatLocation2DReport(Vec2 report)
        {
            return String.Format("{0}; {1}\t", report.x, report.y);
        }

        static void location2D_StateChanged(object sender, TimeValue timestamp, Int32 sensor, Vec2 report)
        {
            Console.WriteLine("Got 2D Location Report, for sensor #{0}", sensor);
            Console.WriteLine(FormatLocation2DReport(report));
        }

        // You will need to install the com_osvr_example_EyeTracker from osvr-core
        // (https://github.com/OSVR/OSVR-Core/blob/master/examples/plugin/com_osvr_example_EyeTracker.cpp)
        // into your osvr-server's osvr-plugins-0 folder, add add the following to your osvr_server_config.json:
        // "plugins": [
        //    "com_osvr_example_EyeTracker"
        // ],
        const string Path = "/com_osvr_example_EyeTracker/EyeTracker/location2D";

        static void Main(string[] args)
        {
            ClientContext.PreloadNativeLibraries();
            using (ServerAutoStarter serverAutoStarter = new ServerAutoStarter())
            using (OSVR.ClientKit.ClientContext context = new OSVR.ClientKit.ClientContext("com.osvr.exampleclients.managed.Location2D"))
            {
#if NET20
                using (var location2D = Location2DInterface.GetInterface(context, Path))
#else
                using (var location2D = context.GetLocation2DInterface(Path))
#endif
                {
                    location2D.StateChanged += location2D_StateChanged;
                    // Pretend that this is your application's main loop
                    for (int i = 0; i < 1000000; ++i)
                    {
                        context.update();
                        var location2DState = location2D.GetState();
                        Console.WriteLine("Location2DState: {0}", FormatLocation2DReport(location2DState.Value));
                    }

                    Console.WriteLine("Library shut down; exiting.");
                }
            }
        }
    }
}
