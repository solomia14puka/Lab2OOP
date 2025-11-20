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
        private static string? GetValue(XmlNode contactNode, string name)
        {
            return contactNode[name]?.InnerText
                   ?? contactNode.Attributes?[name]?.Value;
        }

        public IEnumerable<Contact> Search(
            string xmlPath,
            string? keyword,
            string? attributeName,
            string? attributeValue)
        {
            var doc = new XmlDocument();
            doc.Load(xmlPath);

            var contacts = doc.SelectNodes("//contact")!
                              .Cast<XmlNode>();

            IEnumerable<XmlNode> query = contacts;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(node =>
                {
                    var text =
                        (GetValue(node, "name") ?? "") + " " +
                        (GetValue(node, "faculty") ?? "") + " " +
                        (GetValue(node, "department") ?? "") + " " +
                        (GetValue(node, "specialty") ?? "") + " " +
                        (GetValue(node, "collaboration") ?? "") + " " +
                        (GetValue(node, "timeframe") ?? "");

                    return text.Contains(keyword,
                        System.StringComparison.OrdinalIgnoreCase);
                });
            }

            if (!string.IsNullOrWhiteSpace(attributeName) &&
                !string.IsNullOrWhiteSpace(attributeValue))
            {
                query = query.Where(node =>
                {
                    var attr = node.Attributes?[attributeName];
                    return attr != null &&
                           string.Equals(attr.Value, attributeValue,
                               System.StringComparison.OrdinalIgnoreCase);
                });
            }

            foreach (var node in query)
            {
                var c = new Contact
                {
                    FullName = GetValue(node, "name"),
                    Faculty = GetValue(node, "faculty"),
                    Department = GetValue(node, "department"),
                    Specialty = GetValue(node, "specialty"),
                    CollaborationType = GetValue(node, "collaboration"),
                    Timeframe = GetValue(node, "timeframe")
                };

                if (node.Attributes != null)
                {
                    foreach (XmlAttribute a in node.Attributes)
                        c.Attributes[a.Name] = a.Value;
                }

                yield return c;
            }
        }
    }
}