using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RussianPost.Classes;
using Newtonsoft.Json;

namespace RussianPost.Helpers
{
    /*
     * 
     * Добро пожаловать в мир костылей, приятель.
     * 
     * Так как мне лень разбираться в правильном хранении списка (а конкретно
     * сохранённых посылок), я тупо сохраняю всё это дело в сериализованный JSON
     * массив, ибо его НАМНОГО легче просто взять и спарсить, сохранить туда
     * своё говно и обратно запаковать в строку.
     * 
     */

    class Config
    {
        List<PackageMin> Packages;

        public void Init()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values["packages"] == null)
            {
                var PackagesList = new List<PackageMin>();
                string json = JsonConvert.SerializeObject(PackagesList);
                localSettings.Values["packages"] = json;
                Packages = PackagesList;
            } else
            {
                Packages = JsonConvert.DeserializeObject<List<PackageMin>>(localSettings.Values["packages"].ToString());
            }
        }

        public void ImportNewList(List<PackageMin> NewPackages)
        {
            Packages = NewPackages;
        }

        public void Save()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            string json = JsonConvert.SerializeObject(Packages);
            localSettings.Values["packages"] = json;
        }

        public List<PackageMin> GetPackages()
        {
            return Packages;
        }

        public void AddPackage(string ID, string Name, string LastStatus, bool ReadyToGet)
        {
            var Package = new PackageMin { ID = ID, Name = Name, LastState = LastStatus, ReadyToGet = ReadyToGet };

            // Проверяем, есть ли дубли
            var dublicate = false;
            foreach (PackageMin pkg in Packages)
                if (pkg.ID == ID)
                    dublicate = true;
            
            if (dublicate == false)
                Packages.Add(Package);
        }

        public void RemovePackage(string ID)
        {
            for (var i = 0; i < Packages.Count; i++)
            {
                if (Packages.ElementAt(i).ID == ID)
                {
                    Packages.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
