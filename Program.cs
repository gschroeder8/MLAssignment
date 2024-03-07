using NLog;
//dotnet add package NLog --version 5.2.8

    string path = Directory.GetCurrentDirectory() + "//nlog.config";
    var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();

    static void UserMenu()
    {
        Console.WriteLine("\nMenu:");
        Console.WriteLine("1. View all movies");
        Console.WriteLine("2. Add a new movie");
        Console.WriteLine("3. Exit");
    }

    static int UserChoice()
    {
        Console.Write("Please enter your choice: ");
        return int.Parse(Console.ReadLine());
    }
    
        try
        {
            logger.Info("Welcome to the Movie Library!");

            var movies = LoadMoviesFromCSV("movies.csv");
            logger.Info("Number of movies loaded: {0}", movies.Length);

            while (true)
            {
                UserMenu();
                var choice = UserChoice();

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
    

    static (int Id, string Title, string[] Genres)[] LoadMoviesFromCSV(string fileName)
    {
        var lines = File.ReadAllLines(fileName);
        var movies = new (int Id, string Title, string[] Genres)[lines.Length - 1];

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            var parts = line.Split(',');

            var genres = parts[2].Split('|');

            movies[i - 1] = (int.Parse(parts[0]), parts[1], genres);
        }

        return movies;
    }

    static void ViewAllMovies(Logger logger, (int Id, string Title, string[] Genres)[] movies)
    {
        logger.Info("\nAll Movies:");
        foreach (var movie in movies)
        {
            Console.WriteLine($"{movie.Id}: {movie.Title} - {string.Join(", ", movie.Genres)}");
        }
    }

    static (int Id, string Title, string[] Genres)[] AddNewMovie(Logger logger, (int Id, string Title, string[] Genres)[] movies)
    {
        Console.Write("Enter movie title: ");
        string? title = Console.ReadLine();

        Console.Write("Enter movie genres (separated by '|'): ");
        string[] genres = Console.ReadLine().Split('|');
        for (int i = 0; i < genres.Length; i++)
        {
            genres[i] = genres[i].Trim();
        }

        bool titleExists = movies.Any(movie => movie.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

        if (titleExists)
        {
            logger.Warn("Movie '{0}' already exists.", title);
            return movies;
        }

        int newId = movies.Max(movie => movie.Id) + 1;

        var newMovie = (newId, title, genres);
        var newMovieList = new (int Id, string Title, string[] Genres)[movies.Length + 1];
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


