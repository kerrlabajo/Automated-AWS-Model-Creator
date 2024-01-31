using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using System.Windows.Forms;

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

        public static string WriteYamlFile(Dictionary<string, string> data)
        {
            if (data == null)
            {
                Console.WriteLine("No data provided to write.");
                return "";
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "YAML files (*.yaml)|*.yaml|All files (*.*)|*.*";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    var serializer = new SerializerBuilder().Build();
                    string yamlContent = serializer.Serialize(data);

                    File.WriteAllText(filePath, yamlContent);
                    return filePath;
                }
                else
                {
                    Console.WriteLine("---------- Canceled ------------");
                    return "";
                }
            }
        }
    }
}
