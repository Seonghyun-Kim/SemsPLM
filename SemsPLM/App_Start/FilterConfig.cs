using SemsPLM.Filter;
using System.Web;
using System.Web.Mvc;

namespace SemsPLM
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new GlobalExceptionFilter());
        }
    }
}
