using System;
using System.IO;
using System.Threading.Tasks;
using shogi.Logic;

namespace shogi.Data
{
    public class GameStorage
    {
        private readonly string _path;

        public GameStorage(string path)
        {
            _path = path;
        }

        public async Task<GameEngine?> LoadAsync()
        {
            if (!File.Exists(_path))
                return null;

            try
            {
                string saveString = await File.ReadAllTextAsync(_path);
                var lines = saveString.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length < 1)
                    return null;

                var gameInfo = lines[0].Split(';');
                if (gameInfo.Length < 2)
                    return null;

                Player currentPlayer = Enum.Parse<Player>(gameInfo[0]);
                int score = int.Parse(gameInfo[1]);

                Piece[,] board = new Piece[9, 9];
                int lineIndex = 1;

                for (int y = 0; y < 9; y++)
                {
                    for (int x = 0; x < 9; x++)
                    {
                        if (lineIndex >= lines.Length)
                            return null;

                        var cellInfo = lines[lineIndex].Split(';');
                        if (cellInfo.Length < 3)
                            return null;

                        PieceType type = Enum.Parse<PieceType>(cellInfo[1]);
                        Player owner = Enum.Parse<Player>(cellInfo[0]);
                        bool promoted = bool.Parse(cellInfo[2]);

                        board[x, y] = new Piece(type, owner, promoted);
                        lineIndex++;
                    }
                }

                List<String> steps = new List<string>();
                for (int i = lineIndex; i < lines.Length; i++)
                {
                    steps.Add(lines[i]);
                }

                var game = new GameEngine();
                game.Init(board, currentPlayer, score, steps);
                return game;
            }
            catch
            {
                return null;
            }
        }

        public async Task SaveAsync(GameEngine game)
        {
            string saveString = "";

            saveString += $"{game.CurrentPlayer};{game.Score}\n";

            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    var piece = game.Board[x, y];
                    saveString += $"{piece.Owner};{piece.Type};{piece.Promoted}\n";
                }
            }
            
            foreach (var line in game.Steps)
            {
                saveString += $"{line}\n";
            }

            await File.WriteAllTextAsync(_path, saveString);
        }
    }
}