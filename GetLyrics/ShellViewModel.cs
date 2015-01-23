using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.Windows;

namespace GetLyrics
{
    class ShellViewModel : PropertyChangedBase, IShellViewModel
    {
        #region Private Fields
        private ObservableCollection<AudioFile> _files = new ObservableCollection<AudioFile>();
        private AudioFile _selectedFile;
        private Album _selectedAlbum;
        private readonly IShellService _shellService;
        private readonly IWindowManager _windowManager;
        private readonly IDialogService _dialogService;
        #endregion

        #region Properties
        /*
        public ObservableCollection<AudioFile> AudioFiles
        {
            get
            {
                return _files;
            }
            set
            {
                _files = value;
                NotifyOfPropertyChange(() => AudioFiles);
                NotifyOfPropertyChange(() => Albums);
            }
        } */

        public ObservableCollection<AudioFile> AudioFiles
        {
            get
            {
                return new ObservableCollection<AudioFile>(_shellService.Files);
            }
        }

        public ObservableCollection<Album> Albums
        {
            get
            {
                return new ObservableCollection<Album>(_shellService.Albums);
            }
        }

        public AudioFile SelectedFile
        {
            get
            {
                return _selectedFile;
            }
            set
            {
                _selectedFile = value;
                NotifyOfPropertyChange(() => SelectedFile); 
            }
        }

        public Album SelectedAlbum
        {
            get
            {
                return _selectedAlbum;
            }
            set
            {
                _selectedAlbum = value;
                NotifyOfPropertyChange(() => SelectedAlbum);
                NotifyOfPropertyChange(() => CanEditAlbum);
            }
        }

        public bool HasFiles
        {
            get
            {
                return AudioFiles.Any();
            }
        }

        public bool IsProcessing
        {
            get { return _shellService.IsProcessing; }
        }

        public bool ValidDragDrop { get; set; }
        #endregion

        public ShellViewModel(IShellService shellService, IWindowManager windowManager, IDialogService dialogService)
        {
            _shellService = shellService;
            _windowManager = windowManager;
            _dialogService = dialogService;

            shellService.OnFilesChanged += SynchroniseViewModels;
            shellService.OnFinishedProceessing += OnFinishedProcessing;
        }

        #region Actions
        public bool CanEditAlbum
        {
            get { return _selectedAlbum != null; }
        }

        public void EditAlbum()
        {
            var title = String.Format(Strings.CoverArtWindowTitle, _selectedAlbum.ArtistName,
                _selectedAlbum.AlbumName);

            _windowManager.ShowWindow(new CoverArtItemViewModel(_selectedAlbum), settings: new Dictionary<string, object>
            {
                { "AllowDrop", true },
                { "Width", 620},
                { "Height", 380},
                { "ResizeMode", ResizeMode.NoResize },
                { "Title", title }
            });
        }

        // Bypass guard method
        public void EditDoubleClick()
        {
            EditAlbum();
        }

        public bool CanAddFiles
        {
            get { return true; }
        }

        public void AddFiles()
        {
            var paths = _dialogService.OpenFiles();
            Task.Run(() => _shellService.ProcessAddFilesDialog(paths));
            NotifyOfPropertyChange(() => IsProcessing);
        }

        public bool CanScanFolder
        {
            get { return true; }
        }

        public void ScanFolder()
        {
            var path = _dialogService.OpenFolders();
        }

        public bool CanShowOptions
        {
            get { return true; }
        }

        public void ShowOptions()
        {
            
        }

        #endregion

        #region Events
        public void FilesDroppedOnWindow(DragEventArgs e)
        {
            if (!ValidDragDrop) return;

            var fileNames = e.Data.GetDropFileNames();
            Task.Run(() => _shellService.ProcessDragDrop(fileNames));

            ValidDragDrop = false;
            NotifyOfPropertyChange(() => IsProcessing);
        }

        public void FilesDraggedOnWindow(DragEventArgs e)
        {
            ValidDragDrop = false;
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            var files = e.Data.GetDropFileNames();

            if (_shellService.ValidateDragDrop(files))
            {
                e.Effects = DragDropEffects.Copy;
                ValidDragDrop = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }
        #endregion

        #region Private Methods
        private void SynchroniseViewModels(object sender, EventArgs e)
        {
            NotifyOfPropertyChange(() => AudioFiles);
            NotifyOfPropertyChange(() => Albums);
        }

        private void OnFinishedProcessing(object sender, EventArgs e)
        {
            NotifyOfPropertyChange(() => IsProcessing);
        }
        #endregion 
    }
}
