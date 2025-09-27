using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LisBot.Common.Telegram.Exceptions;
using LisBot.Common.Telegram.ViewModels;
using LisBot.Common.Telegram.ViewModels.CallbackQuery;
using Newtonsoft.Json;
using Telegram.Bot.Extensions.Handlers.Services.Markup;
using Telegram.Bot.Extensions.Handlers.Services.Messaging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LisBot.Common.Telegram.Services;

public class DateSelectionMessageFormatter
{
    private readonly Dictionary<int, Func<Task>> _buttonActions;

    private readonly CallbackButtonGenerator _buttonGenerator;


    public event Func<DateOnly, Task<bool>>? DateSelected;

    public event Func<DateSelectionView, Task>? ViewChanged;


    public event Func<Task>? DateConfirmed;


    public Message GenerateMessage(DateSelectionView view, string messageText, DateOnly currentDate)
    {
        MessageBuilder builder = new();
        builder.WithText(messageText);

        switch (view)
        {
            case DateSelectionView.YearSelection:
                GenerateMessageForYearSelection(builder, currentDate);
                break;
            case DateSelectionView.MonthSelection:
                GenerateMessageForMonthSelection(builder, currentDate);
                break;
            case DateSelectionView.DaySelection:
                GenerateMessageForDaySelection(builder, currentDate);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(view), view, null);
        }

