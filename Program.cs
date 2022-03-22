﻿
using System.Diagnostics;
using System.Net;

public class DotNutBuster
{
    public static string wordlist_path = "sampleWordList.txt";
    public static string domain = "her.st";
    public static Thread  newThread = new Thread(Bust);
    public static Queue<string> dirs = new Queue<string>();
    public static List<string> found_directories = new List<string>();
    public static int url_count;
    //performance monitoring properties:
    public static bool verbious = true;
    public static Stopwatch wps_stopwatch = new Stopwatch();
    public static Stopwatch main_stopwatch = new Stopwatch();
    public static Dictionary<string, string> testedUrls = new Dictionary<string, string>();
    public static void Main()
    {
        ReadWordlist();
        newThread.Start();
        wps_stopwatch.Start();
        main_stopwatch.Start();
        Thread.Sleep(1000);
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
        WebClient client = new WebClient();
        string url = "";
        while (dirs.Count > 0)
        {
            try
            {
                url = dirs.Dequeue();
                client.DownloadString(url);
                Console.WriteLine(url);
                found_directories.Add(url);
                testedUrls.Add("valid url", url);
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
                {
                    testedUrls.TryAdd((ex.Message), url);
                    if(verbious == true)
                    {
                        var resp = (HttpWebResponse)ex.Response;
                        if (resp.StatusCode == HttpStatusCode.NotFound)
                        {
                            Console.WriteLine($"{url} : {resp.StatusCode}");
                        }
                    }
                }

            }
            finally
            {
                if (wps_stopwatch.Elapsed.TotalSeconds > 10)
                {
                    var newCount = dirs.Count;
                    Console.WriteLine($"WPS { ((url_count - newCount) / wps_stopwatch.Elapsed.TotalSeconds).ToString("0.00") }");
                    wps_stopwatch.Restart();
                    url_count = newCount;
                }
            }
        }
    }
    public static void PrintResults()
    {
        Console.WriteLine($"total time :  {main_stopwatch.Elapsed.Seconds}");
        Console.WriteLine("results:");
        foreach (var found_dir in found_directories)
        {
            Console.WriteLine(found_dir);
        }
        Console.ReadLine();
    }
}