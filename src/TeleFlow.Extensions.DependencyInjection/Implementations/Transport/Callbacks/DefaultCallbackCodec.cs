using System;
using System.Text;
using TeleFlow.Abstractions.Transport.Callbacks;

namespace TeleFlow.Extensions.DependencyInjection.Implementations.Transport.Callbacks;

public sealed class DefaultCallbackCodec : ICallbackCodec
{
    private const string Prefix = "tf1";
    private const char Sep = '\u001F'; // Unit Separator (редко встречается в данных)

    public string EncodeAction(CallbackToken token)
    {
        if (token is null) throw new ArgumentNullException(nameof(token));

        var kind = token.Kind ?? string.Empty;
        var data = token.Data ?? string.Empty;

        // kind пустым быть может (для чужих расширений), но обычно это ошибка.
        // Тут не валидирую жестко — кодек это wire-format, а валидация смысла выше уровнем.
        var sb = new StringBuilder(Prefix.Length + 1 + kind.Length + 1 + data.Length + 8);
        sb.Append(Prefix);
        sb.Append(Sep);
        sb.Append(Escape(kind));
        sb.Append(Sep);
        sb.Append(Escape(data));
        return sb.ToString();
    }

    public bool TryDecodeAction(string data, out CallbackToken token)
    {
        token = default!;

        if (string.IsNullOrEmpty(data))
            return false;

        // Быстрая проверка префикса: "tf1" + Sep + ...
        if (data.Length < Prefix.Length + 2) // минимум: "tf1" + Sep + "a" + Sep
            return false;

        if (!data.StartsWith(Prefix, StringComparison.Ordinal))
            return false;

        if (data[Prefix.Length] != Sep)
            return false;

        // Разделим на 3 части: prefix, kind, data
        // data может содержать Sep в escaped-виде (\s), поэтому split безопасен.
        // Реальные Sep в kind/data мы экранируем, значит raw Sep там быть не должно.
        var firstSep = Prefix.Length; // позиция Sep после Prefix
        var secondSep = data.IndexOf(Sep, firstSep + 1);
        if (secondSep < 0)
            return false;

        var rawKind = data.Substring(firstSep + 1, secondSep - (firstSep + 1));
        var rawPayload = data.Substring(secondSep + 1);

        string kind;
        string payload;

        try
        {
            kind = Unescape(rawKind);
            payload = Unescape(rawPayload);
        }
        catch
        {
            return false;
        }

        token = new CallbackToken(kind, payload);
        return true;
    }

    private static string Escape(string input)
    {
        if (input.Length == 0) return string.Empty;

        // Если нет символов для экранирования — вернем как есть
        var needs = false;
        foreach (var ch in input)
        {
            if (ch == '\\' || ch == Sep || ch == '\n' || ch == '\r')
            {
                needs = true;
                break;
            }
        }

        if (!needs) return input;

        var sb = new StringBuilder(input.Length + 8);
        foreach (var ch in input)
        {
            switch (ch)
            {
                case '\\':
                    sb.Append("\\\\");
                    break;
                case '\n':
                    sb.Append("\\n");
                    break;
                case '\r':
                    sb.Append("\\r");
                    break;
                case Sep:
                    sb.Append("\\s"); // separator escaped
                    break;
                default:
                    sb.Append(ch);
                    break;
            }
        }
        return sb.ToString();
    }

    private static string Unescape(string input)
    {
        if (input.Length == 0) return string.Empty;

        var sb = new StringBuilder(input.Length);
        for (int i = 0; i < input.Length; i++)
        {
            var ch = input[i];
            if (ch != '\\')
            {
                sb.Append(ch);
                continue;
            }

            // trailing backslash -> invalid
            if (i == input.Length - 1)
                throw new FormatException("Invalid escape sequence.");

            var next = input[++i];
            switch (next)
            {
                case '\\':
                    sb.Append('\\');
                    break;
                case 'n':
                    sb.Append('\n');
                    break;
                case 'r':
                    sb.Append('\r');
                    break;
                case 's':
                    sb.Append(Sep);
                    break;
                default:
                    throw new FormatException($"Unknown escape sequence: \\{next}");
            }
        }

        return sb.ToString();
    }
}
