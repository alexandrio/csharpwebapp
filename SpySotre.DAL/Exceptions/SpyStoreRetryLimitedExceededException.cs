using System;
namespace SpyStore.DAL.Exceptions
{
	public class SpyStoreRetryLimitedExceededException : SpyStoreException
	{
		public SpyStoreRetryLimitedExceededException()
		{
		}
		public SpyStoreRetryLimitedExceededException(string message) : base(message) { }
		public SpyStoreRetryLimitedExceededException(string message, Exception innerException) { }
	}
}

