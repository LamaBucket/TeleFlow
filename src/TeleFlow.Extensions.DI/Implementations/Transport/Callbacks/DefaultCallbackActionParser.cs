using System;
using TeleFlow.Abstractions.Transport.Callbacks;
using TeleFlow.Core.Transport.Callbacks;

namespace TeleFlow.Extensions.DI.Implementations.Transport.Callbacks;

public sealed class DefaultCallbackActionParser : ICallbackActionParser
{
    // Kind codes (стабильные "опкоды" протокола action->token)
    private const string CmdExec   = "cmd.exec";

    private const string StepBack  = "step.back";
    private const string StepFin   = "step.finish";
    private const string StepGoTo  = "step.goto";

    private const string UiNext    = "ui.next";
    private const string UiPrev    = "ui.prev";
    private const string UiToggle  = "ui.tgl";

    public CallbackToken Parse(CallbackAction action)
    {
        if (action is null) throw new ArgumentNullException(nameof(action));

        return action switch
        {
            CallbackAction.Command.Execute e => new CallbackToken(CmdExec, RequireNonEmpty(e.CommandKey, nameof(e.CommandKey))),

            CallbackAction.Step.Back       => new CallbackToken(StepBack, string.Empty),
            CallbackAction.Step.Finish     => new CallbackToken(StepFin,  string.Empty),
            CallbackAction.Step.GoTo g     => new CallbackToken(StepGoTo, RequireNonEmpty(g.StepId, nameof(g.StepId))),

            CallbackAction.Ui.NextPage     => new CallbackToken(UiNext,   string.Empty),
            CallbackAction.Ui.PrevPage     => new CallbackToken(UiPrev,   string.Empty),
            CallbackAction.Ui.ToggleIndex t=> new CallbackToken(UiToggle, t.Index.ToString()),

            _ => throw new NotSupportedException($"Unsupported callback action type: {action.GetType().FullName}")
        };
    }

    public bool TryParse(CallbackToken token, out CallbackAction action)
    {
        action = default!;

        if (token is null)
            return false;

        // Kind/Data могут прилететь null из чужой реализации
        var kind = token.Kind ?? string.Empty;
        var data = token.Data ?? string.Empty;

        switch (kind)
        {
            case CmdExec:
                if (string.IsNullOrWhiteSpace(data)) return false;
                action = new CallbackAction.Command.Execute(data);
                return true;

            case StepBack:
                action = new CallbackAction.Step.Back();
                return true;

            case StepFin:
                action = new CallbackAction.Step.Finish();
                return true;

            case StepGoTo:
                if (string.IsNullOrWhiteSpace(data)) return false;
                action = new CallbackAction.Step.GoTo(data);
                return true;

            case UiNext:
                action = new CallbackAction.Ui.NextPage();
                return true;

            case UiPrev:
                action = new CallbackAction.Ui.PrevPage();
                return true;

            case UiToggle:
                if (!int.TryParse(data, out var idx)) return false;
                if (idx < 0) return false;
                action = new CallbackAction.Ui.ToggleIndex(idx);
                return true;

            default:
                return false;
        }
    }

    private static string RequireNonEmpty(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", paramName);

        return value;
    }
}
