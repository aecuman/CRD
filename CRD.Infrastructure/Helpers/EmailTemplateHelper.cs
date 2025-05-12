using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CRD.Infrastructure.Helpers
{
    public static class EmailTemplateHelper
    {
        public static string LoadTemplateFromEmbedded(string fileName, Dictionary<string, string> placeholders)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"CRD.Infrastructure.EmailTemplates.{fileName}";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
                return $"[Embedded template not found: {resourceName}]";

            using var reader = new StreamReader(stream, Encoding.UTF8);
            var content = reader.ReadToEnd();

            foreach (var entry in placeholders)
            {
                string key = $"{{{{{entry.Key}}}}}";
                string value = entry.Value ?? string.Empty;
                content = content.Replace(key, value);
            }

            // Optional cleanup
            content = Regex.Replace(content, "{{.*?}}", string.Empty); // remove unmatched placeholders
            return content.Trim(); // remove any trailing newlines or whitespace
        }
    }
}
