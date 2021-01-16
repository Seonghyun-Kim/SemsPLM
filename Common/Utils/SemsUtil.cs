using Common.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SemsUtil
    {

        public static string MakeSeq(int seq, string format)
        {
            return seq.ToString(format);
        }

        public static Boolean IsBatter(string baseRev, string targetRev)
        {
            Boolean result = true;
            int tmpBaseRev = CurrentLettersToNumber(baseRev.Replace(Convert.ToChar(CommonConstant.REVISION_PREFIX), ' ').Trim());
            int tmpTargetRev = CurrentLettersToNumber(targetRev.Replace(Convert.ToChar(CommonConstant.REVISION_PREFIX), ' ').Trim());
            if (tmpBaseRev > tmpTargetRev)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public static string MakeMajorRevisonUp(string str)
        {
            if (String.IsNullOrEmpty(str)) return CommonConstant.REVISION_PREFIX + CommonConstant.INIT_REVISION;
            string sR = "";
            if (str.IndexOf('.') > 0)
            {
                string[] ss = str.Split('.');
                sR = ss[0].Replace(Convert.ToChar(CommonConstant.REVISION_PREFIX), ' ').Trim();
            }
            else
            {
                sR = str;
            }
            string newR = CommonConstant.REVISION_PREFIX + NumberToletters((CurrentLettersToNumber(sR) + 1));
            return newR;
        }

        public static string MakeMajorRevisionDown(string str)
        {
            if (String.IsNullOrEmpty(str) || str.Equals(CommonConstant.REVISION_PREFIX + CommonConstant.INIT_REVISION)) return CommonConstant.REVISION_PREFIX + CommonConstant.INIT_REVISION;
            string sR = "";
            if (str.IndexOf('.') > 0)
            {
                string[] ss = str.Split('.');
                sR = ss[0].Replace(Convert.ToChar(CommonConstant.REVISION_PREFIX), ' ').Trim();
            }
            else
            {
                sR = str;
            }
            string newR = CommonConstant.REVISION_PREFIX + NumberToletters((CurrentLettersToNumber(sR) - 1));
            return newR;
        }

        public static string NumberToletters(int value, int length = 2)
        {
            //string map = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string map = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ";

            char[] result = new char[length];
            var x = value;
            for (int i = 0; i < length; i++)
            {
                int threshold = (int)Math.Pow(Convert.ToDouble(map.Length), length - i - 1);
                var index = Math.Min(map.Length - 1, x / threshold);
                result[i] = map[index];
                x -= threshold * index;
            }
            return new string(result);
        }

        public static int CurrentLettersToNumber(string str)
        {
            int result = 0;
            for (int i = 0; i < CommonConstant.MAX_NUMBER; i++)
            {
                if (str.Equals(NumberToletters(i)))
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        public static string SingleMakeMajorRevisonUp(string str)
        {
            if (String.IsNullOrEmpty(str)) return CommonConstant.REVISION_PREFIX + CommonConstant.INIT_REVISION;
            string sR = "";
            if (str.IndexOf('.') > 0)
            {
                string[] ss = str.Split('.');
                sR = ss[0].Replace(Convert.ToChar(CommonConstant.REVISION_PREFIX), ' ').Trim();
            }
            else
            {
                sR = str;
            }
            string newR = SingleNumberToletters((CurrentLettersToNumber(sR) + 1));
            return newR;
        }

        public static string SingleNumberToletters(int value, int length = 1)
        {
            //string map = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string map = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ";

            char[] result = new char[length];
            var x = value;
            for (int i = 0; i < length; i++)
            {
                int threshold = (int)Math.Pow(Convert.ToDouble(map.Length), length - i - 1);
                var index = Math.Min(map.Length - 1, x / threshold);
                result[i] = map[index];
                x -= threshold * index;
            }
            return new string(result);
        }
    }

    public class StringValue : System.Attribute
    {
        private string _value;
        public StringValue(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
    }

    public static class StringEnum
    {
        public static string GetStringValue(ValueType e)
        {
            if (e == null)
            {
                return "";
            }
            string output = null;

            Type type = e.GetType();

            FieldInfo fi = type.GetField(e.ToString());
            if (fi == null)
            {
                return "";
            }
            StringValue[] attrs = fi.GetCustomAttributes(typeof(StringValue), false) as StringValue[];

            if (attrs.Length > 0)
            {
                output = attrs[0].Value;
            }

            return output;
        }
    }
}
