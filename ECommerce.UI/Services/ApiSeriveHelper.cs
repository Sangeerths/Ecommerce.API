using Spectre.Console;
namespace ECommerce.UI.Services;

public class ApiSeriveHelper
{
    public static int AskPositiveInt(string prompt, int? defaultValue = null)
    {
        var textPrompt = new TextPrompt<int>(prompt)
            .Validate(value => value > 0
                ? ValidationResult.Success()
                : ValidationResult.Error("[red]Value must be greater than 0.[/]"));

        if (defaultValue.HasValue)
        {
            textPrompt.DefaultValue(defaultValue.Value);
        }

        return AnsiConsole.Prompt(textPrompt);
    }

    public static int AskNonNegativeInt(string prompt, int? defaultValue = null)
    {
        var textPrompt = new TextPrompt<int>(prompt)
            .Validate(value => value >= 0
                ? ValidationResult.Success()
                : ValidationResult.Error("[red]Value cannot be negative.[/]"));

        if (defaultValue.HasValue)
        {
            textPrompt.DefaultValue(defaultValue.Value);
        }

        return AnsiConsole.Prompt(textPrompt);
    }

    public static decimal AskPositiveDecimal(string prompt, decimal? defaultValue = null)
    {
        var textPrompt = new TextPrompt<decimal>(prompt)
            .Validate(value => value > 0
                ? ValidationResult.Success()
                : ValidationResult.Error("[red]Value must be greater than 0.[/]"));

        if (defaultValue.HasValue)
        {
            textPrompt.DefaultValue(defaultValue.Value);
        }

        return AnsiConsole.Prompt(textPrompt);
    }

    public static decimal AskNonNegativeDecimal(string prompt, decimal? defaultValue = null)
    {
        var textPrompt = new TextPrompt<decimal>(prompt)
            .Validate(value => value >= 0
                ? ValidationResult.Success()
                : ValidationResult.Error("[red]Value cannot be negative.[/]"));

        if (defaultValue.HasValue)
        {
            textPrompt.DefaultValue(defaultValue.Value);
        }

        return AnsiConsole.Prompt(textPrompt);
    }

    public static string AskNonEmptyString(string prompt, string? defaultValue = null, int? maxLength = null)
    {
        var textPrompt = new TextPrompt<string>(prompt)
            .Validate(value =>
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return ValidationResult.Error("[red]This field cannot be empty.[/]");
                }

                if (maxLength.HasValue && value.Length > maxLength.Value)
                {
                    return ValidationResult.Error($"[red]Must be {maxLength.Value} characters or fewer.[/]");
                }

                return ValidationResult.Success();
            });

        if (!string.IsNullOrWhiteSpace(defaultValue))
        {
            textPrompt.DefaultValue(defaultValue);
        }

        return AnsiConsole.Prompt(textPrompt).Trim();
    }

   
    public static bool Confirm(string prompt, bool defaultValue = false)
        => AnsiConsole.Confirm(prompt, defaultValue);

    private const string BackLabel = "[grey]<< Back[/]";

   
    public static T? PickOrNone<T>(
        string title,
        List<T> items,
        Func<T, string> displayConverter,
        string emptyMessage = "No items found.",
        int pageSize = 10) where T : class
    {
        if (items.Count == 0)
        {
            AnsiConsole.MarkupLine($"[yellow]{emptyMessage}[/]");
            return null;
        }

        return Pick(title, items, displayConverter, pageSize);
    }

    public static T? Pick<T>(
        string title,
        List<T> items,
        Func<T, string> displayConverter,
        int pageSize = 10) where T : class
    {
        var displayChoices = new List<string> { BackLabel };
        var lookup = new Dictionary<string, T>();

        foreach (var item in items)
        {
            var display = displayConverter(item);

            // Guard against duplicate display strings colliding in the lookup.
            var uniqueDisplay = display;
            int suffix = 1;
            while (lookup.ContainsKey(uniqueDisplay))
            {
                uniqueDisplay = $"{display} ({++suffix})";
            }

            lookup[uniqueDisplay] = item;
            displayChoices.Add(uniqueDisplay);
        }

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .PageSize(pageSize)
                .MoreChoicesText("[grey](Move up and down to reveal more)[/]")
                .AddChoices(displayChoices));

        return selected == BackLabel ? null : lookup[selected];
    }
}
