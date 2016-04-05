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
using System.Collections.Generic;
using System.Linq;

namespace Serilog.Enricher.WhenDo
{
    public class WhenDoEnricher : ILogEventEnricher
    {
        readonly Action<LogEvent, ILogEventPropertyFactory> _doFunc;
        readonly Func<LogEvent, bool>[] _whenFuncs;

        public WhenDoEnricher(IEnumerable<Func<LogEvent, bool>> whenFuncs, Action<LogEvent, ILogEventPropertyFactory> doFunc)
        {
            _whenFuncs = whenFuncs.ToArray();
            _doFunc = doFunc;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (_whenFuncs.All(s => s(logEvent))) _doFunc(logEvent, propertyFactory);
        }
    }
}