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
    public class WhenEnricherConfiguration
    {
        readonly LoggerEnrichmentConfiguration _config;

        public WhenEnricherConfiguration(LoggerEnrichmentConfiguration config)
        {
            _config = config;
        }

        WhenDoEnricherConfiguration CreateWhenDoEnricherConfiguration(Func<LogEvent, bool> when)
        {
            return new WhenDoEnricherConfiguration(func => _config.With(new WhenDoEnricher(when, func)));
        }

        public WhenDoEnricherConfiguration IsLevelEqualTo(LogEventLevel level)
        {
            Func<LogEventLevel, Func<LogEvent, bool>> when = l => e => e.Level == l;

            return CreateWhenDoEnricherConfiguration(when(level));
        }

        public WhenDoEnricherConfiguration IsLevelOrLess(LogEventLevel level)
        {
            Func<LogEventLevel, Func<LogEvent, bool>> when = l => e => e.Level <= l;

            return CreateWhenDoEnricherConfiguration(when(level));
        }

        public WhenDoEnricherConfiguration IsLevelOrHigher(LogEventLevel level)
        {
            Func<LogEventLevel, Func<LogEvent, bool>> when = l => e => e.Level >= l;

            return CreateWhenDoEnricherConfiguration(when(level));
        }

        public WhenDoEnricherConfiguration HasProperty(params string[] properties)
        {
            Func<string[], Func<LogEvent, bool>> when = p => e => e.Properties.Keys.Intersect(p).Any();

            return CreateWhenDoEnricherConfiguration(when(properties));
        }

        public WhenDoEnricherConfiguration MissingProperty(params string[] properties)
        {
            Func<string[], Func<LogEvent, bool>> when = p => e => !e.Properties.Keys.Intersect(p).Any();

            return CreateWhenDoEnricherConfiguration(when(properties));
        }

        public WhenDoEnricherConfiguration IsException(params Type[] exceptionTypes)
        {
            Func<Type[], Func<LogEvent, bool>> when = es => e => e.Exception != null && es.Contains(e.Exception.GetType());

            return CreateWhenDoEnricherConfiguration(when(exceptionTypes));
        }

        public WhenDoEnricherConfiguration IsExceptionOf<TException>()
        {
            return IsException(typeof(TException));
        }

        public WhenDoEnricherConfiguration NoException()
        {
            Func<LogEvent, bool> when = e => e.Exception == null;

            return CreateWhenDoEnricherConfiguration(when);
        }

        public WhenDoEnricherConfiguration AnyException()
        {
            Func<LogEvent, bool> when = e => e.Exception != null;

            return CreateWhenDoEnricherConfiguration(when);
        }

        public WhenDoEnricherConfiguration IsNotException(params Type[] exceptionTypes)
        {
            Func<Type[], Func<LogEvent, bool>> when = es => e => e.Exception != null && !es.Contains(e.Exception.GetType());

            return CreateWhenDoEnricherConfiguration(when(exceptionTypes));
        }

        public WhenDoEnricherConfiguration IsNotExceptionOf<TException>()
        {
            return IsNotException(typeof(TException));
        }

    }
}