using Spectre.Console;
namespace HabitTracker.Controllers;


internal abstract class BaseController
{
    protected void DisplayMessage(string message, string color = "yellow")
    {
        AnsiConsole.MarkupLine($"[{color}]{message}[/]");
    }
    protected void ConfirmAction(string actionDescription, Action onConfirm)
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"Are you sure you want to {actionDescription}?")
                .AddChoices("Yes", "No"));
        if (choice == "Yes")
        {
            onConfirm();
            DisplayMessage($"{actionDescription} completed successfully!", "green");
        }
        else
        {
            DisplayMessage($"{actionDescription} cancelled.", "red");
        }
    }
}
