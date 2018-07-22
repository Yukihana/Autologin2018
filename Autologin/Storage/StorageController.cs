namespace Autologin.Storage
{
    using Autologin.Extensions;
    #region Includes

    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Timers;
    using System.Xml.Serialization;

    #endregion

    public class StorageController
    {
        #region Constructor(s)

        /// <summary>
        /// Initialises an instance of the StorageController class.
        /// </summary>
        public StorageController()
        {
            DataCache = Load();
        }

        #endregion

        #region DataModel
        /// <summary>
        /// The Actual Data Structure
        /// </summary>
        private StorageModel DataCache;
        #endregion

        #region Public Interface

        /// <summary>
        /// Gets the value of a storage property.
        /// </summary>
        /// <param name="PropertyName">Name of the property</param>
        /// <returns>Data contents of the property</returns>
        public object GetData(string PropertyName)
        {
            var p = DataCache.GetType().GetProperty(PropertyName);
            return p?.GetValue(DataCache, null);
        }

        /// <summary>
        /// Sets the value of a storage property.
        /// </summary>
        /// <param name="PropertyName">Name of the property</param>
        /// <param name="Value">Data contents of the property</param>
        /// <returns>True on success</returns>
        public bool SetData(string PropertyName, object Value)
        {
            var p = DataCache.GetType().GetProperty(PropertyName);
            if (p != null)
            {
                p.SetValue(DataCache, Value, null);
                InitiateSave();
                return true;
            }
            return false;
        }

        /// <summary>
        /// A Synchronous call to Save, run on the primary thread to make sure save completes before exiting the application.
        /// </summary>
        public void FinalSave()
        {
            Save(DataCache);
        }

        #endregion

        #region Autonomous Save Logic
        Timer AutosaveTimer = null;
        BackgroundWorker BGWriter = null;
        double SaveDelay = 60000;
        /// <summary>
        /// Gracefully delays the Save operation everytime it is invoked, to disarm frequent write attempts.
        /// </summary>
        private void InitiateSave()
        {
            // One-Time Assignment
            if(AutosaveTimer==null)
            {
                AutosaveTimer = new Timer();
                AutosaveTimer.Elapsed += AutosaveTimer_Elapsed;
            }

            // Reset Timer
            if(AutosaveTimer.Enabled)
            {
                AutosaveTimer.Stop();
            }
            AutosaveTimer.Interval = SaveDelay;

            // Start Timer
            AutosaveTimer.Start();
        }
        /// <summary>
        /// When the timer has elapsed, this starts the Save operation in a BackgroundWorker.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutosaveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // One-Time Assignment
            if (BGWriter == null)
            {
                BGWriter = new BackgroundWorker();
                BGWriter.DoWork += WriteToDiskAsync;
            }

            // Return if the worker is busy
            if(BGWriter.IsBusy)
            {
                return;
            }

            // Start saving on a separate thread
            BGWriter.RunWorkerAsync(DataCache);
        }

        private void WriteToDiskAsync(object sender, DoWorkEventArgs e)
        {
            // Make a deepclone so changes don't interfere with threading
            var WriteData = SerializationExtensions.DeepClone(e.Argument as StorageModel);

            // Write to disk
            Save(WriteData);
        }

        #endregion

        #region Load / Save : Private

        /// <summary>
        /// Loads storage data from the disk.
        /// </summary>
        private StorageModel Load()
        {
            // Prepare
            string StorePath = GetStorePath();

            if (File.Exists(StorePath))
            {
                // Load
                using (MemoryStream m = new MemoryStream())
                {
                    // Read into memory
                    using (FileStream fs = File.OpenRead(StorePath))
                    {
                        fs.CopyTo(m);
                    }

                    // Parse as Config
                    XmlSerializer x = new XmlSerializer(typeof(StorageModel));

                    m.Position = 0;
                    try
                    {
                        return x.Deserialize(m) as StorageModel;
                    }
                    catch (Exception)
                    {
                        //Coordinator.Notify(new NotificationObject(e.Message, e.Source, MessageBoxImage.Error)); TODO
                    }
                }
            }

            // Failsafe Default Assignment
            return new StorageModel();
        }

        /// <summary>
        /// Saves storage data to the disk.
        /// </summary>
        private void Save(StorageModel DataCache)
        {
            // Prepare
            string StorePath = GetStorePath();

            // Generate output file in memory-stream
            using (MemoryStream m = new MemoryStream())
            {
                // Attempt to render the MemoryStream
                XmlSerializer x = new XmlSerializer(typeof(StorageModel));

                try
                {
                    x.Serialize(m, DataCache);
                }
                catch (Exception)
                {
                    m.Position = 0;
                    x.Serialize(m, new StorageModel());
                }

                // Attempt to render the File
                try
                {
                    using (FileStream fs = new FileStream(StorePath, FileMode.Create, FileAccess.Write))
                    {
                        m.WriteTo(fs);
                    }
                }
                catch (Exception)
                {
                    //Coordinator.Notify(new NotificationObject(e.Message, e.Source, MessageBoxImage.Error)); TODO
                }
            }
        }

        #endregion

        #region Miscellaneous

        /// <summary>
        /// Gets the specified or default path to the storage file on the secondary storage media.
        /// </summary>
        /// <returns>The absolute path to the file</returns>
        public static string GetStorePath()
        {
            string r = "Config.xml";
            string ArgName = "DataStore=";

            // Check if startup parameter was passed
            foreach (string a in Environment.GetCommandLineArgs())
            {
                if (a.StartsWith(ArgName, StringComparison.OrdinalIgnoreCase))
                {
                    r = a.Substring(ArgName.Length - 1);
                }
            }

            // Set full path address
            if (!Path.IsPathRooted(r))
            {
                r = Path.GetFullPath(r);
            }

            return r;
        }

        #endregion
    }
}
