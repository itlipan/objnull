using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCWeb.DataSvc.Svc;
using MVCWeb.Model.Models;

namespace MVCWeb.Controllers
{
    public class RobotController : BaseController
    {
        public IGitHubEventDataSvc GitHubEventDataSvc { get; set; }

        // GET: Robot
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GitHubEventPage(int pageSize, int pageNum = 1)
        {
            int totalCount = 0;
            List<GitHubEvent> eventList = GitHubEventDataSvc.GetPagedEntitys(ref pageNum, pageSize, it => true, it => it.CreateDate, true, out totalCount).ToList();

            ViewBag.EventList = eventList;
            ViewBag.TotalCount = totalCount;
            ViewBag.CurrentPage = pageNum;
            ViewBag.ShowPager = totalCount > pageSize;
            
            return View();
        }
    }
}