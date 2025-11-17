using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Lab2.Models;
using Contact = Lab2.Models.Contact;

namespace Lab2.Services
{
    public class DomSearchStrategy : IXmlSearchStrategy
    {
        public IEnumerable<Contact> Search(string xmlPath, string? keyword, string? attributeName, string? attributeValue)
        {
            var doc = new XmlDocument();
            doc.Load(xmlPath);

            var conditions = new List<string>();
            if (!string.IsNullOrWhiteSpace(attributeName) && !string.IsNullOrWhiteSpace(attributeValue))
                conditions.Add($"@{attributeName}='{EscapeQuotes(attributeValue!)}'");
            if (!string.IsNullOrWhiteSpace(keyword))
                conditions.Add($"contains(., '{EscapeQuotes(keyword!)}')");

            string predicate = conditions.Count > 0 ? $"[{string.Join(" and ", conditions)}]" : string.Empty;
            string xpath = $"//contacts/contact{predicate}";

            var nodeList = doc.SelectNodes(xpath);
            var results = new List<Contact>();
            if (nodeList is null) return results;

            foreach (XmlNode node in nodeList)
            {
                var c = new Contact
                {
                    FullName = node.SelectSingleNode("name")?.InnerText,
                    Faculty = node.SelectSingleNode("faculty")?.InnerText,
                    Department = node.SelectSingleNode("department")?.InnerText,
                    Specialty = node.SelectSingleNode("specialty")?.InnerText,
                    CollaborationType = node.SelectSingleNode("collaboration")?.InnerText,
                    Timeframe = node.SelectSingleNode("timeframe")?.InnerText
                };

                if (node.Attributes != null)
                {
                    foreach (XmlAttribute a in node.Attributes)
                        c.Attributes[a.Name] = a.Value;
                }
                results.Add(c);
            }
            return results;
        }

        private static string EscapeQuotes(string s) => s.Replace("'", "&apos;");
    }
}
