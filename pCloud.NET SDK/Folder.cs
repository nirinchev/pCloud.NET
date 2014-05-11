using System.Collections.Generic;
using System.Runtime.Serialization;

namespace pCloud.NET
{
    public class Folder : StorageItem
    {
        [DataMember(Name = "folderid")]
        public long FolderId { get; set; }
    }

    public class ListedFolder : Folder
    {
        [IgnoreDataMember]
        public IEnumerable<StorageItem> Contents { get; set; }
    }
}
