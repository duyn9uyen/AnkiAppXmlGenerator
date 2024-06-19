using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AnkiAppXmlGenerator;

static public class Constants
{
    //TODO: Consider using a CSV parser library instead of regex. Can be buggy.
    public static readonly string SplitRegex = @"(?:,)|(['""][^""]*['""])(?:,)*";
}

class Program
{
    static string Filepath { get; set; } = "files";
    static string? OutputFileName { get; set; }
    static string? DeckName { get; set; }

    static void Main(string[] args)
    {
        var source = ReadFile();

        var modifiedData = AddEmptyNotesField(source);

        if(DeckName is null)
            throw new Exception($"{DeckName} can't be null");

        XElement flashDeck = new XElement("deck", new XAttribute("name", DeckName),
            new XElement("fields",
                new XElement("tts", "", new XAttribute("name", "Front"), new XAttribute("sides", "11"), new XAttribute("lang", "en-US")),
                new XElement("tts", "", new XAttribute("name", "Back"), new XAttribute("sides", "01"), new XAttribute("lang", "vi-VN")),
                new XElement("text", "", new XAttribute("name", "Notes"), new XAttribute("sides", "01"))
            ),
            new XElement("cards",
                from str in modifiedData
                let fields = Regex.Split(str, Constants.SplitRegex)
                                    .Where(x => !string.IsNullOrEmpty(x))
                                    .ToArray()
                select new XElement("card",
                    new XElement("tts", fields[0], new XAttribute("name", "Front")),
                    new XElement("tts", fields[1], new XAttribute("name", "Back")),
                    new XElement("text", fields[2], new XAttribute("name", "Notes"))
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
        if(DeckName is null)
            throw new Exception($"{DeckName} is required");

        OutputFileName = $"{DeckName}.xml";

        Console.WriteLine($"Reading File: '{inputFilename}'\n");

        string[] datarows = File.ReadAllLines(Path.Combine(Filepath, inputFilename));

        return datarows;
    }

    static List<string> AddEmptyNotesField(string[] datarows)
    {
        List<string> modifiedData = new();

        foreach (var row in datarows)
        {
            var originalFields = Regex.Split(row, Constants.SplitRegex)
                                    .Select(x => x.Trim())
                                    .Where(match => !string.IsNullOrEmpty(match))
                                    .ToList();

            if (originalFields.Count == 2)
            {
                // missing the third note field, so let's add an empty note.
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

        if(OutputFileName is null)
        {
            throw new Exception($"{OutputFileName} can't be null");
        }
        else
        {
            // Write xml string to a new file.
            using StreamWriter outputFile = new StreamWriter(Path.Combine(Filepath, OutputFileName));
            outputFile.WriteLine(flashDeck);
        }
    }
}
