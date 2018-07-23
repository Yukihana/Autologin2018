namespace Autologin.DataTypes
{
    #region Includes
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

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
