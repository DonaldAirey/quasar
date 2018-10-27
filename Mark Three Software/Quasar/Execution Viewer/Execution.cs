/*************************************************************************************************************************
*
*	File:			Execution.cs
*	Description:	An execution record.
*					Donald Roy Airey  Copyright ©  2002 - All Rights Reserved
*
*************************************************************************************************************************/

using Shadows.Quasar.Common;
using Shadows.Quasar.Client;
using System;
using System.Data;

namespace Shadows.Quasar.Viewers.Execution
{

	/// <summary>
	/// An execution of a trade.
	/// </summary>
	public class Execution
	{

		// Private Members
		protected bool isLocal;
		protected bool iRowVersionModified;
		protected bool isBlockOrderIdModified;
		protected bool isTransactionTypeCodeModified;
		protected bool isBrokerIdModified;
		protected bool isBrokerNameModified;
		protected bool isBrokerSymbolModified;
		protected bool isExecutionIdModified;
		protected bool isQuantityModified;
		protected bool isPriceModified;
		protected bool isCommissionModified;
		protected bool isAccruedInterestModified;
		protected bool isUserFee0Modified;
		protected bool isUserFee1Modified;
		protected bool isUserFee2Modified;
		protected bool isUserFee3Modified;
		protected bool isTradeDateModified;
		protected bool isSettlementDateModified;
		protected bool isCreatedTimeModified;
		protected bool isCreatedLoginIdModified;
		protected bool isCreatedLoginNameModified;
		protected bool isModifiedTimeModified;
		protected bool isModifiedLoginIdModified;
		protected bool isModifiedLoginNameModified;
		protected bool isCommissionCalculated;
		protected bool isAccruedInterestCalculated;
		protected bool isUserFee0Calculated;
		protected bool isUserFee1Calculated;
		protected bool isUserFee2Calculated;
		protected bool isUserFee3Calculated;
		protected object rowVersion;
		protected object blockOrderId;
		protected object transactionTypeCode;
		protected object brokerId;
		protected object brokerName;
		protected object brokerSymbol;
		protected object executionId;
		protected object quantity;
		protected object price;
		protected object commission;
		protected object accruedInterest;
		protected object userFee0;
		protected object userFee1;
		protected object userFee2;
		protected object userFee3;
		protected object tradeDate;
		protected object settlementDate;
		protected object createdTime;
		protected object createdLoginId;
		protected object createdLoginName;
		protected object modifiedTime;
		protected object modifiedLoginId;
		protected object modifiedLoginName;

		// Public Properties
		public bool IsLocal {get {return this.isLocal;}}
		public long RowVersion {get {return (long)this.rowVersion;} set {this.rowVersion = value; this.iRowVersionModified = true;}}
		public int BlockOrderId {get {return (int)this.blockOrderId;} set {this.blockOrderId = value; this.isBlockOrderIdModified = true;}}
		public int TransactionTypeCode {get {return (int)this.transactionTypeCode;} set {this.transactionTypeCode = value; this.isTransactionTypeCodeModified = true;}}
		public string BrokerName {get {return (string)this.brokerName;} set {this.brokerName = value; this.isBrokerNameModified = true;}}
		public string BrokerSymbol {get {return (string)this.brokerSymbol;} set {this.brokerSymbol = value; this.isBrokerSymbolModified = true;}}
		public int ExecutionId {get {return (int)this.executionId;} set {this.executionId = value; this.isExecutionIdModified = true;}}
		public decimal Quantity {get {return (decimal)this.quantity;} set {this.quantity = value; this.isQuantityModified = true;}}
		public decimal Price {get {return (decimal)this.price;} set {this.price = value; this.isPriceModified = true;}}
		public decimal Commission {get {return (decimal)this.commission;} set {this.commission = value; this.isCommissionModified = true;}}
		public decimal AccruedInterest {get {return (decimal)this.accruedInterest;} set {this.accruedInterest = value; this.isAccruedInterestModified = true;}}
		public decimal UserFee0 {get {return (decimal)this.userFee0;} set {this.userFee0 = value; this.isUserFee0Modified = true;}}
		public decimal UserFee1 {get {return (decimal)this.userFee1;} set {this.userFee1 = value; this.isUserFee1Modified = true;}}
		public decimal UserFee2 {get {return (decimal)this.userFee2;} set {this.userFee2 = value; this.isUserFee2Modified = true;}}
		public decimal UserFee3 {get {return (decimal)this.userFee3;} set {this.userFee3 = value; this.isUserFee3Modified = true;}}
		public DateTime TradeDate {get {return (DateTime)this.tradeDate;} set {this.tradeDate = value; this.isTradeDateModified = true;}}
		public DateTime SettlementDate {get {return (DateTime)this.settlementDate;} set {this.settlementDate = value; this.isSettlementDateModified = true;}}
		public DateTime CreatedTime {get {return (DateTime)this.createdTime;} set {this.createdTime = value; this.isCreatedTimeModified = true;}}
		public int CreatedLoginId {get {return (int)this.createdLoginId;} set {this.createdLoginId = value; this.isCreatedLoginIdModified = true;}}
		public int CreatedLoginName {get {return (int)this.createdLoginName;} set {this.createdLoginName = value; this.isCreatedLoginNameModified = true;}}
		public DateTime ModifiedTime {get {return (DateTime)this.modifiedTime;} set {this.modifiedTime = value; this.isModifiedTimeModified = true;}}
		public int ModifiedLoginId {get {return (int)this.modifiedLoginId;} set {this.modifiedLoginId = value; this.isModifiedLoginIdModified = true;}}
		public int ModifiedLoginName {get {return (int)this.modifiedLoginName;} set {this.modifiedLoginName = value; this.isModifiedLoginNameModified = true;}}

