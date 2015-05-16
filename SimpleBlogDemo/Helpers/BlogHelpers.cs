namespace SimpleBlogDemo.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    using Dropbox.Api;
    using Dropbox.Api.Files;
    using SimpleBlogDemo.Models;

    public static class BlogHelpers
    {
        private static Dictionary<string, Article> ArticleCache = new Dictionary<string, Article>();

        public static async Task<Article> GetArticle(this DropboxClient client, string blogName, ArticleMetadata metadata, bool bypassCache = false)
        {
            if (metadata == null || string.IsNullOrEmpty(blogName))
            {
                return null;
            }

            var key = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", blogName, metadata.Name);

            Article article;
            if (!bypassCache)
            {
                lock (ArticleCache)
                {
                    if (ArticleCache.TryGetValue(key, out article))
                    {
                        if (article.Metadata.Rev == metadata.Rev)
                        {
                            return article;
                        }
                    }
                }
            }

            try
            {
                using (var download = await client.Files.DownloadAsync("/" + metadata.Filename))
                {
                    var content = await download.GetContentAsStringAsync();

                    var html = content.ParseMarkdown();

                    article = Article.FromMetadata(download.Response, html);
                }
            }
            catch (ApiException<DownloadError> e)
            {
                if (e.ErrorResponse.IsNoFile)
                {
                    return null;
                }

                throw;
            }

            lock (ArticleCache)
            {
                ArticleCache[key] = article;
            }

            return article;
        }

        public static void FlushCache(this ControllerBase controller, string blogName)
        {
            var prefix = blogName + ":";

            lock (ArticleCache)
            {
                var keys = from k in ArticleCache.Keys
                           where k.StartsWith(prefix)
                           select k;

                foreach (var key in keys)
                {
                    ArticleCache.Remove(key);
                }
            }
        }

        public static async Task<IEnumerable<ArticleMetadata>> GetArticleList(this DropboxClient client)
        {
            var list = await client.Files.ListFolderAsync(string.Empty);

            var articles = new List<ArticleMetadata>();
            foreach (var item in list.Entries)
            {
                if (!item.Metadata.IsFile)
                {
                    continue;
                }

                var fileMetadata = item.Metadata.AsFile;

                var metadata = ArticleMetadata.Parse(item.Name, fileMetadata.Rev);
                if (metadata != null)
                {
                    articles.Add(metadata);
                }
            }

            articles.Sort((l, r) => l.Date.CompareTo(r.Date));

            return articles;
        }

        public static Tuple<string, DateTime, string> ParseBlogFileName(this string filename)
        {
            var elements = filename.Split('.');
            if (elements.Length != 3 || elements[2] != "md" || elements[1].Length != 8)
            {
                return null;
            }

            var name = elements[0];
            ulong dateInteger;
            if (!ulong.TryParse(elements[1], out dateInteger))
            {
                return null;
            }

            int year = (int)(dateInteger / 10000);
            int month = (int)((dateInteger / 100) % 100);
            int day = (int)(dateInteger % 100);

            if (month < 1 || month > 12 || day < 1 || day > 31)
            {
                return null;
            }

            var date = new DateTime(year, month, day);

            return Tuple.Create(
                elements[0],
                date,
                string.Format(CultureInfo.InvariantCulture, "{0}-{1:yyyy-MM-dd}", elements[0], date));
        }
    }

    public class Blog
    {
        public string BlogName { get; private set; }

        public IReadOnlyList<ArticleMetadata> BlogArticles { get; private set; }

        public DateTime MostRecent
        {
            get
            {
                if (this.BlogArticles.Count == 0)
                {
                    return DateTime.MinValue;
                }

                return this.BlogArticles.Last().Date;
            }
        }

        public static async Task<Blog> FromUserAsync(UserProfile user)
        {
            if (string.IsNullOrWhiteSpace(user.BlogName) ||
                string.IsNullOrWhiteSpace(user.DropboxAccessToken))
            {
                return null;
            }

            using (var client = new DropboxClient(user.DropboxAccessToken, userAgent: "SimpleBlogDemo"))
            {
                return new Blog
                {
                    BlogName = user.BlogName,
                    BlogArticles = new List<ArticleMetadata>(await client.GetArticleList()).AsReadOnly()
                };
            }
        }
    }

    public class ArticleMetadata
    {
        public string Name { get; private set; }

        public string DisplayName { get; private set; }

        public string Rev { get; private set; }

        public DateTime Date { get; private set; }

        public string Filename
        {
            get
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}.{1:yyyyMMdd}.md",
                    this.Name,
                    this.Date);
            }
        }

        public static ArticleMetadata Parse(string filename, string rev)
        {
            var parsed = filename.ParseBlogFileName();
            if (parsed == null)
            {
                return null;
            }

            return new ArticleMetadata
            {
                Name = parsed.Item1,
                Date = parsed.Item2,
                DisplayName = parsed.Item3,
                Rev = rev
            };
        }
    }
 
    public class Article
    {
        public Article(string name, ArticleMetadata metadata, HtmlString content)
        {
            this.Name = name;
            this.Metadata = metadata;
            this.Content = content;
        }

        public string Name { get; private set; }

        public ArticleMetadata Metadata { get; private set; }

        public HtmlString Content { get; private set; }

        public static Article FromMetadata(FileMetadataWithName metadata, HtmlString content)
        {
            var parsed = metadata.Name.ParseBlogFileName();

            return new Article(
                metadata.Name,
                ArticleMetadata.Parse(metadata.Name, metadata.Metadata.Rev),
                content);
        }
    }
}
