using System;
using System.Collections.Generic;

namespace TeleFlow.Core.Commands.Stateful.Steps.DateInput;

public sealed class DateInputCommandStepViewModel
{
    private readonly DateTime _vmInitDateTime = DateTime.UtcNow;


    public int YearSelected { get; set; }

    public int MonthSelected { get; set; }

    public int DaySelected { get; set; }


    public DateInputCommandStepPage Page { get; set; }

    public int YearPageIndex { get; set; } = 0;

    public int InitYearDate { get; init; }

    public DateInputCommandStepViewModel()
    {
        _vmInitDateTime = DateTime.UtcNow;

        YearSelected = _vmInitDateTime.Year;
        MonthSelected = _vmInitDateTime.Month;
        DaySelected = _vmInitDateTime.Day;

        Page = DateInputCommandStepPage.YearSelection;

        YearPageIndex = 0;
        InitYearDate = _vmInitDateTime.Year;
    }
}

public enum DateInputCommandStepPage
{
    YearSelection,
    MonthSelection,
    DateSelection
}