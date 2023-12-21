using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using ChadNedzlek.AdventOfCode.Library;

namespace ChadNedzlek.AdventOfCode.Y2023.CSharp.solvers
{
    public class Problem20 : DualAsyncProblemBase
    {
        public enum PulseType
        {
            Low,
            High,
        }

        protected override async Task ExecutePart1Async(string[] data)
        {
            Dictionary<string, ElfModule> modules = BuildModules(data, out ElfModule broadcaster);
            var flippy = modules["lm"];

            SignalManager signals = new SignalManager(false);
            var state = flippy.State;
            for (int i = 0; i < 1000; i++)
            {
                signals.Send(broadcaster, null, PulseType.Low);
                signals.ProcessAll();
                var newState = flippy.State;
                if (newState != state)
                {
                    Console.WriteLine($"Module {flippy.Name} changes from {state} to {newState} on {i}");
                    state = newState;
                }
            }
            
            Console.WriteLine($"Pulse count = high {signals.HighPulses} * low {signals.LowPulses} = {signals.HighPulses * signals.LowPulses}");
        }

        protected override async Task ExecutePart2Async(string[] data)
        {
            Dictionary<string, ElfModule> modules = BuildModules(data, out ElfModule broadcaster);
            Dictionary<string, long> highFlips = new ();

            var binaryCounters = modules["rx"].Inputs.SelectMany(i => i.Inputs).SelectMany(i => i.Inputs).ToList();
            long lcm = 1;
            foreach (var counter in binaryCounters)
            {
                var bits = SubNodes(counter);
                List<ElfModule> ordered = GetOrderedBitNodes(bits, broadcaster);
                
                int max = 1 << ordered.Count;
                int start = 0;
                for (int i = 0; i < ordered.Count; i++)
                {
                    if (counter.Outputs.Contains(ordered[i]))
                    {
                        start |= 1 << i;
                    }
                }

                int cycleLength = max - start;
                lcm = Helpers.Lcm(lcm, cycleLength);
            }
            
            Console.WriteLine($"Result = {lcm}");
        }

        private static List<ElfModule> GetOrderedBitNodes(HashSet<ElfModule> bits, ElfModule broadcaster)
        {
            ElfModule head = bits.First(n => n.Inputs.Contains(broadcaster));
            List<ElfModule> ordered = new() { head };
            bits.Remove(head);
            while (bits.Count != 0)
            {
                var next = bits.First(n => n.Inputs.Contains(ordered[^1]));
                bits.Remove(next);
                ordered.Add(next);
            }

            return ordered;
        }

        private HashSet<ElfModule> SubNodes(ElfModule counter, HashSet<ElfModule> nodes = null)
        {
            nodes ??= new HashSet<ElfModule>();
            foreach (var input in counter.Inputs.OfType<FlipFlopElfModule>())
            {
                if (nodes.Add(input))
                {
                    nodes = SubNodes(input, nodes);
                }
            }

            return nodes;
        }

        private static Dictionary<string, ElfModule> BuildModules(string[] data, out ElfModule broadcaster)
        {
            Dictionary<string, ElfModule> modules = new();
            broadcaster = null;
            foreach (var n in data.Select(d => d.Split(' ', 2)[0]))
            {
                switch (n[0])
                {
                    case '%':
                        modules.Add(n[1..], new FlipFlopElfModule(n[1..]));
                        break;
                    case '&':
                        modules.Add(n[1..], new ConjunctionModule(n[1..]));
                        break;
                    default:
                        modules.Add(n, broadcaster = new BroadcastModule(n));
                        break;
                }
            }

            foreach (var line in data)
            {
                var (name, outputs) = Data.Parse<string, string>(line, @"^[%&]?(\w+) -> (.*)$");
                var m = modules[name];
                foreach (var outputName in outputs.Split(',',
                             StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!modules.TryGetValue(outputName, out var o))
                    {
                        modules.Add(outputName, o = new NoopModule(outputName));
                    }
                    
                    m.ConnectOutput(o);
                    o.ConnectInput(m);
                }
            }

            return modules;
        }

        public class SignalManager
        {
            public bool Quiet { get; }
            
            public long LowPulses { get; private set; }
            public long HighPulses { get; private set; }
            
            private readonly Queue<(ElfModule target, ElfModule from, PulseType pulse)> _pending = new();

            public SignalManager(bool quiet)
            {
                Quiet = quiet;
            }

            public void Send(ElfModule target, ElfModule from, PulseType pulse)
            {
                if (!Quiet)
                    Helpers.VerboseLine($"Module {from.Name ?? "button"} sending {pulse} to {target.Name}");
                
                if (pulse == PulseType.High)
                    HighPulses++;
                else
                    LowPulses++;
            
                _pending.Enqueue((target, from, pulse));
            }

