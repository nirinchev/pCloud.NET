using System;
using System.Linq;
using System.Runtime.Serialization;

namespace pCloud.NET.Dtos
{
	[DataContract]
	public class DigestResponse
	{
		[DataMember(Name = "digest")]
		public string Digest { get; set; }
	}
}