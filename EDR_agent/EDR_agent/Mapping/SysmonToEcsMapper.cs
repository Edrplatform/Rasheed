using System;

using EDR_agent.Models;


namespace EDR_agent.Mapping
{
    public static class SysmonToEcsMapper
    {

        public static EcsEvent Map(SysmonEvent sysmonEvent)
        {

            if (sysmonEvent == null) return null;

            // reuse existing mapping logic

            if (sysmonEvent.EventId == 1)
            {
                return MapProcessCreate(sysmonEvent);
            }

            if (sysmonEvent.EventId == 3)
            {
                return MapNetworkConnect(sysmonEvent);
            }

            if (sysmonEvent.EventId == 11)
            {
                return MapFileCreate(sysmonEvent);
            }

            if (sysmonEvent.EventId == 13)
            {
                return MapRegistry(sysmonEvent);
            }

            if (sysmonEvent.EventId == 22)
            {
                return MapDnsQuery(sysmonEvent);
            }

            return null;
        }

        private static string FormatTimestamp(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return raw;
            try
            {
                var dt = DateTime.Parse(raw);
                // match EDRAgent formatting (seconds precision, no offset)
                return dt.ToString("yyyy-MM-ddTHH:mm:ss");
            }
            catch
            {
                return raw;
            }
        }



        private static EcsEvent MapProcessCreate(SysmonEvent sysmonEvent)
        {

            var ecs = new EcsEvent();


            ecs.Timestamp = FormatTimestamp(sysmonEvent.Timestamp);


            ecs.EventCode = sysmonEvent.EventId;
            ecs.EventCategory = "process";
            ecs.EventAction = "start";


            ecs.HostName = sysmonEvent.Computer;


            if (sysmonEvent.Data != null)
            {

                ecs.ProcessExecutable = sysmonEvent.Data.Image;


                if (!string.IsNullOrWhiteSpace(sysmonEvent.Data.ProcessId))
                {
                    if (int.TryParse(sysmonEvent.Data.ProcessId, out var pid))
                    {
                        ecs.ProcessPid = pid;
                    }
                }

                ecs.ProcessCommandLine = sysmonEvent.Data.CommandLine;


                if (!string.IsNullOrWhiteSpace(sysmonEvent.Data.Image))
                {

                    try
                    {

                        var imagePath = sysmonEvent.Data.Image;

                        var parts = imagePath.Split('\\');

                        ecs.ProcessName = parts[parts.Length - 1];

                    }
                    catch
                    {
                    }
                }

                // parent process information (if present in Sysmon data)
                if (!string.IsNullOrWhiteSpace(sysmonEvent.Data.ParentImage))
                {
                    ecs.ProcessParentExecutable = sysmonEvent.Data.ParentImage;
                    try
                    {
                        var pparts = sysmonEvent.Data.ParentImage.Split('\\');
                        ecs.ProcessParentName = pparts[pparts.Length - 1];
                    }
                    catch
                    {
                    }
                }

                if (!string.IsNullOrWhiteSpace(sysmonEvent.Data.ParentProcessId))
                {
                    if (int.TryParse(sysmonEvent.Data.ParentProcessId, out var ppid))
                    {
                        // include as part of command-line if needed in future; not required now
                    }
                }

                // user information (prefer Data.User, fall back to Data.ParentUser)
                var userString = !string.IsNullOrWhiteSpace(sysmonEvent.Data.User) ? sysmonEvent.Data.User : sysmonEvent.Data.ParentUser;

                if (!string.IsNullOrWhiteSpace(userString))
                {
                    var user = userString;

                    var index = user.IndexOf('\\');

                    if (index > 0)
                    {
                        ecs.UserDomain = user.Substring(0, index);
                        ecs.UserName = user.Substring(index + 1);
                    }
                    else
                    {
                        ecs.UserName = user;
                    }

                }

            }

            return ecs;
        }



