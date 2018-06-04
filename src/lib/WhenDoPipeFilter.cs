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

    public class WhenDoPipeFilter : ILogEventFilter
    {
        readonly Func<LogEvent, bool> _doFunc;
        readonly Func<LogEvent, bool>[] _whenFuncs;

        public WhenDoPipeFilter(IEnumerable<Func<LogEvent, bool>> whenFuncs, Func<LogEvent, bool> doFunc)
        {
            _whenFuncs = whenFuncs.ToArray();
            _doFunc = doFunc;
        }

        public bool IsEnabled(LogEvent logEvent)
        {
            if (_whenFuncs.All(s => s(logEvent)))
            {
                return _doFunc(logEvent);
            }

            return true;
        }
    }
}