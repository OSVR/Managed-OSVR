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

namespace DisplayParameter
{
    class DisplayParameter
    {
        static string GetPoseDisplay(Pose3 pose)
        {
            return String.Format(
                "Translation: (x: {0}, y: {1}, z: {2}), rotation: (x: {3}, y: {4}, z: {5}, w: {6})",
                pose.translation.x,
                pose.translation.y,
                pose.translation.z,
                pose.rotation.x,
                pose.rotation.y,
                pose.rotation.z,
                pose.rotation.w);
        }

        static void Main(string[] args)
        {
            ClientContext.PreloadNativeLibraries();
			using (OSVR.ClientKit.ClientContext context = new OSVR.ClientKit.ClientContext("com.osvr.exampleclients.managed.DisplayParameter"))
            {
                string displayDescription = context.getStringParameter("/display");

                Console.WriteLine("Got value of /display:");
                Console.WriteLine(displayDescription);

                for (var i = 0; i < 1000; i++)
                {
                    context.update();

                    using(var displayConfig = context.GetDisplayConfig())
                    {
                        // GetDisplayConfig can sometimes fail, returning null
                        if (displayConfig != null)
                        {
                            var numViewers = displayConfig.GetNumViewers();
                            Console.WriteLine("There are {0} viewers for this display.", numViewers);
                            for (uint viewer = 0; viewer < numViewers; viewer++)
                            {
                                var numEyes = displayConfig.GetNumEyesForViewer(viewer);
                                Console.WriteLine("There are {0} eyes for viewer {1}.", numEyes, viewer);

                                var viewerPose = displayConfig.GetViewerPose(viewer);
                                Console.WriteLine("Viewer pose for viewer {0}: {1}",
                                    viewer, GetPoseDisplay(viewerPose));

                                for (byte eye = 0; eye < numEyes; eye++)
                                {
                                    var numSurfaces = displayConfig.GetNumSurfacesForViewerEye(viewer, eye);
                                    Console.WriteLine("There are {0} surfaces for eye {1} on viewer {2}.",
                                        numSurfaces, eye, viewer);

                                    var viewerEyePose = displayConfig.GetViewerEyePose(viewer, eye);
                                    Console.WriteLine("Viewer eye pose for eye {0} on viewer {1}: {2}.",
                                        eye, viewer, GetPoseDisplay(viewerEyePose));

                                    var viewerEyeMatrixd = displayConfig.GetViewerEyeViewMatrixd(viewer, eye, MatrixConventionsFlags.Default);
                                    Console.WriteLine("Viewer eye view-matrix (double) for eye {0} on viewer {1}: {2}",
                                        eye, viewer, viewerEyeMatrixd.ToString());

                                    var viewerEyeMatrixf = displayConfig.GetViewerEyeViewMatrixf(viewer, eye, MatrixConventionsFlags.Default);
                                    Console.WriteLine("Viewer eye view-matrix (float) for eye {0} on viewer {1}: {2}",
                                        eye, viewer, viewerEyeMatrixf.ToString());

                                    for (uint surface = 0; surface < numSurfaces; surface++)
                                    {
                                        Console.WriteLine("surface {0} for eye {1} for viewer {2}:",
                                            surface, eye, viewer);

                                        var viewport = displayConfig.GetRelativeViewportForViewerEyeSurface(
                                            viewer, eye, surface);
                                        Console.WriteLine("Relative viewport: {0}", viewport.ToString());

                                        var projectiond = displayConfig.GetProjectionMatrixForViewerEyeSurfaced(
                                            viewer, eye, surface, 1.0f, 1000.0f, MatrixConventionsFlags.Default);
                                        Console.WriteLine("Projection (double): {0}", projectiond.ToString());

                                        var projectionf = displayConfig.GetProjectionMatrixForViewerEyeSurfacef(
                                            viewer, eye, surface, 1.0f, 1000.0, MatrixConventionsFlags.Default);
                                        Console.WriteLine("Projection (float): {0}", projectionf.ToString());
                                    }
                                }
                            }
                            break;
                        }
                    }
                }

                Console.WriteLine("Library shut down; exiting.");
            }
        }
    }
}
