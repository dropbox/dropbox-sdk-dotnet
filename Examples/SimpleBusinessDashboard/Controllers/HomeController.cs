using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Dropbox.Api;
using Dropbox.Api.Team;
using Dropbox.Api.Users;

using SimpleBusinessDashboard.Helpers;

namespace SimpleBusinessDashboard.Controllers
{
    public partial class HomeController : AsyncController
    {
        private string RedirectUri
        {
            get
            {
                if (this.Request.Url.Host.ToLowerInvariant() == "localhost")
                {
                    return "http://localhost:5000/Home/Auth";
                }

                var builder = new UriBuilder(
                    Uri.UriSchemeHttps,
                    this.Request.Url.Host);

                builder.Path = "/Home/Auth";

                return builder.ToString();
            }
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }

        // GET: /Home
        public async Task<ActionResult> IndexAsync()
        {
            DropboxTeamClient teamClient = GetTeamClient();

            /* run asynchronous data retrieval synchronously */
            await SetViewTeamInfo(teamClient);
            await SetViewLinkedApps(teamClient);
            await SetViewActivityData(teamClient);
            await SetViewMemberList(teamClient);

            return View();
        }

        private bool AccountLinked()
        {
            return !string.IsNullOrWhiteSpace(MvcApplication.AccessToken);
        }

        private DropboxTeamClient GetTeamClient()
        {
            return AccountLinked() ? new DropboxTeamClient(MvcApplication.AccessToken) : null;
        }

        private async Task SetViewTeamInfo(DropboxTeamClient client)
        {
            /* sample data */
            string teamName = "PlaceholderTeam";
            uint numMembers = 26;

            if (AccountLinked())
            {
                var teamInfo = await client.Team.GetInfoAsync();
                teamName = teamInfo.Name;
                numMembers = teamInfo.NumProvisionedUsers;
            }

            ViewBag.TeamName = teamName;
            ViewBag.NumMembers = numMembers;
        }

        private async Task SetViewLinkedApps(DropboxTeamClient client)
        {
            /* sample data */
            int totalUserLinkedApps = 8;

            if (AccountLinked())
            {
                var linkedAppsResult = await client.Team.LinkedAppsListTeamLinkedAppsAsync();

                /* retrieve distinct set of linked app names */
                HashSet<string> linkedAppNames = new HashSet<string>();
                foreach (MemberLinkedApps appData in linkedAppsResult.Apps)
                {
                    foreach (var memberLinkedApp in appData.LinkedApiApps)
                    {
                        if (memberLinkedApp.Linked != null)
                        {
                            linkedAppNames.Add(memberLinkedApp.AppName);
                        }
                    }
                }
                totalUserLinkedApps = linkedAppNames.Count;
            }

            ViewBag.TotalUserLinkedApps = totalUserLinkedApps;
        }

        private async Task SetViewActivityData(DropboxTeamClient client)
        {
            /* sample data */
            ulong numDailyActives = 17;
            ulong numWeeklySharedFolders = 4;
            var activeUsers = new List<ulong?> { 6, 10, 4, 5, 8, 2, 9 };

            if (AccountLinked())
            {
                var activityInfo = await client.Team.ReportsGetActivityAsync();

                activeUsers = (List<ulong?>)activityInfo.ActiveUsers1Day;
                /* grab the last element in the array, which contains numDailyActives for today */
                numDailyActives = activeUsers.Last() == null ? 0 : (ulong)activeUsers.Last();

                var sharedFoldersWeekly = (List<ulong?>)activityInfo.ActiveSharedFolders7Day;
                /* grab the last element in the array, which contains numWeeklySharedFolders for this week */
                numWeeklySharedFolders = sharedFoldersWeekly.Last() == null ? 0 : (ulong)sharedFoldersWeekly.Last();
            }

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            ViewBag.NumDailyActives = numDailyActives;
            ViewBag.NumWeeklySharedFolders = numWeeklySharedFolders;
            ViewBag.ActiveUsers = serializer.Serialize(activeUsers);
        }

