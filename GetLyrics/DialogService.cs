using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ookii.Dialogs.Wpf;

namespace GetLyrics
{
    public class DialogService : IDialogService
    {
        public string[] OpenFiles()
        {
            var title = Strings.OpenFileDialogTitleString;
            var dialog = new VistaOpenFileDialog {Title = title, Multiselect = true};
            
            var exts = ListToFilter(AudioFile.Extensions);
            dialog.Filter = String.Format(("Audio Files({0})|{0}"), exts);

            var result = dialog.ShowDialog();

            return result != true ? null : dialog.FileNames;
        }

        public string OpenFolders()
        {
            var title = Strings.OpenFolderDialogTitleString;
            var dialog = new VistaFolderBrowserDialog();

            var result = dialog.ShowDialog();

            return result != true ? null : dialog.SelectedPath;

        }

        public static string ListToFilter(string[] extensions)
        {
            var exts = new StringBuilder();
            extensions.ToList().ForEach(x =>
            {
                var y = x.Insert(0, "*");
                y += ";";
                exts.Append(y);
            });

            return extensions.ToString();
        }
    }
}
