using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Autologin.Extensions
{
    public static class SerializationExtensions
    {
        /// <summary>
        /// Deep-Clones an object using Serialization.
        /// Supposedly performance heavy, but a very effective general purpose solution.
        /// </summary>
        /// <typeparam name="T">Type must implement [Serializable]</typeparam>
        /// <param name="obj">The object to be deep-cloned</param>
        /// <returns></returns>
        public static T DeepClone<T>(this T obj)
        {
            if (typeof(T).IsSerializable)
            {
                using (var ms = new MemoryStream())
                {
                    var serializer = new BinaryFormatter();
                    serializer.Serialize(ms, obj);
                    ms.Position = 0;

                    return (T)serializer.Deserialize(ms);
                }
            }
            else
            {
                throw new Exception("The provided type is not serializable.");
            }
        }
    }
}