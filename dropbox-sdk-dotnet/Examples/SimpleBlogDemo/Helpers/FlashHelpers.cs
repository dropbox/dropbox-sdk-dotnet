using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SimpleBlogDemo.Helpers
{
    public enum FlashLevel
    {
        Success,
        Info,
        Warning,
        Danger
    }

    public static class FlashHelpers
    {
        public static void Flash(this ControllerBase controller, string message, FlashLevel level = FlashLevel.Info)
        {
            object flashStackObject;
            List<FlashItem> flashStack;

            if (!controller.TempData.TryGetValue("flash", out flashStackObject))
            {
                flashStackObject = new List<FlashItem>();
                controller.TempData["flash"] = flashStackObject;
            }

            flashStack = flashStackObject as List<FlashItem>;

            flashStack.Add(new FlashItem {
                Message = message,
                Level = level
            });
        }

        public static HtmlString RenderFlash(this HtmlHelper html)
        {
            object flashStackObject;
            List<FlashItem> flashStack;

            html.ViewContext.TempData.TryGetValue("flash", out flashStackObject);

            flashStack = flashStackObject as List<FlashItem>;
            if (flashStack == null || flashStack.Count == 0)
            {
                return null;
            }

            var top = flashStack[0];
            if (flashStack.Count > 1)
            {
                flashStack.RemoveAt(0);
                html.ViewContext.TempData["flash"] = flashStack;
            }

            var level = top.Level.ToString().ToLowerInvariant();
            var message = HttpUtility.HtmlEncode(top.Message).Replace("\r", "").Replace("\n", "<br />\n").Replace("'", "@squo;");
            
            var builder = new StringBuilder();
            builder.AppendFormat("<div class=\"margin-top-10 alert alert-{0}\">", level).AppendLine();
            builder.AppendLine("  <span class=\"close\" data-dismiss=\"alert\">&times;</span>");
            builder.AppendLine(message);
            builder.AppendLine("</div>");

            return new HtmlString(builder.ToString());
        }

        class FlashItem
        {
            public string Message { get; set; }
            public FlashLevel Level { get; set; }
        }
    }
}
