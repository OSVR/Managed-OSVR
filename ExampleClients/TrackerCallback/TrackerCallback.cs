/// Managed-OSVR binding
///
/// <copyright>
/// Copyright 2014 Sensics, Inc.
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
        public static void myTrackerCallback(Object sender, TimeValue timestamp, PoseReport report)
        {
            Console.WriteLine("Got POSE report: Position = ({0}, {1}, {2}), orientation ({3}, {4}, {5}, {6})",
                report.pose.translation.x,
                report.pose.translation.y,
                report.pose.translation.z,
                report.pose.rotation.w,
                report.pose.rotation.x,
                report.pose.rotation.y,
                report.pose.rotation.z);
        }

        // Orientation callback
        public static void myOrientationCallback(Object sender, TimeValue timestamp, OrientationReport report)
        {
            Console.WriteLine("Got ORIENTATION report: Orientation = ({0}, {1}, {2}, {3})",
                report.rotation.w,
                report.rotation.x,
                report.rotation.y,
                report.rotation.z);
        }

        // Position callback
        public static void myPositionCallback(Object sender, TimeValue timestamp, PositionReport report)
        {
            Console.WriteLine("Got POSITION report: Position = ({0}, {1}, {2})",
                report.xyz.x,
                report.xyz.y,
                report.xyz.z);
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            using (ClientContext context = new ClientContext("org.opengoggles.exampleclients.managed.TrackerCallback"))
            {
                // This is just one of the paths. You can also use:
                // /me/hands/right
                // /me/head
                using (Interface lefthand = context.getInterface("/me/hands/left"))
                {

                    TrackerCallbacks callbacks = new TrackerCallbacks();
                    // The coordinate system is right-handed, withX to the right, Y up, and Z near.
                    var poseInterface = new PoseInterface(lefthand);
                    poseInterface.Start();
                    poseInterface.StateChanged += TrackerCallbacks.myTrackerCallback;

                    // If you just want orientation
                    var orientationInterface = new OrientationInterface(lefthand);
                    orientationInterface.Start();
                    orientationInterface.StateChanged += TrackerCallbacks.myOrientationCallback;

                    // or position
                    var positionInterface = new PositionInterface(lefthand);
                    positionInterface.Start();
                    positionInterface.StateChanged += TrackerCallbacks.myPositionCallback;

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
