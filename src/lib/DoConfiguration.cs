// Copyright 2016 CaptiveAire Systems
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;

using Serilog.Core;
using Serilog.Events;

namespace Serilog.Enricher.WhenDo
{
    public class DoConfiguration
    {
        readonly Func<Action<LogEvent, ILogEventPropertyFactory>, LoggerConfiguration> _doEnrich;

        readonly Func<Func<LogEvent, bool>, LoggerConfiguration> _doFilter;

        public DoConfiguration(Func<Action<LogEvent, ILogEventPropertyFactory>, LoggerConfiguration> doEnrich, Func<Func<LogEvent, bool>, LoggerConfiguration> doFilter)
        {
            _doEnrich = doEnrich;
            _doFilter = doFilter;
        }

        public LoggerConfiguration RemovePropertyIfPresent(params string[] properties)
        {
            Func<string[], Action<LogEvent, ILogEventPropertyFactory>> action = (p) => (e, f) =>
                {
                    foreach (var prop in p)
                    {
                        e.RemovePropertyIfPresent(prop);
                    }
                };

            return _doEnrich(action(properties));
        }

        public LoggerConfiguration SendTo(ILogger sendToLogger)
        {
            if (sendToLogger == null) throw new ArgumentNullException(nameof(sendToLogger));

            Func<ILogger, Func<LogEvent, bool>> action = (l) => (e) =>
            {
                // write event to other logger
                l.Write(e);

                // log event to this logger as well
                return true;
            };

            return _doFilter(action(sendToLogger));
        }

        public LoggerConfiguration PipeTo(ILogger pipeToLogger)
        {
            if (pipeToLogger == null) throw new ArgumentNullException(nameof(pipeToLogger));

            Func<ILogger, Func<LogEvent, bool>> action = (l) => (e) =>
            {
                // write event to other logger
                l.Write(e);

                // do not log event to this logger (disable event)
                return false;
            };

            return _doFilter(action(pipeToLogger));
        }

        public LoggerConfiguration AddPropertyIfAbsent(string name, object value, bool destructureObject = false)
        {
            Func<string, object, bool, Action<LogEvent, ILogEventPropertyFactory>> action = (n, v, d) => (e, f) =>
                { e.AddPropertyIfAbsent(f.CreateProperty(n, v, d)); };

            return _doEnrich(action(name, value, destructureObject));
        }

        public LoggerConfiguration AddOrUpdateProperty(string name, object value, bool destructureObject = false)
        {
            Func<string, object, bool, Action<LogEvent, ILogEventPropertyFactory>> action = (n, v, d) => (e, f) =>
                { e.AddOrUpdateProperty(f.CreateProperty(n, v, d)); };

            return _doEnrich(action(name, value, destructureObject));
        }
    }
}