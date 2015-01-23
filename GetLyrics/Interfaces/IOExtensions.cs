using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;
using Castle.Core.Resource;
using System.Net;
using System.Windows;

namespace GetLyrics
{
    public static class Extensions
    {
        public static bool IsDirectory(this string path)
        {
            try
            {
                return File.GetAttributes(path).HasFlag(FileAttributes.Directory);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsAudioFile(this string path)
        {
            return AudioFile.IsValid(path);
        }

        public static IEnumerable<AudioFile> GetAudioFiles(this DirectoryInfo directory)
        {
            var ret = new List<AudioFile>();
            var files = directory.GetAllFiles();
            files.ToList().AsParallel().ForAll(x =>
                {
                    try
                    {
                        var audioFile = new AudioFile(x);
                        if (!audioFile.HasCoverArt)
                            ret.Add(audioFile);
                    }
                    catch
                    {
                    }
                });

            return ret;
        }

        public static string[] GetDropFileNames(this IDataObject data)
        {
            return (string[])data.GetData(DataFormats.FileDrop);
        }

        public static bool IsImageFile(this string fileName)
        {
            string[] exts = { ".jpg", ".png", ".bmp" };
            return exts.Contains(Path.GetExtension(fileName));
        }

        public static byte[] GetBytes(this BitmapImage image)
        {
            byte[] data;
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                data = stream.ToArray();
            }

            return data;
        }

        public static byte[] GetImageBytesFromWeb(this Uri uri)
        {
            var fileName = Path.GetTempFileName();
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(uri, fileName);
            }
            byte[] imageData;
            using (var stream = File.OpenRead(fileName))
            {
                imageData = new byte[stream.Length];
                stream.Read(imageData, 0, (int)stream.Length);
            }
            File.Delete(fileName);
            return imageData;
        }

        public static IEnumerable<FileInfo> GetAllFiles(this DirectoryInfo directory)
        {
            return directory.EnumerateFiles("*.*", SearchOption.AllDirectories);
        }
    }
}
