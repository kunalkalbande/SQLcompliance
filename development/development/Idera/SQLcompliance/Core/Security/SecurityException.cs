using System;
using System.Runtime.Serialization;

namespace Idera.SQLcompliance.Core.Security {

	/// <summary>
	/// Base authentication exception class for SQLsecure.
	/// </summary>
	[Serializable]
	public class SqlComplianceSecurityException : Exception {

		public SqlComplianceSecurityException() {}

		public SqlComplianceSecurityException(string message)
			: base(message) {}

		public SqlComplianceSecurityException(string message, Exception innerException)
			:base(message, innerException) {}

		protected SqlComplianceSecurityException(SerializationInfo info, StreamingContext context)
			:base(info, context) {}

	}

	[Serializable]
	public class LocalSecurityException : SqlComplianceSecurityException {

		public LocalSecurityException() {}

		public LocalSecurityException(string message)
			: base(message) {}

		public LocalSecurityException(string message, Exception innerException)
			:base(message, innerException) {}

		protected LocalSecurityException(SerializationInfo info, StreamingContext context)
			:base(info, context) {}

	}

	[Serializable]
	public class RemoteSecurityException : SqlComplianceSecurityException {

		public RemoteSecurityException() {}

		public RemoteSecurityException(string message)
			: base(message) {}

		public RemoteSecurityException(string message, Exception innerException)
			:base(message, innerException) {}

		protected RemoteSecurityException(SerializationInfo info, StreamingContext context)
			:base(info, context) {}

	}

}