        return builder.Build();
    }

    private void GenerateMessageForYearSelection(MessageBuilder builder, DateOnly currentDate)
    {
        int columns = 3;
        int rows = 3;

        int buttonCount = columns * rows;

        int centerYear = currentDate.Year;

        int currentYear = centerYear - (buttonCount - 1) / 2;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (row == 0 && col == 0)
                {
                    builder.WithInlineButton(GenerateActionButton(async () =>
                    {
                        if (DateSelected is not null)
                            await DateSelected.Invoke(currentDate.AddYears(-9));
                    }, "<<"));
                }
                else if (row == rows - 1 && col == columns - 1)
                {
                    builder.WithInlineButton(GenerateActionButton(async () => {
                        if(DateSelected is not null)
                            await DateSelected.Invoke(currentDate.AddYears(9));
                    }, ">>"));
                }
                else
                {
                    string markedAsCurrent = currentYear == currentDate.Year ? "*" : "";

                    var dateInButton = GenerateDaySafeDateOnly(currentYear, currentDate.Month, currentDate.Day);

                    var button = GenerateActionButton(async () =>
                    {
                        await SelectDateAndNavigateToView(dateInButton, DateSelectionView.MonthSelection);
                    }, markedAsCurrent + currentYear.ToString() + markedAsCurrent);

                    builder.WithInlineButton(button);

                    currentYear += 1;   
                }
            }
            builder.WithNewButtonLine();
        }
    }

    private void GenerateMessageForMonthSelection(MessageBuilder builder, DateOnly currentDate)
    {
        builder
        .WithInlineButton(GenerateActionButton(async () =>
        {
            if (DateSelected is not null)
                await DateSelected.Invoke(currentDate.AddYears(-1));
        }, "<<"))
        .WithInlineButton(GenerateActionButton(async () =>
        {
            if (ViewChanged is not null)
                await ViewChanged.Invoke(DateSelectionView.YearSelection);
        }, currentDate.ToString("yyyy", new CultureInfo("ru-RU"))))
        .WithInlineButton(GenerateActionButton(async () =>
        {
            if (DateSelected is not null)
                await DateSelected.Invoke(currentDate.AddYears(1));
        }, ">>"))
        .WithNewButtonLine();

        int columns = 4;
        int rows = 3;

        int currentMonth = 1;

        var culture = new CultureInfo("ru-RU");

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                string markedAsCurrent = currentMonth == currentDate.Month ? "*" : "";
                var dateInButton = GenerateDaySafeDateOnly(currentDate.Year, currentMonth, currentDate.Day);

                var button = GenerateActionButton(async () =>
                {
                    await SelectDateAndNavigateToView(dateInButton, DateSelectionView.DaySelection);
                }, markedAsCurrent + culture.DateTimeFormat.MonthNames[currentMonth - 1] + markedAsCurrent);
                builder.WithInlineButton(button);

                currentMonth += 1;
            }
            builder.WithNewButtonLine();
        }
    }

    private void GenerateMessageForDaySelection(MessageBuilder builder, DateOnly currentDate)
    {
        builder
        .WithInlineButton(GenerateActionButton(async () => {
            if(DateSelected is not null)
                await DateSelected.Invoke(currentDate.AddYears(-1));
        }, "<<"))
        .WithInlineButton(GenerateActionButton(async () => {
            if(ViewChanged is not null)
                await ViewChanged.Invoke(DateSelectionView.MonthSelection);
        }, currentDate.ToString("MMM yyyy", new CultureInfo("ru-RU"))))
        .WithInlineButton(GenerateActionButton(async () => {
            if(DateSelected is not null)
                await DateSelected.Invoke(currentDate.AddYears(1));
        }, ">>"))
        .WithNewButtonLine();

        for (int i = 0; i < 7; i++)
        {
            builder.WithInlineButton(GenerateActionButton(async () => { }, new CultureInfo("ru-RU").DateTimeFormat.AbbreviatedDayNames[(i + 1) % 7]));
        }
        builder.WithNewButtonLine();

        
        int columns = 7;
        int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
        int weekDay = GetWeekDayMondayFirst(new(currentDate.Year, currentDate.Month, 1)); // 0 = Monday
        int totalCells = daysInMonth + weekDay;
        int rows = (int)Math.Ceiling(totalCells / (double)columns);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                int cellIndex = row * columns + col;
                int day = cellIndex - weekDay + 1;

                if (day < 1 || day > daysInMonth)
                {
                    // Empty cell
                    builder.WithInlineButton(GenerateActionButton(async () => { }, " "));
                }
                else
                {
                    string markedAsCurrent = day == currentDate.Day ? "*" : "";
                    // Valid day button
                    var dateInButton = GenerateDaySafeDateOnly(currentDate.Year, currentDate.Month, day);

                    var button = GenerateActionButton(
                        async () => {
                            if (DateSelected is not null)
                            {
                                bool ok = await DateSelected.Invoke(dateInButton);

                                if (ok)
                                {
                                    if(DateConfirmed is not null)
                                        await DateConfirmed.Invoke();
                                }
                            }
                        },
                        markedAsCurrent + day.ToString() + markedAsCurrent
                    );

                    builder.WithInlineButton(button);
                }
            }
            builder.WithNewButtonLine();
        }


        builder.WithInlineButton(GenerateActionButton(async () => {
            if (DateSelected is not null)
                await DateSelected.Invoke(currentDate.AddMonths(-1));
        }, "<"));
        builder.WithInlineButton(GenerateActionButton(async () => {
            if(DateSelected is not null)
                await DateSelected.Invoke(currentDate.AddMonths(1));
        }, ">"));
    }


    private async Task SelectDateAndNavigateToView(DateOnly dateToSelect, DateSelectionView viewToNavigate)
    {
        if (DateSelected is not null)
        {
            bool ok = await DateSelected.Invoke(dateToSelect);

            if (ok && ViewChanged is not null)
                await ViewChanged.Invoke(viewToNavigate);
        }
    }


    private ReplyButtonModel<CallbackQueryViewModel> GenerateActionButton(Func<Task> action, string buttonText)
    {
        var vm = _buttonGenerator.GenerateVM();

        _buttonActions.Add(vm.BID, action);

        return new(vm, buttonText);
    }


    public async Task HandleUpdate(Update update)
    {
        if (update.CallbackQuery is null || update.Type != UpdateType.CallbackQuery)
            throw new InvalidUserInput("This command accepts only the button input");

        await ParseCallbackQuery(update.CallbackQuery);
    }

    private async Task ParseCallbackQuery(CallbackQuery query)
    {
        if(string.IsNullOrEmpty(query.Data))
            throw new ArgumentException($"No data in {nameof(query)}");

        var btnViewModel = JsonConvert.DeserializeObject<CallbackQueryViewModel>(query.Data) ?? throw new ArgumentException($"The data in {nameof(query)} was in incorrect format.");

        if (!_buttonGenerator.IsFromCurrentSession(btnViewModel))
            throw new InvalidUserInput("The button is not from current session");

        if(!_buttonActions.ContainsKey(btnViewModel.BID))
            throw new ArgumentException("The Button UID was not found");

        await _buttonActions[btnViewModel.BID].Invoke();
    }


    private static DateOnly GenerateDaySafeDateOnly(int year, int month, int day)
    {
        int maxDay = DateTime.DaysInMonth(year, month);
        int safeDay = Math.Min(day, maxDay);

        return new DateOnly(year, month, safeDay);
    }

    private static int GetWeekDayMondayFirst(DateOnly currentDate)
    {
        int weekDay = (int)new DateOnly(currentDate.Year, currentDate.Month, 1).DayOfWeek;

        weekDay = weekDay == 0 ? 7 : weekDay;
        weekDay -= 1;

        return weekDay;
    }

    public DateSelectionMessageFormatter()
    {
        _buttonActions = [];
        _buttonGenerator = new();
        _buttonGenerator.StartNewSession();
    }
}

public enum DateSelectionView
{
    YearSelection,
    MonthSelection,
    DaySelection
}