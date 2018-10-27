<?xml version="1.0" encoding="UTF-8" standalone="yes"?>

	<!-- This template is used to turn flat, cartesian data from the database into a hierarchical dataset that can
	be manipulated futher with XSL into a working document. -->

	<xsl:stylesheet version="1.0" xmlns="urn:schemas-microsoft-com:office:spreadsheet"
		xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
		xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
		xmlns:stqh="urn:schemas-shadowtech-com:quasar:hierarchy">
	<xsl:output method="xml" encoding="UTF-8" standalone="yes" indent="yes"/>

	<!-- Externally provided parameters -->
	
	<xsl:param name="modelSectorFlag" select="0"/>
	<xsl:param name="modelSecurityFlag" select="0"/>

	<!-- Match the Document root. -->
	
	<xsl:template match="Appraisal">

			<!-- Count up the number of sectors, sub sectors and individual line items in the hierarchical appraisal.  The
			value 'constantCount' is the number of fixed lines (heading, Grand total line, etc.), that don't change with
			the data. 'rowCount' is the final number of rows in the spreadsheet.  We need this to know where the grand total
			will go and where the spreadsheet boundaries are to construct the formulas.  -->

			<xsl:variable name="constantCount" select="3"/>
			<xsl:variable name="sectors" select="count(.//Sector)"/>
			<xsl:variable name="bonds" select="count(.//Bond)"/>
			<xsl:variable name="currencies" select="count(.//Currency)"/>
			<xsl:variable name="equities" select="count(.//Equity)"/>
			<xsl:variable name="columnCount" select="22"/>
			<xsl:variable name="rowCount" select="number($constantCount)+number($sectors)*2+number($bonds)+number($currencies)+number($equities)"/>

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
					<ss:NamedRange ss:Name="securityId" ss:RefersTo="=Sheet1!C2"/>
					<ss:NamedRange ss:Name="sectorId" ss:RefersTo="=Sheet1!C2"/>
					<ss:NamedRange ss:Name="positionTypeCode" ss:RefersTo="=Sheet1!C3"/>
					<ss:NamedRange ss:Name="modelPercent" ss:RefersTo="=Sheet1!C5"/>
					<ss:NamedRange ss:Name="actualPercent" ss:RefersTo="=Sheet1!C6"/>
					<ss:NamedRange ss:Name="currentPrice" ss:RefersTo="=Sheet1!C14"/>
					<ss:NamedRange ss:Name="crossPrice" ss:RefersTo="=Sheet1!C15"/>
					<ss:NamedRange ss:Name="priceFactor" ss:RefersTo="=Sheet1!C16"/>
					<ss:NamedRange ss:Name="positionQuantity" ss:RefersTo="=Sheet1!C17"/>
					<ss:NamedRange ss:Name="proposedQuantity" ss:RefersTo="=Sheet1!C18"/>
					<ss:NamedRange ss:Name="orderedQuantity" ss:RefersTo="=Sheet1!C19"/>
					<ss:NamedRange ss:Name="allocatedQuantity" ss:RefersTo="=Sheet1!C20"/>
					<ss:NamedRange ss:Name="quantityFactor" ss:RefersTo="=Sheet1!C21"/>
					<ss:NamedRange ss:Name="marketValue" ss:RefersTo="=Sheet1!C22"/>
					<ss:NamedRange ss:Name="totalMarketValue">
						<xsl:attribute name="ss:RefersTo">=Sheet1!R<xsl:value-of select="$rowCount"/>C<xsl:value-of select="$columnCount"/></xsl:attribute>
					</ss:NamedRange>
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
					<Style ss:ID="ShortPosition">
						<ss:Font ss:FontName="Microsoft Sans Serif" ss:Size="8" ss:Color="#FF0000" ss:Bold="0"
						ss:Italic="0" ss:Underline="None" />
					</Style>
					<ss:Style ss:ID="Price">
						<ss:NumberFormat ss:Format="#,##0.00"/>
					</ss:Style>
					<ss:Style ss:ID="Percent">
						<ss:NumberFormat ss:Format="0.000%"/>
					</ss:Style>
					<ss:Style ss:ID="EditPercent">
						<ss:NumberFormat ss:Format="0.000%"/>
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
						<x:SplitHorizontal>2</x:SplitHorizontal>
						<x:ActivePane>2</x:ActivePane>
					</x:WorksheetOptions>

					<!-- Table: The meat of the spreadsheet -->

					<ss:Table>

						<!-- Column Definitions -->

						<ss:Column ss:Index="1" ss:Width="28" ss:AutoFitWidth="0" ss:Hidden="1"/>
						<ss:Column ss:Index="2" ss:Width="28" ss:AutoFitWidth="0" ss:Hidden="1"/>
						<ss:Column ss:Index="3" ss:Width="28" ss:AutoFitWidth="0" ss:Hidden="1"/>
						<ss:Column ss:Index="4" ss:Width="28" ss:AutoFitWidth="0" ss:Hidden="1"/>
						<ss:Column ss:Index="5" ss:Width="31" ss:AutoFitWidth="0" ss:StyleID="Percent"/>
						<ss:Column ss:Index="6" ss:Width="31" ss:AutoFitWidth="0" ss:StyleID="Percent"/>
						<ss:Column ss:Index="7" ss:Width="31" ss:AutoFitWidth="0" ss:StyleID="Percent"/>
						<ss:Column ss:Index="8" ss:Width="5" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="9" ss:Width="5" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="10" ss:Width="5" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="11" ss:Width="5" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="12" ss:Width="150" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="13" ss:Width="33" ss:AutoFitWidth="0" ss:StyleID="Price"/>
						<ss:Column ss:Index="14" ss:Width="33" ss:AutoFitWidth="0" ss:StyleID="Price"/>
						<ss:Column ss:Index="15" ss:Width="33" ss:AutoFitWidth="0" ss:Hidden="1"/>
						<ss:Column ss:Index="16" ss:Width="33" ss:AutoFitWidth="0" ss:Hidden="1"/>
						<ss:Column ss:Index="17" ss:Width="52" ss:AutoFitWidth="0" ss:StyleID="Quantity"/>
						<ss:Column ss:Index="18" ss:Width="52" ss:AutoFitWidth="0" ss:StyleID="Quantity"/>
						<ss:Column ss:Index="19" ss:Width="52" ss:AutoFitWidth="0" ss:StyleID="Quantity"/>
						<ss:Column ss:Index="20" ss:Width="52" ss:AutoFitWidth="0" ss:StyleID="Quantity"/>
						<ss:Column ss:Index="21" ss:Width="32" ss:AutoFitWidth="0" ss:Hidden="1"/>
						<ss:Column ss:Index="22" ss:Width="65" ss:AutoFitWidth="0" ss:StyleID="MarketValue"/>

						<!-- This column needs to be included to guarantee that the last readable column can be scrolled 
						into the viewer.  The column width should be equal to the largest column in the report. -->
						<ss:Column ss:Index="23" ss:Width="180" ss:AutoFitWidth="0"/>

						<!-- The Column Headings -->

						<ss:Row ss:Height="12" ss:StyleID="Header">
							<ss:Cell ss:Index="5" ss:StyleID="HeaderLeftText">
								<ss:Data ss:Type="String">% Port</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="6" ss:StyleID="HeaderLeftText">
								<ss:Data ss:Type="String">% Port</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="7" ss:StyleID="HeaderLeftText">
								<ss:Data ss:Type="String">% Bet</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="12" ss:StyleID="HeaderLeftText">
								<ss:Data ss:Type="String">Security</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="13" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Average</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="14" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Price</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="17" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Position</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="18" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Proposed</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="19" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Ordered</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="20" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Allocated</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="22" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Market</ss:Data>
							</ss:Cell>
						</ss:Row>
						<ss:Row ss:Height="12" ss:StyleID="Header">
							<ss:Cell ss:Index="5" ss:StyleID="HeaderLeftText">
								<ss:Data ss:Type="String">Target</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="6" ss:StyleID="HeaderLeftText">
								<ss:Data ss:Type="String">Actual</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="7" ss:StyleID="HeaderLeftText">
							</ss:Cell>
							<ss:Cell ss:Index="12" ss:StyleID="HeaderLeftText">
								<ss:Data ss:Type="String">Name</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="13" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Cost</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="14" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="15" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="16" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="17" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="18" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="19" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="20" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="21" ss:StyleID="HeaderLeftText" />
							<ss:Cell ss:Index="22" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Value</ss:Data>
							</ss:Cell>
						</ss:Row>

						<!-- The Main Body of the Spreadsheet -->
						
						<xsl:apply-templates/>
						
						<!-- The Grand Total -->

						<xsl:if test="count(*) > 0">
							<ss:Row ss:Height="12">
								<xsl:variable name="subtotalCount" select="number($sectors)*2+number($bonds)+number($currencies)+number($equities)"/>
								<ss:Cell ss:Index="5">
									<xsl:attribute name="ss:Formula">=Subtotal(9, R[-1]C:R[<xsl:value-of select="-number($subtotalCount)"/>]C)</xsl:attribute>
								</ss:Cell>
								<ss:Cell ss:Index="8" ss:StyleID="Subtotal"><ss:Data ss:Type="String">Total Market Value</ss:Data></ss:Cell>
								<ss:Cell ss:Index="22" ss:StyleID="BoldMarketValue">
									<xsl:attribute name="ss:Formula">=Subtotal(9, R[-1]C:R[<xsl:value-of select="-number($subtotalCount)"/>]C)</xsl:attribute>
								</ss:Cell>
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

	<!-- Levels Template -->
	<!-- Appraisals are managed hierarchially, according the the categories associated with the hierarchy.  It can be though of as an outline format
	where any sector in the document can have any number of subsectors.  This template will recursively calculate the subtotals for all the sectors
	that appear below it.  Security lines also are generated from this template.  At any time, a security can appear at the same sector as another
	outline sector.  The sub-sector will appear first when this happens. -->

	<xsl:template match="Sector">

		<!-- These variables are used to count up subtotals. -->

		<xsl:variable name="sectors" select="count(.//Sector)"/>
		<xsl:variable name="bonds" select="count(.//Bond)"/>
		<xsl:variable name="currencies" select="count(.//Currency)"/>
		<xsl:variable name="equities" select="count(.//Equity)"/>
		<xsl:variable name="subtotalCount" select="number($sectors)*2+number($bonds)+number($currencies)+number($equities)"/>
		<xsl:variable name="columnCount" select="21"/>
		<xsl:variable name="depth" select="count(ancestor::*)"/>
		<xsl:variable name="index" select="7+number($depth)"/>

		<!-- Levels Heading -->
		
		<ss:Row ss:Height="17.25">
			<ss:Cell ss:Index="1"><ss:Data ss:Type="Number">1</ss:Data></ss:Cell>
			<ss:Cell ss:Index="2"><ss:Data ss:Type="Number"><xsl:value-of select="@sectorId"/></ss:Data></ss:Cell>
			<xsl:if test="$modelSectorFlag = 1 and number($depth)=1">
				<ss:Cell ss:Index="5">
					<xsl:attribute name="ss:StyleID">EditPercent</xsl:attribute>
					<xsl:if test="@modelPercent">
						<ss:Data ss:Type="Number"><xsl:value-of select="@modelPercent"/></ss:Data>
					</xsl:if>
				</ss:Cell>
			</xsl:if>
			<ss:Cell ss:Index="6">
				<xsl:attribute name="ss:Formula">=R[<xsl:value-of select="1+number($subtotalCount)"/>]C<xsl:value-of select="$columnCount"/>/totalMarketValue</xsl:attribute>
			</ss:Cell>
			<ss:Cell ss:StyleID="Sector">
				<xsl:attribute name="ss:Index"><xsl:value-of select="$index"/></xsl:attribute>
				<ss:Data ss:Type="String"><xsl:value-of select="@name"/></ss:Data>
			</ss:Cell>
		</ss:Row>
		
		<!-- SubSectors and Securities -->
		
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
				<xsl:if test="$sectors = 0">
					<ss:Data ss:Type="String">Subtotal</ss:Data>
				</xsl:if>
				<xsl:if test="$sectors != 0">
					<ss:Data ss:Type="String"><xsl:value-of select="@sectorName"/> Subtotal</ss:Data>
				</xsl:if>
			</ss:Cell>
			<ss:Cell ss:Index="22" ss:StyleID="BoldMarketValue">
				<xsl:attribute name="ss:Formula">=Subtotal(9, R[-1]C:R[<xsl:value-of select="-number($subtotalCount)"/>]C)</xsl:attribute>
			</ss:Cell>
		</ss:Row>
	</xsl:template>

	<!-- Bond Template -->
	<xsl:template match="Bond">
	
		<!-- These variables are used to count up subtotals. -->

		<xsl:variable name="taxLotsCost" select="sum(.//Account/@taxLotsCost)"/>
		<xsl:variable name="taxLotsQuantity" select="sum(.//Account/@taxLotsQuantity)"/>
		<xsl:variable name="proposedOrdersQuantity" select="sum(.//Account/@proposedOrdersQuantity)"/>
		<xsl:variable name="ordersQuantity" select="sum(.//Account/@ordersQuantity)"/>
		<xsl:variable name="allocationsQuantity" select="sum(.//Account/@allocationsQuantity)"/>
		<xsl:variable name="subtotalRow" select="1+count(../*)-position()"/>

		<ss:Row ss:Height="12">
			<ss:Cell ss:Index="1"><ss:Data ss:Type="Number">2</ss:Data></ss:Cell>
			<ss:Cell ss:Index="2"><ss:Data ss:Type="Number"><xsl:value-of select="@securityId"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="3"><ss:Data ss:Type="Number"><xsl:value-of select="@positionTypeCode"/></ss:Data></ss:Cell>
			<xsl:if test="$modelSecurityFlag = 1">
				<ss:Cell ss:Index="5">
					<xsl:attribute name="ss:StyleID">EditPercent</xsl:attribute>
					<xsl:if test="@modelPercent">
						<ss:Data ss:Type="Number"><xsl:value-of select="@modelPercent"/></ss:Data>
					</xsl:if>
				</ss:Cell>
			</xsl:if>
			<ss:Cell ss:Index="6">
				<xsl:attribute name="ss:Formula">=marketValue/totalMarketValue</xsl:attribute>
			</ss:Cell>
			<ss:Cell ss:Index="12">
				<xsl:if test="@positionTypeCode=1">
					<xsl:attribute name="ss:StyleID">ShortPosition</xsl:attribute>
				</xsl:if>
				<ss:Data ss:Type="String"><xsl:value-of select="@name"/></ss:Data>
			</ss:Cell>
			<xsl:if test="$taxLotsQuantity!=0.0">
				<ss:Cell ss:Index="13">
						<ss:Data ss:Type="Number"><xsl:value-of select="number($taxLotsCost) div number($taxLotsQuantity)"/></ss:Data>
				</ss:Cell>
			</xsl:if>
			<ss:Cell ss:Index="14"><ss:Data ss:Type="Number"><xsl:value-of select="@price"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="16"><ss:Data ss:Type="Number"><xsl:value-of select="@priceFactor"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="17"><ss:Data ss:Type="Number"><xsl:value-of select="$taxLotsQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="18" ss:StyleID="EditQuantity"><ss:Data ss:Type="Number"><xsl:value-of select="$proposedOrdersQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="19"><ss:Data ss:Type="Number"><xsl:value-of select="$ordersQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="20"><ss:Data ss:Type="Number"><xsl:value-of select="$allocationsQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="21"><ss:Data ss:Type="Number"><xsl:value-of select="@quantityFactor"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="22" ss:Formula="=currentPrice*priceFactor*(positionQuantity+proposedQuantity+orderedQuantity+allocatedQuantity)*quantityFactor"><ss:Data ss:Type="Number">0</ss:Data></ss:Cell>

		</ss:Row>
		
	</xsl:template>

	<!-- Currency Template -->
	<xsl:template match="Currency">
	
		<!-- These variables are used to count up subtotals. -->

		<xsl:variable name="taxLotsCost" select="sum(.//Account/@taxLotsCost)"/>
		<xsl:variable name="taxLotsQuantity" select="sum(.//Account/@taxLotsQuantity)"/>
		<xsl:variable name="proposedOrdersQuantity" select="sum(.//Account/@proposedOrdersQuantity)"/>
		<xsl:variable name="ordersQuantity" select="sum(.//Account/@ordersQuantity)"/>
		<xsl:variable name="allocationsQuantity" select="sum(.//Account/@allocationsQuantity)"/>
		<xsl:variable name="subtotalRow" select="1+count(../*)-position()"/>

		<ss:Row ss:Height="12">
			<ss:Cell ss:Index="1"><ss:Data ss:Type="Number">2</ss:Data></ss:Cell>
			<ss:Cell ss:Index="2"><ss:Data ss:Type="Number"><xsl:value-of select="@securityId"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="3"><ss:Data ss:Type="Number"><xsl:value-of select="@positionTypeCode"/></ss:Data></ss:Cell>
			<xsl:if test="$modelSecurityFlag = 1">
				<ss:Cell ss:Index="5">
					<xsl:attribute name="ss:StyleID">EditPercent</xsl:attribute>
					<xsl:if test="@modelPercent">
						<ss:Data ss:Type="Number"><xsl:value-of select="@modelPercent"/></ss:Data>
					</xsl:if>
				</ss:Cell>
			</xsl:if>
			<ss:Cell ss:Index="6">
				<xsl:attribute name="ss:Formula">=marketValue/totalMarketValue</xsl:attribute>
			</ss:Cell>
			<ss:Cell ss:Index="12">
				<xsl:if test="@positionTypeCode=1">
					<xsl:attribute name="ss:StyleID">ShortPosition</xsl:attribute>
				</xsl:if>
				<ss:Data ss:Type="String"><xsl:value-of select="@name"/></ss:Data>
			</ss:Cell>
			<xsl:if test="$taxLotsQuantity!=0.0">
				<ss:Cell ss:Index="13">
						<ss:Data ss:Type="Number"><xsl:value-of select="number($taxLotsCost) div number($taxLotsQuantity)"/></ss:Data>
				</ss:Cell>
			</xsl:if>
			<ss:Cell ss:Index="14"><ss:Data ss:Type="Number"><xsl:value-of select="@price"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="17"><ss:Data ss:Type="Number"><xsl:value-of select="$taxLotsQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="18" ss:StyleID="EditQuantity"><ss:Data ss:Type="Number"><xsl:value-of select="$proposedOrdersQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="19"><ss:Data ss:Type="Number"><xsl:value-of select="$ordersQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="20"><ss:Data ss:Type="Number"><xsl:value-of select="$allocationsQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="22" ss:Formula="=currentPrice*(positionQuantity+proposedQuantity+orderedQuantity+allocatedQuantity)"><ss:Data ss:Type="Number">0</ss:Data></ss:Cell>

		</ss:Row>
		
	</xsl:template>

	<!-- Equity Template -->
	<xsl:template match="Equity">
	
		<!-- These variables are used to count up subtotals. -->

		<xsl:variable name="taxLotsCost" select="sum(.//Account/@taxLotsCost)"/>
		<xsl:variable name="taxLotsQuantity" select="sum(.//Account/@taxLotsQuantity)"/>
		<xsl:variable name="proposedOrdersQuantity" select="sum(.//Account/@proposedOrdersQuantity)"/>
		<xsl:variable name="ordersQuantity" select="sum(.//Account/@ordersQuantity)"/>
		<xsl:variable name="allocationsQuantity" select="sum(.//Account/@allocationsQuantity)"/>
		<xsl:variable name="subtotalRow" select="1+count(../*)-position()"/>

		<ss:Row ss:Height="12">
			<ss:Cell ss:Index="1"><ss:Data ss:Type="Number">2</ss:Data></ss:Cell>
			<ss:Cell ss:Index="2"><ss:Data ss:Type="Number"><xsl:value-of select="@securityId"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="3"><ss:Data ss:Type="Number"><xsl:value-of select="@positionTypeCode"/></ss:Data></ss:Cell>
			<xsl:if test="$modelSecurityFlag = 1">
				<ss:Cell ss:Index="5">
					<xsl:attribute name="ss:StyleID">EditPercent</xsl:attribute>
					<xsl:if test="@modelPercent">
						<ss:Data ss:Type="Number"><xsl:value-of select="@modelPercent"/></ss:Data>
					</xsl:if>
				</ss:Cell>
			</xsl:if>
			<ss:Cell ss:Index="6">
				<xsl:attribute name="ss:Formula">=marketValue/totalMarketValue</xsl:attribute>
			</ss:Cell>
			<ss:Cell ss:Index="7" ss:Formula="=actualPercent-modelPercent"><ss:Data ss:Type="Number">0</ss:Data></ss:Cell>
			<ss:Cell ss:Index="12">
				<xsl:if test="@positionTypeCode=1">
					<xsl:attribute name="ss:StyleID">ShortPosition</xsl:attribute>
				</xsl:if>
				<ss:Data ss:Type="String"><xsl:value-of select="@name"/></ss:Data>
			</ss:Cell>
			<xsl:if test="$taxLotsQuantity!=0.0">
				<ss:Cell ss:Index="13">
						<ss:Data ss:Type="Number"><xsl:value-of select="number($taxLotsCost) div number($taxLotsQuantity)"/></ss:Data>
				</ss:Cell>
			</xsl:if>
			<ss:Cell ss:Index="14"><ss:Data ss:Type="Number"><xsl:value-of select="@price"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="15"><ss:Data ss:Type="Number"><xsl:value-of select="@lastCrossPrice"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="17"><ss:Data ss:Type="Number"><xsl:value-of select="$taxLotsQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="18" ss:StyleID="EditQuantity"><ss:Data ss:Type="Number"><xsl:value-of select="$proposedOrdersQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="19"><ss:Data ss:Type="Number"><xsl:value-of select="$ordersQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="20"><ss:Data ss:Type="Number"><xsl:value-of select="$allocationsQuantity"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="22" ss:Formula="=currentPrice*crossPrice*(positionQuantity+proposedQuantity+orderedQuantity+allocatedQuantity)"><ss:Data ss:Type="Number">0</ss:Data></ss:Cell>

		</ss:Row>
		
	</xsl:template>
	
</xsl:stylesheet>
