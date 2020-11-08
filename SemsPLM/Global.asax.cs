using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SemsPLM
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DynamicLoadDll();
        }

        static bool DynamicLoadDll()
        {
            string assemblyFile = (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;

            FileInfo fi = new FileInfo(assemblyFile);

            string assemblyPath = fi.Directory.FullName;

            ArrayList arrDllList = new ArrayList();

            arrDllList.Add(Path.Combine(assemblyPath + @"\Common.dll"));
            arrDllList.Add(Path.Combine(assemblyPath + @"\Document.dll"));
            arrDllList.Add(Path.Combine(assemblyPath + @"\EBom.dll"));
            arrDllList.Add(Path.Combine(assemblyPath + @"\Pms.dll"));
            arrDllList.Add(Path.Combine(assemblyPath + @"\ChangeOrder.dll"));
            arrDllList.Add(Path.Combine(assemblyPath + @"\ChangeRequest.dll"));

            foreach (var data in arrDllList)
            {
                if (!File.Exists(data.ToString()))
                {   //없으면 에러
                    return false;
                }

                Assembly asm = Assembly.LoadFrom(data.ToString());

                //로드가 되었나?
                if (asm == null)
                {
                    return false;
                }
            }

            ////로드가되었으면 
            ////Dll에 소속된 구성요소 리스트를 받아온다.
            //Type[] types = asm.GetExportedTypes();

            ////types[]의 내용으로 원하는 네임스페이스나 클래스를 찾을수 있다.
            ////이 예제에서는 네임스페이스와 클래스가 한개뿐이기 때문에 그냥 0번 오브젝트를 사용한다.
            //m_Type = Activator.CreateInstance(types[0]);


            //Dll로드가 정상적으로 끝났다.
            return true;
        }
    }
}
