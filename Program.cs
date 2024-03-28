using NLog;

class Program
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    static void Main(string[] args)
    {
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
                        ViewAllMovies(movies);
                        break;
                    case 2:
                        movies = AddNewMovie(movies);
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
    }

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

    static (int Id, string Title, string[] Genres, string Director, TimeSpan? Runtime)[] LoadMoviesFromCSV(string fileName)
    {
        return File.ReadAllLines(fileName)
            .Skip(1)
            .Select(line => line.Split(','))
            .Select(parts =>
            {
                var genres = parts[2].Split('|');
                var director = parts[3];
                TimeSpan? runtime = TimeSpan.TryParse(parts[4], out TimeSpan parsedTime) ? parsedTime : (TimeSpan?)null;
                return (int.Parse(parts[0]), parts[1], genres, director, runtime);
            })
            .ToArray();
    }

    static void ViewAllMovies((int Id, string Title, string[] Genres, string Director, TimeSpan? Runtime)[] movies)
    {
        logger.Info("\nAll Movies:");
        foreach (var movie in movies)
        {
            string runtimeString = movie.Runtime.HasValue ? movie.Runtime.ToString() : "unassigned";
            Console.WriteLine($"{movie.Id}: {movie.Title} - {string.Join(", ", movie.Genres)} - Director: {movie.Director} - Runtime: {runtimeString}");
        }
    }

    static (int Id, string Title, string[] Genres, string Director, TimeSpan? Runtime)[] AddNewMovie((int Id, string Title, string[] Genres, string Director, TimeSpan? Runtime)[] movies)
    {
        Console.Write("Enter movie title: ");
        string title = Console.ReadLine();

        Console.Write("Enter movie genres (separated by '|'): ");
        string[] genres = Console.ReadLine().Split('|').Select(genre => genre.Trim()).ToArray();

        Console.Write("Enter director: ");
        string director = Console.ReadLine();

        TimeSpan? runtime = null;
        Console.Write("Enter runtime (HH:mm:ss or 'unassigned'): ");
        TimeSpan.TryParse(Console.ReadLine(), out TimeSpan parsedTime);
        runtime = parsedTime;

        if (movies.Any(movie => movie.Title.Equals(title, StringComparison.OrdinalIgnoreCase)))
        {
            logger.Warn("Movie '{0}' already exists.", title);
            return movies;
        }

        int newId = movies.Max(movie => movie.Id) + 1;

        var newMovie = (newId, title, genres, director, runtime);
        return movies.Append(newMovie).ToArray();
    }
}
