using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace RussianPost.Common
{
    class ConfigurationManager
    {
        public static ConfigurationManager Instance;
        
        private ApplicationDataContainer container;

        private ConfigurationManager()
        {
            container = ApplicationData.Current.LocalSettings;
        }

        public static void Initialize()
        {
            Instance = new ConfigurationManager();
        }

        public void SetParameter<T>(string key, T value)
        {
            container.Values[key] = value;
        }

        public T GetParameter<T>(string key)
        {
            return (T)container.Values[key];
        }
    }
}
