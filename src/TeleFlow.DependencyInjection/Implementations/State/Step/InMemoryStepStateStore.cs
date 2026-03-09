using System;
using System.Collections.Concurrent;
using TeleFlow.Abstractions.State.Step;

namespace TeleFlow.DependencyInjection.Implementations.State.Step;

public sealed class InMemoryStepStateStore : IStepStateStore
{
    private sealed class Entry
    {
        public required int MessageId { get; init; }
        public required object ViewModel { get; init; }
        public required Type StepDataType { get; init; }
    }

    private readonly ConcurrentDictionary<long, Entry> _states = new();

    public Task<StepState<TData>?> GetState<TData>(long chatId) 
        where TData : StepData
    {
        if (!_states.TryGetValue(chatId, out var entry))
            return Task.FromResult<StepState<TData>?>(null);

        if (entry.StepDataType != typeof(TData))
            return Task.FromResult<StepState<TData>?>(null);

        var typed = (TData)entry.ViewModel;

        StepState<TData> state = new(entry.MessageId, typed);
        return Task.FromResult<StepState<TData>?>(state);
    }

    public Task SetState<TData>(long chatId, StepState<TData> state) where TData : StepData
    {
        if (state is null) throw new ArgumentNullException(nameof(state));
        if (state.StepData is null) throw new ArgumentException("ViewModel must not be null.", nameof(state));
        if (state.MessageId <= 0) throw new ArgumentException("MessageId must be a positive integer.", nameof(state));

        _states[chatId] = new Entry
        {
            MessageId = state.MessageId,
            ViewModel = state.StepData,
            StepDataType = typeof(TData)
        };

        return Task.CompletedTask;
    }

    public Task RemoveState(long chatId)
    {
        _states.TryRemove(chatId, out _);
        return Task.CompletedTask;
    }
}