using Common.Constant;
using Common.Factory;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Trigger
{
    public class TriggerUtil
    {

        public static string StatusCheckPromote(HttpSessionStateBase Context, string RelType, string CurrentStatus, int OID, int RootOID, string Action, string Comment)
        {
            string result = "";
            List<Dictionary<string, string>> checkProgram = BPolicyRepository.SelCheckProgram(new BPolicy { Type = RelType, OID = Convert.ToInt32(CurrentStatus) });
            if (checkProgram != null && checkProgram.Count > 0)
            {
                checkProgram.ForEach(item =>
                {
                    if (result == null || result.Length < 1)
                    {
                        string returnMessage = TriggerUtil.Invoke(item[CommonConstant.POLICY_TRIGGER_CLASS], item[CommonConstant.POLICY_TRIGGER_FUNCTION], new object[] { Context, RelType, CurrentStatus, Convert.ToString(OID), Convert.ToString(RootOID), Action, Comment });
                        if (returnMessage != null && returnMessage.Length > 0)
                        {
                            result = returnMessage;
                        }
                    }
                });
            }
            return result;
        }

        public static string StatusPromote(HttpSessionStateBase Context, bool Transaction, string RelType, string CurrentStatus, int OID, int RootOID, string Action, string Comment)
        {
            string result = "";
            try
            {
                if (Transaction)
                {
                    DaoFactory.BeginTransaction();
                }

                string checkProgram = StatusCheckPromote(Context, RelType, CurrentStatus, OID, RootOID, Action, Comment);
                if (checkProgram != null && checkProgram.Length > 0)
                {
                    throw new Exception(checkProgram);
                }

                string strNextAction = BPolicyRepository.SelBPolicy(new BPolicy { Type = RelType, OID = Convert.ToInt32(CurrentStatus) }).First().NextActionOID;
                string strActionOID = "";
                if (strNextAction != null)
                {
                    strNextAction.Split(',').ToList().ForEach(action =>
                    {
                        if (action.IndexOf(Action) > -1)
                        {
                            strActionOID = action.Substring(action.IndexOf(":") + 1);
                            return;
                        }
                    });
                    if (strActionOID.Length > 0)
                    {
                        DObjectRepository.UdtDObject(Context, new DObject { OID = OID, BPolicyOID = Convert.ToInt32(strActionOID) });
                    }
                }

                List<Dictionary<string, string>> actionProgram = BPolicyRepository.SelActionProgram(new BPolicy { Type = RelType, OID = Convert.ToInt32(CurrentStatus) });
                if (actionProgram != null && actionProgram.Count > 0)
                {
                    actionProgram.ForEach(item =>
                    {
                        string returnMessage = TriggerUtil.Invoke(item[CommonConstant.POLICY_TRIGGER_CLASS], item[CommonConstant.POLICY_TRIGGER_FUNCTION], new object[] { Context, RelType, CurrentStatus, Convert.ToString(OID), Convert.ToString(RootOID), Action, Comment });
                        if (returnMessage != null && returnMessage.Length > 0)
                        {
                            throw new Exception(returnMessage);
                        }
                    });
                }

                if (Transaction)
                {
                    DaoFactory.Commit();
                }
            }
            catch (Exception ex)
            {
                if (Transaction)
                {
                    DaoFactory.Rollback();
                }
                result = ex.Message;
            }
            return result;
        }

        public static string StatusObjectPromote(HttpSessionStateBase Context, bool Transaction, string RelType, string CurrentStatus, string GoStatusOID, int OID, int RootOID, string Action, string Comment)
        {
            string result = "";
            try
            {
                if (Transaction)
                {
                    DaoFactory.BeginTransaction();
                }

                string checkProgram = StatusCheckPromote(Context, RelType, CurrentStatus, OID, RootOID, Action, Comment);
                if (checkProgram != null && checkProgram.Length > 0)
                {
                    throw new Exception(checkProgram);
                }

                if (GoStatusOID != null)
                {
                    DObjectRepository.UdtDObject(Context, new DObject { OID = OID, BPolicyOID = Convert.ToInt32(GoStatusOID) });
                }
                else
                {
                    string strNextAction = BPolicyRepository.SelBPolicy(new BPolicy { Type = RelType, OID = Convert.ToInt32(CurrentStatus) }).First().NextActionOID;
                    string strActionOID = "";
                    if (strNextAction != null && strNextAction.Length > 0)
                    {
                        strNextAction.Split(',').ToList().ForEach(action =>
                        {
                            if (action.IndexOf(CommonConstant.ACTION_PROMOTE) > -1)
                            {
                                strActionOID = action.Substring(action.IndexOf(":") + 1);
                                return;
                            }
                        });
                        if (strActionOID.Length > 0)
                        {
                            DObjectRepository.UdtDObject(Context, new DObject { OID = OID, BPolicyOID = Convert.ToInt32(strActionOID) });
                        }                        
                    }
                }

                List<Dictionary<string, string>> actionProgram = BPolicyRepository.SelActionProgram(new BPolicy { Type = RelType, OID = Convert.ToInt32(CurrentStatus) });
                if (actionProgram != null && actionProgram.Count > 0)
                {
                    actionProgram.ForEach(item =>
                    {
                        string returnMessage = TriggerUtil.Invoke(item[CommonConstant.POLICY_TRIGGER_CLASS], item[CommonConstant.POLICY_TRIGGER_FUNCTION], new object[] { Context, RelType, CurrentStatus, Convert.ToString(OID), Convert.ToString(RootOID), Action, Comment, GoStatusOID });
                        if (returnMessage != null && returnMessage.Length > 0)
                        {
                            throw new Exception(returnMessage);
                        }
                    });
                }

                if (Transaction)
                {
                    DaoFactory.Commit();
                }
            }
            catch (Exception ex)
            {
                if (Transaction)
                {
                    DaoFactory.Rollback();
                }
                result = ex.Message;
            }
            return result;
        }


        //_classSpace include(namespace)
        public static string Invoke(string _classSpace, string _methodName, object[] _args)
        {
            object resultObj = null;
            Type type = Type.GetType(_classSpace);
            object obj = Activator.CreateInstance(type);
            MethodInfo[] mis = type.GetMethods();
            bool hasMethod = false;
            for (int i = 0; i < mis.Length; i++)
            {
                MethodInfo mi = mis[i];
                if (mi.Name == _methodName)
                {
                    hasMethod = true;
                    break;
                }
            }
            if (hasMethod)
            {
                object[] args = new object[] { _args };
                resultObj = type.InvokeMember(_methodName, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, obj, args);
            }
            return Convert.ToString(resultObj);
        }

    }
}
