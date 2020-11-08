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
                {   //������ ����
                    return false;
                }

                Assembly asm = Assembly.LoadFrom(data.ToString());

                //�ε尡 �Ǿ���?
                if (asm == null)
                {
                    return false;
                }
            }

            ////�ε尡�Ǿ����� 
            ////Dll�� �Ҽӵ� ������� ����Ʈ�� �޾ƿ´�.
            //Type[] types = asm.GetExportedTypes();

            ////types[]�� �������� ���ϴ� ���ӽ����̽��� Ŭ������ ã���� �ִ�.
            ////�� ���������� ���ӽ����̽��� Ŭ������ �Ѱ����̱� ������ �׳� 0�� ������Ʈ�� ����Ѵ�.
            //m_Type = Activator.CreateInstance(types[0]);


            //Dll�ε尡 ���������� ������.
            return true;
        }
    }
}
