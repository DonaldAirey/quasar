/*************************************************************************************************************************
*
*	File:			Position.cs
*	Description:	Used to identify a position, that is, a combination of a SecurityId and position (long, short).
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using System;

namespace MarkThree.Quasar
{

	/// <summary>
	/// Summary description for Position.
	/// </summary>
	public class Position
	{

		private int accountId;
		private int securityId;
		private int positionTypeCode;

		/// <summary>Account Identifier</summary>
		public int AccountId {get {return this.accountId;}}

		/// <summary>Security Identifier</summary>
		public int SecurityId {get {return this.securityId;}}

		/// <summary>Position Type Code (Long or Short)</summary>
		public int PositionTypeCode {get {return this.positionTypeCode;}}

		/// <summary>
		/// Create a Position record
		/// </summary>
		/// <param name="SecurityId">The security identifier.</param>
		/// <param name="PositionTypeCode">The PositionTypeCode.</param>
		public Position(int AccountId, int SecurityId, int PositionTypeCode)
		{

			// Initialize Members
			this.accountId = AccountId;
			this.securityId = SecurityId;
			this.positionTypeCode = PositionTypeCode;

		}

	}

}