        private async Task SetViewMemberList(DropboxTeamClient client)
        {
            /* sample data */
            var memListResult = new MembersListResult(GetMemberListSampleData(), "", true);

            if (AccountLinked())
            {
                memListResult = await client.Team.MembersListAsync();
            }

            ViewBag.TeamMembers = memListResult.Members;
        }

        private List<TeamMemberInfo> GetMemberListSampleData()
        {
            /* sample data */
            var teamMemInfoList = new List<TeamMemberInfo>();
            teamMemInfoList.Add(new TeamMemberInfo(new TeamMemberProfile("id#", "john@company.org", true, new TeamMemberStatus(),
                                new Name("", "", "", "John Smith"), TeamMembershipType.Full.Instance, new List<string>()), new AdminTier()));
            teamMemInfoList.Add(new TeamMemberInfo(new TeamMemberProfile("id#", "akhil@company.org", true, new TeamMemberStatus(),
                                new Name("", "", "", "Akhil Goel"), TeamMembershipType.Full.Instance, new List<string>()), new AdminTier()));
            teamMemInfoList.Add(new TeamMemberInfo(new TeamMemberProfile("id#", "lisa@company.org", true, new TeamMemberStatus(),
                                new Name("", "", "", "Lisa Reynolds"), TeamMembershipType.Full.Instance, new List<string>()), new AdminTier()));
            teamMemInfoList.Add(new TeamMemberInfo(new TeamMemberProfile("id#", "emory@company.org", true, new TeamMemberStatus(),
                                new Name("", "", "", "Emory Lee"), TeamMembershipType.Full.Instance, new List<string>()), new AdminTier()));
            teamMemInfoList.Add(new TeamMemberInfo(new TeamMemberProfile("id#", "ian@company.org", true, new TeamMemberStatus(),
                                new Name("", "", "", "Ian Webber"), TeamMembershipType.Full.Instance, new List<string>()), new AdminTier()));

            return teamMemInfoList;
        }

        // GET: /Home/Connect
        public ActionResult Connect()
        {
            if (string.IsNullOrWhiteSpace(MvcApplication.AppKey) ||
                string.IsNullOrWhiteSpace(MvcApplication.AppSecret))
            {
                this.Flash("Please set app key and secret in this project's Web.config file and restart. " +
                           "App key/secret can be found in the Dropbox App Console, here: " +
                           "https://www.dropbox.com/developers/apps", FlashLevel.Danger);

                return RedirectToAction("Index");
            }

            var redirect = DropboxOAuth2Helper.GetAuthorizeUri(
                OAuthResponseType.Code,
                MvcApplication.AppKey,
                RedirectUri);

            return Redirect(redirect.ToString());
        }

        // POST : /Home/Disconnect
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Disconnect()
        {
            MvcApplication.AccessToken = "";
            this.Flash("This account has been disconnected from Dropbox.", FlashLevel.Success);
            return this.RedirectToAction("Index");
        }

        // GET: /Home/Auth
        public async Task<ActionResult> AuthAsync(string code, string state)
        {
            try
            {
                var response = await DropboxOAuth2Helper.ProcessCodeFlowAsync(
                    code,
                    MvcApplication.AppKey,
                    MvcApplication.AppSecret,
                    this.RedirectUri);

                MvcApplication.AccessToken = response.AccessToken;

                this.Flash("This account has been connected to Dropbox.", FlashLevel.Success);
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                var message = string.Format(
                    "code: {0}\nAppKey: {1}\nAppSecret: {2}\nRedirectUri: {3}\nException : {4}",
                    code,
                    MvcApplication.AppKey,
                    MvcApplication.AppSecret,
                    this.RedirectUri,
                    e);
                this.Flash(message, FlashLevel.Danger);
                return RedirectToAction("Index");
            }
        }
    }
}
