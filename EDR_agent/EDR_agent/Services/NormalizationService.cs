using System;

using System.Collections.Generic;

using EDR_agent.Models;
using EDR_agent.Parsing;
using EDR_agent.Mapping;
using EDR_agent.Writing;


namespace EDR_agent.Services
{
    public static class NormalizationService
    {

        public static void Run(string inputFilePath, string outputFilePath)
        {

            Console.WriteLine("NormalizationService.Run started.");



            // 1- Read Sysmon events from input file

            IEnumerable<SysmonEvent> sysmonEvents = SysmonParser.ReadEvents(inputFilePath);



            // 2- Map Sysmon → ECS

            var ecsEvents = new List<EcsEvent>();

            foreach (var sysmonEvent in sysmonEvents)
            {

                var ecsEvent = SysmonToEcsMapper.Map(sysmonEvent);

                if (ecsEvent != null)
                {
                    ecsEvents.Add(ecsEvent);
                }

            }



            // 3- Write ECS events to output file

            EcsWriter.WriteEvents(outputFilePath, ecsEvents);



            Console.WriteLine("NormalizationService.Run finished.");
        }
    }
}
