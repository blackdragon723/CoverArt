using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GetLyrics
{
    public class AzLyrics
    {
        static int counter = 0;
        static bool countingDown = false;
        static Stopwatch timer = new Stopwatch();
        public static string ScrapeAzLyrics(string artistName, string songName)
        {
            if (counter == 20)
            {
                counter = 0;
                countingDown = true;
                timer.Restart();
                throw new InvalidOperationException("Must wait blah blah");
            }
            else if (countingDown)
            {
                if (timer.ElapsedMilliseconds < 60000)
                {
                    throw new InvalidOperationException("Must wait");
                }
                countingDown = false;
                timer.Stop();
            }

            if (artistName == null || songName == null)
            {
                throw new NullReferenceException();
            }

            string[] punctuation = 
            {
                "!", ",", "'", ".", "?", "\""
            };

            foreach (string character in punctuation)
            {
                artistName = artistName.Replace(character, string.Empty);
                songName = songName.Replace(character, string.Empty);
            }

            artistName = artistName.Replace("&", "and");
            songName = songName.Replace("&", "and");

            if (songName.Contains("(") && songName.Contains(")"))
            {
                var index1 = songName.IndexOf('(');
                var index2 = songName.IndexOf(')');

                songName = songName.Remove(index1, index2 - index1 + 1);
            }

            artistName = artistName.ToLower();
            if (artistName.StartsWith("the"))
            {
                artistName = artistName.Remove(0, 3);
            }
            artistName = artistName.Replace(" ", string.Empty);

            songName = songName.ToLower();
            songName = songName.Replace(" ", string.Empty);

            string path = "/" + artistName + "/" + songName + ".html";
            path = @"http://www.azlyrics.com/lyrics" + path;

            string siteHtml = string.Empty;
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 5.1; rv:31.0) Gecko/20100101 Firefox/31.0");
                    siteHtml = client.DownloadString(path);
                    siteHtml = WebUtility.HtmlDecode(siteHtml);
                }
                catch (WebException ex)
                {
                    Helper.Log(ex);

                    if (ex.Status == WebExceptionStatus.ProtocolError &&
                        ex.Response != null)
                    {
                        var response = ex.Response as HttpWebResponse;
                        if (response.StatusCode == HttpStatusCode.NotFound)
                        {
                            throw new FileNotFoundException();
                        }
                    }
                }
            }

            var firstTrim = siteHtml.IndexOf("<!-- start of lyrics -->");
            if (firstTrim == -1)
            {
                //throw new FileNotFoundException(); // bad web page - doesn't return 404
            }

            firstTrim += 24;
            siteHtml = siteHtml.Remove(0, firstTrim);

            var secondTrim = siteHtml.IndexOf("<!-- end of lyrics -->");
            siteHtml = siteHtml.Remove(secondTrim);

            siteHtml = siteHtml.Replace("<br />", string.Empty);
            siteHtml = siteHtml.Remove(0, 2);

            counter++;
            return siteHtml;
        }
    }
}