        private static EcsEvent MapNetworkConnect(SysmonEvent sysmonEvent)
        {

            var ecs = new EcsEvent();


            ecs.Timestamp = FormatTimestamp(sysmonEvent.Timestamp);


            ecs.EventCode = sysmonEvent.EventId;
            ecs.EventCategory = "network";
            ecs.EventAction = "connection_established";


            ecs.HostName = sysmonEvent.Computer;


            if (sysmonEvent.Data != null)
            {

                ecs.ProcessExecutable = sysmonEvent.Data.Image;

                if (!string.IsNullOrWhiteSpace(sysmonEvent.Data.Image))
                {

                    try
                    {

                        var imagePath = sysmonEvent.Data.Image;

                        var parts = imagePath.Split('\\');

                        ecs.ProcessName = parts[parts.Length - 1];

                    }
                    catch
                    {
                    }
                }


                // network fields

                ecs.NetworkProtocol = sysmonEvent.Data.Protocol;


                ecs.SourceIp = sysmonEvent.Data.SourceIp;

                if (!string.IsNullOrWhiteSpace(sysmonEvent.Data.SourcePort))
                {
                    if (int.TryParse(sysmonEvent.Data.SourcePort, out var sPort))
                    {
                        ecs.SourcePort = sPort;
                    }
                }


                ecs.DestinationIp = sysmonEvent.Data.DestinationIp;

                if (!string.IsNullOrWhiteSpace(sysmonEvent.Data.DestinationPort))
                {
                    if (int.TryParse(sysmonEvent.Data.DestinationPort, out var dPort))
                    {
                        ecs.DestinationPort = dPort;
                    }
                }


            }


            return ecs;
        }


        private static EcsEvent MapFileCreate(SysmonEvent sysmonEvent)
        {

            var ecs = new EcsEvent();


            ecs.Timestamp = FormatTimestamp(sysmonEvent.Timestamp);


            ecs.EventCode = sysmonEvent.EventId;
            ecs.EventCategory = "file";
            ecs.EventAction = "create";


            ecs.HostName = sysmonEvent.Computer;


            if (sysmonEvent.Data != null)
            {

                ecs.ProcessExecutable = sysmonEvent.Data.Image;

                if (!string.IsNullOrWhiteSpace(sysmonEvent.Data.ProcessId))
                {
                    if (int.TryParse(sysmonEvent.Data.ProcessId, out var pid))
                    {
                        ecs.ProcessPid = pid;
                    }
                }

                if (!string.IsNullOrWhiteSpace(sysmonEvent.Data.Image))
                {

                    try
                    {

                        var imagePath = sysmonEvent.Data.Image;

                        var parts = imagePath.Split('\\');

                        ecs.ProcessName = parts[parts.Length - 1];

                    }
                    catch
                    {
                    }
                }


                // file fields

                if (!string.IsNullOrWhiteSpace(sysmonEvent.Data.TargetFilename))
                {
                    ecs.FilePath = sysmonEvent.Data.TargetFilename;

                    try
                    {
                        var parts = sysmonEvent.Data.TargetFilename.Split('\\');
                        ecs.FileName = parts[parts.Length - 1];
                    }
                    catch
                    {
                    }
                }
                else if (!string.IsNullOrWhiteSpace(sysmonEvent.Data.FileName))
                {
                    ecs.FilePath = sysmonEvent.Data.FileName;
                    ecs.FileName = sysmonEvent.Data.FileName;
                }


                var userString = !string.IsNullOrWhiteSpace(sysmonEvent.Data.User) ? sysmonEvent.Data.User : sysmonEvent.Data.ParentUser;

                if (!string.IsNullOrWhiteSpace(userString))
                {

                    var user = userString;

                    var index = user.IndexOf('\\');

                    if (index > 0)
                    {
                        ecs.UserDomain = user.Substring(0, index);
                        ecs.UserName = user.Substring(index + 1);
                    }
                    else
                    {
                        ecs.UserName = user;
                    }

                }

            }


            return ecs;
        }


