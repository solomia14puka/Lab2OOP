using Contact = Lab2.Models.Contact;

namespace Lab2.Services
{
    public interface IXmlSearchStrategy
    {
        IEnumerable<Contact> Search(string xmlPath, string? keyword, string? attributeName, string? attributeValue);
    }
}
