using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using EDR_agent.Mapping;
using EDR_agent.Models;

namespace EDR_agent.Services
{
    public class NormalizationWorkerPool
    {
        private readonly int _workerCount;
        private readonly ChannelWriter<EcsEvent> _outputWriter;

        public NormalizationWorkerPool(int workerCount, ChannelWriter<EcsEvent> outputWriter)
        {
            _workerCount = workerCount;
            _outputWriter = outputWriter;
        }

        public Task RunAsync(ChannelReader<SysmonEvent> inputReader, CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < _workerCount; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    while (await inputReader.WaitToReadAsync(cancellationToken))
                    {
                        while (inputReader.TryRead(out var evt))
                        {
                            try
                            {
                                var ecs = SysmonToEcsMapper.Map(evt);
                                if (ecs != null)
                                {
                                    await _outputWriter.WriteAsync(ecs, cancellationToken);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Normalization worker error: {ex.Message}");
                            }
                        }
                    }
                }, cancellationToken));
            }

            return Task.WhenAll(tasks);
        }
    }
}