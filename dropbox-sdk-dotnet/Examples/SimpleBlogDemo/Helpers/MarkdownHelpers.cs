// <copyright file="MarkdownHelpers.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>

namespace SimpleBlogDemo.Helpers
{
    using System.Linq;
    using System.Text;
    using System.Web;
    using Markdig;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Html;

    public static class MarkdownHelpers
    {
        private static readonly MarkdownConverter Converter = new MarkdownConverter();

        public static HtmlString ParseMarkdown(this string markdown)
        {
            return Converter.Convert(markdown);
        }

        private class MarkdownConverter
        {
            private MarkdownPipeline pipeline;

            public MarkdownConverter()
            {
                this.pipeline = new MarkdownPipelineBuilder()
                    .DisableHtml()
                    .Build();
            }

            public HtmlString Convert(string markdown)
            {
                return new HtmlString(Markdown.ToHtml(markdown, this.pipeline));
            }
        }
    }
}
