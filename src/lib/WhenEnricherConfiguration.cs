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

namespace Serilog.Enricher.WhenDo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Serilog.Core;
    using Serilog.Events;

    public class WhenEnricherConfiguration
    {
        readonly LoggerConfiguration _configuration;

        readonly Func<LogEvent, bool>[] _whenFuncs;

        public WhenEnricherConfiguration(
            LoggerConfiguration configuration,
            IEnumerable<Func<LogEvent, bool>> conditions = null)
        {
            _configuration = configuration;
            _whenFuncs = (conditions ?? new Func<LogEvent, bool>[0]).ToArray();
        }

        WhenEnricherConfiguration GetComposedWhenEnricherConfiguration(params Func<LogEvent, bool>[] when)
            => new WhenEnricherConfiguration(_configuration, _whenFuncs.Concat(when));

        public DoConfiguration Do() => new DoConfiguration(DoEnrich, DoFilter);

        LoggerConfiguration DoFilter(Func<LogEvent, bool> func)
        {
            return this._configuration.Filter.With(new WhenDoPipeFilter(this._whenFuncs, func));
        }

        LoggerConfiguration DoEnrich(Action<LogEvent, ILogEventPropertyFactory> func)
        {
            return this._configuration.Enrich.With(new WhenDoEnricher(this._whenFuncs, func));
        }

        public WhenEnricherConfiguration All() => GetComposedWhenEnricherConfiguration(s => true);

        public WhenEnricherConfiguration IsLevelEqualTo(LogEventLevel level)
            => GetComposedWhenEnricherConfiguration(WhenLevelEquals(level));

        Func<LogEvent, bool> WhenLevelEquals(LogEventLevel l) => e => e.Level == l;

        public WhenEnricherConfiguration IsLevelOrLess(LogEventLevel level)
        {
            Func<LogEvent, bool> When(LogEventLevel l) => e => e.Level <= l;

            return GetComposedWhenEnricherConfiguration(When(level));
        }

        public WhenEnricherConfiguration IsLevelOrHigher(LogEventLevel level)
        {
            Func<LogEvent, bool> When(LogEventLevel l) => e => e.Level >= l;

            return GetComposedWhenEnricherConfiguration(When(level));
        }

        public WhenEnricherConfiguration HasProperty(params string[] properties)
        {
            return GetComposedWhenEnricherConfiguration(WhenHasProperty(properties));
        }

        Func<LogEvent, bool> WhenHasProperty(string[] p) => e => e.Properties.Keys.Intersect(p).Any();

        public WhenEnricherConfiguration MissingProperty(params string[] properties)
        {
            Func<LogEvent, bool> OuterWhen(string[] p)
            {
                bool InnerWhen(LogEvent e) => !e.Properties.Keys.Intersect(p).Any();

                return InnerWhen;
            }

            return GetComposedWhenEnricherConfiguration(OuterWhen(properties));
        }

        public WhenEnricherConfiguration IsException(params Type[] exceptionTypes)
        {
            Func<LogEvent, bool> OuterWhen(Type[] es)
            {
                bool InnerWhen(LogEvent e) => e.Exception != null && es.Contains(e.Exception.GetType());

                return InnerWhen;
            }

            return GetComposedWhenEnricherConfiguration(OuterWhen(exceptionTypes));
        }

        public WhenEnricherConfiguration IsException<TException>()
        {
            return IsException(typeof(TException));
        }

        public WhenEnricherConfiguration NoException()
        {
            bool When(LogEvent e) => e.Exception == null;

            return GetComposedWhenEnricherConfiguration(When);
        }

        public WhenEnricherConfiguration AnyException()
        {
            bool When(LogEvent e) => e.Exception != null;

            return GetComposedWhenEnricherConfiguration((Func<LogEvent, bool>)When);
        }

        public WhenEnricherConfiguration IsNotException(params Type[] exceptionTypes)
        {
            Func<LogEvent, bool> OuterWhen(Type[] es)
            {
                bool InnerWhen(LogEvent e) => e.Exception != null && !es.Contains(e.Exception.GetType());

                return InnerWhen;
            }

            return GetComposedWhenEnricherConfiguration(OuterWhen(exceptionTypes));
        }

        public WhenEnricherConfiguration IsNotExceptionOf<TException>() => IsNotException(typeof(TException));

        public WhenEnricherConfiguration FromSourceContext(params string[] sources)
        {
            Func<LogEvent, bool> OuterWhen(string[] p)
            {
                bool InnerWhen(LogEvent e)
                {
                    var sourceContext = e.Properties.Where(_ => _.Key == "SourceContext")
                        .Select(_ => _.Value.ToString().RemoveQuotes())
                        .FirstOrDefault();
                    return p.Contains(sourceContext);
                }

                return InnerWhen;
            }

            return GetComposedWhenEnricherConfiguration(OuterWhen(sources));
        }

        public WhenEnricherConfiguration FromSourceContext<T>()
        {
            return FromSourceContext(typeof(T).FullName);
        }
    }
}