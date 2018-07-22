namespace Autologin.Storage
{
    #region Includes
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml.Serialization;
    using Autologin.Models;
    #endregion

    [Serializable, XmlRoot("AutologinData")]
    public class StorageModel : ModelBase
    {
        #region Constructor(s)
        public StorageModel()
        {
            ActivityData = new ActivityModel();
        }
        #endregion

        #region ActivityModel (Generic)
        private ActivityModel _activityData;
        public ActivityModel ActivityData
        {
            get => _activityData;
            set
            {
                _activityData = value;
                OnPropertyChanged("ActivityData");
            }
        }
        #endregion
    }
}