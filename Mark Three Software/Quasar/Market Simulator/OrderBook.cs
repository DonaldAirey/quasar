﻿//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by a tool.
//     Runtime Version: 1.0.3705.288
//
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated>
//------------------------------------------------------------------------------

namespace Shadows.Quasar.Market {
    using System;
    using System.Data;
    using System.Xml;
    using System.Runtime.Serialization;
    
    
    [Serializable()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.ComponentModel.ToolboxItem(true)]
    public class OrderBook : DataSet {
        
        private OrderDataTable tableOrder;
        
        public OrderBook() {
            this.InitClass();
            System.ComponentModel.CollectionChangeEventHandler schemaChangedHandler = new System.ComponentModel.CollectionChangeEventHandler(this.SchemaChanged);
            this.Tables.CollectionChanged += schemaChangedHandler;
            this.Relations.CollectionChanged += schemaChangedHandler;
        }
        
        protected OrderBook(SerializationInfo info, StreamingContext context) {
            string strSchema = ((string)(info.GetValue("XmlSchema", typeof(string))));
            if ((strSchema != null)) {
                DataSet ds = new DataSet();
                ds.ReadXmlSchema(new XmlTextReader(new System.IO.StringReader(strSchema)));
                if ((ds.Tables["Order"] != null)) {
                    this.Tables.Add(new OrderDataTable(ds.Tables["Order"]));
                }
                this.DataSetName = ds.DataSetName;
                this.Prefix = ds.Prefix;
                this.Namespace = ds.Namespace;
                this.Locale = ds.Locale;
                this.CaseSensitive = ds.CaseSensitive;
                this.EnforceConstraints = ds.EnforceConstraints;
                this.Merge(ds, false, System.Data.MissingSchemaAction.Add);
                this.InitVars();
            }
            else {
                this.InitClass();
            }
            this.GetSerializationData(info, context);
            System.ComponentModel.CollectionChangeEventHandler schemaChangedHandler = new System.ComponentModel.CollectionChangeEventHandler(this.SchemaChanged);
            this.Tables.CollectionChanged += schemaChangedHandler;
            this.Relations.CollectionChanged += schemaChangedHandler;
        }
        
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibilityAttribute(System.ComponentModel.DesignerSerializationVisibility.Content)]
        public OrderDataTable Order {
            get {
                return this.tableOrder;
            }
        }
        
        public override DataSet Clone() {
            OrderBook cln = ((OrderBook)(base.Clone()));
            cln.InitVars();
            return cln;
        }
        
        protected override bool ShouldSerializeTables() {
            return false;
        }
        
        protected override bool ShouldSerializeRelations() {
            return false;
        }
        
        protected override void ReadXmlSerializable(XmlReader reader) {
            this.Reset();
            DataSet ds = new DataSet();
            ds.ReadXml(reader);
            if ((ds.Tables["Order"] != null)) {
                this.Tables.Add(new OrderDataTable(ds.Tables["Order"]));
            }
            this.DataSetName = ds.DataSetName;
            this.Prefix = ds.Prefix;
            this.Namespace = ds.Namespace;
            this.Locale = ds.Locale;
            this.CaseSensitive = ds.CaseSensitive;
            this.EnforceConstraints = ds.EnforceConstraints;
            this.Merge(ds, false, System.Data.MissingSchemaAction.Add);
            this.InitVars();
        }
        
        protected override System.Xml.Schema.XmlSchema GetSchemaSerializable() {
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            this.WriteXmlSchema(new XmlTextWriter(stream, null));
            stream.Position = 0;
            return System.Xml.Schema.XmlSchema.Read(new XmlTextReader(stream), null);
        }
        
        internal void InitVars() {
            this.tableOrder = ((OrderDataTable)(this.Tables["Order"]));
            if ((this.tableOrder != null)) {
                this.tableOrder.InitVars();
            }
        }
        
        private void InitClass() {
            this.DataSetName = "OrderBook";
            this.Prefix = "";
            this.Namespace = "http://tempuri.org/OrderBook.xsd";
            this.Locale = new System.Globalization.CultureInfo("en-US");
            this.CaseSensitive = false;
            this.EnforceConstraints = true;
            this.tableOrder = new OrderDataTable();
            this.Tables.Add(this.tableOrder);
        }
        