        private static EcsEvent MapRegistry(SysmonEvent sysmonEvent)
        {

            var ecs = new EcsEvent();


            ecs.Timestamp = FormatTimestamp(sysmonEvent.Timestamp);


            ecs.EventCode = sysmonEvent.EventId;
            ecs.EventCategory = "registry";
            ecs.EventAction = "modification";    



            ecs.HostName = sysmonEvent.Computer;


            if (sysmonEvent.Data != null)
            {

                ecs.ProcessExecutable = sysmonEvent.Data.Image;

                if (!string.IsNullOrWhiteSpace(sysmonEvent.Data.ProcessId))
                {
                    if (int.TryParse(sysmonEvent.Data.ProcessId, out var pid))
                    {
                        ecs.ProcessPid = pid;
                    }
                }

                if (!string.IsNullOrWhiteSpace(sysmonEvent.Data.Image))
                {

                    try
                    {

                        var imagePath = sysmonEvent.Data.Image;

                        var parts = imagePath.Split('\\');

                        ecs.ProcessName = parts[parts.Length - 1];

                    }
                    catch
                    {
                    }
                }


                // registry fields

                if (!string.IsNullOrWhiteSpace(sysmonEvent.Data.TargetObject))
                {
                    ecs.RegistryKey = sysmonEvent.Data.TargetObject;
                }

                if (!string.IsNullOrWhiteSpace(sysmonEvent.Data.Details))
                {
                    ecs.RegistryValue = sysmonEvent.Data.Details;
                }

                var userString = !string.IsNullOrWhiteSpace(sysmonEvent.Data.User) ? sysmonEvent.Data.User : sysmonEvent.Data.ParentUser;

                if (!string.IsNullOrWhiteSpace(userString))
                {

                    var user = userString;

                    var index = user.IndexOf('\\');

                    if (index > 0)
                    {
                        ecs.UserDomain = user.Substring(0, index);
                        ecs.UserName = user.Substring(index + 1);
                    }
                    else
                    {
                        ecs.UserName = user;
                    }

                }

            }


            return ecs;
        }


        private static EcsEvent MapDnsQuery(SysmonEvent sysmonEvent)
        {

            var ecs = new EcsEvent();


            ecs.Timestamp = FormatTimestamp(sysmonEvent.Timestamp);


            ecs.EventCode = sysmonEvent.EventId;
            ecs.EventCategory = "network";
            ecs.EventAction = "dns_query";


            ecs.HostName = sysmonEvent.Computer;


            if (sysmonEvent.Data != null)
            {

                ecs.ProcessExecutable = sysmonEvent.Data.Image;

                if (!string.IsNullOrWhiteSpace(sysmonEvent.Data.Image))
                {

                    try
                    {

                        var imagePath = sysmonEvent.Data.Image;

                        var parts = imagePath.Split('\\');

                        ecs.ProcessName = parts[parts.Length - 1];

                    }
                    catch
                    {
                    }
                }


                // dns fields

                if (!string.IsNullOrWhiteSpace(sysmonEvent.Data.QueryName))
                {
                    ecs.DnsQuestionName = sysmonEvent.Data.QueryName;
                    ecs.UrlDomain = sysmonEvent.Data.QueryName;
                }

                if (!string.IsNullOrWhiteSpace(sysmonEvent.Data.QueryStatus))
                {
                    ecs.DnsResponseCode = sysmonEvent.Data.QueryStatus;
                }

                if (!string.IsNullOrWhiteSpace(sysmonEvent.Data.QueryResults) && sysmonEvent.Data.QueryResults != "-")
                {
                    ecs.DnsAnswers = sysmonEvent.Data.QueryResults;
                }

                var userString = !string.IsNullOrWhiteSpace(sysmonEvent.Data.User) ? sysmonEvent.Data.User : sysmonEvent.Data.ParentUser;

                if (!string.IsNullOrWhiteSpace(userString))
                {

                    var user = userString;

                    var index = user.IndexOf('\\');

                    if (index > 0)
                    {
                        ecs.UserDomain = user.Substring(0, index);
                        ecs.UserName = user.Substring(index + 1);
                    }
                    else
                    {
                        ecs.UserName = user;
                    }

                }

            }


            return ecs;
        }
    }
}
