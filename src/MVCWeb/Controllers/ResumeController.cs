using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCWeb.Model.Models;
using MVCWeb.DataSvc.Svc;
using Ganss.XSS;

namespace MVCWeb.Controllers
{
    public class ResumeController : BaseController
    {
        public IResumeDataSvc ResumeDataSvc { get; set; }
        public INullUserDataSvc NullUserDataSvc { get; set; }

        public HtmlSanitizer HtmlST = new HtmlSanitizer();

        //查看
        public ActionResult See(string githubID)
        {
            string gid = "";
            if(string.IsNullOrEmpty(githubID))
            {
                gid = CurrentUser == null ? "" : CurrentUser.GitHubLogin;
            }
            else
            {
                gid = githubID;
            }

            Resume resume = NullUserDataSvc.GetByCondition(n => n.GitHubLogin == gid).FirstOrDefault().Resumes.FirstOrDefault();
            ViewBag.Resume = resume;
            ViewBag.GID = gid;
            ViewBag.Edit = false;
            ViewBag.See = false;
            //公开的简历才能查看
            if (resume != null && resume.Public)
            {
                ViewBag.See = true;
            }
            //简历本人可以编辑查看
            if (CurrentUser != null && CurrentUser.GitHubLogin == gid)
            {
                ViewBag.Edit = true;
                ViewBag.See = true;
            }
            return View();
        }

        //编辑
        public ActionResult Edit()
        {
            Resume resume = ResumeDataSvc.GetByCondition(r => r.UserID == CurrentUser.ID).FirstOrDefault();
            ViewBag.Resume = resume;
            return View();
        }

        //保存
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditSave(string md, string html, bool isPub)
        {
            Resume resume = ResumeDataSvc.GetByCondition(r => r.UserID == CurrentUser.ID).FirstOrDefault();
            if(resume == null)
            {
                resume = new Resume();
                resume.UserID = CurrentUser.ID;
                resume.MD = md;
                resume.Html = HtmlST.Sanitize(html);
                resume.Public = isPub;
                ResumeDataSvc.Add(resume);
            }
            else
            {
                resume.MD = md;
                resume.Html = HtmlST.Sanitize(html);
                resume.Public = isPub;
                ResumeDataSvc.Update(resume);
            }
            return Json(new { msg = "done", gid = CurrentUser.GitHubLogin });
        }
    }
}