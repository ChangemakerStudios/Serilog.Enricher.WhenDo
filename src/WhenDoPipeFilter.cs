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