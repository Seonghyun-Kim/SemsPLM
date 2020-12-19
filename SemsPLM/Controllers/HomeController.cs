using Common;
using IBatisNet.DataMapper;
using SemsPLM.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SemsPLM.Controllers
{
    [AuthorizeFilter]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult CallMenuView(MenuModel _param)
        {
            try
            {
                if(_param.MenuUrl == null || _param.MenuUrl.Length < 1)
                {
                    return null;
                }
                return Redirect(_param.MenuUrl);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}