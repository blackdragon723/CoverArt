using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GetLyrics
{
    public class ShellService : IShellService
    {
        private readonly List<AudioFile> _files = new List<AudioFile>();

        #region Events
        /// <summary>
        /// Notifies of the change in list to sync with viewmodels, etc
        /// </summary>
        public event FilesChangedEventHandler OnFilesChanged;
        protected void FilesChanged()
        {
            if (OnFilesChanged != null)
            {
                OnFilesChanged(this, new EventArgs());
            }
        }
        #endregion

        public event FinishedProcessingEventHandler OnFinishedProceessing;

        protected void FinishedProcessing()
        {
            if (OnFinishedProceessing != null)
            {
                OnFinishedProceessing(this, new EventArgs());
            }
        }

        #region Properties

        public bool IsProcessing { get; private set; }

        public IReadOnlyCollection<AudioFile> Files
        {
            get
            {
                return _files.AsReadOnly();
            }
        }

        public IReadOnlyCollection<Album> Albums
        {
            get
            {
                return GetAlbums().Select(album => CreateAlbum(album.Key, album.Value)).ToList();
            }
        }

        #endregion

        #region Public Methods

        public bool ValidateDragDrop(string[] paths)
        {
            return paths.Any(x => x.IsAudioFile()) || paths.Any(x => x.IsDirectory());
        }

        public void ProcessDragDrop(string[] paths)
        {
            IsProcessing = true;

            var files = GetAudioFiles(paths);
            var unique = files.Except(_files);
            AddRange(unique);

            IsProcessing = false;
            FinishedProcessing();
        }

        public void ProcessAddFilesDialog(string[] paths)
        {
            IsProcessing = true;

            var files = new List<AudioFile>();
            paths.ToList().AsParallel().ForAll(x =>
            {
                var newFile = new AudioFile(x);

                if (!newFile.HasCoverArt)
                    files.Add(newFile);
            });

            AddRange(files);
            IsProcessing = false;
            FinishedProcessing();
        }


        #endregion

        #region Private Methods

        private IEnumerable<KeyValuePair<string, string>> GetAlbums(bool onlyEmpty = false)
        {
            if (onlyEmpty)
            {
                return new HashSet<KeyValuePair<string, string>>(Files.Where(x => x.Album != null)
                                                .Where(x => !x.HasCoverArt)
                                                .Select(x => new KeyValuePair<string,string>(x.Album, x.Artist)));
            }

            return new HashSet<KeyValuePair<string, string>>(Files.Where(x => x.Album != null)
                .Select(x => new KeyValuePair<string, string>(x.Album, x.Artist)));
        }

        private Album CreateAlbum(string name, string artist)
        {
            var files = Files.Where(x => x.Album == name && x.Artist == artist).ToList();
            return new Album(files)
            {
                AlbumName = name,
            };
        }

        private static IEnumerable<AudioFile> GetAudioFiles(IEnumerable<string> paths)
        {
            var ret = new List<AudioFile>();
            foreach (var path in paths)
            {
                if (path.IsDirectory())
                {
                    var directory = new DirectoryInfo(path);
                    var files = directory.GetAudioFiles();
                    ret.AddRange(files);
                }
                else
                {
                    try
                    {
                        var file = new AudioFile(path);
                        ret.Add(file);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return ret;
        }

        private void Add(AudioFile file)
        {
            _files.Add(file);
            FilesChanged();
        }

        private void AddRange(IEnumerable<AudioFile> files)
        {
            _files.AddRange(files);
            FilesChanged();
        }

        #endregion
    }
}
