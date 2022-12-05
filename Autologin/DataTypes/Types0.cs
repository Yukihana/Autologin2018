using System;

namespace Autologin.DataTypes
{
    [Serializable]
    public class Keyval
    {
        public string key = string.Empty;
        public string val = string.Empty;

        public Keyval()
        {
        }

        public Keyval(string K, string V)
        {
            key = K;
            val = V;
        }
    }
}