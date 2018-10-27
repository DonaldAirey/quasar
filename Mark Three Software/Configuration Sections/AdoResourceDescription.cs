namespace MarkThree
{

	using System;
	using System.Collections.Generic;
	using System.Text;

	public class AdoResourceDescription : ResourceDescription
	{

		public readonly Type Type;

		public AdoResourceDescription(string name, Type type) : base(name)
		{

			this.Type = type;

		}

	}

}
