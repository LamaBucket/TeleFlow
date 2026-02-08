using System;
using System.Collections.Concurrent;
using TeleFlow.Models.Interactive;
using TeleFlow.Services.ViewModels;

namespace TeleFlow.Services.Defaults.ViewModels;

public sealed class InMemoryChatInteractiveStateStore : IChatInteractiveStateStore
{
    private sealed class Entry
    {
        public required int MessageId { get; init; }
        public required object ViewModel { get; init; }
        public required Type VmType { get; init; }
    }

    private readonly ConcurrentDictionary<long, Entry> _states = new();

    public Task<InteractiveState<TVm>?> GetState<TVm>(long chatId) where TVm : class
    {
        if (!_states.TryGetValue(chatId, out var entry))
            return Task.FromResult<InteractiveState<TVm>?>(null);

        if (entry.VmType != typeof(TVm))
            return Task.FromResult<InteractiveState<TVm>?>(null);

        var typed = (TVm)entry.ViewModel;

        InteractiveState<TVm> state = new(entry.MessageId, typed);
        return Task.FromResult<InteractiveState<TVm>?>(state);
    }

    public Task SetState<TVm>(long chatId, InteractiveState<TVm> state) where TVm : class
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