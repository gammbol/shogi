using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using shogi;
using shogi.Data;
using shogi.Logic;
using shogi.UI;

namespace shogi.UI
{
    public class ConsoleMenu
    {
        private readonly RecordStorage _recordStorage = new("records.txt");
        private readonly GameStorage _gameStorage = new("savegame.txt");

        public async Task RunAsync()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Новая игра");
                Console.WriteLine("2. Загрузить игру");
                Console.WriteLine("3. Рекорды");
                Console.WriteLine("4. Выход");
                Console.Write("Выберите пункт: ");

                var key = Console.ReadLine();

                switch (key)
                {
                    case "1":
                        await StartNewGameAsync();
                        break;

                    case "2":
                        await LoadGameAsync();
                        break;

                    case "3":
                        ShowRecords();
                        break;

                    case "4":
                        return;

                    default:
                        Console.WriteLine("Неверный ввод");
                        Thread.Sleep(800);
                        break;
                }
            }
        }

        private async Task StartNewGameAsync()
        {
            var engine = new GameEngine();
            engine.Init();

            var boardUI = new shogi.UI.Board();
            boardUI.UpdateFromGameEngine(engine.Board);
            var renderer = new ConsoleRenderer(boardUI);

            while (!engine.IsFinished)
            {
                Console.Clear();
                renderer.Render();

                Console.WriteLine("Введите ход (пример: 5 3 5 4):");
                var input = Console.ReadLine();

                if (!await engine.TryMakeMove(input))
                {
                    Console.WriteLine("Неверный ход");
                    Thread.Sleep(800);
                }

                renderer.UpdateBoard(engine.Board);
            }

            Console.Clear();
            renderer.Render();
            Console.WriteLine("Игра завершена!");

            Console.Write("Введите имя игрока: ");
            var name = Console.ReadLine() ?? "Player";

            _recordStorage.AddRecord(new RecordEntry(name, engine.Score));
        }

        private async Task LoadGameAsync()
        {
            var engine = await _gameStorage.LoadAsync();
            if (engine == null)
            {
                Console.WriteLine("Сохранение не найдено");
                Console.ReadKey();
                return;
            }

            var board = new Board();
            board.UpdateFromGameEngine(engine.Board);
            var renderer = new ConsoleRenderer(board);

            while (!engine.IsFinished)
            {
                Console.Clear();
                renderer.Render();

                Console.WriteLine("Введите ход:");
                var input = Console.ReadLine();

                if (!await engine.TryMakeMove(input))
                    Console.WriteLine("Неверный ход");

                renderer.UpdateBoard(engine.Board);
            }

            Console.WriteLine("Игра завершена");
            Console.ReadKey();
        }

        private void ShowRecords()
        {
            var list = _recordStorage.LoadRecords();

            Console.Clear();
            Console.WriteLine("Лучшие партии:");

            foreach (var r in list.Take(10))
                Console.WriteLine($"{r.Name} — {r.Score}");

            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}