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
using System.Linq;

using Serilog.Configuration;
using Serilog.Events;

namespace Serilog.Enricher.WhenDo
{
    using System.Collections.Generic;

    public class WhenEnricherConfiguration
    {
        readonly LoggerConfiguration _configuration;

        readonly Func<LogEvent, bool>[] _whenFuncs;
        
        public WhenEnricherConfiguration(LoggerConfiguration configuration, IEnumerable<Func<LogEvent, bool>> conditions = null)
        {
            _configuration = configuration;
            _whenFuncs = (conditions ?? new Func<LogEvent, bool>[0]).ToArray();
        }

        WhenEnricherConfiguration GetComposedWhenEnricherConfiguration(params Func<LogEvent, bool>[] when)
        {
            return new WhenEnricherConfiguration(_configuration, _whenFuncs.Concat(when));
        }

        public DoConfiguration Do()
        {
            return
                new DoConfiguration(
                    func => _configuration.Enrich.With(new WhenDoEnricher(_whenFuncs, func)),
                    func => _configuration.Filter.With(new WhenDoPipeFilter(_whenFuncs, func)));
        }

        public WhenEnricherConfiguration IsLevelEqualTo(LogEventLevel level)
        {
            Func<LogEventLevel, Func<LogEvent, bool>> when = l => e => e.Level == l;

            return GetComposedWhenEnricherConfiguration(when(level));
        }

        public WhenEnricherConfiguration IsLevelOrLess(LogEventLevel level)
        {
            Func<LogEventLevel, Func<LogEvent, bool>> when = l => e => e.Level <= l;

            return GetComposedWhenEnricherConfiguration(when(level));
        }

        public WhenEnricherConfiguration IsLevelOrHigher(LogEventLevel level)
        {
            Func<LogEventLevel, Func<LogEvent, bool>> when = l => e => e.Level >= l;

            return GetComposedWhenEnricherConfiguration(when(level));
        }

        public WhenEnricherConfiguration HasProperty(params string[] properties)
        {
            Func<string[], Func<LogEvent, bool>> when = p => e => e.Properties.Keys.Intersect(p).Any();

            return GetComposedWhenEnricherConfiguration(when(properties));
        }

        public WhenEnricherConfiguration MissingProperty(params string[] properties)
        {
            Func<string[], Func<LogEvent, bool>> when = p => e => !e.Properties.Keys.Intersect(p).Any();

            return GetComposedWhenEnricherConfiguration(when(properties));
        }

        public WhenEnricherConfiguration IsException(params Type[] exceptionTypes)
        {
            Func<Type[], Func<LogEvent, bool>> when = es => e => e.Exception != null && es.Contains(e.Exception.GetType());

            return GetComposedWhenEnricherConfiguration(when(exceptionTypes));
        }

        public WhenEnricherConfiguration IsException<TException>()
        {
            return IsException(typeof(TException));
        }

        public WhenEnricherConfiguration NoException()
        {
            Func<LogEvent, bool> when = e => e.Exception == null;

            return GetComposedWhenEnricherConfiguration(when);
        }

        public WhenEnricherConfiguration AnyException()
        {
            Func<LogEvent, bool> when = e => e.Exception != null;

            return GetComposedWhenEnricherConfiguration(when);
        }

        public WhenEnricherConfiguration IsNotException(params Type[] exceptionTypes)
        {
            Func<Type[], Func<LogEvent, bool>> when = es => e => e.Exception != null && !es.Contains(e.Exception.GetType());

            return GetComposedWhenEnricherConfiguration(when(exceptionTypes));
        }

        public WhenEnricherConfiguration IsNotExceptionOf<TException>()
        {
            return IsNotException(typeof(TException));
        }

        public WhenEnricherConfiguration FromSourceContext(params string[] sources)
        {
            Func<string[], Func<LogEvent, bool>> when = p => e =>
            {
                var sourceContext = e.Properties.Where(_ => _.Key == "SourceContext").Select(_ => _.Value.ToString().RemoveQuotes()).FirstOrDefault();
                return p.Contains(sourceContext);
            };

            return GetComposedWhenEnricherConfiguration(when(sources));
        }

        public WhenEnricherConfiguration FromSourceContext<T>()
        {
            return FromSourceContext(typeof(T).FullName);
        }
    }
}