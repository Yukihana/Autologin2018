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

        public static PingReply GetPingByTries(List<string> Destinations, int Attempts = 4, int Timeout = 2000, int TimeoutIncrement = 1000)
        {
            PingReply r = null;
            Ping P = new Ping();
            for (int i = 0; i < Attempts; i++)
            {
                foreach (string t in Destinations)
                {
                    try
                    {
                        r = P.Send(t, Timeout);
                    }
                    catch (Exception)
                    {
                        r = null;
                    }
                    if (r != null)
                    {
                        if (r.Status == IPStatus.Success)
                        {
                            return r;
                        }
                    }
                }
                Timeout += TimeoutIncrement;
            }

            return null;
        }

        public static PingReply GetPingByTries(List<IPAddress> Destinations, int Attempts = 4, int Timeout = 2000, int TimeoutIncrement = 1000)
        {
            PingReply r = null;
            Ping P = new Ping();
            for (int i = 0; i < Attempts; i++)
            {
                foreach (IPAddress t in Destinations)
                {
                    try
                    {
                        r = P.Send(t, Timeout);
                    }
                    catch (Exception)
                    {
                        r = null;
                    }
                    if (r.Status == IPStatus.Success)
                    {
                        return r;
                    }
                }
                Timeout += TimeoutIncrement;
            }

            return null;
        }
    }
}
