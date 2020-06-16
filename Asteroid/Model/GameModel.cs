using Asteroid.Persistence;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Asteroid.Model
{
    public class GameModel
    {
        private AsteroidTable _table;
        private IAsteroidDataAccess _dataAccess;
        private bool _bGameStopped;
        private bool _bIsGameOver;
        private int[] _spawnArray = new int[11] { 0,1,2,3,4,5,6,7,8,9,10 };

        public GameModel(IAsteroidDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _bGameStopped = true;
            _bIsGameOver = false;
            _table = new AsteroidTable();
        }
        public AsteroidTable Table { get { return _table; } }
        public void NewGame()
        {
            _table = new AsteroidTable();
            _bGameStopped = true;
            _bIsGameOver = false;
        }
        private void ShuffleArray()
        {
            Random rng = new Random();

            _spawnArray = _spawnArray.OrderBy(x => rng.Next()).ToArray();
        }
        public void MoveLeft() 
        {
            if (_table.PlayerPosition != 0 && _bGameStopped == false) //
            {
                if(_table[9,_table.PlayerPosition - 1] != 2) //
                {
                    _table[9, _table.PlayerPosition] = 0; //
                    _table.PlayerPosition -= 1; //
                    _table[9, _table.PlayerPosition] = 1; //
                }
                else 
                {
                    _bIsGameOver = true;
                }
            }
        }
        public void MoveRight()
        {
            if (_table.PlayerPosition != 10 && _bGameStopped == false)
            {
                if (_table[9, _table.PlayerPosition + 1] != 2)
                {
                    _table[9, _table.PlayerPosition] = 0;
                    _table.PlayerPosition += 1;
                    _table[9, _table.PlayerPosition] = 1;
                }
                else
                {
                    _bIsGameOver = true;
                }
            }
        }
        public void ModelTick()
        {
            _table.SurvivedTime += 1000;
            for (int i = 9; i>=0;i--)
            {
                for (int j=10;j>=0;j--)
                {
                    if (_table[i, j] == 2 && i == 9)
                    {
                        _table[i, j] = 0;
                    }
                    else if(_table[i,j] == 2 && i != 9)
                    {
                        if(_table[i+1,j]==1)
                        {
                            _bIsGameOver = true;
                        }
                        else
                        {
                            _table[i + 1, j] = 2;
                            _table[i, j] = 0;
                        }
                    }
                }
            }

            if(_table.bCanSpawn && _table.AsteroidNumber <12)
            {

                ShuffleArray();
                for (int i = 0; i< _table.AsteroidNumber; i++)
                {
                    _table[0, _spawnArray[i]] = 2;
                }
                _table.AsteroidNumber++;
            }
            _table.bCanSpawn = !_table.bCanSpawn;
        }
        public Int32 GameTime { get { return _table.SurvivedTime; } }
        public int PlayerPosition { get { return _table.PlayerPosition; } }
        public bool bGameStopped { get { return _bGameStopped; } set { _bGameStopped = value; } }
        public int RowCount { get { return 10; } }
        public int ColumnCount { get { return 11; } }

        public bool bIsGameOver { get { return _bIsGameOver; } }

        public async Task LoadGameAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            _table = await _dataAccess.LoadAsync(path);
            _bGameStopped = true;
            _bIsGameOver = false;
        }
        public async Task SaveGameAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            await _dataAccess.SaveAsync(path, _table);
        }
    }
}
