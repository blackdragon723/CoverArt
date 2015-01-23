using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;

namespace GetLyrics
{
    public class Album : PropertyChangedBase
    {
        #region Fields

        private readonly List<AudioFile> _files;

        #endregion

        #region Properties

        public IReadOnlyCollection<AudioFile> Files
        {
            get
            {
                return _files.AsReadOnly();
            }
        }

        public BitmapImage CoverArt
        {
            get
            {
                return HasCoverArt ? Files.First().CoverArt : null;
            }
        }

        public string AlbumName { set; get; }
        public string ArtistName
        {
            get
            {
                return Files.First().Artist;
            }
        }

        public string HasCoverArtString
        {
            get
            {
                return HasCoverArt ? "Yes" : "No";
            }
        }

        public bool HasCoverArt
        {
            get { return Files.All(x => x.HasCoverArt); }
        }

        #endregion
        
        public Album(IEnumerable<AudioFile> files)
        {
            _files = new List<AudioFile>(files);
        }

        public void Update()
        {
            NotifyOfPropertyChange(() => HasCoverArtString);
        }

        
    }
}
