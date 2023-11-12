using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RussianPost.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Globalization;

namespace RussianPost.Helpers
{
    class API
    {
        public async Task<Package> getPackageByID(string TrackID)
        {
            Windows.Web.Http.HttpClient clientOb = new Windows.Web.Http.HttpClient();
            Uri connectionUrl = new Uri("https://www.pochta.ru/api/tracking/api/v1/trackings/by-barcodes");
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            pairs.Add("language", "ru");
            pairs.Add("track-numbers", TrackID);
            Windows.Web.Http.HttpFormUrlEncodedContent formContent = new Windows.Web.Http.HttpFormUrlEncodedContent(pairs);
            try
            {
                Windows.Web.Http.HttpResponseMessage response = await clientOb.GetAsync(new Uri(connectionUrl + "?" + formContent));
                if (response.IsSuccessStatusCode)
                {
                    var buffer = await response.Content.ReadAsBufferAsync();
                    var byteArray = buffer.ToArray();
                    var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                    var result = JsonConvert.DeserializeObject<JToken>(responseString);
                    //  var dialog = new MessageDialog("Имя: " + result["detailedTrackings"][0]["trackingItem"]["title"] + "\n Статус: " + result["detailedTrackings"][0]["trackingItem"]["commonStatus"]);

                    // Чтобы лишний раз не усложнять код
                    var jsonPackageItem = result["detailedTrackings"][0]["trackingItem"];

                    var Package = new Package();
                    Package.ID = jsonPackageItem["barcode"].ToString();
                    Package.Name = jsonPackageItem["title"].ToString();
                    Package.CommonStatus = jsonPackageItem["commonStatus"].ToString();

                    if (jsonPackageItem["globalStatus"].ToString() == "ARCHIVED")
                    {
                        Package.Status = 2;
                    }
                    else if (jsonPackageItem["globalStatus"].ToString() == "ARRIVED")
                    {
                        Package.Status = 1;
                    }
                    else
                    {
                        Package.Status = 0;
                    }

                    string additionalPackageInfo = jsonPackageItem["mailCtgText"].ToString();
                    string weight = "";
                    if (jsonPackageItem["weight"] == null)
                    {
                        weight = "";
                    }
                    else if (Convert.ToInt32(jsonPackageItem["weight"].ToString()) < 950)
                    {
                        weight = string.Format(" · {0} г.", jsonPackageItem["weight"]);
                    }
                    else if (Convert.ToInt32(jsonPackageItem["weight"].ToString()) > 950)
                    {
                        int w = Convert.ToInt32(jsonPackageItem["weight"].ToString()) * 1000;
                        weight = string.Format(" · {0} кг.", w);
                    }

                    string packageType = "Посылка " + additionalPackageInfo + weight;

                    Package.Description = packageType + "\n" +
                        "От кого: " + jsonPackageItem["sender"].ToString() + "\n" +
                        "Кому: " + jsonPackageItem["recipient"].ToString() + "\n" +
                        "Куда: " + jsonPackageItem["indexTo"] + ", " + jsonPackageItem["destinationCityName"];

                    Package.Weight = Convert.ToUInt32(jsonPackageItem["weight"].ToString());
                    Package.From = jsonPackageItem["sender"].ToString();
                    Package.To = jsonPackageItem["recipient"].ToString();
                    Package.ToCity = jsonPackageItem["indexTo"] + ", " + jsonPackageItem["destinationCityName"];

                    foreach (JToken place in jsonPackageItem["trackingHistoryItemList"])
                    {
                        var PlaceTracking = new MovingHistory();
                        PlaceTracking.Status = place["humanStatus"].ToString();
                        PlaceTracking.Place = place["description"].ToString();

                        string formatter = "dd.MM.yyyy H:mm:ss";
                        string dateString = place["date"].ToString();
                        DateTime date = DateTime.ParseExact(dateString, formatter, CultureInfo.InvariantCulture);

                        DateTime currentDate = DateTime.Now;
                        CultureInfo ruCulture = new CultureInfo("ru");
                        DateTimeFormatInfo ruDateTimeFormat = new DateTimeFormatInfo();
                        ruDateTimeFormat.ShortDatePattern = "d MMMM, HH:mm";
                        ruDateTimeFormat.LongDatePattern = "d MMMM yyyy г., HH:mm";
                        ruCulture.DateTimeFormat = ruDateTimeFormat;

                        string humanReadableDate = "";
                        if (currentDate.Year == date.Year)
                        {
                            humanReadableDate = date.ToString("d MMMM, HH:mm", ruCulture);
                        }
                        else
                        {
                            humanReadableDate = date.ToString("d MMMM yyyy г., HH:mm", ruCulture);
                        }

                        PlaceTracking.Date = humanReadableDate;

                        Package.History.Add(PlaceTracking);
                    }

                    return Package;
                }
                else
                {
                    throw new Exception("Посылка не найдена");
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw new Exception(ex.Message);
            }
        }
    }
}
