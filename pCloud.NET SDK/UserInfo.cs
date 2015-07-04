using System;
using System.Runtime.Serialization;

namespace pCloud.NET
{
    public sealed class UserInfo
    {
        [DataMember(Name = "userid")]
        public string UserId { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "emailverified")]
        public bool EmailVerified { get; set; }

        [DataMember(Name = "registered")]
        public DateTime Registered { get; set; }

        [DataMember(Name = "language")]
        public string Language { get; set; }

        [DataMember(Name = "premium")]
        public bool Premium { get; set; }

        [DataMember(Name = "usedquota")]
        public long UsedQuota { get; set; }

        [DataMember(Name = "quota")]
        public long Quota { get; set; }
    }
}
