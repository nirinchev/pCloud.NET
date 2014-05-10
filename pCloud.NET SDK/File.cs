using System.Runtime.Serialization;

namespace pCloud.NET
{
    public class File : StorageItem
    {
        [DataMember(Name = "fileid")]
        public long FileId { get; set; }

        [DataMember(Name = "size")]
        public long Size { get; set; }

        [DataMember(Name = "hash")]
        public string Hash { get; set; }

        [DataMember(Name = "contenttype")]
        public string ContentType { get; set; }
    }
}
