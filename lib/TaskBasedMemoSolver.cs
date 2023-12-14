using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ChadNedzlek.AdventOfCode.Library;

public class TaskBasedMemoSolver<TState, TSolution> where TState : ITaskMemoState<TState, TSolution>, IEquatable<TState>
{
    private static readonly Dictionary<TState, TSolution> _solveCache = new();
    public int CacheHits { get; private set; } 

    private static readonly TaskFactory _custom = new CustomTaskFactory();

    public Task<TSolution> Solve(TState state)
    {
        if (_solveCache.TryGetValue(state, out var sol))
        {
            CacheHits++;
            return Task.FromResult(sol);
        }

        return _custom.StartNew(() => SolveAsync(state)).Unwrap();
        
        async Task<TSolution> SolveAsync(TState s)
        
        {
            await Task.Yield();
            var result = await s.Solve(this);
            _solveCache.Add(s, result);
            return result;
        }
    }

    internal class CustomTaskFactory : TaskFactory
    {
        private class SingleThreadedScheduler : TaskScheduler
        {
            private readonly Channel<Task> _pending = Channel.CreateUnbounded<Task>();
            private Task _runner;

            public SingleThreadedScheduler()
            {
                _runner = Task.Run(ProcessingThread);
            }

            private async Task ProcessingThread()
            {
                await foreach (var item in _pending.Reader.ReadAllAsync())
                {
                    TryExecuteTask(item);
                }
            }

            protected override IEnumerable<Task> GetScheduledTasks()
            {
                return ImmutableList<Task>.Empty;
            }

            protected override void QueueTask(Task task)
            {
                _pending.Writer.TryWrite(task);
            }

            protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
            {
                return !taskWasPreviouslyQueued && TryExecuteTask(task);
            }

            protected override bool TryDequeue(Task task)
            {
                bool tryDequeue = base.TryDequeue(task);
                return tryDequeue;
            }
        }

        public CustomTaskFactory() : base(new SingleThreadedScheduler())
        {
        }
    }
}

public interface ITaskMemoState<TState, TSolution> where TState : ITaskMemoState<TState, TSolution>, IEquatable<TState> 
{
    Task<TSolution> Solve(TaskBasedMemoSolver<TState, TSolution> solver);
}
