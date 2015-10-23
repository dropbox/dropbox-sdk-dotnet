namespace SimpleBlogDemo.Helpers
{
    using System.Linq;
    using System.Text;
    using System.Web;

    using Tanka.Markdown;
    using Tanka.Markdown.Blocks;
    using Tanka.Markdown.Html;
    using Tanka.Markdown.Inline;

    public static class MarkdownHelpers
    {
        static readonly MarkdownConverter converter = new MarkdownConverter();

        public static HtmlString ParseMarkdown(this string markdown)
        {
            return converter.Convert(markdown);
        }

        class MarkdownConverter
        {
            private MarkdownParser parser;
            private MarkdownHtmlRenderer renderer;

            public MarkdownConverter()
            {
                this.parser = new MarkdownParser();
                this.renderer = new MarkdownHtmlRenderer();

                var para = this.renderer.Options.Renderers.First(r => r is ParagraphRenderer) as ParagraphRenderer;
                para.SpanRenderers.Insert(0, new SafeTextSpanRenderer());
                for (int i = 0; i < this.renderer.Options.Renderers.Count; i++)
                {
                    if (this.renderer.Options.Renderers[i] is OrderedListRenderer)
                    {
                        this.renderer.Options.Renderers[i] = new SafeOrderedListRenderer(para);
                    }
                    else if (this.renderer.Options.Renderers[i] is UnorederedListRenderer)
                    {
                        this.renderer.Options.Renderers[i] = new SafeUnorderedListRenderer(para);
                    }
                }
            }

            public HtmlString Convert(string markdown)
            {
                var document = this.parser.Parse(markdown);
                return new HtmlString(this.renderer.Render(document));
            }

            class SafeTextSpanRenderer : ISpanRenderer
            {
                public bool CanRender(Span span)
                {
                    return span is TextSpan;
                }

                public void Render(Span span, StringBuilder builder)
                {
                    builder.Append(HttpUtility.HtmlEncode(span.ToString()));
                }
            }

            class SafeOrderedListRenderer : BlockRendererBase<List>
            {
                public SafeOrderedListRenderer(ParagraphRenderer para)
                {
                    this.ParagraphRenderer = para;
                }

                protected override bool CanRender(List block)
                {
                    return block.IsOrdered;
                }

                protected override HtmlTags.HtmlTag Render(Document document, List block)
                {
                    var ol = new HtmlTags.HtmlTag("ol");

                    foreach (Item item in block.Items)
                    {
                        var li = new HtmlTags.HtmlTag("li", ol);

                        var itemHtml = this.ParagraphRenderer.Render(document, item).ToHtmlString();
                        li.AppendHtml(itemHtml.Substring(3, itemHtml.Length - 7));
                    }

                    return ol;
                }

                public ParagraphRenderer ParagraphRenderer { get; private set; }
            }

            class SafeUnorderedListRenderer : BlockRendererBase<List>
            {
                public SafeUnorderedListRenderer(ParagraphRenderer para)
                {
                    this.ParagraphRenderer = para;
                }

                protected override bool CanRender(List block)
                {
                    return !block.IsOrdered;
                }

                protected override HtmlTags.HtmlTag Render(Document document, List block)
                {
                    var ul = new HtmlTags.HtmlTag("ul");

                    foreach (Item item in block.Items)
                    {
                        var li = new HtmlTags.HtmlTag("li", ul);

                        var itemHtml = this.ParagraphRenderer.Render(document, item).ToHtmlString();
                        li.AppendHtml(itemHtml.Substring(3, itemHtml.Length - 7));
                    }

                    return ul;
                }

                public ParagraphRenderer ParagraphRenderer { get; private set; }
            }
        }
    }
}