		// IsNull Indicators		
		public bool IRowVersionNull() {return this.rowVersion == null;}
		public bool IsBlockOrderIdNull() {return this.blockOrderId == null;}
		public bool IsTransactionTypeCodeNull() {return this.transactionTypeCode == null;}
		public bool IsBrokerIdNull() {return this.brokerId == null;}
		public bool IsBrokerNameNull() {return this.brokerName == null;}
		public bool IsBrokerSymbolNull() {return this.brokerSymbol == null;}
		public bool IsExecutionIdNull() {return this.executionId == null;}
		public bool IsQuantityNull() {return this.quantity == null;}
		public bool IsPriceNull() {return this.price == null;}
		public bool IsCommissionNull() {return this.commission == null;}
		public bool IsAccruedInterestNull() {return this.accruedInterest == null;}
		public bool IsUserFee0Null() {return this.userFee0 == null;}
		public bool IsUserFee1Null() {return this.userFee1 == null;}
		public bool IsUserFee2Null() {return this.userFee2 == null;}
		public bool IsUserFee3Null() {return this.userFee3 == null;}
		public bool IsTradeDateNull() {return this.tradeDate == null;}
		public bool IsSettlementDateNull() {return this.settlementDate == null;}
		public bool IsCreatedTimeNull() {return this.createdTime == null;}
		public bool IsCreatedLoginIdNull() {return this.createdLoginId == null;}
		public bool IsCreatedLoginNameNull() {return this.createdLoginName == null;}
		public bool IsModifiedTimeNull() {return this.modifiedTime == null;}
		public bool IsModifiedLoginIdNull() {return this.modifiedLoginId == null;}
		public bool IsModifiedLoginNameNull() {return this.modifiedLoginName == null;}

