<?xml version="1.0" encoding="UTF-8" standalone="yes"?>

<!-- This stylesheet is used to turn flat, cartesian data from the database into a hierarchical dataset that can
be manipulated futher with XSL into a working document. -->
<xsl:stylesheet version="1.0" xmlns="urn:schemas-microsoft-com:office:spreadsheet"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
	xmlns:mts="urn:schemas-markthreesoftware-com:stylesheet">

	<!-- These parameters are used by the loader to specify the attributes of the stylesheet. -->
 	<mts:stylesheetId>EQUITY PLACEMENT</mts:stylesheetId>
	<mts:stylesheetTypeCode>PLACEMENT</mts:stylesheetTypeCode>
	<mts:name>Equity Placement</mts:name>

	<!-- This specified the attributes of the output of the XSL Transaformation. -->
	<xsl:output method="xml" encoding="UTF-8" standalone="yes" indent="yes"/>

	<!-- Match the Document root. -->
	<xsl:template match="Placements">

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
					<ss:NamedRange ss:Name="placementId" ss:RefersTo="=Sheet1!C2"/>
					<ss:NamedRange ss:Name="brokerSymbol" ss:RefersTo="=Sheet1!C3"/>
					<ss:NamedRange ss:Name="quantity" ss:RefersTo="=Sheet1!C4"/>
					<ss:NamedRange ss:Name="timeInForceMnemonic" ss:RefersTo="=Sheet1!C5"/>
					<ss:NamedRange ss:Name="orderTypeMnemonic" ss:RefersTo="=Sheet1!C6"/>
					<ss:NamedRange ss:Name="price1" ss:RefersTo="=Sheet1!C7"/>
					<ss:NamedRange ss:Name="price2" ss:RefersTo="=Sheet1!C8"/>
					<ss:NamedRange ss:Name="modifiedTime" ss:RefersTo="=Sheet1!C9"/>
				</ss:Names>

				<!-- Styles -->
				<ss:Styles>
					<ss:Style ss:ID="Default">
						<ss:Alignment ss:Horizontal="Automatic" ss:Rotate="0.0" ss:Vertical="Center"
						ss:ReadingOrder="Context"/>
						<ss:Font ss:FontName="Microsoft Sans Serif" ss:Size="8" x:Family="Script"/>
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
					<ss:Style ss:ID="Price">
						<ss:NumberFormat ss:Format="#,##0.00"/>
						<ss:Alignment ss:Horizontal="Right" ss:Vertical="Center"/>
					</ss:Style>
					<ss:Style ss:ID="Quantity">
						<ss:NumberFormat ss:Format="#,##0_);[Red]\(#,##0\)"/>
						<ss:Alignment ss:Horizontal="Right" ss:Vertical="Center"/>
					</ss:Style>
					<ss:Style ss:ID="MarketValue">
						<ss:NumberFormat ss:Format="#,##0.00;[Red]\(#,##0.00\);"/>
						<ss:Alignment ss:Horizontal="Right" ss:Vertical="Center"/>
					</ss:Style>
					<ss:Style ss:ID="ShortDate">
						<ss:NumberFormat ss:Format="m/d;@"/>
					</ss:Style>
					<ss:Style ss:ID="ShortDateTime">
						<ss:NumberFormat ss:Format="m/d hh:mm"/>
					</ss:Style>
					<ss:Style ss:ID="EditLeftText" ss:Parent="LeftText">
						<ss:Protection ss:Protected="0"/>
					</ss:Style>
					<ss:Style ss:ID="EditCenterText" ss:Parent="CenterText">
						<ss:Protection ss:Protected="0"/>
					</ss:Style>
					<ss:Style ss:ID="EditRightText" ss:Parent="RightText">
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
					<ss:Style ss:ID="BrokerSymbol" ss:Parent="EditLeftText">
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
						<x:SplitVertical>2</x:SplitVertical>
						<x:LeftColumnVisible>0</x:LeftColumnVisible>
						<x:LeftColumnRightPane>2</x:LeftColumnRightPane>
					</x:WorksheetOptions>

					<!-- Table: The meat of the spreadsheet -->
					<ss:Table>

						<!-- Column Definitions -->
						<ss:Column ss:Index="1" ss:Width="28" ss:Hidden="1" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="2" ss:Width="28" ss:Hidden="1" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="3" ss:Width="37.5" ss:StyleID="LeftText" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="4" ss:Width="49.5" ss:StyleID="Quantity" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="5" ss:Width="31.5" ss:StyleID="CenterText" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="6" ss:Width="31.5" ss:StyleID="CenterText" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="7" ss:Width="34" ss:StyleID="Price" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="8" ss:Width="34" ss:StyleID="Price" ss:AutoFitWidth="0"/>
						<ss:Column ss:Index="9" ss:Width="45" ss:StyleID="ShortDateTime" ss:AutoFitWidth="0"/>

						<!-- This column needs to be included to guarantee that the last readable column can be scrolled 
						into the viewer.  The column width should be equal to the largest column in the report. -->
						<ss:Column ss:Index="10" ss:Width="49.5" ss:AutoFitWidth="0"/>

						<!-- The Column Headings -->
						<ss:Row ss:Height="12" ss:StyleID="Header" >
							<ss:Cell ss:Index="3" ss:StyleID="HeaderLeftText">
								<ss:Data ss:Type="String">Broker</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="4" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Quantity</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="5" ss:StyleID="HeaderCenterText">
								<ss:Data ss:Type="String">TIF</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="6" ss:StyleID="HeaderCenterText">
								<ss:Data ss:Type="String">Price</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="7" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Limit</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="8" ss:StyleID="HeaderRightText">
								<ss:Data ss:Type="String">Stop</ss:Data>
							</ss:Cell>
							<ss:Cell ss:Index="9" ss:StyleID="HeaderRightText">
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
	
	<!-- Placement -->
	<xsl:template match="Placement">

		<!-- Block Heading -->
		<ss:Row ss:Height="12">
			
			<ss:Cell ss:Index="1"><ss:Data ss:Type="Number">1</ss:Data></ss:Cell>
			<ss:Cell ss:Index="2"><ss:Data ss:Type="Number"><xsl:value-of select="@PlacementId"/></ss:Data></ss:Cell>
			<ss:Cell ss:Index="3">
				<xsl:attribute name="ss:StyleID">EditLeftText</xsl:attribute>
				<xsl:if test="@BrokerSymbol">
					<ss:Data ss:Type="String"><xsl:value-of select="@BrokerSymbol"/></ss:Data>
				</xsl:if>
			</ss:Cell>
			<ss:Cell ss:Index="4">
				<xsl:attribute name="ss:StyleID">EditQuantity</xsl:attribute>
				<xsl:if test="@Quantity">
					<ss:Data ss:Type="Number"><xsl:value-of select="@Quantity"/></ss:Data>
				</xsl:if>
			</ss:Cell>
			<ss:Cell ss:Index="5">
				<xsl:attribute name="ss:StyleID">EditCenterText</xsl:attribute>
				<xsl:if test="@TimeInForceMnemonic">
					<ss:Data ss:Type="String"><xsl:value-of select="@TimeInForceMnemonic"/></ss:Data>
				</xsl:if>
			</ss:Cell>
			<ss:Cell ss:Index="6">
				<xsl:attribute name="ss:StyleID">EditCenterText</xsl:attribute>
				<xsl:if test="@OrderTypeMnemonic">
					<ss:Data ss:Type="String"><xsl:value-of select="@OrderTypeMnemonic"/></ss:Data>
				</xsl:if>
			</ss:Cell>
			<ss:Cell ss:Index="7">
				<xsl:attribute name="ss:StyleID">EditPrice</xsl:attribute>
				<xsl:if test="@Price1">
					<ss:Data ss:Type="Number"><xsl:value-of select="@Price1"/></ss:Data>
				</xsl:if>
			</ss:Cell>
			<ss:Cell ss:Index="8">
				<xsl:attribute name="ss:StyleID">EditPrice</xsl:attribute>
				<xsl:if test="@Price2">
					<ss:Data ss:Type="Number"><xsl:value-of select="@Price2"/></ss:Data>
				</xsl:if>
			</ss:Cell>
			<ss:Cell ss:Index="9">
				<xsl:if test="@ModifiedTime">
					<ss:Data ss:Type="DateTime"><xsl:value-of select="@ModifiedTime"/></ss:Data>
				</xsl:if>
			</ss:Cell>
		</ss:Row>

	</xsl:template>
	
	<!-- A Blank line for new execution -->
	<xsl:template match="Placeholder">

		<!-- Execution Row -->
		<ss:Row ss:Height="12" >
			
			<ss:Cell ss:Index="1"><ss:Data ss:Type="Number">2</ss:Data></ss:Cell>
			<ss:Cell ss:Index="3" ss:StyleID="GrayEditLeftText" >
				<ss:Data ss:Type="String">Click here to add a new Placement</ss:Data>
			</ss:Cell>
			<ss:Cell ss:Index="4" ss:StyleID="EditLeftText" />
			<ss:Cell ss:Index="5" ss:StyleID="EditLeftText" />
			<ss:Cell ss:Index="6" ss:StyleID="EditLeftText" />
			<ss:Cell ss:Index="7" ss:StyleID="EditLeftText" />
			<ss:Cell ss:Index="8" ss:StyleID="EditLeftText" />

		</ss:Row>

	</xsl:template>

</xsl:stylesheet>
