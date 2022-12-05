using Autologin.Storage;
using Autologin.ViewModels;
using System.Windows;

namespace Autologin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            DataManager = new StorageController();
            ActivityContext = new ActivityViewModel();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            DataManager.FinalSave();
        }

        // DataManager

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

        // Shared Contexts

        private ActivityViewModel _activityContext;

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
    }
}