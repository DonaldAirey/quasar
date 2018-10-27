namespace MarkThree.Guardian.Forms
{

	using System;
	using MarkThree.Guardian;

	/// <summary>
	/// Summary description for BlotterDetail.
	/// </summary>
	public struct BlotterMatchDetail
	{

		public Blotter Blotter;
		public Match[] Matches;

		public BlotterMatchDetail(Blotter blotter, Match[] matches)
		{

			// Initialize the object
			this.Blotter = blotter;
			this.Matches = matches;

		}

	}

}
