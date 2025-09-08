using System.Text;
using demo.Models;
using demo.ViewModels;
using LisBot.Common.Telegram;
using LisBot.Common.Telegram.Attributes;
using LisBot.Common.Telegram.Builders;
using LisBot.Common.Telegram.Commands;
using LisBot.Common.Telegram.Commands.MultiStep;
using LisBot.Common.Telegram.Commands.MultiStep.StepCommands;
using LisBot.Common.Telegram.Factories.CommandFactories;
using LisBot.Common.Telegram.Models;
using LisBot.Common.Telegram.ViewModels.CallbackQuery;
using Telegram.Bot.Extensions.Handlers.Services.InputValidators;
using Telegram.Bot.Extensions.Handlers.Services.Markup;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;

namespace demo;

public static class AppExtensions
{
    public static void ConfigureUpdateListenerForDemo(this UpdateListenerCommandFactoryBuilder builder)
    {
        builder
        .WithSendText("/start", "Welcome to the bot!")
        .WithMultiStep<List<int>>("/test", options =>
        {
            options
            .SetDefaultStateValue(new List<int>())
            .WithValidation(options =>
            {
                options.WithStepWithValidationLambdaFactory((args, next) =>
                {
                    return new TextInputStepCommand(args.UpdateListenerBuilderArgs.MessageServiceString, "Please enter an integer", async (message) =>
                    {
                        if (int.TryParse(message, out int intValue))
                        {
                            args.State.CurrentValue.Add(intValue);
                        }
                        else
                        {
                            args.State.CurrentValue.Add(-1);
                        }
                    }, new NoValidationUserValidator(), next);
                }, "Test Step");
            })
            .WithLambdaResult((args) =>
            {
                return new LambdaHandler<List<int>>(async (vm) =>
                {
                    await args.MessageServiceString.SendMessage(string.Join(", ", vm));

                    await args.Navigator.Handle("/start");
                });
            });
        })
        .WithMultiStep<DemoViewModel>("/multistep", options =>
        {
            options
            .SetDefaultStateValue(new())
            .WithValidation(options =>
            {
                options
                .WithStepWithValidationLambdaFactory((args, next) =>
                {
                    return new ContactShareStepCommand(args.UpdateListenerBuilderArgs.ReplyMarkupManager, (userInput) =>
                    {
                        args.State.CurrentValue.PhoneNumber = userInput.PhoneNumber;
                    }, "Please Share Your Phone. This will NOT go anywhere", "Share My Phone",
                    new PhoneNumberBelongsToUserValidator(args.UpdateListenerBuilderArgs.MessageServiceString, args.UpdateListenerBuilderArgs.ChatIdProvider),
                    next);
                }, "Phone Number")
                .WithStepWithValidationLambdaFactory((args, next) =>
                {
                    CallbackButtonGenerator generator = new();
                    generator.StartNewSession();
                    var vm = generator.GenerateVM();

                    var messageServiceWrapper = new MessageServiceWrapper(args.UpdateListenerBuilderArgs.MessageService, (builder) =>
                    {
                        builder.WithInlineButtonLine<CallbackQueryViewModel>(new(vm, "Go Back"));
                    });


                    return new TextInputStepCommand(messageServiceWrapper, "Please enter your full name", async (message) =>
                    {
                        args.State.CurrentValue.UserFullName = message;
                    }, new CallbackQueryInputInterceptor<CallbackQueryViewModel>(new GoToPreviousButtonInputInterceptor(vm, args.StepChainBuilder)) , next);
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
                }, "List Object Selection")
                .WithStepWithValidationLambdaFactory((args, next) =>
                {
                    return new DateSelectionStepCommand(next, new NoValidationUserValidator(), args.UpdateListenerBuilderArgs.MessageService, (date) =>
                    {
                        args.State.CurrentValue.SelectedDate = date;
                    }, new(DateOnly.FromDateTime(DateTime.Today)));
                }, "Date");
            })
            .WithLambdaResult(args =>
            {
                return new LambdaHandler<DemoViewModel>(async (vm) =>
                {
                    await args.MessageServiceString.SendMessage("You have successfully completed the multi step command. Here is the data you entered:");
                    await args.MessageServiceString.SendMessage($"Full Name: {vm.UserFullName}");
                    await args.MessageServiceString.SendMessage($"Library Rating: {vm.LibraryRating}");
                    await args.MessageServiceString.SendMessage($"List Object: {vm.ListObject.DisplayName} with value {vm.ListObject.Value}");
                    await args.MessageServiceString.SendMessage($"Date: {vm.SelectedDate.ToShortDateString()}");
                    await args.MessageServiceString.SendMessage($"Phone: {vm.PhoneNumber}");

                    await args.Navigator.Handle("/start");
                });
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