using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTunesLib;

namespace iTunesCore
{
    public class iTunesInstance
    {
        private static iTunesAppClass _instance;
        public static iTunesAppClass Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new iTunesAppClass();
                }
                return _instance;
            }
        }
    }
}
