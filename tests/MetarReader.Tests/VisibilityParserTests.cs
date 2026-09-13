using MetarReader.Core.Weather;
using Xunit;

namespace MetarReader.Tests;

public class VisibilityParserTests
{
    [Theory]
    [InlineData("10+", "10 miles or more")]
    [InlineData("6", "6 miles")]
    [InlineData("1", "1 mile")]
    [InlineData("1/2", "1/2 mile")]
    [InlineData("1 1/4", "1 1/4 miles")]
    [InlineData(null, "unknown")]
    [InlineData("", "unknown")]
    public void Parse_ReturnsExpectedPhrase(string? raw, string expected)
    {
        Assert.Equal(expected, VisibilityParser.Parse(raw));
    }
}
