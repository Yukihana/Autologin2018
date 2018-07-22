namespace Autologin.Models
{
    #region Includes
    using System;
    using System.ComponentModel;
    #endregion

    [Serializable]
    public class ModelBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies value change of a property so that bindings can be updated.
        /// </summary>
        /// <param name="propertyName">The name of the property that was updated</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
