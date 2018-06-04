namespace Serilog.Enricher.WhenDo.Tests
{
    using FluentAssertions;

    using NUnit.Framework;

    [TestFixture]
    public class StringHelpersTests
    {
        [Test]
        public void RemoveQuotesShouldRemoveQuotesFromString()
        {
            var blahBlah = @"Blah Blah";
            var quotedString = $@"""{blahBlah}""";

            quotedString.RemoveQuotes().Should().Be(blahBlah);
        }

        [Test]
        public void RemoveQuotesShouldNotTouchNonQuotedString()
        {
            var blahBlah = @"Blah Blah";
            blahBlah.RemoveQuotes().Should().Be(blahBlah);
        }

        [Test]
        public void RemoveQuotesShouldWorkWithJustQuotes()
        {
            var blahBlah = @"""""";
            blahBlah.RemoveQuotes().Should().BeEmpty();
        }
    }
}