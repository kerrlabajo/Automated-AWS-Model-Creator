using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace LSC_Trainer
{
    static class FileHandler
    {
        public static Dictionary<string, string> ReadYamlFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string content = File.ReadAllText(filePath);

                // Deserialize YAML to a Dictionary<string, string>
                var deserializer = new DeserializerBuilder().Build();
                return deserializer.Deserialize<Dictionary<string, string>>(content);
            }
            else
            {
                Console.WriteLine($"File not found: {filePath}");
                return null;
            }
        }
    }
}
