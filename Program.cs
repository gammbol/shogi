using shogi.UI;

namespace shogi
{
    internal class Program
    {
        static async Task Main()
        {
            var menu = new ConsoleMenu();
            await menu.RunAsync();
        }
    }
}
