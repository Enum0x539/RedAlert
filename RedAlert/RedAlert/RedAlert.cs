using Newtonsoft.Json.Linq;
using RedAlert.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace RedAlert
{
    public class Alerts
    {
        public static string CitiesJson = "";
        private List<AlertCityData> alertCities = new List<AlertCityData>();

        public delegate void AlertReceivedEventHandler(List<AlertCityData> cities);

        public event AlertReceivedEventHandler OnAlertReceived;

        public Alerts()
        {
            CitiesJson = Resources.Data;

            JArray jArray = JArray.Parse(CitiesJson);
            foreach (JObject jObj in jArray)
                alertCities.Add(new AlertCityData(jObj));

            InitializeListener();
        }

        private void AlertReceived(List<AlertCityData> cities)
        {
            if (OnAlertReceived != null)
                OnAlertReceived(cities);
        }

        private void InitializeListener()
        {
            Thread thread = new Thread(() =>
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

                string lastMatah = "";

                while (true)
                {
                    Thread.Sleep(100);

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.oref.org.il/WarningMessages/Alert/alerts.json");
                    HttpWebResponse response = null;

                    try
                    {
                        request.Referer = "https://www.oref.org.il/";
                        request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                        response = (HttpWebResponse)request.GetResponse();
                    }
                    catch { continue; }

                    if (response.StatusCode != HttpStatusCode.OK)
                        continue;

                    string responseStr = new StreamReader(response.GetResponseStream()).ReadToEnd().ToString();
                    if (String.IsNullOrEmpty(responseStr) || responseStr == "\r\n")
                        continue;

                    JObject jObject = JObject.Parse(responseStr);
                    if (jObject["title"].ToString() != "ירי רקטות וטילים")
                        continue;

                    List<AlertCityData> matahCities = new List<AlertCityData>();
                    foreach (string city in jObject["data"])
                    {
                        foreach (AlertCityData alertCity in alertCities)
                        {
                            if (alertCity.Name_he != city)
                                continue;

                            matahCities.Add(alertCity);
                            break;
                        }
                    }

                    List<string> currectAlerts = new List<string>();
                    for (int i = 0; i < matahCities.Count; ++i)
                    {
                        if (i == matahCities.Count)
                        {
                            currectAlerts.Add(matahCities[i].Name_en);
                            currectAlerts.Add(DateTimeOffset.Now.ToUnixTimeSeconds().ToString());
                            break;
                        }

                        currectAlerts.Add(matahCities[i].Name_en);
                    }
                    string current = string.Join("", currectAlerts);
                    if (lastMatah == current)
                        continue;

                    lastMatah = current;
                    Thread callback = new Thread(() =>
                    {
                        AlertReceived(matahCities);
                    });
                    callback.Start();
                }
            });
            thread.Start();
        }

        /// <summary>
        /// Retrieves the bytes of the given coordinates image from Google Maps API with a path-line from Gaza-strip.
        /// </summary>
        /// <param name="PathColorHex"></param>
        /// <param name="Coordinates"></param>
        /// <returns></returns>
        public static Bitmap GetMapPhoto(List<AlertCityData> cities)
        {
            string cords = string.Empty;
            cities.Select(c => c.Coordinates).ToList().ForEach(cord => { cords += $"{cord.Latitude}, {cord.Longitude}|"; });
            cords = cords.Remove(cords.Length - 1, 1);

            string url = $"http://maps.googleapis.com/maps/api/staticmap?center={cities[0].Name_en}&zoom=11&size=640x533&maptype=roadmap&markers=color:red%7Clabel:S%{cords}&language=he&sensor=true&" + /*Yep this is my Google maps API key :)*/ "key=AIzaSyAihoCYFho8rqJwnBjxzBlk56SR0uL7_Ks";
            using (WebClient wc = new WebClient())
            using (var ms = new MemoryStream(wc.DownloadData(url)))
                return new Bitmap(ms);
        }
    }

    public class AlertCityData
    {
        public string Name_he { get; private set; }
        public string Name_en { get; private set; }
        public string Name_ru { get; private set; }
        public string Name_ar { get; private set; }

        public string Zone_he { get; private set; }
        public string Zone_en { get; private set; }
        public string Zone_ru { get; private set; }
        public string Zone_ar { get; private set; }

        public int Countdown { get; private set; }
        public string Time_he { get; private set; }
        public string Time_en { get; private set; }
        public string Time_ru { get; private set; }
        public string Time_ar { get; private set; }

        public DateTime Timestamp { get; } = DateTime.UtcNow.AddHours(3); //"Israel Standard Time" Timestamp.

        public Coordinates Coordinates { get; private set; }

        public AlertCityData(JObject jObject)
        {
            this.Name_he = jObject["name"].ToString();
            this.Name_en = jObject["name_en"].ToString();
            this.Name_ru = jObject["name_ru"].ToString();
            this.Name_ar = jObject["name_ar"].ToString();

            this.Zone_he = jObject["zone"].ToString();
            this.Zone_en = jObject["zone_en"].ToString();
            this.Zone_ru = jObject["zone_ru"].ToString();
            this.Zone_ar = jObject["zone_ar"].ToString();

            this.Countdown = int.Parse(jObject["countdown"].ToString());
            this.Time_he = $"{this.Countdown} שניות";
            this.Time_en = $"{this.Countdown} Seconds";
            this.Time_ru = $"{this.Countdown} секунд";
            this.Time_ar = $"{this.Countdown} ثانية";

            this.Coordinates = new Coordinates(double.Parse(jObject["lat"].ToString()), double.Parse(jObject["lng"].ToString()));
        }
    }

    public class Coordinates
    {
        public double Latitude, Longitude;

        public Coordinates(double Latitude, double Longitude)
        {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
        }
    }
}
