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

namespace ButtonState
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            ClientContext.PreloadNativeLibraries();
			using (ClientContext context = new ClientContext("com.osvr.exampleclients.managed.TrackerCallback"))
            {
#if NET20
                using (var button1 = ButtonInterface.GetInterface(context, "/controller/left/1"))
                using (var button2 = ButtonInterface.GetInterface(context, "/controller/left/2"))
#else
                using (var button1 = context.GetButtonInterface("/controller/left/1"))
                using (var button2 = context.GetButtonInterface("/controller/left/2"))
#endif
                {
                    // Pretend that this is your application's main loop
                    for (int i = 0; i < 1000000; ++i)
                    {
                        context.update();

                        // getting the current state calls into the native DLL, so
                        // try to get the state only once per frame.
                        var button1State = button1.GetState();
                        var button2State = button2.GetState();
                        if (button1State.Value == ButtonInterface.Pressed)
                        {
                            Console.WriteLine("Pressing button 1!");
                        }

                        // re-using button1State
                        if(button1State.Value == ButtonInterface.Pressed
                            && button2State.Value == ButtonInterface.Pressed)
                        {
                            Console.WriteLine("Pressing both button 1 and 2!");
                        }
                    }

                    Console.WriteLine("Library shut down; exiting.");
                }
            }
        }
    }
}
