<?xml version="1.0" encoding="UTF-8" standalone="yes"?>

<!-- This stylesheet is used to turn flat, cartesian data from the database into a hierarchical dataset that can
be manipulated futher with XSL into a working document. -->
<xsl:stylesheet version="1.0" xmlns="urn:schemas-microsoft-com:office:spreadsheet"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
	xmlns:mts="urn:schemas-markthreesoftware-com:stylesheet">

	<!-- These parameters are used by the loader to specify the attributes of the stylesheet. -->
	<mts:stylesheetId>APPRAISAL</mts:stylesheetId>
	<mts:stylesheetTypeCode>APPRAISAL</mts:stylesheetTypeCode>
	<mts:name>Standard Appraisal</mts:name>

	<!-- This specified the attributes of the output of the XSL Transaformation. -->
	<xsl:output method="xml" encoding="UTF-8" standalone="yes" indent="yes"/>

	<!-- Externally provided parameters -->
	<xsl:param name="isSectorModel" select="0"/>
	<xsl:param name="isPositionModel" select="0"/>

	<!-- Match the Document root. -->
	
	<xsl:template match="Appraisal">

			<!-- Count up the number of sector, sub sector and individual line items in the hierarchical appraisal.  The
			value 'constantCount' is the number of fixed lines (heading, Grand total line, etc.), that don't change with
			the data. 'rowCount' is the final number of rows in the spreadsheet.  We need this to know where the grand total
			will go and where the spreadsheet boundaries are to construct the formulas.  -->
			<xsl:variable name="constantCount" select="4"/>
			<xsl:variable name="sector" select="count(.//Sector)"/>
			<xsl:variable name="debt" select="count(.//Debt)"/>
			<xsl:variable name="currency" select="count(.//Currency)"/>
			<xsl:variable name="equity" select="count(.//Equity)"/>
			<xsl:variable name="marketValueColumn" select="21"/>
			<xsl:variable name="rowCount" select="number($constantCount)+number($sector)*2+number($debt)+number($currency)+number($equity)"/>

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
					<ss:NamedRange ss:Name="sectorId" ss:RefersTo="=Sheet1!C2"/>
					<ss:NamedRange ss:Name="securityId" ss:RefersTo="=Sheet1!C3"/>
					<ss:NamedRange ss:Name="positionTypeCode" ss:RefersTo="=Sheet1!C4"/>
					<ss:NamedRange ss:Name="modelPercent" ss:RefersTo="=Sheet1!C5"/>
					<ss:NamedRange ss:Name="actualPercent" ss:RefersTo="=Sheet1!C6"/>
					<ss:NamedRange ss:Name="lastPrice" ss:RefersTo="=Sheet1!C13"/>
					<ss:NamedRange ss:Name="crossPrice" ss:RefersTo="=Sheet1!C14"/>
					<ss:NamedRange ss:Name="priceFactor" ss:RefersTo="=Sheet1!C15"/>
					<ss:NamedRange ss:Name="positionQuantity" ss:RefersTo="=Sheet1!C16"/>
					<ss:NamedRange ss:Name="proposedQuantity" ss:RefersTo="=Sheet1!C17"/>
					<ss:NamedRange ss:Name="orderedQuantity" ss:RefersTo="=Sheet1!C18"/>
					<ss:NamedRange ss:Name="allocatedQuantity" ss:RefersTo="=Sheet1!C19"/>
					<ss:NamedRange ss:Name="quantityFactor" ss:RefersTo="=Sheet1!C20"/>
					<ss:NamedRange ss:Name="marketValue" ss:RefersTo="=Sheet1!C21"/>
					<ss:NamedRange ss:Name="totalMarketValue">
						<xsl:attribute name="ss:RefersTo">=Sheet1!R<xsl:value-of select="$rowCount"/>C<xsl:value-of select="$marketValueColumn"/></xsl:attribute>
					</ss:NamedRange>
					<ss:NamedRange ss:Name="userData0" ss:RefersTo="=Sheet1!C22"/>
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
					<ss:Style ss:ID="CenterText" ss:Parent="Default">
						<ss:Alignment ss:Horizontal="Center" ss:Vertical="Center"/>
					</ss:Style>
					<ss:Style ss:ID="LeftText" ss:Parent="Default">
						<ss:Alignment ss:Horizontal="Left" ss:Vertical="Center"/>
					</ss:Style>
					<ss:Style ss:ID="RightText" ss:Parent="Default">
						<ss:Alignment ss:Horizontal="Right" ss:Vertical="Center"/>
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
					<ss:Style ss:ID="Last" ss:Parent="Default">
						<ss:Interior ss:Color="#D5CCBB" ss:Pattern="Solid"/>
						<ss:Borders>
						    <ss:Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>
						</ss:Borders>
					</ss:Style>
					<ss:Style ss:ID="LastCenterText" ss:Parent="HeaderCenterText">
						<ss:Borders>
						    <ss:Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>
							<ss:Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1"/>
						</ss:Borders>
					</ss:Style>
					<ss:Style ss:ID="LastLeftText" ss:Parent="HeaderLeftText">
						<ss:Borders>
						    <ss:Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>
							<ss:Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1" ss:Color="#000000"/>
						</ss:Borders>
					</ss:Style>
					<ss:Style ss:ID="LastRightText" ss:Parent="HeaderRightText">
						<ss:Borders>
						    <ss:Border ss:Position="Bottom" ss:LineStyle="Continuous" ss:Weight="1"/>
							<ss:Border ss:Position="Right" ss:LineStyle="Continuous" ss:Weight="1" ss:Color="#000000"/>
						</ss:Borders>
					</ss:Style>
					<Style ss:ID="ShortPosition">
						<ss:Font ss:FontName="Microsoft Sans Serif" ss:Size="8" ss:Color="#FF0000" ss:Bold="0"
						ss:Italic="0" ss:Underline="None" />
					</Style>
					<ss:Style ss:ID="Price">
						<ss:NumberFormat ss:Format="#,##0.00"/>
					</ss:Style>
					<ss:Style ss:ID="Percent">
						<ss:NumberFormat ss:Format="0.0%"/>
					</ss:Style>
					<ss:Style ss:ID="EditPercent">
						<ss:NumberFormat ss:Format="0.0%"/>
						<ss:Protection ss:Protected="0"/>
						<ss:Interior ss:Color="#E0E0E0" ss:Pattern="Solid"/>
					</ss:Style>
					<ss:Style ss:ID="Quantity">
						<ss:NumberFormat ss:Format="#,##0_);[Red]\(#,##0\)"/>
					</ss:Style>
					<ss:Style ss:ID="EditQuantity">
						<ss:NumberFormat ss:Format="#,##0_);[Red]\(#,##0\)"/>
						<ss:Protection ss:Protected="0"/>
						<ss:Interior ss:Color="#E0E0E0" ss:Pattern="Solid"/>
					</ss:Style>
					<ss:Style ss:ID="MarketValue">
						<ss:NumberFormat ss:Format="#,##0"/>
					</ss:Style>
					<ss:Style ss:ID="BoldMarketValue">
						<ss:Font ss:Bold="1"/>
						<ss:NumberFormat ss:Format="#,##0"/>
					</ss:Style>
					<ss:Style ss:ID="Sector">
						<ss:Alignment ss:Vertical="Center"/>
						<ss:Font ss:size="8" ss:Bold="1"/>
					</ss:Style>
					<ss:Style ss:ID="Subtotal">
						<ss:Font ss:size="8" ss:Bold="1"/>
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
						<x:FrozenNoSplit/>
						<x:TopRowVisible>0</x:TopRowVisible>
						<x:TopRowBottomPane>2</x:TopRowBottomPane>
						<x:SplitHorizontal>3</x:SplitHorizontal>
						<x:ActivePane>2</x:ActivePane>
					</x:WorksheetOptions>

					<!-- Table: The meat of the spreadsheet -->
					<ss:Table>

						<!-- Column Definitions -->
						<ss:Column ss:Index="1" ss:Width="28" ss:AutoFitWidth="0" ss:Hidden="1"/>
						<ss:Column ss:Index="2" ss:Width="28" ss:AutoFitWidth="0" ss:Hidden="1"/>
						<ss:Column ss:Index="3" ss:Width="28" ss:AutoFitWidth="0" ss:Hidden="1"/>
						<ss:Column ss:Index="4" ss:Width="28" ss:AutoFitWidth="0" ss:Hidden="1"/>
						<ss:Column ss:Index="5" ss:Width="28" ss:AutoFitWidth="0" ss:StyleID="Percent"/>
						<ss:Column ss:Index="6" ss:Width="28" ss:AutoFitWidth="0" ss:StyleID="Percent"/>
						<ss:Column ss:Index="7" ss:Width="5" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="8" ss:Width="5" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="9" ss:Width="5" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="10" ss:Width="5" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="11" ss:Width="150" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="12" ss:Width="33" ss:AutoFitWidth="0" ss:StyleID="Price"/>
						<ss:Column ss:Index="13" ss:Width="33" ss:AutoFitWidth="0" ss:StyleID="Price"/>
						<ss:Column ss:Index="14" ss:Width="33" ss:AutoFitWidth="0" ss:Hidden="1"/>
						<ss:Column ss:Index="15" ss:Width="33" ss:AutoFitWidth="0" ss:Hidden="1"/>
						<ss:Column ss:Index="16" ss:Width="48" ss:AutoFitWidth="0" ss:StyleID="Quantity"/>
						<ss:Column ss:Index="17" ss:Width="48" ss:AutoFitWidth="0" ss:StyleID="Quantity"/>
						<ss:Column ss:Index="18" ss:Width="48" ss:AutoFitWidth="0" ss:StyleID="Quantity"/>
						<ss:Column ss:Index="19" ss:Width="48" ss:AutoFitWidth="0" ss:StyleID="Quantity"/>
						<ss:Column ss:Index="20" ss:Width="32" ss:AutoFitWidth="0" ss:Hidden="1"/>
						<ss:Column ss:Index="21" ss:Width="65" ss:AutoFitWidth="0" ss:StyleID="MarketValue"/>
						<ss:Column ss:Index="22" ss:Width="48" ss:AutoFitWidth="0" ss:StyleID="Quantity"/>
						<ss:Column ss:Index="23" ss:Width="30" ss:AutoFitWidth="0" ss:StyleID="Percent"/>

						<!-- This column needs to be included to guarantee that the last readable column can be scrolled 
						into the viewer.  The column width should be equal to the largest column in the report. -->
						<ss:Column ss:Index="24" ss:Width="150" ss:AutoFitWidth="0"/>

						<!-- The Column Headings -->
						<ss:Row ss:Height="14" ss:StyleID="Last">
							<ss:Cell ss:Index="5" ss:StyleID="LastLeftText">
								<ss:Data ss:Type="String">Provider: Measurisk</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="12" ss:StyleID="LastLeftText">
								<ss:Data ss:Type="String">Risk Level: 95.0% Horizon: 1 Month</ss:Data>
							</ss:Cell>
						</ss:Row>
						<ss:Row ss:Height="12" ss:StyleID="Header">
							<ss:Cell ss:Index="5" ss:StyleID="HeaderLeftText">
								<ss:Data ss:Type="String">% Port</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="6" ss:StyleID="HeaderLeftText">
								<ss:Data ss:Type="String">% Port</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="11" ss:StyleID="HeaderLeftText">
								<ss:Data ss:Type="String">Security</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="12" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Average</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="13" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Jeff</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="16" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Position</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="17" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Proposed</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="18" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Ordered</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="19" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Allocated</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="21" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Market</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="22" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Value at</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="23" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">% at</ss:Data>
							</ss:Cell>
						</ss:Row>
						<ss:Row ss:Height="12" ss:StyleID="Header">
							<ss:Cell ss:Index="5" ss:StyleID="HeaderLeftText">
								<ss:Data ss:Type="String">Target</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="6" ss:StyleID="HeaderLeftText">
								<ss:Data ss:Type="String">Actual</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="11" ss:StyleID="HeaderLeftText">
								<ss:Data ss:Type="String">Name</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="12" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Cost</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="13" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="14" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="15" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="16" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="17" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="18" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="19" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="20" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="21" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Value</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="22" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Risk</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="23" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Risk</ss:Data>
							</ss:Cell>
						</ss:Row>

						<!-- The Main Body of the Spreadsheet -->
						
						<xsl:apply-templates/>
						
						<!-- The Grand Total -->
						<xsl:if test="count(*) > 0">
							<ss:Row ss:Height="12">
								<xsl:variable name="subtotalCount" select="number($sector)*2+number($debt)+number($currency)+number($equity)"/>
								<ss:Cell ss:Index="5">
									<xsl:attribute name="ss:Formula">=Subtotal(9, R[-1]C:R[<xsl:value-of select="-number($subtotalCount)"/>]C)</xsl:attribute>
								</ss:Cell>
								<ss:Cell ss:Index="7" ss:StyleID="Subtotal"><ss:Data ss:Type="String">Total Market Value</ss:Data></ss:Cell>
								<ss:Cell ss:Index="21" ss:StyleID="BoldMarketValue">
									<xsl:attribute name="ss:Formula">=Subtotal(9, R[-1]C:R[<xsl:value-of select="-number($subtotalCount)"/>]C)</xsl:attribute>
								</ss:Cell>
								<xsl:if test="@UserData0">
									<ss:Cell ss:Index="22"><ss:Data ss:Type="Number"><xsl:value-of select="@UserData0"/></ss:Data></ss:Cell>
									<ss:Cell ss:Index="23" ss:Formula="=userData0/marketValue"><ss:Data ss:Type="Number">0</ss:Data></ss:Cell>
								</xsl:if>
							</ss:Row>
						</xsl:if>

						<!-- This row needs to be included as a buffer.  It allows the last line to be scrolled into the
						viewer.  The height of this line should be the same as the height of the largest row in the
						regular report. -->
						<ss:Row ss:Height="17.25"/>
						
					</ss:Table>
				</ss:Worksheet>
			</ss:Workbook>

	</xsl:template>

	<!-- Levels Stylesheet -->
	<!-- Appraisals are managed hierarchially, according the the categories associated with the hierarchy.  It can be though of as an outline format
	where any sector in the document can have any number of subsector.  This stylesheet will recursively calculate the subtotals for all the sector
	that appear below it.  Security lines also are generated from this stylesheet.  At any time, a security can appear at the same sector as another
	outline sector.  The sub-sector will appear first when this happens. -->
	<xsl:template match="Sector">

		<!-- These variables are used to count up subtotals. -->
		<xsl:variable name="sector" select="count(.//Sector)"/>
		<xsl:variable name="debt" select="count(.//Debt)"/>
		<xsl:variable name="currency" select="count(.//Currency)"/>
		<xsl:variable name="equity" select="count(.//Equity)"/>
		<xsl:variable name="subtotalCount" select="number($sector)*2+number($debt)+number($currency)+number($equity)"/>
		<xsl:variable name="marketValueColumn" select="21"/>
		<xsl:variable name="depth" select="count(ancestor::*)"/>
		<xsl:variable name="index" select="6+number($depth)"/>

		<!-- Levels Heading -->
		
		<ss:Row ss:Height="17.25">
			<ss:Cell ss:Index="1"><ss:Data ss:Type="Number">1</ss:Data></ss:Cell>
			<ss:Cell ss:Index="2"><ss:Data ss:Type="Number"><xsl:value-of select="@SectorId"/></ss:Data></ss:Cell>
			<xsl:if test="$isSectorModel = 1 and number($depth)=1">
				<ss:Cell ss:Index="5">
					<xsl:attribute name="ss:StyleID">EditPercent</xsl:attribute>
					<xsl:if test="@ModelPercent">
						<ss:Data ss:Type="Number"><xsl:value-of select="@ModelPercent"/></ss:Data>
					</xsl:if>
				</ss:Cell>
			</xsl:if>
			<ss:Cell ss:Index="6">
				<xsl:attribute name="ss:Formula">=R[<xsl:value-of select="1+number($subtotalCount)"/>]C<xsl:value-of select="$marketValueColumn"/>/totalMarketValue</xsl:attribute>
			</ss:Cell>
			<ss:Cell ss:StyleID="Sector">
				<xsl:attribute name="ss:Index"><xsl:value-of select="$index"/></xsl:attribute>
				<ss:Data ss:Type="String"><xsl:value-of select="@Name"/></ss:Data>
			</ss:Cell>
		</ss:Row>
		
		<!-- SubSector and Security -->
		
		<xsl:apply-templates/>

		<!-- Levels Subtotal:  Hidden when there is only one sub-sector or position.  This supresses the visually 
		disturbing effect of having a subtotal line equal to the line above it. -->
	
		<ss:Row ss:Height="17.25">
			<xsl:attribute name="ss:Hidden">
				<xsl:choose>
					<xsl:when test="count(*) = 1">1</xsl:when>
					<xsl:otherwise>0</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
			<ss:Cell ss:Index="1"><ss:Data ss:Type="Number">3</ss:Data></ss:Cell>
			<ss:Cell ss:StyleID="Subtotal">
				<xsl:attribute name="ss:Index"><xsl:value-of select="$index"/></xsl:attribute>
				<xsl:if test="$sector = 0">
					<ss:Data ss:Type="String">Subtotal</ss:Data>
				</xsl:if>
				<xsl:if test="$sector != 0">
					<ss:Data ss:Type="String"><xsl:value-of select="@SectorName"/> Subtotal</ss:Data>
				</xsl:if>
			</ss:Cell>
			<ss:Cell ss:Index="21" ss:StyleID="BoldMarketValue">
				<xsl:attribute name="ss:Formula">=Subtotal(9, R[-1]C:R[<xsl:value-of select="-number($subtotalCount)"/>]C)</xsl:attribute>
			</ss:Cell>
		</ss:Row>
	</xsl:template>

	<!-- Debt Stylesheet -->
	<xsl:template match="Debt">
	
		<!-- These variables are used to count up subtotals. -->
		<xsl:variable name="taxLotCost" select="sum(.//Account/@TaxLotCost)"/>
		<xsl:variable name="taxLotQuantity" select="sum(.//Account/@TaxLotQuantity)"/>
		<xsl:variable name="proposedOrderQuantity" select="sum(.//Account/@ProposedOrderQuantity)"/>
		<xsl:variable name="orderQuantity" select="sum(.//Account/@OrderQuantity)"/>
		<xsl:variable name="allocationQuantity" select="sum(.//Account/@AllocationQuantity)"/>
		<xsl:variable name="subtotalRow" select="1+count(../*)-position()"/>

		<ss:Row ss:Height="12">
			<ss:Cell ss:Index="1"><ss:Data ss:Type="Number">2</ss:Data></ss:Cell>
			<ss:Cell ss:Index="3"><ss:Data ss:Type="Number"><xsl:value-of select="@SecurityId"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="4"><ss:Data ss:Type="Number"><xsl:value-of select="@PositionTypeCode"/></ss:Data></ss:Cell>
			<xsl:if test="$isPositionModel = 1">
				<ss:Cell ss:Index="5">
					<xsl:attribute name="ss:StyleID">EditPercent</xsl:attribute>
					<xsl:if test="@ModelPercent">
						<ss:Data ss:Type="Number"><xsl:value-of select="@ModelPercent"/></ss:Data>
					</xsl:if>
				</ss:Cell>
			</xsl:if>
			<ss:Cell ss:Index="6">
				<xsl:attribute name="ss:Formula">=marketValue/totalMarketValue</xsl:attribute>
			</ss:Cell>
			<ss:Cell ss:Index="11">
				<xsl:if test="@PositionTypeCode=1">
					<xsl:attribute name="ss:StyleID">ShortPosition</xsl:attribute>
				</xsl:if>
				<ss:Data ss:Type="String"><xsl:value-of select="@Name"/></ss:Data>
			</ss:Cell>
			<xsl:if test="$taxLotQuantity!=0.0">
				<ss:Cell ss:Index="12">
						<ss:Data ss:Type="Number"><xsl:value-of select="number($taxLotCost) div number($taxLotQuantity)"/></ss:Data>
				</ss:Cell>
			</xsl:if>
			<ss:Cell ss:Index="13"><ss:Data ss:Type="Number"><xsl:value-of select="@Price"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="15"><ss:Data ss:Type="Number"><xsl:value-of select="@PriceFactor"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="16"><ss:Data ss:Type="Number"><xsl:value-of select="$taxLotQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="17" ss:StyleID="EditQuantity"><ss:Data ss:Type="Number"><xsl:value-of select="$proposedOrderQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="18"><ss:Data ss:Type="Number"><xsl:value-of select="$orderQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="19"><ss:Data ss:Type="Number"><xsl:value-of select="number($orderQuantity)-number($allocationQuantity)"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="20"><ss:Data ss:Type="Number"><xsl:value-of select="@QuantityFactor"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="21" ss:Formula="=lastPrice*priceFactor*(positionQuantity+proposedQuantity+orderedQuantity)*quantityFactor"><ss:Data ss:Type="Number">0</ss:Data></ss:Cell>
			<xsl:if test="@UserData0">
				<ss:Cell ss:Index="22"><ss:Data ss:Type="Number"><xsl:value-of select="@UserData0"/></ss:Data></ss:Cell>
				<ss:Cell ss:Index="23" ss:Formula="=userData0/(lastPrice*priceFactor*positionQuantity*quantityFactor)"><ss:Data ss:Type="Number">0</ss:Data></ss:Cell>
			</xsl:if>

		</ss:Row>
		
	</xsl:template>

	<!-- Currency Stylesheet -->
	<xsl:template match="Currency">
	
		<!-- These variables are used to count up subtotals. -->
		<xsl:variable name="taxLotCost" select="sum(.//Account/@TaxLotCost)"/>
		<xsl:variable name="taxLotQuantity" select="sum(.//Account/@TaxLotQuantity)"/>
		<xsl:variable name="proposedOrderQuantity" select="sum(.//Account/@ProposedOrderQuantity)"/>
		<xsl:variable name="orderQuantity" select="sum(.//Account/@OrderQuantity)"/>
		<xsl:variable name="allocationQuantity" select="sum(.//Account/@AllocationQuantity)"/>
		<xsl:variable name="subtotalRow" select="1+count(../*)-position()"/>

		<ss:Row ss:Height="12">
			<ss:Cell ss:Index="1"><ss:Data ss:Type="Number">2</ss:Data></ss:Cell>
			<ss:Cell ss:Index="3"><ss:Data ss:Type="Number"><xsl:value-of select="@SecurityId"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="4"><ss:Data ss:Type="Number"><xsl:value-of select="@PositionTypeCode"/></ss:Data></ss:Cell>
			<xsl:if test="$isPositionModel = 1">
				<ss:Cell ss:Index="5">
					<xsl:attribute name="ss:StyleID">EditPercent</xsl:attribute>
					<xsl:if test="@ModelPercent">
						<ss:Data ss:Type="Number"><xsl:value-of select="@ModelPercent"/></ss:Data>
					</xsl:if>
				</ss:Cell>
			</xsl:if>
			<ss:Cell ss:Index="6">
				<xsl:attribute name="ss:Formula">=marketValue/totalMarketValue</xsl:attribute>
			</ss:Cell>
			<ss:Cell ss:Index="11">
				<xsl:if test="@PositionTypeCode=1">
					<xsl:attribute name="ss:StyleID">ShortPosition</xsl:attribute>
				</xsl:if>
				<ss:Data ss:Type="String"><xsl:value-of select="@Name"/></ss:Data>
			</ss:Cell>
			<xsl:if test="$taxLotQuantity!=0.0">
				<ss:Cell ss:Index="12">
						<ss:Data ss:Type="Number"><xsl:value-of select="number($taxLotCost) div number($taxLotQuantity)"/></ss:Data>
				</ss:Cell>
			</xsl:if>
			<ss:Cell ss:Index="13"><ss:Data ss:Type="Number"><xsl:value-of select="@Price"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="16"><ss:Data ss:Type="Number"><xsl:value-of select="$taxLotQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="17" ss:StyleID="EditQuantity"><ss:Data ss:Type="Number"><xsl:value-of select="$proposedOrderQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="18"><ss:Data ss:Type="Number"><xsl:value-of select="number($orderQuantity)-number($allocationQuantity)"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="19"><ss:Data ss:Type="Number"><xsl:value-of select="$allocationQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="21" ss:Formula="=lastPrice*(positionQuantity+proposedQuantity+orderedQuantity)"><ss:Data ss:Type="Number">0</ss:Data></ss:Cell>
			<xsl:if test="@UserData0">
				<ss:Cell ss:Index="22"><ss:Data ss:Type="Number"><xsl:value-of select="@UserData0"/></ss:Data></ss:Cell>
				<ss:Cell ss:Index="23" ss:Formula="=userData0/(lastPrice*positionQuantity)"><ss:Data ss:Type="Number">0</ss:Data></ss:Cell>
			</xsl:if>

		</ss:Row>
		
	</xsl:template>

	<!-- Equity Stylesheet -->
	<xsl:template match="Equity">
	
		<!-- These variables are used to count up subtotals. -->
		<xsl:variable name="taxLotCost" select="sum(.//Account/@TaxLotCost)"/>
		<xsl:variable name="taxLotQuantity" select="sum(.//Account/@TaxLotQuantity)"/>
		<xsl:variable name="proposedOrderQuantity" select="sum(.//Account/@ProposedOrderQuantity)"/>
		<xsl:variable name="orderQuantity" select="sum(.//Account/@OrderQuantity)"/>
		<xsl:variable name="allocationQuantity" select="sum(.//Account/@AllocationQuantity)"/>
		<xsl:variable name="subtotalRow" select="1+count(../*)-position()"/>

		<ss:Row ss:Height="12">
			<ss:Cell ss:Index="1"><ss:Data ss:Type="Number">2</ss:Data></ss:Cell>
			<ss:Cell ss:Index="3"><ss:Data ss:Type="Number"><xsl:value-of select="@SecurityId"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="4"><ss:Data ss:Type="Number"><xsl:value-of select="@PositionTypeCode"/></ss:Data></ss:Cell>
			<xsl:if test="$isPositionModel = 1">
				<ss:Cell ss:Index="5">
					<xsl:attribute name="ss:StyleID">EditPercent</xsl:attribute>
					<xsl:if test="@ModelPercent">
						<ss:Data ss:Type="Number"><xsl:value-of select="@ModelPercent"/></ss:Data>
					</xsl:if>
				</ss:Cell>
			</xsl:if>
			<ss:Cell ss:Index="6">
				<xsl:attribute name="ss:Formula">=marketValue/totalMarketValue</xsl:attribute>
			</ss:Cell>
			<ss:Cell ss:Index="11">
				<xsl:if test="@PositionTypeCode=1">
					<xsl:attribute name="ss:StyleID">ShortPosition</xsl:attribute>
				</xsl:if>
				<ss:Data ss:Type="String"><xsl:value-of select="@Name"/></ss:Data>
			</ss:Cell>
			<xsl:if test="$taxLotQuantity!=0.0">
				<ss:Cell ss:Index="12">
						<ss:Data ss:Type="Number"><xsl:value-of select="number($taxLotCost) div number($taxLotQuantity)"/></ss:Data>
				</ss:Cell>
			</xsl:if>
			<ss:Cell ss:Index="13"><ss:Data ss:Type="Number"><xsl:value-of select="@Price"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="14"><ss:Data ss:Type="Number"><xsl:value-of select="@LastCrossPrice"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="16"><ss:Data ss:Type="Number"><xsl:value-of select="$taxLotQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="17" ss:StyleID="EditQuantity"><ss:Data ss:Type="Number"><xsl:value-of select="$proposedOrderQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="18"><ss:Data ss:Type="Number"><xsl:value-of select="number($orderQuantity)-number($allocationQuantity)"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="19"><ss:Data ss:Type="Number"><xsl:value-of select="$allocationQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="21" ss:Formula="=lastPrice*crossPrice*(positionQuantity+proposedQuantity+orderedQuantity)"><ss:Data ss:Type="Number">0</ss:Data></ss:Cell>
			<xsl:if test="@UserData0">
				<ss:Cell ss:Index="22"><ss:Data ss:Type="Number"><xsl:value-of select="@UserData0"/></ss:Data></ss:Cell>
				<ss:Cell ss:Index="23" ss:Formula="=userData0/(lastPrice*crossPrice*positionQuantity)"><ss:Data ss:Type="Number">0</ss:Data></ss:Cell>
			</xsl:if>
		</ss:Row>
		
	</xsl:template>
	
</xsl:stylesheet>
