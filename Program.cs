using System.Xml.Linq;

namespace AnkiAppXmlGenerator;
class Program
{
    static void Main(string[] args)
    {
        // CSV file to read from.
        // Column 1 is the English word
        // Column 2 is the Vietnamese word
        string inputFilename = "new-vocab.csv";
        string filepath = "files";
        //string deckName = "myDeckName";

        Console.WriteLine("Enter name of deck:  ");
        string? deckName = Console.ReadLine();
        if(String.IsNullOrEmpty(deckName))
            throw new Exception("deckname is required");

        string outputFileName = $"{deckName}.xml";

        Console.WriteLine($"Reading File: '{inputFilename}'\n");

        string[] source = File.ReadAllLines(Path.Combine(filepath, inputFilename));

        XElement flashDeck = new XElement("deck", new XAttribute("name", deckName),
            new XElement("fields",
                new XElement("tts", "", new XAttribute("name", "Front"), new XAttribute("sides", "11"), new XAttribute("lang", "en-US")),
                new XElement("tts", "", new XAttribute("name", "Back"), new XAttribute("sides", "01"), new XAttribute("lang", "vi-VN")),
                new XElement("text", "", new XAttribute("name", "Notes"), new XAttribute("sides", "01"))
            ),
            new XElement("cards",
                from str in source
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

        Console.WriteLine(flashDeck);

        // Write xml string to a new file.
        using StreamWriter outputFile = new StreamWriter(Path.Combine(filepath, outputFileName));
        outputFile.WriteLine(flashDeck);
    }
}
