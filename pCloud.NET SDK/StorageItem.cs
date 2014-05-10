using System;
using System.Runtime.Serialization;

namespace pCloud.NET
{
    public abstract class StorageItem
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "parentfolderid")]
        public long ParentFolderId { get; set; }

        [DataMember(Name = "path")]
        public string Path { get; set; }

        [DataMember(Name = "created")]
        public DateTime Created { get; set; }

        [DataMember(Name = "modified")]
        public DateTime Modified { get; set; }

        internal StorageItem()
        {

        }
    }
}
