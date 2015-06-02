﻿/// Managed-OSVR binding
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

namespace OSVR.ClientKit
{
    #region Generic Delegate Types

    /// <summary>
    /// Delegate of the StateChanged event on IInterface."/>
    /// </summary>
    public delegate void StateChangedHandler<T>(Object sender, TimeValue timestamp, T report);

    /// <summary>
    /// Used internally by InterfaceBase to pass the native method used to
    /// get the current interface from derived classes to the base. Reduces
    /// boilerplate code.
    /// </summary>
    public delegate Byte GetStateFunc<T>(SafeClientInterfaceHandle iface, ref TimeValue timestamp, ref T state);

    #endregion

    /// <summary>
    /// Represents the current state of an interface, along with
    /// the timestamp of the last report.
    /// </summary>
    /// <typeparam name="T">The type of the state value.
    /// Example: Quaternion for an orientation interface.</typeparam>
    public struct InterfaceState<T>
    {
        /// <summary>
        /// The timestamp of the interface report that produced this state.
        /// </summary>
        public TimeValue Timestamp;

        /// <summary>
        /// The last or current value of the interface.
        /// </summary>
        public T Value;
    }

    /// <summary>
    /// .Net interface for an OSVR interface. Provides methods for getting the current
    /// state of the interface and an event for when the interface receives a new report.
    /// </summary>
    /// <typeparam name="TReport">The type of the interface report.
    /// Example: OrientationReport for an orientation interface.</typeparam>
    /// <typeparam name="TState">The type of the interface state.
    /// Example: Quaternion for an orientation interface.</typeparam>
    public interface IInterface<TReport, TState> : IDisposable
    {
        /// <summary>
        /// Initialize the interface and start producing events. 
        /// </summary>
        void Start();

        /// <summary>
        /// Get the current state of the interface. Depending on the implementation, this
        /// may call into the native OSVR ClientKit DLL to get the current state, so it
        /// is recommended that you cache this state rather than call this method multiple times.
        /// </summary>
        /// <returns>The current state of the interface.</returns>
        InterfaceState<TState> GetState();

        /// <summary>
        /// An event which is fired when the interface receives a new report. This usually
        /// occurs when the ClientContext is updated. Implementations MUST allow subscriptions
        /// to this event prior to Start.
        /// </summary>
        event StateChangedHandler<TReport> StateChanged;
    }

    /// <summary>
    /// Base class for interface wrappers.
    /// </summary>
    /// <typeparam name="TReport">The type of the interface report.</typeparam>
    /// <typeparam name="TState">The type of the state value.</typeparam>
    public abstract class InterfaceBase<TReport, TState> : IInterface<TReport, TState>
    {
        protected Interface iface;
        protected readonly GetStateFunc<TState> stateGetter;
        // It might be possible to have a RegisterCallbackFunc<TReport> type
        // here and implement Start generically, but I kept getting type errors
        // when referencing the native methods.

        public InterfaceBase(Interface iface, 
            GetStateFunc<TState> stateGetter)
        {
            // Take ownership of the Interface instance.
            this.iface = iface;
            this.stateGetter = stateGetter;
        }

        public string Path { get; private set; }
        public abstract void Start();

        public virtual InterfaceState<TState> GetState()
        {
            TimeValue timestamp = default(TimeValue);
            TState state = default(TState);
            stateGetter(this.iface.Handle, ref timestamp, ref state);
            return new InterfaceState<TState>
            {
                Timestamp = timestamp,
                Value = state,
            };
        }

        public event StateChangedHandler<TReport> StateChanged;

        /// <summary>
        /// Called by derived classes to invoke the StateChanged event.
        /// </summary>
        /// <param name="timestamp">The timestamp of the report.</param>
        /// <param name="report">The value of the interface report.</param>
        protected virtual void OnStateChanged(TimeValue timestamp, TReport report)
        {
            if (StateChanged != null)
            {
                StateChanged(this, timestamp, report);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.iface != null)
                {
                    this.iface.Dispose();
                    this.iface = null;
                }
            }
        }
    }
}
