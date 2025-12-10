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
            
            string SaveString = await File.ReadAllTextAsync(_path);

            var info = SaveString.Split('\n');

            var gameInfo = info[0].Split(';');
            Player CurrentPlayer = (Player)Enum.Parse(typeof(Player), gameInfo[0]);
            int Score = int.Parse(gameInfo[1]);
            
            var boardInfo = info[1].Split('\n');

            Piece[,] Board = new Piece[9, 9];
            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    PieceType Type = (PieceType)Enum.Parse(typeof(PieceType), boardInfo[(x+1)*y]);
                    Player Owner = (Player)Enum.Parse(typeof(Player), boardInfo[(x+1)*y]);
                    bool Promoted =  (bool)Enum.Parse(typeof(bool), boardInfo[(x+1)*y]);
                    
                    Board[x, y] = new Piece(Type, Owner, Promoted);
                }
            }

            GameEngine game = new GameEngine();
            game.Init(Board, CurrentPlayer, Score);
            
            return game;

        }

        public async void SaveAsync(GameEngine game)
        {
            string SaveString = "";
            
            SaveString += $"{game.CurrentPlayer};";
            SaveString += $"{game.Score}\n";

            for (int x = 0; x < 9; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    SaveString += $"{game.Board[x, y].Owner};{game.Board[x, y].Type};{game.Board[x, y].Promoted}\n";
                }
            }
            
            File.WriteAllText(_path, SaveString);
        }
    }
}