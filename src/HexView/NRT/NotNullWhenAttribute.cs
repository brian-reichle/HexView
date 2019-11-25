#if NETFRAMEWORK

namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Parameter)]
	sealed class NotNullWhenAttribute : Attribute
	{
		public NotNullWhenAttribute(bool resultValue)
		{
			ResultValue = resultValue;
		}

		public bool ResultValue { get; }
	}
}

#endif
