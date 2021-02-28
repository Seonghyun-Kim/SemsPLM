using Common.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SemsPLM.Filter
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public GlobalExceptionFilter()
        {

        }

        public virtual void OnException(ExceptionContext filterContext)
        {
            LogFactory.WriteLog(filterContext.Exception);
        }
    }
}