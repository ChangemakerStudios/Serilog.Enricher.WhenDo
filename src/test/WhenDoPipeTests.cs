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
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using Serilog.Debugging;
using Serilog.Events;

namespace Serilog.Enricher.WhenDo.Tests
{
    [TestFixture]
    public class WhenDoPipeTests
    {
        public WhenDoPipeTests()
        {
            SelfLog.Enable(s => Console.Error.WriteLine(s));
        }

        [Test]
        public void PipeToSecondaryLoggerShouldMatchAndWriteToSecondaryLogger()
        {
            var eventStackSink = new LogEventStackSink();
            var secondarySink = new LogEventStackSink();

            var secondaryLogger = new LoggerConfiguration().MinimumLevel.Verbose()
                .WriteTo.Sink(secondarySink, LogEventLevel.Verbose)
                .CreateLogger();

            var logger = new LoggerConfiguration().MinimumLevel.Verbose()
                .WriteTo.Sink(eventStackSink, LogEventLevel.Verbose)
                .When()
                .FromSourceContext<WhenDoPipeTests>()
                .Do()
                .SendTo(secondaryLogger)
                .CreateLogger();

            logger.Information("Hello");
            eventStackSink.Message.Count.Should().Be(1);
            eventStackSink.Message.First().Should().Be("Hello");
            eventStackSink.Clear();

            logger.ForContext<WhenDoPipeTests>().Information("Testing");

            eventStackSink.Message.Count.Should().Be(1);
            eventStackSink.Message.First().Should().Be("Testing");

            secondarySink.Message.Count.Should().Be(1);
            secondarySink.Message.First().Should().Be("Testing");
        }

        [Test]
        public void RouteToSecondaryLoggerShouldMatchAndWriteToSecondaryLogger()
        {
            var eventStackSink = new LogEventStackSink();
            var secondarySink = new LogEventStackSink();

            var secondaryLogger = new LoggerConfiguration().MinimumLevel.Verbose()
                .Enrich.WithProperty("Secondary", true)
                .WriteTo.Sink(secondarySink, LogEventLevel.Verbose)
                .CreateLogger();

            var logger = new LoggerConfiguration().MinimumLevel.Verbose()
                .Enrich.WithProperty("Primary", true)
                .When()
                .HasProperty("Redirect")
                .Do()
                .PipeTo(secondaryLogger)
                .WriteTo.Sink(eventStackSink, LogEventLevel.Verbose)
                .CreateLogger();

            logger.Information("Hello");
            eventStackSink.Events.Count.Should().Be(1);
            eventStackSink.Message.First().Should().Be("Hello");
            eventStackSink.Clear();

            logger.ForContext("Redirect", true).Verbose("HelloThere");
            eventStackSink.Events.Count.Should().Be(0);
            secondarySink.Events.Count.Should().Be(1);
            secondarySink.Message.First().Should().Be("HelloThere");
        }
    }
}