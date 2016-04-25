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

namespace Imaging
{
    class Imaging
    {
        static void Main(string[] args)
        {
            ClientContext.PreloadNativeLibraries();
            using (ServerAutoStarter serverAutoStarter = new ServerAutoStarter())
            using (OSVR.ClientKit.ClientContext context = new OSVR.ClientKit.ClientContext("com.osvr.exampleclients.managed.Imaging"))
            {
                // there are three types of interfaces for eye tracking, 2D, 3D, and blinking
#if NET20
                using (var imaging = ImagingInterface.GetInterface(context, "/camera"))
#else
                using (var imaging = context.GetImagingInterface("/camera"))
#endif
                {
                    imaging.StateChanged += imaging_StateChanged;

                    // Pretend that this is your application's main loop
                    for (int i = 0; i < 10000000; ++i)
                    {
                        context.update();
                    }

                    Console.WriteLine("Library shut down; exiting.");
                }
            }
        }

        static uint reportNum = 0;
        static void imaging_StateChanged(object sender, TimeValue timestamp, int sensor, ImagingState report)
        {
            var m = report.metadata;
            Console.WriteLine("Got Imaging report {0}:", reportNum++);
            Console.WriteLine("\twidth: {0}", m.width);
            Console.WriteLine("\theight: {0}", m.height);
            Console.WriteLine("\tdepth: {0}", m.depth);
            Console.WriteLine("\tnumber of color channels: {0}", m.channels);
            Console.WriteLine("\ttype: {0}", m.type.ToString());
        }
    }
}
