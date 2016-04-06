namespace Serilog.Enricher.WhenDo.Tests
{
    using FluentAssertions;

    using NUnit.Framework;

    using Serilog.Events;

    [TestFixture]
    public class WhenDoPipeTests
    {
        [Test]
        public void PipeToSecondaryLoggerShouldMatchAndWriteToSecondaryLogger()
        {
            var eventStackSink = new LogEventStackSink();
            var secondarySink = new LogEventStackSink();

            var secondaryLogger = new LoggerConfiguration()
                .WriteTo.Sink(secondarySink, LogEventLevel.Verbose)
                .CreateLogger();
            
            var logger =
                new LoggerConfiguration()
                    .WriteTo.Sink(eventStackSink, LogEventLevel.Verbose)
                    .When().FromSourceContext<WhenDoPipeTests>().Do().PipeTo(secondaryLogger)
                    .CreateLogger();

            logger.Information("Hello");
            eventStackSink.Events.Count.Should().Be(1);
            eventStackSink.Events.Clear();

            logger.ForContext<WhenDoPipeTests>().Information("Hello");
            eventStackSink.Events.Count.Should().Be(1);
            secondarySink.Events.Count.Should().Be(1);
        }

        [Test]
        public void RouteToSecondaryLoggerShouldMatchAndWriteToSecondaryLogger()
        {
            var eventStackSink = new LogEventStackSink();
            var secondarySink = new LogEventStackSink();

            var secondaryLogger = new LoggerConfiguration()
                .WriteTo.Sink(secondarySink, LogEventLevel.Verbose)
                .CreateLogger();

            var logger =
                new LoggerConfiguration()
                    .WriteTo.Sink(eventStackSink, LogEventLevel.Verbose)
                    .When().FromSourceContext<WhenDoPipeTests>().Do().RouteTo(secondaryLogger)
                    .CreateLogger();

            logger.Information("Hello");
            eventStackSink.Events.Count.Should().Be(1);
            eventStackSink.Events.Clear();

            logger.ForContext<WhenDoPipeTests>().Information("Hello");
            eventStackSink.Events.Count.Should().Be(0);
            secondarySink.Events.Count.Should().Be(1);
        }
    }
}