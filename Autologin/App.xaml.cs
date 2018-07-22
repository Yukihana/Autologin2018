namespace Autologin
{
    #region Includes
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Windows;

    using Autologin.Models;
    using Autologin.Storage;
    using Autologin.ViewModels;
    using Autologin.Extensions;
    #endregion

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Ctors and Dtors
        #region Constructor(s)
        public App() : base()
        {
            DataManager = new StorageController();
            ActivityContext = new ActivityViewModel();
        }
        #endregion
        #region OnExit
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            DataManager.FinalSave();
        }
        #endregion

        // DataManager
        #region DataManager (Generic)
        private StorageController _dataManager;
        /// <summary>
        /// Get a reference to the primary storage controller for the application.
        /// </summary>
        public StorageController DataManager
        {
            get => _dataManager;
            private set
            {
                _dataManager = value;
            }
        }
        #endregion

        // Shared Contexts
        #region ActivityContext (Generic)
        ActivityViewModel _activityContext;
        /// <summary>
        /// Gets a reference to the ActivityContext.
        /// </summary>
        internal ActivityViewModel ActivityContext
        {
            get => _activityContext;
            private set
            {
                _activityContext = value;
            }
        }
        #endregion


    }
}
