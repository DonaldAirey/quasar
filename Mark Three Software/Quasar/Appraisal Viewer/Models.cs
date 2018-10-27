/*************************************************************************************************************************
*
*	File:			Models.cs
*	Description:	Methods for creating an maintaining ideal portfolio models.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

namespace Shadows.Quasar.Viewers.Appraisal
{

	using Shadows.Quasar.Client;
	using Shadows.Quasar.Common;
	using System;
	using System.Collections;
	using System.Data;
	using System.Diagnostics;
	using System.Threading;
	using System.Xml;
	using System.Xml.Xsl;
	using System.Xml.XPath;
	using System.Windows.Forms;

	/// <summary>
	/// A command batch used to create a model and pass the model id back to the caller.
	/// </summary>
	internal class ModelBatch : RemoteBatch
	{

		private RemoteParameter modelIdParameter;

		/// <summary>The parameter that contains the model identifier as a return parameter.</summary>
		public RemoteParameter ModelIdParameter {get {return this.modelIdParameter;} set {this.modelIdParameter = value;}}

		/// <summary>
		/// Constructs a command batch that is used to create a model.
		/// </summary>
		public ModelBatch()
		{

			// Initialize the object.
			this.modelIdParameter = null;

		}

	}
		
	/// <summary>
	/// A model is an abstract portfolio.  Sectors and securities are specified in percentage of market value terms.
	/// </summary>
	public class Models
	{

		/// <summary>
		/// Chooses or creates a model for the appraisal.
		/// </summary>
		/// <param name="accountId">The account used to select a model.</param>
		public static int SelectModel(int accountId)
		{

			// The logic in this method will determine if a temporary model is needed and built it.  If a temporary model is
			// required, it will be built using this command batch.  In all cases, the appropriate model for the given account will
			// be returned to the caller.  In some cases, a model will be constructed on the fly from the existing values in the
			// account.  These temporary models will use most of the position and trading tables
			ModelBatch modelBatch = null;
			
			try
			{

				// Lock the tables
				Debug.Assert(!ClientMarketData.AreLocksHeld);
				ClientMarketData.AccountLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.AllocationLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.CurrencyLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.DebtLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.EquityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ModelLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.SectorTargetLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.PositionTargetLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectTreeLock.AcquireWriterLock(CommonTimeout.LockWait);
				ClientMarketData.OrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ProposedOrderLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SchemeLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SectorLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TaxLotLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.TransactionTypeLock.AcquireReaderLock(CommonTimeout.LockWait);
			
				// Find the account record that is being opened.
				ClientMarketData.AccountRow accountRow = ClientMarketData.Account.FindByAccountId(accountId);
				if (accountRow == null)
					throw new Exception(String.Format("Account {0} has been deleted", accountId));

				// The objective is to find out whether a 'Self' model must be created from the existing positions, or whether a
				// an empty or a copy of a model is required to view an account appraisal.  The first test is to see whether any model
				// has been assigned to the account.
				if (accountRow.IsModelIdNull())
				{

					// This will create an empty position model for the appraisal.
					modelBatch = Models.CreateEmptyModel(accountRow);

				}
				else
				{

					// At this point, a model has been assigned to the account.  Get the model and find out if a temporary copy
					// needs to be made.
					ClientMarketData.ModelRow modelRow = ClientMarketData.Model.FindByModelId(accountRow.ModelId);
					if (modelRow == null)
						throw new Exception(String.Format("Model {0} has been deleted", accountRow.ModelId));
					
					// A 'self' model is one that requires a calculation of the current positions.
					if (!modelRow.SectorSelf && !modelRow.SecuritySelf)
					{

						// Currently, the existing model is used on an appraisal.  Any changes to the model in the appraisal view
						// will be stored in the persistent model.  It may be useful sometime in the future to make a copy of the
						// model and prompt the user to save it when the appraisal is closed.
						return modelRow.ModelId;

					}
					else
					{

						// Make sure that the account has been assigned a scheme before attempting to build a model from it.
						if (accountRow.IsSchemeIdNull())
							throw new Exception(String.Format("No scheme has been assigned to account {0}.", accountRow));
					
						// If the account has a default scheme, make sure it still exists.
						ClientMarketData.SchemeRow schemeRow = ClientMarketData.Scheme.FindBySchemeId(accountRow.SchemeId);
						if (schemeRow == null)
							throw new ArgumentException("This scheme has been deleted", accountRow.SchemeId.ToString());

						// Create a model based on the current sector totals.
						if (modelRow.SectorSelf)
							modelBatch = Models.CreateSectorSelfModel(accountRow, schemeRow);

						// Create a model based on the current position totals.
						if (modelRow.SecuritySelf)
							modelBatch = Models.CreatePositionSelfModel(accountRow, schemeRow);

					}

				}

			}
			finally
			{

				// Release the table locks.
				if (ClientMarketData.AccountLock.IsReaderLockHeld) ClientMarketData.AccountLock.ReleaseReaderLock();
				if (ClientMarketData.AllocationLock.IsReaderLockHeld) ClientMarketData.AllocationLock.ReleaseReaderLock();
				if (ClientMarketData.CurrencyLock.IsReaderLockHeld) ClientMarketData.CurrencyLock.ReleaseReaderLock();
				if (ClientMarketData.DebtLock.IsReaderLockHeld) ClientMarketData.DebtLock.ReleaseReaderLock();
				if (ClientMarketData.EquityLock.IsReaderLockHeld) ClientMarketData.EquityLock.ReleaseReaderLock();
				if (ClientMarketData.ModelLock.IsWriterLockHeld) ClientMarketData.ModelLock.ReleaseWriterLock();
				if (ClientMarketData.SectorTargetLock.IsWriterLockHeld) ClientMarketData.SectorTargetLock.ReleaseWriterLock();
				if (ClientMarketData.PositionTargetLock.IsWriterLockHeld) ClientMarketData.PositionTargetLock.ReleaseWriterLock();
				if (ClientMarketData.ObjectLock.IsWriterLockHeld) ClientMarketData.ObjectLock.ReleaseWriterLock();
				if (ClientMarketData.ObjectTreeLock.IsWriterLockHeld) ClientMarketData.ObjectTreeLock.ReleaseWriterLock();
				if (ClientMarketData.OrderLock.IsReaderLockHeld) ClientMarketData.OrderLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld) ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.ProposedOrderLock.IsReaderLockHeld) ClientMarketData.ProposedOrderLock.ReleaseReaderLock();
				if (ClientMarketData.SchemeLock.IsReaderLockHeld) ClientMarketData.SchemeLock.ReleaseReaderLock();
				if (ClientMarketData.SectorLock.IsReaderLockHeld) ClientMarketData.SectorLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld) ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.TaxLotLock.IsReaderLockHeld) ClientMarketData.TaxLotLock.ReleaseReaderLock();
				if (ClientMarketData.TransactionTypeLock.IsReaderLockHeld) ClientMarketData.TransactionTypeLock.ReleaseReaderLock();
				Debug.Assert(!ClientMarketData.AreLocksHeld);

			}

			// At this point, a batch is ready to be sent that will create the model and populate it with target values.  The data
			// structure is an overloaded version of the 'RemoteBatch' class.  The 'ModelBatch' contains a member which references
			// the 'modelId' return value from the creation of the model.  This value will be returned to the caller as a reference
			// to the temporary model.
			ClientMarketData.Send(modelBatch);

			// Rethrow a generic error message for the failed model.
			if (modelBatch.HasExceptions)
				throw new Exception("Can't create model.");

			// Return the model identifier generated by the server.
			return (int)modelBatch.ModelIdParameter.Value;
			
		}
		
		/// <summary>
		/// Creates a temporary, empty model portfolio.
		/// </summary>
		/// <param name="accountRow">An account record.</param>
		/// <returns>A batch of commands that will create the empty model.</returns>
		private static ModelBatch CreateEmptyModel(ClientMarketData.AccountRow accountRow)
		{

			// Create the batch and fill it in with the assembly and type needed for this function.
			ModelBatch modelBatch = new ModelBatch();
			RemoteAssembly remoteAssembly = modelBatch.Assemblies.Add("Service.Core");
			RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.Model");

			// This method will insert a generic, empty, security level model.
			RemoteMethod insertModel = remoteType.Methods.Add("Insert");
			insertModel.Parameters.Add("modelId", DataType.Int, Direction.ReturnValue);
			insertModel.Parameters.Add("rowVersion", DataType.Long, Direction.Output);
			insertModel.Parameters.Add("modelTypeCode", ModelType.Security);
			insertModel.Parameters.Add("name", "Untitled");
			insertModel.Parameters.Add("schemeId", accountRow.SchemeId);
			insertModel.Parameters.Add("algorithmId", Algorithm.SecurityRebalancer);
			insertModel.Parameters.Add("temporary", true);

			// Save the reference to the 'modelId' return parameter.
			modelBatch.ModelIdParameter = insertModel.Parameters["modelId"];

			// This batch will create an empty, position based model.
			return modelBatch;

		}
		
		/// <summary>
		/// Creates a temporary copy of a model portfolio.
		/// </summary>
		/// <param name="modelRow">The original model record.</param>
		/// <returns>A batch of commands that will create a copy of the original model.</returns>
		private static ModelBatch CopyModel(ClientMarketData.ModelRow modelRow)
		{

			// Create the batch and fill it in with the assembly and type needed for this function.
			ModelBatch modelBatch = new ModelBatch();
			RemoteAssembly remoteAssembly = modelBatch.Assemblies.Add("Service.Core");
			RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.Model");

			// This method will insert a copy of the original model's header.
			RemoteMethod insertModel = remoteType.Methods.Add("Insert");
			insertModel.Parameters.Add("modelId", DataType.Int, Direction.ReturnValue);
			insertModel.Parameters.Add("rowVersion", DataType.Long, Direction.Output);
			insertModel.Parameters.Add("objectTypeCode", modelRow.ObjectRow.ObjectTypeCode);
			insertModel.Parameters.Add("name", String.Format("Copy of {0}", modelRow.ObjectRow.Name));
			insertModel.Parameters.Add("schemeId", modelRow.SchemeId);
			insertModel.Parameters.Add("algorithmId", modelRow.AlgorithmId);
			insertModel.Parameters.Add("temporary", true);
			insertModel.Parameters.Add("description", modelRow.ObjectRow.Description);

			// For a sector model, copy each of the sector level targets into the destination model.
			if (modelRow.ModelTypeCode == ModelType.Sector)
			{

				// The object Type for this operation.
				RemoteType sectorTargetType = remoteAssembly.Types.Add("Shadows.WebService.Core.SectorTarget");

				// Add the position level target to the model.
				foreach (ClientMarketData.SectorTargetRow sectorTargetRow in modelRow.GetSectorTargetRows())
				{
					RemoteMethod insertSector = sectorTargetType.Methods.Add("Insert");
					insertSector.Parameters.Add("modelId", insertModel.Parameters["modelId"]);
					insertSector.Parameters.Add("sectorId", sectorTargetRow.SectorId);
					insertSector.Parameters.Add("percent", sectorTargetRow.Percent);
				}

			}

			// For a position model, copy each of the position level targets into the destination model.
			if (modelRow.ModelTypeCode == ModelType.Security)
			{

				// The object Type for this operation.
				RemoteType positionTargetType = remoteAssembly.Types.Add("Shadows.WebService.Core.PositionTarget");

				// Add the position level target to the model.
				foreach (ClientMarketData.PositionTargetRow positionTargetRow in modelRow.GetPositionTargetRows())
				{
					RemoteMethod insertSecurity = positionTargetType.Methods.Add("Insert");
					insertSecurity.Parameters.Add("modelId", insertModel.Parameters["modelId"]);
					insertSecurity.Parameters.Add("securityId", positionTargetRow.SecurityId);
					insertSecurity.Parameters.Add("positionTypeCode", positionTargetRow.PositionTypeCode);
					insertSecurity.Parameters.Add("percent", positionTargetRow.Percent);
				}

			}

			// Save the reference to the 'modelId' return parameter.
			modelBatch.ModelIdParameter = insertModel.Parameters["modelId"];

			// This batch will create a copy of the original model.
			return modelBatch;

		}
		
		/// <summary>
		/// Creates a temporary model based on the current sector level targets.
		/// </summary>
		/// <param name="accountRow">An account used as a basis for the targets.</param>
		/// <param name="schemeRow">The scheme used to select sector targets.</param>
		/// <returns>A batch of commands that will create a model containing the current sector weights of the account.</returns>
		private static ModelBatch CreateSectorSelfModel(ClientMarketData.AccountRow accountRow, ClientMarketData.SchemeRow schemeRow)
		{

			// This command batch will create a temporary model and populate it with the current position level percentages as the
			// target values.
			ModelBatch modelBatch = new ModelBatch();
			RemoteTransaction remoteTransaction = modelBatch.Transactions.Add();
			RemoteAssembly remoteAssembly = modelBatch.Assemblies.Add("Service.Core");
			RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.Model");

			// Create the temporary model.
			RemoteMethod insertModel = remoteType.Methods.Add("Insert");
			insertModel.Parameters.Add("modelId", DataType.Int, Direction.ReturnValue);
			insertModel.Parameters.Add("rowVersion", DataType.Long, Direction.Output);
			insertModel.Parameters.Add("modelTypeCode", ModelType.Sector);
			insertModel.Parameters.Add("name", "Untitled");
			insertModel.Parameters.Add("schemeId", schemeRow.SchemeId);
			insertModel.Parameters.Add("algorithmId", Algorithm.SectorMergeRebalancer);
			insertModel.Parameters.Add("temporary", true);

			// The 'Self Sector' uses the market value of all the account and sub-account.
			decimal accountMarketValue = MarketValue.Calculate(accountRow.CurrencyRow, accountRow,
				MarketValueFlags.EntirePosition | MarketValueFlags.IncludeChildAccounts);

			// No need to construct a model if the account market value is zero.
			if (accountMarketValue != 0.0M)
			{

				// Create a new outline for the model to follow.  This will collect the tax lots, proposed orders, orders
				// and allocations into industry classification sectors.
				Common.Appraisal appraisal = new Common.Appraisal(accountRow, schemeRow, true);

				// The object Type for this operation.
				RemoteType sectorTargetType = remoteAssembly.Types.Add("Shadows.WebService.Core.SectorTarget");

				// Now that we have an outline to follow, we are going to run through each of the sectors, calculate the market
				// value, and create an entry in the temporary model for that sector and it's current weight of the overall market
				// value.
				foreach (AppraisalSet.SchemeRow driverScheme in appraisal.Scheme)
					foreach (AppraisalSet.ObjectTreeRow driverTree in
						driverScheme.ObjectRow.GetObjectTreeRowsByFKObjectObjectTreeParentId())
						foreach (AppraisalSet.SectorRow driverSector in
							driverTree.ObjectRowByFKObjectObjectTreeChildId.GetSectorRows())
						{

							// This sector is the destination for the market value calculation.
							ClientMarketData.SectorRow sectorRow = ClientMarketData.Sector.FindBySectorId(driverSector.SectorId);
						
							// Calculate the market value of all the securities held by all the accounts in the current sector.
							decimal sectorMarketValue = MarketValue.Calculate(accountRow.CurrencyRow, accountRow, sectorRow,
								MarketValueFlags.EntirePosition | MarketValueFlags.IncludeChildAccounts);

							// Add the position level target to the model.
							RemoteMethod insertSector = sectorTargetType.Methods.Add("Insert");
							insertSector.Parameters.Add("modelId", insertModel.Parameters["modelId"]);
							insertSector.Parameters.Add("sectorId", sectorRow.SectorId);
							insertSector.Parameters.Add("percent", sectorMarketValue / accountMarketValue);

						}

			}

			// Save the reference to the 'modelId' return parameter.
			modelBatch.ModelIdParameter = insertModel.Parameters["modelId"];

			// This batch will create a temporary model based on the sector totals of the original account.
			return modelBatch;

		}

		/// <summary>
		/// Creates a temporary model based on the current position level targets.
		/// </summary>
		/// <param name="accountRow">An account used as a basis for the targets.</param>
		/// <param name="schemeRow">The scheme used to select sector targets.</param>
		/// <returns>A batch of commands that will create a model containing the current position weights of the account.</returns>
		private static ModelBatch CreatePositionSelfModel(ClientMarketData.AccountRow accountRow, ClientMarketData.SchemeRow schemeRow)
		{

			// Create the batch and fill it in with the assembly and type needed for this function.
			ModelBatch modelBatch = new ModelBatch();
			RemoteTransaction remoteTransaction = modelBatch.Transactions.Add();
			RemoteAssembly remoteAssembly = modelBatch.Assemblies.Add("Service.Core");
			RemoteType remoteType = remoteAssembly.Types.Add("Shadows.WebService.Core.Model");

			// Create the temporary, position model based on the scheme used by the original account.
			RemoteMethod insertModel = remoteType.Methods.Add("Insert");
			insertModel.Parameters.Add("modelId", DataType.Int, Direction.ReturnValue);
			insertModel.Parameters.Add("rowVersion", DataType.Long, Direction.Output);
			insertModel.Parameters.Add("modelTypeCode", ModelType.Security);
			insertModel.Parameters.Add("name", "Untitled");
			insertModel.Parameters.Add("schemeId", schemeRow.SchemeId);
			insertModel.Parameters.Add("algorithmId", Algorithm.SecurityRebalancer);
			insertModel.Parameters.Add("temporary", true);

			// The 'Self Security' model uses the market value of all the positions, regardless of account or sub-account, when
			// calculating the denominator for the percentages.
			decimal accountMarketValue = MarketValue.Calculate(accountRow.CurrencyRow, accountRow,
				MarketValueFlags.EntirePosition | MarketValueFlags.IncludeChildAccounts);

			// If the account market value is zero, we can't do much more to create a model.
			if (accountMarketValue != 0.0M)
			{

				// Create a new outline for the model to follow.  This will collect the tax lots, proposed orders orders and
				// allocations into positions that can be used for calculating percentages.
				Common.Appraisal appraisal = new Common.Appraisal(accountRow, true);

				// Run through each of the positions, starting with the security.
				foreach (AppraisalSet.SecurityRow driverSecurity in appraisal.Security)
				{

					// This is a position is the destination for the market value calculation.
					ClientMarketData.SecurityRow securityRow =
						ClientMarketData.Security.FindBySecurityId(driverSecurity.SecurityId);

					// The object Type for this operation.
					RemoteType positionTargetType = remoteAssembly.Types.Add("Shadows.WebService.Core.PositionTarget");

					// Run through each of the positions in the appraisal calculating the market value of each position. The ratio
					// of this market value to the account's market value is the model percentage.
					foreach (AppraisalSet.PositionRow positionRow in driverSecurity.GetPositionRows())
					{

						// Calculate the market value of the given position.
						decimal securityMarketValue = MarketValue.Calculate(accountRow.CurrencyRow, accountRow,
							securityRow, positionRow.PositionTypeCode,
							MarketValueFlags.EntirePosition | MarketValueFlags.EntirePosition);

						// Add the position level target to the model.
						RemoteMethod insertPosition = positionTargetType.Methods.Add("Insert");
						insertPosition.Parameters.Add("modelId", insertModel.Parameters["modelId"]);
						insertPosition.Parameters.Add("securityId", securityRow.SecurityId);
						insertPosition.Parameters.Add("positionTypeCode", positionRow.PositionTypeCode);
						insertPosition.Parameters.Add("percent", securityMarketValue / accountMarketValue);

					}

				}

			}

			// Save the reference to the 'modelId' return parameter.
			modelBatch.ModelIdParameter = insertModel.Parameters["modelId"];

			// This batch will create a temporary model based on the position totals of the original account.
			return modelBatch;

		}
	
	}

}
