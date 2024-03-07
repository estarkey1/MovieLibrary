using NLog;

public class Movie
{
    public int MovieId { get; set; }
    public string Title { get; set; }
    public string Genres { get; set; }
}

class Program
{
    static void Main()
    {
        string path = Directory.GetCurrentDirectory() + "\\nlog.config";
        var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();

        Console.WriteLine("Enter 1 to add movies to the file.");
        Console.WriteLine("Enter 2 to view movies in the file.");
        string? choice = Console.ReadLine();

        if (choice == "1")
        {
            AddMovie();
        }
        else if (choice == "2")
        {
            ViewMovies();
        }
        else
        {
            Console.WriteLine("Invalid choice. Please enter either 1 or 2.");
        }
    }

    static void AddMovie()
    {
        Console.WriteLine("Enter movie details:");

        Console.Write("MovieId: ");
        int movieId = int.Parse(Console.ReadLine());

        Console.Write("Title: ");
        string title = Console.ReadLine();

        Console.Write("Genres (Seperate with a |): ");
        string genres = Console.ReadLine();

        var movie = new Movie { MovieId = movieId, Title = title, Genres = genres };

        using (var writer = new StreamWriter("movies.csv", true))
        {
            writer.WriteLine($"{movie.MovieId},{EscapeCsvField(movie.Title)},{EscapeCsvField(movie.Genres)}");
        }

        Console.WriteLine("Movie added successfully!");
    }

    static void ViewMovies()
{
    // Read and display all movies from the CSV file
    using (var reader = new StreamReader("movies.csv"))
    {
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            var values = line.Split(',');

            if (values.Length == 3 &&
                int.TryParse(values[0], out int movieId) &&
                !string.IsNullOrEmpty(values[1]) &&
                !string.IsNullOrEmpty(values[2]))
            {
                string title = UnescapeCsvField(values[1]);
                string genres = UnescapeCsvField(values[2]);

                Console.WriteLine($"MovieId: {movieId}, Title: {title}, Genres: {genres}");
            }
            else
            {
                Console.WriteLine($"Error parsing line: {line}");
            }
        }
    }
}

    static string EscapeCsvField(string field)
    {
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            return $"\"{field.Replace("\"", "\"\"")}\"";
        else
            return field;
    }

    static string UnescapeCsvField(string field)
    {
        if (field.StartsWith("\"") && field.EndsWith("\""))
            return field.Substring(1, field.Length - 2).Replace("\"\"", "\"");
        else
            return field;
    }
}