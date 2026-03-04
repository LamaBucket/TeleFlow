using Microsoft.Extensions.DependencyInjection;
using TeleFlow.Abstractions.Engine.ChatIdentity;
using TeleFlow.Abstractions.Engine.Commands.Stateful;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Results;
using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Abstractions.Engine.Pipeline.Contexts;
using TeleFlow.Abstractions.State.Step;
using TeleFlow.Abstractions.Transport.Messaging;

namespace TeleFlow.Core.Commands.Stateful.Steps.Base;

public abstract class StatefulStep<TData> : ICommandStep
    where TData : StepData
{
    private readonly StatefulStepOptions<TData> _options;

    private IStepRenderService<TData> RenderService => _options.RenderService;


    public async Task<CommandStepResult> Handle(UpdateContext args)
    {
        var state = await LoadStateOrNull(args.ServiceProvider);

        if(state is null)
            return CommandStepResult.HoldOn(CommandStepHoldOnReason.InvalidInput, _options.StateExpiredMessage);

        return await Handle(args, state);
    }

    protected abstract Task<CommandStepResult> Handle(UpdateContext context, StepState<TData> state);


    public virtual async Task OnEnter(IServiceProvider serviceProvider)
    {
        var msgService = serviceProvider.GetRequiredService<IMessageSendService>();

        var data = await CreateDefaultStepData(serviceProvider);
        var msg = RenderService.Render(serviceProvider, data);

        var response = await msgService.SendMessage(msg);
        
        var state = new StepState<TData>(response.MessageId, data);

        await UpsertState(serviceProvider, state);
    }

    protected abstract Task<TData> CreateDefaultStepData(IServiceProvider sp);


    protected async Task UpsertAndRerender(IServiceProvider sp, StepState<TData> state)
    {
        await RerenderMessage(sp, state);
        await UpsertState(sp, state);
    }

    protected async Task FinalizeStep(IServiceProvider sp)
    {
        var state = await LoadStateOrNull(sp);

        if(state is not null)
            await RemoveState(sp);
    }


    private async Task RerenderMessage(IServiceProvider sp, StepState<TData> state)
    {
        var msgEditor = sp.GetRequiredService<IMessageEditService>();

        var message = RenderService.Render(sp, state.StepData);

        await msgEditor.Edit(state.MessageId, message);
    }


    private static async Task<StepState<TData>?> LoadStateOrNull(IServiceProvider sp)
    {
        var chatId = sp.GetRequiredService<IChatIdProvider>().GetChatId();
        var store = sp.GetRequiredService<IStepStateStore>();

        return await store.GetState<TData>(chatId);
    }

    private static async Task UpsertState(IServiceProvider sp, StepState<TData> state)
    {
        var chatId = sp.GetRequiredService<IChatIdProvider>().GetChatId();
        var store  = sp.GetRequiredService<IStepStateStore>();

        await store.SetState(chatId, state);
    }

    private static async Task RemoveState(IServiceProvider sp)
    {
        var chatId = sp.GetRequiredService<IChatIdProvider>().GetChatId();
        var store = sp.GetRequiredService<IStepStateStore>();

        await store.RemoveState(chatId);
    }


    public StatefulStep(StatefulStepOptions<TData> options)
    {
        _options = options;
    }
}