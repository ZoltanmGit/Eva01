using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Persistence
{
    public class AsteroidTable
    {
        private Int32[,] _tableValues;
        private Int32 _asteroidNumbers;
        private Int32 _survivedTime;
        private Int32 _positionOfPlayer;
        private bool _bCanSpawn;

        public AsteroidTable()
        {
            _tableValues = new int[10, 11];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    _tableValues[i, j] = 0;
                }
                
            }
            _positionOfPlayer = 5;
            _tableValues[9, _positionOfPlayer] = 1;
            _bCanSpawn = true;
            _asteroidNumbers = 1;
        }
        public Int32 SurvivedTime { get { return _survivedTime; } set { _survivedTime = value; } }
        public Int32 AsteroidNumber { get { return _asteroidNumbers; } set { _asteroidNumbers = value; } }
        public Int32 PlayerPosition { get { return _positionOfPlayer; } set { _positionOfPlayer = value; } }
        public bool bCanSpawn { get { return _bCanSpawn; } set { _bCanSpawn = value; } }
        public Int32 this[Int32 x, Int32 y] { get { return _tableValues[x, y]; } set { _tableValues[x, y] = value; } }
    }
}
