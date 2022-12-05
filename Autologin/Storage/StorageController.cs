using Autologin.Extensions;
using System;
using System.ComponentModel;
using System.IO;
using System.Timers;
using System.Xml.Serialization;

namespace Autologin.Storage
{
    public class StorageController
    {
        // Infrastructure

        private StorageModel _dataCache;

        //Lifecycle

        public StorageController()
        {
            _dataCache = Load();
        }

        // API

        /// <summary>
        /// Gets the value of a storage property.
        /// </summary>
        /// <param name="PropertyName">Name of the property</param>
        /// <returns>Data contents of the property</returns>
        public object GetData(string PropertyName)
        {
            var p = _dataCache.GetType().GetProperty(PropertyName);
            return p?.GetValue(_dataCache, null);
        }

        /// <summary>
        /// Sets the value of a storage property.
        /// </summary>
        /// <param name="PropertyName">Name of the property</param>
        /// <param name="Value">Data contents of the property</param>
        /// <returns>True on success</returns>
        public bool SetData(string PropertyName, object Value)
        {
            var p = _dataCache.GetType().GetProperty(PropertyName);
            if (p != null)
            {
                p.SetValue(_dataCache, Value, null);
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
            Save(_dataCache);
        }

        // Autosave

        private Timer AutosaveTimer = null;
        private BackgroundWorker BGWriter = null;
        private double SaveDelay = 60000;

        /// <summary>
        /// Gracefully delays the Save operation everytime it is invoked, to disarm frequent write attempts.
        /// </summary>
        private void InitiateSave()
        {
            // One-Time Assignment
            if (AutosaveTimer == null)
            {
                AutosaveTimer = new Timer();
                AutosaveTimer.Elapsed += AutosaveTimer_Elapsed;
            }

            // Reset Timer
            if (AutosaveTimer.Enabled)
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
            if (BGWriter.IsBusy)
            {
                return;
            }

            // Start saving on a separate thread
            BGWriter.RunWorkerAsync(_dataCache);
        }

        private void WriteToDiskAsync(object sender, DoWorkEventArgs e)
        {
            // Make a deepclone so changes don't interfere with threading
            var WriteData = SerializationExtensions.DeepClone(e.Argument as StorageModel);

            // Write to disk
            Save(WriteData);
        }

        // Load / Save

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
                catch
                {
                    //Coordinator.Notify(new NotificationObject(e.Message, e.Source, MessageBoxImage.Error)); TODO
                }
            }
        }

        // Miscellaneous

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
                    r = a.Substring(ArgName.Length - 1);
            }

            // Set full path address
            if (!Path.IsPathRooted(r))
                r = Path.GetFullPath(r);

            return r;
        }
    }
}