using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using MigrationTool.Model;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;
using Ude; // Universal Detector Engine

namespace MigrationTool
{
    internal class Program
    {
        public static Encoding DetectFileEncoding(string filePath)
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                var detector = new CharsetDetector();
                detector.Feed(fileStream);
                detector.DataEnd();
                if (detector.Charset != null)
                {
                    return Encoding.GetEncoding(detector.Charset);
                }
            }
            return Encoding.Default; // Fallback to default encoding if detection fails
        }
    
    static void Main(string[] args)
        {
            // Register code page provider to support ANSI encoding (1252)
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Specify the input and output file paths
            string inputFilePath = @"C:\Users\denys\source\repos\ConvertJsonToUtf8\ConvertJsonToUtf8\ANSI.json";
            string outputFilePath = @"C:\Users\denys\source\repos\ConvertJsonToUtf8\ConvertJsonToUtf8\UTF8.json";

            Console.WriteLine(DetectFileEncoding(inputFilePath));

            Encoding detectedEncoding = DetectFileEncoding(inputFilePath);

            try
            {
                // Read the ANSI-encoded file
                string ansiContent;
                using (var reader = new StreamReader(inputFilePath, detectedEncoding)) 
                {
                    ansiContent = reader.ReadToEnd();
                }

                Console.WriteLine(ansiContent);

                //Deserialize to ensure content is read correctly
                var document = JsonConvert.DeserializeObject<JSON.Document>(ansiContent);
                
                // Serialize the document to UTF-8 and save it
                using (var writer = new StreamWriter(outputFilePath, false, new UTF8Encoding(false)))
                {
                    writer.Write(JsonConvert.SerializeObject(document, Formatting.Indented));
                }

                Console.WriteLine("Conversion completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}

namespace MigrationTool.Model
{
    internal class JSON
    {
        public class Content
        {
            public string File { get; set; }
            public string MD5 { get; set; }
        }

        public class Document
        {
            public Guid Id { get; set; }
            public Guid VsId { get; set; }
            public DateTime DateCreated { get; set; }
            public string Creator { get; set; }
            public DateTime DateLastModified { get; set; }
            public string LastModifier { get; set; }
            public string DocumentTitle { get; set; }
            public string AKTEN_ID { get; set; }
            public string Kategorie { get; set; }
            public DateTime? Aufbewahrungsfrist { get; set; }
            public DateTime? Loeschdatum { get; set; }
            public string Dokumentendatum { get; set; }
            public bool Verschickbar { get; set; }
            public string Bemerkung { get; set; }
            public List<Content> Content { get; set; }
        }
    }
}

