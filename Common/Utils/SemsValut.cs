using Common.Constant;
using Common.Models;
using Common.Models.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.Utils
{
    public class SemsValut
    {
        public static HttpFile SaveFile(IObjectFile dObject, HttpPostedFileBase httpFile)
        {
            HttpFile retFile = new HttpFile();

            string StoragePath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["FileStorage"]);
            string ValutPath = System.Configuration.ConfigurationManager.AppSettings["ValutPath"];

            string SavePath = dObject.Type + "/" + dObject.OID.ToString();

            try
            {
                retFile.FileSize = httpFile.ContentLength;

                /*폴더생성*/
                DirectoryInfo di = new DirectoryInfo(StoragePath + "/" + ValutPath + "/" + SavePath);

                if (!di.Exists) { di.Create(); }

                FileInfo uploadFile = new FileInfo(di.FullName + "/" + httpFile.FileName);

                string ext = uploadFile.Extension;
                string purefileName = uploadFile.Name.Substring(0, uploadFile.Name.Length - ext.Length);
                string ConvFileName = SemsEncrypt.AESEncrypt256Text(uploadFile.Name, CommonConstant.TEXT_ENCRYPT_KEY);

                FileInfo fi = new FileInfo(di.FullName + "/" + ConvFileName);

                if (fi.Exists)
                {
                    /*중복되지 않은 파일명 생성*/
                    int cnt = 1;
                    string newFileName = "";
                    string pureName = purefileName;

                    while (fi.Exists)
                    {
                        newFileName = pureName + " (" + (cnt++) + ")" + ext;
                        ConvFileName = SemsEncrypt.AESEncrypt256Text(newFileName, CommonConstant.TEXT_ENCRYPT_KEY);
                        fi = new FileInfo(StoragePath + "/" + ValutPath + "/" + SavePath + "/" + ConvFileName);
                    }
                }

                SemsEncrypt.AESEncrypt256File(httpFile, fi.FullName, CommonConstant.FILE_ENCRYPT_KEY);

                retFile.OID = dObject.OID;
                retFile.Type = dObject.Type;
                retFile.OrgNm = uploadFile.Name;
                retFile.ConvNm = ConvFileName;
                retFile.Ext = ext;

                return retFile;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void FileDelete(HttpFile httpFile)
        {
            try
            {
                string StoragePath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["FileStorage"]);
                string ValutPath = System.Configuration.ConfigurationManager.AppSettings["ValutPath"];

                string SavePath = httpFile.Type + "/" + httpFile.OID.ToString();

                string fileFullDirectory = Path.Combine(StoragePath, ValutPath, SavePath, httpFile.ConvNm);

                FileInfo fi = new FileInfo(fileFullDirectory);

                if (!fi.Exists)
                {
                    return;
                }

                try
                {
                    fi.Delete();
                }
                catch { }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Stream GetFileStream(HttpFile fileModel)
        {
            try
            {
                string StoragePath = System.Web.HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["FileStorage"]);
                string ValutPath = System.Configuration.ConfigurationManager.AppSettings["ValutPath"];
                string TempPath = System.Configuration.ConfigurationManager.AppSettings["TempPath"];

                string SavePath = fileModel.Type + "/" + fileModel.OID.ToString();

                string fileFullDirectory = Path.Combine(StoragePath, ValutPath, SavePath, fileModel.ConvNm);

                FileInfo fi = new FileInfo(fileFullDirectory);

                if (!fi.Exists)
                {
                    throw new Exception("파일이 존재하지않습니다.");
                }

                /*폴더생성*/
                DirectoryInfo di = new DirectoryInfo(StoragePath + "/" + TempPath);

                if (!di.Exists) { di.Create(); }

                FileInfo downloadFi = new FileInfo(Path.Combine(StoragePath, TempPath, fileModel.ConvNm));

                fi.CopyTo(downloadFi.FullName, true);

                SemsEncrypt.AESDecrypt256File(fi, downloadFi.FullName, CommonConstant.FILE_ENCRYPT_KEY);

                var fs = new FileStream(downloadFi.FullName, FileMode.Open);
                return fs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
