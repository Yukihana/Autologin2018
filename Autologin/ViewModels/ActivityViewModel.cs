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
                DataModel.TimerState = 0;
                InvokeRoutine();
            }
        }
        #endregion

        // Execution
        #region Routine
        private void InvokeRoutine()
        {
            DataModel.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;
            if (! AsyncWorkerIsBusy)
            {
                Task.Run(()=>AsyncSubroutine());
            }
        }

        private bool AsyncWorkerIsBusy = false;
        private async void AsyncSubroutine()
        {
            // Mark locking flag
            AsyncWorkerIsBusy = true;

            // Run Check Routine
            if(DataModel.WebStatus)
            {
                await PassiveSubroutine();
            }
            else
            {
                await ActiveSubroutine();
            }

            // Unmark locking flag
            AsyncWorkerIsBusy = false;

            // Post-Processing
            DataModel.ProgressState = (DataModel.WebStatus) ? System.Windows.Shell.TaskbarItemProgressState.Normal : System.Windows.Shell.TaskbarItemProgressState.Error;
        }
        /// <summary>
        /// To be used when last poll failed to ping websites.
        /// </summary>
        private async Task ActiveSubroutine()
        {
            bool n = false, w = false, g = false, p = false, d = false;

            // Ping in near-to-far order
            n = ProcessNicStatus();
            g = ProcessGatewayStatus();
            p = ProcessPortalStatus();

            // Check for Login
            if (g && p)
            {
                d = await ProcessLoginStatus();
                if (!d)
                {
                    await DoLogin();
                }
                DataModel.LoginStatus = await ProcessLoginStatus();
            }
            else
            {
                DataModel.LoginStatus = false;
            }

            // Finally make sure web is connected
            w = ProcessWebStatus();
        }
        /// <summary>
        /// To be used when last poll succeeded at pinging websites.
        /// </summary>
        private async Task PassiveSubroutine()
        {
            bool n = false, w = false, g = false, p = false, d = false;

            // Ping in far-to-near order
            w = ProcessWebStatus();
            p = ProcessPortalStatus();
            g = ProcessGatewayStatus();
            n = ProcessNicStatus();

            // Login if conditions meet
            if(!w && p && g)
            {
                d = await ProcessLoginStatus();
                if(!d)
                {
                    await DoLogin();
                }
                DataModel.LoginStatus = await ProcessLoginStatus();
            }

            if (!g)
            {
                DataModel.LoginStatus = false;
            }

            // Final Check
            w = ProcessWebStatus();
        }
        #endregion

        #region AsyncRoutineParts
        private bool ProcessNicStatus()
        {
            // Get State
            var r = NetworkExtensions.GetPingByTries(
                new List<IPAddress>()
                    {
                        DataModel.Nic
                    },
                DataModel.Attempts,
                DataModel.Timeout,
                DataModel.TimeoutIncrement
                );

            // Process Results
            DataModel.NicStatus = !(r == null);
            if (r != null)
            {
                DataModel.LastNicPing = r.RoundtripTime;
                DataModel.LastNicDest = r.Address.ToString();
            }

            return DataModel.NicStatus;
        }
        private bool ProcessGatewayStatus()
        {
            // Get State
            var r = NetworkExtensions.GetPingByTries(
                new List<IPAddress>()
                    {
                        DataModel.Gateway
                    },
                DataModel.Attempts,
                DataModel.Timeout,
                DataModel.TimeoutIncrement
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
                DataModel.Attempts,
                DataModel.Timeout,
                DataModel.TimeoutIncrement
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
        private async Task<bool> ProcessLoginStatus()
        {
            try
            {
                HttpClient client = new HttpClient();
                var responseData = await client.GetStringAsync(DataModel.LoginAction);
                DataModel.LoginStatus = responseData.Contains(DataModel.LoginSearchString);
                return DataModel.LoginStatus;
            }
            catch(Exception)
            {
            }

            return false;
        }
        private async Task<bool> DoLogin()
        {
            HttpClient client = new HttpClient();
            var httpparams = NetworkExtensions.EncodeParams(DataModel.LoginCredentials);
            string responseData = string.Empty;
            try
            {   
                if (DataModel.LoginIsPost)
                {
                    var response = await client.PostAsync(DataModel.LoginAction, httpparams);
                    responseData = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    responseData = await client.GetStringAsync(DataModel.LoginAction + "?" + httpparams);
                }
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }
        #endregion
    }
}
