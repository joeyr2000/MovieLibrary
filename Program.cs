using System;
using System.IO;
using NLog.Web;
using System.Collections.Generic;

namespace MovieLibrary
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Directory.GetCurrentDirectory() + "\\nlog.config";
            var logger = NLog.Web.NLogBuilder.ConfigureNLog(path).GetCurrentClassLogger();
            var file = "movies.csv";
            string input;

            logger.Info("Program started");
            //Create List to hold all movies
            List<string> allMovies = new List<string>();
            
            StreamReader sr = new StreamReader(file);
            while (!sr.EndOfStream) {
                allMovies.Add(sr.ReadLine());
            }
            sr.Close();


            do {
            Console.WriteLine("Press 1 to view all movies");
            Console.WriteLine("Press 2 to add new movie");
            Console.WriteLine("Press any other key to exit");

            input = Console.ReadLine();

            if (input == "1") {
                    //Read all movies from List

                    foreach (string line in allMovies) {
                        string[] movie = line.Split(',');
                        string[] genre = movie[movie.Length - 1].Split('|');
                        string[] movieTitle = new string[movie.Length-2];

                        for (int i = 0; i < movie.Length - 2; i++){
                            movieTitle[i] = movie[i+1];
                        }

                        Console.WriteLine($"Movie: {String.Join(',', movieTitle)} || Genre(s): {String.Join(',', genre)}");
                    }

            } else if (input == "2") {
                //Accept new movie entry

                Console.WriteLine("Enter movie title: ");
                string newMovie = Console.ReadLine();
                bool movieExists = false;

                //Check if movie title already registered
                foreach (string line in allMovies) {
                    string[] movie = line.Split(',');
                    string[] movieTitle = new string[movie.Length-2];

                    for (int i = 0; i < movie.Length - 2; i++){
                        movieTitle[i] = movie[i+1];
                    }
                    if (newMovie.Equals(String.Join(',', movieTitle))) {
                        logger.Warn("Movie already exists");
                        movieExists = true;
                        break;
                    }
                }

                //If movie does not exist, accept genre
                if (!movieExists) {
                    List<string> genre = new List<string>();
                    string ans;

                    do {
                        Console.WriteLine("Enter genre: ");
                        genre.Add(Console.ReadLine());

                        Console.WriteLine("Add another genre? (Y / N)");
                        ans = Console.ReadLine().ToLower();

                        while (ans != "y" && ans != "n") {
                            logger.Info("Invalid input");
                            ans = Console.ReadLine().ToLower();
                        }
                    } while (ans == "y");

                    //Find most recent movie entry to generate new MovieID
                    int movieID = 0;
                    string lastLine = allMovies[allMovies.Count - 1];
                    string[] lastMovie = lastLine.Split(',');

                    try {
                        int.TryParse(lastMovie[0], out movieID);
                    } catch (Exception ex) {
                        logger.Warn(ex);
                    }
                    movieID++;
                    sr.Close();

                    //Write succesful movie entry + add to List of movies
                    StreamWriter sw = new StreamWriter(file, true);
                    string newEntry = movieID + "," + newMovie + "," + String.Join('|',genre);
                    sw.WriteLine(newEntry);
                    allMovies.Add(newEntry);
                    sw.Close();

                    Console.WriteLine($"Movie added!\nMovieID: {movieID} || Movie Title: {newMovie} || Genre(s): {String.Join(',',genre)}");
                }
            }

            //Program runs while user chooses 1 or 2
            } while (input == "1" || input == "2");

            logger.Info("Program ended");
        }
    }
}
