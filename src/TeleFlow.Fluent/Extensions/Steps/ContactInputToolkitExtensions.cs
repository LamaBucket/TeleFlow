using TeleFlow.Core.Commands.Stateful;
using TeleFlow.Core.Commands.Stateful.Steps.ContactInput.Render;
using TeleFlow.Core.Commands.Stateful.Steps.SingleInput.ContactInput;
using TeleFlow.Fluent.Builders.Commands.Stateful;
using TeleFlow.Fluent.Builders.Commands.Stateful.Filters;
using TeleFlow.Fluent.Configuration.ContactInput;
using Telegram.Bot.Types;

namespace TeleFlow.Fluent.Extensions.Steps;

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
