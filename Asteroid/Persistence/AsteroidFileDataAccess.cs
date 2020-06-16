using System;
using System.IO;
using System.Threading.Tasks;

namespace Asteroid.Persistence
{
    class AsteroidFileDataAccess : IAsteroidDataAccess
    {
        public async Task<AsteroidTable> LoadAsync(String path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path)) // fájl megnyitása
                {
                    String line = await reader.ReadLineAsync();
                    String[] numbers = line.Split(' '); // beolvasunk egy sort, és a szóköz mentén széttöredezzük
                    Int32 AsteroidNumbers = Int32.Parse(numbers[0]); // 
                    Int32 SurvivedTime = Int32.Parse(numbers[1]); //
                    Int32 PlayerPosition = Int32.Parse(numbers[2]); //
                    bool BcanSpawn = Boolean.Parse(numbers[3]); //
                    AsteroidTable table = new AsteroidTable(); // létrehozzuk a táblát

                    for (Int32 i = 0; i < 10; i++)
                    {
                        line = await reader.ReadLineAsync();
                        numbers = line.Split(' ');

                        for (Int32 j = 0; j < 11; j++)
                        {
                            //table.SetValue(i, j, Int32.Parse(numbers[j]), false);
                            table[i, j] = Int32.Parse(numbers[j]);
                        }
                    }
                    table.AsteroidNumber = AsteroidNumbers;
                    table.SurvivedTime = SurvivedTime;
                    table.PlayerPosition = PlayerPosition;
                    table.bCanSpawn = BcanSpawn;

                    return table;
                }
            }
            catch
            {
                throw new AsteroidDataException();
            }
        }
        public async Task SaveAsync(String path, AsteroidTable table)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path)) // fájl megnyitása
                {
                    writer.Write(table.AsteroidNumber); // kiírjuk a méreteket
                    await writer.WriteAsync(" " + table.SurvivedTime);
                    await writer.WriteAsync(" " + table.PlayerPosition);
                    await writer.WriteLineAsync(" " + table.bCanSpawn);
                    for (Int32 i = 0; i < 10; i++)
                    {
                        for (Int32 j = 0; j < 11; j++)
                        {
                            await writer.WriteAsync(table[i, j] + " "); // kiírjuk az értékeket
                        }
                        await writer.WriteLineAsync();
                    }
                }
            }
            catch
            {
                throw new AsteroidDataException();
            }
        }

    }
}
