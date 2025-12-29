using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using EDR_agent.Models;

namespace EDR_agent.Collectors
{
    /// <summary>Collect Sysmon events from Windows Event Log and append to JSONL file</summary>
    public class SysmonCollector
    {
        private const string SYSMON_LOG_NAME = "Microsoft-Windows-Sysmon/Operational";
        private const string EVENT_QUERY_FILTER = "*[System[(EventID=1 or EventID=3 or EventID=11 or EventID=13 or EventID=22)]]";
        private const int BATCH_SIZE = 50;
        private const int BATCH_DELAY_MS = 2000;

        // New: write events into channels instead of directly to file
        public async Task RunAsync(System.Threading.Channels.ChannelWriter<SysmonEvent> rawWriter, System.Threading.Channels.ChannelWriter<SysmonEvent> normalizeWriter, CancellationToken cancellationToken)
        {
            Console.WriteLine("SysmonCollector: starting collection (channel mode)...");

            var query = new EventLogQuery(SYSMON_LOG_NAME, PathType.LogName, EVENT_QUERY_FILTER);

            using (var reader = new EventLogReader(query))
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var events = ReadEventBatch(reader, BATCH_SIZE);

                        if (events.Count > 0)
                        {
                            Console.WriteLine($"SysmonCollector: collected {events.Count} events, dispatching to channels");

                            foreach (var evt in events)
                            {
                                // Attempt to write to both channels, honoring cancellation
                                await rawWriter.WriteAsync(evt, cancellationToken);
                                await normalizeWriter.WriteAsync(evt, cancellationToken);
                            }

                            Console.WriteLine($"SysmonCollector: dispatched {events.Count} events");
                        }

                        await Task.Delay(BATCH_DELAY_MS, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        // stop collecting and allow completion/flush
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("SysmonCollector error: " + ex.Message);
                        await Task.Delay(BATCH_DELAY_MS, cancellationToken);
                    }
                }
            }

            // signal completion
            rawWriter.Complete();
            normalizeWriter.Complete();
        }

        private List<SysmonEvent> ReadEventBatch(EventLogReader reader, int maxEvents)
        {
            var events = new List<SysmonEvent>();
            EventRecord record;

            while ((record = reader.ReadEvent()) != null && events.Count < maxEvents)
            {
                using (record)
                {
                    try
                    {
                        var evt = ParseEvent(record);
                        if (evt != null)
                        {
                            events.Add(evt);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed to parse event: " + ex.Message);
                    }
                }
            }

            return events;
        }

        private SysmonEvent ParseEvent(EventRecord record)
        {
            var xml = record.ToXml();
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            var dataNodes = xmlDoc.GetElementsByTagName("Data");

            var data = new SysmonData();

            foreach (XmlElement node in dataNodes)
            {
                var name = node.GetAttribute("Name");
                var value = node.InnerText;

                if (string.IsNullOrEmpty(name)) continue;

                switch (name)
                {
                    case "UtcTime": data.UtcTime = value; break;
                    case "Image": data.Image = value; break;
                    case "ProcessId": data.ProcessId = value; break;
                    case "CommandLine": data.CommandLine = value; break;
                    case "User": data.User = value; break;
                    case "Protocol": data.Protocol = value; break;
                    case "SourceIp": data.SourceIp = value; break;
                    case "SourcePort": data.SourcePort = value; break;
                    case "DestinationIp": data.DestinationIp = value; break;
                    case "DestinationPort": data.DestinationPort = value; break;
                    case "TargetFilename": data.TargetFilename = value; break;
                    case "FileName": data.FileName = value; break;
                    case "TargetObject": data.TargetObject = value; break;
                    case "Details": data.Details = value; break;
                    case "ParentProcessId": data.ParentProcessId = value; break;
                    case "ParentImage": data.ParentImage = value; break;
                    case "ParentCommandLine": data.ParentCommandLine = value; break;
                    case "ParentUser": data.ParentUser = value; break;
                    case "QueryName": data.QueryName = value; break;
                    case "QueryResults": data.QueryResults = value; break;
                    case "QueryStatus": data.QueryStatus = value; break;
                    default:
                        // ignore unknown fields
                        break;
                }
            }

            var sysmonEvent = new SysmonEvent
            {
                Timestamp = record.TimeCreated?.ToString("o") ?? DateTime.UtcNow.ToString("o"),
                EventId = (int)record.Id,
                Computer = record.MachineName ?? "UNKNOWN",
                Data = data
            };

            return sysmonEvent;
        }
    }
}
