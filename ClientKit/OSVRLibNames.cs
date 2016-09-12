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


namespace OSVR
{
    internal static class OSVRLibNames
    {
        // Should be defined if used with Unity and UNITY_IOS or UNITY_XBOX360 are defined
#if MANAGED_OSVR_INTERNAL_PINVOKE
        public const string ClientKit = "__Internal";
        public const string Util = "__Internal";
        public const string RenderManager = "__Internal";
#else
        public const string ClientKit = "osvrClientKit";
        public const string Util = "osvrUtil";
        public const string RenderManager = "osvrRenderManager";
#endif
    }
}
