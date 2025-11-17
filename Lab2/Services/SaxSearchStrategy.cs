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
    public class SaxSearchStrategy : IXmlSearchStrategy
    {
        public IEnumerable<Contact> Search(string xmlPath, string? keyword, string? attributeName, string? attributeValue)
        {
            var results = new List<Contact>();
            using var reader = XmlReader.Create(xmlPath, new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true });

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "contact")
                {
                    using var subtree = reader.ReadSubtree();
                    var contact = new Contact();

                    if (reader.HasAttributes)
                    {
                        for (int i = 0; i < reader.AttributeCount; i++)
                        {
                            reader.MoveToAttribute(i);
                            contact.Attributes[reader.Name] = reader.Value;
                        }
                        reader.MoveToElement();
                    }

                    if (!string.IsNullOrWhiteSpace(attributeName) && !string.IsNullOrWhiteSpace(attributeValue))
                    {
                        if (!contact.Attributes.TryGetValue(attributeName!, out var val) ||
                            !string.Equals(val, attributeValue, StringComparison.OrdinalIgnoreCase))
                            continue;
                    }

                    string allText = string.Empty;
                    using (subtree)
                    {
                        var inner = subtree;
                        while (inner.Read())
                        {
                            if (inner.NodeType == XmlNodeType.Element)
                            {
                                var name = inner.Name;
                                if (name is "name" or "faculty" or "department" or "specialty" or "collaboration" or "timeframe")
                                {
                                    var value = inner.ReadElementContentAsString();
                                    switch (name)
                                    {
                                        case "name": contact.FullName = value; break;
                                        case "faculty": contact.Faculty = value; break;
                                        case "department": contact.Department = value; break;
                                        case "specialty": contact.Specialty = value; break;
                                        case "collaboration": contact.CollaborationType = value; break;
                                        case "timeframe": contact.Timeframe = value; break;
                                    }
                                    allText += value + " ";
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(keyword) &&
                        allText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) < 0)
                        continue;

                    results.Add(contact);
                }
            }
            return results;
        }
    }
}
