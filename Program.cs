using iTunesSearcher.Entities;
using iTunesSearcher.Helpers;
using iTunesSearcher.Models;
using iTunesSearcher.WebClients;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace iTunesSearcher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
             .MinimumLevel.Information()
             .WriteTo.RollingFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log"), LogEventLevel.Information, fileSizeLimitBytes: 1073741824)
             .CreateLogger();

            await Input();
        }

        private static async Task Input()
        {
            try
            {
                Console.WriteLine("Availible commands:");
                Console.WriteLine("f - find albums by artist");
                Console.WriteLine("e - exit");

                Console.Write("\nWhat should I do? ");
                var input = Console.ReadLine();

                if (input == "f")
                {
                    Console.Write("Set the artist: ");
                    string artist = null;

                    while (string.IsNullOrEmpty(artist))
                    {
                        CheckNullInput(Console.ReadLine(), out artist);
                    }

                    var res = await SearchArtistAlbums(artist.ValidateArtist());
                    if (res != null && res.resultCount > 0)
                    {
                        Console.WriteLine($"\nFind {res.resultCount} albums:\n");
                        Console.WriteLine($"Release Date - Artist - Album");
                        foreach (var i in res.results)
                            Console.WriteLine($"{i.releaseDate:yyyy.MM.dd} - {i.artistName} - {i.collectionName}");
                    }
                    else
                    {
                        Console.WriteLine("For the specified artist there is no data in the database");
                    }

                    Console.WriteLine();
                    await Input();
                }
                else if (input == "e")
                {
                    Console.Write("Do you really want to leave?(y/n) ");
                    var exit = Console.ReadLine();

                    if (exit == "y")
                    {
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine();
                        await Input();
                    }
                }
                else
                {
                    Console.WriteLine("Unexpected command, please try again...\n");
                    await Input();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
                Log.Error(JsonConvert.SerializeObject(ex));
            }
        }

        /// <summary>
        /// Поиск исполнителей по введенному имени, вывод в консоль и сохранение в базу, если ответ апи пуст то вывод из базы
        /// </summary>
        /// <param name="artist"></param>
        /// <returns></returns>
        private static async Task<AlbumResponseModel> SearchArtistAlbums(string artist)
        {
            var validArtist = artist.ValidateArtist();
            var url = $"{Consts.iTunes_base_url}/search?term={validArtist}&media=music&entity=album&attribute=artistTerm&limit=200";

            var response = await Source.Instance.GetDataAsync(url);
            if (response != null)
            {
                var res = JsonConvert.DeserializeObject<AlbumResponseModel>(response);
                if (res.resultCount != 0)
                {
                    res.results = res.results.OrderByDescending(x => x.releaseDate).ToList();

                    using DB db = new DB();
                    var rows = db.Albums.Select(x => x.collectionId).ToArray();
                    foreach (var i in res.results)
                    {
                        if (!rows.Contains(i.collectionId))
                            db.Albums.Add(i);
                    }
                    db.SaveChanges();

                    return res;
                }
                else
                {
                    Console.WriteLine("Nothing was found by request, the result from the database is shown below:");

                    using DB db = new DB();
                    var rows = db.Albums.Where(x => x.artistName.ToLower().Contains(artist)).ToList();
                    if (rows != null && rows.Count > 0)
                    {
                        AlbumResponseModel model = new AlbumResponseModel()
                        {
                            resultCount = rows.Count,
                            results = rows
                        };

                        return model;
                    }

                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Проверка что имя исполнителя не пустое
        /// </summary>
        /// <param name="artist"></param>
        /// <param name="res"></param>
        private static void CheckNullInput(string artist, out string res)
        {
            if (string.IsNullOrEmpty(artist))
                Console.Write("Incorrect input, try again: ");

            res = artist;
        }
    }
}
