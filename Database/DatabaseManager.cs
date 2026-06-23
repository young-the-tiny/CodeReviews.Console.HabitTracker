using Microsoft.Data.Sqlite;
using HabitTracker.Models;

namespace HabitTracker.Database;


internal static class DatabaseManager
{
    const string ConnectionString = "Data Source=habit-Tracker.db";

    public static void Init() => Exec(
        """
        CREATE TABLE IF NOT EXISTS drinking_water (
            Id INTEGER PRIMARY KEY,
            Date TEXT NOT NULL,
            Quantity INTEGER NOT NULL);
        """);
    public static void Add(string date, int quantity) => Exec(
        "INSERT INTO drinking_water (Date, Quantity) VALUES (@date, @quantity);",
        ("@date", date), ("@quantity", quantity));

    public static void Update(int id, string date, int quantity) => Exec(
        "UPDATE drinking_water SET Date = @date, Quantity = @quantity WHERE Id = @id;",
        ("@id", id), ("@date", date), ("@quantity", quantity));

    public static void Delete(int id) => Exec(
        "DELETE FROM drinking_water WHERE Id = @id;",
        ("@id", id));

    public static bool Exists(int id)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT COUNT(1) FROM drinking_water WHERE Id = @id;";
        cmd.Parameters.AddWithValue("@id", id);
        return Convert.ToInt64(cmd.ExecuteScalar()) > 0;
    }

    public static List<DrinkingWater> All()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Id, Date, Quantity FROM drinking_water;";
        using var r = cmd.ExecuteReader();
        var rows = new List<DrinkingWater>();
        while (r.Read())
            rows.Add(new DrinkingWater(r.GetInt32(0), r.GetString(1), r.GetInt32(2)));
        return rows;
    }

    static void Exec(string sql, params (string name, object value)[] ps)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = sql;
        foreach (var (name, value) in ps)
            cmd.Parameters.AddWithValue(name, value);
        cmd.ExecuteNonQuery();
    }
}
