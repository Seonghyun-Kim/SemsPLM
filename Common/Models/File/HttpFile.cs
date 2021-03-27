using Common.Factory;
using Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.Models.File
{
    interface IHttpFile
    {
        int? FileOID { get; set; }

        int? OID { get; set; }

        string Type { get; set; }

        // FileName을 컨트롤 하고 싶었으나 FileName이 Get만 허용하여 OrgNm을 따로 만듬.
        string OrgNm { get; set; }

        string ConvNm { get; set; }

        string Ext { get; set; }

        int FileSize { get; set; }

        int? CreateUs { get; set; }

        string CreateUsNm { get; set; }

        DateTime? CreateDt { get; set; }
    }

    public interface IObjectFile
    {
        int? OID { get; set; }

        string Type { get; set; }

        List<HttpPostedFileBase> Files { get; set; }

        List<HttpFile> delFiles { get; set; }
    }
    
    public class HttpFile : IHttpFile
    {
        public int? FileOID { get; set; }

        public int? OID { get; set; }

        public string Type { get; set; }

        // FileName을 컨트롤 하고 싶었으나 FileName이 Get만 허용하여 OrgNm을 따로 만듬.
        public string OrgNm { get; set; }

        public string ConvNm { get; set; }

        public string Ext { get; set; }

        public int FileSize { get; set; }

        public int? CreateUs { get; set; }

        public string CreateUsNm { get; set; }

        public DateTime? CreateDt { get; set; }

        public int? DeleteUs { get; set; }

        public string DeleteUsNm { get; set; }

        public DateTime? DeleteDt { get; set; }
    }

    public class HttpFileRepository
    {
        public static bool InsertData(HttpSessionStateBase Context, IObjectFile @object)
        {
            try
            {
                if (@object == null)
                {
                    return true;
                }

                @object.Files.ForEach(item =>
                {
                    HttpFile file = null;
                    try
                    {
                        file = SemsValut.SaveFile(@object, item);
                        file.CreateUs = Convert.ToInt32(Context["UserOID"]); //HttpContext.Current.Session["UserOID"].ToString();
                        DaoFactory.SetInsert("Comm.InsFile", file);
                    }
                    catch (Exception ex)
                    {
                        if (item != null)
                        {
                            SemsValut.FileDelete(file);
                        }

                        throw ex;
                    }

                });

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int DeleteData(HttpSessionStateBase Context, HttpFile httpFile)
        {
            if (httpFile.FileOID == null) { throw new Exception("파일을 삭제할 수 없습니다."); }
            int retValue = 0;
            httpFile.DeleteUs = Convert.ToInt32(Context["UserOID"]);// HttpContext.Current.Session["UserOID"].ToString();

            HttpFile _file = SelFile(Context, httpFile);
            SemsValut.FileDelete(_file);

            DaoFactory.SetUpdate("Comm.DelFile", httpFile);

            return retValue;
        }

        public static HttpFile SelFile(HttpSessionStateBase Context, HttpFile httpFile)
        {
            return DaoFactory.GetData<HttpFile>("Comm.SelFile", httpFile);
        }

        public static List<HttpFile> SelFiles(HttpSessionStateBase Context, HttpFile httpFile)
        {
            if (httpFile == null || httpFile.OID == null) return null;
            return DaoFactory.GetList<HttpFile>("Comm.SelFile", httpFile);
        }

        public static HttpFile ReviseFiles(HttpSessionStateBase Context, HttpFile httpFile, int? NewOID)
        {
            if (httpFile == null || httpFile.OID == null) return null;

            List<HttpFile> SelReviseFiles = HttpFileRepository.SelFiles(Context, httpFile);

            if (SelReviseFiles.Count > 0 && SelReviseFiles != null)
            {
                SelReviseFiles.ForEach(v =>
                {
                    v.OID = NewOID;
                    v.CreateUs = Convert.ToInt32(Context["UserOID"]);
                    DaoFactory.SetInsert("Comm.InsFile", v);
                });
            }
            return httpFile;
        }
    }
}
