namespace shogi
{
    internal class Program
    {
        static void Main()
        {
            const int size = 9;
            string[,] board = new string[size, size];

            // Заполняем поле пустыми клетками
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    board[row, col] = ".";
                }
            }

            // Отрисовываем поле в консоли
            Console.WriteLine("   a b c d e f g h i");
            for (int row = 0; row < size; row++)
            {
                Console.Write($"{row + 1} ");
                for (int col = 0; col < size; col++)
                {
                    Console.Write($" {board[row, col]}");
                }
                Console.WriteLine();
            }
        }
    }
}
