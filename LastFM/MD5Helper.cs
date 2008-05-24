using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace LastFM
{
    class MD5Helper
    {
        public static string MD5(string input)
        {
            MD5CryptoServiceProvider md = new MD5CryptoServiceProvider();
            byte[] pass = ASCIIEncoding.Default.GetBytes(input);
            byte[] outp = md.ComputeHash(pass);

            string hash = "";
            foreach (byte b in outp)
            {
                hash += b.ToString("x2");
            }
            return hash;
        }
    }
}
