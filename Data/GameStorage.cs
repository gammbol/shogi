using System;
using System.IO;
using System.Threading.Tasks;

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

            // Заглушка 
            return null;
        }
    }
}