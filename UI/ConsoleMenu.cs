using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using shogi.Data;

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
                Console.WriteLine("1.Новая игра");
                Console.WriteLine("2.Загрузить игру");
                Console.WriteLine("3.Рекорды");
                Console.WriteLine("4.Выход");
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
                        break;
                }
            }
        }

        private async Task StartNewGameAsync()
        {
            var engine = new GameEngine();
            engine.Init();

            var renderer = new ConsoleRenderer(engine.Board);

            while (!engine.IsFinished)
            {
                Console.Clear();
                renderer.Render();
                Console.WriteLine("Введите ход (пример: 2 3 2 4):");

                var input = Console.ReadLine();
                if (!engine.TryMakeMove(input))
                {
                    Console.WriteLine("Неверный ход");
                    Thread.Sleep(800);
                }
            }

            Console.Clear();
            renderer.Render();
            Console.WriteLine("Игра завершена!");

            Console.Write("Введите имя игрока: ");
            var name = Console.ReadLine();

            var record = new RecordEntry(name ?? "Player", engine.Score);
            await _recordStorage.AddRecordAsync(record);
        }

        private async Task LoadGameAsync()
        {
            var loaded = await _gameStorage.LoadAsync();
            if (loaded == null)
            {
                Console.WriteLine("Сохранение не найдено");
                Console.ReadKey();
                return;
            }

            var engine = loaded;
            var renderer = new ConsoleRenderer(engine.Board);

            while (!engine.IsFinished)
            {
                Console.Clear();
                renderer.Render();
                Console.WriteLine("Введите ход:");

                var input = Console.ReadLine();

                if (!engine.TryMakeMove(input))
                    Console.WriteLine("Неверный ход");
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
