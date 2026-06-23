using HabitTracker.Database;
using HabitTracker.UI;

namespace HabitTracker;

class Program
{
    static void Main(string[] args)
    {
        DatabaseManager.Init();
        new UserInterface().Run();
    }
}