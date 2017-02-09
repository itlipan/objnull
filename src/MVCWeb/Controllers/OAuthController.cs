using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Configuration;
using Newtonsoft.Json;
using Octokit;
using MVCWeb.DataSvc.Svc;
using MVCWeb.Model.Models;
using MVCWeb.Redis.Base;
using MVCWeb.Redis.Models;

namespace MVCWeb.Controllers
{
    public class OAuthController : BaseController
    {
        public INullUserDataSvc NullUserDataSvc { get; set; }
        public INewBeeDataSvc NewBeeDataSvc { get; set; }
        public INewBeeFloorDataSvc NewBeeFloorDataSvc { get; set; }
        public IMyRedisDB MyRedisDB { get; set; }

        //用户信息
        public ActionResult UserInfo()
        {
            ViewBag.ClientID = ConfigurationManager.AppSettings["client_id"];
            //开发用OAuth跳转至changetolocalhost.com，修改changetolocalhost.com为localhost:2221完成本地OAuth登录
#if Debug
            ViewBag.Domain = "changetolocalhost.com";
#endif
#if Release
            ViewBag.Domain = "objnull.com";
#endif
            if (CurrentUser != null)
            {
                ViewBag.User = CurrentUser;
                ViewBag.MsgCount = MyRedisDB.RedisDB.SetLength(MyRedisKeys.Pre_CMsg + CurrentUser.ID)
                    + MyRedisDB.RedisDB.SetLength(MyRedisKeys.Pre_RMsg + CurrentUser.ID) + MyRedisDB.RedisDB.SetLength(MyRedisKeys.Pre_SysMsg + CurrentUser.ID);
                if (string.IsNullOrEmpty(HttpContext.ReadCookie("LastLogin")))
                {
                    NullUser user = NullUserDataSvc.GetByID(CurrentUser.ID);
                    user.LastLoginDate = DateTime.Now;
                    user.LastLoginIP = HttpContext.Request.UserHostAddress;
                    NullUserDataSvc.Update(user);
                    HttpContext.WriteCookie("LastLogin", DateTime.Now.ToString(), DateTime.Now.AddDays(1).Date);
                }
            }
            return PartialView();
        }

        [HttpPost]
        //更新账号
        public ActionResult UpdateInfo()
        {
            NullUser user = NullUserDataSvc.GetByID(CurrentUser.ID);
            if (UpdateUserInfo("github", user.GitHubAccessToken))
            {
                return Json(new { msg = "done" });
            }
            else
            {
                return Json(new { msg = "error" });
            }
        }

        //退出
        public ActionResult LogOut()
        {
            HttpContext.WriteCookie("UID", "", DateTime.Now.AddDays(-1));
            HttpContext.WriteCookie("UName", "", DateTime.Now.AddDays(-1));
            HttpContext.WriteCookie("UAvatar", "", DateTime.Now.AddDays(-1));
            HttpContext.WriteCookie("LoginType", "", DateTime.Now.AddDays(-1));
            HttpContext.WriteCookie("GLogin", "", DateTime.Now.AddDays(-1));
            HttpContext.WriteCookie("SKEY", "", DateTime.Now.AddDays(-1));
            HttpContext.WriteCookie("LastLogin", "", DateTime.Now.AddDays(-1));
            HttpContext.WriteCookie("Role", "", DateTime.Now.AddDays(-1));
            return RedirectToAction("Index", "Home");
        }

