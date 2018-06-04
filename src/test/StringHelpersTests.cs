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