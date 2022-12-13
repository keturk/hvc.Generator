// MIT License
//
// Copyright (c) 2022 Kamil Ercan Turkarslan
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Globalization;
using Antlr4.StringTemplate;
using hvc.Extensions;

namespace hvc.Generator;

public class TemplateStack
{
    protected SortedDictionary<String, TemplateGroupFile> TemplateGroupFiles;

    public TemplateStack()
    {
        TemplateGroupFiles =
            new SortedDictionary<String, TemplateGroupFile>(StringComparer.InvariantCultureIgnoreCase);
    }

    public Template this[String templateName] => Get(templateName).GetInstanceOf(templateName);

    public TemplateStack Push(TemplateGroupFile templateGroupFile)
    {
        foreach (var name in templateGroupFile.GetTemplateNames())
        {
            if (TemplateGroupFiles.ContainsKey(name))
                TemplateGroupFiles.Remove(name);

            TemplateGroupFiles.Add(name, templateGroupFile);
        }

        return this;
    }

    public TemplateGroupFile Get(String name)
    {
        var templateName = $"/{name}";

        if (!TemplateGroupFiles.ContainsKey(templateName))
            throw new KeyNotFoundException();

        return TemplateGroupFiles[templateName];
    }

    public static TemplateStack Create(Type startType, Type baseType)
    {
        var typeHierarchy = startType.GetTypesInHierarchy(baseType);

        var templateStack = new TemplateStack();

        foreach (var currentType in typeHierarchy)
        {
            var typeName = currentType.IsGenericType
                ? currentType.Name.BeforeOccurrenceOf("`")
                : currentType.Name;

            var templateResourceName = currentType.Assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith($"{typeName}.stg"));
            if (templateResourceName == null)
                continue;

            var resourceFile = currentType.Assembly.GetManifestResourceStream(templateResourceName);
            if (resourceFile == null)
                continue;

            var templateFilename = Path.Combine(Path.GetTempPath(), templateResourceName);
            using (var fileStream = new FileStream(templateFilename, FileMode.Create, FileAccess.Write))
                resourceFile.CopyTo(fileStream);
            
            templateStack.Push(CreateTemplateGroupFile(templateFilename));
            File.Delete(templateFilename);
        }

        return templateStack;
    }

    private static TemplateGroupFile CreateTemplateGroupFile(String filename)
    {
        var newTemplateGroupFile = new TemplateGroupFile(filename);

        newTemplateGroupFile.RegisterRenderer(typeof(String), new StringRenderer());

        return newTemplateGroupFile;
    }

    internal class StringRenderer : IAttributeRenderer
    {
        public String ToString(Object obj, String formatString, CultureInfo culture)
        {
            if (String.IsNullOrWhiteSpace(formatString))
                return (String)obj;

            return formatString switch
            {
                "CamelCase" => ((String)obj).CamelCase(),
                "PluralCamelCase" => ((String)obj).ToPlural().CamelCase(),
                "SingularCamelCase" => ((String)obj).ToSingular().CamelCase(),
                "Plural" => ((String)obj).ToPlural(),
                "Singular" => ((String)obj).ToSingular(),
                "LowerSingular" => ((String)obj).ToLower().ToSingular(),
                "LowerPlural" => ((String)obj).ToLower().ToPlural(),
                "PluralSnakeCase" => ((String)obj).ToPlural().SnakeCase(),
                "SingularSnakeCase" => ((String)obj).ToSingular().SnakeCase(),
                "SnakeCase" => ((String)obj).SnakeCase(),
                _ => throw new InvalidOperationException($"Unknown String format '{formatString}'")
            };
        }
    }
}