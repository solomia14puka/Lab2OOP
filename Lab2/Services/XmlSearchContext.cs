using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Services
{
    public class XmlSearchContext
    {
        private IXmlSearchStrategy _strategy;
        public XmlSearchContext(IXmlSearchStrategy defaultStrategy) => _strategy = defaultStrategy;
        public void SetStrategy(IXmlSearchStrategy strategy) => _strategy = strategy;

        public IEnumerable<Models.Contact> Search(string xmlPath, string? keyword, string? attributeName, string? attributeValue)
            => _strategy.Search(xmlPath, keyword, attributeName, attributeValue);
    }
}
