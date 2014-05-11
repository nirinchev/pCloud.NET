using pCloud.NET;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace pCloud
{
    public class StorageItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FolderTemplate { get; set; }

        public DataTemplate FileTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is Folder)
            {
                return this.FolderTemplate;
            }
            else
            {
                return this.FileTemplate;
            }
        }
    }
}
