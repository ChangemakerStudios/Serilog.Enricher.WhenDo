// Copyright 2016-2018 CaptiveAire Systems
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

        public DoConfiguration(
            Func<Action<LogEvent, ILogEventPropertyFactory>, LoggerConfiguration> doEnrich,
            Func<Func<LogEvent, bool>, LoggerConfiguration> doFilter)
        {
            _doEnrich = doEnrich;
            _doFilter = doFilter;
        }

        public LoggerConfiguration RemovePropertyIfPresent(params string[] properties)
        {
            Action<LogEvent, ILogEventPropertyFactory> OuterAction(string[] p)
            {
                void InnerAction(LogEvent e, ILogEventPropertyFactory f)
                {
                    foreach (var prop in p)
                    {
                        e.RemovePropertyIfPresent(prop);
                    }
                }

                return InnerAction;
            }

            return _doEnrich(OuterAction(properties));
        }

        public LoggerConfiguration SendTo(ILogger sendToLogger)
        {
            if (sendToLogger == null) throw new ArgumentNullException(nameof(sendToLogger));

            Func<LogEvent, bool> OuterFunc(ILogger l)
            {
                bool InnerFunc(LogEvent e)
                {
                    // write event to other logger
                    l.Write(e);

                    // log event to this logger as well
                    return true;
                }

                return InnerFunc;
            }

            return _doFilter(OuterFunc(sendToLogger));
        }

        public LoggerConfiguration PipeTo(ILogger pipeToLogger)
        {
            if (pipeToLogger == null) throw new ArgumentNullException(nameof(pipeToLogger));

            Func<LogEvent, bool> OuterFunc(ILogger l)
            {
                bool InnerFunc(LogEvent e)
                {
                    // write event to other logger
                    l.Write(e);

                    // do not log event to this logger (disable event)
                    return false;
                }

                return InnerFunc;
            }

            return _doFilter(OuterFunc(pipeToLogger));
        }

        public LoggerConfiguration AddPropertyIfAbsent(string name, object value, bool destructureObject = false)
        {
            Action<LogEvent, ILogEventPropertyFactory> OuterAction(string n, object v, bool d)
            {
                void InnerAction(LogEvent e, ILogEventPropertyFactory f)
                {
                    e.AddPropertyIfAbsent(f.CreateProperty(n, v, d));
                }

                return InnerAction;
            }

            return _doEnrich(OuterAction(name, value, destructureObject));
        }

        public LoggerConfiguration AddOrUpdateProperty(string name, object value, bool destructureObject = false)
        {
            Action<LogEvent, ILogEventPropertyFactory> OuterAction(string n, object v, bool d)
            {
                void InnerAction(LogEvent e, ILogEventPropertyFactory f)
                {
                    e.AddOrUpdateProperty(f.CreateProperty(n, v, d));
                }

                return InnerAction;
            }

            return _doEnrich(OuterAction(name, value, destructureObject));
        }
    }
}