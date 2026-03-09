using TeleFlow.Core.Commands.Stateful.Filters;
using TeleFlow.Fluent.Builders.Commands.Stateful.Filters;

namespace TeleFlow.Fluent.Extensions.Filters;

/// <summary>
/// Fluent helpers for attaching step-level filters.
/// </summary>
public static class StepFiltersToolkitExtensions
{
    public static TBuilder RequireSelfContact<TBuilder>(this TBuilder builder, string errorMessage = "Please share your own contact.")
        where TBuilder : StepFilterBuilder
    {
        builder.AddFilter(() => new RequireSelfContactFilter(errorMessage));
        return builder;
    }

    public static TBuilder RequireTextMatch<TBuilder>(this TBuilder builder, string pattern, string? errorMessage = null)
        where TBuilder : StepFilterBuilder
    {
        builder.AddFilter(() => new RequireTextMatchFilter(pattern, errorMessage ?? "Input does not match the required format"));
        return builder;
    }

    public static TBuilder RequireLettersOnly<TBuilder>(this TBuilder builder, string errorMessage = "Only letters are allowed")
        where TBuilder : StepFilterBuilder
    {
        builder.AddFilter(() => RequireTextMatchFilter.LettersOnly(errorMessage));
        return builder;
    }

    public static TBuilder RequireLettersAndHyphen<TBuilder>(this TBuilder builder, string errorMessage = "Only letters and '-' are allowed")
        where TBuilder : StepFilterBuilder
    {
        builder.AddFilter(() => RequireTextMatchFilter.LettersAndHyphen(errorMessage));
        return builder;
    }

    public static TBuilder RequireNumbersOnly<TBuilder>(this TBuilder builder, string errorMessage = "Only numbers are allowed")
        where TBuilder : StepFilterBuilder
    {
        builder.AddFilter(() => RequireTextMatchFilter.NumbersOnly(errorMessage));
        return builder;
    }

    public static TBuilder RequireNoWhitespace<TBuilder>(this TBuilder builder, string errorMessage = "Spaces are not allowed")
        where TBuilder : StepFilterBuilder
    {
        builder.AddFilter(() => RequireTextMatchFilter.NoWhitespace(errorMessage));
        return builder;
    }
}
