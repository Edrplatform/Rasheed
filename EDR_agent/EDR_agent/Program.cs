using System;

using System.IO;

using EDR_agent;
using EDR_agent.Services;


namespace EDR_agent
{
    public class Program
    {
        public static void Main(string[] args)
        {

            Console.WriteLine("---------- EDR Agent (Sysmon → ECS) ----------");


            // 1- Build dynamic paths under AppData\Roaming\EDRAgent

            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var agentFolderPath = Path.Combine(appDataPath, "EDRAgent");


            // 2- Create folder if not exists

            if (!Directory.Exists(agentFolderPath))
            {
                Directory.CreateDirectory(agentFolderPath);
            }


            // 3- Define input & output file paths

            var inputFilePath = Path.Combine(agentFolderPath, "sysmon_events.jsonl");

            var outputFilePath = Path.Combine(agentFolderPath, "B_events_normalized_ecs.jsonl");


            Console.WriteLine("Sysmon input file path: " + inputFilePath);

            Console.WriteLine("ECS output file path:  " + outputFilePath);


            // Load agent config (from AppData\EDRAgent\agent_config.json) or use defaults
            var options = Config.ConfigLoader.LoadOrDefault(agentFolderPath);

            if (string.IsNullOrWhiteSpace(options.RawFilePath)) options.RawFilePath = inputFilePath;
            if (string.IsNullOrWhiteSpace(options.NormalizedFilePath)) options.NormalizedFilePath = outputFilePath;

            // CLI modes: --collect, --normalize, --both
            var modeCollect = args != null && (Array.Exists(args, a => a == "--collect") || Array.Exists(args, a => a == "collect"));
            var modeNormalize = args != null && (Array.Exists(args, a => a == "--normalize") || Array.Exists(args, a => a == "normalize"));
            var modeBoth = args != null && (Array.Exists(args, a => a == "--both") || Array.Exists(args, a => a == "both"));

            // default: if no args -> normalize (one-shot)
            if (!modeCollect && !modeNormalize && !modeBoth)
            {
                modeNormalize = true;
            }

            // If collect-only: run collector -> raw writer
            if (modeCollect && !modeBoth && !modeNormalize)
            {
                try
                {
                    Console.WriteLine("Starting collection mode (press Ctrl+C to stop)...");

                    var cts = new System.Threading.CancellationTokenSource();
                    Console.CancelKeyPress += (s, e) => { e.Cancel = true; cts.Cancel(); };

                    var rawChannel = System.Threading.Channels.Channel.CreateBounded<Models.SysmonEvent>(new System.Threading.Channels.BoundedChannelOptions(options.ChannelCapacity) { FullMode = System.Threading.Channels.BoundedChannelFullMode.Wait });
                    var normalizeChannel = System.Threading.Channels.Channel.CreateBounded<Models.SysmonEvent>(new System.Threading.Channels.BoundedChannelOptions(options.ChannelCapacity) { FullMode = System.Threading.Channels.BoundedChannelFullMode.Wait });

                    var collector = new Collectors.SysmonCollector();
                    var rawWriter = new Services.RawBatchWriter(options.RawFilePath, options.BatchSize, options.FlushIntervalMs);

                    var collectorTask = collector.RunAsync(rawChannel.Writer, normalizeChannel.Writer, cts.Token);
                    var rawWriterTask = rawWriter.RunAsync(rawChannel.Reader, System.Threading.CancellationToken.None);

                    Task.WaitAll(new Task[] { collectorTask, rawWriterTask });

                    Console.WriteLine("Collection stopped by user.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error during collection:");
                    Console.WriteLine(ex.ToString());
                }

                return;
            }

            // If both: run collector + normalization workers + writers
            if (modeBoth)
            {
                try
                {
                    Console.WriteLine("Starting collect+normalize mode (press Ctrl+C to stop)...");

                    var cts = new System.Threading.CancellationTokenSource();
                    Console.CancelKeyPress += (s, e) => { e.Cancel = true; cts.Cancel(); };

                    var rawChannel = System.Threading.Channels.Channel.CreateBounded<Models.SysmonEvent>(new System.Threading.Channels.BoundedChannelOptions(options.ChannelCapacity) { FullMode = System.Threading.Channels.BoundedChannelFullMode.Wait });
                    var normalizeChannel = System.Threading.Channels.Channel.CreateBounded<Models.SysmonEvent>(new System.Threading.Channels.BoundedChannelOptions(options.ChannelCapacity) { FullMode = System.Threading.Channels.BoundedChannelFullMode.Wait });
                    var ecsChannel = System.Threading.Channels.Channel.CreateBounded<Models.EcsEvent>(new System.Threading.Channels.BoundedChannelOptions(options.ChannelCapacity) { FullMode = System.Threading.Channels.BoundedChannelFullMode.Wait });

                    var collector = new Collectors.SysmonCollector();
                    var rawWriter = new Services.RawBatchWriter(options.RawFilePath, options.BatchSize, options.FlushIntervalMs);
                    var normalizedWriter = new Services.NormalizedBatchWriter(options.NormalizedFilePath, options.BatchSize, options.FlushIntervalMs);
                    var workerPool = new Services.NormalizationWorkerPool(options.WorkerCount, ecsChannel.Writer);

                    var collectorTask = collector.RunAsync(rawChannel.Writer, normalizeChannel.Writer, cts.Token);
                    var rawWriterTask = rawWriter.RunAsync(rawChannel.Reader, System.Threading.CancellationToken.None);
                    var normalizationTask = workerPool.RunAsync(normalizeChannel.Reader, System.Threading.CancellationToken.None);
                    var normalizedWriterTask = normalizedWriter.RunAsync(ecsChannel.Reader, System.Threading.CancellationToken.None);

                    Task.WaitAll(new Task[] { collectorTask, rawWriterTask, normalizationTask, normalizedWriterTask });

                    Console.WriteLine("Collect+Normalize stopped by user.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error during collect+normalize:");
                    Console.WriteLine(ex.ToString());
                }

                return;
            }

            // If normalize-only (one-shot from file) or default
            if (modeNormalize && !modeBoth && !modeCollect)
            {
                try
                {
                    Console.WriteLine("Starting normalization process (from file)...");

                    NormalizationService.Run(options.RawFilePath, options.NormalizedFilePath);

                    Console.WriteLine("Normalization completed successfully.");

                }
                catch (Exception ex)
                {

                    Console.WriteLine("Error during normalization:");
                    Console.WriteLine(ex.ToString());

                }

                return;
            }
        }
    }
}
