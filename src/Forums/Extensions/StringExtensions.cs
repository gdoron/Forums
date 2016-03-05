using System;
using CommonMark;
using CommonMark.Syntax;
using Ganss.XSS;

namespace Forums.Extensions
{
    public static class StringExtensions
    {
        public static string ParseMarkdown(this string input, bool sanitize = true)
        {
            Block document = CommonMarkConverter.Parse(input);
            using (var writer = new System.IO.StringWriter())
            {
                // write the HTML output
                CommonMarkConverter.ProcessStage3(document, writer);
                var results = writer.ToString().Trim();
                if (sanitize)
                {
                    results = results.Sanitize();
                }

                results = results.Replace(Environment.NewLine, "<br/>");
                return results;
            }
        }

        public static string Sanitize(this string input)
        {
            var htmlSanitizer = new HtmlSanitizer();
            var sanitizedHtml = htmlSanitizer.Sanitize(input);
            return sanitizedHtml;
        }
    }
}