		// IsModified Indicators
		public bool IRowVersionModified() {return this.iRowVersionModified;}
		public bool IsBlockOrderIdModified() {return this.isBlockOrderIdModified;}
		public bool IsTransactionTypeCodeModified() {return this.isTransactionTypeCodeModified;}
		public bool IsBrokerIdModified() {return this.isBrokerIdModified;}
		public bool IsBrokerNameModified() {return this.isBrokerNameModified;}
		public bool IsBrokerSymbolModified() {return this.isBrokerSymbolModified;}
		public bool IsExecutionIdModified() {return this.isExecutionIdModified;}
		public bool IsQuantityModified() {return this.isQuantityModified;}
		public bool IsPriceModified() {return this.isPriceModified;}
		public bool IsCommissionModified() {return this.isCommissionModified;}
		public bool IsAccruedInterestModified() {return this.isAccruedInterestModified;}
		public bool IsUserFee0Modified() {return this.isUserFee0Modified;}
		public bool IsUserFee1Modified() {return this.isUserFee1Modified;}
		public bool IsUserFee2Modified() {return this.isUserFee2Modified;}
		public bool IsUserFee3Modified() {return this.isUserFee3Modified;}
		public bool IsTradeDateModified() {return this.isTradeDateModified;}
		public bool IsSettlementDateModified() {return this.isSettlementDateModified;}
		public bool IsCreatedTimeModified() {return this.isCreatedTimeModified;}
		public bool IsCreatedLoginIdModified() {return this.isCreatedLoginIdModified;}
		public bool IsCreatedLoginNameModified() {return this.isCreatedLoginNameModified;}
		public bool IsModifiedTimeModified() {return this.isModifiedTimeModified;}
		public bool IsModifiedLoginIdModified() {return this.isModifiedLoginIdModified;}
		public bool IsModifiedLoginNameModified() {return this.isModifiedLoginNameModified;}
		public bool IsCommissionCalculated() {return this.isCommissionCalculated;}
		public bool IsAccruedInterestCalculated() {return this.isAccruedInterestCalculated;}

		// IsCalcated Indicators
		public bool IsUserFee0Calculated() {return this.isUserFee0Calculated;}
		public bool IsUserFee1Calculated() {return this.isUserFee1Calculated;}
		public bool IsUserFee2Calculated() {return this.isUserFee2Calculated;}
		public bool IsUserFee3Calculated() {return this.isUserFee3Calculated;}

		/// <summary>
		/// Creates an Execution Object.
		/// </summary>
		public Execution()
		{

			// Initialize members
			this.isLocal = false;
			this.rowVersion = null;
			this.blockOrderId = null;
			this.brokerId = null;
			this.executionId = null;
			this.quantity = null;
			this.price = null;
			this.commission = null;
			this.accruedInterest = null;
			this.userFee0 = null;
			this.userFee1 = null;
			this.userFee2 = null;
			this.userFee3 = null;
			this.tradeDate = null;
			this.settlementDate = null;
			this.createdTime = null;
			this.createdLoginId = null;
			this.modifiedTime = null;
			this.modifiedLoginId = null;

			// Initialize Field Status
			iRowVersionModified = false;
			isBlockOrderIdModified = false;
			isTransactionTypeCodeModified = false;
			isBrokerIdModified = false;
			isBrokerNameModified = false;
			isBrokerSymbolModified = false;
			isExecutionIdModified = false;
			isQuantityModified = false;
			isPriceModified = false;
			isCommissionModified = false;
			isAccruedInterestModified = false;
			isUserFee0Modified = false;
			isUserFee1Modified = false;
			isUserFee2Modified = false;
			isUserFee3Modified = false;
			isTradeDateModified = false;
			isSettlementDateModified = false;
			isCreatedTimeModified = false;
			isCreatedLoginIdModified = false;
			isCreatedLoginNameModified = false;
			isModifiedTimeModified = false;
			isModifiedLoginIdModified = false;
			isModifiedLoginNameModified = false;

			// Initialize Calculation status
			this.isCommissionCalculated = true;
			this.isAccruedInterestCalculated = true;
			this.isUserFee0Calculated = true;
			this.isUserFee1Calculated = true;
			this.isUserFee2Calculated = true;
			this.isUserFee3Calculated = true;

		}

		public int BrokerId
		{
			
			get {return (int)this.brokerId;}
			
			set
			{
				
				this.brokerId = value;
				this.isBrokerIdModified = true;
			
				ClientMarketData.BrokerRow brokerRow = ClientMarketData.Broker.FindByBrokerId(value);
				if (brokerRow != null)
				{

					this.brokerSymbol = brokerRow.Symbol;
					this.isBrokerSymbolModified = true;

					this.brokerName = brokerRow.ObjectRow.Name;
					this.isBrokerNameModified = true;

				}

			}
		
		}

	}

	public class LocalExecution : Execution
	{

		/// <summary>
		/// Creates an Execution Object.
		/// </summary>
		public LocalExecution(int executionId)
		{

			// Initialize members
			this.isLocal = true;
			this.executionId = executionId;

		}

