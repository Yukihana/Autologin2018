namespace Autologin.ViewModels
{
    #region Includes
    using Autologin.Extensions;
    using Autologin.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.NetworkInformation;
    using System.Text;
    using System.Threading.Tasks;
    using System.Timers;
    using System.Windows;
    using System.Windows.Threading;
    #endregion

    class ActivityViewModel : ViewModelBase
    {
        #region Constructor(s)
        public ActivityViewModel()
        {
            DataIdentifier = "ActivityData";

            // Refresh/load the components
            ResetModel();
            ResetTimer();

            // First Run
            InvokeRoutine();

            // Timer Start
            timer.Start();
        }
        #endregion

        // Components
        #region DataModel
        private ActivityModel _dataModel;
        /// <summary>
        /// The DataModel for the associated view
        /// </summary>
        public ActivityModel DataModel
        {
            get => _dataModel;
            set
            {
                _dataModel = value;
                SaveData(this, new ActivityModel());
            }
        }
        /// <summary>
        /// Loads the DataModel using the Storage Controller.
        /// </summary>
        void ResetModel()
        {
            // Load Data
            LoadData(this, new ActivityModel());
        }
        #endregion

        #region Timer
        /// <summary>
        /// A Timer object to repeat the primary routine
        /// </summary>
        public Timer timer = null;
        /// <summary>
        /// Resets/loads the timer settings
        /// </summary>
        void ResetTimer(bool force = false)
        {
            // One time, first assignment
            if (timer == null)
            {
                timer = new Timer
                {
                    AutoReset = true,
                    Interval = 1000
                };
                timer.Elapsed += Timer_Elapsed;
            }

            // Sync the timer state
            if (timer.Enabled != DataModel.TimerEnabled)
            {
                if (DataModel.TimerEnabled)
                {
                    timer.Start();
                }
                else
                {
                    timer.Stop();
                }
            }
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Stop timer if disabled
            if(!DataModel.TimerEnabled)
            {
                timer.Stop();
                return;
            }

            // If routine is currently active: Reset the timer and discard this cycle
            if (AsyncWorkerIsBusy)
            {
                DataModel.TimerState = 0;
                return;
            }

            // Increment if within cycle
            DataModel.TimerState++;

            // Check if it's a hit, execute
            if(DataModel.TimerState >= DataModel.TimerCycle)
            {
                InvokeRoutine();
                DataModel.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;
                DataModel.TimerState = 0;
            }
        }
        #endregion

        // Execution
        #region Routine
        private void InvokeRoutine()
        {
            if (AsyncWorkerIsBusy != true)
            {
                Task.Run(()=>AsyncSubroutine());
            }
        }

        private bool AsyncWorkerIsBusy = false;
        private async void AsyncSubroutine()
        {
            // Mark locking flag
            AsyncWorkerIsBusy = true;

            bool w = false, g = false, p = false, n = false, d = false;
            // Web
            w = ProcessWebStatus();

            // Gateway
            if(w == false)
            {
                g = ProcessGatewayStatus();
            }

            // Portal
            if (g == true)
            {
                p = ProcessPortalStatus();
            }

            // Login
            if(p == true)
            {
                n = await ProcessLoginStatus();

                if(n == false)
                {
                    d = await DoLogin();
                }
            }

            // Recheck for Connectivity
            if(d == true)
            {
                w = ProcessWebStatus();
            }

            // Unmark locking flag
            AsyncWorkerIsBusy = false;

            // Post-Processing
            DataModel.ProgressState = (DataModel.WebStatus) ? System.Windows.Shell.TaskbarItemProgressState.Normal : System.Windows.Shell.TaskbarItemProgressState.Error;

            // Invoke Statistics on Async
            AsyncStatistics();
        }
        #endregion


        #region AsyncRoutineParts
        private bool ProcessWebStatus()
        {
            // Get State
            var r = NetworkExtensions.GetPingByTries(
                DataModel.Websites,
                DataModel.Attempts,
                DataModel.Timeout,
                DataModel.TimeoutIncrement
                );

            // Process Results
            DataModel.WebStatus = !(r == null);
            if (r != null)
            {
                DataModel.LastWebPing = r.RoundtripTime;
                DataModel.LastWebDest = r.Address.ToString();
            }

            return DataModel.WebStatus;
        }
        private bool ProcessGatewayStatus()
        {
            // Get State
            var r = NetworkExtensions.GetPingByTries(
                new List<IPAddress>()
                    {
                        DataModel.Gateway
                    },
                5,
                5000
                );

            // Process Results
            DataModel.GatewayStatus = !(r == null);
            if (r != null)
            {
                DataModel.LastGatewayPing = r.RoundtripTime;
                DataModel.LastGatewayDest = r.Address.ToString();
            }

            return DataModel.GatewayStatus;
        }
        private bool ProcessPortalStatus()
        {
            // Get State
            var r = NetworkExtensions.GetPingByTries(
                new List<IPAddress>()
                    {
                        DataModel.Portal
                    },
                5,
                5000
                );

            // Process Results
            DataModel.PortalStatus = !(r == null);
            if (r != null)
            {
                DataModel.LastPortalPing = r.RoundtripTime;
                DataModel.LastPortalDest = r.Address.ToString();
            }

            return DataModel.PortalStatus;
        }
        private async Task<bool> DoLogin()
        {
            HttpClient client = new HttpClient();
            var httpparams = new FormUrlEncodedContent(
                new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("user", "souviks_sim"),
                    new KeyValuePair<string, string>("pass", "8697243779"),
                    new KeyValuePair<string, string>("login", "Login")
                }
            );
            var response = await client.PostAsync("http://10.254.254.4/0/up/", httpparams);
            var responseData = await response.Content.ReadAsStringAsync();

            return responseData.Contains("12964004564");
        }
        private async Task<bool> ProcessLoginStatus()
        {
            HttpClient client = new HttpClient();
            var responseData = await client.GetStringAsync("http://10.254.254.4/0/up/");
            return responseData.Contains("12964004564");
        }
        #endregion

        private void AsyncStatistics()
        {
            
        }
    }
}
