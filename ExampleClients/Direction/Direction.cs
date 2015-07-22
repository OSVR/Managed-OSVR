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

namespace Direction
{
    class Direction
    {
        static string FormatDirectionReport(Vec3 report)
        {
            return String.Format("direction: {0}; {1}; {2}\n", report.x, report.y, report.z);
        }

        static void direction_StateChanged(object sender, TimeValue timestamp, Int32 sensor, Vec3 report)
        {
            Console.WriteLine("Got direction report, for sensor #{0}: {1}",
                sensor, FormatDirectionReport(report));
        }

        const string Path = "/com_osvr_Multi/Location2D/direction";

        static void Main(string[] args)
        {
            ClientContext.PreloadNativeLibraries();
            using (OSVR.ClientKit.ClientContext context = new OSVR.ClientKit.ClientContext("com.osvr.exampleclients.managed.Direction"))
            {
#if NET20
                using (var direction = DirectionInterface.GetInterface(context, Path))
#else
                using (var direction = context.GetDirectionInterface(Path))
#endif
                {
                    direction.StateChanged += direction_StateChanged;

                    // Pretend that this is your application's main loop
                    for (int i = 0; i < 1000000; ++i)
                    {
                        context.update();
                    }

                    Console.WriteLine("Library shut down; exiting.");
                }
            }
        }
    }
}
