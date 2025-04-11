using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//Prompt: Add a class for utility functions
namespace MyRazorApp.Helpers
{
    public sealed class Utils
    {
        private static Utils _instance;
        private static readonly object _lock = new();

        private Utils() { }

        public static Utils Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new Utils();
                }
            }
        }

        public void ExportToJson<T>(IEnumerable<T> data, string filePath, Func<T, object> selector = null)
        {
            var output = selector != null ? data.Select(selector).Cast<object>() : data.Cast<object>();
            File.WriteAllText(filePath, JsonConvert.SerializeObject(output, Formatting.Indented));
        }

        public Func<T, Dictionary<string, object>> CreatePropertySelector<T>(Dictionary<string, bool> propertyFlags)
        {
            return item =>
            {
                var result = new Dictionary<string, object>();
                foreach (var prop in typeof(T).GetProperties())
                {
                    if (propertyFlags.TryGetValue(prop.Name, out bool include) && include)
                    {
                        result[prop.Name] = prop.GetValue(item) ?? string.Empty;
                    }
                }
                return result;
            };
        }
    }
}