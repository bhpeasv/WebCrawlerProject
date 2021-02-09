using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace WebCrawlerProject
{
    class Program
    {
        private static HashSet<string> AllUrls = new HashSet<string>();
        private static List<string> Frontier = new List<string>();

        static void Main(string[] args)
        {
            Console.Write("Enter some url: ");
            string urlStr = Console.ReadLine();

            Uri url = new UriBuilder(urlStr).Uri;
            Console.WriteLine("Url = " + url.ToString());

            ResolvePage(url);

            foreach (string link in Frontier)
            {
                Console.WriteLine(link);
            }
        }

        private static void ResolvePage(Uri uri)
        {
            Uri hostUrl = new UriBuilder(uri.Host).Uri;

            WebClient client = new WebClient();
            string pageContent = client.DownloadString(uri.ToString());
            var hrefPattern = new Regex("href\\s*=\\s*(\"(?<url>[^\"]*)\"|('?<url>[^']*')|(?<url>\\S+))", RegexOptions.IgnoreCase);

            var urls = hrefPattern.Matches(pageContent);

            foreach (Match url in urls)
            {
                string newUrl = hrefPattern.Match(url.Value).Groups["url"].Value;
                Uri absoluteUrl = Normalize(hostUrl, newUrl);
                if (absoluteUrl != null)
                { 
                    string newLink = absoluteUrl.ToString().ToLower();
                    if (! AllUrls.Contains(newLink))
                    {
                        AllUrls.Add(newLink);
                        Frontier.Add(newLink);
                    }
                }
            }
        }

        private static Uri Normalize(Uri hostUrl, string newUrl)
        {
            if (Uri.TryCreate(newUrl, UriKind.RelativeOrAbsolute, out var url))
            {
                return Uri.TryCreate(hostUrl, url, out var absoluteUrl) ? absoluteUrl : null;
            }
            return null;
        }
    }
}
