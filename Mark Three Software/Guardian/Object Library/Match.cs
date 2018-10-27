namespace MarkThree.Guardian
{

	using MarkThree.Guardian.Client;
	using System;
	using System.Drawing;

	/// <summary>
	/// Summary description for Match.
	/// </summary>
	public class Match
	{

		private int matchId;

		public Match(int matchId)
		{

			this.matchId = matchId;

		}

		/// <summary>The Primary Identifier of this object.</summary>
		public int MatchId {get {return this.matchId;}}

	}

}
