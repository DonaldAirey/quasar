<?xml version="1.0" encoding="utf-8" standalone="yes"?>
<stylesheet version="1.0" xmlns="http://www.w3.org/1999/XSL/Transform"
	xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
	xmlns:x="urn:schemas-microsoft-com:office:excel"
	xmlns:c="urn:schemas-microsoft-com:office:component:spreadsheet"
	xmlns:mts="urn:schemas-markthreesoftware-com:stylesheet">
	
	<!-- Version 1.0 -->

	<!-- These parameters are used by the loader to specify the attributes of the stylesheet. -->
 	<mts:stylesheetId>EXECUTION VIEWER</mts:stylesheetId>
	<mts:stylesheetTypeCode>EXECUTION</mts:stylesheetTypeCode>
	<mts:name>Execution Viewer</mts:name>  

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
            <ss:Font ss:Color="#0000DD" ss:FontName="Times New Roman" ss:Size="10" ss:Bold="1" ss:Horizontal="Left"/>
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
            <ss:Font ss:Color="#00FF00" ss:FontName="Times New Roman" ss:Size="10" ss:Bold="1" ss:Horizontal="Center"/>
          </ss:Style>
          <ss:Style ss:ID="SellText" ss:Parent="CenterText">
            <ss:Font ss:Color="#FF0000" ss:FontName="Times New Roman" ss:Size="10" ss:Bold="1" ss:Horizontal="Center"/>
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
				
				<ss:Worksheet ss:Name="Sheet1">
					<x:WorksheetOptions>
						<x:DoNotDisplayColHeaders/>
						<x:DoNotDisplayGridlines/>
						<x:DoNotDisplayRowHeaders/>
						<mts:DoNotSelectRows/>
					</x:WorksheetOptions>

					<ss:Table mts:HeaderHeight="13">

						<ss:Columns>
              <ss:Column mts:ColumnId="SecuritySymbol" ss:Width="46" ss:StyleID="TopLeftText" mts:PrintWidth="46" mts:Description="Symbol" mts:Image="iVBORw0KGgoAAAANSUhEUgAAAD0AAAAPCAYAAABX0MdPAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAbdJREFUSEvdV7FKA0EQHf/HwgMLAzZeZ/7AgIUcFpJKgoWENBIs5EghR6oEvyAphEshnD9gkZ8Q8gt24855E8fJ3ub2iIg3Ybi93ZnZ93Zndi97aASMTJ+msHxbwup9BR/m12iZzWfYvepi/BBjPIoxeUyap+MEJ+NJrolpAxGNziPs3/b9dGDsXbrLeL6xlP3wbohSIbqIsH3axt51b1NvTJ9QKgWXUAwWa7xiDhnDZbceUzgkpjptIMLhcfiDHAfSu09guY+By3dq6/6yDKpq552BFbICWkctDA4C7Jx1NtJbp4V8Z9Aum9/0rzsvlTLQLhNpJk67PByYGiC9L9c1aWEj05Z9dTnofnrf5meLJf1cOHmMzh8iHBwGCNGlaRSkfZ4MVPvI/rI2+egxGaeKH9lwHB/cZAt0Rfk6adBVAdvsbItnOyzZt2yxfThA+pz+KWnbArqI7YR09pohXVs+K1V3pzkltxHVxFx+vrjz9M5esvxrzMdZp59OPVtN2oC77OQcrvg+uNkWKDgRD0++T/E6gf6TzxfpRYbJqDjQ9s311XDNSbOkixTTeZr/8aBPQKr1Juonw/jy05IBqwMAAAAASUVORK5CYII="/>
              <ss:Column mts:ColumnId="OrderTypeMnemonic" ss:Width="33" ss:StyleID="TopLeftText" mts:PrintWidth="33" mts:Description="Side" mts:Image="iVBORw0KGgoAAAANSUhEUgAAACwAAAAPCAYAAACfvC2ZAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAVFJREFUSEvVlD1rwzAQhq9bhgwaMnjoEujYoYIMFXQKZAl0aMZAhuJfUEyGIrIUkSlTMZnyQzMUrj45gquwo3NKwDl4sT5Od4/vhO4WbwuEyo4/R/r03wjYbRyW32U/tS/xsD94ldUY5q9zdF8O7aftpYiNC8yLqYHXFot1kRQyI3+ycC5s8TixvyTHOR8wkxOwoMIheehGat7m959uwvhh7Cu8XC3/lD5uBc2D8T1aa/KNfSR+qTj5KkfIRhm6rUM90Zi/52g31s+bxNsb7zft8TUPfIp7Lk5TXmIiWGIEpRTutjvUT1osnpCfC+u01nUszQ+D4aCucAdgDkRg4WwXyLafTnHUwNX9TDkGyLaKXrOqPCeokfLvrxS4raKXAvN4EgbI7jMsPgoxcOo6NF2LuP2XXgfK7d/h6WwqApZU4No+YJ6Nfy70420I6O29Jf0ChVmsRh2JEUcAAAAASUVORK5CYII="/>
              <ss:Column mts:ColumnId="ExecutionQuantity" ss:Width="62" ss:StyleID="TopLeftText" mts:PrintWidth="62" mts:Description="Quantity" mts:Image="iVBORw0KGgoAAAANSUhEUgAAAFIAAAAPCAYAAAB3PJiyAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAdpJREFUWEftVjFrwkAUvm4OHTI4ZOgidOzQgEMDnYQuQoc6Cg4lv6CIQwkuJTg5leDkD3UovOadnpyv7+J7qRUsOXiYu3vvfV8+v0tyNXoZganG5muDP+1oqgAKWcwLKD/LNjQarEpYr9Y2yuraDJ+HUHwUkL/nbSg0QM38MOljuhVylsN0NhUFkHFQ91b1OFP84LHD9dePcdHk7nsxOpm0vxNS+G84YOdgOv8rZyMO1zuEH8rX9NDci+nd9qwjx5PxgVWpdXHuBt0LrXM9mq4hRqiW4tflau5LyjWbZGDibgzFooCkn0D2mkE+z+2ciz1hsu+v+0cFe9Aauu/nWAFIDT2+x7i5Hn4eh0lxuboQlltHrVBE1M5EUQTLxRKS++RoOEI0l677c8k19uOGwwnhcjzquEm5SLSgOaZz3dk68gxC+hhS8X2R6zj6fwSHQ/dDokp04HK2QlbPP0mD3zqScx7nujqROZ6YH3KvhLPU8XUamagb2behREjOHRwJyRE6lSM1+I4/vY+TCBnfxPa7Typknas0R4p9KO6cFTqGvvNCPGhfmifhqNHC5drvyMHTQCWk9LnVhNCl1pj0IbWv7+ROF/vjoKzT4lxKvsFvx6bhH6OmPf5L3Tdw7d7uW/2G5gAAAABJRU5ErkJggg=="/>
              <ss:Column mts:ColumnId="ExecutionPrice" ss:Width="42" ss:StyleID="TopLeftText" mts:PrintWidth="42" mts:Description="Price" mts:Image="iVBORw0KGgoAAAANSUhEUgAAADgAAAAPCAYAAACx+QwLAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAXRJREFUSEvdVbFqwzAQvW4ZOmjI4KFLoGOHGjJU0CnQJdChGQMZir+ghAzFZCkiU6ZiMuVDMxSuOjkqylmKpZCU1gcP+6TT6T3d2bqavEwQtO2+dvTonpFAtVRYfVbdwKbC7WZrUOl3GD+PUX0oLN/L/41lzZ+0uAD5KGuBixLni/kh3rTPgAHjcT7fLm3sw/eN4OHdz5MH5HAvMKGClqitOvdD3RAbd85ugsHtwFRwOps2ysvLbX1LNOSH1v32eDErELJ+hmqlMB/mWLwWWOpeJv8YfgTu41zf7WDK4TObm8/5xtu4+OZJA4kjTSCEwPVqjfl9Hg0faXe9O0/jZPZ56nsKPzcWete9uoInCAytsQL5vDveFsMPKYVfU6D+BlMShMjZHG3keTV91U/hcywWRF+Y+yMl4SUE+lrXbe8UfgcVzG4yc9fFJuDfX6gN+SH41oXa8FztSdzMPTh6GkULjD2IvxIH8kGa32l+100A3X1dxjfqrwvw7RsLSwAAAABJRU5ErkJggg=="/>
              <ss:Column mts:ColumnId="CreatedTime" ss:Width="45" ss:StyleID="TopLeftText" mts:PrintWidth="45" mts:Description="Time" mts:Image="iVBORw0KGgoAAAANSUhEUgAAADwAAAAPCAYAAAC4EqxxAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAXJJREFUSEvlVbFqwzAQvW4ZOnjI4KFLoGOHCjJU0CnQJdChGQMZir+ghAzFZCkiU6ZiMuVDMxSuOrkq6kWuJOOEkAge9knvTvd8Outq8jJB0GP3taPH+Q8SrJYKq8/qPLGpcLvZGlT6HcbPY1QfCsv3ch9LPdcFeOwuYsbE0PuSNhcgH2UteFHifDH/izdtM2BgEN8O7nt0m+vRNsjhj2BfhT1zJMaeBivMtemdz3tPT+R+XfvC4HZgKjydTffKz48Dt62wEO9U1otZgZD3c1QrhWIosHgtTM+SHYNfwQ7fPfEUo8l21+xenBuTQwyHNJFY0ghZluF6tUZxL5JhE+S+fD7U9+Tv+jTFbZMj94Heda+u8BEE/yfM91Ha5BTyqQXrHg4RfeupFY4R3CaPFB/I+pn566Y4We4hBVPsNjmFfCC/yc1dGyI29WlKvzZV2Ipzj3VqPrF8cw+PnkbJgmM3ODUeyAdpftfi7jIAdPdeEr4BTe1+v4cwsRMAAAAASUVORK5CYII="/>
              <ss:Column mts:ColumnId="DestinationStateTypeCode" ss:Hidden="1" ss:Width="15" ss:StyleID="TopCenterText" mts:PrintWidth="20" mts:Description="In State"/>
              <ss:Column mts:ColumnId="DestinationStateMnemonic" ss:Hidden="1" ss:Width="60" ss:StyleID="TopCenterText" mts:PrintWidth="35" mts:Description="In State"/>
              <ss:Column mts:ColumnId="SourceStateTypeCode" ss:Hidden="1" ss:Width="15" ss:StyleID="TopCenterText" mts:PrintWidth="20" mts:Description="Out State"/>
              <ss:Column mts:ColumnId="SourceStateMnemonic" ss:Hidden="1" ss:Width="60" ss:StyleID="TopCenterText" mts:PrintWidth="35" mts:Description="Out State"/>
              <ss:Column mts:ColumnId="BlotterName" ss:Hidden="1" ss:Width="70" ss:StyleID="TopLeftText" mts:Printed="false"  mts:Description="Blotter" />
              <ss:Column mts:ColumnId="TimeInForce" ss:Hidden="1" ss:Width="30" ss:StyleID="TopLeftText" mts:PrintWidth="33" mts:Description="TIF"/>
              <ss:Column mts:ColumnId="ExecutionId" ss:Hidden="1" ss:Width="30" ss:StyleID="TopRightText" mts:PrintWidth="24" mts:Description="Id" />
							<ss:Column mts:ColumnId="SecurityId" ss:Hidden="1" ss:Width="60" ss:StyleID="TopRightText" mts:Printed="false" mts:Description="Sec. Id" />
							<ss:Column mts:ColumnId="Commission" ss:Hidden="1" ss:Width="45" ss:StyleID="TopRightText" mts:Description="Comm."/>
							<ss:Column mts:ColumnId="Destination" ss:Hidden="1" ss:Width="65" ss:StyleID="TopLeftText" mts:Description="Destination"/>
							<ss:Column mts:ColumnId="Market" ss:Hidden="1" ss:Width="45" ss:StyleID="TopRightText" mts:Description="Net"/>
							<ss:Column mts:ColumnId="GrossValue" ss:Hidden="1" ss:Width="45" ss:StyleID="TopRightText" mts:Description="Gross"/>
							<ss:Column mts:ColumnId="NetValue" ss:Hidden="1" ss:Width="55" ss:StyleID="TopLeftText" mts:Description="Net"/>
							<ss:Column mts:ColumnId="CreatedName" ss:Hidden="1" ss:Width="90" ss:StyleID="TopLeftText" mts:Printed="false" mts:Description="Who"/>
							<ss:Column mts:ColumnId="BrokerId" ss:Hidden="1" ss:Width="30" ss:StyleID="TopLeftText" mts:Printed="false" mts:Description="Broker Id"/>
							<ss:Column mts:ColumnId="BrokerName" ss:Hidden="1" ss:Width="90" ss:StyleID="TopLeftText" mts:Printed="false" mts:Description="Broker Name"/>
							<ss:Column mts:ColumnId="BrokerSymbol" ss:Width="65" ss:StyleID="TopLeftText" mts:PrintWidth="65" mts:Description="Broker" mts:Image="iVBORw0KGgoAAAANSUhEUgAAAFYAAAAPCAYAAAB+1zjIAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAadJREFUWEftlj9rAjEUwNPNocMNDjd0ETp26IFDDzoJXYQOdRQcyn2CIg7lcCnByakcTn5Qh8JrXjQlxpeLr+aklgs8NO/en7wfL49cjV5GINTafG3wp12xCCBYOZdQfVatnMJgVcF6tdZSqf9i+DwE+SGhfC8PZa505xAq94XpkKEtIn/Mt2BnJUxn0315U3tCoGZR9kGdymuvg3O457qAvcj7O7DMDjEg7E6ndORN8OQ61Z+Tq2lb0bvt6Y4dT8Z7rey2trs3EGw9pQvFienPydWkbTEpQKTdFORCQtbPoHgt9EzFfUh+IO5sfXvUYyz7u33t7TwhG8rP1bnnCNUR8zuyQ6jIUiRJAsvFErL7jCW+MWvHsW1Q79sbH/Pd2FKx7DiuH/pTvtzaYtiLznVn27G/BOsrngLg6myQLngDiYLn8+PW0KT9FqyasdwkbnF1ncTtvGNjUZ3OraMpe5F0E/1+5SZoGuwx3f2nwaY3qX6rcsDWvWN9V7du9vrGQGhGh8YHp6bYtvodO3gasMDGPsR/jCfyh1w/D7K7VmIyEPh2bSU+g2//OsUqZnC4fAAAAABJRU5ErkJggg=="/>
							<ss:Column mts:ColumnId="BrokerAccountId" ss:Hidden="1" ss:Width="30" ss:StyleID="TopLeftText" mts:Printed="false" mts:Description="Account Id"/>
							<ss:Column mts:ColumnId="BrokerAccountMnemonic" ss:Width="76" ss:StyleID="TopLeftText" mts:PrintWidth="76" mts:Description="Account" mts:Image="iVBORw0KGgoAAAANSUhEUgAAAGUAAAAPCAYAAAD9EwHzAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAexJREFUWEftl7FqwzAQhtUtQwcPGTxkMXTsUEMKNXSpoUugQ7PV0KGETiFDCRmKyVJMhmIyFJMpr5Fsea8MheudHBVFjWK50FikFhyypFNy+j+fJJ9077vAsKw/11TVpSIFvJbH/Euf9Z56jBGUZJxA9pHVdkgNZqg32nw25zXpH49j6D/3gXXuOpC8JRC/xrX9pQYouKwxJcKWEQP0Gb4MgQXXQQ5lhB2jobEBljL+/94Xxf7WgJ51NsBMCdobKCXeEgJCxdbssjk2odkPKJQQBGqAmeKdeTxToseI1yYmoFBt4n9oH1vjknXgcJQtjfqihwiY23QhmSTgt33Ak587UltnfME4LorqJwMTvrK/Om8L8OZ/1T55jumY/N/71lPZGJ4pXOvN2UJbGwHxL3xgjuNAOkl5w8RoscJPCLSrLY+Vfabf082Rx2RAuphM1mSbD2ucNvJMKQFFzYYiQVR4RRBNoah+u9qm67LJL4eCZ4lJUHKWlBGkhmK2CwkGzGk6/Bb1Gygmb7QOnslc8RKYbpPHkCn849FtufwqVgRl3/69a//XZZVpv+7sUOMoahety7bx9D3Nv1PC27AQim3BH2M84U0Iq+UKoVwF/Drsn9dWqQZ40cqmWQ6Fvk1qq0YDOjbSaQqL5YKbKF+DpYHrMRBAWQAAAABJRU5ErkJggg=="/>
							<ss:Column mts:ColumnId="BrokerAccountDescription" ss:Hidden="1" ss:Width="120" ss:StyleID="TopLeftText" mts:Printed="false" mts:Description="Account Description"/>
						</ss:Columns>
						<mts:Constraint mts:PrimaryKey="True">
							<mts:ConstraintColumn mts:ColumnId="ExecutionId" />
						</mts:Constraint>
						<mts:View>
							<mts:ViewColumn mts:ColumnId="ExecutionId" mts:Direction="ascending" />
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

	<template match="Execution">

		<ss:Row>

			<variable name="StateStyle">
				<choose>
					<when test="@StateCode = 0">ActiveStateText</when>
					<when test="@StateCode = 8">ErrorStateText</when>
					<when test="@StateCode = 9">SubmittedStateText</when>
					<otherwise>CenterText</otherwise>
				</choose>
			</variable>
			<variable name="OrderTypeStyle">
				<choose>
					<when test="@OrderTypeCode = 2 or @OrderTypeCode = 4">BuyText</when>
					<when test="@OrderTypeCode = 3 or @OrderTypeCode = 5">SellText</when>
					<otherwise>CenterText</otherwise>
				</choose>
			</variable>
      <variable name="BuyOrSellStyle">
        <choose>
          <when test="@OrderTypeMnemonic='B'">BuyText</when>
          <when test="@OrderTypeMnemonic='S'">SellText</when>
        </choose>
      </variable>
      <variable name="BuyOrSellString">
        <choose>
          <when test="@OrderTypeMnemonic='B'">Buy</when>
          <when test="@OrderTypeMnemonic='S'">Sell</when>
        </choose>
      </variable>
			<if test="@BlotterName">
				<ss:Cell mts:ColumnId="BlotterName">
					<attribute name="ss:StyleID">LeftText</attribute>
					<ss:Data ss:Type="string"><value-of select="@BlotterName" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@CreatedName">
				<ss:Cell mts:ColumnId="CreatedName">
					<attribute name="ss:StyleID">LeftText</attribute>
					<ss:Data ss:Type="string"><value-of select="@CreatedName" /></ss:Data>
				</ss:Cell>
			</if>
      <if test="@CreatedTime">
        <ss:Cell mts:ColumnId="CreatedTime">
          <attribute name="ss:StyleID">Time</attribute>
          <ss:Data ss:Type="dateTime">
            <value-of select="@CreatedTime" />
          </ss:Data>
        </ss:Cell>
      </if>
			<if test="@DestinationShortName">
				<ss:Cell mts:ColumnId="Destination">
					<attribute name="ss:StyleID">LeftText</attribute>
					<ss:Data ss:Type="string"><value-of select="@DestinationShortName" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@DestinationStateMnemonic">
				<ss:Cell mts:ColumnId="DestinationStateMnemonic">
					<attribute name="ss:StyleID"><value-of select="$StateStyle"/></attribute>
					<ss:Data ss:Type="string"><value-of select="@DestinationStateMnemonic" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@Commission">
				<ss:Cell mts:ColumnId="Commission">
					<attribute name="ss:StyleID">Price</attribute>
					<ss:Data ss:Type="decimal"><value-of select="@Commission" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@ExecutionId">
				<ss:Cell mts:ColumnId="ExecutionId">
					<attribute name="ss:StyleID">RightText</attribute>
					<ss:Data ss:Type="int"><value-of select="@ExecutionId" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@ExecutionPrice">
				<ss:Cell mts:ColumnId="ExecutionPrice">
					<attribute name="ss:StyleID">Price</attribute>
					<ss:Data ss:Type="decimal"><value-of select="@ExecutionPrice" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@ExecutionQuantity">
				<ss:Cell mts:ColumnId="ExecutionQuantity">
					<attribute name="ss:StyleID">Quantity</attribute>
					<ss:Data ss:Type="decimal"><value-of select="@ExecutionQuantity" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@GrossValue">
				<ss:Cell mts:ColumnId="GrossValue">
					<attribute name="ss:StyleID">MarketValue</attribute>
					<ss:Data ss:Type="decimal"><value-of select="@GrossValue" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@NetValue">
				<ss:Cell mts:ColumnId="NetValue">
					<attribute name="ss:StyleID">MarketValue</attribute>
					<ss:Data ss:Type="decimal"><value-of select="@NetValue" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@OrderTypeMnemonic">
        <ss:Cell mts:ColumnId="OrderTypeMnemonic">
          <attribute name="ss:StyleID">
            <value-of select="$BuyOrSellStyle" />
          </attribute>
          <ss:Data ss:Type="string">
            <value-of select="$BuyOrSellString" />
          </ss:Data>
        </ss:Cell>
			</if>
			<if test="@SecurityId">
				<ss:Cell mts:ColumnId="SecurityId">
					<attribute name="ss:StyleID">RightText</attribute>
					<ss:Data ss:Type="int"><value-of select="@SecurityId" /></ss:Data>
				</ss:Cell>
			</if>
			<if test="@SecuritySymbol">
				<ss:Cell mts:ColumnId="SecuritySymbol">
					<attribute name="ss:StyleID">LeftText</attribute>
					<ss:Data ss:Type="string">
            <value-of select="concat('   ',@SecuritySymbol)"/>
          </ss:Data>
				</ss:Cell>
			</if>
			<if test="@SourceStateMnemonic">
				<ss:Cell mts:ColumnId="SourceStateMnemonic">
					<attribute name="ss:StyleID"><value-of select="$StateStyle"/></attribute>
					<ss:Data ss:Type="string"><value-of select="@SourceStateMnemonic" /></ss:Data>
				</ss:Cell>
			</if>

			<if test="@TimeInForceName">
				<ss:Cell mts:ColumnId="TimeInForce">
					<attribute name="ss:StyleID">LeftText</attribute>
					<ss:Data ss:Type="string"><value-of select="@TimeInForceName" /></ss:Data>
				</ss:Cell>
			</if>

			<if test="@BrokerId">
				<ss:Cell mts:ColumnId="BrokerId">
					<attribute name="ss:StyleID">LeftText</attribute>
					<ss:Data ss:Type="string">
						<value-of select="@BrokerId" />
					</ss:Data>
				</ss:Cell>
			</if>

			<if test="@BrokerName">
				<ss:Cell mts:ColumnId="BrokerName">
					<attribute name="ss:StyleID">LeftText</attribute>
					<ss:Data ss:Type="string">
						<value-of select="@BrokerName" />
					</ss:Data>
				</ss:Cell>
			</if>

			<if test="@BrokerSymbol">
				<ss:Cell mts:ColumnId="BrokerSymbol">
					<attribute name="ss:StyleID">LeftText</attribute>
					<ss:Data ss:Type="string">
						<value-of select="@BrokerSymbol" />
					</ss:Data>
				</ss:Cell>
			</if>

			<if test="@BrokerAccountId">
				<ss:Cell mts:ColumnId="BrokerAccountId">
					<attribute name="ss:StyleID">LeftText</attribute>
					<ss:Data ss:Type="string">
						<value-of select="@BrokerAccountId" />
					</ss:Data>
				</ss:Cell>
			</if>

			<if test="@BrokerAccountMnemonic">
				<ss:Cell mts:ColumnId="BrokerAccountMnemonic">
					<attribute name="ss:StyleID">LeftText</attribute>
					<ss:Data ss:Type="string">
						<value-of select="@BrokerAccountMnemonic" />
					</ss:Data>
				</ss:Cell>
			</if>

			<if test="@BrokerAccountDescription">
				<ss:Cell mts:ColumnId="BrokerAccountDescription">
					<attribute name="ss:StyleID">LeftText</attribute>
					<ss:Data ss:Type="string">
						<value-of select="@BrokerAccountDescription" />
					</ss:Data>
				</ss:Cell>
			</if>

		</ss:Row>
		
	</template>

</stylesheet>
