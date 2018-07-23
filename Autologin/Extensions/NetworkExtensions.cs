namespace Autologin.Extensions
{
    #region Includes
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.NetworkInformation;
    using System.Text;

    using Autologin.DataTypes;
    #endregion

    public class NetworkExtensions
    {
        public static FormUrlEncodedContent EncodeParams(List<Keyval> vars)
        {
            List<KeyValuePair<string, string>> r = new List<KeyValuePair<string, string>>();
            foreach (Keyval k in vars)
            {
                r.Add(new KeyValuePair<string, string>(k.key, k.val));
            }

            return new FormUrlEncodedContent(r);
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
    }
}
