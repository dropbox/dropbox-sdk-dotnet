// <copyright file="HomeController.cs" company="Dropbox Inc">
// Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>

namespace SimpleBusinessDashboard.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Dropbox.Api;
    using Dropbox.Api.Team;
    using Dropbox.Api.Users;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using SimpleBusinessDashboard.Helpers;

    public partial class HomeController : Controller
    {
        private IConfiguration configuration;

        public HomeController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private string RedirectUri
        {
            get
            {
                if (this.Request.Host.Host.ToLowerInvariant() == "localhost")
                {
                    return string.Format("http://{0}:{1}/Home/Auth", this.Request.Host.Host, this.Request.Host.Port);
                }

                var builder = new UriBuilder(
                    Uri.UriSchemeHttps,
                    this.Request.Host.ToString());

                builder.Path = "/Home/Auth";

                return builder.ToString();
            }
        }

        // GET: /Home
        public async Task<ActionResult> IndexAsync()
        {
            DropboxTeamClient teamClient = this.GetTeamClient();

            /* run asynchronous data retrieval synchronously */
            await this.SetViewTeamInfo(teamClient);
            await this.SetViewLinkedApps(teamClient);
            await this.SetViewActivityData(teamClient);
            await this.SetViewMemberList(teamClient);

            return this.View();
        }

        // GET: /Home/Connect
        public ActionResult Connect()
        {
            if (string.IsNullOrWhiteSpace(this.configuration["AppKey"]) ||
                string.IsNullOrWhiteSpace(this.configuration["AppSecret"]))
            {
                this.Flash(
                    "Please set app key and secret in this project's Web.config file and restart. " +
                           "App key/secret can be found in the Dropbox App Console, here: " +
                           "https://www.dropbox.com/developers/apps", FlashLevel.Danger);

                return this.RedirectToAction("Index");
            }

            var redirect = DropboxOAuth2Helper.GetAuthorizeUri(
                OAuthResponseType.Code,
                this.configuration["AppKey"],
                this.RedirectUri);

            return this.Redirect(redirect.ToString());
        }

        // POST : /Home/Disconnect
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disconnect()
        {
            this.configuration["AccessToken"] = string.Empty;
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
                    this.configuration["AppKey"],
                    this.configuration["AppSecret"],
                    this.RedirectUri);

                this.configuration["AccessToken"] = response.AccessToken;

                this.Flash("This account has been connected to Dropbox.", FlashLevel.Success);
                return this.RedirectToAction("Index");
            }
            catch (Exception e)
            {
                var message = string.Format(
                    "code: {0}\nAppKey: {1}\nAppSecret: {2}\nRedirectUri: {3}\nException : {4}",
                    code,
                    this.configuration["AppKey"],
                    this.configuration["AppSecret"],
                    this.RedirectUri,
                    e);
                this.Flash(message, FlashLevel.Danger);
                return this.RedirectToAction("Index");
            }
        }

        private bool AccountLinked()
        {
            return !string.IsNullOrWhiteSpace(this.configuration["AccessToken"]);
        }

        private DropboxTeamClient GetTeamClient()
        {
            return this.AccountLinked() ? new DropboxTeamClient(this.configuration["AccessToken"]) : null;
        }

        private async Task SetViewTeamInfo(DropboxTeamClient client)
        {
            /* sample data */
            string teamName = "PlaceholderTeam";
            uint numMembers = 26;

            if (this.AccountLinked())
            {
                var teamInfo = await client.Team.GetInfoAsync();
                teamName = teamInfo.Name;
                numMembers = teamInfo.NumProvisionedUsers;
            }

            this.ViewBag.TeamName = teamName;
            this.ViewBag.NumMembers = numMembers;
        }

        private async Task SetViewLinkedApps(DropboxTeamClient client)
        {
            /* sample data */
            int totalUserLinkedApps = 8;

            if (this.AccountLinked())
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

            this.ViewBag.TotalUserLinkedApps = totalUserLinkedApps;
        }

        private async Task SetViewActivityData(DropboxTeamClient client)
        {
            /* sample data */
            ulong numDailyActives = 17;
            ulong numWeeklySharedFolders = 4;
            var activeUsers = new List<ulong?> { 6, 10, 4, 5, 8, 2, 9 };

            if (this.AccountLinked())
            {
                var activityInfo = await client.Team.ReportsGetActivityAsync();

                activeUsers = (List<ulong?>)activityInfo.ActiveUsers1Day;
                /* grab the last element in the array, which contains numDailyActives for today */
                numDailyActives = activeUsers.Last() == null ? 0 : (ulong)activeUsers.Last();

                var sharedFoldersWeekly = (List<ulong?>)activityInfo.ActiveSharedFolders7Day;
                /* grab the last element in the array, which contains numWeeklySharedFolders for this week */
                numWeeklySharedFolders = sharedFoldersWeekly.Last() == null ? 0 : (ulong)sharedFoldersWeekly.Last();
            }

            this.ViewBag.NumDailyActives = numDailyActives;
            this.ViewBag.NumWeeklySharedFolders = numWeeklySharedFolders;
            this.ViewBag.ActiveUsers = JsonSerializer.Serialize(activeUsers);
        }

        private async Task SetViewMemberList(DropboxTeamClient client)
        {
            /* sample data */
            var memListResult = new MembersListResult(this.GetMemberListSampleData(), string.Empty, true);

            if (this.AccountLinked())
            {
                memListResult = await client.Team.MembersListAsync();
            }

            this.ViewBag.TeamMembers = memListResult.Members;
        }

        private List<TeamMemberInfo> GetMemberListSampleData()
        {
            /* sample data */
            var teamMemInfoList = new List<TeamMemberInfo>();
            teamMemInfoList.Add(new TeamMemberInfo(
                new TeamMemberProfile(
                    "id#",
                    "john@company.org",
                    true,
                    new TeamMemberStatus(),
                    new Name(string.Empty, string.Empty, string.Empty, "John Smith", string.Empty),
                    TeamMembershipType.Full.Instance,
                    new List<string>(),
                    "123"),
                new AdminTier()));
            teamMemInfoList.Add(new TeamMemberInfo(
                new TeamMemberProfile(
                    "id#",
                    "akhil@company.org",
                    true,
                    new TeamMemberStatus(),
                    new Name(string.Empty, string.Empty, string.Empty, "Akhil Goel", string.Empty),
                    TeamMembershipType.Full.Instance,
                    new List<string>(),
                    "123"),
                new AdminTier()));
            teamMemInfoList.Add(new TeamMemberInfo(
                new TeamMemberProfile(
                    "id#",
                    "lisa@company.org",
                    true,
                    new TeamMemberStatus(),
                    new Name(string.Empty, string.Empty, string.Empty, "Lisa Reynolds", string.Empty),
                    TeamMembershipType.Full.Instance,
                    new List<string>(),
                    "123"),
                new AdminTier()));
            teamMemInfoList.Add(new TeamMemberInfo(
                new TeamMemberProfile(
                    "id#",
                    "emory@company.org",
                    true,
                    new TeamMemberStatus(),
                    new Name(string.Empty, string.Empty, string.Empty, "Emory Lee", string.Empty),
                    TeamMembershipType.Full.Instance,
                    new List<string>(),
                    "123"),
                new AdminTier()));
            teamMemInfoList.Add(new TeamMemberInfo(
                new TeamMemberProfile(
                    "id#",
                    "ian@company.org",
                    true,
                    new TeamMemberStatus(),
                    new Name(string.Empty, string.Empty, string.Empty, "Ian Webber", string.Empty),
                    TeamMembershipType.Full.Instance,
                    new List<string>(),
                    "123"),
                new AdminTier()));

            return teamMemInfoList;
        }
    }
}
