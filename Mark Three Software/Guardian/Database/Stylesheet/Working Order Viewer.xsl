<?xml version="1.0" encoding="utf-8" standalone="yes"?>
<stylesheet version="1.0" xmlns="http://www.w3.org/1999/XSL/Transform"
	xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
	xmlns:x="urn:schemas-microsoft-com:office:excel"
	xmlns:c="urn:schemas-microsoft-com:office:component:spreadsheet"
	xmlns:mts="urn:schemas-markthreesoftware-com:stylesheet">
	
	<!-- Version 1.0 -->

	<!-- These parameters are used by the loader to specify the attributes of the stylesheet. -->
 	<mts:stylesheetId>WORKING ORDER VIEWER</mts:stylesheetId>
	<mts:stylesheetTypeCode>WORKING ORDER</mts:stylesheetTypeCode>
	<mts:name>Working Order Viewer</mts:name>  

	<template match="Document">

			<ss:Workbook xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
				xmlns:x="urn:schemas-microsoft-com:office:excel"
				xmlns:c="urn:schemas-microsoft-com:office:component:spreadsheet"
				xmlns:mts="urn:schemas-markthreesoftware-com:stylesheet">

				<ss:Styles>
					<ss:Style ss:ID="Default">
						<mts:Display>
							<ss:Borders>
								<ss:Border ss:Position="Bottom" ss:Weight="1" ss:Color="#C0C0C0"/>
							</ss:Borders>
							<ss:Protection ss:Protected="1"/>
						</mts:Display>
						<mts:Printer>
							<ss:Font ss:FontName="Courier New" ss:Size="7" />
						</mts:Printer>
					</ss:Style>
					<ss:Style ss:ID="CenterText" ss:Parent="Default">
						<ss:Alignment ss:Horizontal="Center" />
					</ss:Style>
					<ss:Style ss:ID="TopCenterText" ss:Parent="Default">
						<ss:Alignment ss:Vertical="Top" ss:Horizontal="Center" />
					</ss:Style>
					<ss:Style ss:ID="CenterCenterText" ss:Parent="Default">
						<ss:Alignment ss:Vertical="Center" ss:Horizontal="Center" />
					</ss:Style>
					<ss:Style ss:ID="LeftText" ss:Parent="Default">
						<ss:Alignment />
					</ss:Style>
					<ss:Style ss:ID="TopLeftText" ss:Parent="Default">
						<ss:Alignment ss:Vertical="Top" />
					</ss:Style>
					<ss:Style ss:ID="RightText" ss:Parent="Default">
						<ss:Alignment ss:Horizontal="Right" />
					</ss:Style>
					<ss:Style ss:ID="TopRightText" ss:Parent="Default">
						<ss:Alignment ss:Vertical="Top" ss:Horizontal="Right" />
					</ss:Style>
					<ss:Style ss:ID="EditText" ss:Parent="Default">
						<mts:Display>
							<ss:Protection ss:Protected="0" />
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="Price" ss:Parent="RightText">
						<ss:NumberFormat ss:Format="#,##0.00;-#,##0.00;"/>
						<mts:Display>
							<ss:Font>
								<mts:Animation mts:Direction="Nil" mts:StartColor="#0000FF" />
								<mts:Animation mts:Direction="Up" mts:StartColor="#00FF00" />
								<mts:Animation mts:Direction="Down" mts:StartColor="#FF0000" />
							</ss:Font>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="Percent" ss:Parent="RightText">
						<ss:NumberFormat ss:Format="0.00%;(0.00)%;"/>
					</ss:Style>
					<ss:Style ss:ID="EditCenterText" ss:Parent="CenterText">
						<mts:Display>
							<ss:Protection ss:Protected="0" />
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="EditPercent" ss:Parent="Percent">
						<mts:Display>
							<ss:Protection ss:Protected="0" />
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="SoftBidPrice" ss:Parent="RightText">
						<ss:NumberFormat ss:Format="&quot;Bid&quot;+#,##0.00;&quot;Bid&quot;-#,##0.00;"/>
					</ss:Style>
					<ss:Style ss:ID="SoftAskPrice" ss:Parent="RightText">
						<ss:NumberFormat ss:Format="&quot;Ask&quot;+#,##0.00;&quot;Ask&quot;-#,##0.00;"/>
					</ss:Style>
					<ss:Style ss:ID="Quantity" ss:Parent="RightText">
						<ss:NumberFormat ss:Format="#,##0"/>
					</ss:Style>
					<ss:Style ss:ID="EditQuantity" ss:Parent="Quantity">
						<mts:Display>
							<ss:Protection ss:Protected="0" />
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="MarketValue" ss:Parent="RightText">
						<ss:NumberFormat ss:Format="#,##0.00"/>
					</ss:Style>
					<ss:Style ss:ID="Commission" ss:Parent="RightText">
						<ss:NumberFormat ss:Format="#,##0.00;#,##0.00;"/>
					</ss:Style>
					<ss:Style ss:ID="Time" ss:Parent="LeftText">
						<ss:Alignment ss:Horizontal="Center" />
						<ss:NumberFormat ss:Format="h:mm A/P"/>
					</ss:Style>
					<ss:Style ss:ID="EditTime" ss:Parent="Time">
						<mts:Display>
							<ss:Protection ss:Protected="0" />
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="ShortDate" ss:Parent="CenterText">
						<ss:NumberFormat ss:Format="m/d;@"/>
					</ss:Style>
					<ss:Style ss:ID="DateTime" ss:Parent="LeftText">
						<ss:NumberFormat ss:Format="MM-dd-yy h:mm A/P"/>
					</ss:Style>
					<ss:Style ss:ID="TopDateTime" ss:Parent="LeftText">
						<ss:Alignment ss:Vertical="Top" />
						<ss:NumberFormat ss:Format="mm-dd-yy h:mm A/P"/>
					</ss:Style>
					<ss:Style ss:ID="ActiveStatusText" ss:Parent="CenterText">
						<mts:Display>
							<ss:Font ss:Color="#00C000"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="ErrorStatusText" ss:Parent="CenterText">
						<mts:Display>
							<ss:Font ss:Color="#CF0000"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="SubmittedStatusText" ss:Parent="CenterText">
						<mts:Display>
							<ss:Font ss:Color="#0000CF"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="HeldStatusText" ss:Parent="CenterText">
						<mts:Display>
							<ss:Font ss:Color="#000064"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="FilledStatusText" ss:Parent="CenterText">
						<mts:Display>
							<ss:Font ss:Color="#404040"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="BuyText" ss:Parent="CenterText">
						<mts:Display>
							<ss:Font ss:Color="#000064"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="SellText" ss:Parent="CenterText">
						<mts:Display>
							<ss:Font ss:Color="#640000"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="FilledCenterText" ss:Parent="CenterText">
						<mts:Display>
							<ss:Font ss:Color="#808080"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="FilledRightText" ss:Parent="RightText">
						<mts:Display>
							<ss:Font ss:Color="#808080"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="FilledLeftText" ss:Parent="LeftText">
						<mts:Display>
							<ss:Font ss:Color="#808080"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="FilledQuantity" ss:Parent="Quantity">
						<mts:Display>
							<ss:Font ss:Color="#808080"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="FilledPrice" ss:Parent="Price">
						<mts:Display>
							<ss:Font ss:Color="#808080" >
								<mts:Animation mts:Direction="Nil" mts:StartColor="#0000FF" />
								<mts:Animation mts:Direction="Up" mts:StartColor="#00FF00" />
								<mts:Animation mts:Direction="Down" mts:StartColor="#FF0000" />
							</ss:Font>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="FilledPercent" ss:Parent="Percent">
						<mts:Display>
							<ss:Font ss:Color="#808080"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="FilledSoftBidPrice" ss:Parent="SoftBidPrice">
						<mts:Display>
							<ss:Font ss:Color="#808080"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="FilledSoftAskPrice" ss:Parent="SoftAskPrice">
						<mts:Display>
							<ss:Font ss:Color="#808080"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="ExecutedQuantity" ss:Parent="Quantity">
						<mts:Display>
							<ss:Font ss:Color="#808080"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="FilledMarketValue" ss:Parent="MarketValue">
						<mts:Display>
							<ss:Font ss:Color="#808080"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="FilledCommission" ss:Parent="Commission">
						<mts:Display>
							<ss:Font ss:Color="#808080"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="FilledTime" ss:Parent="Time">
						<mts:Display>
							<ss:Font ss:Color="#808080"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="FilledShortDate" ss:Parent="ShortDate">
						<mts:Display>
							<ss:Font ss:Color="#808080"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="FilledDateTime" ss:Parent="DateTime">
						<mts:Display>
							<ss:Font ss:Color="#808080"/>
						</mts:Display>
					</ss:Style>
					
				</ss:Styles>
				
				<c:ComponentOptions>
					<c:Toolbar ss:Hidden="1">
						<c:HideOfficeLogo/>
					</c:Toolbar>
				</c:ComponentOptions>

				<ss:Worksheet ss:Name="Sheet1">
					<x:WorksheetOptions>
						<x:DoNotDisplayColHeaders/>
						<x:DoNotDisplayGridlines/>
						<x:DoNotDisplayRowHeaders/>
						<mts:DoNotSelectRows/>
					</x:WorksheetOptions>

					<ss:Table mts:HeaderHeight="25">

						<ss:Columns>
							<ss:Column mts:ColumnId="StatusImage" ss:Width="20" ss:StyleID="TopCenterText" mts:PrintWidth="35" mts:Image="iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAlwSFlzAAALEQAACxEBf2RfkQAAAelJREFUOE+t0t9PUmEYB3C778/of2htuTBZZ24ao5GWKMrRA5wDIqCw+JEdDCkMGd20+GUoDQ+r7ALxQoaLETdsLBZ3navGRRPWYnLvtyNObl6nY/O9eS/e5/28z/O8z8DAda1IJMLHE5tHoVCwYzVrM4FV49hLDyNbeUaPuO3ad163XvQ46CPX8qxo5SY+Ee9ms9k/nU4H2x93sWSdQWjdguArM/w8B6+HxQvnHIJrJoQ3LFhenPxHAIIg/Gg2mwi/jYLVT3QDw28sWFvRw76o7gLrvjPAzI7/JoBEIrHfbrfxPrINHfOkB/ie6wiAnVP+JIBoNLrZarWQ3BLAzD+9FKCnRr8TQCwW8zcaDezsfJEAtdQDc7eE8wx413yvB5OqB/sEkEwmF0RRxO7XPakEDTYCC70eOCxTcNtn4FrS4LWPg+rhfYEAUqnU43q9jr3cATgDDb/XgIDPCOkbYdKpQKtHt6bHRyi1ijJI++2LMhisVqs4PPwGhmGgHBv+zNGPOEajMLBa5ZB04calM5fJZG6VSiWUy2VwRhPu3B3i+xrSXC53s1AonFQqFdhsNshksnBfwGlwsVg8rtVqcDqdkMvlH/oG8vn8L2mB53lQFEXO+1ViOp12xOPxv1IJdYVCce+q+Gs5/w/Ocy+mFPCPZQAAAABJRU5ErkJggg==" mts:Description="Status Image"/>
							<ss:Column mts:ColumnId="StatusCode" ss:Hidden="1" ss:Width="15" ss:StyleID="TopCenterText" mts:PrintWidth="20" mts:Description="Status&#10;Code"/>
							<ss:Column mts:ColumnId="StatusName" ss:Hidden="1" ss:Width="40" ss:StyleID="TopCenterText" mts:PrintWidth="35" mts:Description="Status"/>
							<ss:Column mts:ColumnId="SubmissionTypeImage" ss:Width="32" ss:StyleID="TopCenterText" mts:PrintWidth="35" mts:Description="Submit"/>
							<ss:Column mts:ColumnId="SubmissionTypeCode" ss:Hidden="1" ss:Width="15" ss:StyleID="TopCenterText" mts:PrintWidth="35" mts:Description="Submit"/>
							<ss:Column mts:ColumnId="BlotterName" ss:Hidden="1" ss:Width="70" ss:StyleID="TopLeftText" mts:Printed="false"  mts:Description="Blotter" />
							<ss:Column mts:ColumnId="TimeInForce" ss:Hidden="1" ss:Width="30" ss:StyleID="TopLeftText" mts:PrintWidth="33" mts:Description="TIF"/>
							<ss:Column mts:ColumnId="WorkingOrderId" ss:Hidden="1" ss:Width="30" ss:StyleID="TopRightText" mts:PrintWidth="24" mts:Description="Order&#10;Id" />
							<ss:Column mts:ColumnId="SecurityId" ss:Hidden="1" ss:Width="60" ss:StyleID="TopRightText" mts:Printed="false" mts:Description="Security&#10;Id" />
							<ss:Column mts:ColumnId="SecuritySymbol" ss:Width="35" ss:StyleID="TopLeftText" mts:PrintWidth="31" mts:Description="Ticker" />
							<ss:Column mts:ColumnId="OrderTypeMnemonic" ss:Hidden="1" ss:Width="23" ss:StyleID="TopLeftText" mts:PrintWidth="45" mts:Description="Side" />
							<ss:Column mts:ColumnId="OrderTypeDescription" ss:Width="23" ss:StyleID="TopLeftText" mts:PrintWidth="45" mts:Description="Side" />
							<ss:Column mts:ColumnId="OrderTypeImage" ss:Hidden="1" ss:Width="23" ss:StyleID="TopLeftText" mts:PrintWidth="45" mts:Description="Side" />
							<ss:Column mts:ColumnId="SourceOrderQuantity" ss:Hidden="1" ss:Width="42" ss:StyleID="TopRightText" mts:PrintWidth="35" mts:Description="Ordered&#10;Quantity"/>
							<ss:Column mts:ColumnId="SubmittedQuantity" ss:Width="42" ss:StyleID="TopRightText" mts:PrintWidth="37" mts:Description="Match&#10;Quantity" />
							<ss:Column mts:ColumnId="LeavesQuantity" ss:Width="42" ss:StyleID="TopRightText" mts:PrintWidth="35" mts:Description="Leaves&#10;Quantity" />
							<ss:Column mts:ColumnId="WorkingQuantity" ss:Hidden="1" ss:Width="42" ss:StyleID="TopRightText" mts:PrintWidth="37" mts:Description="Working&#10;Quantity" />
							<ss:Column mts:ColumnId="DestinationOrderQuantity" ss:Hidden="1" ss:Width="42" ss:StyleID="TopRightText" mts:PrintWidth="37" mts:Description="Sent&#10;Quantity" />
							<ss:Column mts:ColumnId="ExecutedQuantity" ss:Hidden="1" ss:Width="42" ss:StyleID="TopRightText" mts:PrintWidth="37" mts:Description="Executed&#10;Quantity"/>
							<ss:Column mts:ColumnId="AllocatedQuantity" ss:Hidden="1" ss:Width="42" ss:StyleID="TopRightText" mts:PrintWidth="37" mts:Description="Allocated&#10;Quantity"/>
							<ss:Column mts:ColumnId="LastPrice" ss:Width="40" ss:StyleID="TopRightText" mts:Printed="false" mts:Description="Last&#10;Price"/>
							<ss:Column mts:ColumnId="AskPrice" ss:Hidden="1" ss:Width="40" ss:StyleID="TopRightText" mts:Printed="false" mts:Description="Ask&#10;Price"/>
							<ss:Column mts:ColumnId="BidPrice" ss:Hidden="1" ss:Width="40" ss:StyleID="TopRightText" mts:Printed="false" mts:Description="Bid&#10;Price"/>
							<ss:Column mts:ColumnId="LimitPrice" ss:Width="40" ss:StyleID="TopRightText" mts:PrintWidth="37" mts:Description="Limit&#10;Price"/>
							<ss:Column mts:ColumnId="Volume" ss:Width="40" ss:StyleID="TopRightText" mts:Printed="false" mts:Description="Volume"/>
							<ss:Column mts:ColumnId="AverageDailyVolume" ss:Hidden="1" ss:Width="45" ss:StyleID="TopCenterText" mts:Description="ADV"/>
							<ss:Column mts:ColumnId="VolumeCategoryMnemonic" ss:Width="45" ss:StyleID="TopCenterText" mts:Description="ADV"/>
							<ss:Column mts:ColumnId="IsInstitutionMatch" ss:Width="20" ss:StyleID="CenterCenterText" mts:PrintWidth="37" mts:Description="I" />
							<ss:Column mts:ColumnId="IsHedgeMatch" ss:Width="20" ss:StyleID="CenterCenterText" mts:PrintWidth="37" mts:Description="H" />
							<ss:Column mts:ColumnId="IsBrokerMatch" ss:Width="20" ss:StyleID="CenterCenterText" mts:PrintWidth="37" mts:Description="B"  />
							<ss:Column mts:ColumnId="StartTime" ss:Width="35" ss:StyleID="TopCenterText" mts:PrintWidth="37" mts:Description="Start&#10;Time"/>
							<ss:Column mts:ColumnId="StopTime" ss:Width="35" ss:StyleID="TopCenterText" mts:PrintWidth="37" mts:Description="End&#10;Time"/>
							<ss:Column mts:ColumnId="MaximumVolatility" ss:Width="42" ss:StyleID="TopRightText" mts:PrintWidth="37" mts:Description="Maximum&#10;Volatility"/>
							<ss:Column mts:ColumnId="NewsFreeTime" ss:Width="48" ss:StyleID="TopRightText" mts:PrintWidth="37" mts:Description="News Free&#10;Time"/>
							<ss:Column mts:ColumnId="TimeLeft" ss:Width="35" ss:StyleID="TopCenterText" mts:PrintWidth="35" mts:Description="Sleep&#10;Time"/>
							<ss:Column mts:ColumnId="AveragePrice" ss:Width="40" ss:StyleID="TopRightText" mts:PrintWidth="37" mts:Description="Avg.&#10;Price"/>
							<ss:Column mts:ColumnId="LimitTypeMnemonic" ss:Hidden="1" ss:Width="50" ss:StyleID="TopLeftText" mts:Description="Limit&#10;Type"/>
							<ss:Column mts:ColumnId="Destination" ss:Hidden="1" ss:Width="55" ss:StyleID="TopLeftText" mts:Description="Destination"/>
							<ss:Column mts:ColumnId="IsAllocated" ss:Width="40" ss:StyleID="TopCenterText" mts:PrintWidth="25" mts:Description="Allocated"/>
							<ss:Column mts:ColumnId="AccountName" ss:Hidden="1"  ss:Width="50" ss:StyleID="TopLeftText" mts:Printed="false" mts:Description="Account"/>
							<ss:Column mts:ColumnId="CommissionRateType" ss:Hidden="1" ss:Width="52" ss:StyleID="TopLeftText" mts:Printed="false" mts:Description="Comm Type"/>
							<ss:Column mts:ColumnId="CommissionRate" ss:Hidden="1" ss:Width="50" ss:StyleID="TopRightText" mts:Printed="false" mts:Description="Comm Rate"/>
							<ss:Column mts:ColumnId="Commission" ss:Hidden="1" ss:Width="50" ss:StyleID="TopRightText" mts:PrintWidth="45" mts:Description="Comission"/>
							<ss:Column mts:ColumnId="StopLimit" ss:Hidden="1" ss:Width="30" ss:StyleID="TopRightText" mts:Description="Stop"/>
							<ss:Column mts:ColumnId="Market" ss:Hidden="1" ss:Width="45" ss:StyleID="TopLeftText" mts:Description="Market"/>
							<ss:Column mts:ColumnId="MarketCapitalization" ss:Width="45" ss:StyleID="TopLeftText" mts:Description="Market&#10;Cap."/>
							<ss:Column mts:ColumnId="CreatedTime" ss:Hidden="1" ss:Width="68" ss:StyleID="TopDateTime" mts:PrintWidth="65" mts:Description="Created&#10;Time"/>
							<ss:Column mts:ColumnId="CreatedName" ss:Hidden="1" ss:Width="90" ss:StyleID="TopLeftText" mts:Printed="false" mts:Description="Created&#10;By"/>
							<ss:Column mts:ColumnId="Aon" ss:Hidden="1" ss:Width="25" ss:StyleID="TopCenterText" mts:Printed="false"  mts:Description="AON"/>
							<ss:Column mts:ColumnId="CmtaBroker" ss:Hidden="1" ss:Width="55" ss:StyleID="TopLeftText" mts:Printed="false"  mts:Description="CMTA Broker" />
							<ss:Column mts:ColumnId="DiscretionInstruction" ss:Hidden="1" ss:Width="85" ss:StyleID="TopCenterText" mts:Printed="false"  mts:Description="Discretion Instruction" />
							<ss:Column mts:ColumnId="DiscretionOffset" ss:Hidden="1" ss:Width="80" ss:StyleID="TopLeftText" mts:Printed="false"  mts:Description="Discresion Offset." />
							<ss:Column mts:ColumnId="DisplayQuantity" ss:Hidden="1" ss:Width="40" ss:StyleID="TopRightText" mts:Printed="false"  mts:Description="Display" />
							<ss:Column mts:ColumnId="FilledGross" ss:Hidden="1" ss:Width="50" ss:StyleID="MarketValue" mts:Printed="false"  mts:Description="Filled Gross" />
							<ss:Column mts:ColumnId="FixNote" ss:Hidden="1" ss:Width="40" ss:StyleID="TopLeftText" mts:Printed="false"  mts:Description="FIX Note" />
							<ss:Column mts:ColumnId="ImportId" ss:Hidden="1" ss:Width="50" ss:StyleID="TopLeftText" mts:Printed="false"  mts:Description="Import Id" />
							<ss:Column mts:ColumnId="OmsFixNote" ss:Hidden="1" ss:Width="60" ss:StyleID="TopLeftText" mts:Printed="false"  mts:Description="OMS Fix Note" />
							<ss:Column mts:ColumnId="PeggedDifference" ss:Hidden="1" ss:Width="70" ss:StyleID="TopRightText" mts:Printed="false"  mts:Description="Peg Difference" />
							<ss:Column mts:ColumnId="Proactive" ss:Hidden="1" ss:Width="40" ss:StyleID="TopLeftText" mts:Printed="false"  mts:Description="Proactive" />
							<ss:Column mts:ColumnId="TradeDate" ss:Hidden="1" ss:Width="50" ss:StyleID="ShortDate" mts:Printed="false"  mts:Description="Trade Date" />
							<ss:Column mts:ColumnId="FilledNet" ss:Hidden="1" ss:Width="50" ss:StyleID="MarketValue" mts:Printed="false"  mts:Description="Filled Net" />
							<ss:Column mts:ColumnId="UploadTime" ss:Hidden="1" ss:Width="65" ss:StyleID="Time" mts:Printed="false"  mts:Description="Upload Date" />
							<ss:Column mts:ColumnId="UnfilledQuantity" ss:Hidden="1" ss:Width="50" ss:StyleID="TopRightText" mts:Printed="false"  mts:Description="UnfilledQuantity" />
						</ss:Columns>
						<mts:Constraint mts:PrimaryKey="True">
							<mts:ConstraintColumn mts:ColumnId="WorkingOrderId" />
						</mts:Constraint>
						<mts:View>
							<mts:ViewColumn mts:ColumnId="SourceOrderQuantity" mts:Direction="descending" />
						</mts:View>
						
						<apply-templates />
						
					</ss:Table>
					
				</ss:Worksheet>
				
			</ss:Workbook>

	</template>
	
	<!-- Template for building incremental updates to the viewed document -->
	<template match="Fragment">
		<mts:Fragment>
			<apply-templates/>
		</mts:Fragment>
	</template>
	
	<!-- Incremental Inserts to the viewed document -->
	<template match="Insert">
		<mts:Insert>
			<apply-templates/>
		</mts:Insert>
	</template>
	
	<!-- Incremental Updates to the viewed document -->
	<template match="Update">
		<mts:Update>
			<apply-templates/>
		</mts:Update>
	</template>
	
	<!-- Incremental Deletions from the viewed document -->
	<template match="Delete">
		<mts:Delete>
			<apply-templates/>
		</mts:Delete>
	</template>

	<template match="WorkingOrder">

		<ss:Row>
		
			<variable name="StatusStyle">
				<choose>
					<when test="@StatusCode = 0">ActiveStatusText</when>
					<when test="@StatusCode = 8">ErrorStatusText</when>
					<when test="@StatusCode = 9">SubmittedStatusText</when>
					<otherwise>FilledStatusText</otherwise>
				</choose>
			</variable>
			<variable name="OrderTypeStyle">
				<choose>
					<when test="@StatusCode != 6 and (@OrderTypeCode = 0 or @OrderTypeCode = 2)">BuyText</when>
					<when test="@StatusCode != 6 and (@OrderTypeCode = 1 or @OrderTypeCode = 3)">SellText</when>
					<otherwise>FilledCenterText</otherwise>
				</choose>
			</variable>
			<variable name="TimeStyle">
				<choose>
					<when test="@StatusCode != 6">Time</when>
					<otherwise>FilledTime</otherwise>
				</choose>
			</variable>
			<variable name="EditCenterTextStyle">
				<choose>
					<when test="@StatusCode != 6">EditCenterText</when>
					<otherwise>CenterText</otherwise>
				</choose>
			</variable>
			<variable name="EditTimeStyle">
				<choose>
					<when test="@StatusCode != 6">EditTime</when>
					<otherwise>FilledTime</otherwise>
				</choose>
			</variable>
			<variable name="EditPercentStyle">
				<choose>
					<when test="@StatusCode != 6">EditPercent</when>
					<otherwise>FilledPercent</otherwise>
				</choose>
			</variable>
			<variable name="EditQuantityStyle">
				<choose>
					<when test="@StatusCode != 6">EditQuantity</when>
					<otherwise>FilledQuantity</otherwise>
				</choose>
			</variable>
			<variable name="CenterTextStyle">
				<choose>
					<when test="@StatusCode != 6">CenterText</when>
					<otherwise>FilledCenterText</otherwise>
				</choose>
			</variable>
			<variable name="LeftTextStyle">
				<choose>
					<when test="@StatusCode != 6">LeftText</when>
					<otherwise>FilledLeftText</otherwise>
				</choose>
			</variable>
			<variable name="RightTextStyle">
				<choose>
					<when test="@StatusCode != 6">RightText</when>
					<otherwise>FilledRightText</otherwise>
				</choose>
			</variable>
			<variable name="QuantityStyle">
				<choose>
					<when test="@StatusCode != 6">Quantity</when>
					<otherwise>ExecutedQuantity</otherwise>
				</choose>
			</variable>
			<variable name="PriceStyle">
				<choose>
					<when test="@StatusCode != 6">Price</when>
					<otherwise>FilledPrice</otherwise>
				</choose>
			</variable>
			<variable name="PercentStyle">
				<choose>
					<when test="@StatusCode != 6">Percent</when>
					<otherwise>FilledPercent</otherwise>
				</choose>
			</variable>
			<variable name="SoftAskPriceStyle">
				<choose>
					<when test="@StatusCode != 6">SoftAskPrice</when>
					<otherwise>FilledSoftAskPrice</otherwise>
				</choose>
			</variable>
			<variable name="SoftBidPriceStyle">
				<choose>
					<when test="@StatusCode != 6">SoftBidPrice</when>
					<otherwise>FilledSoftBidPrice</otherwise>
				</choose>
			</variable>
			<variable name="MarketValueStyle">
				<choose>
					<when test="@StatusCode != 6">MarketValue</when>
					<otherwise>FilledMarketValue</otherwise>
				</choose>
			</variable>
			<variable name="CommissionStyle">
				<choose>
					<when test="@StatusCode != 6">Commission</when>
					<otherwise>FilledCommission</otherwise>
				</choose>
			</variable>
			<variable name="DateTimeStyle">
				<choose>
					<when test="@StatusCode != 6">DateTime</when>
					<otherwise>FilledDateTime</otherwise>
				</choose>
			</variable>
			<variable name="ShortDateStyle">
				<choose>
					<when test="@StatusCode != 6">ShortDate</when>
					<otherwise>FilledShortDate</otherwise>
				</choose>
			</variable>

			<if test="@AccountName">
				<ss:Cell mts:ColumnId="AccountName">
					<attribute name="ss:StyleID"><value-of select="$CenterTextStyle"/></attribute>
					<if test="@AccountName"><ss:Data ss:Type="string"><value-of select="@AccountName" /></ss:Data></if>
				</ss:Cell>
			</if>
			<if test="@AllocatedQuantity">
				<ss:Cell mts:ColumnId="AllocatedQuantity">
					<attribute name="ss:StyleID"><value-of select="$QuantityStyle"/></attribute>
					<ss:Data ss:Type="decimal"><value-of select="@AllocatedQuantity" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@Aon">
				<ss:Cell mts:ColumnId="Aon">
					<attribute name="ss:StyleID"><value-of select="$CenterTextStyle"/></attribute>
					<choose>
						<when test="@Aon=1"><ss:Data ss:Type="string">YES</ss:Data></when>
						<otherwise><ss:Data ss:Type="string">NO</ss:Data></otherwise>
					</choose>
				</ss:Cell>
			</if>
			<if test="@AskPrice">
				<ss:Cell mts:ColumnId="AskPrice">
					<attribute name="ss:StyleID"><value-of select="$PriceStyle"/></attribute>
					<ss:Data ss:Type="decimal"><value-of select="@AskPrice" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@AverageDailyVolume">
				<ss:Cell mts:ColumnId="AverageDailyVolume">
					<attribute name="ss:StyleID">
						<value-of select="$QuantityStyle"/>
					</attribute>
					<ss:Data ss:Type="decimal">
						<value-of select="@AverageDailyVolume" />
					</ss:Data>
				</ss:Cell>
			</if>
			<if test="@VolumeCategoryMnemonic">
				<ss:Cell mts:ColumnId="VolumeCategoryMnemonic">
					<attribute name="ss:StyleID">
						<value-of select="$QuantityStyle"/>
					</attribute>
					<ss:Data ss:Type="string">
						<value-of select="@VolumeCategoryMnemonic" />
					</ss:Data>
				</ss:Cell>
			</if>
			<if test="@AveragePrice">
				<ss:Cell mts:ColumnId="AveragePrice">
					<attribute name="ss:StyleID"><value-of select="$PriceStyle"/></attribute>
					<ss:Data ss:Type="decimal"><value-of select="@AveragePrice" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@BidPrice">
				<ss:Cell mts:ColumnId="BidPrice">
					<attribute name="ss:StyleID"><value-of select="$PriceStyle"/></attribute>
					<ss:Data ss:Type="decimal"><value-of select="@BidPrice" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@BlotterName">
				<ss:Cell mts:ColumnId="BlotterName">
					<attribute name="ss:StyleID"><value-of select="$LeftTextStyle"/></attribute>
					<ss:Data ss:Type="string"><value-of select="@BlotterName" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@CreatedName">
				<ss:Cell mts:ColumnId="CreatedName">
					<attribute name="ss:StyleID"><value-of select="$LeftTextStyle"/></attribute>
					<ss:Data ss:Type="string"><value-of select="@CreatedName" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@CreatedTime">
				<ss:Cell mts:ColumnId="CreatedTime">
					<attribute name="ss:StyleID"><value-of select="$DateTimeStyle"/></attribute>
					<ss:Data ss:Type="DateTime"><value-of select="@CreatedTime" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@CmtaBrokerName">
				<ss:Cell mts:ColumnId="CmtaBroker">
					<attribute name="ss:StyleID"><value-of select="$LeftTextStyle"/></attribute>
					<ss:Data ss:Type="string"><value-of select="@CmtaBrokerName" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@Commission">
				<ss:Cell mts:ColumnId="Commission">
					<attribute name="ss:StyleID"><value-of select="$CommissionStyle"/></attribute>
					<ss:Data ss:Type="decimal"><value-of select="@Commission" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@CommissionRateTypeCount and @CommissionRate">
				<ss:Cell mts:ColumnId="CommissionRate">
					<attribute name="ss:StyleID"><value-of select="$PriceStyle"/></attribute>
					<choose>
						<when test="@CommissionRateTypeCount=1">
							<ss:Data ss:Type="decimal"><value-of select="@CommissionRate" /></ss:Data>
						</when>
					</choose>
				</ss:Cell>
			</if>
			<if test="@CommissionRateTypeCount and @CommissionRateTypeName">
				<ss:Cell mts:ColumnId="CommissionRateType">
					<attribute name="ss:StyleID"><value-of select="$LeftTextStyle"/></attribute>
					<choose>
						<when test="@CommissionRateTypeCount=1">
							<ss:Data ss:Type="string"><value-of select="@CommissionRateTypeName" /></ss:Data>
						</when>
						<when test="@CommissionRateTypeCount=2">
							<ss:Data ss:Type="string">Multiple</ss:Data>
						</when>
					</choose>
				</ss:Cell>
			</if>
			<if test="@SourceOrderQuantity">
				<ss:Cell mts:ColumnId="SourceOrderQuantity">
					<attribute name="ss:StyleID"><value-of select="$QuantityStyle"/></attribute>
					<ss:Data ss:Type="decimal"><value-of select="@SourceOrderQuantity" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@DestinationOrderQuantity">
				<ss:Cell mts:ColumnId="DestinationOrderQuantity">
					<attribute name="ss:StyleID"><value-of select="$QuantityStyle"/></attribute>
					<ss:Data ss:Type="decimal"><value-of select="@DestinationOrderQuantity" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@DestinationShortName">
				<ss:Cell mts:ColumnId="Destination">
					<attribute name="ss:StyleID"><value-of select="$LeftTextStyle"/></attribute>
					<ss:Data ss:Type="string"><value-of select="@DestinationShortName" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@DiscretionInstruction">
				<ss:Cell mts:ColumnId="DiscretionInstruction">
					<attribute name="ss:StyleID"><value-of select="$CenterTextStyle"/></attribute>
					<ss:Data ss:Type="string">YES</ss:Data>
				</ss:Cell>
			</if>
			<if test="@DiscretionOffset">
				<ss:Cell mts:ColumnId="DiscretionOffset">
					<attribute name="ss:StyleID"><value-of select="$LeftTextStyle"/></attribute>
					<ss:Data ss:Type="decimal"><value-of select="@DiscretionOffset" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@DisplayQuantity">
				<ss:Cell mts:ColumnId="DisplayQuantity">
					<attribute name="ss:StyleID"><value-of select="$QuantityStyle"/></attribute>
					<ss:Data ss:Type="decimal"><value-of select="@DisplayQuantity" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@StopTime">
				<ss:Cell mts:ColumnId="StopTime">
					<attribute name="ss:StyleID"><value-of select="$EditTimeStyle"/></attribute>
					<ss:Data ss:Type="DateTime"><value-of select="@StopTime" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@ExecutedQuantity">
				<ss:Cell mts:ColumnId="ExecutedQuantity">
					<attribute name="ss:StyleID"><value-of select="$QuantityStyle"/></attribute>
					<ss:Data ss:Type="decimal"><value-of select="@ExecutedQuantity" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@FilledNet">
				<ss:Cell mts:ColumnId="FilledNet">
					<attribute name="ss:StyleID"><value-of select="$MarketValueStyle"/></attribute>
					<ss:Data ss:Type="decimal"><value-of select="@FilledNet" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@FixNote">
				<ss:Cell mts:ColumnId="FixNote">
					<attribute name="ss:StyleID"><value-of select="$LeftTextStyle"/></attribute>
					<ss:Data ss:Type="string"><value-of select="@FixNote" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@ImportId">
				<ss:Cell mts:ColumnId="ImportId">
					<attribute name="ss:StyleID"><value-of select="$LeftTextStyle"/></attribute>
					<ss:Data ss:Type="string"><value-of select="@ImportId" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@IsInstitutionMatch">
				<ss:Cell mts:ColumnId="IsInstitutionMatch">
					<choose>
						<when test="@IsInstitutionMatch='True'">
							<ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAlwSFlzAAAK7wAACu8BfXaKSAAAAXtJREFUOE/V0UlLQlEUB/D/M3s0aVYOSYEuhAbNsiKiRURFRJCLFkLtGmxfELSOFmXQ4DOkYVeLJit9NEtEFEoWNC36CPoFmuF0EwykCF16t+f8zrn3f4HUOoso4lwI8YL0DuOoTO7ym9Ckubmn0ZtBavOXk8SBh8QHeKCCC48joQGyeFVUJapJOikJxw0w3CvM+lOZFW5I4gpryOcE3A5f9TOsoQpRSZmz/DNm0PrTZziS220XLZ9Wfx1l7GCZFbhocQMKOHEdwyYvw3P8C5bQHrekXiyN2IPN1HaiI5OoIqxigcFcCAgMBXvZZi2ZfAWUPcu/Yh4dv96uXsnxNR7oqPlYT02HOjKs5RE3zUWGAn1U7dOS0cvwHP/GsPXv4NaRhUWc1bDmhv1iqhULqdNfS1W7GirfjeJ3hrv+T92LHJb0uXlbTRYfgx4llX1vdvIfDNsS+7IVyDCNy5KtAjKyLOQuhgV0J4ZjXVOQYwx76RNpYTjQkxxOye4v23eJoKnyKGMAAAAASUVORK5CYII=</ss:Data>
						</when>
						<otherwise>
							<ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAlwSFlzAAALEwAACxMBAJqcGAAAACZJREFUOE9j/P//PwNFAGQAJZgizWDXU2L7qAGQ2BsNxNEwAKUDAJpD0E2H+d9aAAAAAElFTkSuQmCCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==</ss:Data>
						</otherwise>
					</choose>
				</ss:Cell>
			</if>
			<if test="@IsHedgeMatch">
				<ss:Cell mts:ColumnId="IsHedgeMatch">
					<choose>
						<when test="@IsHedgeMatch='True'">
							<ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAlwSFlzAAAK7wAACu8BfXaKSAAAAXtJREFUOE/V0UlLQlEUB/D/M3s0aVYOSYEuhAbNsiKiRURFRJCLFkLtGmxfELSOFmXQ4DOkYVeLJit9NEtEFEoWNC36CPoFmuF0EwykCF16t+f8zrn3f4HUOoso4lwI8YL0DuOoTO7ym9Ckubmn0ZtBavOXk8SBh8QHeKCCC48joQGyeFVUJapJOikJxw0w3CvM+lOZFW5I4gpryOcE3A5f9TOsoQpRSZmz/DNm0PrTZziS220XLZ9Wfx1l7GCZFbhocQMKOHEdwyYvw3P8C5bQHrekXiyN2IPN1HaiI5OoIqxigcFcCAgMBXvZZi2ZfAWUPcu/Yh4dv96uXsnxNR7oqPlYT02HOjKs5RE3zUWGAn1U7dOS0cvwHP/GsPXv4NaRhUWc1bDmhv1iqhULqdNfS1W7GirfjeJ3hrv+T92LHJb0uXlbTRYfgx4llX1vdvIfDNsS+7IVyDCNy5KtAjKyLOQuhgV0J4ZjXVOQYwx76RNpYTjQkxxOye4v23eJoKnyKGMAAAAASUVORK5CYII=</ss:Data>
						</when>
						<otherwise>
							<ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAlwSFlzAAALEwAACxMBAJqcGAAAACZJREFUOE9j/P//PwNFAGQAJZgizWDXU2L7qAGQ2BsNxNEwAKUDAJpD0E2H+d9aAAAAAElFTkSuQmCCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==</ss:Data>
						</otherwise>
					</choose>
				</ss:Cell>
			</if>
			<if test="@IsBrokerMatch">
				<ss:Cell mts:ColumnId="IsBrokerMatch">
					<choose>
						<when test="@IsBrokerMatch='True'">
							<ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAlwSFlzAAAK7wAACu8BfXaKSAAAAXtJREFUOE/V0UlLQlEUB/D/M3s0aVYOSYEuhAbNsiKiRURFRJCLFkLtGmxfELSOFmXQ4DOkYVeLJit9NEtEFEoWNC36CPoFmuF0EwykCF16t+f8zrn3f4HUOoso4lwI8YL0DuOoTO7ym9Ckubmn0ZtBavOXk8SBh8QHeKCCC48joQGyeFVUJapJOikJxw0w3CvM+lOZFW5I4gpryOcE3A5f9TOsoQpRSZmz/DNm0PrTZziS220XLZ9Wfx1l7GCZFbhocQMKOHEdwyYvw3P8C5bQHrekXiyN2IPN1HaiI5OoIqxigcFcCAgMBXvZZi2ZfAWUPcu/Yh4dv96uXsnxNR7oqPlYT02HOjKs5RE3zUWGAn1U7dOS0cvwHP/GsPXv4NaRhUWc1bDmhv1iqhULqdNfS1W7GirfjeJ3hrv+T92LHJb0uXlbTRYfgx4llX1vdvIfDNsS+7IVyDCNy5KtAjKyLOQuhgV0J4ZjXVOQYwx76RNpYTjQkxxOye4v23eJoKnyKGMAAAAASUVORK5CYII=</ss:Data>
						</when>
						<otherwise>
							<ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAlwSFlzAAALEwAACxMBAJqcGAAAACZJREFUOE9j/P//PwNFAGQAJZgizWDXU2L7qAGQ2BsNxNEwAKUDAJpD0E2H+d9aAAAAAElFTkSuQmCCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==</ss:Data>
						</otherwise>
					</choose>
				</ss:Cell>
			</if>
			<if test="@ExecutedQuantity and @QuantityAllocated">
				<ss:Cell mts:ColumnId="IsAllocated">
					<attribute name="ss:StyleID"><value-of select="$LeftTextStyle"/></attribute>
					<choose>
						<when test="@ExecutedQuantity != 0 and @ExecutedQuantity &lt;= @QuantityAllocated"><ss:Data ss:Type="string">YES</ss:Data></when>
						<otherwise><ss:Data ss:Type="string">NO</ss:Data></otherwise>
					</choose>
				</ss:Cell>
			</if>
			<if test="@LastPrice">
				<ss:Cell mts:ColumnId="LastPrice">
					<attribute name="ss:StyleID"><value-of select="$PriceStyle"/></attribute>
					<ss:Data ss:Type="decimal"><value-of select="@LastPrice" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@LeavesQuantity">
				<ss:Cell mts:ColumnId="LeavesQuantity">
					<attribute name="ss:StyleID"><value-of select="$QuantityStyle"/></attribute>
						<ss:Data ss:Type="decimal"><value-of select="@LeavesQuantity" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@LimitTypeMnemonic">
				<ss:Cell mts:ColumnId="LimitTypeMnemonic">
					<attribute name="ss:StyleID"><value-of select="$LeftTextStyle"/></attribute>
					<ss:Data ss:Type="string"><value-of select="@LimitTypeMnemonic" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@MarketCapitalization">
				<ss:Cell mts:ColumnId="MarketCapitalization">
					<attribute name="ss:StyleID">
						<value-of select="$QuantityStyle"/>
					</attribute>
					<ss:Data ss:Type="decimal">
						<value-of select="@MarketCapitalization" />
					</ss:Data>
				</ss:Cell>
			</if>
			<if test="@MarketShortName">
				<ss:Cell mts:ColumnId="Market">
					<attribute name="ss:StyleID"><value-of select="$CenterTextStyle"/></attribute>
					<ss:Data ss:Type="string"><value-of select="@MarketShortName" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@MarketValue">
				<ss:Cell mts:ColumnId="FilledGross">
					<attribute name="ss:StyleID"><value-of select="$MarketValueStyle"/></attribute>
					<ss:Data ss:Type="decimal"><value-of select="@MarketValue" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@MaximumVolatility">
				<ss:Cell mts:ColumnId="MaximumVolatility">
					<attribute name="ss:StyleID"><value-of select="$EditPercentStyle"/></attribute>
					<ss:Data ss:Type="decimal"><value-of select="@MaximumVolatility" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@NewsFreeTime">
				<ss:Cell mts:ColumnId="NewsFreeTime">
					<attribute name="ss:StyleID"><value-of select="$EditQuantityStyle"/></attribute>
					<ss:Data ss:Type="decimal"><value-of select="@NewsFreeTime" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@OmsFixNote">
				<ss:Cell mts:ColumnId="OmsFixNote">
					<attribute name="ss:StyleID"><value-of select="$LeftTextStyle"/></attribute>
					<ss:Data ss:Type="string"><value-of select="@OmsFixNote" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@OrderTypeMnemonic">
				<ss:Cell mts:ColumnId="OrderTypeMnemonic">
					<attribute name="ss:StyleID"><value-of select="$OrderTypeStyle"/></attribute>
					<ss:Data ss:Type="string"><value-of select="@OrderTypeMnemonic" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@OrderTypeDescription">
				<ss:Cell mts:ColumnId="OrderTypeDescription">
					<attribute name="ss:StyleID">
						<value-of select="$OrderTypeStyle"/>
					</attribute>
					<ss:Data ss:Type="string">
						<value-of select="@OrderTypeDescription" />
					</ss:Data>
				</ss:Cell>
			</if>
			<if test="@OrderTypeImage">
				<ss:Cell mts:ColumnId="OrderTypeImage">
					<ss:Data ss:Type="image">
						<value-of select="@OrderTypeImage" />
					</ss:Data>
				</ss:Cell>
			</if>
			<if test="@PeggedDifference">
				<ss:Cell mts:ColumnId="PeggedDifference">
					<attribute name="ss:StyleID"><value-of select="$PriceStyle"/></attribute>
					<ss:Data ss:Type="decimal"><value-of select="@PeggedDifference" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@ProactivePassiveName">
				<ss:Cell mts:ColumnId="Proactive">
					<attribute name="ss:StyleID"><value-of select="$LeftTextStyle"/></attribute>
					<ss:Data ss:Type="string"><value-of select="@ProactivePassiveName" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@SecurityId">
				<ss:Cell mts:ColumnId="SecurityId">
					<attribute name="ss:StyleID"><value-of select="$RightTextStyle"/></attribute>
					<ss:Data ss:Type="int"><value-of select="@SecurityId" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@SecuritySymbol">
				<ss:Cell mts:ColumnId="SecuritySymbol">
					<attribute name="ss:StyleID"><value-of select="$LeftTextStyle"/></attribute>
					<ss:Data ss:Type="string"><value-of select="@SecuritySymbol" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@SpecifiedLimit">
				<ss:Cell mts:ColumnId="LimitPrice">
					<choose>
						<when test="@AskBidFlag = 'ASK'">
							<attribute name="ss:StyleID"><value-of select="$SoftAskPriceStyle"/></attribute>
							<if test="@SpecifiedLimit"><ss:Data ss:Type="decimal"><value-of select="@SpecifiedLimit" /></ss:Data></if>
						</when>
						<when test="@AskBidFlag = 'BID'">
							<attribute name="ss:StyleID"><value-of select="$SoftBidPriceStyle"/></attribute>
							<if test="@SpecifiedLimit"><ss:Data ss:Type="decimal"><value-of select="@SpecifiedLimit" /></ss:Data></if>
						</when>
						<otherwise>
							<attribute name="ss:StyleID"><value-of select="$PriceStyle"/></attribute>
							<if test="@SpecifiedLimit"><ss:Data ss:Type="decimal"><value-of select="@SpecifiedLimit" /></ss:Data></if>
						</otherwise>
					</choose>
				</ss:Cell>
			</if>
			<if test="@StartTime">
				<ss:Cell mts:ColumnId="StartTime">
					<attribute name="ss:StyleID"><value-of select="$EditTimeStyle"/></attribute>
					<ss:Data ss:Type="DateTime"><value-of select="@StartTime" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@StatusCode">
				<ss:Cell mts:ColumnId="StatusCode">
					<attribute name="ss:StyleID">
						<value-of select="$RightTextStyle"/>
					</attribute>
					<ss:Data ss:Type="int">
						<value-of select="@StatusCode" />
					</ss:Data>
				</ss:Cell>
			</if>
			<if test="@StatusCode">
				<ss:Cell mts:ColumnId="StatusImage">
					<ss:Data ss:Type="image">
						<choose>
							<when test="@StatusCode=0">
								<ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAlwSFlzAAALEQAACxEBf2RfkQAAAl5JREFUOE+tk11I02EUxp//f5bpmDO3TKdDEV1mNkzKUrdCm0oTtc3Isuyibyypm+gigwqCICLCiyKEbuwLhyhiiIl9CRmrHF00QtYHBpYUmJDkzD0dQSQ1u/LcnJvz/J73fc77Aotd/YDtMdD0Gsiay+4FDnQq6O4Atp0G1Hne74Frn915oYkLVfQpytO/B14CJW8txiCrLRy06tmpUcRrTgUSDR/ZWEtequJQ8Rq+AvZOjUjP9iboR3koi3QlMZBrZBvQOA/gBS4H693k+e3k2VK+MGiHHwKFz1dEfWXtJnJnKkedJraHq4NyDf08wDsgc8BmCfGcAE4VMbg/h8+MuhDrbOSeDE5WJrPXrJ2UjEoWzL5Poz5hfSl5soA8lk8ezyX3rSV3pdGfY+ADoOG/i3sElH3YkkbWifjgehFbyd2rOFycQI+C/vvAsgUBknSiOHQNlYvoaM60OJ3ckcJheyzvAT3NgPZfAKUPONxnjhmZqLWTRzaQNZniLOLKFLLcTJaZ+N0eQ48KbxdgmgXpAVyBjUnkiTxxFWH1arLKwt/uZLIikZPOeAaLYxksMnLUrmdbpDokp9k6A2mRVQ1kx0nS4iirmnL9UZrANt2SiUCWniFnLMcdBo4VRHNscxRHbJG8BUhc09WuUXzjLhG65RQVZn5xrGSrNuxbK5DnAa4HrDqOFy7nT7uOI/ladpvCeBuomQFIslfGHHH8VRbPN+ui2awqPgkzdXpAkVfX4E+PoN8awXaDhgK9OiuDJsDYulT1tkRqPt0BztwAwucmfRe4eFNeqvSMRf3AfwCHJCQ0JeevfQAAAABJRU5ErkJggg==</ss:Data>
							</when>
							<when test="@StatusCode=9">
								<ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAlwSFlzAAAK7wAACu8BfXaKSAAAAXtJREFUOE/V0UlLQlEUB/D/M3s0aVYOSYEuhAbNsiKiRURFRJCLFkLtGmxfELSOFmXQ4DOkYVeLJit9NEtEFEoWNC36CPoFmuF0EwykCF16t+f8zrn3f4HUOoso4lwI8YL0DuOoTO7ym9Ckubmn0ZtBavOXk8SBh8QHeKCCC48joQGyeFVUJapJOikJxw0w3CvM+lOZFW5I4gpryOcE3A5f9TOsoQpRSZmz/DNm0PrTZziS220XLZ9Wfx1l7GCZFbhocQMKOHEdwyYvw3P8C5bQHrekXiyN2IPN1HaiI5OoIqxigcFcCAgMBXvZZi2ZfAWUPcu/Yh4dv96uXsnxNR7oqPlYT02HOjKs5RE3zUWGAn1U7dOS0cvwHP/GsPXv4NaRhUWc1bDmhv1iqhULqdNfS1W7GirfjeJ3hrv+T92LHJb0uXlbTRYfgx4llX1vdvIfDNsS+7IVyDCNy5KtAjKyLOQuhgV0J4ZjXVOQYwx76RNpYTjQkxxOye4v23eJoKnyKGMAAAAASUVORK5CYII=</ss:Data>
							</when>
							<otherwise>
								<ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAlwSFlzAAALEwAACxMBAJqcGAAAACZJREFUOE9j/P//PwNFAGQAJZgizWDXU2L7qAGQ2BsNxNEwAKUDAJpD0E2H+d9aAAAAAElFTkSuQmCCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==</ss:Data>
							</otherwise>
						</choose>
					</ss:Data>
				</ss:Cell>
			</if>
			<if test="@StatusName">
				<ss:Cell mts:ColumnId="StatusName">
					<attribute name="ss:StyleID"><value-of select="$StatusStyle"/></attribute>
					<ss:Data ss:Type="string"><value-of select="@StatusName" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@StopLimit">
				<ss:Cell mts:ColumnId="StopLimit">
					<attribute name="ss:StyleID"><value-of select="$PriceStyle"/></attribute>
					<ss:Data ss:Type="decimal"><value-of select="@StopLimit" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@SubmissionTypeCode">
				<ss:Cell mts:ColumnId="SubmissionTypeCode">
					<attribute name="ss:StyleID">
						<value-of select="$EditCenterTextStyle"/>
					</attribute>
					<ss:Data ss:Type="int">
						<value-of select="@SubmissionTypeCode" />
					</ss:Data>
				</ss:Cell>
			</if>
			<if test="@SubmissionTypeCode">
				<ss:Cell mts:ColumnId="SubmissionTypeImage">
					<attribute name="ss:StyleID">
						<value-of select="$EditCenterTextStyle"/>
					</attribute>
					<choose>
						<when test="@SubmissionTypeCode=1">
							<ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAlwSFlzAAAK7wAACu8BfXaKSAAAAg1JREFUOE+tkltI02EYh3/SiZFzmnfRVVhGCguxi440itFx2YiYLTtemGS1sswxZ4UlTqEokGAQk2lJ5xypxCxvOmhr5XBu1XSr5qq1qJaTP1H561+76GIii/rgvfjgfZ7vPXzAfztzG/bNVl+OIs/8GLnNi/7aK1M0hh/5PtHcHmDOZtsPyC+YIb80LWlR5nJrtLEzSEOTlzqLl8qqbk5a3PQOC9q3JCXJUFijda0B7mpwU2Xq5dLjvZQbXZRqOkex5IYdy3pmjStKW9E8XNHi54ZTfVwowlnlTmaUOjix5AlR7CTWtgpQ3KpC1tUpY4pSlS2xYqufilo351S6mK57SuwW4T0uQuchygfi99U3vVjZlZcgmbrqSkxrecn8Gi+n691M2dtH7O8nDj4jDr0gjgwSxiBRHSbW2DwJAum66zG1JcjcGh9l+udEmRiHfUSFnzC8Io6GiJMRou4zoWpzJwhSC2wxlfUts00BSowipA8Qla//gPXReBVquwcFD+YlCja2jagufmD26RAl1UPEsTfEiffxF2s/Eju6R0TQgBLn5DGHKNXYBfW1Yeaci1BWL5ZqEsEzgtjKwCg0Dzuwc2jm+GvcelfQdgicb/3CGee/csKvfrX3Q9jm25TUR5IU3QkfcJBK2zeml/V/R6HjLHSDaUnBv5PW396eWdoTSSnsuociV37y4D9m/gTcyw/NY+8hRgAAAABJRU5ErkJggg==</ss:Data>
						</when>
						<when test="@SubmissionTypeCode=2">
							<ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAlwSFlzAAAK7wAACu8BfXaKSAAAAhxJREFUOE+t0l1Ik2EUB/Czkj4G7d2HBBG7MVJM07UNrUZZGFEJZjW00OHMbH5sY3PTajlzlWuZiWxCNIiii7wpkEyXU4ylOLVpQ4dNIbpJSroaTXZT/nvZ7UQW9Vyf8zt/znOI/turJH3GrfQI1VCANKT4a5cx8lfmvs2g9+NTSO7v/80ibmomYdJQqik18ir0Al1jd9Ex3oLql0pwG7d9p+tUkRQiaBREns32oHXEAOPbSjR4lNB4i5DjEq+RmYbpDu3dEBKa+D97ptvR5L2M2sESVPQfw4XXB3BucB+K32RA1M6NsWla6TZtXRfiX2Oinf4b0HvLUOU5gdIBGYoH0qH0ZqLcl42aaRlO9u0BWekTy0gTEIGFF3VMGlA/fBaqIQXOD2Xi4mgWVGM5qJ6SQDMjhW4+H01hBVLaNi0kAlYmap/Som70DFQjebj0LgtV/lxcDchQH8yDPnQY5sUCWL8UgmPbHEoAhG1M9F6gDg2+01D75FBPSFA7K4d27iDM4QLYvp6C8n0uOA7OAnWQJAEQ2ZnVB0EtDBNFuOLPj0/WzR+C5XMhTEvHsfsJb5UcdJO9ji3rLlHkYGLdISOaP5RAFzgan9zCxj3iSVujh+Sh55S24Tfu7BLE3GELbMFy2BbZRU5Ksd2ZskyPqTSpQ9rRyV3pX3bCtaRFdq/4F3WTkx4RL6nmeJGd1GL3rh9s3HFykTz5xn+s/APCPvcuDotPNAAAAABJRU5ErkJggg==</ss:Data>
						</when>
						<otherwise>
							<ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAAlwSFlzAAAK7wAACu8BfXaKSAAAAsNJREFUOE+tk9tLk3Ecxl9zDofTHdw8zVMmalqWNjUxtTKXaUoH0xw1nCdqtdqozLAyO4Cn6ECU2E0qbhndSNBNdONFRBBFEIQXgbV686e2WczZcnv6mrD5B/jCc/l5Dt/3fTluNZ57HNcyJJfYrHGxzBIdwSwRCmZVyplFIWVWeSizyELZY6mYWSUhbFQmZqNigW2A45p82UMyic35/g1mng5jur8Psz3nYW9vxpypGs6W3XDpCuHWquGu2YRFEupz8Ewc8NVnYE2IZ/axUXzvuwL+ogHsVC1+6kvxq0aN+coUuDUqeLfLgaJQkhjQyPFCHsh8BhaVitlH+vGj04RpsxaOxlI4a7LgqUgEdkjh3SYECgOB4iAyIJUIMa4M8Bs8UUUyx0AvZtoaMHesAi5tDlBJcIkE2EmARgSUhwB7KX1J+8V4Fb3GbzAWp2S/73bAYa6FS18M78E0YI8S2EXJ/4EwoEYGHKYZWpIuHG8TBH6D5wlK5uxrhdNYBc/RXAIovYzAKtq8BOoUQGME0BIFNJNOxOBD8oobvExSsIUuM9yGMuDIZuCAiiaQQS3BegKPRwPGWMAcv6y2RHxKD/I3GE9WsL9dRngMGkqj13SIgGraX08zDDHL0IUk4HIy0EG6kYLPG1cYvE5VsMVuI3CSDPRZQB01qKP0pcrmOKCdoGupQHc60LsBuJOJb9lCf4N36Qrm7T0Nr6mcIHoDurU0JZyqU/pZukcnwTczgPtk3p8NDGRjKm+FwcdMJfPcboX33D46UAHQtJ7qR1J9atJKZlfJ4FYm8HALwaRHubDnB/sbTGSE2TDYDVzXU91K4Ew+HY0gE9W/tA7ooeoPKH0wDxgmjeTBWSD84vsSJ6K4JluWdHJqq5KfypHyTC3mZ9UikoB35Ar4uXwh7ywI5ueLRPxCsYj/UxQ86UrjGlbjR+b+AZa91WGTif0vAAAAAElFTkSuQmCC</ss:Data>
						</otherwise>
					</choose>
				</ss:Cell>
			</if>
			<if test="@SubmittedQuantity">
				<ss:Cell mts:ColumnId="SubmittedQuantity">
					<attribute name="ss:StyleID"><value-of select="$EditQuantityStyle"/></attribute>
						<ss:Data ss:Type="decimal"><value-of select="@SubmittedQuantity" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@TimeLeft">
				<ss:Cell mts:ColumnId="TimeLeft">
					<attribute name="ss:StyleID">
						<value-of select="$RightTextStyle"/>
					</attribute>
					<ss:Data ss:Type="string">
						<value-of select="@TimeLeft" />
					</ss:Data>
				</ss:Cell>
			</if>
			<if test="@TradeDate">
				<ss:Cell mts:ColumnId="TradeDate">
					<attribute name="ss:StyleID"><value-of select="$ShortDateStyle"/></attribute>
					<ss:Data ss:Type="DateTime"><value-of select="@TradeDate" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@TimeInForceName">
				<ss:Cell mts:ColumnId="TimeInForce">
					<attribute name="ss:StyleID"><value-of select="$LeftTextStyle"/></attribute>
					<ss:Data ss:Type="string"><value-of select="@TimeInForceName" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@UploadTime">
				<ss:Cell mts:ColumnId="UploadTime">
					<attribute name="ss:StyleID"><value-of select="$DateTimeStyle"/></attribute>
					<ss:Data ss:Type="DateTime"><value-of select="@UploadTime" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@UnfilledQuantity">
				<ss:Cell mts:ColumnId="UnfilledQuantity">
					<attribute name="ss:StyleID"><value-of select="$QuantityStyle"/></attribute>
					<ss:Data ss:Type="decimal"><value-of select="@UnfilledQuantity" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@WorkingOrderId">
				<ss:Cell mts:ColumnId="WorkingOrderId">
					<attribute name="ss:StyleID"><value-of select="$RightTextStyle"/></attribute>
					<ss:Data ss:Type="int"><value-of select="@WorkingOrderId" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@WorkingQuantity">
				<ss:Cell mts:ColumnId="WorkingQuantity">
					<attribute name="ss:StyleID"><value-of select="$QuantityStyle"/></attribute>
						<ss:Data ss:Type="decimal"><value-of select="@WorkingQuantity" /></ss:Data>
				</ss:Cell>
			</if>

		</ss:Row>
		
	</template>

</stylesheet>
