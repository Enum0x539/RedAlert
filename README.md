# Description
Implementation of Israel Home Front Command's (AKA "Pikud Ha Oref") public API, gets real-time alerts about rockets lunched from Gaza-strip into Israel's territory. 

## About the library
RedAlert is a library that synchronizing with Israel Home Front Command's API to get the alerts in real-time,
the library interface is user-friendly and has lots of features which are described below.

## Features
* Get red alerts in real-time.
 
* Fetches location data from alerts codes (coordinates, city names, city zones, time to run for safe-zone)

* Supports 4 languages: Hebrew, Arabic, Russian and English.

* Generates random coordinates within the given coordinates.

* Gets the picture of Israel with markers over the alert's location with path lines.

## Example code
```cs
using RedAlert;

private static void Main()
{
    //Creating the event subscriber
    Alerts alerts = new Alerts();
    alerts.OnAlertReceived += Alerts_OnAlertReceived;
}

private static void Alerts_OnAlertReceived(List<AlertCityData> cities)
{
    //With every new alert, this function will get to execute.
    
    DateTime occurence = cities[0].Timestamp;
    List<string> zones = new List<string>();
    List<string> descriptions = new List<string>();
    List<string> result = new List<string>();

    //Adding the last alert zones into a list.
    foreach (AlertCityData city in cities)
    {
        if (!zones.Contains(city.Zone_he))
            zones.Add(city.Zone_he);
    }

    //Sorting all the alerts, every alert into his zone.
    for (int i = 0; i < zones.Count; ++i)
    {
        descriptions.Add("");
        foreach (AlertCityData city in cities)
        {
            if (city.Zone_he != zones[i])
                continue;

            descriptions[i] += $"{city.Name_he} - ({city.Time_he})\n";
        }
    }

    //Adding the results into a list for easier use.
    for (int i = 0; i < zones.Count; ++i)
        result.Add($"{zones[i]}\n\n{descriptions[i]}");

    //Printing the alerts with their information
    Console.WriteLine($"New Alert! [{occurence.ToShortDateString()}] {occurence.ToShortTimeString()}:\n" + string.Join("\n\n", result));
}
```

## Short documentation
| Function             | Parameters  |  Description | Returns       |
| -----------          |                                                           ----------- | -----------  | -----------   |
| GetMapPhoto()        |  PathColorHex, Coordinates      | Retrieves the bytes of the given coordinates image from Google Maps API with a path-line from Gaza-strip.        | byte array         |
| RandomCoordinates()  | Coordinates        | Generates random coordinates within the given coordinates.         | Coordinates         |

---
**AlertCityData**: <br>
Name_he - returns the name of the alert city in Hebrew. <br>
Name_en - returns the name of the alert city in English. <br>
Name_ru - returns the name of the alert city in Russian. <br>
Name_ar - returns the name of the alert city in Arabic. <br>
 <br>
Zone_he - returns the name of the alert zone in Hebrew. <br>
Zone_en - returns the name of the alert zone in English. <br>
Zone_ru - returns the name of the alert zone in Russian. <br>
Zone_ar - returns the name of the alert zone in Arabic. <br>
 <br>
Countdown - returns the time to ran to into safe-zone.  <br>
Time_he - returns the time to ran to into safe-zone in Hebrew. <br>
Time_en - returns the time to ran to into safe-zone in English. <br>
Time_ru - returns the time to ran to into safe-zone in Russian. <br>
Time_ar - returns the time to ran to into safe-zone in Arabic. <br>
 <br>
Timestamp - returns the timestamp of the alert in "Israel Standard Time". <br>
Coordinates - returns the coordinates of the alert.

## Information
1) This library will only work for people whose locations is in Israel because Israel Home Front Command (Pikud Ha Oref) accepts only the requests from Israel.<br>
2) The enemy cannot exploit or use this library for bad usage because it does not contain any private sensitive information, we use the public API of Israel Home Front Command (Pikud Ha Oref) <br>
3) This library is very simple to use and have very good performance

## Dependencies
[Newtonsoft.Json 13.0.1](https://www.nuget.org/packages/Newtonsoft.Json/)<br><br>Installation:<br>
```Install-Package Newtonsoft.Json -Version 13.0.1```

## This project is under Berkeley Software Distribution (BSD) license.
* The source code doesn’t need to be public when a distribution of the software is made.
* Modifications to the software can be released under any license.
* Changes made to the source code may not be documented.
* It offers no explicit position on patent usage.
* The license and copyright notice must be included in the documentation of the compiled version of the source code (as opposed to only in the source code).
* The BSD 3-clause states that the names of the author and contributors can’t be used to promote products derived from the software without permission.

## Educational Purposes
"Copyright Disclaimer Under Section 107 of the Copyright Act 1976, allowance is made for "fair use" for purposes such as criticism, comment, news reporting, teaching scholarship, and research. Fair use is a use permitted by copyright statutes that might otherwise be infringing. Non-profit, educational, or personal use tips the balance in favor of fair use."