        private bool ShouldSerializeOrder() {
            return false;
        }
        
        private void SchemaChanged(object sender, System.ComponentModel.CollectionChangeEventArgs e) {
            if ((e.Action == System.ComponentModel.CollectionChangeAction.Remove)) {
                this.InitVars();
            }
        }
        
        public delegate void OrderRowChangeEventHandler(object sender, OrderRowChangeEvent e);
        
        [System.Diagnostics.DebuggerStepThrough()]
        public class OrderDataTable : DataTable, System.Collections.IEnumerable {
            
            private DataColumn columnBlockOrderId;
            
            private DataColumn columnBrokerId;
            
            private DataColumn columnSecurityId;
            
            private DataColumn columnQuantityOrdered;
            
            private DataColumn columnQuantityExecuted;
            
            private DataColumn columnTimeInForceCode;
            
            private DataColumn columnOrderTypeCode;
            
            private DataColumn columnPrice1;
            
            private DataColumn columnPrice2;
            
            internal OrderDataTable() : 
                    base("Order") {
                this.InitClass();
            }
            
            internal OrderDataTable(DataTable table) : 
                    base(table.TableName) {
                if ((table.CaseSensitive != table.DataSet.CaseSensitive)) {
                    this.CaseSensitive = table.CaseSensitive;
                }
                if ((table.Locale.ToString() != table.DataSet.Locale.ToString())) {
                    this.Locale = table.Locale;
                }
                if ((table.Namespace != table.DataSet.Namespace)) {
                    this.Namespace = table.Namespace;
                }
                this.Prefix = table.Prefix;
                this.MinimumCapacity = table.MinimumCapacity;
                this.DisplayExpression = table.DisplayExpression;
            }
            
            [System.ComponentModel.Browsable(false)]
            public int Count {
                get {
                    return this.Rows.Count;
                }
            }
            
            internal DataColumn BlockOrderIdColumn {
                get {
                    return this.columnBlockOrderId;
                }
            }
            
            internal DataColumn BrokerIdColumn {
                get {
                    return this.columnBrokerId;
                }
            }
            
            internal DataColumn SecurityIdColumn {
                get {
                    return this.columnSecurityId;
                }
            }
            
            internal DataColumn QuantityOrderedColumn {
                get {
                    return this.columnQuantityOrdered;
                }
            }
            
            internal DataColumn QuantityExecutedColumn {
                get {
                    return this.columnQuantityExecuted;
                }
            }
            
            internal DataColumn TimeInForceCodeColumn {
                get {
                    return this.columnTimeInForceCode;
                }
            }
            
            internal DataColumn OrderTypeCodeColumn {
                get {
                    return this.columnOrderTypeCode;
                }
            }
            
            internal DataColumn Price1Column {
                get {
                    return this.columnPrice1;
                }
            }
            
            internal DataColumn Price2Column {
                get {
                    return this.columnPrice2;
                }
            }
            
            public OrderRow this[int index] {
                get {
                    return ((OrderRow)(this.Rows[index]));
                }
            }
            
            public event OrderRowChangeEventHandler OrderRowChanged;
            
            public event OrderRowChangeEventHandler OrderRowChanging;
            
            public event OrderRowChangeEventHandler OrderRowDeleted;
            
            public event OrderRowChangeEventHandler OrderRowDeleting;
            
            public void AddOrderRow(OrderRow row) {
                this.Rows.Add(row);
            }
            
            public OrderRow AddOrderRow(int BlockOrderId, int BrokerId, int SecurityId, System.Decimal QuantityOrdered, System.Decimal QuantityExecuted, int TimeInForceCode, int OrderTypeCode, System.Decimal Price1, System.Decimal Price2) {
                OrderRow rowOrderRow = ((OrderRow)(this.NewRow()));
                rowOrderRow.ItemArray = new object[] {
                        BlockOrderId,
                        BrokerId,
                        SecurityId,
                        QuantityOrdered,
                        QuantityExecuted,
                        TimeInForceCode,
                        OrderTypeCode,
                        Price1,
                        Price2};
                this.Rows.Add(rowOrderRow);
                return rowOrderRow;
            }
            
