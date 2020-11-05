using System.Collections.Generic;

namespace AndASM_NMS.Util
{
	public static class Shims
	{
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
		{
			return new HashSet<T>(source);
		}
	}
}