namespace Autologin.Extensions
{
    #region Includes
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Text;
    #endregion

    public class NetworkExtensions
    {
        public static long PingIP(IPAddress Destination, int Timeout = 5000)
        {
            long r = 0;
            Ping P = new Ping();
            PingReply PReply = P.Send(Destination, Timeout);
            if (PReply.Status != IPStatus.Success)
            {
                r = (int)PReply.Status;
                if (r > 0)
                {
                    r *= -1;
                }
            }
            else
            {
                r = PReply.RoundtripTime;
            }

            return r;
        }

        public static long PingDomain(string Domain, int Timeout = 5000)
        {
            long r = 0;
            Ping P = new Ping();
            PingReply PReply = null;
            try
            {
                PReply = P.Send(Domain, Timeout);
            }
            catch(Exception)
            {
                return -1;
            }

            if (PReply.Status != IPStatus.Success)
            {
                r = (int)PReply.Status;
                if (r > 0)
                {
                    r *= -1;
                }
            }
            else
            {
                r = PReply.RoundtripTime;
            }

            return r;
        }
    }
}
