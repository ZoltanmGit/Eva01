using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asteroid.Model;

namespace AsteroidTest
{
    [TestClass]
    public class AsteroidaTest
    {
        private GameModel _model;
        
        [TestInitialize]
        public void Initialize()
        {
            _model = new GameModel(null);
        }
        [TestMethod]
        public void TestNewGame()
        {
            _model.NewGame();
            Assert.AreEqual(1, _model.Table[9, 5]); //Játékos kezdőhelye középen van, e
            Assert.AreEqual(5, _model.Table.PlayerPosition); // 
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (i != 9 && j != 5)
                    {
                        Assert.AreEqual(0, _model.Table[i, j]); //Mező kezdőértékei 0-k
                    }
                }
            }
            Assert.AreEqual(true, _model.bGameStopped); //Áll e a játék új indításakor
            Assert.AreEqual(false, _model.bIsGameOver); //Nincs-e vége a játéknak
            Assert.AreEqual(1,_model.Table.AsteroidNumber); // Kezdő aszteroida érték 1-e
            Assert.AreEqual(0,_model.Table.SurvivedTime); //Még nincs túlélt idő
            Assert.AreEqual(true, _model.Table.bCanSpawn); //Lehet-e új aszteroidát spawnolni
        }
        [TestMethod]
        public void TestMovement()
        {
            _model.MoveLeft();
            Assert.AreEqual(5, _model.Table.PlayerPosition); //Szünet közben nem változik
            _model.bGameStopped = false; //Megengedjük a mozgást
            _model.MoveLeft();
            Assert.AreEqual(4, _model.Table.PlayerPosition);
            _model.MoveLeft(); //3
            _model.MoveLeft(); //2
            _model.MoveLeft(); //1
            _model.MoveLeft(); //0
            _model.MoveLeft(); // Nem lépünk ki a mezőből
            Assert.AreEqual(0, _model.Table.PlayerPosition);
            _model.MoveRight(); //1
            _model.MoveRight();
            Assert.AreEqual(2, _model.Table.PlayerPosition);
            for (int i = 0; i < 20; i++)
            {
                _model.MoveRight();
            }
            Assert.AreEqual(10, _model.Table.PlayerPosition); //Jobbra sem lép ki

        }
        [TestMethod]
        public void TestTick()
        {
            _model.ModelTick();//Léptetünk egyet a játékban,
            int AsteroidCounter = 0;
            int AsteroidPosition=-2;
            for (int i = 0; i < 10; i++)
            {
                if(_model.Table[0,i] == 2)
                {
                    AsteroidCounter++;
                    AsteroidPosition = i;
                }
            }
            Assert.AreEqual(1, AsteroidCounter); //Egy tick() után csak egy asteroida jelenik meg az első sorban.
            Assert.AreEqual(2, _model.Table[0, AsteroidPosition]); // Ahol megjelenik (Ami random) ott a mező értéke 2
            Assert.AreEqual(1000, _model.GameTime); // Egy sec telt el eddig
            Assert.AreEqual(false, _model.Table.bCanSpawn); //Letiltjuk a következő tick()-nek az aszteroidák spawnolását
            _model.ModelTick(); //Léptetünk még egyet
            Assert.AreEqual(2000, _model.GameTime); // Kettő sec telt el eddig
            Assert.AreEqual(0, _model.Table[0, AsteroidPosition]); //Az előző pozícióban 0 a mező értéke, mivel onnan elment az aszteroida
            Assert.AreEqual(2, _model.Table[1, AsteroidPosition]); // Az oszlop következő mezőjében azonban 2 a cella értéke
            Assert.AreEqual(true, _model.Table.bCanSpawn);// Továbbá a következő Tick()-nél a spawnolás megengedett.
            AsteroidCounter = 0;
            for (int i = 0; i < 10; i++)
            {
                if (_model.Table[0, i] == 2)
                {
                    AsteroidCounter++;
                }
            }
            Assert.AreEqual(0, AsteroidCounter);//Nincs enek új aszeroidák
        }
        [TestMethod]
        public void TestAsteroidNumbers()
        {
            _model.NewGame(); // új járékot kezdünk
            Assert.AreEqual(1, _model.Table.AsteroidNumber);//Kezdéskor 1 spawn lesz
            for (int i = 0; i < 22; i++)
            {
                int TempCounter = 0;
                _model.ModelTick();
                for (int j = 0; j < 11; j++)
                {
                    if(_model.Table[0,j] == 2)
                    {
                        TempCounter++;
                    }
                }
                if(!_model.Table.bCanSpawn) //Ha a következő tick()-ben tiltott akkor összevetjük
                {
                    Assert.AreEqual(_model.Table.AsteroidNumber-1, TempCounter); //indexelés miatt levonunk
                }
                else //Ha nem tiltott akkor 0 új spawnolt aszteroida lesz
                {
                    Assert.AreEqual(0, TempCounter);
                }
            }
        }
        [TestMethod]
        public void TestGameOver()
        {
            _model.NewGame(); // Új játékot kezdünk
            _model.Table[8, 5] = 2; //manuálisan aszteroidát rakunk a játékos elé
            _model.ModelTick();
            Assert.AreEqual(true, _model.bIsGameOver);
        }
    }
}
