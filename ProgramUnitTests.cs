using System.Text.RegularExpressions;
using Xunit;

namespace AnkiAppXmlGenerator;

public class ProgramUnitTests
{
    [Theory]
    [InlineData("\"hello, my friend\", tiếng Việt, my notes", 3)]
    [InlineData("\"(comma in quotes example) dear friend, I'm home\", \"bạn thân mến, tôi đang ở nhà\", more notes here", 3)]
    [InlineData("more english text, tiếng Việt", 2)]
    [InlineData("\"dear friend, I'm home\", \"Thanks, your other friend.\",", 2)]
    [InlineData("\"dear friend, I'm home\", \"Thanks, your other friend.\"", 2)]
    public void IgnoreCommasInQuotes(string input, int expectedFieldCount)
    {
        //string SplitRegex = @"(?:,)|(['""][^""]*['""])(?:,)*";


        var matches = Regex.Matches(input, Constants.SplitRegex);


        var separatedValues = Regex.Split(input, Constants.SplitRegex)
                                    .Select(x => x.Trim())
                                    .Where(match => !string.IsNullOrEmpty(match))
                                    .ToList();

        Assert.Equal(expectedFieldCount, separatedValues.Count);
    }
}