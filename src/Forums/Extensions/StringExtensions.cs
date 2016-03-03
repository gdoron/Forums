using System;
using CommonMark;
using CommonMark.Syntax;

namespace Forums.Extensions
{
    public static class StringExtensions
    {
        public static string ParseMarkdown(this string input)
        {
            Block document = CommonMarkConverter.Parse(input);
            using (var writer = new System.IO.StringWriter())
            {
                // write the HTML output
                CommonMarkConverter.ProcessStage3(document, writer);
                var results = writer.ToString();
                results = results.Replace(Environment.NewLine, "<br/>");
                return results;
            }
        }
    }
}