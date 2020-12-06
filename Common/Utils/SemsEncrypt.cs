using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.Utils
{
    public class SemsEncrypt
    {
        public static string AESEncrypt256Text(string _text, string _password)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            byte[] PlainText = Encoding.Unicode.GetBytes(_text);
            byte[] Salt = Encoding.ASCII.GetBytes(_password.Length.ToString());

            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(_password, Salt);//암호화 된 키

            ICryptoTransform Encrytor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16)); //암호화되면 길이, 암호화 시킬 것?
            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encrytor, CryptoStreamMode.Write);

            cryptoStream.Write(PlainText, 0, PlainText.Length);
            cryptoStream.FlushFinalBlock();

            byte[] CipherBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            string base64String = Convert.ToBase64String(CipherBytes);

            return base64String.Replace("/", "_");
        }

        public static string AESDecrypte256Text(string _text, string _password)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            byte[] EncryptedData = Convert.FromBase64String(_text.Replace("_", "/"));
            byte[] Salt = Encoding.ASCII.GetBytes(_password.Length.ToString());

            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(_password, Salt);

            ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
            MemoryStream memoryStream = new MemoryStream(EncryptedData);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);

            byte[] PlainText = new byte[EncryptedData.Length];
            int DecrypedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);

            memoryStream.Close();
            cryptoStream.Close();

            return Encoding.Unicode.GetString(PlainText, 0, DecrypedCount);
        }

        public static void AESEncrypt256File(HttpPostedFileBase _file, string _outputFile, string _password)
        {
            try
            {
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] Salt = Encoding.ASCII.GetBytes(_password.Length.ToString());
                byte[] key = UE.GetBytes(_password);

                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(_password, Salt);//암호화 된 키

                FileStream fsCrypt = new FileStream(_outputFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                ICryptoTransform Encrytor = RMCrypto.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16)); //암호화되면 길이, 암호화 시킬 것?

                CryptoStream cs = new CryptoStream(fsCrypt, Encrytor, CryptoStreamMode.Write);

                //FileStream fsIn = _file.InputStream as FileStream;

                BinaryReader br = new BinaryReader(_file.InputStream);
                byte[] binData = br.ReadBytes(_file.ContentLength);

                foreach (byte b in binData)
                {
                    cs.WriteByte(b);
                }
                cs.Flush();
                cs.Close();
                fsCrypt.Close();
                br.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void AESDecrypt256File(FileInfo fileInfo, string _outputFile, string _password)
        {
            try
            {
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] Salt = Encoding.ASCII.GetBytes(_password.Length.ToString());
                byte[] key = UE.GetBytes(_password);

                PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(_password, Salt);//암호화 된 키

                FileStream fsCrypt = fileInfo.OpenRead();

                RijndaelManaged RMCrypto = new RijndaelManaged();
                ICryptoTransform Decrytor = RMCrypto.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16)); //암호화되면 길이, 암호화 시킬 것?

                CryptoStream cs = new CryptoStream(fsCrypt, Decrytor, CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(_outputFile, FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                    fsOut.WriteByte((byte)data);

                fsOut.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
