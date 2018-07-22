namespace Autologin.Models
{
    #region MyRegion
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Xml.Serialization;
    #endregion

    [Serializable]
    public class ActivityModel : ModelBase
    {
        #region Constructor(s)
        /// <summary>
        /// 
        /// </summary>
        public ActivityModel() : base()
        {

        }
        #endregion

        // Addresses
        #region Gateway
        string _gateway;
        /// <summary>
        /// Store as string since IPAddress is not serializable.
        /// </summary>
        public string GatewayIP
        {
            get=> _gateway;
            set
            {
                _gateway = value;
                OnPropertyChanged("GatewayIP");
            }
        }
        /// <summary>
        /// IP Address of the Gateway
        /// </summary>
        [XmlIgnore]
        public IPAddress Gateway
        {
            get
            {
                IPAddress.TryParse(GatewayIP, out IPAddress result);
                return result ?? new IPAddress(0);
            }
            set
            {
                GatewayIP = value.ToString();
                OnPropertyChanged("Gateway");
            }
        }
        #endregion
        #region Portal
        string _portal;
        /// <summary>
        /// Store as string since IPAddress is not serializable.
        /// </summary>
        public string PortalIP
        {
            get => _portal;
            set
            {
                _portal = value;
                OnPropertyChanged("PortalIP");
            }
        }
        /// <summary>
        /// IP Address of the Login-Portal
        /// </summary>
        [XmlIgnore]
        public IPAddress Portal
        {
            get
            {
                IPAddress.TryParse(PortalIP, out IPAddress result);
                return result ?? new IPAddress(0);
            }
            set
            {
                PortalIP = value.ToString();
                OnPropertyChanged("Portal");
            }
        }
        #endregion
        #region NIC
        string _nic;
        /// <summary>
        /// Store as string since IPAddress is not serializable.
        /// </summary>
        public string NicIP
        {
            get => _nic;
            set
            {
                _nic = value;
                OnPropertyChanged("NicIP");
            }
        }
        /// <summary>
        /// IP Address of the computer's network interface
        /// </summary>
        [XmlIgnore]
        public IPAddress Nic
        {
            get
            {
                IPAddress.TryParse(NicIP, out IPAddress result);
                return result ?? new IPAddress(0);
            }
            set
            {
                NicIP = value.ToString();
                OnPropertyChanged("Nic");
            }
        }
        #endregion
        #region Websites
        List<string> _websites;
        /// <summary>
        /// Addresses of websites to ping for connectivity.
        /// </summary>
        public List<string> Websites
        {
            get => _websites;
            set
            {
                _websites = value;
                OnPropertyChanged("Websites");
            }
        }
        #endregion

        // Timer
        #region TimerEnabled
        bool _timerEnabled;
        public bool TimerEnabled
        {
            get => _timerEnabled;
            set
            {
                _timerEnabled = value;
                OnPropertyChanged("TimerEnabled");
            }
        }
        #endregion
        #region TimerCycle
        int _timerCycle;
        public int TimerCycle
        {
            get => _timerCycle;
            set
            {
                _timerCycle = value;
                OnPropertyChanged("TimerCycle");
            }
        }
        #endregion
        #region TimerState
        int _timerState;
        public int TimerState
        {
            get => _timerState;
            set
            {
                _timerState = value;
                OnPropertyChanged("TimerState");
            }
        }
        #endregion

        // Status
        #region WebStatus
        private bool _webStatus;
        public bool WebStatus
        {
            get => _webStatus;
            set
            {
                _webStatus = value;
                OnPropertyChanged("WebStatus");
            }
        }
        #endregion
        #region MyRegion
        private bool _gatewayStatus;
        public bool GatewayStatus
        {
            get => _gatewayStatus;
            set
            {
                _gatewayStatus = value;
                OnPropertyChanged("GatewayStatus");
            }
        }
        #endregion
        #region PortalStatus
        private bool _portalStatus;
        public bool PortalStatus
        {
            get => _portalStatus;
            set
            {
                _portalStatus = value;
                OnPropertyChanged("PortalStatus");
            }
        }
        #endregion

    }
}
