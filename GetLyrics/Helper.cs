using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Windows;
using Google.API.Search;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace GetLyrics
{
    class Helper
    {
        public static ObservableCollection<AudioFile> GetAudioFilesFromDirectory(DirectoryInfo di)
        {
            var ret = new ObservableCollection<AudioFile>();
           
            string[] extensions = 
            {
                "*.mp3",
                "*.m4a",
                "*.flac"
            };

            foreach (var searchPattern in extensions)
            {
                try
                {
                    foreach (var file in di.EnumerateFiles(searchPattern, SearchOption.AllDirectories).Select(fi => new AudioFile(fi)))
                    {
                        ret.Add(file);
                    }
                }
                catch (Exception e)
                {
                    Log(e);
                }
            }

            return ret;
        }

        public static void Log(Exception e)
        {
            var fullString = string.Format("{0}: {1} \t {2}", DateTime.Now.ToString(@"d/M/yyyy hh:mm:ss tt"), e.Message, e.InnerException);

            using (var log = new StreamWriter("log.txt", true))
            {
                log.WriteLine(fullString);
            }
        }

        
        public static string ParseSing365(string artistName, string songName)
        {
            const string url = @"http://www.sing365.com/music/lyric.nsf/{0}-lyrics-{1}";

            string[] punctuation = 
            {
                "!", "(", ")", ",", "'", ".", "&", "?",
            };

            foreach (string special in punctuation)
            {
                artistName = artistName.Replace(special, string.Empty);
                songName = songName.Replace(special, string.Empty);
            }

            artistName = artistName.Replace(" ", "-");
            songName = songName.Replace(" ", "-");

            SearchGoogle(string.Format(url, songName, artistName));

            return string.Empty;
        }

        public static List<string> SearchGoogle(string searchExpression)
        {
            var urlTemplate = @"http://ajax.googleapis.com/ajax/services/search/web?v=1.0&rsz=large&safe=active&q={0}&start={1}";

            Uri searchUrl;
            var ret = new List<string>();
            int[] offsets = { 0, 8, 16, 24, 32, 40, 48 };

            foreach (var offset in offsets)
            {
                searchUrl = new Uri(string.Format(urlTemplate, searchExpression, offset));
                var page = new WebClient().DownloadString(searchUrl);

                JObject o = (JObject)JsonConvert.DeserializeObject(page);
                var resultsLinq = from result in o["responseData"]["results"].Children()
                                  select result.Value<string>("url").ToString();

                foreach (string s in resultsLinq)
                {
                    ret.Add(s);
                } 
            }
            return ret;
        }
    }
}
