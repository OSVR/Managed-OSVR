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

namespace OSVR.ClientKit
{
    /// <summary>
    /// This is a utility base class for wrapping and adapting an existing IInterface of TSource into
    /// a new IInterface of TDest. This is useful when converting the raw report data
    /// from the generic OSVR types into a framework specific type./>
    /// </summary>
    /// <typeparam name="TReportSource">The report type of the wrapped interface.</typeparam>
    /// <typeparam name="TReportDest">The report type of the new wrapper interface.</typeparam>
    /// <typeparam name="TStateSource">The type of the state value of the wrapped interface.</typeparam>
    /// <typeparam name="TStateDest">The type of the state value of the new wrapper interface.</typeparam>
    public abstract class InterfaceAdapter<TReportSource, TReportDest, TStateSource, TStateDest> : IInterface<TReportDest, TStateDest>
    {
        protected IInterface<TReportSource, TStateSource> iface;
        protected bool started = false;

        public InterfaceAdapter(IInterface<TReportSource, TStateSource> iface)
        {
            this.iface = iface;
        }

        public void Start()
        {
            // not thread safe.
            if (!started)
            {
                this.iface.Start();
                this.iface.StateChanged += iface_StateChanged;
                started = true;
            }
        }

        /// <summary>
        /// Implemented in a derived class to convert an instance of TReportSource into an
        /// instance of TReportDest.
        /// </summary>
        /// <param name="sourceValue">The source value to convert.</param>
        /// <returns>The TReportDest equivalent of sourceValue.</returns>
        protected abstract TReportDest ConvertReportValue(TReportSource sourceValue);

        /// <summary>
        /// Implemented in a derived class to convert an instance of TStateSource into an
        /// instance of TStateDest.
        /// </summary>
        /// <param name="sourceValue">The source value to convert.</param>
        /// <returns>The TStateDest equivalent of sourceValue</returns>
        protected abstract TStateDest ConvertStateValue(TStateSource sourceValue);

        /// <summary>
        /// Callback for the original interface's StateChanged event. It shouldn't
        /// be common to override this in a derived class implementation.
        /// </summary>
        protected virtual void iface_StateChanged(object sender, TimeValue timestamp, TReportSource report)
        {
            if(StateChanged != null)
            {
                this.StateChanged(this, timestamp, ConvertReportValue(report));
            }
        }

        public InterfaceState<TStateDest> GetState()
        {
            var state = this.iface.GetState();
            return new InterfaceState<TStateDest>
            {
                Timestamp = state.Timestamp,
                Value = ConvertStateValue(state.Value)
            };
        }

        public event StateChangedHandler<TReportDest> StateChanged;

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                this.iface.Dispose();
                this.iface = null;
            }
        }
    }
}
