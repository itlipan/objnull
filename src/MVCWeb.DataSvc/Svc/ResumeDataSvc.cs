﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVCWeb.Model.Models;
using MVCWeb.DataSvc.Base;

namespace MVCWeb.DataSvc.Svc
{
    //继承基础操作接口获得基础操作
    public interface IResumeDataSvc : IBaseDataSvc<Resume>
    {
    }

    //继承基础操作实现
    public class ResumeDataSvc : BaseDataSvc<Resume>, IResumeDataSvc
    {

    }
}
