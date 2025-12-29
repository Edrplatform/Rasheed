using System;

using System.Collections.Generic;

using System.IO;

using System.Text.Json;
using System.Text.Encodings.Web;

using EDR_agent.Models;


namespace EDR_agent.Writing
{
    public static class EcsWriter
    {

        public static void WriteEvents(string outputFilePath, IEnumerable<EcsEvent> eventsList)
        {

            Console.WriteLine("EcsWriter.WriteEvents: " + outputFilePath);


            using (var stream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var writer = new StreamWriter(stream))
            {

                foreach (var ecsEvent in eventsList)
                {
                    // Serialize only non-null fields so output matches the attached project's behavior
                    var options = new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    };

                    var ecsObject = BuildEcsObject(ecsEvent);
                    var json = JsonSerializer.Serialize(ecsObject, options);

                    writer.WriteLine(json);

                }

            }


            Console.WriteLine("EcsWriter.WriteEvents: done.");
        }

        private static Dictionary<string, object> BuildEcsObject(EcsEvent ecs)
        {
            var obj = new Dictionary<string, object>
            {
                ["@timestamp"] = ecs.Timestamp,
                ["event.code"] = ecs.EventCode,
                ["event.category"] = ecs.EventCategory,
                ["event.action"] = ecs.EventAction,
                ["host.name"] = ecs.HostName
            };

            // Per-EDRAgent ordering: registry events add registry fields first
            if (ecs.EventCategory == "registry")
            {
                if (!string.IsNullOrWhiteSpace(ecs.RegistryKey)) obj["registry.path"] = ecs.RegistryKey;
                if (!string.IsNullOrWhiteSpace(ecs.RegistryValue)) obj["registry.value"] = ecs.RegistryValue;

                if (!string.IsNullOrWhiteSpace(ecs.ProcessExecutable)) obj["process.executable"] = ecs.ProcessExecutable;
                if (!string.IsNullOrWhiteSpace(ecs.ProcessName)) obj["process.name"] = ecs.ProcessName;
            }
            else if (ecs.EventCategory == "network")
            {
                // network events: include network fields first, then minimal process fields (executable + name)
                if (!string.IsNullOrWhiteSpace(ecs.SourceIp)) obj["source.ip"] = ecs.SourceIp;
                if (ecs.SourcePort.HasValue) obj["source.port"] = ecs.SourcePort.Value;
                if (!string.IsNullOrWhiteSpace(ecs.DestinationIp)) obj["destination.ip"] = ecs.DestinationIp;
                if (ecs.DestinationPort.HasValue) obj["destination.port"] = ecs.DestinationPort.Value;
                if (!string.IsNullOrWhiteSpace(ecs.NetworkProtocol)) obj["network.protocol"] = ecs.NetworkProtocol;

                // For DNS queries, EDRAgent puts dns fields before process fields; add them now if present
                if (!string.IsNullOrWhiteSpace(ecs.DnsQuestionName)) obj["dns.question.name"] = ecs.DnsQuestionName;
                if (!string.IsNullOrWhiteSpace(ecs.UrlDomain)) obj["url.domain"] = ecs.UrlDomain;
                if (!string.IsNullOrWhiteSpace(ecs.DnsResponseCode)) obj["dns.response_code"] = ecs.DnsResponseCode;
                if (!string.IsNullOrWhiteSpace(ecs.DnsAnswers)) obj["dns.answers"] = ecs.DnsAnswers;

                if (!string.IsNullOrWhiteSpace(ecs.ProcessExecutable)) obj["process.executable"] = ecs.ProcessExecutable;
                if (!string.IsNullOrWhiteSpace(ecs.ProcessName)) obj["process.name"] = ecs.ProcessName;
            }
            else
            {
                // process-related fields first for other event types
                if (!string.IsNullOrWhiteSpace(ecs.ProcessExecutable)) obj["process.executable"] = ecs.ProcessExecutable;
                if (!string.IsNullOrWhiteSpace(ecs.ProcessName)) obj["process.name"] = ecs.ProcessName;
                if (!string.IsNullOrWhiteSpace(ecs.ProcessCommandLine)) obj["process.command_line"] = ecs.ProcessCommandLine;
                if (ecs.ProcessPid.HasValue) obj["process.pid"] = ecs.ProcessPid.Value;
                if (!string.IsNullOrWhiteSpace(ecs.ProcessParentExecutable)) obj["process.parent.executable"] = ecs.ProcessParentExecutable;
                if (!string.IsNullOrWhiteSpace(ecs.ProcessParentName)) obj["process.parent.name"] = ecs.ProcessParentName;
            }

            // user info (if present)
            if (!string.IsNullOrWhiteSpace(ecs.UserDomain)) obj["user.domain"] = ecs.UserDomain;
            if (!string.IsNullOrWhiteSpace(ecs.UserName)) obj["user.name"] = ecs.UserName;


            // file fields
            if (!string.IsNullOrWhiteSpace(ecs.FilePath)) obj["file.path"] = ecs.FilePath;
            if (!string.IsNullOrWhiteSpace(ecs.FileName)) obj["file.name"] = ecs.FileName;

            // dns fields (if not already placed earlier for network/dns events)
            if (!obj.ContainsKey("dns.question.name") && !string.IsNullOrWhiteSpace(ecs.DnsQuestionName)) obj["dns.question.name"] = ecs.DnsQuestionName;
            if (!obj.ContainsKey("url.domain") && !string.IsNullOrWhiteSpace(ecs.UrlDomain)) obj["url.domain"] = ecs.UrlDomain;
            if (!obj.ContainsKey("dns.response_code") && !string.IsNullOrWhiteSpace(ecs.DnsResponseCode)) obj["dns.response_code"] = ecs.DnsResponseCode;
            if (!obj.ContainsKey("dns.answers") && !string.IsNullOrWhiteSpace(ecs.DnsAnswers)) obj["dns.answers"] = ecs.DnsAnswers;

            // registry fields (if not already handled)
            if (ecs.EventCategory != "registry")
            {
                if (!string.IsNullOrWhiteSpace(ecs.RegistryKey)) obj["registry.path"] = ecs.RegistryKey;
                if (!string.IsNullOrWhiteSpace(ecs.RegistryValue)) obj["registry.value"] = ecs.RegistryValue;
            }

            return obj;
        }
    }
}

