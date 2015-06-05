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

namespace InterfaceAdapter
{
    /// <summary>
    /// This represents some framework specific Vector3 type.
    /// We'll be creating an adapter to convert from either a PositionReport
    /// or a Vec3 value to this.
    /// </summary>
    struct Vector3f
    {
        public float X;
        public float Y;
        public float Z;
    }

    /// <summary>
    /// The purpose of this class is to convert position reports from Vec3 to Vector3f.
    /// Note: for Vector3fPositionInterface, the report and state types are the same (Vector3f)
    /// </summary>
    class Vector3fPositionInterface : InterfaceAdapter<Vec3, Vector3f>
    {
        // note: in .net > 2, the preferred way is to create an extension method instead.
        public static Vector3fPositionInterface GetVector3fPositionInterface(ClientContext context, string path)
        {
            var positionInterface = PositionInterface.GetInterface(context, path);
            return new Vector3fPositionInterface(positionInterface);
        }

        public Vector3fPositionInterface(IInterface<Vec3> iface) : base(iface) { }

        override protected Vector3f Convert(Vec3 sourceValue)
        {
            return new Vector3f
            {
                X = (float)sourceValue.x,
                Y = (float)sourceValue.y,
                Z = (float)sourceValue.z,
            };
        }
    }

    class MainClass
    {
        // Position callback. Notice we're getting Vector3f reports, and not the native OSVR PositionReport (with Vec3 values).
        public static void myPositionCallback(Object sender, TimeValue timestamp, Int32 sensor, Vector3f report)
        {
            Console.WriteLine("Got POSITION report: Position = ({0}, {1}, {2}), Sensor = ({3})",
                report.X,
                report.Y,
                report.Z,
                sensor);
        }

        public static void Main(string[] args)
        {
            using (ClientContext context = new ClientContext("org.opengoggles.exampleclients.managed.TrackerCallback"))
            {
                // We're creating an adapter from the OSVR native PositionInterface to a custom interface where the
                // report and state types are both Vector3f.
                using (var lefthand = Vector3fPositionInterface.GetVector3fPositionInterface(context, "/me/hands/left"))
                {
                    lefthand.Start();
                    lefthand.StateChanged += myPositionCallback;

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
