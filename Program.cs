using NLog;

    string path = Directory.GetCurrentDirectory() + "\\nlog.config";
    var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();

        try
        {
            logger.Info("Welcome to the Movie Library!");

            var movies = LoadMoviesFromCSV("movies.csv");
            logger.Info("Number of movies loaded: {0}", movies.Length);

            while (true)
            {
                DisplayMenu();
                var choice = GetUserChoice();

                switch (choice)
                {
                    case 1:
                        ViewAllMovies(logger, movies);
                        break;
                    case 2:
                        movies = AddNewMovie(logger, movies);
                        break;
                    case 3:
                        logger.Info("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred: {0}", ex.Message);
            Console.WriteLine("An error occurred. Please check the log for details.");
        }

    static Movie[] LoadMoviesFromCSV(string fileName)
    {
        var lines = File.ReadAllLines(fileName);
        var movies = new Movie[lines.Length - 1]; 

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            var parts = line.Split(',');

            var movie = new Movie();
            movie.Id = int.Parse(parts[0]);
            movie.Title = parts[1];
            movie.Genres = parts[2].Split('|');

            movies[i - 1] = movie;
        }

        return movies;
    }

    static void ViewAllMovies(Logger logger, Movie[] movies)
    {
        logger.Info("\nAll Movies:");
        foreach (var movie in movies)
        {
            Console.WriteLine($"{movie.Id}: {movie.Title} - {string.Join(", ", movie.Genres)}");
        }
    }

    static Movie[] AddNewMovie(Logger logger, Movie[] movies)
    {
        Console.Write("Enter movie title: ");
        string title = Console.ReadLine();

        Console.Write("Enter movie genres (separated by '|'): ");
        string[] genres = Console.ReadLine().Split('|');
        for (int i = 0; i < genres.Length; i++)
        {
            genres[i] = genres[i].Trim();
        }

        bool titleExists = false;
        foreach (var movie in movies)
        {
            if (movie.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
            {
                titleExists = true;
                break;
            }
        }

        if (titleExists)
        {
            logger.Warn("Movie '{0}' already exists.", title);
            return movies;
        }

        int newId = 0;
        foreach (var movie in movies)
        {
            if (movie.Id > newId)
            {
                newId = movie.Id;
            }
        }
        newId++;

        var newMovie = new Movie { Id = newId, Title = title, Genres = genres };
        var newMovieList = new Movie[movies.Length + 1];
        Array.Copy(movies, newMovieList, movies.Length);
        newMovieList[movies.Length] = newMovie;

        try
        {
            File.AppendAllText("movies.csv", $"{newId},{title},{string.Join("|", genres)}\n");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to write the new movie to the CSV file: {0}", ex.Message);
            Console.WriteLine("An error occurred while adding the movie. Please check the log for details.");
        }

        logger.Info("Movie '{0}' added successfully!", title);

        return newMovieList;
    }

    static void DisplayMenu()
    {
        Console.WriteLine("\nMenu:");
        Console.WriteLine("1. View all movies");
        Console.WriteLine("2. Add a new movie");
        Console.WriteLine("3. Exit");
    }

    static int GetUserChoice()
    {
        Console.Write("Please enter your choice: ");
        return int.Parse(Console.ReadLine());
    }

class Movie
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string[] Genres { get; set; }
}
