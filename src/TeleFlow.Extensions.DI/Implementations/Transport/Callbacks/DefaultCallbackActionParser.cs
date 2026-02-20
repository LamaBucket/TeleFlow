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
    private const string UiNoop    = "ui.noop";
    private const string UiGoTo    = "ui.goto";
    private const string UiSelect  = "ui.sel";

    public CallbackToken Parse(CallbackAction action)
    {
        if (action is null) throw new ArgumentNullException(nameof(action));

        return action switch
        {
            // ----- Command -----
            CallbackAction.CommandAction.Execute e
                => new CallbackToken(CmdExec, RequireNonEmpty(e.CommandKey, nameof(e.CommandKey))),

            // ----- Step -----
            CallbackAction.StepAction.Back
                => new CallbackToken(StepBack, string.Empty),

            CallbackAction.StepAction.Finish
                => new CallbackToken(StepFin, string.Empty),

            CallbackAction.StepAction.GoTo g
                => new CallbackToken(StepGoTo, RequireNonEmpty(g.StepId, nameof(g.StepId))),

            // ----- UI -----
            CallbackAction.UiAction.NextPage
                => new CallbackToken(UiNext, string.Empty),

            CallbackAction.UiAction.PrevPage
                => new CallbackToken(UiPrev, string.Empty),

            CallbackAction.UiAction.NoOperation
                => new CallbackToken(UiNoop, string.Empty),

            CallbackAction.UiAction.GoToPage p
                => new CallbackToken(UiGoTo, p.Page.ToString()),

            CallbackAction.UiAction.SelectIndex s
                => new CallbackToken(UiSelect, s.Index.ToString()),

            _ => throw new NotSupportedException($"Unsupported callback action type: {action.GetType().FullName}")
        };
    }

    public bool TryParse(CallbackToken token, out CallbackAction action)
    {
        action = default!;

        if (token is null)
            return false;

        // Kind/Data могут прилететь null из чужой реализации
        string kind = token.Kind ?? string.Empty;
        string data = token.Data ?? string.Empty;

        switch (kind)
        {
            // ----- Command -----
            case CmdExec:
                if (string.IsNullOrWhiteSpace(data)) return false;
                action = new CallbackAction.CommandAction.Execute(data);
                return true;

            // ----- Step -----
            case StepBack:
                action = new CallbackAction.StepAction.Back();
                return true;

            case StepFin:
                action = new CallbackAction.StepAction.Finish();
                return true;

            case StepGoTo:
                if (string.IsNullOrWhiteSpace(data)) return false;
                action = new CallbackAction.StepAction.GoTo(data);
                return true;

            // ----- UI -----
            case UiNext:
                action = new CallbackAction.UiAction.NextPage();
                return true;

            case UiPrev:
                action = new CallbackAction.UiAction.PrevPage();
                return true;

            case UiNoop:
                action = new CallbackAction.UiAction.NoOperation();
                return true;

            case UiGoTo:
                if (!int.TryParse(data, out int page)) return false;
                action = new CallbackAction.UiAction.GoToPage(page);
                return true;

            case UiSelect:
                // ВАЖНО: не запрещаем отрицательные индексы.
                // Это позволяет шагам использовать SelectIndex как локальный протокол (например -1/-2) при необходимости.
                if (!int.TryParse(data, out int idx)) return false;
                action = new CallbackAction.UiAction.SelectIndex(idx);
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