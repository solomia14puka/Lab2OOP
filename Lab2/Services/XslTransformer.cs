using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace Lab2.Services
{
    public class XslTransformer
    {
        public string TransformToHtml(string xmlPath, string xslPath, string? outputPath = null)
        {
            outputPath ??= Path.Combine(Path.GetDirectoryName(xmlPath)!,
                                        Path.GetFileNameWithoutExtension(xmlPath) + ".html");

            var xslt = new XslCompiledTransform(true);
            using var xslReader = XmlReader.Create(xslPath);
            xslt.Load(xslReader);
            using var xmlReader = XmlReader.Create(xmlPath);
            using var writer = XmlWriter.Create(outputPath, xslt.OutputSettings);
            xslt.Transform(xmlReader, writer);
            return outputPath;
        }
    }
}
