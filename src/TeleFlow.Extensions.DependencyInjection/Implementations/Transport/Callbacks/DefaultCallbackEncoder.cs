using System;
using System.Text;
using TeleFlow.Abstractions.Transport.Callbacks;

namespace TeleFlow.Extensions.DependencyInjection.Implementations.Transport.Callbacks;

public sealed class DefaultCallbackEncoder : ICallbackCodec
{
    private const string Prefix = "tf1";

    public string EncodeAction(CallbackAction action)
    {
        if (action is null) throw new ArgumentNullException(nameof(action));

        return action switch
        {
            CallbackAction.Ui.NextPage =>
                $"{Prefix}|u:np",

            CallbackAction.Ui.PrevPage =>
                $"{Prefix}|u:pp",

            CallbackAction.Ui.ToggleIndex t =>
                $"{Prefix}|u:ti:{t.Index}",

            CallbackAction.Step.Back =>
                $"{Prefix}|s:b",

            CallbackAction.Step.Finish =>
                $"{Prefix}|s:f",

            CallbackAction.Step.GoTo g =>
                $"{Prefix}|s:g:{Escape(g.StepId)}",

            CallbackAction.Command.Execute c =>
                $"{Prefix}|c:e:{Escape(c.CommandKey)}",

            _ => throw new NotSupportedException($"Unsupported CallbackAction type: {action.GetType().FullName}")
        };
    }

    public bool TryDecodeAction(string data, out CallbackAction action)
    {
        action = null!;

        if (string.IsNullOrWhiteSpace(data))
            return false;

        // Быстрый reject по префиксу
        if (!data.StartsWith(Prefix, StringComparison.Ordinal))
            return false;

        // tf1|<kind>:<code>[:<arg>]
        // Пример: tf1|u:ti:12
        // Пример: tf1|s:g:step%3Aid
        // Пример: tf1|c:e:my%7Ccommand
        int pipe = data.IndexOf('|');
        if (pipe < 0 || pipe == data.Length - 1)
            return false;

        string payload = data[(pipe + 1)..];

        // kind:code[:arg]
        // kind — 1 символ (u/s/c)
        // code — короткий токен (np/pp/ti/b/f/g/e)
        // arg — optional
        var parts = payload.Split(':', 3, StringSplitOptions.None);
        if (parts.Length < 2)
            return false;

        string kind = parts[0];
        string code = parts[1];
        string arg = parts.Length == 3 ? parts[2] : "";

        try
        {
            switch (kind)
            {
                case "u":
                    switch (code)
                    {
                        case "np":
                            action = new CallbackAction.Ui.NextPage();
                            return true;

                        case "pp":
                            action = new CallbackAction.Ui.PrevPage();
                            return true;

                        case "ti":
                            if (!TryParseInt(arg, out int idx))
                                return false;
                            action = new CallbackAction.Ui.ToggleIndex(idx);
                            return true;

                        default:
                            return false;
                    }

                case "s":
                    switch (code)
                    {
                        case "b":
                            action = new CallbackAction.Step.Back();
                            return true;

                        case "f":
                            action = new CallbackAction.Step.Finish();
                            return true;

                        case "g":
                            if (arg.Length == 0)
                                return false;
                            action = new CallbackAction.Step.GoTo(Unescape(arg));
                            return true;

                        default:
                            return false;
                    }

                case "c":
                    switch (code)
                    {
                        case "e":
                            if (arg.Length == 0)
                                return false;
                            action = new CallbackAction.Command.Execute(Unescape(arg));
                            return true;

                        default:
                            return false;
                    }

                default:
                    return false;
            }
        }
        catch
        {
            // Любая неожиданная ошибка декода -> false (не наша кнопка / битая)
            action = null!;
            return false;
        }
    }

    private static bool TryParseInt(string s, out int value)
    {
        // не даём пробелам/плюсам/и т.п. "проходить"
        return int.TryParse(s, System.Globalization.NumberStyles.Integer,
            System.Globalization.CultureInfo.InvariantCulture, out value);
    }

    // Escape/Unescape: percent-encoding (коротко, безопасно, не тащит Uri)
    private static string Escape(string s)
    {
        if (s is null) throw new ArgumentNullException(nameof(s));

        // Оставляем ASCII alnum и несколько безопасных символов, остальное кодируем.
        var sb = new StringBuilder(s.Length);
        foreach (char ch in s)
        {
            if ((ch >= 'a' && ch <= 'z') ||
                (ch >= 'A' && ch <= 'Z') ||
                (ch >= '0' && ch <= '9') ||
                ch == '_' || ch == '-' || ch == '.')
            {
                sb.Append(ch);
            }
            else
            {
                sb.Append('%');
                sb.Append(((int)ch).ToString("X4")); // 4 hex (UTF-16 code unit)
            }
        }
        return sb.ToString();
    }

    private static string Unescape(string s)
    {
        if (s is null) throw new ArgumentNullException(nameof(s));

        var sb = new StringBuilder(s.Length);
        for (int i = 0; i < s.Length; i++)
        {
            char ch = s[i];
            if (ch == '%' && i + 4 < s.Length)
            {
                string hex = s.Substring(i + 1, 4);
                if (int.TryParse(hex, System.Globalization.NumberStyles.HexNumber,
                    System.Globalization.CultureInfo.InvariantCulture, out int code))
                {
                    sb.Append((char)code);
                    i += 4;
                    continue;
                }
            }
            sb.Append(ch);
        }
        return sb.ToString();
    }
}