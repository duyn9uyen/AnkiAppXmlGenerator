using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AnkiAppXmlGenerator;
class Program
{
    static string Filepath { get; set; } = "files";
    static string? OutputFileName { get; set; }
    static string? DeckName { get; set; }

    //TODO: There is a bug with this regex when parsing CSV files with quotes around commas
    // works:  my english text, tiếng Việt, my notes here
    // works (will add an empty note): more english text, tiếng Việt
    // does not work because the second field is in quotes: "(comma example) dear friend, I'm home", "bạn thân mến, tôi đang ở nhà", more notes here
    //(Consider using a CSV parser library instead of regex.)
    static readonly string SplitRegex = @"(?:,)|(['""].+['""])(?:,)";

    static void Main(string[] args)
    {
        var source = ReadFile();

        var modifiedData = EditData(source);

        var originalFields = Regex.Split(source[0], SplitRegex)
                                    //.Where(x => !string.IsNullOrEmpty(x))
                                    .ToList();

        var modifiedFields = Regex.Split(modifiedData[0], SplitRegex)
                                    .Where(x => !string.IsNullOrEmpty(x))
                                    .ToArray();

        if(DeckName is null)
            throw new Exception("DeckName can't be null");

        XElement flashDeck = new XElement("deck", new XAttribute("name", DeckName),
            new XElement("fields",
                new XElement("tts", "", new XAttribute("name", "Front"), new XAttribute("sides", "11"), new XAttribute("lang", "en-US")),
                new XElement("tts", "", new XAttribute("name", "Back"), new XAttribute("sides", "01"), new XAttribute("lang", "vi-VN")),
                new XElement("text", "", new XAttribute("name", "Notes"), new XAttribute("sides", "01"))
            ),
            new XElement("cards",
                from str in modifiedData
                //let fields = str.Split(',')
                let fields = Regex.Split(str, SplitRegex)
                                    .Where(x => !string.IsNullOrEmpty(x))
                                    .ToArray()
                select new XElement("card",
                    new XElement("field", new XAttribute("name", "Front"),
                        new XElement("tts", fields[0])
                    ),
                    new XElement("field", new XAttribute("name", "Back"),
                        new XElement("tts", fields[1])
                    ),
                    new XElement("field", fields[2], new XAttribute("name", "Notes"))
                )
            )
        );

        WriteFile(flashDeck);
    }

    static string[] ReadFile()
    {
        // CSV file to read from.
        // Column 1 is the English word
        // Column 2 is the Vietnamese word
        string inputFilename = "new-vocab.csv";

        //DeckName = "myDeckName";
        Console.WriteLine("Enter name of deck:  ");
        DeckName = Console.ReadLine();
        if(String.IsNullOrEmpty(DeckName))
            throw new Exception("deckname is required");

        OutputFileName = $"{DeckName}.xml";

        Console.WriteLine($"Reading File: '{inputFilename}'\n");

        string[] datarows = File.ReadAllLines(Path.Combine(Filepath, inputFilename));

        return datarows;
    }

    static List<string> EditData(string[] datarows)
    {
        List<string> modifiedData = new();

        foreach (var row in datarows)
        {
            // var originalFields = row.Split(",");

            var originalFields = Regex.Split(row, SplitRegex)
                                    .Where(x => !string.IsNullOrEmpty(x))
                                    .Select(y => y.Trim())
                                    .ToList();

            if (originalFields.Count == 2)
            {
                // missing the third note field, so let's add an empty note.
                // TODO: We need to have a space here, otherwise it will get stripped
                originalFields.Add(" ");
            }

            // can use the inbuilt string.Join() method
            // to create a comma-separated string from a list of strings
            var allFields = string.Join(",", originalFields);

            modifiedData.Add(allFields);
        }

        return modifiedData;
    }

    static void WriteFile(XElement flashDeck)
    {
        Console.WriteLine(flashDeck);

        // Write xml string to a new file.
        using StreamWriter outputFile = new StreamWriter(Path.Combine(Filepath, OutputFileName));
        outputFile.WriteLine(flashDeck);
    }
}
