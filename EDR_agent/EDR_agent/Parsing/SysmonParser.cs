using System;

using System.Collections.Generic;

using System.IO;

using System.Text.Json;

using EDR_agent.Models;



namespace EDR_agent.Parsing
{
    public static class SysmonParser
    {



        public static IEnumerable<SysmonEvent> ReadEvents(string inputFilePath)
        {

            Console.WriteLine("SysmonParser.ReadEvents: " + inputFilePath);


            var eventsList = new List<SysmonEvent>();


            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine("Sysmon input file not found.");
                return eventsList;
            }


            foreach (var line in File.ReadLines(inputFilePath))
            {

                if (string.IsNullOrWhiteSpace(line)) continue;


                try
                {

                    var sysmonEvent = JsonSerializer.Deserialize<SysmonEvent>(line);

                    if (sysmonEvent != null)
                    {
                        eventsList.Add(sysmonEvent);
                    }

                }
                catch (Exception ex)
                {

                    Console.WriteLine("Error parsing Sysmon line:");
                    Console.WriteLine(ex.Message);

                }

            }


            Console.WriteLine("SysmonParser.ReadEvents: total events = " + eventsList.Count);

            return eventsList;
        }
    }
}
