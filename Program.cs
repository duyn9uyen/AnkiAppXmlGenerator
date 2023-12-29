using System.Xml.Linq;

namespace AnkiAppXmlGenerator;
class Program
{
    static string Filepath = "files";
    static string OutputFileName = "";
    static string? DeckName;

    static void Main(string[] args)
    {
        var source = ReadFile();

        var modifiedData = EditData(source);

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
                let fields = str.Split(',')
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
            var originalFields = row.Split(',').ToList();

            if (originalFields.Count == 2)
            {
                // missing the third note field, so let's add an empty string.
                originalFields.Add("");
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
