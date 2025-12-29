using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EDR_agent.Models;

namespace EDR_agent.Services
{
    public class RawBatchWriter
    {
        private readonly string _rawFilePath;
        private readonly int _batchSize;
        private readonly int _flushIntervalMs;

        public RawBatchWriter(string rawFilePath, int batchSize, int flushIntervalMs)
        {
            _rawFilePath = rawFilePath ?? throw new ArgumentNullException(nameof(rawFilePath));
            _batchSize = batchSize;
            _flushIntervalMs = flushIntervalMs;
        }

        public async Task RunAsync(System.Threading.Channels.ChannelReader<SysmonEvent> reader, CancellationToken cancellationToken)
        {
            var buffer = new List<SysmonEvent>(_batchSize);
            var lastFlush = DateTime.UtcNow;

            while (await reader.WaitToReadAsync(cancellationToken))
            {
                while (reader.TryRead(out var evt))
                {
                    buffer.Add(evt);

                    if (buffer.Count >= _batchSize)
                    {
                        await FlushAsync(buffer, cancellationToken);
                        buffer.Clear();
                        lastFlush = DateTime.UtcNow;
                    }
                }

                var now = DateTime.UtcNow;
                if (buffer.Count > 0 && (now - lastFlush).TotalMilliseconds >= _flushIntervalMs)
                {
                    await FlushAsync(buffer, cancellationToken);
                    buffer.Clear();
                    lastFlush = DateTime.UtcNow;
                }

                await Task.Delay(50);
            }

            // final flush
            if (buffer.Count > 0)
            {
                await FlushAsync(buffer, cancellationToken);
            }

            Console.WriteLine("RawBatchWriter: completed and flushed.");
        }

        private async Task FlushAsync(List<SysmonEvent> buffer, CancellationToken cancellationToken)
        {
            try
            {
                using (var fs = new FileStream(_rawFilePath, FileMode.Append, FileAccess.Write, FileShare.Read, 4096, useAsync: true))
                using (var sw = new StreamWriter(fs))
                {
                    foreach (var evt in buffer)
                    {
                        var json = JsonSerializer.Serialize(evt);
                        await sw.WriteLineAsync(json.AsMemory(), cancellationToken);
                    }

                    await sw.FlushAsync();
                }

                Console.WriteLine($"RawBatchWriter: flushed {buffer.Count} events to {_rawFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("RawBatchWriter error: " + ex.Message);
            }
        }
    }
}