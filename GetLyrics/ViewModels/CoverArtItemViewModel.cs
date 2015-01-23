using System;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using DragEventArgs = System.Windows.DragEventArgs;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace GetLyrics
{
    class CoverArtItemViewModel : PropertyChangedBase, ICoverArtItemViewModel, IViewAware
    {
        private readonly Album _album;
        private byte[] _data;
        private bool _fromWeb;
        
        // todo
        //private bool _validDragDrop;

        private BitmapImage _newImage;

        public ImageSource NewImage
        {
            get { return _newImage; }
        }

        public ImageSource CurrentImage
        {
            get { return _album.CoverArt; }
        }

        public string Name
        {
            get { return _album.AlbumName; }
        }

        public CoverArtItemViewModel(Album album)
        {
            _album = album;
        }

        public void FilesDroppedOnWindow(DragEventArgs e)
        {
            var formats = e.Data.GetFormats();

            if (formats.Contains("text/html"))
            {
                ProcessDragDropFromWeb(e.Data);
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                ProcessDragDropFromExplorer(e.Data);
            }

            NotifyOfPropertyChange(() => NewImage);
            NotifyOfPropertyChange(() => CanSaveCoverArt);
        }

        private void ProcessDragDropFromWeb(IDataObject data)
        {
            _fromWeb = true;
            var obj = data.GetData("text/html");
            var html = obj as string ?? GetHtmlFromStream(obj);

            var match = new Regex(@"<img[^/]src=""([^""]*)""").Match(html);

            if (!match.Success) return;

            var uri = new Uri(match.Groups[1].Value);
            _data = uri.GetImageBytesFromWeb();

            _newImage = CreateImage();
        }

        private void ProcessDragDropFromExplorer(IDataObject data)
        {
            var files = (string[])data.GetData(DataFormats.FileDrop);
            try
            {
                var picturePath = files.Single(x => x.IsImageFile());
                _newImage = new BitmapImage(new Uri(picturePath));
            }
            catch (InvalidOperationException)
            {
            }
        }

        private BitmapImage CreateImage()
        {
            var image = new BitmapImage();

            using (var stream = new MemoryStream(_data))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
            }
            return image;
        }

        private static string GetHtmlFromStream(object obj)
        {
            var ms = (MemoryStream)obj;
            var buffer = new byte[ms.Length];
            ms.Read(buffer, 0, (int)ms.Length);
            return buffer[1] == (byte)0 ? Encoding.Unicode.GetString(buffer) : Encoding.ASCII.GetString(buffer);
        }

        //todo
        public void FilesDraggedOnWindow(DragEventArgs e)
        {
            
        }

        public bool CanSaveCoverArt
        {
            get { return _newImage != null; }
        }

        public void SaveCoverArt()
        {
            if (_newImage == null) return;

            foreach (var file in _album.Files)
            {
                var data = _fromWeb ? _data : _newImage.GetBytes();
                file.EmbedCoverArt(data);
                _album.Update();
            }

            Close();
        }

        public void Close()
        {
            _dialogWindow.Close();
        }

        #region IViewAware

        private Window _dialogWindow;
        public void AttachView(object view, object context = null)
        {
            _dialogWindow = view as Window;
            if (ViewAttached != null)
                ViewAttached(this,
                   new ViewAttachedEventArgs { Context = context, View = view });
        }

        public object GetView(object context = null)
        {
            return _dialogWindow;
        }

        public event EventHandler<ViewAttachedEventArgs> ViewAttached;

        #endregion
    }
}
