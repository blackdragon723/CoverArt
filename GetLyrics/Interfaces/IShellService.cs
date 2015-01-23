using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetLyrics
{
    public delegate void FilesChangedEventHandler(object sender, EventArgs e);

    public delegate void FinishedProcessingEventHandler(object sender, EventArgs e);
    public interface IShellService
    {
        event FilesChangedEventHandler OnFilesChanged;
        event FinishedProcessingEventHandler OnFinishedProceessing;

        IReadOnlyCollection<AudioFile> Files { get; }
        IReadOnlyCollection<Album> Albums { get; }
        bool IsProcessing { get; }

        bool ValidateDragDrop(string[] paths);
        void ProcessDragDrop(string[] paths);
        void ProcessAddFilesDialog(string[] paths);
    }
}
