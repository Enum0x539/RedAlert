using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace RedAlert
{
    public class RedAlert
    {
        public static string CitiesJson = "";
        public static List<AlertCityData> alertCities = new List<AlertCityData>();
        private static byte[] bArray = new byte[57]{ 0x75, 0x67, 0x67, 0x63, 0x66, 0x3a, 0x2f, 0x2f, 0x6a, 0x6a, 0x6a, 0x2e, 0x62, 0x65, 0x72, 0x73, 0x2e, 0x62, 0x65, 0x74, 0x2e, 0x76, 0x79, 0x2f, 0x4a, 0x6e, 0x65, 0x61, 0x76, 0x61, 0x74, 0x5a, 0x72, 0x66, 0x66, 0x6e, 0x74, 0x72, 0x66, 0x2f, 0x4e, 0x79, 0x72, 0x65, 0x67, 0x2f, 0x6e, 0x79, 0x72, 0x65, 0x67, 0x66, 0x2e, 0x77, 0x66, 0x62, 0x61 };

        public delegate void AlertReceivedEventHandler(List<AlertCityData> cities);
        public static event AlertReceivedEventHandler OnAlertReceived;

        public static void Setup(AlertReceivedEventHandler eventHandler)
        {
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Functions.Foo("\\pvgvrf.wfba")))
                Functions.DownloadFile(Functions.Foo("uggcf://pqa.qvfpbeqncc.pbz/nggnpuzragf/825091638782459912/843166842016366612/pvgvrf.wfba"), Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Functions.Foo("\\pvgvrf.wfba"));

            CitiesJson = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Functions.Foo("\\pvgvrf.wfba"));

            JArray jArray = JArray.Parse(CitiesJson);
            foreach (JObject jObj in jArray)
                alertCities.Add(new AlertCityData(jObj));

            OnAlertReceived = eventHandler;
            InitializeListener();
        }

        private static void InitializeListener()
        {
            Thread thread = new Thread(() =>
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

                string lastMatah = "";
                while (true)
                {
                    Thread.Sleep(100);

                    Leaf.xNet.HttpResponse response = null;
                    using (Leaf.xNet.HttpRequest httpRequest = new Leaf.xNet.HttpRequest())
                    {
                        try
                        {
                            httpRequest.AddHeader(Functions.Foo("K-Erdhrfgrq-Jvgu"), Functions.Foo("KZYUggcErdhrfg"));
                            httpRequest.AddHeader(Functions.Foo("Ersrere"), Functions.Foo("https://www.oref.org.il/"));
                            response = httpRequest.Get(Functions.Foo(Encoding.Default.GetString(bArray)));
                        }
                        catch { continue; }

                        if (String.IsNullOrEmpty(response.ToString()))
                            continue;

                        if (response.StatusCode != Leaf.xNet.HttpStatusCode.OK)
                            continue;

                        JObject jObject = JObject.Parse(Encoding.UTF8.GetString(Encoding.Default.GetBytes(response.ToString())));
                        if (jObject["title"] == null || jObject["title"].ToString() != "התרעות פיקוד העורף")
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
                            RedAlert.OnAlertReceived(matahCities);
                        });
                        callback.Start();
                    }
                }
            });
            thread.Start();
        }
    }

    class Functions
    {
        public static void DownloadFile(string link, string path)
        {
            WebClient wb = new WebClient();
            wb.Headers.Add("User-Agent", "TyarZHDSbUdJVoHhvvcw6dUa8bQkURMXAhj9pxChX4J68Lt2a98gP625uDJA");
            wb.DownloadFile(link, path);
        }

        public static string Foo(string input)
        {
            return !string.IsNullOrEmpty(input) ? new string(input.Select(x => (x >= 'a' && x <= 'z') ? (char)((x - 'a' + 13) % 26 + 'a') : ((x >= 'A' && x <= 'Z') ? (char)((x - 'A' + 13) % 26 + 'A') : x)).ToArray()) : input;
        }
    }

    public class AlertCityData
    {
        public string Name_he = "";
        public string Name_en = "";
        public string Name_ru = "";
        public string Name_ar = "";

        public string Zone_he = "";
        public string Zone_en = "";
        public string Zone_ru = "";
        public string Zone_ar = "";

        public int Countdown = 0;
        public string Time_he = "";
        public string Time_en = "";
        public string Time_ru = "";
        public string Time_ar = "";

        public double Latitude = 0;
        public double Longitude = 0;

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

            this.Latitude = double.Parse(jObject["lat"].ToString());
            this.Longitude = double.Parse(jObject["lng"].ToString());
        }
    }
}
