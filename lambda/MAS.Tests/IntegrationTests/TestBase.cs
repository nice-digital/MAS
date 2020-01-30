using AngleSharp.Html;
using AngleSharp.Html.Parser;
using MAS.Tests.Infrastructure;
using Shouldly;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace MAS.Tests.IntegrationTests
{
    public abstract class TestBase : IDisposable
    {
        private const string NewLine = "\r\n";
        private const string Indentation = "\t";
        private static PrettyMarkupFormatter HtmlFormatter = new PrettyMarkupFormatter { Indentation = Indentation, NewLine = NewLine };

        private static Regex MergeTagAtStartOfLine = new Regex(@"^\s+\*\|", RegexOptions.Multiline);
        private static Regex SpaceAfterMergeTag = new Regex(@"\|\*\s+", RegexOptions.Multiline);
        private static Regex SpaceBeforeMergeTag = new Regex(@"\s+\*\|", RegexOptions.Multiline);

        protected readonly MASWebApplicationFactory _factory;
        protected readonly ITestOutputHelper _output;

        public TestBase(ITestOutputHelper output)
        {
            _factory = new MASWebApplicationFactory(output);
            _output = output;

            // Pretty format HMTL by default yo
            ShouldlyConfiguration.ShouldMatchApprovedDefaults
                .WithScrubber(s =>
                {
                    // Don't try to pretty print XML, assume it's already nicely formatted for easy diffing
                    if (s.Contains("<?xml"))
                        return s;

                    HtmlParser parser = new HtmlParser();
                    using (var document = parser.ParseDocument(s))
                    using (var stringWriter = new StringWriter())
                    {
                        // TODO: Use document.Body if there's no content in the head
                        document.ToHtml(stringWriter, HtmlFormatter);
                        s = stringWriter.ToString();
                    }
                    
                    // Tidy up MailChimp merge tags for nice diffing
                    s = MergeTagAtStartOfLine.Replace(s, "*|");
                    s = SpaceAfterMergeTag.Replace(s, "|*" + NewLine + Indentation);
                    s = SpaceBeforeMergeTag.Replace(s, NewLine + "*|");

                    return s;
                });
        }

        public virtual void Dispose()
        {
            _factory.Dispose();
        }
    }
}
