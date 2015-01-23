using System.IO;
using Caliburn.Micro;

namespace GetLyrics
{
    public class FileBase : PropertyChangedBase
    {
        public FileBase(string path)
        {
            FileInfo = new FileInfo(path);
        }

        #region Properties

        private FileInfo FileInfo { get; set; }
        public string FileName
        {
            get { return FileInfo.Name; }
        }
        public string FileNameWithoutExtension
        {
            get { return FileInfo.Name.Replace(Extension, ""); }
        }
        public string FullPath
        {
            get { return FileInfo.FullName; }
        }
        public string Extension
        {
            get { return FileInfo.Extension; }
        }

        #endregion
    }
}
