
using System.Diagnostics;
using System.Net;

public class DotNutBuster
{
    public static bool verbious = false;
    public static string wordlist_path = "sampleWordList.txt";
    public static string domain = "her.st";
    public static Queue<string> dirs = new Queue<string>();
    public static int url_count;
    public static WebClient client = new WebClient();
    public static Stopwatch stopwatch = new Stopwatch();
    public static List<string> found_directories = new List<string>();
    public static void Main()
    {
        ReadWordlist();
        stopwatch.Start();
        Bust();
        PrintResults();
    }
    public static void ReadWordlist()
    {
        foreach (var line in File.ReadAllLines(wordlist_path))
        {
            dirs.Enqueue($"https://{domain}/{line}.html");
        }
        url_count = dirs.Count;
    }
    public static void Bust()
    {
        while (dirs.Count > 0)
        {
            try
            {
                string url = dirs.Dequeue();
                //Console.WriteLine($"trying {url}");
                client.DownloadString(url);
                Console.WriteLine(url);
                found_directories.Add(url);
            }
            catch (WebException ex)
            {
                if (verbious == true && ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
                {
                    var resp = (HttpWebResponse)ex.Response;
                    if (resp.StatusCode == HttpStatusCode.NotFound)
                    {
                        Console.WriteLine($": {resp.StatusCode}");
                    }
                }

            }
            finally
            {
                if (stopwatch.Elapsed.TotalSeconds > 10)
                {
                    var newCount = dirs.Count;
                    Console.WriteLine($"WPS { ((url_count - newCount) / stopwatch.Elapsed.TotalSeconds).ToString("0.00") }");
                    stopwatch.Restart();
                    url_count = newCount;
                }
            }
        }
    }
    public static void PrintResults()
    {
        Console.WriteLine("results:");
        foreach (var found_dir in found_directories)
        {
            Console.WriteLine(found_dir);
        }
        Console.ReadLine();
    }
}