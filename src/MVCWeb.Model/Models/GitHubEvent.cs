using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCWeb.Model.Models
{
    public class GitHubEvent
    {
        /// <summary>
        /// 标识
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Github登录名
        /// </summary>
        public string GitHubLogin { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 添加日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 事件类型
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// 项目
        /// </summary>
        public string RepoName { get; set; }

        /// <summary>
        /// 抓取机器人
        /// </summary>
        public string Robot { get; set; }

        /// <summary>
        /// 项目描述
        /// </summary>
        public string RepoDescription { get; set; }
        
    }
}
