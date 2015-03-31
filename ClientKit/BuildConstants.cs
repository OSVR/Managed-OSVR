/// Managed-OSVR binding
///
/// <copyright>
/// Copyright 2015 Sensics, Inc.
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
///

using System;
using System.Collections.Generic;

namespace OSVR.ClientKit
{
    internal class BuildConstants
    {
#if NET20
        public const string FrameworkDescription = ".NET 2.0";
#elif NET45
        public const string FrameworkDescription = ".NET 4.5";
#else
#error "Building for some unrecognized .NET version!"
#endif

#if DEBUG
        public const string Configuration = "Debug";
#else
        public const string Configuration = "Release";
#endif

#if MANAGED_OSVR_INTERNAL_PINVOKE
        public const bool InternalPInvoke = true;
        public const string InternalPInvokeString = "/Static Linking";
#else
        public const bool InternalPInvoke = false;
        public const string InternalPInvokeString = "";
#endif

        public static string FullConfiguration
        {
            get
            {
                var components = new List<string>() { FrameworkDescription, Configuration };
#if MANAGED_OSVR_INTERNAL_PINVOKE
                components.Add("Static Linking");
#endif
                return String.Join("/", components.ToArray());
            }
        }
    }
}
