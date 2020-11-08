using IBatisNet.Common.Utilities;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace Common.Factory
{
    public class DaoFactory
    {
        private static object syncLock = new object();
        private static ISqlMapper mapper = null;
        private static ISqlMapper Instance
        {
            get
            {
                try
                {
                    if (mapper == null)
                    {
                        lock (syncLock)
                        {
                            if (mapper == null)
                            {
                                string sProvider = ConfigurationManager.AppSettings.Get("Provider");

                                string sSqlMap = string.Empty;

                                switch (sProvider)
                                {
                                    case "MSSQL":
                                        sSqlMap = "Sqlmap_Mssql";
                                        break;
                                    case "ORACLE":
                                        sSqlMap = "Sqlmap_Oracle";
                                        break;
                                    case "POSTGRESQL":
                                        sSqlMap = "Sqlmap_Postgresql";
                                        break;
                                    case "MARIADB":
                                        sSqlMap = "Sqlmap_Maria";
                                        break;
                                    default:
                                        throw new Exception("DB 공급자가 설정되지 않았습니다.");
                                }

                                DomSqlMapBuilder dom = new DomSqlMapBuilder();
                               
                                string assemblyFile = (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
                                FileInfo file = new FileInfo(assemblyFile);

                                string Directory = file.Directory.FullName;

                                FileInfo fInfo = new FileInfo(Directory + string.Format(@"\Config\{0}.config", sSqlMap));

                                mapper = dom.ConfigureAndWatch(fInfo, Configure);

                            }
                        }
                    }

                    return mapper;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        protected static void Configure(object obj)
        {
            mapper = null;
        }


        public static List<T> GetList<T>(string statementName, object parameterObject)
        {
            return Instance.QueryForList<T>(statementName, parameterObject).Cast<T>().ToList();
        }

        public static T GetData<T>(string statementName, object parameterObject)
        {
            return Instance.QueryForObject<T>(statementName, parameterObject);
        }

        public static int SetInsert(string statementName, object parameterObject)
        {
            object oid = Instance.Insert(statementName, parameterObject);
            return oid == null ? 1 : (int)oid;
        }

        public static void SetVoidInsert(string statementName, object parameterObject)
        {
            Instance.Insert(statementName, parameterObject);
        }

        public static int SetUpdate(string statementName, object parameterObject)
        {
            return (int)Instance.Update(statementName, parameterObject);
        }

        public static int SetDelete(string statementName, object parameterObject)
        {
            return (int)Instance.Delete(statementName, parameterObject);
        }

        public static void BeginTransaction()
        {
            Instance.BeginTransaction();
        }

        public static void Commit()
        {
            Instance.CommitTransaction();
        }

        public static void Rollback()
        {
            Instance.RollBackTransaction();
        }
    }
}
