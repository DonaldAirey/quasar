<?xml version="1.0" encoding="utf-8" standalone="yes"?>
<stylesheet version="1.0" xmlns="http://www.w3.org/1999/XSL/Transform"
	xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
	xmlns:x="urn:schemas-microsoft-com:office:excel"
	xmlns:c="urn:schemas-microsoft-com:office:component:spreadsheet"
	xmlns:mts="urn:schemas-markthreesoftware-com:stylesheet">
	
	<!-- Version 1.0 -->

	<!-- These parameters are used by the loader to specify the attributes of the stylesheet. -->
 	<mts:stylesheetId>QUOTE VIEWER</mts:stylesheetId>
	<mts:stylesheetTypeCode>QUOTE</mts:stylesheetTypeCode>
	<mts:name>Quote Viewer</mts:name>  

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
          <ss:Style ss:ID="BorderLeftText" ss:Parent="LeftText">
            <ss:Border ss:Position="Bottom" ss:Weight="2" ss:Color="#000000"/>
            <ss:Border ss:Position="Top" ss:Weight="2" ss:Color="#000000"/>
            <ss:Border ss:Position="Right" ss:Weight="2" ss:Color="#000000"/>
            <ss:Border ss:Position="Left" ss:Weight="2" ss:Color="#000000"/>
          </ss:Style>
          <ss:Style ss:ID="BoldBorderLeftText" ss:Parent="BorderLeftText">
            <ss:Font ss:FontName="Times New Roman" ss:Size="10" ss:Bold="1" />
          </ss:Style>
          <ss:Style ss:ID="BorderCenterText" ss:Parent="CenterText">
            <ss:Border ss:Position="Bottom" ss:Weight="2" ss:Color="#000000"/>
            <ss:Border ss:Position="Top" ss:Weight="2" ss:Color="#000000"/>
            <ss:Border ss:Position="Right" ss:Weight="2" ss:Color="#000000"/>
            <ss:Border ss:Position="Left" ss:Weight="2" ss:Color="#000000"/>
          </ss:Style>
          <ss:Style ss:ID="BoldBorderCenterText" ss:Parent="BorderCenterText">
            <ss:Font ss:FontName="Times New Roman" ss:Size="10" ss:Bold="1" />
          </ss:Style>
          <ss:Style ss:ID="BottomBorder" ss:Parent="LeftText">
            <ss:Border ss:Position="Bottom" ss:Weight="2" ss:Color="#000000"/>
          </ss:Style>
          <ss:Style ss:ID="FilledCell" ss:Parent="Default">
            <ss:Interior ss:Color="#555555"/>            
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
          <ss:Style ss:ID="LastPrice" ss:Parent="RightText">
            <ss:NumberFormat ss:Format="#,##0.00;-#,##0.00;"/>
            <mts:Display>
              <ss:Font ss:Bold="1" ss:Size="11">
                <mts:Animation mts:Direction="Nil" mts:StartColor="#0000FF" />
                <mts:Animation mts:Direction="Up" mts:StartColor="#00FF00" />
                <mts:Animation mts:Direction="Down" mts:StartColor="#FF0000" />
              </ss:Font>
            </mts:Display>
          </ss:Style>
          <ss:Style ss:ID="StaticPrice" ss:Parent="RightText">
            <ss:NumberFormat ss:Format="#,##0.00;-#,##0.00;"/>
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
					<ss:Style ss:ID="ActiveStateText" ss:Parent="CenterText">
						<mts:Display>
							<ss:Font ss:Color="#00C000"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="ErrorStateText" ss:Parent="CenterText">
						<mts:Display>
							<ss:Font ss:Color="#CF0000"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="SubmittedStateText" ss:Parent="CenterText">
						<mts:Display>
							<ss:Font ss:Color="#0000CF"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="HeldStateText" ss:Parent="CenterText">
						<mts:Display>
							<ss:Font ss:Color="#000064"/>
						</mts:Display>
					</ss:Style>
					<ss:Style ss:ID="FilledStateText" ss:Parent="CenterText">
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
					<ss:Style ss:ID="ExecutionQuantity" ss:Parent="Quantity">
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

					<ss:Table mts:HeaderHeight="0">

						<ss:Columns>
							<ss:Column mts:ColumnId="FirstColumn" ss:Width="100" ss:StyleID="TopCenterText" mts:PrintWidth="35" mts:Description="FirstColumn"/>
							<ss:Column mts:ColumnId="SecondColumn" ss:Width="60" ss:StyleID="TopCenterText" mts:PrintWidth="20" mts:Description="SecondColumn"/>
							<ss:Column mts:ColumnId="ThirdColumn" ss:Width="60" ss:StyleID="TopCenterText" mts:PrintWidth="35" mts:Description="ThirdColumn"/>
							<ss:Column mts:ColumnId="SecurityId" ss:Hidden="1" ss:Width="60" ss:StyleID="TopRightText" mts:Printed="false" mts:Description="Sec. Id" />
              <ss:Column mts:ColumnId="RowOrder" ss:Hidden="1" ss:Width="15" ss:StyleID="TopRightText"  mts:Printed="false" mts:Description="RowOrder" />
            </ss:Columns>

            <mts:View>
              <mts:ViewColumn mts:ColumnId="RowOrder" mts:Direction="ascending" />
            </mts:View>
            
            <mts:Constraint mts:PrimaryKey="True">
              <mts:ConstraintColumn mts:ColumnId="RowOrder" />
            </mts:Constraint>
            
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

  <template match="Quote">
    

    <!-- Security Name Row-->
    <ss:Row>
      <ss:Cell mts:ColumnId="FirstColumn">
        <attribute name="ss:StyleID">BoldBorderLeftText</attribute>
        <ss:Data ss:Type="string"><value-of select="@SecurityName"/></ss:Data>
      </ss:Cell>

      <ss:Cell mts:ColumnId="SecondColumn">
        <attribute name="ss:StyleID">BoldBorderCenterText</attribute>
        <ss:Data ss:Type="string"><value-of select="@Symbol"/></ss:Data>
      </ss:Cell>

      <ss:Cell mts:ColumnId="ThirdColumn">
        <attribute name="ss:StyleID">BottomBorder</attribute>
      </ss:Cell>

      <ss:Cell mts:ColumnId="RowOrder">
        <ss:Data ss:Type="int">-1</ss:Data>
      </ss:Cell>
    </ss:Row>

    <!-- Empty Filled Row -->
    <ss:Row>
      <ss:Cell mts:ColumnId="RowOrder">
        <ss:Data ss:Type="int">0</ss:Data>
       </ss:Cell>
      <ss:Cell mts:ColumnId="FirstColumn">
        <attribute name="ss:StyleID">FilledCell</attribute>
      </ss:Cell>
      <ss:Cell mts:ColumnId="SecondColumn">
        <attribute name="ss:StyleID">FilledCell</attribute>
      </ss:Cell>
      <ss:Cell mts:ColumnId="ThirdColumn">
        <attribute name="ss:StyleID">FilledCell</attribute>
      </ss:Cell>
    </ss:Row>


    <!-- Last Trade Row -->
    <ss:Row>
      <ss:Cell mts:ColumnId="FirstColumn">
        <attribute name="ss:StyleID">LeftText</attribute>
        <ss:Data ss:Type="string">   Last Trade</ss:Data>
      </ss:Cell>
      
      <ss:Cell mts:ColumnId="SecondColumn">
        <attribute name="ss:StyleID">LastPrice</attribute>
        <ss:Data ss:Type="decimal">
          <value-of select="@LastPrice" />
        </ss:Data>
      </ss:Cell>

      <ss:Cell mts:ColumnId="ThirdColumn">
        <attribute name="ss:StyleID">Percent</attribute>
        <ss:Data ss:Type="decimal">
          <value-of select="@DayPercent"/>
        </ss:Data>
      </ss:Cell>
      
      <ss:Cell mts:ColumnId="RowOrder">
        <ss:Data ss:Type="int">1</ss:Data>
      </ss:Cell>
    </ss:Row>

    <!-- Bid/Ask Price Row -->
    <ss:Row>
      <ss:Cell mts:ColumnId="FirstColumn">
        <attribute name="ss:StyleID">LeftText</attribute>
        <ss:Data ss:Type="string">   Bid/Ask</ss:Data>
      </ss:Cell>

      <ss:Cell mts:ColumnId="SecondColumn">
        <attribute name="ss:StyleID">Price</attribute>
        <ss:Data ss:Type="decimal">
          <value-of select="@BidPrice" />
        </ss:Data>
      </ss:Cell>

      <ss:Cell mts:ColumnId="ThirdColumn">
        <attribute name="ss:StyleID">Price</attribute>
        <ss:Data ss:Type="decimal">
          <value-of select="@AskPrice"/>
        </ss:Data>
      </ss:Cell>
      
      <ss:Cell mts:ColumnId="RowOrder">
        <ss:Data ss:Type="int">2</ss:Data>
      </ss:Cell>
      
     </ss:Row>

    <!-- Bid/Ask Price Row -->
    <ss:Row>
      <ss:Cell mts:ColumnId="FirstColumn">
        <attribute name="ss:StyleID">LeftText</attribute>
        <ss:Data ss:Type="string"> Size</ss:Data>
      </ss:Cell>

      <ss:Cell mts:ColumnId="SecondColumn">
        <attribute name="ss:StyleID">Quantity</attribute>
        <ss:Data ss:Type="decimal">
          <value-of select="@BidSize" />
        </ss:Data>
      </ss:Cell>

      <ss:Cell mts:ColumnId="ThirdColumn">
        <attribute name="ss:StyleID">Quantity</attribute>
        <ss:Data ss:Type="decimal">
          <value-of select="@AskSize"/>
        </ss:Data>
      </ss:Cell>

      <ss:Cell mts:ColumnId="RowOrder">
        <ss:Data ss:Type="int">3</ss:Data>
      </ss:Cell>
    </ss:Row>

    <!-- Volume/ADV Row -->
    <ss:Row>
      <ss:Cell mts:ColumnId="FirstColumn">
        <attribute name="ss:StyleID">LeftText</attribute>
        <ss:Data ss:Type="string">   Volume/ADV</ss:Data>
      </ss:Cell>

      <ss:Cell mts:ColumnId="SecondColumn">
        <attribute name="ss:StyleID">Quantity</attribute>
        <ss:Data ss:Type="decimal">
          <value-of select="@Volume" />
        </ss:Data>
      </ss:Cell>

      <ss:Cell mts:ColumnId="ThirdColumn">
        <attribute name="ss:StyleID">Quantity</attribute>
        <ss:Data ss:Type="decimal">
          <value-of select="@AverageDailyVolume"/>
        </ss:Data>
      </ss:Cell>

      <ss:Cell mts:ColumnId="RowOrder">
        <ss:Data ss:Type="int">4</ss:Data>
      </ss:Cell>
    </ss:Row>

    <!-- Prev Close/Open -->
    <ss:Row>
      <ss:Cell mts:ColumnId="FirstColumn">
        <attribute name="ss:StyleID">LeftText</attribute>
        <ss:Data ss:Type="string">   Prev. Close/Open</ss:Data>
      </ss:Cell>

      <ss:Cell mts:ColumnId="SecondColumn">
        <attribute name="ss:StyleID">StaticPrice</attribute>
        <ss:Data ss:Type="decimal">
          <value-of select="@PreviousClosePrice" />
        </ss:Data>
      </ss:Cell>

      <ss:Cell mts:ColumnId="ThirdColumn">
        <attribute name="ss:StyleID">StaticPrice</attribute>
        <ss:Data ss:Type="decimal">
          <value-of select="@OpenPrice"/>
        </ss:Data>
      </ss:Cell>

      <ss:Cell mts:ColumnId="RowOrder">
        <ss:Data ss:Type="int">5</ss:Data>
      </ss:Cell>
    </ss:Row>

    <!-- Days Range Row -->
    <ss:Row>
      <ss:Cell mts:ColumnId="FirstColumn">
        <attribute name="ss:StyleID">LeftText</attribute>
        <ss:Data ss:Type="string">   Days Range</ss:Data>
      </ss:Cell>

      <ss:Cell mts:ColumnId="SecondColumn">
        <attribute name="ss:StyleID">StaticPrice</attribute>
        <ss:Data ss:Type="decimal">
          <value-of select="@DayLowPrice" />
        </ss:Data>
      </ss:Cell>

      <ss:Cell mts:ColumnId="ThirdColumn">
        <attribute name="ss:StyleID">StaticPrice</attribute>
        <ss:Data ss:Type="decimal">
          <value-of select="@DayHighPrice"/>
        </ss:Data>
      </ss:Cell>

      <ss:Cell mts:ColumnId="RowOrder">
        <ss:Data ss:Type="int">6</ss:Data>
      </ss:Cell>
    </ss:Row>

    <!-- Year Range Row -->
    <ss:Row>
      <ss:Cell mts:ColumnId="FirstColumn">
        <attribute name="ss:StyleID">LeftText</attribute>
        <ss:Data ss:Type="string">   52 Wk Range</ss:Data>
      </ss:Cell>

      <ss:Cell mts:ColumnId="SecondColumn">
        <attribute name="ss:StyleID">RightText</attribute>
        <ss:Data ss:Type="string">N/A</ss:Data>
      </ss:Cell>

      <ss:Cell mts:ColumnId="ThirdColumn">
        <attribute name="ss:StyleID">RightText</attribute>
        <ss:Data ss:Type="string">N/A</ss:Data>
      </ss:Cell>

      <ss:Cell mts:ColumnId="RowOrder">
        <ss:Data ss:Type="int">7</ss:Data>
      </ss:Cell>
    </ss:Row>
    
    <!-- Empty Filled Row -->
    <ss:Row>
      <ss:Cell mts:ColumnId="RowOrder">
        <ss:Data ss:Type="int">8</ss:Data>
      </ss:Cell>
      <ss:Cell mts:ColumnId="FirstColumn">
        <attribute name="ss:StyleID">FilledCell</attribute>
      </ss:Cell>
      <ss:Cell mts:ColumnId="SecondColumn">
        <attribute name="ss:StyleID">FilledCell</attribute>
      </ss:Cell>
      <ss:Cell mts:ColumnId="ThirdColumn">
        <attribute name="ss:StyleID">FilledCell</attribute>
      </ss:Cell>
    </ss:Row>

  </template>



</stylesheet>
