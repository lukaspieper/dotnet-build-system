﻿using System.IO;
using System.Xml.Xsl;
using Nuke.Common.IO;

namespace Utilities
{
    static class XmlTransformation
    {
        internal static void TransformXml(AbsolutePath sourceXmlPath, AbsolutePath xsltPath, AbsolutePath outputFilePath, XsltArgumentList arguments = null)
        {
            Serilog.Log.Information($"Transforming with '{xsltPath}'.");

            var transformation = new XslCompiledTransform();
            transformation.Load(xsltPath);

            using (var streamWriter = new StreamWriter(outputFilePath))
            {
                transformation.Transform(sourceXmlPath, arguments, streamWriter);
            }

            Serilog.Log.Information($"Transformation completed. Output file: '{outputFilePath}'.");
        }
    }
}