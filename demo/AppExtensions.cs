using System.Text;
using demo.Models;
using demo.ViewModels;
using TeleFlow;
using TeleFlow.Attributes;
using TeleFlow.Builders;
using TeleFlow.Commands;
using TeleFlow.Commands.MultiStep;
using TeleFlow.Commands.MultiStep.StepCommands;
using TeleFlow.Factories.CommandFactories;
using TeleFlow.Models;
using TeleFlow.ViewModels.CallbackQuery;
using TeleFlow.Services.InputValidators;
using TeleFlow.Services.Markup;
using TeleFlow.Services.Messaging;
using Telegram.Bot.Types;

namespace demo;

public static class AppExtensions
{
    public static void ConfigureUpdateListenerForDemo(this UpdateListenerCommandFactoryBuilder<UpdateDistributorNextHandlerBuildArgs> builder)
    {
        builder
        .WithLambda("/start", (args) =>
        {
            return new SendTextCommand(args.BuildTimeArgs.FromUpdateDistributorArgs.MessageServiceString, "Welcome to the bot!");
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
                    return new ContactShareStepCommand(args.UpdateListenerBuilderArgs.BuildTimeArgs.FromUpdateDistributorArgs.ReplyMarkupManager, (userInput) =>
                    {
                        args.State.CurrentValue.PhoneNumber = userInput.PhoneNumber;
                    }, "Please Share Your Phone. This will NOT go anywhere", "Share My Phone",
                    new PhoneNumberBelongsToUserValidator(args.UpdateListenerBuilderArgs.BuildTimeArgs.FromUpdateDistributorArgs.MessageServiceString, args.UpdateListenerBuilderArgs.BuildTimeArgs.FromUpdateDistributorArgs.ChatIdProvider, "The input was invalid", "The phone number does not belong to you."),
                    next);
                }, "Phone Number")
                .WithStepWithValidationLambdaFactoryGoBackButton((args, next, validator) =>
                {
                    return new TextInputStepCommand(args.UpdateListenerBuilderArgs.BuildTimeArgs.FromUpdateDistributorArgs.MessageServiceString, "Please enter your full name", async (message) =>
                    {
                        args.State.CurrentValue.UserFullName = message;
                    }, next, validator);
                }, "User Full Name")
                .WithStepWithValidationLambdaFactory((args, next) =>
                {
                    return new EnumValueSelectionStepCommand<DemoEnum>(args.UpdateListenerBuilderArgs.BuildTimeArgs.FromUpdateDistributorArgs.MessageService, "Please select one of the values",
                    async (userInput) =>
                    {
                        args.State.CurrentValue.LibraryRating = userInput;
                    },
                    (enumValue) =>
                    {
                        if (enumValue == DemoEnum.None)
                            return null;

                        return enumValue.ToString();
                    }, new(), next);
                }, "Library Rating")
                .WithStepWithValidationLambdaFactory((args, next) =>
                {
                    return new ListValueSelectionStepCommand<DemoListObject>(args.UpdateListenerBuilderArgs.BuildTimeArgs.FromUpdateDistributorArgs.MessageService, "Please select the value.",
                    async (userInput) =>
                    {
                        args.State.CurrentValue.ListObject = userInput;
                    },
                    new(3),
                    async () =>
                    {
                        return new List<DemoListObject>() { new() { DisplayName = "List Object 1", Value = "Value 1" }, new() { DisplayName = "List Object 2", Value = "Value 2" } };
                    },
                    (listObject) =>
                    {
                        return listObject.DisplayName;
                    }, next);
                }, "List Object Selection")
                .WithStepWithValidationLambdaFactoryGoBackButton((args, next, validator) =>
                {
                    return new DateSelectionStepCommand(next, validator, args.UpdateListenerBuilderArgs.BuildTimeArgs.FromUpdateDistributorArgs.MessageService, (date) =>
                    {
                        args.State.CurrentValue.SelectedDate = date;
                    }, "Pick a date", new(DateOnly.FromDateTime(DateTime.Today).AddYears(-18), TeleFlow.Services.DateSelectionView.YearSelection), null, new(DateOnly.FromDateTime(DateTime.Today).AddYears(-14), "You are too young!"));
                }, "Date");
            })
            .WithLambdaResult(args =>
            {
                return new LambdaHandler<DemoViewModel>(async (vm) =>
                {
                    StringBuilder sb = new();

                    sb.AppendLine("You have successfully completed the multi step command. Here is the data you entered:");
                    sb.AppendLine($"Full Name: {vm.UserFullName}");
                    sb.AppendLine($"Library Rating: {vm.LibraryRating}");
                    sb.AppendLine($"List Object: {vm.ListObject.DisplayName} with value {vm.ListObject.Value}");
                    sb.AppendLine($"Date: {vm.SelectedDate.ToShortDateString()}");
                    sb.AppendLine($"Phone: {vm.PhoneNumber}");

                    await args.BuildTimeArgs.FromUpdateDistributorArgs.MessageServiceString.SendMessage(sb.ToString());
                    await args.BuildTimeArgs.Navigator.Handle("/start");
                });
            });
        })

        .WithAuthenticationWithDefaultIfNotAuthenticated(options =>
        {
            options
            .WithCondition("/what-is-conditional-command", (ConditionalCommandBuilder<UpdateDistributorNextHandlerBuildArgs> options) =>
            {
                options.WithCondition(async (args) =>
                {
                    return args.BuildTimeArgs.FromUpdateDistributorArgs.ChatIdProvider.GetChatId() == 0;
                })
                .WithLambdaIfTrue((args) =>
                {
                    return new NavigateWithTextCommand(args.BuildTimeArgs.Navigator, "/start", args.BuildTimeArgs.FromUpdateDistributorArgs.MessageServiceString, "This is a text before a redirect!");
                })
                .WithLambdaIfFalse((args) =>
                {
                    return new SendTextCommand(args.BuildTimeArgs.FromUpdateDistributorArgs.MessageServiceString, "You Did not pass the conditions!");
                });
            });
        });
    }

    public static UpdateListenerCommandFactoryBuilder<UpdateDistributorNextHandlerBuildArgs> WithAuthenticationWithDefaultIfNotAuthenticated(this UpdateListenerCommandFactoryBuilder<UpdateDistributorNextHandlerBuildArgs> builder, Action<UpdateListenerCommandFactoryBuilder<UpdateDistributorNextHandlerBuildArgs>> options)
    {
        return builder

        .WithCondition(
        async (args) =>
        {
            return true;
        },
        (UpdateListenerCommandExecutionArgs<UpdateDistributorNextHandlerBuildArgs> args) =>
        {
            return new NavigateWithTextCommand(args.BuildTimeArgs.Navigator, "/start", args.BuildTimeArgs.FromUpdateDistributorArgs.MessageServiceString, "You did not pass the condition");
        },
        options);
    }


    public static MultiStepCommandBuilder<TState, TBuildArgs> WithValidation<TState, TBuildArgs>(this MultiStepCommandBuilder<TState, TBuildArgs> mscb, Action<StepManagerWithValidationCommandBuilder<TState, TBuildArgs>> options) where TState : notnull where TBuildArgs : class
    {
        return mscb.WithValidation(new MessageBuilderOptions(1), "All good", "Edit", DefaultValidationMessageFormatter, options);
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

    public static StepManagerWithValidationCommandBuilder<TState, UpdateDistributorNextHandlerBuildArgs> WithStepWithValidationLambdaFactoryGoBackButton<TState>(this StepManagerWithValidationCommandBuilder<TState, UpdateDistributorNextHandlerBuildArgs> smwvcb, Func<MultiStepCommandBuilderArgs<TState, UpdateDistributorNextHandlerBuildArgs>, StepCommand?, IUserInputValidator, StepCommand> lambdaFactory, string validationDisplayName) where TState : notnull
    {
        return smwvcb.WithStepWithValidationLambdaFactory<TState, UpdateDistributorNextHandlerBuildArgs>((args, next) =>
        {
            CallbackButtonGenerator generator = new();
            generator.StartNewSession();
            var vm = generator.GenerateVM();

            var messageServiceWrapper = new MessageServiceWrapper(args.UpdateListenerBuilderArgs.BuildTimeArgs.FromUpdateDistributorArgs.MessageService, (builder) =>
            {
                builder.WithInlineButtonLine<CallbackQueryViewModel>(new(vm, "Go Back"));
            }, args.UpdateListenerBuilderArgs.BuildTimeArgs.FromUpdateDistributorArgs.InlineMarkupManager);


            var oldUpdateListenerArgs = args.UpdateListenerBuilderArgs;

            var updateListenerWithInjectionArgs = new UpdateDistributorNextHandlerBuildArgs(messageServiceWrapper,
                                                                                            oldUpdateListenerArgs.BuildTimeArgs.FromUpdateDistributorArgs.MessageServiceImages,
                                                                                            messageServiceWrapper,
                                                                                            oldUpdateListenerArgs.BuildTimeArgs.FromUpdateDistributorArgs.ReplyMarkupManager,
                                                                                            oldUpdateListenerArgs.BuildTimeArgs.FromUpdateDistributorArgs.InlineMarkupManager,
                                                                                            oldUpdateListenerArgs.BuildTimeArgs.FromUpdateDistributorArgs.MediaDownloaderService,
                                                                                            oldUpdateListenerArgs.BuildTimeArgs.FromUpdateDistributorArgs.ChatIdProvider);

            var newArgs = new MultiStepCommandBuilderArgs<TState, UpdateDistributorNextHandlerBuildArgs>(new(args.UpdateListenerBuilderArgs.NavigatorArgs, new(updateListenerWithInjectionArgs, args.UpdateListenerBuilderArgs.BuildTimeArgs.Navigator)), args.State, args.StepChainBuilder);

            return lambdaFactory.Invoke(newArgs, next, new CallbackQueryInputInterceptor<CallbackQueryViewModel>(new ExecutePreviousCommandInterceptor(vm, args.StepChainBuilder)));

        }, validationDisplayName);
    }

    public static StepManagerWithValidationCommandBuilder<TState, TBuildArgs> WithStepWithValidationLambdaFactory<TState, TBuildArgs>(this StepManagerWithValidationCommandBuilder<TState, TBuildArgs> smwvcb, Func<MultiStepCommandBuilderArgs<TState, TBuildArgs>, StepCommand?, StepCommand> lambdaFactory, string validationDisplayName)
    where TBuildArgs : class
    {
        return smwvcb.WithStepWithValidation((args) =>
        {
            return new StepCommandFactory((next) => { return lambdaFactory.Invoke(args, next); });
        }, validationDisplayName);
    }

}