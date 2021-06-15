# RedAlert
Full Implementation of Israel Home Front Command's RedAlert. 

## About the library
RedAlert is a library that synchronizing with Israel Home Front Command to get the alerts in real-time,
the library interface is user-friendly so everyone can use it easily.

## Contacts
**Discord:** Enum#6690 <br>
**Instagram:** yenon_k11

## Nugets in use
**[Netonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)** <br>
**[Leaf.xNet](https://github.com/csharp-leaf/Leaf.xNet)**

## Exmaple code
```cs
using RedAlert;

static void Main()
{
    //Creating the event subscriber
    Alerts.Setup(RedAlertReceived);
}

private static void RedAlertReceived(List<AlertCityData> cities)
{
    //With every new alert, this function will gets to execute.
    
    //Converting the time to the universal time and adds 3 hours to get the current time in Israel.
    DateTime occurence = DateTime.Now.ToUniversalTime().AddHours(3);
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

 # Educational Purposes
"Copyright Disclaimer Under Section 107 of the Copyright Act 1976, allowance is made for "fair use" for purposes such as criticism, comment, news reporting, teaching scholarship, and research. Fair use is a use permitted by copyright statutes that might otherwise be infringing. Non-profit, educational, or personal use tips the balance in favor of fair use."
