# Description
Real-time rocket alert integration with Israel Home Front Command’s public API (Pikud HaOref).
Because waiting for rocket sirens is so last century… 💥🚀

## About the library
RedAlert is a C# library that continuously syncs with Israel Home Front Command’s live alert API to fetch rocket and other emergency alerts across Israel’s territory — not just Gaza-strip launches.
It offers an easy-to-use event-driven interface with multi-language support and handy mapping capabilities to visualize affected zones.

## Features
🚨 Real-time retrieval of Israel Home Front Command alerts

🌍 Supports multiple launch zones, not limited to Gaza — because rockets love to surprise

📍 Provides detailed location info (coordinates, city names, alert zones)

🌐 Localization for Hebrew, Arabic, Russian, and English

🔄 Runs a lightweight background thread for continuous updates and event notifications

## How to Use
```cs
using RedAlert;
using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        var alerts = new Alerts();
        alerts.OnAlertReceived += Alerts_OnAlertReceived;

        Console.WriteLine("Listening for alerts... Stay safe out there! 🚦");
        Console.ReadKey();
    }

    private static void Alerts_OnAlertReceived(List<AlertCityData> cities)
    {
        if (cities == null || cities.Count == 0) return;

        DateTime alertTime = cities[0].Timestamp;
        var zones = new List<string>();
        var descriptions = new List<string>();
        var results = new List<string>();

        foreach (var city in cities)
            if (!zones.Contains(city.Zone_he))
                zones.Add(city.Zone_he);

        for (int i = 0; i < zones.Count; i++)
        {
            descriptions.Add("");
            foreach (var city in cities)
                if (city.Zone_he == zones[i])
                    descriptions[i] += $"{city.Name_he} - ({city.Time_he})\n";
        }

        for (int i = 0; i < zones.Count; i++)
            results.Add($"{zones[i]}\n\n{descriptions[i]}");

        Console.WriteLine($"🔥 New Alert! [{alertTime.ToShortDateString()} {alertTime.ToShortTimeString()}]:\n" +
                          string.Join("\n\n", results));
    }
}
```
## Information
🧠 Performance optimized: Light background polling thread keeps your CPU chill while staying alert-ready.

🤖 AI-free zone: Because sometimes the best algorithms are just good ol’ C# and coffee ☕.

## Dependencies
[Newtonsoft.Json 13.0.1](https://www.nuget.org/packages/Newtonsoft.Json/)<br><br>Installation:<br>
```Install-Package Newtonsoft.Json -Version 13.0.1```

## This project is under Berkeley Software Distribution (BSD) license.
    Use, modify, and distribute freely (including commercial use)

    No obligation to open-source your modifications

    Don’t use authors’ names to promote derived products without permission

    Must include license and copyright notices in distributions

Disclaimer
"Fair use is real — this project is for education, safety, and curiosity. No rockets were harmed in making this code."
