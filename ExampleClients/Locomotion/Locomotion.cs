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

namespace Locomotion
{
    class Locomotion
    {
        static void locomotionPosition_StateChanged(object sender, TimeValue timestamp, Int32 sensor, Vec2 report)
        {
            Console.WriteLine("Got report: sensor is {0}. Position is ( x: {1}, y: {2} )",
                sensor, report.x, report.y);
        }

        static void locomotionVelocity_StateChanged(object sender, TimeValue timestamp, Int32 sensor, Vec2 report)
        {
            Console.WriteLine("Got report: sensor is {0}. Velocity is ( x: {1}, y: {2} )",
                sensor, report.x, report.y);
        }


        static void Main(string[] args)
        {
            ClientContext.PreloadNativeLibraries();
            using (ServerAutoStarter serverAutoStarter = new ServerAutoStarter())
            using (OSVR.ClientKit.ClientContext context = new OSVR.ClientKit.ClientContext("com.osvr.exampleclients.managed.Locomotion"))
            {
#if NET20
                using (var naviPosition = NaviPositionInterface.GetInterface(context, "/me/feet/both"))
                using (var naviVelocity = NaviVelocityInterface.GetInterface(context, "/me/feet/both"))
#else
                using (var naviPosition = context.GetNaviPositionInterface("/me/feet/both"))
                using (var naviVelocity = context.GetNaviVelocityInterface("/me/feet/both"))
#endif
                {
                    naviPosition.StateChanged += locomotionPosition_StateChanged;
                    naviVelocity.StateChanged += locomotionVelocity_StateChanged;

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
