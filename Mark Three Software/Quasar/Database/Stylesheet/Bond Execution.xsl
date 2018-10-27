<?xml version="1.0" encoding="UTF-8" standalone="yes"?>

<!-- This stylesheet is used to turn flat, cartesian data from the database into a hierarchical dataset that can
be manipulated futher with XSL into a working document. -->
<xsl:stylesheet version="1.0" xmlns="urn:schemas-microsoft-com:office:spreadsheet"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
	xmlns:mts="urn:schemas-markthreesoftware-com:stylesheet">

	<!-- These parameters are used by the loader to specify the attributes of the stylesheet. -->
 	<mts:stylesheetId>BOND EXECUTION</mts:stylesheetId>
	<mts:stylesheetTypeCode>EXECUTION</mts:stylesheetTypeCode>
	<mts:name>Bond Execution</mts:name>

	<!-- This specified the attributes of the output of the XSL Transaformation. -->
	<xsl:output method="xml" encoding="UTF-8" standalone="yes" indent="yes"/>

	<!-- Match the Document root. -->
	<xsl:template match="Execution">

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
					<ss:NamedRange ss:Name="executionId" ss:RefersTo="=Sheet1!C2"/>
					<ss:NamedRange ss:Name="brokerSymbol" ss:RefersTo="=Sheet1!C5"/>
					<ss:NamedRange ss:Name="quantity" ss:RefersTo="=Sheet1!C6"/>
					<ss:NamedRange ss:Name="price" ss:RefersTo="=Sheet1!C7"/>
					<ss:NamedRange ss:Name="commission" ss:RefersTo="=Sheet1!C9"/>
					<ss:NamedRange ss:Name="userFee0" ss:RefersTo="=Sheet1!C10"/>
					<ss:NamedRange ss:Name="tradeDate" ss:RefersTo="=Sheet1!C11"/>
					<ss:NamedRange ss:Name="settlementDate" ss:RefersTo="=Sheet1!C12"/>
					<ss:NamedRange ss:Name="createdTime" ss:RefersTo="=Sheet1!C14"/>
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
						<ss:NumberFormat ss:Format="#,##0.00"/>
						<ss:Alignment ss:Horizontal="Right"/>
					</ss:Style>
					<ss:Style ss:ID="Quantity">
						<ss:NumberFormat ss:Format="#,##0_);[Red]\(#,##0\)"/>
						<ss:Alignment ss:Horizontal="Right"/>
					</ss:Style>
					<ss:Style ss:ID="MarketValue">
						<ss:NumberFormat ss:Format="#,##0.00;[Red]\(#,##0.00\);"/>
						<ss:Alignment ss:Horizontal="Right"/>
					</ss:Style>
					<ss:Style ss:ID="Percent">
						<ss:NumberFormat ss:Format="0.000%;"/>
					</ss:Style>
					<ss:Style ss:ID="ShortDate">
						<ss:NumberFormat ss:Format="m/d;@"/>
					</ss:Style>
					<ss:Style ss:ID="ShortDateTime">
						<ss:NumberFormat ss:Format="d/m hh:mm"/>
					</ss:Style>
					<ss:Style ss:ID="EditLeftText" ss:Parent="LeftText">
						<ss:Protection ss:Protected="0"/>
					</ss:Style>
					<ss:Style ss:ID="EditPrice" ss:Parent="Price">
						<ss:Protection ss:Protected="0"/>
					</ss:Style>
					<ss:Style ss:ID="EditQuantity" ss:Parent="Quantity">
						<ss:Protection ss:Protected="0"/>
					</ss:Style>
					<ss:Style ss:ID="EditShortDate" ss:Parent="ShortDate">
						<ss:Protection ss:Protected="0"/>
					</ss:Style>
					<ss:Style ss:ID="GrayEditLeftText" ss:Parent="EditLeftText">
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
						<x:SplitHorizontal>1</x:SplitHorizontal>
						<x:TopRowVisible>0</x:TopRowVisible>
						<x:TopRowBottomPane>1</x:TopRowBottomPane>
						<x:SplitVertical>3</x:SplitVertical>
						<x:LeftColumnVisible>0</x:LeftColumnVisible>
						<x:LeftColumnRightPane>3</x:LeftColumnRightPane>
					</x:WorksheetOptions>

					<!-- Table: The meat of the spreadsheet -->
					<ss:Table>

						<!-- Column Definitions -->
						<ss:Column ss:Index="1" ss:Width="28" ss:Hidden="1" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="2" ss:Width="28" ss:Hidden="1" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="3" ss:Width="28" ss:Hidden="1" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="4" ss:Width="28" ss:Hidden="1" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="5" ss:Width="37.5" ss:StyleID="LeftText" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="6" ss:Width="49.5" ss:StyleID="Quantity" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="7" ss:Width="42" ss:StyleID="Price" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="8" ss:Width="42" ss:StyleID="Price" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="9" ss:Width="45" ss:StyleID="Price" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="10" ss:Width="45" ss:StyleID="Price" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="11" ss:Width="28" ss:StyleID="ShortDate" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="12" ss:Width="28" ss:StyleID="ShortDate" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="13" ss:Width="55" ss:StyleID="MarketValue" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="14" ss:Width="45" ss:StyleID="ShortDateTime" ss:AutoFitWidth="0"/>

						<!-- This column needs to be included to guarantee that the last readable column can be scrolled 
						into the viewer.  The column width should be equal to the largest column in the report. -->
						<ss:Column ss:Index="15" ss:Width="49.5" ss:AutoFitWidth="0"/>

						<!-- The Column Headings -->
						<ss:Row ss:Height="12" ss:StyleID="Header" >
							<ss:Cell ss:Index="5" ss:StyleID="HeaderLeftText">
								<ss:Data ss:Type="String">Broker</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="6" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Quantity</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="7" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Price</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="8" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Yield</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="9" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Comm.</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="10" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Accr'd</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="11" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Trade</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="12" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Settle</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="13" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Net</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="14" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Timestamp</ss:Data>
							</ss:Cell>
						</ss:Row>

						<!-- The Main Body of the Spreadsheet -->
						
						<xsl:apply-templates />
						
						<!-- This row needs to be included as a buffer.  It allows the last line to be scrolled into the
						viewer.  The height of this line should be the same as the height of the largest row in the
						regular report. -->
						<ss:Row ss:Height="12"/>

					</ss:Table>
				</ss:Worksheet>
			</ss:Workbook>

	</xsl:template>
	
	<!-- Broker -->
	<xsl:template match="Broker">
		<xsl:apply-templates />
	</xsl:template>
	
	<!-- Execution -->
	<xsl:template match="GlobalExecution">

		<!-- Execution Row -->
		<ss:Row ss:Height="12">
			
			<ss:Cell ss:Index="1"><ss:Data ss:Type="Number">1</ss:Data></ss:Cell>
			<ss:Cell ss:Index="2"><ss:Data ss:Type="Number"><xsl:value-of select="@ExecutionId"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="3"><ss:Data ss:Type="Number"><xsl:value-of select="../@Coupon"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="4"><ss:Data ss:Type="DateTime"><xsl:value-of select="../@MaturityDate"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="5">
				<xsl:attribute name="ss:StyleID">EditLeftText</xsl:attribute>
				<ss:Data ss:Type="String"><xsl:value-of select="@BrokerSymbol"/></ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="6">
				<xsl:attribute name="ss:StyleID">EditQuantity</xsl:attribute>
				<ss:Data ss:Type="Number"><xsl:value-of select="@Quantity"/></ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="7">
				<xsl:attribute name="ss:StyleID">EditPrice</xsl:attribute>
				<ss:Data ss:Type="Number"><xsl:value-of select="@Price"/></ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="8">
				<xsl:attribute name="ss:Formula">=YieldFromPrice(RC3, RC4, RC12, RC7)</xsl:attribute>
				<xsl:attribute name="ss:StyleID">Percent</xsl:attribute>
				<ss:Data ss:Type="Number">0</ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="9" ss:Formula="=DebtCommission(RC6)">
				<xsl:attribute name="ss:StyleID">EditPrice</xsl:attribute>
				<ss:Data ss:Type="Number">0</ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="10">
				<xsl:attribute name="ss:Formula">=DebtAccruedInterest(RC3, RC4, RC12)*RC6/100</xsl:attribute>
				<xsl:attribute name="ss:StyleID">MarketValue</xsl:attribute>
				<ss:Data ss:Type="Number">0</ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="11">
				<xsl:attribute name="ss:StyleID">EditShortDate</xsl:attribute>
				<ss:Data ss:Type="DateTime"><xsl:value-of select="@TradeDate"/></ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="12">
				<xsl:attribute name="ss:StyleID">EditShortDate</xsl:attribute>
				<ss:Data ss:Type="DateTime"><xsl:value-of select="@SettlementDate"/></ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="13">
				<xsl:choose>
					<xsl:when test="../@TransactionTypeCode=3">
						<xsl:attribute name="ss:Formula">=RC6*RC7*0.01-RC9-RC10</xsl:attribute>
					</xsl:when>
					<xsl:otherwise>
						<xsl:attribute name="ss:Formula">=RC6*RC7*0.01+RC9+RC10</xsl:attribute>
					</xsl:otherwise>
				</xsl:choose>
				<xsl:attribute name="ss:StyleID">MarketValue</xsl:attribute>
				<ss:Data ss:Type="Number">0</ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="14">
				<xsl:if test="@CreatedTime">
					<ss:Data ss:Type="DateTime"><xsl:value-of select="@CreatedTime"/></ss:Data>
				</xsl:if>
			</ss:Cell>
		</ss:Row>

	</xsl:template>
	
	<!-- Partial Execution -->
	<xsl:template match="LocalExecution">

		<!-- Execution Row -->
		<ss:Row ss:Height="12">
			
			<ss:Cell ss:Index="1"><ss:Data ss:Type="Number">2</ss:Data></ss:Cell>
			<ss:Cell ss:Index="2"><ss:Data ss:Type="Number"><xsl:value-of select="@ExecutionId"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="3"><ss:Data ss:Type="Number"><xsl:value-of select="../@Coupon"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="4"><ss:Data ss:Type="DateTime"><xsl:value-of select="../@MaturityDate"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="5">
				<xsl:attribute name="ss:StyleID">EditLeftText</xsl:attribute>
				<xsl:if test="@BrokerSymbol">
					<ss:Data ss:Type="String"><xsl:value-of select="@BrokerSymbol"/></ss:Data>
				</xsl:if>
			</ss:Cell>
			<ss:Cell ss:Index="6">
				<xsl:attribute name="ss:StyleID">EditQuantity</xsl:attribute>
				<ss:Data ss:Type="Number"><xsl:value-of select="@Quantity"/></ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="7">
				<xsl:attribute name="ss:StyleID">EditPrice</xsl:attribute>
				<ss:Data ss:Type="Number"><xsl:value-of select="@Price"/></ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="8">
				<xsl:attribute name="ss:Formula">=YieldFromPrice(RC3, RC4, RC12, RC7)</xsl:attribute>
				<xsl:attribute name="ss:StyleID">Percent</xsl:attribute>
				<ss:Data ss:Type="Number">0</ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="9" ss:Formula="=DebtCommission(RC6)">
				<xsl:attribute name="ss:StyleID">EditPrice</xsl:attribute>
				<ss:Data ss:Type="Number">0</ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="10">
				<xsl:attribute name="ss:Formula">=DebtAccruedInterest(RC3, RC4, RC12)*RC6/100</xsl:attribute>
				<xsl:attribute name="ss:StyleID">MarketValue</xsl:attribute>
				<ss:Data ss:Type="Number">0</ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="11">
				<xsl:attribute name="ss:StyleID">EditShortDate</xsl:attribute>
				<ss:Data ss:Type="DateTime"><xsl:value-of select="@TradeDate"/></ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="12">
				<xsl:attribute name="ss:StyleID">EditShortDate</xsl:attribute>
				<ss:Data ss:Type="DateTime"><xsl:value-of select="@SettlementDate"/></ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="13">
				<xsl:choose>
					<xsl:when test="../../@TransactionTypeCode=3">
						<xsl:attribute name="ss:Formula">=RC6*RC7*0.01-RC9-RC10</xsl:attribute>
					</xsl:when>
					<xsl:otherwise>
						<xsl:attribute name="ss:Formula">=RC6*RC7*0.01+RC9+RC10</xsl:attribute>
					</xsl:otherwise>
				</xsl:choose>
				<xsl:attribute name="ss:StyleID">MarketValue</xsl:attribute>
				<ss:Data ss:Type="Number">0</ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="14">
				<xsl:if test="@CreatedTime">
					<ss:Data ss:Type="DateTime"><xsl:value-of select="@CreatedTime"/></ss:Data>
				</xsl:if>
			</ss:Cell>
		</ss:Row>

	</xsl:template>

	<!-- A Blank line for new execution -->
	<xsl:template match="Placeholder">

		<!-- Execution Row -->
		<ss:Row ss:Height="12" >
			
			<ss:Cell ss:Index="1"><ss:Data ss:Type="Number">3</ss:Data></ss:Cell>
			<ss:Cell ss:Index="5" ss:StyleID="GrayEditLeftText" >
				<ss:Data ss:Type="String">Click here to add a new Execution</ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="6" ss:StyleID="EditLeftText" />
			<ss:Cell ss:Index="7" ss:StyleID="EditLeftText" />
			<ss:Cell ss:Index="9" ss:StyleID="EditLeftText" />
			<ss:Cell ss:Index="10" ss:StyleID="EditLeftText" />
			<ss:Cell ss:Index="11" ss:StyleID="EditLeftText" />
			<ss:Cell ss:Index="12" ss:StyleID="EditLeftText" />

		</ss:Row>

	</xsl:template>

</xsl:stylesheet>
