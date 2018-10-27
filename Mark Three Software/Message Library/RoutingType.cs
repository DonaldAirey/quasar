using System;

namespace MarkThree
{

	/// <summary>
	/// FIX RoutingType.
	/// </summary>
	[Serializable()]
	public enum RoutingType
	{
		TargetFirm = 1,
		TargetList = 2,
		BlockFirm = 3,
		BlockList = 4
	}

}