        //添加用户或更新用户信息
        public bool UpdateUserInfo(string loginType, string token)
        {
            if (loginType == "github")
            {
                GitHubUser githubUser = GitHub.GetGitHubUser(token);
                if (githubUser.id == 0)
                {
                    return false;
                }
                NullUser user = NullUserDataSvc.GetByCondition(u => u.GitHubID == githubUser.id).FirstOrDefault();
                if (user == null)
                {
                    user = new NullUser();
                    user.LoginType = "github";
                    user.GitHubAccessToken = token;
                    user.Name = githubUser.name;
                    user.AvatarUrl = githubUser.avatar_url;
                    user.GitHubLogin = githubUser.login;
                    user.GitHubID = githubUser.id;
                    user.Role = (int)EnumUserRole.普通;
                    user.Email = githubUser.email;
                    user.FollowingCount = githubUser.following;
                    user.FollowerCount = githubUser.followers;
                    NullUserDataSvc.Add(user);

                    //用关注人数小于500的机器人账号关注普通用户
                    if (user.Email == null || (user.Email != null && !user.Email.EndsWith("objnull.com")))
                    {
                        NullUser robot = NullUserDataSvc.GetByCondition(u => u.Email.EndsWith("objnull.com") && u.GitHubLogin.Contains("robot") && u.FollowingCount < 500).First();
                        GitHubClient github = new GitHubClient(new ProductHeaderValue("objnulldotcom"));
                        github.Credentials = new Credentials(robot.GitHubAccessToken);
                        github.User.Get(robot.GitHubLogin);
                        github.User.Followers.Follow(user.GitHubLogin);
                        robot.FollowingCount += 1;
                        NullUserDataSvc.Update(robot);
                    }

                    if (string.IsNullOrEmpty(user.Email))
                    {
                        SysMsg msg = new SysMsg();
                        msg.Date = DateTime.Now;
                        msg.Title = "您还未设置邮箱";
                        msg.Msg = "请到<a href=\"/Home/UserProfile\">我的主页</a>设置或修改<a href=\"https://github.com/settings/profile\" target=\"_blank\">GitHub</a>邮箱显示后更新账号。";
                        string key = MyRedisKeys.Pre_SysMsg + user.ID;
                        MyRedisDB.SetAdd(key, msg);
                    }
                }
                else
                {
                    user.Name = githubUser.name;
                    user.AvatarUrl = githubUser.avatar_url;
                    user.GitHubID = githubUser.id;
                    user.GitHubLogin = githubUser.login;
                    user.GitHubAccessToken = token;
                    user.Email = githubUser.email;
                    user.FollowingCount = githubUser.following;
                    user.FollowerCount = githubUser.followers;
                    NullUserDataSvc.Update(user);
                }

                HttpContext.WriteCookie("UID", user.ID.ToString(), DateTime.Now.AddYears(3));
                HttpContext.WriteCookie("UName", HttpUtility.UrlEncode(githubUser.name), DateTime.Now.AddYears(3));
                HttpContext.WriteCookie("UAvatar", githubUser.avatar_url, DateTime.Now.AddYears(3));
                HttpContext.WriteCookie("LoginType", user.LoginType, DateTime.Now.AddYears(3));
                HttpContext.WriteCookie("GLogin", githubUser.login, DateTime.Now.AddYears(3));
                HttpContext.WriteCookie("SKEY", Utils.RijndaelEncrypt(user.ID.ToString()), DateTime.Now.AddYears(3));
                HttpContext.WriteCookie("Role", Utils.RijndaelEncrypt(user.ID.ToString() + ";" + user.Role.ToString()), DateTime.Now.AddYears(3));
            }
            return true;
        }

        #region GitHub

        public ActionResult GitHubLogin(string code, string state)
        {
            //获取token
            GitHubAccessToken token;
            using (HttpClient hc = new HttpClient())
            {
                FormUrlEncodedContent postData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", ConfigurationManager.AppSettings["client_id"]),
                    new KeyValuePair<string, string>("client_secret", ConfigurationManager.AppSettings["client_secret"]),
                    new KeyValuePair<string, string>("code", code)
                });
                hc.DefaultRequestHeaders.Add("Accept", "application/json");
                HttpResponseMessage response = hc.PostAsync("https://github.com/login/oauth/access_token", postData).Result;
                token = JsonConvert.DeserializeObject<GitHubAccessToken>(response.Content.ReadAsStringAsync().Result);
            }
            if (token.access_token == null)
            {
                throw new Exception("获取token失败：code=" + code + " state=" + state);
            }
            //添加用户或更新用户信息
            UpdateUserInfo("github", token.access_token);

            return RedirectToAction("NewBeeList", "Home");
        }

        #endregion
    }
}