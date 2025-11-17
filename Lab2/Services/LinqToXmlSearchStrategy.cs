using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Lab2.Models;
using Contact = Lab2.Models.Contact;

namespace Lab2.Services
{
    public class LinqToXmlSearchStrategy : IXmlSearchStrategy
    {
        public IEnumerable<Contact> Search(string xmlPath, string? keyword, string? attributeName, string? attributeValue)
        {
            var xdoc = XDocument.Load(xmlPath);
            var contacts = xdoc.Root!.Elements("contact");

            if (!string.IsNullOrWhiteSpace(attributeName) && !string.IsNullOrWhiteSpace(attributeValue))
            {
                contacts = contacts.Where(e =>
                    string.Equals((string?)e.Attribute(attributeName!), attributeValue, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                contacts = contacts.Where(e => e.Descendants()
                    .Any(d => !string.IsNullOrEmpty(d.Value) &&
                              d.Value.Contains(keyword!, StringComparison.OrdinalIgnoreCase)));
            }

            foreach (var e in contacts)
            {
                var c = new Contact
                {
                    FullName = (string?)e.Element("name"),
                    Faculty = (string?)e.Element("faculty"),
                    Department = (string?)e.Element("department"),
                    Specialty = (string?)e.Element("specialty"),
                    CollaborationType = (string?)e.Element("collaboration"),
                    Timeframe = (string?)e.Element("timeframe")
                };
                foreach (var a in e.Attributes())
                    c.Attributes[a.Name.LocalName] = a.Value;

                yield return c;
            }
        }
    }
}
