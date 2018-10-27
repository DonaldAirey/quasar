using System;
using System.Collections;

namespace Shadows.Quasar.Rule
{

	public class SecurityList : ArrayList
	{

		public new int Add(object value)
		{
			throw new Exception("Value is not compatible with SecurityList.");
		}
		
		public int Add(Security security)
		{

			return base.Add(security);

		}

		public bool Contains(Security security)
		{

			return this.Contains(security);

		}

	}
	
}
