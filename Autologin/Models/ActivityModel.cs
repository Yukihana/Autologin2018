namespace Autologin.Models
{
    using Autologin.DataTypes;
    #region MyRegion
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Windows.Shell;
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
            // Saved Data
            Websites = new List<string>();
            Gateway = new IPAddress(0);
            Portal = new IPAddress(0);
            Nic = new IPAddress(0);

            LoginAction = string.Empty;
            LoginCredentials = new List<Keyval>();
            LoginIsPost = false;
            LoginSearchString = string.Empty;

            Attempts = 4;
            Timeout = 2000;
            TimeoutIncrement = 1000;

            TimerEnabled = true;
            TimerState = 0;
            TimerCycle = 60;

            // Ignored Data
            ProgressValue = 0;
            ProgressState = TaskbarItemProgressState.Normal;

            NicStatus = false;
            LastNicPing = 0;
            LastNicDest = string.Empty;

            GatewayStatus = false;
            LastGatewayPing = 0;
            LastGatewayDest = string.Empty;

            PortalStatus = false;
            LastPortalPing = 0;
            LastPortalDest = string.Empty;

            WebStatus = false;
            LastWebPing = 0;
            LastWebDest = string.Empty;

            LoginStatus = false;
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

        // Login
        #region LoginAction
        private string _loginAction;
        /// <summary>
        /// The target site of the login form.
        /// </summary>
        public string LoginAction
        {
            get => _loginAction;
            set
            {
                _loginAction = value;
                OnPropertyChanged("LoginAction");
            }
        }
        #endregion
        #region LoginCredentials
        private List<Keyval> _loginCredentials;
        /// <summary>
        /// The parameters sent in the form to authenticate the login.
        /// </summary>
        public List<Keyval> LoginCredentials
        {
            get => _loginCredentials;
            set
            {
                _loginCredentials = value;
                OnPropertyChanged("LoginCredentials");
            }
        }
        #endregion
        #region LoginIsPost
        private bool _loginIsPost;
        /// <summary>
        /// Set whether the login form uses the GET or POST method.
        /// </summary>
        public bool LoginIsPost
        {
            get => _loginIsPost;
            set
            {
                _loginIsPost = value;
                OnPropertyChanged("LoginIsPost");
            }
        }
        #endregion
        #region LoginSearchString
        private string _loginSearchString;
        /// <summary>
        /// The target site of the login form.
        /// </summary>
        public string LoginSearchString
        {
            get => _loginSearchString;
            set
            {
                _loginSearchString = value;
                OnPropertyChanged("LoginSearchString");
            }
        }
        #endregion

        // Misc
        #region Attempts
        private int _attempts;
        /// <summary>
        /// Denotes the number of attempts to ping before concluding it as timeout.
        /// </summary>
        public int Attempts
        {
            get => _attempts;
            set
            {
                _attempts = value;
                OnPropertyChanged("Attempts");
            }
        }
        #endregion
        #region Timeout
        private int _timeout;
        /// <summary>
        /// Denotes the maximum amount of time to wait for a ping reply (in miliseconds).
        /// </summary>
        public int Timeout
        {
            get => _timeout;
            set
            {
                _timeout = value;
                OnPropertyChanged("Timeout");
            }
        }
        #endregion
        #region TimeoutIncrement
        private int _timeoutIncrement;
        /// <summary>
        /// Denotes the maximum amount of time to wait for a ping reply (in miliseconds).
        /// </summary>
        public int TimeoutIncrement
        {
            get => _timeoutIncrement;
            set
            {
                _timeoutIncrement = value;
                OnPropertyChanged("TimeoutIncrement");
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
        [XmlIgnore]
        public int TimerState
        {
            get => _timerState;
            set
            {
                _timerState = value;
                ProgressFloat = (TimerCycle != 0) ? ((float)TimerState / TimerCycle) : 0;
                ProgressValue = (int)(ProgressFloat * 100);
                OnPropertyChanged("TimerState");
            }
        }
        #endregion

        // Fields not to be saved
        // Progress
        #region ProgressValue
        private int _progressValue;
        [XmlIgnore]
        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                OnPropertyChanged("ProgressValue");
            }
        }
        #endregion
        #region ProgressFloat
        private float _progressFloat;
        [XmlIgnore]
        public float ProgressFloat
        {
            get => _progressFloat;
            set
            {
                _progressFloat = value;
                OnPropertyChanged("ProgressFloat");
            }
        }
        #endregion
        #region ProgressState
        private TaskbarItemProgressState _progressState;
        [XmlIgnore]
        public TaskbarItemProgressState ProgressState
        {
            get => _progressState;
            set
            {
                _progressState = value;
                OnPropertyChanged("ProgressState");
            }
        }
        #endregion

        // NicStatus
        #region NicStatus
        private bool _nicStatus;
        /// <summary>
        /// Last polled status of internet availability.
        /// </summary>
        [XmlIgnore]
        public bool NicStatus
        {
            get => _nicStatus;
            set
            {
                _nicStatus = value;
                OnPropertyChanged("NicStatus");
            }
        }
        #endregion
        #region NicPing
        private long _lastNicPing;
        /// <summary>
        /// Ping time from last nic poll.
        /// </summary>
        [XmlIgnore]
        public long LastNicPing
        {
            get => _lastNicPing;
            set
            {
                _lastNicPing = value;
                OnPropertyChanged("LastNicPing");
            }
        }
        #endregion
        #region NicDest
        private string _lastNicDest;
        /// <summary>
        /// Ping destination from last nic poll.
        /// </summary>
        [XmlIgnore]
        public string LastNicDest
        {
            get => _lastNicDest;
            set
            {
                _lastNicDest = value;
            }
        }
        #endregion

        // GatewayStatus
        #region GatewayStatus
        private bool _gatewayStatus;
        /// <summary>
        /// Last polled status of internet availability.
        /// </summary>
        [XmlIgnore]
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
        #region GatewayPing
        private long _lastGatewayPing;
        /// <summary>
        /// Ping time from last gateway poll.
        /// </summary>
        [XmlIgnore]
        public long LastGatewayPing
        {
            get => _lastGatewayPing;
            set
            {
                _lastGatewayPing = value;
                OnPropertyChanged("LastGatewayPing");
            }
        }
        #endregion
        #region GatewayDest
        private string _lastGatewayDest;
        /// <summary>
        /// Ping destination from last gateway poll.
        /// </summary>
        [XmlIgnore]
        public string LastGatewayDest
        {
            get => _lastGatewayDest;
            set
            {
                _lastGatewayDest = value;
            }
        }
        #endregion

        // PortalStatus
        #region PortalStatus
        private bool _portalStatus;
        /// <summary>
        /// Last polled status of internet availability.
        /// </summary>
        [XmlIgnore]
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
        #region PortalPing
        private long _lastPortalPing;
        /// <summary>
        /// Ping time from last portal poll.
        /// </summary>
        [XmlIgnore]
        public long LastPortalPing
        {
            get => _lastPortalPing;
            set
            {
                _lastPortalPing = value;
                OnPropertyChanged("LastPortalPing");
            }
        }
        #endregion
        #region PortalDest
        private string _lastPortalDest;
        /// <summary>
        /// Ping destination from last portal poll.
        /// </summary>
        [XmlIgnore]
        public string LastPortalDest
        {
            get => _lastPortalDest;
            set
            {
                _lastPortalDest = value;
            }
        }
        #endregion

        // WebStatus
        #region WebStatus
        private bool _webStatus;
        /// <summary>
        /// Last polled status of internet availability.
        /// </summary>
        [XmlIgnore]
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
        #region WebPing
        private long _lastWebPing;
        /// <summary>
        /// Ping time from last website poll.
        /// </summary>
        [XmlIgnore]
        public long LastWebPing
        {
            get => _lastWebPing;
            set
            {
                _lastWebPing = value;
                OnPropertyChanged("LastWebPing");
            }
        }
        #endregion
        #region WebDest
        private string _lastWebDest;
        /// <summary>
        /// Ping destination from last website poll.
        /// </summary>
        [XmlIgnore]
        public string LastWebDest
        {
            get => _lastWebDest;
            set
            {
                _lastWebDest = value;
                OnPropertyChanged("LastWebDest");
            }
        }
        #endregion

        // LoggedInStatus
        #region LoginStatus
        private bool _loginStatus;
        /// <summary>
        /// Last polled status of internet availability.
        /// </summary>
        [XmlIgnore]
        public bool LoginStatus
        {
            get => _loginStatus;
            set
            {
                _loginStatus = value;
                OnPropertyChanged("LoginStatus");
            }
        }
        #endregion
    }
}
