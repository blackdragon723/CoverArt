using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using TagLib;

namespace GetLyrics
{
    public class AudioFile : FileBase, IEquatable<AudioFile>
    {
        #region Static Methods and Constants

        public static readonly string[] Extensions = 
        {
            ".mp3", ".wav", ".flac", ".m4a"
        };
        public static bool IsValid(FileInfo file)
        {
            string ext = file.Extension.ToLower();
            return Extensions.Contains(ext);
        }
        public static bool IsValid(string path)
        {
            var file = new FileInfo(path);
            return IsValid(file);
        }

        #endregion
        

        #region Fields

        private TagLib.File _id3;

        #endregion

        #region Properties

        public bool ToGetLyrics { set; get; }
        public string Artist
        {
            set { _id3.Tag.Performers = new[] { value }; }
            get { return _id3.Tag.FirstPerformer; }
        }
        public string TrackName
        {
            set { _id3.Tag.Title = value; }
            get { return _id3.Tag.Title; }
        }
        public string Album
        {
            set { _id3.Tag.Album = value; }
            get { return _id3.Tag.Album; }
        }

        public bool HasCoverArt
        {
            get { return _id3.Tag.Pictures.Any(); }
        }

        public string HasLyricsString
        {
            get
            {
                return HasLyrics ? Strings.Yes : Strings.No;
            }
        }

        public string Lyrics { get { return _id3.Tag.Lyrics; } }

        //todo fix this
        public BitmapImage CoverArt
        {
            get
            {
                var image = new BitmapImage();
                var bytes = _id3.Tag.Pictures[0].Data;

                using (var stream = new MemoryStream(bytes.ToArray()))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    return image;
                }
            }
        }

        public bool HasLyrics
        {
            get
            {
                return !String.IsNullOrWhiteSpace(_id3.Tag.Lyrics);
            }
        }

        #endregion

        #region Constructors

        public AudioFile(string path)
            : base(path)
        {
            Create();
        }

        public AudioFile(FileSystemInfo file)
            : base(file.FullName)
        {
            Create();
        }

        private void Create()
        {
            if (!ValidAudioFile)
            {
                throw new InvalidOperationException(Strings.InvalidAudioFileCreation);
            }

            _id3 = TagLib.File.Create(FullPath);
            if (TrackName == null)
            {
                TrackName = FileNameWithoutExtension;
            }
        }

        private bool ValidAudioFile
        {
            get
            {
                return Extensions.Contains(Extension.ToLower());
            }
        }

        #endregion

        #region Public Methods

        public void EmbedLyrics(string lyrics)
        {
            _id3.Tag.Lyrics = lyrics;
            _id3.Save();
            NotifyOfPropertyChange();
        }


        //todo this too
        public void EmbedCoverArt(byte[] cover)
        {
            if (cover == null) return;

            _id3.Tag.Pictures = new IPicture[] { new Picture(new ByteVector(cover)) };
            _id3.Save();
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(() => HasCoverArt);
            //NotifyOfPropertyChange("HasCoverArtString");
        }

        public void FixSong(Dictionary<string, string> metadata)
        {
            Album = metadata["album_name"];
            Artist = metadata["artist"];
            TrackName = metadata["song_name"];
            _id3.Tag.Year = uint.Parse(metadata["year"]);
            _id3.Tag.Track = uint.Parse(metadata["track_no"]);
            _id3.Tag.TrackCount = uint.Parse(metadata["no_tracks"]);
            _id3.Tag.Comment = metadata["bio"];
            _id3.Tag.Genres = new[] { metadata["genre"] };

            _id3.Save();
        }

        #endregion

        #region IEquatable
        public bool Equals(AudioFile file)
        {
            if (file == null)
                return false;

            return file.Artist == Artist && file.TrackName == TrackName;
        }

        public override int GetHashCode()
        {
            var hashArtist = Artist == null ? 0 : Artist.GetHashCode();
            var hashTrackName = TrackName == null ? 0 : TrackName.GetHashCode();

            return hashArtist ^ hashTrackName;
        }
        #endregion

        bool IEquatable<AudioFile>.Equals(AudioFile other)
        {
            if (other == null)
                return false;

            return other.Artist == Artist && other.TrackName == TrackName;
        }
    }
}
