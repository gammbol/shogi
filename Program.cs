using shogi.UI;

namespace shogi
{
    internal class Program
    {
        static async Task Main()
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            var menu = new ConsoleMenu();
            await menu.RunAsync();
        }
    }
}
