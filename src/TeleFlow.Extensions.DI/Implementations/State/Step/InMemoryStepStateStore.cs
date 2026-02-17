using System;
using System.Collections.Concurrent;
using TeleFlow.Abstractions.State.Step;

namespace TeleFlow.Extensions.DI.Implementations.State.Step;

public sealed class InMemoryStepStateStore : IStepStateStore
{
    private sealed class Entry
    {
        public required int MessageId { get; init; }
        public required object ViewModel { get; init; }
        public required Type VmType { get; init; }
    }

    private readonly ConcurrentDictionary<long, Entry> _states = new();

    public Task<StepState<TVm>?> GetState<TVm>(long chatId) where TVm : class
    {
        if (!_states.TryGetValue(chatId, out var entry))
            return Task.FromResult<StepState<TVm>?>(null);

        if (entry.VmType != typeof(TVm))
            return Task.FromResult<StepState<TVm>?>(null);

        var typed = (TVm)entry.ViewModel;

        StepState<TVm> state = new(entry.MessageId, typed);
        return Task.FromResult<StepState<TVm>?>(state);
    }

    public Task SetState<TVm>(long chatId, StepState<TVm> state) where TVm : class
    {
        if (state is null) throw new ArgumentNullException(nameof(state));
        if (state.ViewModel is null) throw new ArgumentException("ViewModel must not be null.", nameof(state));
        if (state.MessageId <= 0) throw new ArgumentException("MessageId must be a positive integer.", nameof(state));

        _states[chatId] = new Entry
        {
            MessageId = state.MessageId,
            ViewModel = state.ViewModel,
            VmType = typeof(TVm)
        };

        return Task.CompletedTask;
    }

    public Task RemoveState(long chatId)
    {
        _states.TryRemove(chatId, out _);
        return Task.CompletedTask;
    }
}