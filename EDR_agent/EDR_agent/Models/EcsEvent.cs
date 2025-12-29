using System.Text.Json.Serialization;


namespace EDR_agent.Models
{
    public class EcsEvent
    {

        [JsonPropertyName("@timestamp")]
        public string Timestamp { get; set; }


        [JsonPropertyName("event.code")]
        public int EventCode { get; set; }


        [JsonPropertyName("event.category")]
        public string EventCategory { get; set; }


        [JsonPropertyName("event.action")]
        public string EventAction { get; set; }


        [JsonPropertyName("host.name")]
        public string HostName { get; set; }


        [JsonPropertyName("process.executable")]
        public string ProcessExecutable { get; set; }


        [JsonPropertyName("process.name")]
        public string ProcessName { get; set; }


        [JsonPropertyName("process.command_line")]
        public string ProcessCommandLine { get; set; }


        [JsonPropertyName("process.pid")]
        public int? ProcessPid { get; set; }


        [JsonPropertyName("user.domain")]
        public string UserDomain { get; set; }


        [JsonPropertyName("user.name")]
        public string UserName { get; set; }


        // network fields

        [JsonPropertyName("source.ip")]
        public string SourceIp { get; set; }


        [JsonPropertyName("source.port")]
        public int? SourcePort { get; set; }


        [JsonPropertyName("destination.ip")]
        public string DestinationIp { get; set; }


        [JsonPropertyName("destination.port")]
        public int? DestinationPort { get; set; }


        [JsonPropertyName("network.protocol")]
        public string NetworkProtocol { get; set; }


        // file fields

        [JsonPropertyName("file.path")]
        public string FilePath { get; set; }


        [JsonPropertyName("file.name")]
        public string FileName { get; set; }


        // dns fields

        [JsonPropertyName("dns.question.name")]
        public string DnsQuestionName { get; set; }


        [JsonPropertyName("dns.answers")]
        public string DnsAnswers { get; set; }

        [JsonPropertyName("url.domain")]
        public string UrlDomain { get; set; }

        [JsonPropertyName("dns.response_code")]
        public string DnsResponseCode { get; set; }


        // registry fields

        [JsonPropertyName("registry.path")]
        public string RegistryKey { get; set; }


        [JsonPropertyName("registry.value")]
        public string RegistryValue { get; set; }

        // parent process fields

        [JsonPropertyName("process.parent.executable")]
        public string ProcessParentExecutable { get; set; }

        [JsonPropertyName("process.parent.name")]
        public string ProcessParentName { get; set; }
    }
}
