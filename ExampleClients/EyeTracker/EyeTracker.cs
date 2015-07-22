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

namespace EyeTracker
{
    class EyeTracker
    {
        static string FormatEyeTracker2DReport(Vec2 report)
        {
            return String.Format("{0}; {1}\t", report.x, report.y);
        }

        static string FormatEyeTracker3DReport(EyeTracker3DState report)
        {
            return String.Format("basePoint: {0}; {1}; {2}\n", report.basePoint.x, report.basePoint.y, report.basePoint.z) +
                String.Format("direction: {0}; {1}; {2}\n", report.direction.x, report.direction.y, report.direction.z);
        }

        static void eyeTracker2D_StateChanged(object sender, TimeValue timestamp, Int32 sensor, Vec2 report)
        {
            Console.WriteLine("Got 2D eye tracker report, for sensor #{0}", sensor);
            Console.WriteLine(FormatEyeTracker2DReport(report));
        }

        static void eyeTracker3D_StateChanged(object sender, TimeValue timestamp, Int32 sensor, EyeTracker3DState report)
        {
            Console.WriteLine("Got 3D eye tracker report, for sensor #{0}", sensor);
            Console.WriteLine(FormatEyeTracker3DReport(report));
        }

        static void eyeTrackerBlink_StateChanged(object sender, TimeValue timestamp, Int32 sensor, Byte report)
        {
            Console.WriteLine("Got eye tracker blink Location Report, for sensor #{0}", sensor);
            Console.WriteLine("blink value: {0}", report);
        }

        const string EyeTrackerPath = "/me/eyes/left";

        static void Main(string[] args)
        {
            ClientContext.PreloadNativeLibraries();
            using (OSVR.ClientKit.ClientContext context = new OSVR.ClientKit.ClientContext("com.osvr.exampleclients.managed.EyeTracker"))
            {
                // there are three types of interfaces for eye tracking, 2D, 3D, and blinking
#if NET20
                using (var eyeTracker2D = EyeTracker2DInterface.GetInterface(context, EyeTrackerPath))
                using (var eyeTracker3D = EyeTracker3DInterface.GetInterface(context, EyeTrackerPath))
                using (var eyeTrackerBlink = EyeTrackerBlinkInterface.GetInterface(context, EyeTrackerPath))
#else
                using (var eyeTracker2D = context.GetEyeTracker2DInterface(EyeTrackerPath))
                using (var eyeTracker3D = context.GetEyeTracker3DInterface(EyeTrackerPath))
                using (var eyeTrackerBlink = context.GetEyeTrackerBlinkInterface(EyeTrackerPath))
#endif
                {
                    eyeTracker2D.StateChanged += eyeTracker2D_StateChanged;
                    eyeTracker3D.StateChanged += eyeTracker3D_StateChanged;
                    eyeTrackerBlink.StateChanged += eyeTrackerBlink_StateChanged;

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
