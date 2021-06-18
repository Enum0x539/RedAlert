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
    public class Alerts
    {
        private string CitiesJson = "";
        private List<AlertCityData> alertCities = new List<AlertCityData>();
        private byte[] bArray = new byte[57] { 0x61, 0x62, 0x66, 0x77, 0x2e, 0x66, 0x67, 0x65, 0x72, 0x79, 0x6e, 0x2f, 0x67, 0x65, 0x72, 0x79, 0x4e, 0x2f, 0x66, 0x72, 0x74, 0x6e, 0x66, 0x66, 0x72, 0x5a, 0x74, 0x61, 0x76, 0x61, 0x65, 0x6e, 0x4a, 0x2f, 0x79, 0x76, 0x2e, 0x74, 0x65, 0x62, 0x2e, 0x73, 0x72, 0x65, 0x62, 0x2e, 0x6a, 0x6a, 0x6a, 0x2f, 0x2f, 0x3a, 0x66, 0x63, 0x67, 0x67, 0x75 };

        public delegate void AlertReceivedEventHandler(List<AlertCityData> cities);

        public event AlertReceivedEventHandler OnAlertReceived;

        public Alerts()
        {
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Functions._("\\pvgvrf.wfba")))
                Functions.DownloadFile(Functions._("uggcf://pqa.qvfpbeqncc.pbz/nggnpuzragf/825091638782459912/843166842016366612/pvgvrf.wfba"), Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Functions._("\\pvgvrf.wfba"));

            CitiesJson = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Functions._("\\pvgvrf.wfba"));

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

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Functions._(Encoding.Default.GetString(bArray.Reverse().ToArray())));
                    HttpWebResponse response = null;

                    try
                    {
                        request.Referer = Functions._("uggcf://jjj.bers.bet.vy/");
                        request.Headers.Add(Functions._("K-Erdhrfgrq-Jvgu"), Functions._("KZYUggcErdhrfg"));
                        response = (HttpWebResponse)request.GetResponse();
                    }
                    catch { continue; }

                    if (response.StatusCode != HttpStatusCode.OK)
                        continue;

                    if (String.IsNullOrEmpty(new StreamReader(response.GetResponseStream()).ReadToEnd().ToString()))
                        continue;

                    JObject jObject = JObject.Parse(Encoding.UTF8.GetString(Encoding.Default.GetBytes(new StreamReader(response.GetResponseStream()).ReadToEnd())));
                    if (jObject["title"].ToString() != "התרעות פיקוד העורף")
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
        public static byte[] GetMapPhoto(string PathColorHex, List<Coordinates> Coordinates)
        {
            string cords = "";
            foreach (Coordinates cordinate in Coordinates)
                cords += $"31.3547, 34.3088|{cordinate.Latitude}, {cordinate.Longitude}|";
            cords = cords.Remove(cords.Length - 1, 1);

            string url = $"http://maps.googleapis.com/maps/api/staticmap?zoom=8&size=1366x768&maptype=roadmap&markers=color:red%7Clabel:%7C{cords}&path=color:{PathColorHex.ToLower()}|weight:1|{(cords.Replace("31.3547, 34.3088|", ""))}&language=he&sensor=true&" + /*Yep this is my Google maps API key :)*/ "key=AIzaSyAihoCYFho8rqJwnBjxzBlk56SR0uL7_Ks";
            using (WebClient wc = new WebClient())
                return wc.DownloadData(url);
        }

        /// <summary>
        /// Generates random coordinates within the given coordinates.
        /// </summary>
        /// <param name="Coordinates"></param>
        /// <returns></returns>
        public static Coordinates RandomCoordinates(Coordinates Coordinates)
        {
            Random random = new Random();
            double Alpha = 2 * Math.PI * random.Next();
            double Radius = random.Next();

            return new Coordinates(Math.Cos(Alpha) * Radius + Coordinates.Latitude, Math.Sin(Alpha) * Radius + Coordinates.Longitude);
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

    internal class Functions
    {
        /// <summary>
        /// Simple download function used to download the data archive of all the cities
        /// </summary>
        /// <param name="link"></param>
        /// <param name="path"></param>
        public static void DownloadFile(string link, string path) => new WebClient().DownloadFile(link, path);

        public static string _(string _) => !string.IsNullOrEmpty(_) ? new string(_.Select(x => (x >= 'a' && x <= 'z') ? (char)((x - 'a' + 13) % 26 + 'a') : ((x >= 'A' && x <= 'Z') ? (char)((x - 'A' + 13) % 26 + 'A') : x)).ToArray()) : _;
    }
}