            public void ProcessAll()
            {
                while (_pending.TryDequeue(out var s))
                {
                    s.target.UpdateState(s.from, s.pulse, this);
                }
            }
        }

        public IReadOnlySet<ElfModule> GetAllInputModules(ElfModule source)
        {
            HashSet<ElfModule> inputs = new();
            foreach (var i in source.Inputs)
            {
                inputs.UnionWith(GetAllInputModules(i));
            }

            inputs.Remove(source);
            return inputs;
        }

        public record struct ModuleInputStates(ImmutableDictionary<ElfModule, PulseType> States)
        {
            public readonly bool Equals(ModuleInputStates other)
            {
                if (other.States.Count != States.Count)
                    return false;
                foreach (var p in States)
                {
                    if (!other.States.TryGetValue(p.Key, out var o) || !p.Value.Equals(o))
                    {
                        return false;
                    }
                }

                return true;
            }

            public readonly override int GetHashCode()
            {
                HashCode code = new HashCode();
                foreach (var p in States.OrderBy(s => s.Key.Name))
                {
                    code.Add(p.Key);
                    code.Add(p.Value);
                }

                return code.ToHashCode();
            }
        }

        public record struct ModuleCycle(ElfModule Module, long Length, ImmutableDictionary<long, PulseType> Pulses)
        {
            public (long when, PulseType pulse) GetNextPulse(long current)
            {
                long offset = current % Length;
                foreach (var pulse in Pulses.OrderBy(p => p.Key))
                {
                    if (pulse.Key >= offset)
                    {
                        return (pulse.Key - offset + current, pulse.Value);
                    }
                }

                var first = Pulses.MinBy(p => p.Key);
                return (first.Key - offset + Length + current, first.Value);
            }
        }

        public abstract class ElfModule
        {
            protected ElfModule(string name)
            {
                Name = name;
            }

            public string Name { get; }
            public ImmutableList<ElfModule> Inputs { get; private set; } = ImmutableList<ElfModule>.Empty;
            public ImmutableList<ElfModule> Outputs { get; private set; } = ImmutableList<ElfModule>.Empty;
            
            public PulseType State { get; private set; }

            public void UpdateState(ElfModule from, PulseType pulse, SignalManager signals)
            {
                var newState = OnUpdateState(from, pulse);
                if (newState.HasValue)
                {
                    State = newState.Value;
                    SendStateToOutputs(signals);
                }
            }

            public void SendStateToOutputs(SignalManager signals)
            {
                foreach (var output in Outputs)
                {
                    signals.Send(output, this, State);
                }
            }

            public abstract PulseType? OnUpdateState(ElfModule from, PulseType pulse);

            public void ConnectInput(ElfModule module)
            {
                Inputs = Inputs.Add(module);
                OnInputConnect(module);
            }

            public void ConnectOutput(ElfModule module)
            {
                Outputs = Outputs.Add(module);
                OnOutputConnect(module);
            }

            protected virtual void OnInputConnect(ElfModule module)
            {
            }

            protected virtual void OnOutputConnect(ElfModule module)
            {
            }

            public override string ToString() => Name;
        }

        public class FlipFlopElfModule : ElfModule
        {
            public FlipFlopElfModule(string name) : base(name)
            {
            }

            public override PulseType? OnUpdateState(ElfModule from, PulseType pulse)
            {
                if (pulse == PulseType.High)
                {
                    return default;
                }

                return State switch
                {
                    PulseType.Low => PulseType.High,
                    PulseType.High => PulseType.Low,
                };
            }
        }

        public class ConjunctionModule : ElfModule
        {
            private Dictionary<ElfModule, PulseType> _memory = new();
            
            public ConjunctionModule(string name) : base(name)
            {
            }

            protected override void OnInputConnect(ElfModule module)
            {
                _memory.Add(module, PulseType.Low);
            }

            public override PulseType? OnUpdateState(ElfModule from, PulseType pulse)
            {
                _memory[from] = pulse;

                if (_memory.Values.All(m => m == PulseType.High))
                {
                    return PulseType.Low;
                }

                return PulseType.High;
            }
        }

        public class BroadcastModule : ElfModule
        {
            public BroadcastModule(string name) : base(name)
            {
            }

            public override PulseType? OnUpdateState(ElfModule from, PulseType pulse)
            {
                return pulse;
            }
        }
        
        public class NoopModule : ElfModule
        {
            public NoopModule(string name) : base(name)
            {
            }

            public override PulseType? OnUpdateState(ElfModule from, PulseType pulse)
            {
                return default;
            }
        }
    }
}