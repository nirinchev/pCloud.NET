using System;

namespace pCloud.NET
{
    public class pCloudException : Exception
    {
        public pCloudException(int errorCode, string errorMessage) : base(errorMessage)
        {

        }
    }
}
