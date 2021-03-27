using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.Utils
{
    public static class SemsConnect
    {
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        //public static extern int SendARP(int DestIP, int SrcIP, [Out] byte[] pMacAddr, ref int PhyAddrLen);
        private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);

        [DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);

        public static string GetMacAddress(string strClientIP)
        {
            string mac_dest = "";
            try
            {
                Int32 ldest = inet_addr(strClientIP);
                Int32 lhost = inet_addr("");
                Int64 macinfo = new Int64();
                Int32 len = 6;
                int res = SendARP(ldest, 0, ref macinfo, ref len);
                string mac_src = macinfo.ToString("X");

                while (mac_src.Length < 12)
                {
                    mac_src = mac_src.Insert(0, "0");
                }

                for (int i = 0; i < 11; i++)
                {
                    if (0 == (i % 2))
                    {
                        if (i == 10)
                        {
                            mac_dest = mac_dest.Insert(0, mac_src.Substring(i, 2));
                        }
                        else
                        {
                            mac_dest = "-" + mac_dest.Insert(0, mac_src.Substring(i, 2));
                        }
                    }
                }
            }
            catch (Exception err)
            {
                throw err;
            }
            return mac_dest;
        }

        #region -- IP 조회 영역
        public static string GetRemoteIP(HttpRequestBase Request)
        {
            //ManagementObjectSearcher query = null;
            //ManagementObjectCollection queryCollection = null;

            //query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
            //queryCollection = query.Get();
            //foreach (ManagementObject mo in queryCollection)
            //{
            //    if (mo["MacAddress"] != null)
            //    {
            //        string Mac = mo["MacAddress"].ToString();                    
            //    }
            //}

            //string macAddresses = "";

            //foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            //{
            //    if (nic.OperationalStatus == OperationalStatus.Up)
            //    {
            //        macAddresses += nic.GetPhysicalAddress().ToString();
            //        break;
            //    }
            //}

            string ipList = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            string RemoteIP = Request.ServerVariables["REMOTE_ADDR"]; //Proxy/Load Balancer IP or original IP if no proxy was used

            if (!string.IsNullOrEmpty(ipList))
            {
                return ipList.Split(',')[0];
            }

            return Request.ServerVariables["REMOTE_ADDR"];
        }

        public static string GetRemoteIP(HttpRequest Request)
        {
            string ipList = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            string RemoteIP = Request.ServerVariables["REMOTE_ADDR"]; //Proxy/Load Balancer IP or original IP if no proxy was used


            if (!string.IsNullOrEmpty(ipList))
            {
                return ipList.Split(',')[0];
            }

            return Request.ServerVariables["REMOTE_ADDR"];
        }

        #endregion

    }
}
