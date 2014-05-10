using System.Runtime.Serialization;

namespace pCloud.NET
{
    public class Folder : StorageItem
    {
        [DataMember(Name = "folderid")]
        public long FolderId { get; set; }
    }
}
