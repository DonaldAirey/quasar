namespace MarkThree
{

	using System;
	using System.Data;

	/// <summary>
	/// Handles filters that personalize the data sent from the server to the client.
	/// </summary>
	/// <param name="userDataRow">Information about the current user.</param>
	/// <param name="dataRow">The row that is to be tested for suitablilty in the client data model.</param>
	/// <returns>true indicates the row can be passed to the client, false indicates it should not.</returns>
	public delegate bool RowFilterDelegate(DataRow userDataRow, DataRow dataRow);
    
}