            public System.Collections.IEnumerator GetEnumerator() {
                return this.Rows.GetEnumerator();
            }
            
            public override DataTable Clone() {
                OrderDataTable cln = ((OrderDataTable)(base.Clone()));
                cln.InitVars();
                return cln;
            }
            
            protected override DataTable CreateInstance() {
                return new OrderDataTable();
            }
            
            internal void InitVars() {
                this.columnBlockOrderId = this.Columns["BlockOrderId"];
                this.columnBrokerId = this.Columns["BrokerId"];
                this.columnSecurityId = this.Columns["SecurityId"];
                this.columnQuantityOrdered = this.Columns["QuantityOrdered"];
                this.columnQuantityExecuted = this.Columns["QuantityExecuted"];
                this.columnTimeInForceCode = this.Columns["TimeInForceCode"];
                this.columnOrderTypeCode = this.Columns["OrderTypeCode"];
                this.columnPrice1 = this.Columns["Price1"];
                this.columnPrice2 = this.Columns["Price2"];
            }
            
            private void InitClass() {
                this.columnBlockOrderId = new DataColumn("BlockOrderId", typeof(int), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnBlockOrderId);
                this.columnBrokerId = new DataColumn("BrokerId", typeof(int), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnBrokerId);
                this.columnSecurityId = new DataColumn("SecurityId", typeof(int), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnSecurityId);
                this.columnQuantityOrdered = new DataColumn("QuantityOrdered", typeof(System.Decimal), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnQuantityOrdered);
                this.columnQuantityExecuted = new DataColumn("QuantityExecuted", typeof(System.Decimal), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnQuantityExecuted);
                this.columnTimeInForceCode = new DataColumn("TimeInForceCode", typeof(int), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnTimeInForceCode);
                this.columnOrderTypeCode = new DataColumn("OrderTypeCode", typeof(int), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnOrderTypeCode);
                this.columnPrice1 = new DataColumn("Price1", typeof(System.Decimal), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnPrice1);
                this.columnPrice2 = new DataColumn("Price2", typeof(System.Decimal), null, System.Data.MappingType.Element);
                this.Columns.Add(this.columnPrice2);
                this.columnBlockOrderId.AllowDBNull = false;
                this.columnBrokerId.AllowDBNull = false;
                this.columnSecurityId.AllowDBNull = false;
                this.columnQuantityOrdered.AllowDBNull = false;
                this.columnQuantityExecuted.AllowDBNull = false;
                this.columnTimeInForceCode.AllowDBNull = false;
                this.columnOrderTypeCode.AllowDBNull = false;
            }
            
            public OrderRow NewOrderRow() {
                return ((OrderRow)(this.NewRow()));
            }
            
            protected override DataRow NewRowFromBuilder(DataRowBuilder builder) {
                return new OrderRow(builder);
            }
            
            protected override System.Type GetRowType() {
                return typeof(OrderRow);
            }
            
            protected override void OnRowChanged(DataRowChangeEventArgs e) {
                base.OnRowChanged(e);
                if ((this.OrderRowChanged != null)) {
                    this.OrderRowChanged(this, new OrderRowChangeEvent(((OrderRow)(e.Row)), e.Action));
                }
            }
            
            protected override void OnRowChanging(DataRowChangeEventArgs e) {
                base.OnRowChanging(e);
                if ((this.OrderRowChanging != null)) {
                    this.OrderRowChanging(this, new OrderRowChangeEvent(((OrderRow)(e.Row)), e.Action));
                }
            }
            
            protected override void OnRowDeleted(DataRowChangeEventArgs e) {
                base.OnRowDeleted(e);
                if ((this.OrderRowDeleted != null)) {
                    this.OrderRowDeleted(this, new OrderRowChangeEvent(((OrderRow)(e.Row)), e.Action));
                }
            }
            
