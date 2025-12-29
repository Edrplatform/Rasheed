using System.Text.Json.Serialization;


namespace EDR_agent.Models
{
    public class SysmonEvent
    {

        [JsonPropertyName("Timestamp")]
        public string Timestamp { get; set; }


        [JsonPropertyName("EventId")]
        public int EventId { get; set; }


        [JsonPropertyName("Computer")]
        public string Computer { get; set; }


        [JsonPropertyName("Data")]
        public SysmonData Data { get; set; }
    }


   public class SysmonData
{

    [JsonPropertyName("UtcTime")]
    public string UtcTime { get; set; }


    [JsonPropertyName("Image")]
    public string Image { get; set; }


    [JsonPropertyName("ProcessId")]
    public string ProcessId { get; set; }


    [JsonPropertyName("CommandLine")]
    public string CommandLine { get; set; }


    [JsonPropertyName("User")]
    public string User { get; set; }


    // network fields (EventId = 3)

    [JsonPropertyName("Protocol")]
    public string Protocol { get; set; }


    [JsonPropertyName("SourceIp")]
    public string SourceIp { get; set; }


    [JsonPropertyName("SourcePort")]
    public string SourcePort { get; set; }


    [JsonPropertyName("DestinationIp")]
    public string DestinationIp { get; set; }


    [JsonPropertyName("DestinationPort")]
    public string DestinationPort { get; set; }


    // file fields (EventId = 11)

    [JsonPropertyName("TargetFilename")]
    public string TargetFilename { get; set; }


    [JsonPropertyName("FileName")]
    public string FileName { get; set; }


    // registry/detailed fields (EventId = 13)

    [JsonPropertyName("TargetObject")]
    public string TargetObject { get; set; }


    [JsonPropertyName("Details")]
    public string Details { get; set; }

    // parent process fields (may appear in Sysmon Data)

    [JsonPropertyName("ParentProcessId")]
    public string ParentProcessId { get; set; }

    [JsonPropertyName("ParentImage")]
    public string ParentImage { get; set; }

    [JsonPropertyName("ParentCommandLine")]
    public string ParentCommandLine { get; set; }

    [JsonPropertyName("ParentUser")]
    public string ParentUser { get; set; }


    // dns fields (EventId = 22)

    [JsonPropertyName("QueryName")]
    public string QueryName { get; set; }


    [JsonPropertyName("QueryResults")]
    public string QueryResults { get; set; }

    [JsonPropertyName("QueryStatus")]
    public string QueryStatus { get; set; }
}

}