		public LocalExecution(ExecutionSet.ExecutionRow executionRow)
		{

			this.isLocal = true;

			// Initialize the members.
			this.rowVersion = executionRow.RowVersion;
			this.blockOrderId = executionRow.BlockOrderId;
			if (!executionRow.IsBrokerIdNull())
			{

				this.brokerId = executionRow.BrokerId;

				ClientMarketData.BrokerRow brokerRow = ClientMarketData.Broker.FindByBrokerId(executionRow.BrokerId);
				if (brokerRow != null)
				{

					this.brokerSymbol = brokerRow.Symbol;
					this.brokerName = brokerRow.ObjectRow.Name;

				}

			}

			this.executionId = executionRow.ExecutionId;
			this.quantity = executionRow.Quantity;
			this.price = executionRow.Price;
			this.commission = executionRow.Commission;
			this.accruedInterest = executionRow.AccruedInterest;
			this.userFee0 = executionRow.UserFee0;
			this.userFee1 = executionRow.UserFee1;
			this.userFee2 = executionRow.UserFee2;
			this.userFee3 = executionRow.UserFee3;
			if (!executionRow.IsTradeDateNull())
				this.tradeDate = executionRow.TradeDate;
			if (!executionRow.IsSettlementDateNull())
				this.settlementDate = executionRow.SettlementDate;

		}

	}
	
	public class GlobalExecution : Execution
	{

		/// <summary>
		/// Creates an Execution Object.
		/// </summary>
		public GlobalExecution(int executionId)
		{

			// Initialize members
			this.isLocal = false;
			this.executionId = executionId;

		}

		public GlobalExecution(ClientMarketData.ExecutionRow executionRow)
		{

			// Initialize members
			this.isLocal = false;
			this.blockOrderId = executionRow.BlockOrderId;
			this.brokerId = executionRow.BrokerId;
			this.transactionTypeCode = executionRow.BlockOrderRow.TransactionTypeCode;
			this.brokerName = executionRow.BrokerRow.ObjectRow.Name;
			this.brokerSymbol = executionRow.BrokerRow.Symbol;
			this.rowVersion = executionRow.RowVersion;
			this.executionId = executionRow.ExecutionId;
			this.quantity = executionRow.Quantity;
			this.price = executionRow.Price;
			this.commission = executionRow.Commission;
			this.accruedInterest = executionRow.AccruedInterest;
			this.userFee0 = executionRow.UserFee0;
			this.userFee1 = executionRow.UserFee1;
			this.userFee2 = executionRow.UserFee2;
			this.userFee3 = executionRow.UserFee3;
			this.tradeDate = executionRow.TradeDate;
			this.settlementDate = executionRow.SettlementDate;
			this.createdTime = executionRow.CreatedTime;
			this.createdLoginId = executionRow.CreatedLoginId;
			ClientMarketData.LoginRow createdLogin = ClientMarketData.Login.FindByLoginId(this.CreatedLoginId);
			if (createdLogin != null)
				this.createdLoginName = createdLogin.ObjectRow.Name;
			this.modifiedTime = executionRow.ModifiedTime;
			this.modifiedLoginId = executionRow.ModifiedLoginId;
			ClientMarketData.LoginRow modifiedLogin = ClientMarketData.Login.FindByLoginId(this.ModifiedLoginId);
			if (modifiedLogin != null)
				this.modifiedLoginName = modifiedLogin.ObjectRow.Name;

			// Initialize Field Status
			iRowVersionModified = true;
			isBlockOrderIdModified = true;
			isTransactionTypeCodeModified = true;
			isBrokerIdModified = true;
			isBrokerNameModified = true;
			isBrokerSymbolModified = true;
			isExecutionIdModified = true;
			isQuantityModified = true;
			isPriceModified = true;
			isCommissionModified = true;
			isAccruedInterestModified = true;
			isUserFee0Modified = true;
			isUserFee1Modified = true;
			isUserFee2Modified = true;
			isUserFee3Modified = true;
			isTradeDateModified = true;
			isSettlementDateModified = true;
			isCreatedTimeModified = true;
			isCreatedLoginIdModified = true;
			isCreatedLoginNameModified = true;
			isModifiedTimeModified = true;
			isModifiedLoginIdModified = true;
			isModifiedLoginNameModified = true;

		}

	}

}