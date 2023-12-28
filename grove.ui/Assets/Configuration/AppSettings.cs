using Newtonsoft.Json;
using UnityEngine;

namespace Configuration
{
    public class AppSettings
    {
        [System.Serializable]
        public class ConfigData
        {
            public string DEV_GROVE_API_URL { get; set; }
        }

        private ConfigData configData;

        public AppSettings()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            string filePath = System.IO.Path.Combine(Application.dataPath, "../appsettings.json");
            if (System.IO.File.Exists(filePath))
            {
                string jsonContents = System.IO.File.ReadAllText(filePath);
                configData = JsonConvert.DeserializeObject<ConfigData>(jsonContents);
            }
            else
            {
                Debug.LogError("Config file not found");
            }
        }

        public string DEV_GROVE_API_URL()
        {
            return configData?.DEV_GROVE_API_URL;
        }
    }
}