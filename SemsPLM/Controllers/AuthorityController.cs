using Common.Models;
using Common.Utils;
using SemsPLM.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SemsPLM.Controllers
{
    public class AuthorityController : Controller
    {
       
        public ActionResult Login()
        {
            return View("Login/index");
        }

        public ActionResult TryLogin(Person param)
        {
            try
            {
                param.Password = SemsSecureEDecode.Encrypt(param.Password);
                Person person = PersonRepository.LoginSelPerson(param);
                if (person == null || person.OID < 1)
                {
                    throw new Exception("잘못된 접속 정보입니다.");
                }

                string SessionID = Session.SessionID;
                Session["UserOID"] = person.OID;
                Session["LoginID"] = person.ID;
                Session["UserNm"] = person.Name;
                if (person.Thumbnail != null && person.Thumbnail.Length > 0)
                {
                    Session["Thumbnail"] = Url.Content("~/images/Thumbnail/") + person.Thumbnail;
                }
                else
                {
                    Session["Thumbnail"] = "";
                }
                
                Session["SessionID"] = SessionID;

                string IsCookies = System.Configuration.ConfigurationManager.AppSettings["IsCookies"];
                if (IsCookies.ToLower().Equals("true"))
                {
                    string CookiesKey = System.Configuration.ConfigurationManager.AppSettings["CookiesKey"];
                    string CookiesHour = System.Configuration.ConfigurationManager.AppSettings["CookiesHour"];

                    HttpCookie loginCookies = new HttpCookie(CookiesKey);
                    loginCookies.Values["UserOID"] = Convert.ToString(person.OID);
                    loginCookies.Expires = DateTime.Now.AddHours(Convert.ToDouble(CookiesHour));
                    Response.Cookies.Add(loginCookies);
                }
                
                return Json(new { result = "Redirect", url = Url.Action("index", "Home") });
            }
            catch(Exception ex)
            {
                return Json(new ResultJsonModel { isError = true, resultMessage = ex.Message, resultDescription = ex.ToString() });
            }
        }


        [AuthorizeFilter]
        public ActionResult logout()
        {
            if (Session != null)
            {
                string IsCookies = System.Configuration.ConfigurationManager.AppSettings["IsCookies"];
                if (IsCookies.ToLower().Equals("true"))
                {
                    string CookiesKey = System.Configuration.ConfigurationManager.AppSettings["CookiesKey"];
                    if (Request.Cookies.AllKeys.Contains(CookiesKey))
                    {
                        HttpCookie loginCookies = Request.Cookies[CookiesKey];
                        loginCookies.Values["UserOID"] = null;
                        loginCookies.Expires = DateTime.Now.AddHours(-1);
                        Response.Cookies.Add(loginCookies);
                    }
                }
                Session.Clear();
                FormsAuthentication.SignOut();
            }

            return View("Login/Logout");
        }

        public ActionResult TryLogOut()
        {
            return RedirectToAction("logout", "Authority");
        }
    }
}