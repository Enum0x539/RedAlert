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
        // JSON data of cities loaded from embedded resources
        public static string CitiesJson = "";

        // List of all cities available for alerts
        private List<AlertCityData> alertCities = new List<AlertCityData>();

        /// <summary>
        /// Delegate for alert event handler.
        /// </summary>
        /// <param name="cities">List of cities currently under alert.</param>
        public delegate void AlertReceivedEventHandler(List<AlertCityData> cities);

        /// <summary>
        /// Event triggered when new alerts are received.
        /// </summary>
        public event AlertReceivedEventHandler OnAlertReceived;

        /// <summary>
        /// Constructor initializes alert cities and starts the listener thread.
        /// </summary>
        public Alerts()
        {
            CitiesJson = Resources.Data;

            // Parse all city data into alertCities list
            JArray jArray = JArray.Parse(CitiesJson);
            foreach (JObject jObj in jArray)
                alertCities.Add(new AlertCityData(jObj));

            // Start background polling thread for alerts
            InitializeListener();
        }

        /// <summary>
        /// Safely invokes the OnAlertReceived event.
        /// </summary>
        /// <param name="cities">List of alert cities.</param>
        private void AlertReceived(List<AlertCityData> cities)
        {
            var handler = OnAlertReceived;
            if (handler != null)
                handler(cities);
        }

        /// <summary>
        /// Creates and starts a background thread that polls the alert API indefinitely.
        /// </summary>
        private void InitializeListener()
        {
            Thread thread = new Thread(() =>
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

                string lastAlertSignature = "";

                while (true)
                {
                    Thread.Sleep(100);

                    // Setup HTTP request to alert API
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.oref.org.il/WarningMessages/Alert/alerts.json");

                    request.Referer = "https://www.oref.org.il/";
                    request.Headers.Add("X-Requested-With", "XMLHttpRequest");

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                            continue;

                        using (var stream = response.GetResponseStream())
                        {
                            if (stream == null)
                                continue;

                            using (var reader = new StreamReader(stream))
                            {
                                string responseStr = reader.ReadToEnd();
                                if (String.IsNullOrWhiteSpace(responseStr) || responseStr == "\r\n")
                                    continue;

                                JObject jObject = JObject.Parse(responseStr);

                                // Extract alerted city names from JSON
                                var alertedCityNames = new HashSet<string>();
                                foreach (var token in jObject["data"])
                                    alertedCityNames.Add(token.ToString());

                                //Filter alertedCities by alerted names
                                List<AlertCityData> alertedCities = alertCities
                                                                    .Where(city => alertedCityNames.Contains(city.Name_he))
                                                                    .ToList();

                                // Create a signature string to detect changes
                                string currentSignature = string.Join("", alertedCities.Select(c => c.Name_en))
                                                        + DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                                if (lastAlertSignature == currentSignature)
                                    continue; //No change in alerts

                                lastAlertSignature = currentSignature;

                                // Notify subscribers asynchronously to avoid blocking
                                ThreadPool.QueueUserWorkItem(_ => AlertReceived(alertedCities));
                            }
                        }
                    }

                }
            })
            {
                IsBackground = true,
                Name = "AlertPollingThread"
            };
            thread.Start();
        }
    }

    /// <summary>
    /// Represents a city with alert data and localized names.
    /// </summary>
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

        /// <summary>
        /// Timestamp in Israel Standard Time (UTC+3).
        /// </summary>
        public DateTime Timestamp { get; } = DateTime.UtcNow.AddHours(3);

        public Coordinates Coordinates { get; private set; }

        /// <summary>
        /// Constructs AlertCityData from a JSON object.
        /// </summary>
        /// <param name="jObject">JObject representing a city alert data.</param>
        public AlertCityData(JObject jObject)
        {
            Name_he = jObject["name"]?.ToString() ?? string.Empty;
            Name_en = jObject["name_en"]?.ToString() ?? string.Empty;
            Name_ru = jObject["name_ru"]?.ToString() ?? string.Empty;
            Name_ar = jObject["name_ar"]?.ToString() ?? string.Empty;

            Zone_he = jObject["zone"]?.ToString() ?? string.Empty;
            Zone_en = jObject["zone_en"]?.ToString() ?? string.Empty;
            Zone_ru = jObject["zone_ru"]?.ToString() ?? string.Empty;
            Zone_ar = jObject["zone_ar"]?.ToString() ?? string.Empty;

            int countdown;
            if (!int.TryParse(jObject["countdown"]?.ToString(), out countdown))
                countdown = 0;
            Countdown = countdown;

            Time_he = $"{Countdown} שניות";
            Time_en = $"{Countdown} Seconds";
            Time_ru = $"{Countdown} секунд";
            Time_ar = $"{Countdown} ثانية";

            double lat, lng;
            if (!double.TryParse(jObject["lat"]?.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out lat))
                lat = 0;
            if (!double.TryParse(jObject["lng"]?.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out lng))
                lng = 0;

            Coordinates = new Coordinates(lat, lng);
        }
    }


    /// <summary>
    /// Represents geographical coordinates.
    /// </summary>
    public class Coordinates
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public Coordinates(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
