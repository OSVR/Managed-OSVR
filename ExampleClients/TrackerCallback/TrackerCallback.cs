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

﻿using System;
using OSVR.ClientKit;

namespace TrackerCallback
{
    public class TrackerCallbacks
    {
        // Pose callback
        public static void myTrackerCallback(Object sender, TimeValue timestamp, Int32 sensor, Pose3 report)
        {
            Console.WriteLine("Got POSE report: Position = ({0}, {1}, {2}), orientation ({3}, {4}, {5}, {6}), sensor ({7})",
                report.translation.x,
                report.translation.y,
                report.translation.z,
                report.rotation.w,
                report.rotation.x,
                report.rotation.y,
                report.rotation.z,
                sensor);
        }

        // Orientation callback
        public static void myOrientationCallback(Object sender, TimeValue timestamp, Int32 sensor, Quaternion report)
        {
            Console.WriteLine("Got ORIENTATION report: Orientation = ({0}, {1}, {2}, {3}), Sensor = ({4})",
                report.w,
                report.x,
                report.y,
                report.z,
                sensor);
        }

        // Position callback
        public static void myPositionCallback(Object sender, TimeValue timestamp, Int32 sensor, Vec3 report)
        {
            Console.WriteLine("Got POSITION report: Position = ({0}, {1}, {2}), Sensor = ({3})",
                report.x,
                report.y,
                report.z,
                sensor);
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            ClientContext.PreloadNativeLibraries();
			using (ClientContext context = new ClientContext("com.osvr.exampleclients.managed.TrackerCallback"))
            {
                // This is just one of the paths. You can also use:
                // /me/hands/right
                // /me/head
                using (Interface lefthand = context.getInterface("/me/hands/left"))
                {

                    TrackerCallbacks callbacks = new TrackerCallbacks();
                    // The coordinate system is right-handed, withX to the right, Y up, and Z near.
                    var poseInterface = new PoseInterface(lefthand);
                    poseInterface.StateChanged += TrackerCallbacks.myTrackerCallback;

                    // If you just want orientation
                    var orientationInterface = new OrientationInterface(lefthand);
                    orientationInterface.StateChanged += TrackerCallbacks.myOrientationCallback;

                    // or position
                    var positionInterface = new PositionInterface(lefthand);
                    positionInterface.StateChanged += TrackerCallbacks.myPositionCallback;

                    bool resetYawMode = false;
                    // Pretend that this is your application's main loop
                    for (int i = 0; i < 1000000; ++i)
                    {
                        // toggle between reset yaw mode and normal mode
                        // every 5000 iterations.
                        if (i % 5000 == 0)
                        {
                            resetYawMode = !resetYawMode;
                            if(resetYawMode)
                            {
                                context.SetRoomRotationUsingHead();
                            }
                            else
                            {
                                context.ClearRoomToWorldTransform();
                            }
                        }
                        context.update();
                    }

                    Console.WriteLine("Library shut down; exiting.");
                }
            }
        }
    }
}
