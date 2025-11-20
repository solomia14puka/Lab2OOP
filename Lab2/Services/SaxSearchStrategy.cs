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
            using var reader = XmlReader.Create(xmlPath);

            Contact? current = null;
            string? currentElement = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "contact")
                    {
                        current = new Contact();

                        if (reader.HasAttributes)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                var name = reader.Name;
                                var value = reader.Value;

                                current.Attributes[name] = value;

                                switch (name)
                                {
                                    case "name": current.FullName = value; break;
                                    case "faculty": current.Faculty = value; break;
                                    case "department": current.Department = value; break;
                                    case "specialty": current.Specialty = value; break;
                                    case "collaboration": current.CollaborationType = value; break;
                                    case "timeframe": current.Timeframe = value; break;
                                }
                            }
                            reader.MoveToElement();
                        }
                    }
                    else
                    {
                        currentElement = reader.Name;
                    }
                }
                else if (reader.NodeType == XmlNodeType.Text && current != null && currentElement != null)
                {
                    var text = reader.Value;

                    switch (currentElement)
                    {
                        case "name": current.FullName = text; break;
                        case "faculty": current.Faculty = text; break;
                        case "department": current.Department = text; break;
                        case "specialty": current.Specialty = text; break;
                        case "collaboration": current.CollaborationType = text; break;
                        case "timeframe": current.Timeframe = text; break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "contact" && current != null)
                {
                    yield return current;
                    current = null;
                }
            }
        }
    }
}