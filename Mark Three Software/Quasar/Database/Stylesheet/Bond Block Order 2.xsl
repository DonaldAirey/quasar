<?xml version="1.0" encoding="UTF-8" standalone="yes"?>

<!-- This stylesheet is used to turn flat, cartesian data from the database into a hierarchical dataset that can
be manipulated futher with XSL into a working document. -->
<xsl:stylesheet version="1.0" xmlns="urn:schemas-microsoft-com:office:spreadsheet"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
	xmlns:mts="urn:schemas-markthreesoftware-com:stylesheet">

	<!-- These parameters are used by the loader to specify the attributes of the stylesheet. -->
 	<mts:stylesheetId>BOND BLOCK ORDER</mts:stylesheetId>
	<mts:stylesheetTypeCode>BLOCK ORDER</mts:stylesheetTypeCode>
	<mts:name>Bond Block Order</mts:name>

	<!-- This specified the attributes of the output of the XSL Transaformation. -->
	<xsl:output method="xml" encoding="UTF-8" standalone="yes" indent="yes"/>

	<!-- Parameters -->
	<xsl:param name="sortMethod" select="0"/>

	<!-- Match the Document root. -->
	<xsl:template match="Blotter">

			<xsl:variable name="headerRows" select="1"/>
			<xsl:variable name="blockOrder" select="count(.//BlockOrder)"/>
			<xsl:variable name="rowCount" select="number($headerRows)+number($blockOrder)"/>
			<xsl:variable name="columnCount" select="17"/>

			<!-- This much is a standard spreadsheet header that was pulled from the XML output of EXCEL. -->

			<ss:Workbook xmlns:o="urn:schemas-microsoft-com:office:office"
				xmlns:x="urn:schemas-microsoft-com:office:excel"
				xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
				xmlns:c="urn:schemas-microsoft-com:office:component:spreadsheet">

				<!-- ExcelWoorkbook -->

				<x:ExcelWorkbook>
					<x:ActiveSheet>0</x:ActiveSheet>
					<x:HideWorkbookTabs/>
					<x:HideHorizontalScrollBar/>
					<x:HideVerticalScrollBar/>
				</x:ExcelWorkbook>
				
				<!-- Names -->

				<ss:Names>
					<ss:NamedRange ss:Name="rowType" ss:RefersTo="=Sheet1!C1"/>
					<ss:NamedRange ss:Name="blockOrderId" ss:RefersTo="=Sheet1!C2"/>
					<ss:NamedRange ss:Name="securityId" ss:RefersTo="=Sheet1!C3"/>
					<ss:NamedRange ss:Name="lastPrice" ss:RefersTo="=Sheet1!C10"/>
					<ss:NamedRange ss:Name="yield" ss:RefersTo="=Sheet1!C11"/>
					<ss:NamedRange ss:Name="quantityOrdered" ss:RefersTo="=Sheet1!C14"/>
					<ss:NamedRange ss:Name="quantityExecuted" ss:RefersTo="=Sheet1!C15"/>
					<ss:NamedRange ss:Name="tradeDate" ss:RefersTo="=Sheet1!C16"/>
					<ss:NamedRange ss:Name="settlementDate" ss:RefersTo="=Sheet1!C17"/>
				</ss:Names>

				<!-- Styles -->

				<ss:Styles>
					<ss:Style ss:ID="Default">
						<ss:Alignment ss:Horizontal="Automatic" ss:Rotate="0.0" ss:Vertical="Center"
						ss:ReadingOrder="Context"/>
						<ss:Font ss:FontName="Microsoft Sans Serif" ss:Size="8" ss:Color="Automatic" ss:Bold="0"
						ss:Italic="0" ss:Underline="None"/>
						<ss:Interior ss:Color="Automatic" ss:Pattern="None"/>
						<ss:NumberFormat ss:Format="General"/>
						<ss:Protection ss:Protected="1"/>
					</ss:Style>
					<ss:Style ss:ID="CenterText">
						<ss:Alignment ss:Horizontal="Center"/>
					</ss:Style>
					<ss:Style ss:ID="LeftText">
						<ss:Alignment ss:Horizontal="Left"/>
					</ss:Style>
					<ss:Style ss:ID="RightText">
						<ss:Alignment ss:Horizontal="Right"/>
					</ss:Style>
					<ss:Style ss:ID="Header" ss:Parent="Default">
						<ss:Interior ss:Color="#D5CCBB" ss:Pattern="Solid"/>
					</ss:Style>
					<ss:Style ss:ID="HeaderCenterText" ss:Parent="CenterText">
						<ss:Interior ss:Color="#D5CCBB" ss:Pattern="Solid"/>
						<ss:Borders>
							<ss:Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1" ss:Color="#000000"/>
						</ss:Borders>
					</ss:Style>
					<ss:Style ss:ID="HeaderLeftText" ss:Parent="LeftText">
						<ss:Interior ss:Color="#D5CCBB" ss:Pattern="Solid"/>
						<ss:Borders>
							<ss:Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1" ss:Color="#000000"/>
						</ss:Borders>
					</ss:Style>
					<ss:Style ss:ID="HeaderRightText" ss:Parent="RightText">
						<ss:Interior ss:Color="#D5CCBB" ss:Pattern="Solid"/>
						<ss:Borders>
							<ss:Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1" ss:Color="#000000"/>
						</ss:Borders>
					</ss:Style>
					<ss:Style ss:ID="Price">
						<ss:NumberFormat ss:Format="#,##0.000"/>
						<ss:Alignment ss:Horizontal="Right"/>
					</ss:Style>
					<ss:Style ss:ID="Quantity">
						<ss:NumberFormat ss:Format="#,##0_);[Red]\(#,##0\)"/>
						<ss:Alignment ss:Horizontal="Right"/>
					</ss:Style>
					<ss:Style ss:ID="MarketValue">
						<ss:NumberFormat ss:Format="#,##0.00"/>
						<ss:Alignment ss:Horizontal="Right"/>
					</ss:Style>
					<ss:Style ss:ID="Percent">
						<ss:NumberFormat ss:Format="0.000%;"/>
					</ss:Style>
					<ss:Style ss:ID="Date">
						<ss:NumberFormat ss:Format="m/d/yy"/>
					</ss:Style>
					<ss:Style ss:ID="EditPrice" ss:Parent="Price">
						<ss:Protection ss:Protected="0"/>
					</ss:Style>
					<ss:Style ss:ID="EditQuantity" ss:Parent="Quantity">
						<ss:Protection ss:Protected="0"/>
					</ss:Style>
					<ss:Style ss:ID="EditPercent" ss:Parent="Percent">
						<ss:Protection ss:Protected="0"/>
					</ss:Style>
					<ss:Style ss:ID="EditDate" ss:Parent="Date">
						<ss:Protection ss:Protected="0"/>
					</ss:Style>
					<Style ss:ID="NewLeftText" ss:Parent="LeftText">
						<Font ss:FontName="Microsoft Sans Serif" ss:Size="8" ss:Color="#339966"/>
					</Style>
					<ss:Style ss:ID="ClosedCenterText" ss:Parent="CenterText">
						<ss:Font ss:FontName="Microsoft Sans Serif" ss:Size="8" ss:Color="#808080"/>
					</ss:Style>
					<ss:Style ss:ID="ClosedRightText" ss:Parent="RightText">
						<ss:Font ss:FontName="Microsoft Sans Serif" ss:Size="8" ss:Color="#808080"/>
					</ss:Style>
					<ss:Style ss:ID="ClosedLeftText" ss:Parent="LeftText">
						<ss:Font ss:FontName="Microsoft Sans Serif" ss:Size="8" ss:Color="#808080"/>
					</ss:Style>
					<ss:Style ss:ID="ClosedPrice" ss:Parent="Price">
						<ss:Font ss:FontName="Microsoft Sans Serif" ss:Size="8" ss:Color="#808080"/>
					</ss:Style>
					<ss:Style ss:ID="ClosedQuantity" ss:Parent="Quantity">
						<ss:Font ss:FontName="Microsoft Sans Serif" ss:Size="8" ss:Color="#808080"/>
					</ss:Style>
					<Style ss:ID="SellQuantity" ss:Parent="Quantity">
						<Font ss:FontName="Microsoft Sans Serif" ss:Size="8" ss:Color="#FF0000"/>
					</Style>
					<ss:Style ss:ID="ClosedMarketValue" ss:Parent="MarketValue">
						<ss:Font ss:FontName="Microsoft Sans Serif" ss:Size="8" ss:Color="#808080"/>
					</ss:Style>
					<ss:Style ss:ID="ClosedPercent" ss:Parent="Percent">
						<ss:Font ss:FontName="Microsoft Sans Serif" ss:Size="8" ss:Color="#808080"/>
					</ss:Style>
					<ss:Style ss:ID="ClosedDate" ss:Parent="Date">
						<ss:Font ss:FontName="Microsoft Sans Serif" ss:Size="8" ss:Color="#808080"/>
					</ss:Style>
				</ss:Styles>

				<!-- Componenet Options -->

				<c:ComponentOptions>
					<c:Toolbar ss:Hidden="1">
						<c:HideOfficeLogo/>
					</c:Toolbar>
				</c:ComponentOptions>

				<!-- Worksheet: "Sheet1" -->
				
				<ss:Worksheet ss:Name="Sheet1">
					<x:WorksheetOptions>
						<x:DoNotDisplayColHeaders/>
						<x:DoNotDisplayGridlines/>
						<x:DoNotDisplayRowHeaders/>
						<x:FreezePanes/>
						<x:SplitHorizontal>2</x:SplitHorizontal>
						<x:TopRowVisible>0</x:TopRowVisible>
						<x:TopRowBottomPane>2</x:TopRowBottomPane>
						<x:SplitVertical>2</x:SplitVertical>
						<x:LeftColumnVisible>0</x:LeftColumnVisible>
						<x:LeftColumnRightPane>3</x:LeftColumnRightPane>
					</x:WorksheetOptions>

					<!-- Table: The meat of the spreadsheet -->

					<ss:Table>

						<!-- Column Definitions -->

						<ss:Column ss:Index="1" ss:Width="28" ss:Hidden="1" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="2" ss:Width="28" ss:Hidden="1" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="3" ss:Width="28" ss:Hidden="1" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="4" ss:Width="18.75" ss:StyleID="LeftText" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="5" ss:Width="28.5" ss:StyleID="LeftText" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="6" ss:Width="51" ss:StyleID="LeftText" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="7" ss:Width="90" ss:StyleID="LeftText" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="8" ss:Width="36" ss:StyleID="Percent" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="9" ss:Width="39.75" ss:StyleID="Date" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="10" ss:Width="34" ss:StyleID="Price" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="11" ss:Width="36" ss:StyleID="Percent" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="12" ss:Width="33" ss:StyleID="Price" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="13" ss:Width="33" ss:StyleID="Price" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="14" ss:Width="49.5" ss:StyleID="Quantity" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="15" ss:Width="49.5" ss:StyleID="Quantity" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="16" ss:Width="39.75" ss:StyleID="Date" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="17" ss:Width="39.75" ss:StyleID="Date" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="18" ss:Width="41" ss:StyleID="MarketValue" ss:AutoFitWidth="0"/>

						<!-- This column needs to be included to guarantee that the last readable column can be scrolled 
						into the viewer.  The column width should be equal to the largest column in the report. -->
						<ss:Column ss:Index="19" ss:Width="90" ss:AutoFitWidth="0"/>

						<!-- The Column Headings -->

						<ss:Row ss:Height="12" ss:StyleID="Header">
							<ss:Cell ss:Index="4" ss:StyleID="HeaderLeftText">
								<ss:Data ss:Type="String">Side</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="5" ss:StyleID="HeaderLeftText">
								<ss:Data ss:Type="String">Status</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="6" ss:StyleID="HeaderLeftText">
								<ss:Data ss:Type="String">CUSIP</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="7" ss:StyleID="HeaderLeftText">
								<ss:Data ss:Type="String">Name</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="8" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Coupon</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="9" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Maturity</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="10" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Price</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="11" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Yield</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="12" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Duration</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="13" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Convex</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="14" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Quantity</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="15" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Quantity</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="16" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Trade</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="17" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Settle</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="18" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Accr'd</ss:Data>
							</ss:Cell>
						</ss:Row>
						<ss:Row ss:Height="12" ss:StyleID="Header">
							<ss:Cell ss:Index="4" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="5" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="6" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="7" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="8" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="9" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Date</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="10" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="11" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="12" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="13" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="14" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Ordered</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="15" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Executed</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="16" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Date</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="17" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Date</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="18" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Int.</ss:Data>
							</ss:Cell>
						</ss:Row>

						<!-- The Main Body of the Spreadsheet -->
						
						<xsl:choose>
							<xsl:when test="$sortMethod = 1">
								<xsl:apply-templates>
									<xsl:sort select="@QuantityExecuted" data-type="number" order="ascending"/>
								</xsl:apply-templates>
							</xsl:when>
							<xsl:when test="$sortMethod = 2">
								<xsl:apply-templates>
									<xsl:sort select="@QuantityLeaves" data-type="number" order="ascending"/>
								</xsl:apply-templates>
							</xsl:when>
							<xsl:when test="$sortMethod = 3">
								<xsl:apply-templates>
									<xsl:sort select="@QuantityOrdered" data-type="number" order="ascending"/>
								</xsl:apply-templates>
							</xsl:when>
							<xsl:when test="$sortMethod = 4">
								<xsl:apply-templates>
									<xsl:sort select="@SecuritySymbol" data-type="text" order="ascending"/>
								</xsl:apply-templates>
							</xsl:when>
							<xsl:when test="$sortMethod = 100">
								<xsl:apply-templates>
									<xsl:sort select="@SecurityName" data-type="text" order="descending"/>
								</xsl:apply-templates>
							</xsl:when>
							<xsl:when test="$sortMethod = 101">
								<xsl:apply-templates>
									<xsl:sort select="@QuantityExecuted" data-type="number" order="descending"/>
								</xsl:apply-templates>
							</xsl:when>
							<xsl:when test="$sortMethod = 102">
								<xsl:apply-templates>
									<xsl:sort select="@QuantityLeaves" data-type="number" order="descending"/>
								</xsl:apply-templates>
							</xsl:when>
							<xsl:when test="$sortMethod = 103">
								<xsl:apply-templates>
									<xsl:sort select="@QuantityOrdered" data-type="number" order="descending"/>
								</xsl:apply-templates>
							</xsl:when>
							<xsl:when test="$sortMethod = 104">
								<xsl:apply-templates>
									<xsl:sort select="@SecuritySymbol" data-type="text" order="descending"/>
								</xsl:apply-templates>
							</xsl:when>
							<xsl:otherwise>
								<xsl:apply-templates>
									<xsl:sort select="@SecurityName" data-type="text" order="ascending"/>
								</xsl:apply-templates>
							</xsl:otherwise>							
						</xsl:choose>
						
						<!-- This row needs to be included as a buffer.  It allows the last line to be scrolled into the
						viewer.  The height of this line should be the same as the height of the largest row in the
						regular report. -->
						<ss:Row ss:Height="12"/>

					</ss:Table>
				</ss:Worksheet>
			</ss:Workbook>

	</xsl:template>

	<!-- Block Order -->
	<xsl:template match="BlockOrder">

		<!-- Select the color coding based on the statusCode. -->
		<xsl:variable name="SecurityNameStyle">
			<xsl:choose>
				<xsl:when test="@StatusCode = 1">NewLeftText</xsl:when>
				<xsl:when test="@StatusCode = 3">ClosedLeftText</xsl:when>
				<xsl:otherwise>LeftText</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="LeftTextStyle">
			<xsl:choose>
				<xsl:when test="@StatusCode = 3">ClosedLeftText</xsl:when>
				<xsl:otherwise>LeftText</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="RightTextStyle">
			<xsl:choose>
				<xsl:when test="@StatusCode = 3">ClosedRightText</xsl:when>
				<xsl:otherwise>RightText</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="PriceStyle">
			<xsl:choose>
				<xsl:when test="@StatusCode = 3">ClosedPrice</xsl:when>
				<xsl:otherwise>Price</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="QuantityStyle">
			<xsl:choose>
				<xsl:when test="@StatusCode = 3">ClosedQuantity</xsl:when>
				<xsl:otherwise>Quantity</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="MarketValueStyle">
			<xsl:choose>
				<xsl:when test="@StatusCode = 3">ClosedMarketValue</xsl:when>
				<xsl:otherwise>MarketValue</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="PercentStyle">
			<xsl:choose>
				<xsl:when test="@StatusCode = 3">ClosedPercent</xsl:when>
				<xsl:otherwise>Percent</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="DateStyle">
			<xsl:choose>
				<xsl:when test="@StatusCode = 3">ClosedDate</xsl:when>
				<xsl:otherwise>Date</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="EditPriceStyle">
			<xsl:choose>
				<xsl:when test="@StatusCode = 3">ClosedPrice</xsl:when>
				<xsl:otherwise>EditPrice</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="EditQuantityStyle">
			<xsl:choose>
				<xsl:when test="@StatusCode = 3">ClosedQuantity</xsl:when>
				<xsl:otherwise>EditQuantity</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="EditPercentStyle">
			<xsl:choose>
				<xsl:when test="@StatusCode = 3">ClosedPercent</xsl:when>
				<xsl:otherwise>EditPercent</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:variable name="EditDateStyle">
			<xsl:choose>
				<xsl:when test="@StatusCode = 3">ClosedDate</xsl:when>
				<xsl:otherwise>EditDate</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>

		<!-- Block Heading -->
		<ss:Row ss:Height="12">
			
			<ss:Cell ss:Index="1"><ss:Data ss:Type="Number">1</ss:Data></ss:Cell>
			<ss:Cell ss:Index="2"><ss:Data ss:Type="Number"><xsl:value-of select="@BlockOrderId"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="3"><ss:Data ss:Type="Number"><xsl:value-of select="@SecurityId"/></ss:Data></ss:Cell>
			<xsl:if test="@TransactionTypeCode">
				<ss:Cell ss:Index="4">
					<xsl:attribute name="ss:StyleID"><xsl:value-of select="$LeftTextStyle"/></xsl:attribute>
					<ss:Data ss:Type="String"><xsl:value-of select="@TransactionTypeName"/></ss:Data>
				</ss:Cell>
			</xsl:if>
			<ss:Cell ss:Index="5">
					<xsl:choose>
						<xsl:when test="@StatusCode = 0">
							<xsl:attribute name="ss:StyleID">NewLeftText</xsl:attribute>
						</xsl:when>
						<xsl:when test="@StatusCode = 3">
							<xsl:attribute name="ss:StyleID">ClosedLeftText</xsl:attribute>
						</xsl:when>
					</xsl:choose>
				<ss:Data ss:Type="String"><xsl:value-of select="@StatusName"/></ss:Data>
			</ss:Cell>
			<xsl:if test="@SecurityId">
				<ss:Cell ss:Index="6">
					<xsl:attribute name="ss:StyleID"><xsl:value-of select="$LeftTextStyle"/></xsl:attribute>
					<ss:Data ss:Type="String"><xsl:value-of select="@SecuritySymbol"/></ss:Data>
				</ss:Cell>
				<ss:Cell ss:Index="7">
					<xsl:attribute name="ss:StyleID"><xsl:value-of select="$LeftTextStyle"/></xsl:attribute>
					<ss:Data ss:Type="String"><xsl:value-of select="@SecurityName"/></ss:Data>
				</ss:Cell>
				<ss:Cell ss:Index="8">
				<xsl:attribute name="ss:StyleID"><xsl:value-of select="$PercentStyle"/></xsl:attribute>
					<ss:Data ss:Type="Number"><xsl:value-of select="@Coupon"/></ss:Data>
				</ss:Cell>
				<ss:Cell ss:Index="9">
				<xsl:attribute name="ss:StyleID"><xsl:value-of select="$DateStyle"/></xsl:attribute>
					<ss:Data ss:Type="DateTime"><xsl:value-of select="@MaturityDate"/></ss:Data>
				</ss:Cell>
				<ss:Cell ss:Index="10">
				<xsl:attribute name="ss:StyleID"><xsl:value-of select="$EditPriceStyle"/></xsl:attribute>
					<ss:Data ss:Type="Number"><xsl:value-of select="@LastPrice"/></ss:Data>
				</ss:Cell>
			</xsl:if>
			<ss:Cell ss:Index="11" ss:Formula="=YieldFromPrice(RC8, RC9, RC17, RC10)">
				<xsl:attribute name="ss:StyleID"><xsl:value-of select="$EditPercentStyle"/></xsl:attribute>
				<ss:Data ss:Type="Number">0</ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="12" ss:Formula="=Duration(RC8, RC9, RC17, RC11)">
				<xsl:attribute name="ss:StyleID"><xsl:value-of select="$PriceStyle"/></xsl:attribute>
				<ss:Data ss:Type="Number">0</ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="13" ss:Formula="=Convexity(RC8, RC9, RC17, RC11)">
				<xsl:attribute name="ss:StyleID"><xsl:value-of select="$PriceStyle"/></xsl:attribute>
				<ss:Data ss:Type="Number">0</ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="14">
				<xsl:choose>
					<xsl:when test="@TransactionTypeCode = 3 or @TransactionTypeCode = 4">
						<xsl:attribute name="ss:StyleID">SellQuantity</xsl:attribute>
					</xsl:when>
					<xsl:when test="@StatusCode = 3">
						<xsl:attribute name="ss:StyleID">ClosedQuantity</xsl:attribute>
					</xsl:when>
				</xsl:choose>
				<ss:Data ss:Type="Number"><xsl:value-of select="@QuantityOrdered"/></ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="15">
				<xsl:attribute name="ss:StyleID"><xsl:value-of select="$EditQuantityStyle"/></xsl:attribute>
				<ss:Data ss:Type="Number"><xsl:value-of select="@QuantityExecuted"/></ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="16">
				<xsl:attribute name="ss:StyleID"><xsl:value-of select="$DateStyle"/></xsl:attribute>
				<ss:Data ss:Type="DateTime"><xsl:value-of select="@TradeDate"/></ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="17">
				<xsl:attribute name="ss:StyleID"><xsl:value-of select="$EditDateStyle"/></xsl:attribute>
				<ss:Data ss:Type="DateTime"><xsl:value-of select="@SettlementDate"/></ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="18" ss:Formula="=DebtAccruedInterest(RC8, RC9, RC17)*RC14/100">
				<xsl:attribute name="ss:StyleID"><xsl:value-of select="$MarketValueStyle"/></xsl:attribute>
				<ss:Data ss:Type="Number">0</ss:Data>
			</ss:Cell>
			
		</ss:Row>
		
		<!-- Levels Subcategories -->
		
		<xsl:apply-templates/>

	</xsl:template>
	
</xsl:stylesheet>
