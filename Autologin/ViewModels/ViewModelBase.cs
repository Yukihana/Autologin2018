using Autologin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Autologin.ViewModels
{
    class ViewModelBase
    {
        #region DataIdentifier
        private string _dataIdentifier = string.Empty;
        /// <summary>
        /// Storage identifier for the model data
        /// </summary>
        protected string DataIdentifier
        {
            get => _dataIdentifier;
            set
            {
                _dataIdentifier = value;
            }
        }
        #endregion
        #region SaveLock
        private bool _saveLock = false;
        /// <summary>
        /// Flag to prevent saving during loading data.
        /// </summary>
        public bool SaveLock
        {
            get => _saveLock;
            set
            {
                _saveLock = value;
            }
        }
        #endregion

        #region Load and Save
        /// <summary>
        /// Loads data from the datacache.
        /// </summary>
        protected static void LoadData<T,X>(T ThisViewModel, X DefVal) where T: ViewModelBase where X: ModelBase
        {
            X r = default(X);

            // Try to retrieve the data from the storage controller
            if (Application.Current is App a)
            {
                r = (X)a.DataManager.GetData(ThisViewModel.DataIdentifier);
            }

            // If r is null, apply default value
            if (r == null)
            {
                r = DefVal;
            }

            // Apply changes to data
            ThisViewModel.SaveLock = true;   // Disable automatic save-on-change
            ThisViewModel.GetType().GetProperty("DataModel")?.SetValue(ThisViewModel, r, null);
            ThisViewModel.SaveLock = false;  // Enable automatic save-on-change
        }

        /// <summary>
        /// Saves data to the datacache.
        /// </summary>
        protected static void SaveData<T, X>(T ThisViewModel, X DefVal) where T : ViewModelBase where X : ModelBase
        {
            if (Application.Current is App a && ThisViewModel.SaveLock != true)
            {
                a.DataManager.SetData(
                    ThisViewModel.DataIdentifier,
                    ThisViewModel.GetType().GetProperty("DataModel")?.GetValue(ThisViewModel, null)
                    );
            }
        }

        #endregion
    }
}