            protected override void OnRowDeleting(DataRowChangeEventArgs e) {
                base.OnRowDeleting(e);
                if ((this.OrderRowDeleting != null)) {
                    this.OrderRowDeleting(this, new OrderRowChangeEvent(((OrderRow)(e.Row)), e.Action));
                }
            }
            
            public void RemoveOrderRow(OrderRow row) {
                this.Rows.Remove(row);
            }
        }
        
        [System.Diagnostics.DebuggerStepThrough()]
        public class OrderRow : DataRow {
            
            private OrderDataTable tableOrder;
            
            internal OrderRow(DataRowBuilder rb) : 
                    base(rb) {
                this.tableOrder = ((OrderDataTable)(this.Table));
            }
            
            public int BlockOrderId {
                get {
                    return ((int)(this[this.tableOrder.BlockOrderIdColumn]));
                }
                set {
                    this[this.tableOrder.BlockOrderIdColumn] = value;
                }
            }
            
            public int BrokerId {
                get {
                    return ((int)(this[this.tableOrder.BrokerIdColumn]));
                }
                set {
                    this[this.tableOrder.BrokerIdColumn] = value;
                }
            }
            
            public int SecurityId {
                get {
                    return ((int)(this[this.tableOrder.SecurityIdColumn]));
                }
                set {
                    this[this.tableOrder.SecurityIdColumn] = value;
                }
            }
            
            public System.Decimal QuantityOrdered {
                get {
                    return ((System.Decimal)(this[this.tableOrder.QuantityOrderedColumn]));
                }
                set {
                    this[this.tableOrder.QuantityOrderedColumn] = value;
                }
            }
            
            public System.Decimal QuantityExecuted {
                get {
                    return ((System.Decimal)(this[this.tableOrder.QuantityExecutedColumn]));
                }
                set {
                    this[this.tableOrder.QuantityExecutedColumn] = value;
                }
            }
            
            public int TimeInForceCode {
                get {
                    return ((int)(this[this.tableOrder.TimeInForceCodeColumn]));
                }
                set {
                    this[this.tableOrder.TimeInForceCodeColumn] = value;
                }
            }
            
            public int OrderTypeCode {
                get {
                    return ((int)(this[this.tableOrder.OrderTypeCodeColumn]));
                }
                set {
                    this[this.tableOrder.OrderTypeCodeColumn] = value;
                }
            }
            
            public System.Decimal Price1 {
                get {
                    try {
                        return ((System.Decimal)(this[this.tableOrder.Price1Column]));
                    }
                    catch (InvalidCastException e) {
                        throw new StrongTypingException("Cannot get value because it is DBNull.", e);
                    }
                }
                set {
                    this[this.tableOrder.Price1Column] = value;
                }
            }
            
            public System.Decimal Price2 {
                get {
                    try {
                        return ((System.Decimal)(this[this.tableOrder.Price2Column]));
                    }
                    catch (InvalidCastException e) {
                        throw new StrongTypingException("Cannot get value because it is DBNull.", e);
                    }
                }
                set {
                    this[this.tableOrder.Price2Column] = value;
                }
            }
            
            public bool IsPrice1Null() {
                return this.IsNull(this.tableOrder.Price1Column);
            }
            
            public void SetPrice1Null() {
                this[this.tableOrder.Price1Column] = System.Convert.DBNull;
            }
            
            public bool IsPrice2Null() {
                return this.IsNull(this.tableOrder.Price2Column);
            }
            
            public void SetPrice2Null() {
                this[this.tableOrder.Price2Column] = System.Convert.DBNull;
            }
        }
        
        [System.Diagnostics.DebuggerStepThrough()]
        public class OrderRowChangeEvent : EventArgs {
            
            private OrderRow eventRow;
            
            private DataRowAction eventAction;
            
            public OrderRowChangeEvent(OrderRow row, DataRowAction action) {
                this.eventRow = row;
                this.eventAction = action;
            }
            
            public OrderRow Row {
                get {
                    return this.eventRow;
                }
            }
            
            public DataRowAction Action {
                get {
                    return this.eventAction;
                }
            }
        }
    }
}
