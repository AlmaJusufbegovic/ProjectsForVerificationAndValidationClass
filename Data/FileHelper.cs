using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ForBooks.Data
{
    public static class FileHelper
    {
        private static string basePath = "DataFiles";

        static FileHelper()
        {
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
        }

        private static string GetPath(string fileName)
        {
            return Path.Combine(basePath, fileName + ".json");
        }

        // Ucitavanje podataka 
        public static List<T> LoadFromFile<T>(string fileName)
        {
            string path = GetPath(fileName);

            if (!File.Exists(path))
            {
                return new List<T>();
            }

            string json = File.ReadAllText(path);

                List<T>? data = JsonConvert.DeserializeObject<List<T>>(json);

            if (data == null)
            {
                data = new List<T>();
            }

            return data;
        }

        // Cuvanje podataka 
        public static void SaveToFile<T>(string fileName, List<T> data)
        {
            string path = GetPath(fileName);
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}

