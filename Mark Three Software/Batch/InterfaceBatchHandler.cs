namespace MarkThree
{

	/// <summary>
	/// Interface for something that can execute a batch.
	/// </summary>
	public interface IBatchHandler
	{

		Result Execute(Batch batch);

	}

}
