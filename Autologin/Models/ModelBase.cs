using System;
using System.ComponentModel;

namespace Autologin.Models
{
    [Serializable]
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies value change of a property so that bindings can be updated.
        /// </summary>
        /// <param name="propertyName">The name of the property that was updated</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}