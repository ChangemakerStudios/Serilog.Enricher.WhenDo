namespace Serilog.Enricher.WhenDo.Tests
{
    using System.Linq;

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

            var secondaryLogger = new LoggerConfiguration().MinimumLevel.Verbose()
                .WriteTo.Sink(secondarySink, LogEventLevel.Verbose)
                .CreateLogger();

            var logger =
                new LoggerConfiguration().MinimumLevel.Verbose()
                    .WriteTo.Sink(eventStackSink, LogEventLevel.Verbose)
                    .When().FromSourceContext<WhenDoPipeTests>().Do().SendTo(secondaryLogger)
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

            var logger =
                new LoggerConfiguration().MinimumLevel.Verbose()
                    .Enrich.WithProperty("Primary", true)
                    .When().HasProperty("Redirect").Do().PipeTo(secondaryLogger)
                    .WriteTo.Console()
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