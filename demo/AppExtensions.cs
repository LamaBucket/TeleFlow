using System.Text;
using demo.Models;
using demo.Services;
using demo.ViewModels;
using LisBot.Common.Telegram.Attributes;
using LisBot.Common.Telegram.Builders;
using LisBot.Common.Telegram.Commands;
using LisBot.Common.Telegram.Commands.MultiStep;
using LisBot.Common.Telegram.Commands.MultiStep.StepCommands;
using LisBot.Common.Telegram.Factories;
using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace demo;

public static class AppExtensions
{
    public static void ConfigureUpdateListenerForDemo(this UpdateListenerCommandFactoryBuilder builder)
    {
        builder
        .WithSendText("/start", "Welcome to the bot!")
        .WithMultiStep<DemoViewModel>("/what-is-multi-step", options =>
        {
            options
            .WithValidation(options =>
            {
                options
                .WithStepWithValidationLambdaFactory((args, next) =>
                {
                    return new TextInputStepCommand(args.UpdateListenerBuilderArgs.MessageServiceString, "Please enter your full name", async (message) =>
                    {
                        args.State.CurrentValue.UserFullName = message;
                    }, next);
                }, "User Full Name")
                .WithStepWithValidationLambdaFactory((args, next) =>
                {
                    return new EnumValueSelectionStepCommand<DemoEnum>(args.UpdateListenerBuilderArgs.MessageService, "Please select one of the values",
                    async (userInput) =>
                    {
                        args.State.CurrentValue.LibraryRating = userInput;
                    },
                    (enumValue) =>
                    {
                        if (enumValue == DemoEnum.None)
                            return null;

                        return enumValue.ToString();
                    }, next, new());
                }, "Library Rating")
                .WithStepWithValidationLambdaFactory((args, next) =>
                {
                    return new ListValueSelectionStepCommand<DemoListObject>(args.UpdateListenerBuilderArgs.MessageService, "Please select the value.",
                    async (userInput) =>
                    {
                        args.State.CurrentValue.ListObject = userInput;
                    },
                    new(),
                    async () =>
                    {
                        return new List<DemoListObject>() { new() { DisplayName = "List Object 1", Value = "Value 1" }, new() { DisplayName = "List Object 2", Value = "Value 2" } };
                    },
                    (listObject) =>
                    {
                        return listObject.DisplayName;
                    }, next);
                }, "List Object Selection");
            });
        })

        .WithAuthenticationWithDefaultIfNotAuthenticated(options =>
        {
            options
            .WithCondition("/what-is-conditional-command", options =>
            {
                options.WithCondition(async (args) =>
                {
                    return args.ChatIdProvider.GetChatId() == 0;
                })
                .WithLambdaIfTrue((args) =>
                {
                    return new NavigateWithTextCommand(args.Navigator, "/start", args.MessageServiceString, "This is a text before a redirect!");
                })
                .WithSendTextIfFalse("You Did not pass the conditions!");
            });
        });
    }

    public static UpdateListenerCommandFactoryBuilder WithAuthenticationWithDefaultIfNotAuthenticated(this UpdateListenerCommandFactoryBuilder builder, Action<UpdateListenerCommandFactoryBuilder> options)
    {
        return builder

        .WithAuthentication(options, (args) =>
        {
            return new NavigateWithTextCommand(args.Navigator, "/start", args.MessageServiceString, "You are not authenticated. Redirecting");
        });
    }


    public static MultiStepCommandBuilder<TState> WithValidation<TState>(this MultiStepCommandBuilder<TState> mscb, Action<StepManagerWithValidationCommandBuilder<TState>> options) where TState : notnull
    {
        return mscb.WithValidation(new(1), DefaultValidationMessageFormatter, "All good", options);
    }

    public static string DefaultValidationMessageFormatter<TState>(TState state) where TState : notnull
    {
        var props = GetValidatablePropertiesDisplayNameAndValues(state);

        StringBuilder sb = new();

        sb.AppendLine("Check if the data you entered is correct:");
        sb.AppendLine();

        foreach (var prop in props)
        {
            sb.AppendLine($"{prop.Item1}: {prop.Item2.ToString()}");
        }

        return sb.ToString();
    }

    private static IEnumerable<Tuple<string, object>> GetValidatablePropertiesDisplayNameAndValues<TState>(TState state) where TState : notnull
    {
        List<Tuple<string, object>> result = [];

        var type = state.GetType();

        foreach (var propertyInfo in type.GetProperties())
        {
            var propertyAttributes = propertyInfo.GetCustomAttributes(false);

            string? displayName = null;

            foreach (var propertyAttribute in propertyAttributes)
            {
                if (propertyAttribute is ValidateStatePropertyAttribute validatableAttribute)
                {
                    displayName = validatableAttribute.PropertyDisplayName;
                    break;
                }
            }

            if (displayName is null)
                continue;

            object? value = propertyInfo.GetValue(state) ?? "";

            result.Add(new(displayName, value));
        }

        return result;
    }

    public static StepManagerWithValidationCommandBuilder<TState> WithStepWithValidationLambdaFactory<TState>(this StepManagerWithValidationCommandBuilder<TState> smwvcb, Func<MultiStepCommandBuilderArgs<TState>, StepCommand?, StepCommand> lambdaFactory, string validationDisplayName)
    {
        return smwvcb.WithStepWithValidation((args) =>
        {
            return new StepCommandFactory((next) => { return lambdaFactory.Invoke(args, next); });
        }, validationDisplayName);
    }

}