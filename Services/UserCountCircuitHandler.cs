using Microsoft.AspNetCore.Components.Server.Circuits;
using System.Collections.Concurrent;
using ZetaDashboard.Common.Mongo.DataModels;

namespace ZetaDashboard.Services
{
    public sealed class UserCountCircuitHandler : CircuitHandler
    {
        private static readonly ConcurrentDictionary<string, byte> _circuits = new();

        public static int Connected => _circuits.Count;

        // single delayed task controller
        private static CancellationTokenSource? _idleCts;

        private readonly TimeSpan _grace = TimeSpan.FromMinutes(2);      // wait after last user leaves
        private readonly TimeSpan _extraDelay = TimeSpan.FromMinutes(10); // additional delay before exit

        public UserCountCircuitHandler()
        {
            
        }

        public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken token)
        {
            _circuits.TryAdd(circuit.Id, 0);
            Console.WriteLine($"[Circuits][UP] id={circuit.Id} Connected={Connected}");

            // cancel any pending idle shutdown
            _idleCts?.Cancel();
            return Task.CompletedTask;
        }

        public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken token)
        {
            _circuits.TryRemove(circuit.Id, out _);
            Console.WriteLine($"[Circuits][DOWN] id={circuit.Id} Connected={Connected}");

            if (Connected == 0)
            {
                // schedule a one-shot task (no loops)
                _idleCts?.Cancel();
                _idleCts = new CancellationTokenSource();
                var ct = _idleCts.Token;

                _ = Task.Run(async () =>
                {
                    try
                    {
                        Console.WriteLine($"[Idle] No users. Waiting {_grace.TotalSeconds}s (grace)...");
                        await Task.Delay(_grace, ct);

                        if (ct.IsCancellationRequested || Connected > 0)
                        {
                            Console.WriteLine("[Idle] Abort: user reconnected during grace.");
                            return;
                        }


                        Console.WriteLine($"[Idle] Still 0 users. Waiting additional {_extraDelay.TotalMinutes} minutes before exit...");
                        await Task.Delay(_extraDelay, ct);

                        if (ct.IsCancellationRequested || Connected > 0)
                        {
                            Console.WriteLine("[Idle] Abort: user reconnected during extra delay.");
                            return;
                        }

                        Console.WriteLine("[Idle] Exiting process now with code 1...");
                        Environment.Exit(1); // or 0 for clean exit
                    }
                    catch (TaskCanceledException)
                    {
                        Console.WriteLine("[Idle] Idle task canceled (user activity resumed).");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[Idle] Error in idle shutdown task: " + ex.Message);
                    }
                }, ct);
            }

            return Task.CompletedTask;
        }
    }
}
