// <copyright file="FlashHelpers.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>

namespace SimpleBusinessDashboard.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Web;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public enum FlashLevel
    {
        Success,
        Info,
        Warning,
        Danger,
    }

    public static class FlashHelpers
    {
        public static void Flash(this Controller controller, string message, FlashLevel level = FlashLevel.Info)
        {
            if (!controller.TempData.TryGetValue("flash", out object flashStackJson))
            {
                flashStackJson = JsonSerializer.Serialize(new List<FlashItem>());
                controller.TempData["flash"] = flashStackJson;
            }

            var flashStack = JsonSerializer.Deserialize<List<FlashItem>>((string)flashStackJson);

            flashStack.Add(new FlashItem
            {
                Message = message,
                Level = level,
            });

            controller.TempData["flash"] = JsonSerializer.Serialize(flashStack);
        }

        public static HtmlString RenderFlash(this IHtmlHelper html)
        {
            List<FlashItem> flashStack = null;

            if (html.ViewContext.TempData.TryGetValue("flash", out object flashStackJson))
            {
                Console.WriteLine("got some flash");
                flashStack = JsonSerializer.Deserialize<List<FlashItem>>((string)flashStackJson);
            }

            if (flashStack == null || flashStack.Count == 0)
            {
                return null;
            }

            var top = flashStack[0];
            if (flashStack.Count > 1)
            {
                flashStack.RemoveAt(0);
                html.ViewContext.TempData["flash"] = JsonSerializer.Serialize(flashStack);
            }

            var level = top.Level.ToString().ToLowerInvariant();
            var message = HttpUtility.HtmlEncode(top.Message).Replace("\r", string.Empty).Replace("\n", "<br />\n").Replace("'", "@squo;");

            var builder = new StringBuilder();
            builder.AppendFormat("<div class=\"margin-top-10 alert alert-{0}\">", level).AppendLine();
            builder.AppendLine("  <span class=\"close\" data-dismiss=\"alert\">&times;</span>");
            builder.AppendLine(message);
            builder.AppendLine("</div>");

            return new HtmlString(builder.ToString());
        }

        private class FlashItem
        {
            public string Message { get; set; }

            public FlashLevel Level { get; set; }
        }
    }
}
