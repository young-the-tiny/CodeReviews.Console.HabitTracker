using HabitTracker.Controllers;
using Spectre.Console;
using HabitTracker.Database;

namespace HabitTracker.UI;

internal class UserInterface : BaseController, IBaseController
{
    public void Run()
    {
        while (true)
        {
            AnsiConsole.Clear();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[green]Habit Tracker[/]")
                    .AddChoices("View Records", "Add Record", "Update Record", "Delete Record", "Exit"));

            switch (choice)
            {
                case "View Records": ReadItem(); break;
                case "Add Record": CreateItem(); break;
                case "Update Record": UpdateItem(); break;
                case "Delete Record": DeleteItem(); break;
                case "Exit": return;
            }

            AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
            AnsiConsole.Console.Input.ReadKey(true);
        }
    }

    public void ReadItem()
    {
        var records = DatabaseManager.All();

        if (records.Count == 0)
        {
            DisplayMessage("No records yet.");
            return;
        }

        var table = new Table();
        table.AddColumns("Id", "Date", "Quantity");
        foreach (var r in records)
            table.AddRow(r.Id.ToString(), r.Date, r.Quantity.ToString());
        AnsiConsole.Write(table);
    }

    public void CreateItem()
    {
        var date = PromptDate();
        var quantity = PromptQuantity();

        ConfirmAction("add record", () =>
        {
            DatabaseManager.Add(date, quantity);
        });
    }

    public void UpdateItem()
    {
        var id = PromptId();
        if (!DatabaseManager.Exists(id))
        {
            DisplayMessage($"No record with Id {id}.", "red");
            return;
        }

        var date = PromptDate();
        var quantity = PromptQuantity();

        ConfirmAction("update record", () =>
        {
            DatabaseManager.Update(id, date, quantity);
        });
    }

    public void DeleteItem()
    {
        var id = PromptId();
        if (!DatabaseManager.Exists(id))
        {
            DisplayMessage($"No record with Id {id}.", "red");
            return;
        }

        ConfirmAction("delete record", () =>
        {
            DatabaseManager.Delete(id);
        });
    }

    private static int PromptId() =>
        AnsiConsole.Prompt(new TextPrompt<int>("Record [green]Id[/]:"));

    private static string PromptDate() =>
        AnsiConsole.Prompt(
            new TextPrompt<string>("[green]Date[/] (yyyy-MM-dd):")
                .DefaultValue(DateTime.Today.ToString("yyyy-MM-dd"))
                .Validate(s => DateTime.TryParse(s, out _)
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Invalid date[/]")));

    private static int PromptQuantity() =>
        AnsiConsole.Prompt(
            new TextPrompt<int>("[green]Quantity[/]:")
                .Validate(q => q > 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Quantity must be greater than 0[/]")));
}
