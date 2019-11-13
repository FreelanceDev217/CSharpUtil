// To be used for simple encryption/decryption
// David Piao
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PCKLIB
{
    public class SimpleAES
    {
        private static byte[] key = { 123, 217, 19, 11, 52, 26, 85, 45, 47, 184, 27, 162, 37, 112, 234, 124, 241, 24, 175, 144, 173, 53, 196, 29, 24, 26, 17, 218, 131, 236, 53, 209};
        private static byte[] vector = {192, 146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 221, 112, 79, 32, 114, 156};

        private ICryptoTransform encryptor, decryptor;
        private UTF8Encoding encoder;

        public SimpleAES(string IV = "")
        {
            if(IV != "")
            {
                var vector_lis = Encoding.ASCII.GetBytes(IV);
                for(int i = 0; i < 16; i ++)
                {
                    if (i < vector_lis.Length)
                        vector[i] = vector_lis[i];
                }
            }
            RijndaelManaged rm = new RijndaelManaged();
            encryptor = rm.CreateEncryptor(key, vector);
            decryptor = rm.CreateDecryptor(key, vector);
            encoder = new UTF8Encoding();
        }

        public string Encrypt(string unencrypted)
        {
            if (unencrypted == null || unencrypted.Length == 0)
                return "";
            return Convert.ToBase64String(Encrypt(encoder.GetBytes(unencrypted)));
        }

        public string Decrypt(string encrypted)
        {
            if (encrypted == null || encrypted.Length == 0)
                return "";

            return encoder.GetString(Decrypt(Convert.FromBase64String(encrypted)));
        }

        public byte[] Encrypt(byte[] buffer)
        {
            return Transform(buffer, encryptor);
        }

        public byte[] Decrypt(byte[] buffer)
        {
            return Transform(buffer, decryptor);
        }

        protected byte[] Transform(byte[] buffer, ICryptoTransform transform)
        {
            MemoryStream stream = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(stream, transform, CryptoStreamMode.Write))
            {
                cs.Write(buffer, 0, buffer.Length);
            }
            return stream.ToArray();
        }
    }
}
