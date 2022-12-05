using Autologin.Models;
using System;
using System.Xml.Serialization;

namespace Autologin.Storage
{
    [Serializable, XmlRoot("AutologinData")]
    public class StorageModel : ModelBase
    {
        // Infrastructure

        private ActivityModel _activityData;

        // Lifecycle

        public StorageModel()
        {
            ActivityData = new ActivityModel();
        }

        // API

        public ActivityModel ActivityData
        {
            get => _activityData;
            set
            {
                _activityData = value;
                OnPropertyChanged("ActivityData");
            }
        }
    }
}