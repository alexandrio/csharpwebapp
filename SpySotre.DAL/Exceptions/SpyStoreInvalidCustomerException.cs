using System;
namespace SpyStore.DAL.Exceptions
{
	public class SpyStoreInvalidCustomerException:SpyStoreException
	{
		public SpyStoreInvalidCustomerException()
		{
		}
		public SpyStoreInvalidCustomerException(string message) : base(message) { }
		public SpyStoreInvalidCustomerException(string message, Exception innerException) : base(message, innerException) { }
	}
}

