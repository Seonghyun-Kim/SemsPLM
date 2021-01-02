using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SemsPLM.Filter
{
    public class AuthorizeFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["UserOID"] == null)
            {
                string IsCookies = System.Configuration.ConfigurationManager.AppSettings["IsCookies"];
                if (IsCookies.ToLower().Equals("true"))
                {
                    string CookiesKey = System.Configuration.ConfigurationManager.AppSettings["CookiesKey"];
                    if (filterContext.HttpContext.Request.Cookies[CookiesKey] != null)
                    {
                        if (filterContext.HttpContext.Request.Cookies[CookiesKey].Values["UserOID"] == null)
                        {
                            filterContext.Result = new RedirectResult("/Authority/Login");
                            return;
                        }

                        Person person = PersonRepository.LoginSelPerson(new Person { OID = Convert.ToInt32(filterContext.HttpContext.Request.Cookies[CookiesKey].Values["UserOID"]) });
                        if (person == null)
                        {
                            filterContext.Result = new RedirectResult("/Authority/Login");
                            return;
                        }
                        
                        string SessionID = filterContext.HttpContext.Session.SessionID;
                        filterContext.HttpContext.Session["UserOID"] = person.OID;
                        filterContext.HttpContext.Session["LoginID"] = person.ID;
                        filterContext.HttpContext.Session["UserNm"] = person.Name;
                        filterContext.HttpContext.Session["SessionID"] = SessionID;
                        return;
                    }
                }
                filterContext.Result = new RedirectResult("/Authority/Login");
                return;
            }

        }
    }
}