using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchBot.Core
{
    public static class App
    {
        public static DateTime startTime;
        public static Bot bot;

        static Random r;
        public static Random R
        {
            get
            {
                if (r == null)
                    r = new Random();
                return r;
            }
        }

    }
}
