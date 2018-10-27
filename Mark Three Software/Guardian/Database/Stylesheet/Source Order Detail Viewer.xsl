<?xml version="1.0" encoding="utf-8" standalone="yes"?>
<stylesheet version="1.0" xmlns="http://www.w3.org/1999/XSL/Transform"
	xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
	xmlns:x="urn:schemas-microsoft-com:office:excel"
	xmlns:c="urn:schemas-microsoft-com:office:component:spreadsheet"
	xmlns:mts="urn:schemas-markthreesoftware-com:stylesheet">
	
	<!-- Version 1.0 -->

	<!-- These parameters are used by the loader to specify the attributes of the stylesheet. -->
 	<mts:stylesheetId>SOURCE ORDER DETAIL VIEWER</mts:stylesheetId>
	<mts:stylesheetTypeCode>SOURCE ORDER DETAIL</mts:stylesheetTypeCode>
	<mts:name>Source Order Detail Viewer</mts:name>  

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

					<ss:Table mts:HeaderHeight="12.5">

						<ss:Columns>
							<ss:Column mts:ColumnId="StatusTypeCode" ss:Hidden="1" ss:Width="15" ss:StyleID="TopCenterText" mts:PrintWidth="20" mts:Description="Status Code"/>
							<ss:Column mts:ColumnId="StatusName" ss:Width="40" ss:StyleID="TopCenterText" mts:PrintWidth="35" mts:Description="Status"/>
							<ss:Column mts:ColumnId="BlotterName" ss:Hidden="1" ss:Width="70" ss:StyleID="TopLeftText" mts:Printed="false"  mts:Description="Blotter" />
							<ss:Column mts:ColumnId="OrderTypeMnemonic" ss:Hidden="1" ss:Width="23" ss:StyleID="TopLeftText" mts:PrintWidth="45" mts:Description="Side" />
							<ss:Column mts:ColumnId="TimeInForce" ss:Hidden="1" ss:Width="30" ss:StyleID="TopLeftText" mts:PrintWidth="33" mts:Description="TIF"/>
							<ss:Column mts:ColumnId="SourceOrderId" ss:Width="30" ss:StyleID="TopRightText" mts:PrintWidth="24" mts:Description="Id" />
							<ss:Column mts:ColumnId="SecurityId" ss:Hidden="1" ss:Width="60" ss:StyleID="TopRightText" mts:Printed="false" mts:Description="Sec. Id" />
							<ss:Column mts:ColumnId="SecuritySymbol" ss:Hidden="1" ss:Width="35" ss:StyleID="TopLeftText" mts:PrintWidth="31" mts:Description="Symbol" />
							<ss:Column mts:ColumnId="SourceOrderQuantity" ss:Width="42" ss:StyleID="TopRightText" mts:PrintWidth="37" mts:Description="Ordered" />
							<ss:Column mts:ColumnId="PriceTypeMnemonic" ss:Width="45" ss:StyleID="TopLeftText" mts:Description="Pricing"/>
							<ss:Column mts:ColumnId="LimitPrice" ss:Width="40" ss:StyleID="TopRightText" mts:PrintWidth="37" mts:Description="Limit"/>
							<ss:Column mts:ColumnId="StopLimit" ss:Width="30" ss:StyleID="TopRightText" mts:Description="Stop"/>
							<ss:Column mts:ColumnId="Destination" ss:Width="65" ss:StyleID="TopLeftText" mts:Description="Destination"/>
							<ss:Column mts:ColumnId="Market" ss:Width="45" ss:StyleID="TopLeftText" mts:Description="Market"/>
							<ss:Column mts:ColumnId="CreatedTime" ss:Width="68" ss:StyleID="TopDateTime" mts:PrintWidth="65" mts:Description="Time"/>
							<ss:Column mts:ColumnId="CreatedName" ss:Width="90" ss:StyleID="TopLeftText" mts:Printed="false" mts:Description="Who"/>
							<ss:Column mts:ColumnId="Aon" ss:Hidden="1" ss:Width="25" ss:StyleID="TopCenterText" mts:Printed="false"  mts:Description="AON"/>
							<ss:Column mts:ColumnId="CmtaBroker" ss:Hidden="1" ss:Width="55" ss:StyleID="TopLeftText" mts:Printed="false"  mts:Description="CMTA Broker" />
							<ss:Column mts:ColumnId="DiscretionInstruction" ss:Hidden="1" ss:Width="85" ss:StyleID="TopCenterText" mts:Printed="false"  mts:Description="Discretion Instruction" />
							<ss:Column mts:ColumnId="DiscretionOffset" ss:Hidden="1" ss:Width="80" ss:StyleID="TopLeftText" mts:Printed="false"  mts:Description="Discresion Offset." />
							<ss:Column mts:ColumnId="DisplayQuantity" ss:Hidden="1" ss:Width="40" ss:StyleID="TopRightText" mts:Printed="false"  mts:Description="Display" />
							<ss:Column mts:ColumnId="FixNote" ss:Hidden="1" ss:Width="40" ss:StyleID="TopLeftText" mts:Printed="false"  mts:Description="FIX Note" />
							<ss:Column mts:ColumnId="ImportId" ss:Hidden="1" ss:Width="50" ss:StyleID="TopLeftText" mts:Printed="false"  mts:Description="Import Id" />
							<ss:Column mts:ColumnId="OmsFixNote" ss:Hidden="1" ss:Width="60" ss:StyleID="TopLeftText" mts:Printed="false"  mts:Description="OMS Fix Note" />
							<ss:Column mts:ColumnId="PeggedDifference" ss:Hidden="1" ss:Width="70" ss:StyleID="TopRightText" mts:Printed="false"  mts:Description="Peg Difference" />
							<ss:Column mts:ColumnId="Proactive" ss:Hidden="1" ss:Width="40" ss:StyleID="TopLeftText" mts:Printed="false"  mts:Description="Proactive" />
							<ss:Column mts:ColumnId="TradeDate" ss:Hidden="1" ss:Width="50" ss:StyleID="ShortDate" mts:Printed="false"  mts:Description="Trade Date" />
							<ss:Column mts:ColumnId="UploadTime" ss:Hidden="1" ss:Width="65" ss:StyleID="Time" mts:Printed="false"  mts:Description="Upload Date" />
							<ss:Column mts:ColumnId="UnfilledQuantity" ss:Hidden="1" ss:Width="50" ss:StyleID="TopRightText" mts:Printed="false"  mts:Description="UnfilledQuantity" />
						</ss:Columns>
						<mts:Constraint mts:PrimaryKey="True">
							<mts:ConstraintColumn mts:ColumnId="SourceOrderId" />
						</mts:Constraint>
						<mts:View>
							<mts:ViewColumn mts:ColumnId="SourceOrderId" mts:Direction="ascending" />
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

	<template match="SourceOrder">

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
					<when test="@StatusCode != 6 and (@OrderTypeCode = 2 or @OrderTypeCode = 4)">BuyText</when>
					<when test="@StatusCode != 6 and (@OrderTypeCode = 3 or @OrderTypeCode = 5)">SellText</when>
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
					<otherwise>FilledQuantity</otherwise>
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

			<if test="@Aon">
				<ss:Cell mts:ColumnId="Aon">
					<attribute name="ss:StyleID"><value-of select="$CenterTextStyle"/></attribute>
					<choose>
						<when test="@Aon=1"><ss:Data ss:Type="string">YES</ss:Data></when>
						<otherwise><ss:Data ss:Type="string">NO</ss:Data></otherwise>
					</choose>
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
			<if test="@PriceTypeMnemonic">
				<ss:Cell mts:ColumnId="PriceTypeMnemonic">
					<attribute name="ss:StyleID"><value-of select="$LeftTextStyle"/></attribute>
					<ss:Data ss:Type="string"><value-of select="@PriceTypeMnemonic" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@OrderTypeMnemonic">
				<ss:Cell mts:ColumnId="OrderTypeMnemonic">
					<attribute name="ss:StyleID"><value-of select="$OrderTypeStyle"/></attribute>
					<ss:Data ss:Type="string"><value-of select="@OrderTypeMnemonic" /></ss:Data>
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
			<if test="@SourceOrderId">
				<ss:Cell mts:ColumnId="SourceOrderId">
					<attribute name="ss:StyleID"><value-of select="$RightTextStyle"/></attribute>
					<ss:Data ss:Type="int"><value-of select="@SourceOrderId" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@SourceOrderQuantity">
				<ss:Cell mts:ColumnId="SourceOrderQuantity">
					<attribute name="ss:StyleID"><value-of select="$QuantityStyle"/></attribute>
					<ss:Data ss:Type="decimal"><value-of select="@SourceOrderQuantity" /></ss:Data>
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
			<if test="@StatusName">
				<ss:Cell mts:ColumnId="StatusName">
					<attribute name="ss:StyleID"><value-of select="$StatusStyle"/></attribute>
					<ss:Data ss:Type="string"><value-of select="@StatusName" /></ss:Data>
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

		</ss:Row>
		
	</template>

</stylesheet>
