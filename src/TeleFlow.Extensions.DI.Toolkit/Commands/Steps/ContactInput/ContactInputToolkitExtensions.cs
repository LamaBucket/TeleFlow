using TeleFlow.Abstractions.Engine.Commands.Stateful.Steps;
using TeleFlow.Core.Commands.Stateful;
using TeleFlow.Core.Commands.Stateful.Steps.Base;
using TeleFlow.Core.Commands.Stateful.Steps.ContactInput;
using TeleFlow.Core.Commands.Stateful.Steps.ContactInput.Render;
using TeleFlow.Extensions.DI.Builders.Commands.Stateful;
using TeleFlow.Extensions.DI.Builders.Commands.Stateful.Filters;
using Telegram.Bot.Types;

namespace TeleFlow.Extensions.DI.Toolkit.Commands.Steps.ContactInput;

public static class ContactInputToolkitExtensions
{
    public static StepWithRenderPipelineFilterBuilder<ContactInputStepData> AddContactInput(this StepRouterBuilder builder, Func<CommandStepCommitContext, Contact, Task> onCommit, Action<ContactInputToolkitConfig>? configure = null)
    {
        ContactInputToolkitConfig config = new();
        
        if(configure is not null)
            configure(config);

        var renderOptions = config.BuildRenderOptions();
        return builder.Add(baseRenderService: () => new ContactInputRenderService(renderOptions), 
                           factory: (renderService) => new ContactInputStep(config.BuildOptions(renderService, onCommit)));
    }
    
}
