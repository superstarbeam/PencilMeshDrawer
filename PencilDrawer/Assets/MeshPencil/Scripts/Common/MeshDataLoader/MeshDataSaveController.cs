using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace MeshPencil.Common.MeshDataLoader
{
    public static class MeshDataSaveController
    {
        private const string _fileExtension = ".dpd";

        /// <summary>
        /// Save painted points array data
        /// </summary>
        /// <param name="data">painted points array</param>
        /// <param name="subfolderName">subfolder in persistentData</param>
        /// <param name="fileName">file name</param>
        public static void SaveData(byte[,] data, string subfolderName, string fileName)
        {
            (string toSubfolder, string toFile) path = GetPath(subfolderName, fileName);

            if (!Directory.Exists(path.toSubfolder))
            {
                Debug.LogWarning("Mesh Loader : current path was not founded \nCreating new folder...");
                Directory.CreateDirectory(path.toSubfolder);

                SaveData(data, subfolderName, fileName);
            }

            if (data == null)
            {
                Debug.LogWarning("Saved data is null");
                return;
            }

            Serialize(data, path.toFile);
        }

        /// <summary>
        /// Load painted points array data
        /// </summary>
        /// <param name="subfolderName">subfolder in persistentData</param>
        /// <param name="fileName">file name</param>
        public static byte[,] LoadData(string subfolderName, string fileName)
        {
            (string toSubfolder, string toFile) path = GetPath(subfolderName, fileName);

            if (!File.Exists(path.toFile))
            {
                Debug.LogError("Current file is not exists , file path :\n" + path.toFile); 
                return null;
            }

            byte[,] loadedData = (byte[,]) Deserialize(path.toFile);

            return loadedData;
        }

        private static void Serialize(object objectToSerialize, string path)
        {
            using (Stream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(stream, objectToSerialize);
            }
        }

        private static object Deserialize(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                return bformatter.Deserialize(stream);
            }
        }

        private static (string, string) GetPath(string subfolderName, string fileName)
        {
            string pathToSubfolder = Path.Combine(Application.persistentDataPath, subfolderName);
            string pathToFile = Path.Combine(Application.persistentDataPath, subfolderName, fileName += _fileExtension);

            return (pathToSubfolder, pathToFile);
        }
    }
}
