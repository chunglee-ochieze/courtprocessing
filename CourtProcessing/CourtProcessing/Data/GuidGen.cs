using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourtProcessing.Data
{
    public static class GuidGen
    {
        public static string RandomGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public static long MilliTime()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public static string TimeGen(int length)
        {
            if (length > 14)
                length = 14;

            return MilliTime().ToString()[^length..];
        }
    }
}
