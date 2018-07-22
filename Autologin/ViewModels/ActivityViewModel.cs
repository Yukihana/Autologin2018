using Autologin.Extensions;
using Autologin.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Timers;
using System.Windows;

namespace Autologin.ViewModels
{
    class ActivityViewModel : ViewModelBase
    {
        #region Constructor(s)
        public ActivityViewModel()
        {
            DataIdentifier = "ActivityData";

            // Refresh/load the model data
            ResetModel();

            // Refresh/load the timer settings
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
        private int TimerState = 0;
        private bool RoutineBusy = false;
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(!DataModel.TimerEnabled)
            {
                timer.Stop();
                return;
            }

            TimerState++;
            if(TimerState >= DataModel.TimerCycle)
            {
                TimerState = 0;
                if(!RoutineBusy)
                {
                    InvokeRoutine();
                }
            }
        }
        #endregion

        #region Routine
        private async void InvokeRoutine()
        {
            RoutineBusy = true;
            DataModel.WebStatus = false;
            DataModel.GatewayStatus = false;
            DataModel.PortalStatus = false;

            List<bool> PingSet = new List<bool>();

            for(int i = 0;i<5;i++)
            {
                PingSet.Add(NetworkExtensions.PingDomain("www.google.com") >= 0);
            }
            for (int i = 0; i < 5; i++)
            {
                PingSet.Add(NetworkExtensions.PingDomain("www.facebook.com") >= 0);
            }
            if(PingSet.Contains(true))
            {
                DataModel.WebStatus = true;
                RoutineBusy = false;
                return;
            }

            // Fail to ping websites: internet probably off
            PingSet = new List<bool>();
            IPAddress.TryParse("10.10.235.38", out IPAddress GatewayIP);
            for(int i = 0;i<5;i++)
            {
                PingSet.Add(NetworkExtensions.PingIP(GatewayIP) >= 0);
            }
            if (!PingSet.Contains(true))
            {
                RoutineBusy = false;
                return;
            }
            DataModel.GatewayStatus = true;

            // Success to ping gateway: try to access portal
            PingSet = new List<bool>();
            IPAddress.TryParse("10.254.254.4", out IPAddress PortalIP);
            for (int i = 0; i < 5; i++)
            {
                PingSet.Add(NetworkExtensions.PingIP(PortalIP) >= 0);
            }
            if (!PingSet.Contains(true))
            {
                RoutineBusy = false;
                return;
            }
            DataModel.GatewayStatus = true;

            // Success to ping gateway: try to check if logged in or not
            HttpClient client = new HttpClient();
            var responseData = await client.GetStringAsync("http://10.254.254.4/0/up/");
            if (responseData.Contains("12964004564"))
            {
                RoutineBusy = false;
                return;
            }

            // On fail, try to log in
            var httpparams = new FormUrlEncodedContent(
                new List<KeyValuePair<string,string>>()
                {
                    new KeyValuePair<string, string>("user", "souviks_sim"),
                    new KeyValuePair<string, string>("pass", "8697243779"),
                    new KeyValuePair<string, string>("login", "Login")
                }
            );
            var response = await client.PostAsync("http://10.254.254.4/0/up/", httpparams);
            responseData = await response.Content.ReadAsStringAsync();

            if (responseData.Contains("12964004564"))
            {
                // Success logging in
            }
            RoutineBusy = false;
        }
        
        #endregion














        #region Routine
        /// <summary>
        /// The master routine to be run in a separate thread.
        /// </summary>
        void ActivityRoutine()
        {

        }
        #endregion
    }
}
