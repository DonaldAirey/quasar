<?xml version="1.0" encoding="utf-8" standalone="yes"?>
<stylesheet version="1.0" xmlns="http://www.w3.org/1999/XSL/Transform"
	xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
	xmlns:x="urn:schemas-microsoft-com:office:excel"
	xmlns:c="urn:schemas-microsoft-com:office:component:spreadsheet"
	xmlns:mts="urn:schemas-markthreesoftware-com:stylesheet">

  <!-- These parameters are used by the loader to specify the attributes of the stylesheet. -->
  <mts:stylesheetId>MATCH VIEWER</mts:stylesheetId>
  <mts:stylesheetTypeCode>MATCH</mts:stylesheetTypeCode>
  <mts:name>Match Viewer</mts:name>

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
        <ss:Style ss:ID="HighBucket" ss:Parent="Default">
          <mts:Display>
            <ss:Font ss:Color="#FF00FF" ss:FontName="Times New Roman" ss:Size="10" ss:Bold="1"/>
						<ss:Alignment ss:Horizontal="Center" />
					</mts:Display>
        </ss:Style>
        <ss:Style ss:ID="MidBucket" ss:Parent="Default">
          <mts:Display>
            <ss:Font ss:Color="#0000CF" ss:FontName="Times New Roman" ss:Size="10" ss:Bold="1"/>
						<ss:Alignment ss:Horizontal="Right" />
					</mts:Display>
        </ss:Style>
        <ss:Style ss:ID="LowBucket" ss:Parent="Default">
          <mts:Display>
            <ss:Font ss:Color="#00FFAF" ss:FontName="Times New Roman" ss:Size="10" ss:Bold="1"/>
						<ss:Alignment ss:Horizontal="Right" />
					</mts:Display>
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
        <ss:Style ss:ID="Status" ss:Parent="CenterText">
          <ss:Font ss:FontName="MS Sans Serif" ss:Bold="1" ss:Size="12" />
        </ss:Style>
        <ss:Style ss:ID="CoolCountdown" ss:Parent="CenterText">
          <ss:Font ss:FontName="MS Sans Serif" ss:Bold="1" ss:Size="12" ss:Color="#00FF00"/>
        </ss:Style>
        <ss:Style ss:ID="HotCountdown" ss:Parent="CenterText">
          <ss:Font ss:FontName="MS Sans Serif" ss:Bold="1" ss:Size="13" ss:Color="#FF0000"/>
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
        <ss:Style ss:ID="SoftBidPrice" ss:Parent="RightText">
          <ss:NumberFormat ss:Format="&quot;Bid&quot;+#,##0.00;&quot;Bid&quot;-#,##0.00;"/>
        </ss:Style>
        <ss:Style ss:ID="SoftAskPrice" ss:Parent="RightText">
          <ss:NumberFormat ss:Format="&quot;Ask&quot;+#,##0.00;&quot;Ask&quot;-#,##0.00;"/>
        </ss:Style>
        <ss:Style ss:ID="Quantity" ss:Parent="RightText">
          <ss:NumberFormat ss:Format="#,##0"/>
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
        <ss:Style ss:ID="ShortDate" ss:Parent="CenterText">
          <ss:NumberFormat ss:Format="m/d;@"/>
        </ss:Style>
        <ss:Style ss:ID="DateTime" ss:Parent="LeftText">
          <ss:NumberFormat ss:Format="mm-dd-yy h:mm A/P"/>
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
        <ss:Style ss:ID="BuyStatusText" ss:Parent="CenterText">
          <mts:Display>
            <ss:Font ss:Color="#000064"/>
          </mts:Display>
        </ss:Style>
        <ss:Style ss:ID="SellStatusText" ss:Parent="CenterText">
          <mts:Display>
            <ss:Font ss:Color="#640000"/>
          </mts:Display>
        </ss:Style>
        <ss:Style ss:ID="ErrorStatusText" ss:Parent="CenterText">
          <mts:Display>
            <ss:Font ss:Color="#000064"/>
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
        </x:WorksheetOptions>

        <ss:Table mts:HeaderHeight="18">

          <ss:Columns>
            <ss:Column mts:ColumnId="SecuritySymbol" ss:Width="86" ss:StyleID="TopLeftText" mts:PrintWidth="86" mts:Description="Symbol" mts:Image="iVBORw0KGgoAAAANSUhEUgAAAHIAAAAXCAYAAADX5BuUAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAABDhJREFUaEPtWj2IE0EUHsEihUWKK1LYHFh4YGHgCgNn4YKFAQsPLDywkGAhV0mwkOMaCRYSrpBwheQUhLtCyBUHSaHERoiFcCmEWAixUNjCIoVFUhw855vbicO4u5nMbE7i7YbH/r33dvb7Zua9N5szxDfGt51XO6z7qcv8Hz4b8V+Sm3fVY8UbRdbtdtnwaJik6/n3dcTYaDRirXct63fJnM0wBiKxlR+WqfK0QvUXddp9vRsve/z+JFF81LZr1P/Wp2armYqOwUGT6i855hJPFfsIjBt7DZICbFdvrxJr7Ddo/cE6VZ9VqbpVpdrzWuLSft+m4a9hKhEYdD50psOck1ffrgsRRN7iRIK80t0SbTzecJdN7kOK4i8lMroTDwYDApGx+ANT4BnsK08qpErxZpEYLmBoYmqdlTTfHo/Iwc9BKhoGvu8TOro19o/K5F33iJXulYgnIuGOuFI5TuLIV+yaPA6ASP+7byyI3nECX/J+nF/VxzTPP0ld5A6ROOsYh/BRWCkQsknyVrxQR4lMt3xKQBwGkf2vfWMBAVJfkqGe41i/HuXfVG+a9iWpi47ugnVhmRMJEnGAHqE7q2xW/sQ8Nf5FHENfCuZzedx40yDEgt7nnpVIIv6Vve1zTe3Q0VXsdNzleRi24Cx/OU+scKUgDtburP1NpBZU9SBreo5yBrGg87FjJZJI1V6dMuV1fSrWr+N8kl2YL9XO9h3i7FB6mGKp6yFRXbyweDwiQaQkEyNz3Dt4XYna0kbEiAxsUZuijmzzpMdGJPi6rXo96hg2+j3Vj4kddKQfm/ZPskEdOS3GwBck5pfzlFvIEUMNIomc1R61ae9LT8RKG5Fg67bq9Sgd2OgjVfUTllDJ+3E+bd4jyga1oAv22WyWZ633OavBiJzVHrUq4sXE1aCIlQwJqG6vXo/SgU2cnqmdbdtN7NDRXbAXRMpg6eJoki1WjQSRwfITphJMt2JvIBJsXVe9ruvgXOqH3ZP3p7GT/vAeJu3WdaKWPtHRJ2EYd18Q6dobTBrgQqQ+9enkhJEZRmKcnvqMOP825JnYuBKZOZchhhrGhAwXHTG18hgpphm1N08xKk0AmUsdjoHrrCiIxPKQdy3IXHkGhCwoaUFDD7uHhE4jiASBqYjpGYmOyFNscA9yGzG1gkgs07mMOBNbkAgyMSrDSMQLnSpyg9yg6pjoAPvsAojkdRZimAkZLjogCqv86DggDOfzKrbtH3dUfH/kMxPeH58QXXCFbe48ryPxURkfKV2dmdij4SASo9MWjHklX7ZbfEQOwktSpd/SxaVjIjEqx3FyxjUlCERhjGdi1d+kzvqfdEAkkr8kRqIcPGLRXBDZalNtKyhKL/Fk5wQEvREvdNpE5CMJ44v18vF/dkAoQBVrrwk/KPU328EhvkfKP19hL/4ctd8UhOKLNXpPKvOBwW8KeXmMcGZ7FAAAAABJRU5ErkJggg=="/>
            <ss:Column mts:ColumnId="OrderTypeImage" ss:Width="52" ss:StyleID="TopLeftText" mts:PrintWidth="52" mts:Description="Side" mts:Image="iVBORw0KGgoAAAANSUhEUgAAAEUAAAAXCAYAAABdy4LVAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAhpJREFUWEftWDFrwlAQTqFDhg4ZHBy6CB0qdGjAoYFOQhehQwNdBIeSX1CCQxGXEpycSnAKHQq6+f8cCtd3L3nh8kx8TxNJIQkcvuS+u3f3eXfmeeG+uLD73RntFTNgXpqG4b/7EH1HsP5Zn082zPexQuORbfNipRiVnmC3my0ICVchuK8uGME8gPArbK4wIqJVxIWTwjrHCD4DmH3M6pE521eITgyIVeFUGKFPPjF/KqPnUUyKP/UB26jxwngYPg2TSpnOYmLOLYeIV+0tbA/hVBh5/xxfzqNTc/uoWqEGvTMgpODAxf7GTyFpvyd9T3V5ayAX2uIlfAgV9UnxRb4pnsZVFKcqD1lPc8Z5Zd/bcaWMJ+PMsJGHj869SFBgVfdFOJ29zoXxJh70bnqMlEUA9sAG782LK4XdC9mrFKKjOFynJBAMPpNxOnYqGzlOGa/S5+GREOSh2+kmpLCSwbIpI7QVZD95OvoM18LmkJ8y8enaWpYFxnKxLEWGDgGIEdcpa92EqsBxUnj7lKwSHWKOIaXOaqmUFNoCcjX89+qgX6p5Zca/PlVUCq2EqkiRia4iTpWPLCls8uL0PVV4Asw+U/qJv72hKuFSQgvsT43pKLtkhPD2ES8sKgaborc6jBQ8BDYlYZ08u9fsPQXfZnXATcH0b/tg4FG5KQnr5MkPhHwY3bUiOHAekBScui0pKQf8/xQ8CLaS5eAPeWenk07+XhEAAAAASUVORK5CYII="/>
            <ss:Column mts:ColumnId="TimeLeft" ss:Width="78" ss:StyleID="TopLeftText" mts:PrintWidth="81" mts:Description="Status" mts:Image="iVBORw0KGgoAAAANSUhEUgAAAGsAAAAXCAYAAAAMX7G2AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAsZJREFUaEPtWb9LAzEUjuDQweEGhw4ugoOCgwUHD5wEF8HBgovgIP0LpDhI6SKHk5MUp+Ig6Ob/5yDEvHfNkT5zec+7Xo9iAo82l/cr33cvSdO1/kVff31/qdjaQ6Cz3sHgLA/D26Gevk71+9t7Nfkwdn8VNxa19eXh6qxqns4cPj8+tZXJy0T3L/tawoPKxpmePE+iLBMDQ9D0ZYqCZJnVTcKDyh4yPbofLV7GxqcViX/Q5fQkOpwPOr7MPGf5A+aunJ2fYZ+bP5I1vBtiGUZpAQOD/cnpCZLF8ZBX1t0oV6wjIbI5v9Y2pCfR4eJwLyVnL8mhTCdgmx6neWUxPDS3DP51OfrH+unhjCwGg4Is2OBgj4HPMin2IHedL/nu+tBOg+fQbDzr0+rTHNyYkhypX7B3G51DaL5uTnXzdOc1990Q1DvoFXtWaI5I1tX11dyGRzfAOn0LlPVB+3V8l9niy0A28WXErTKXwfVAb+9sY74cDyp7zHTvsKcHN4O8skx/UVIARHwimCVxuBy4cfDr81+WS9W5cnlIxoEowL672UU8OB5yskwZNiEWoJBvd4myevSZ60c6BnrgjzYuBrXz+fDp1MEvSZKcLIYH9fT4xCpxTsrGObJ8JFAwXbB8QNMYZT7rPpfErooTkCXhodXKqktWiEg6tmiyfLHrkCWqLIlS1SSarqxFkxUiWFLxVXHqbHRkyyCcQqoGkdj5CAvtCxJQqlSkpLJaJUvAAx7dESBzKmlKfm3yTqy5AwPJ4dfpgDyAfKl9qF8Q5rGzc/fp0DF82QI+xDjODhR4wBDwoODyUFIhUaeZEzPgmmwmeInLYazg8pZTiuPNEQXYdre6eInO4azgVzOnFMebJWtvdw9vLzicFVzPc0pxvFmy4CJXwoPCzXA/SpsYpEdpfrhjeFBYNZGsVjGA/7MkPCi4wI2yGhj8AIT7ZLn92ON6AAAAAElFTkSuQmCC"/>
            <ss:Column mts:ColumnId="SubmittedQuantity" ss:Width="75" ss:StyleID="TopLeftText" mts:PrintWidth="78" mts:Description="Available" mts:Image="iVBORw0KGgoAAAANSUhEUgAAAGgAAAAXCAYAAADnaAq1AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAtZJREFUaEPtWb9r3DAUVqHDDR08ZLihS6BDAx1qyFBDp0CXQIcedAlkKPcXhCNDObIUkylTOTIdHQrJlv8vQ0HV0/m5z6+SnuQ7+26Q4SHZer/8fXqSzvfi/PO5VvkaHYHJy4mN+fznORz74fFBW/ntEN9zl67PHv3HttQPt5FixOYl+embK/j12D49PmmU1f1Kz77O9OJqode/1m7smxzV6udKZxkBA0PK+n5txRL0Zabrm1rEXi2/L/Xyxgi0YwnEQ4mJGZNfjE5MLK6TmivaYz5NW/+oNRXYWuBewlxBmWUZGYPrhT77dGYJWph+CP9/BIHiUBKaBFJMtA3pxehIcXB8m1x9Phyxq4/VpoKul0HcN0tcltExqE4bggTsFewFsFlhC328x30Cn7labS6u3+4vdP0mfe4HfKAf7EPL42OOPh0Yp5cvbyk/Oh56dxdWNGeKaadvSCnfl+0e5MIffSu+eaXct8CyDTDFB+hyPzF+Qzox9qk57lJ/fjnXx2+OLUEXlxedwwOPo+pbUzFGLMNNn7a+56DTma0OW5e/mGctwMwnzcWlg+M++5jYQ+pAfkBOeVrq6dHU4g39+be5F38FpdZHAASwwwt9UNL4OB+j4+gvxie3o7ax9n3eeZc2RVFsCBLw35qgECA45iOKk+q6pxMgph+KxSeTBM6Q40DQ3e3dsAR11rimolwA8Rf1VZ2PsBTiQlV7aAQNVkGxSwqtoNCyFEvYthU0ZEWk+p68mgy3xHGCpGXuUJY4PmFSQd2lviXInOIknwpOESnS2egb2/+WOvO8ne3EP9eLuYfcfBcdw3cI5WInVuL77ly/ORTYQwISFMip9yFBYj6Ph0/HxVFhv15IOGWCev7MkICVxqevp/YjqaSXCdoTQSdvT+xXhEzQngiQgIePpfCXg6SnyndmrcwyOgbVh2pzYBGwzwTtaXLC/0G2eiSC4ENdlsPF4C82ooxu/4wYjAAAAABJRU5ErkJggg=="/>
            <ss:Column mts:ColumnId="LastPrice" ss:Width="55" ss:StyleID="TopLeftText" mts:PrintWidth="55" mts:Description="Last" mts:Image="iVBORw0KGgoAAAANSUhEUgAAAEkAAAAXCAYAAABH92JbAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAm1JREFUWEftmDFLAzEUxyM4dHC4waGDS8FBwcGCgwe6FFwKDhZcBAfpJ5DiIMVFDgc5HKR0kMNBqINwDkIXQT+XgxDz0qY8Y/LetfZUvBYezV3+yUt+fe/dpXP13boUBf2U5kt652/vb14Cte2aEL27npya3au5xjXs3x7rWhvW+NbuWUN6n0pjnW5HNvYbsnXckslt4mUAOtG57sjCmNpw0k20aUh7DRmdReT+X15fpGiftmXudqZ8GMviD7ScbhzNUBudRxIblBq4pnxpSBBuhbSTlqzt1DSklmr7GPSfARIIpm0UeM6XGUvpxtEQ2nArHETSSdvLoP/U/6F041Lnl/rDjSEkwn/6mEoBhQtsVDNU/pp78G1/XHo81tXG8/naLv9wD+ttDdVv+rDmU1uBqa5XRzXJMMDf0E4fAJJVzFzXBlQWbVYNzJlVm4euediUleWKXsPB4YF3LfB6JKILFUlDM78UvgftESSktTXjXmtIzHz6lyc0XL9rLIwBQNWNqiwvlvX80G4eNXU2wTWeN7lJpICQ48xA8ulwSmKNnarQ57rH+c+rPwiCASSCAbxDfhsSBuhrGzhmMRz0vKDY8wKk+CImIcVX8eSQYKO2UxckTvdTQFx+skRSfDklSFQKuVLxr0RSaaHEptvEkLKkGI4gG8qfgqSeblQ0D9JNVXbKvlRadMOMozQjIMjPp+hi/HPrm6h/WKh1uhlInnXAuS5T4f7NupGn72Ax0IdbzkehIZWXyvpgO4NEvAOtrqzqt+0ZJAISHHDh7xIe0poq3AW1cDMcPLSY/QtO8J/74f8kHUUcJDjYzYxm8AG/+0s9Q2zOcQAAAABJRU5ErkJggg=="/>
            <ss:Column mts:ColumnId="RunningVolume" ss:Width="45" ss:StyleID="TopLeftText" mts:PrintWidth="47" mts:Description="RunningVolume" mts:Image="iVBORw0KGgoAAAANSUhEUgAAAD4AAAAXCAYAAABTYvy6AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAk1JREFUWEflmL9Lw0AUx8//x8H+SRKcioMEBwkuEhwkOEhwkOAgpINQByEOQroU4iC0gxAHoQ4OGTpkcMig8Hzf11pKNZCLOalW+JImvLt3n/fj7nAtfUhpOByq4r1QK/U3eh5RdBOtnFTxWtAq6tfA3UOPnH13aYIs4Pk4Nyrv2CdnzyX3wDPqR4dDwLOXzKiS/kCy7Ry4Rv3ocAj46GnUmOLbmFxAToUs27uOqHgjynMOdJZLyTv4VhRE/L3JNVSZS+V5TjjSmlLST8g78gQk7vFvLnP3iEt936P2NoNON1MEJrqOZu9N+a86j8qyjJK7pDHheARwe9sWeGurLb3tHQfyRADsHacxf3XXrmShXJ4m5B5ytqelnj5ygO9TLvlJn5vwpzOnSh9T6l51jQh9DXj/JCCbd/XwkkubyIgvXQaFngg7oRE50xJHb6PP8QwuIiO+dBkm4BcMzgrOAwrOWHhqaDZ2YcwE1pYsB52I/NNQSr3Mft5nmU2VsVXmaQR83hF29Yj3jLDTZVAucfT0mI8wVnKX8kXG0wqqTgJ0bKXHpUw+M14j6+gvADt7XNJ8ScERNunvr5D4DnudRZqwVYPhQM5TAUeZ1xDG2jv2rGVwJ4FwMVmcD1XwE1911vfdGAVowCPr3xn4p36tYJQtEHd2VEZTAHXnUQBDmca9WBaDd13pjqtiX2ZTZez8+svsVWujJaAAR/Z1J9YN0rLYCzgEYGw6uP3gPzK65+Jfs1etdQafk7Vl8a6Mu/X/1hfwxUD813dlbVq0ivoAEUcdVOyKc6cAAAAASUVORK5CYII="/>
            <ss:Column mts:ColumnId="VolumeCategoryMnemonic" ss:Width="48" ss:StyleID="TopLeftText" mts:PrintWidth="48" mts:Description="ADV" mts:Image="iVBORw0KGgoAAAANSUhEUgAAAEAAAAAXCAYAAAC74kmRAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAA69JREFUWEfVmDFoE1EYx1Nw6OCQwSGDg9ksOBioYKCLgQ4GOhjoYMBBQgcJHSR0kNBFQgc5OkjoIMFBaAchHYRkUOIipIOQDkI6CF0cMjhkcEhB4fP9vvbC5Ux6766HpQd/krt77937/t///713N9d425DEZR5/EomTk5NE+1M70izSN9OJzL1MovS0NO4/Zw7rwVrtllwqPrSEJOzu7Z7i3Qy4932/zitHyutlabxpiHtYB0/D0a+RXDa6X7pSf123w45pZ9DYaegv/aqbVSmvla8mAcPhUCCg+qJqhdpmTSbwsqYEVJ5XxNl2lIRQChj+HIoN+t/6KtXM3YwUVgpWfWzGHQwG0vnc0QAuipohIzQBgx8DCcLx92Npvm9K7kFOSegd9gL7BI3pvU8NqmwYAoJwHkmmDhSfFMMTQHBBoEZQaFKplHrO9WzhUSGwb9DY3G+ZQmhrgX9UAmkQs17RBIVWANKehdFoJL2Dng5OdpLXk1JcLUphtaB+yz/Mz+x73rj+e8395oSv8bTX55yPsWH+A1/NqJgEZRez4QnoHnRlFszaOg6YZSZ7Pyuddkfyy3mZvzYvKOC8/rb3WP7wrw00cAjxEVB8XNT6FFoBnY8d8QPJ85D8Sl7St9IaJOvtwu0FzbhLAoxTxGg/bRzba9SV2pYh4AyqAM/5xH+zCngVgjLd4CNZAPn50T/qq7wJmOyQfaRP5p0tR3pfexowbVAF1X7aOLbXWM/J3kWBIkMrYLwD8+ywKHSpGynBAmSHCVKo8GX5WVkD7h/2NWhIgflp49he43kXDZ7+pbVSeAII0A8kZTYTWvWROVWfope5k1Gfcn58dLp6sDxilWnj+K/N2ubq2DEoAGuEVsC0iSMlgkfibFL4xQYsN7SHAPYCWAU7sCrYEDCrTVwEuO8DoXaCUxVg/A4BurX8LWoFvN7aN+u1YZlMUgixhVsHIhNgiKWix6EA5hNeAWYCMOeFawHkjt/ZD7AX0KJogqcqI/3sUlYt4u9vfX5WX/BuZtEUwagw9skt51StsRBA5pPJpC59/mDIONcpftQE62D9RJ/VHiemAsjbYCQC9JXSB7JMYcPz/nvYwXyE0ODx/7T+s66NyeL93zyDdqwqccifpET6HhAmgDja6scPEzxkqPRjqP6M0dxrRiPAdq2Oqx0EYLG4Mk/w7EMoypEUwGT+J7AQ9okNhoD6dj06AbFNJM6gLMfKLeU0eXxP8B6h9gFk5KrB/fzlfsydiD7kJ7G/Yj8OvwrbKu8AAAAASUVORK5CYII="/>

            <ss:Column mts:ColumnId="SecurityImage" ss:Hidden="1" ss:Width="85" ss:StyleID="TopLeftText" mts:PrintWidth="85" mts:Description="Symbol" mts:Image="iVBORw0KGgoAAAANSUhEUgAAAHEAAAAXCAYAAAA806CXAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAA9BJREFUaEPtWjGLE0EUHv+PhYErDGhhOq8zYOGBhQQLuUqChRzXSLCQcIWEKySnINwVQq4QkkKJjRAL4bY4iIUQC4UtLFJYJMXBc765ndw4zmwmO5NsxN3jsTvZmbez3/fem/dm7xLxg/Hj4NUBiz5HLP4Rsyn/C3lUrlfY5s1NFkURm5xNQqr+93WdMTadTlnvfS/7u4BEHPWHdWo8bVD7RZsOXx+myxG/P08UHa39Fo2+jajb6xaiY/C2S+2XHHOJp4q9BePOUYekAFvWOe7Q9oNtaj5rUnOvSa3nreDS/9Cnya9JIRYMBh8Hi2HOiWvvt4UIEkFc7W6Ndh7v+Msu1yFF0VeQaDfg8XhMIDEVf2AKPJNz40mDVGFoVG9XRThdlnTfnXvi+Oe4EA2DOI4JRp4Z+0d1YrV7NeJJh1kJ71BPkzTilXFdHvdBYvw9LsSAAXIFK846xgY+kDVS5VrFqCRIiOVhAOsuSBx9HQURxpBRX0govXnpgZH7YM1AYHmjLEjUFTV2GxdrnLreWa7RXwrit7zuvOkQYv/wdBhMJIkhdealC0auYqfjLtsmbMEZK18tU+lKibbubP1NoraA6guqaxslC2L/4NMgmEgSQ+rMSxfKC1cs9X5ISoUngkRJJDxyZhW8bkTtmEWEJyZjUXuiTuzzBCeUSBJt+tRwq/bRQzHa8r5tTKg52/SgTlwUY+ALAksbJWLVW9UZiZLM0GfUnsMvQ7E2hhIJuEmfes/lGjps/ULNN00Paj0fzFntPmcz8cRlnVGLYr2Zu8szbxdIuS9BV3XiN7TVey7XaWNCztmmC0bugz3DwuijwGUsdoMEicmWEsIHQqw4ZxRJjjoev6Gt3tP7qSFTH2sar/bB/LPOV2ytWbYzYeQuONr6MF8rcHn4Kki0Eaf/biMhjXgf4lzGepOIGsWFCJ8+IpzyNVGEE9WaM3qjKTlx8TjTOJM3So90IcC7D8fANxoybPlUbiQZKs90kO2EFkzyJDohGIwgEeTlIDby85iLXE6Q1Ii8JAvuSS4jSMTWm4+nuYwFgSAS3mgCDS+zbDCFh2nGY/pt2fNQ84GmZ1ID7BlqF6xZLkT49AFJ2K2H0eAl0M5DdG9cZA6+854ZR5Lk4Nn4DOiDqyARH4TxgdFXkct4TBokwit9AVkE/HXqK7NUvH+o8k6QCG+crYtLrhkxeRS+eCZ271dRh63TM0AiEr0QHigd55zEXp9ae0nBeZknNisQWCFe5n8TkX8ExleQKA8AKvZSAz+k0Ldcx/iDRPGPTMdd4R340gyrKWT9MfgN7c6OwbXJxPQAAAAASUVORK5CYII="/>
            <ss:Column mts:ColumnId="MatchId" ss:Hidden="1" ss:Width="30" ss:StyleID="TopRightText" mts:PrintWidth="20" mts:Description="Match Id"/>
            <ss:Column mts:ColumnId="OrderTypeMnemonic" ss:Hidden="1" ss:Width="23" ss:StyleID="TopLeftText" mts:PrintWidth="45" mts:Description="Side" />
            <ss:Column mts:ColumnId="StatusCode" ss:Hidden="1" ss:Width="15" ss:StyleID="TopCenterText" mts:PrintWidth="20" mts:Description="Status"/>
            <ss:Column mts:ColumnId="StatusName" ss:Hidden="1" ss:Width="47" ss:StyleID="TopCenterText" mts:PrintWidth="35" mts:Description="Status"/>
            <ss:Column mts:ColumnId="WorkingOrderId" ss:Hidden="1" ss:Width="30" ss:StyleID="TopRightText" mts:PrintWidth="24" mts:Description="Order Id" />
            <ss:Column mts:ColumnId="ContraOrderId" ss:Hidden="1" ss:Width="30" ss:StyleID="TopRightText" mts:PrintWidth="24" mts:Description="Contra" />
            <ss:Column mts:ColumnId="SecurityId" ss:Hidden="1" ss:Width="60" ss:StyleID="TopRightText" mts:Printed="false" mts:Description="Security" />
            <ss:Column mts:ColumnId="AskPrice" ss:Hidden="1" ss:Width="30" ss:StyleID="TopRightText" mts:Printed="false" mts:Description="Ask"/>
            <ss:Column mts:ColumnId="BidPrice" ss:Hidden="1" ss:Width="30" ss:StyleID="TopRightText" mts:Printed="false" mts:Description="Bid"/>
            <ss:Column mts:ColumnId="Volume" ss:Hidden="1" ss:Width="37" ss:StyleID="TopRightText" mts:PrintWidth="37" mts:Description="Volume"/>
            <ss:Column mts:ColumnId="AverageDailyVolume" ss:Hidden="1" ss:Width="40" ss:StyleID="TopRightText" mts:PrintWidth="37" mts:Description="ADV"/>
            <ss:Column mts:ColumnId="MarketCapitalization" ss:Hidden="1" ss:Width="40" ss:StyleID="TopRightText" mts:PrintWidth="37" mts:Description="Mkt. Cap."/>
            <ss:Column mts:ColumnId="ContraStandup" ss:Hidden="1" ss:Width="55" ss:StyleID="TopRightText" mts:PrintWidth="37" mts:Description="Contra"/>
            <ss:Column mts:ColumnId="AveragePrice" ss:Hidden="1" ss:Width="30" ss:StyleID="TopRightText" mts:PrintWidth="37" mts:Description="Avg.&#10;Price"/>
            <ss:Column mts:ColumnId="LimitPrice" ss:Hidden="1" ss:Width="30" ss:StyleID="TopRightText" mts:PrintWidth="37" mts:Description="Limit&#10;Price"/>
            <ss:Column mts:ColumnId="LimitTypeMnemonic" ss:Hidden="1" ss:Width="50" ss:StyleID="TopLeftText" mts:Description="Limit&#10;Type"/>
            <ss:Column mts:ColumnId="OmsFixNote" ss:Hidden="1" ss:Width="60" ss:StyleID="TopLeftText" mts:Printed="false"  mts:Description="OMS Fix Note" />
          </ss:Columns>
          <mts:Constraint mts:PrimaryKey="True">
            <mts:ConstraintColumn mts:ColumnId="MatchId" />
          </mts:Constraint>
          <mts:View>
            <mts:ViewColumn mts:ColumnId="MatchId" mts:Direction="descending" />
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

  <template match="Match">

    <ss:Row ss:Height="22" mts:PrintHeight="13">

      <variable name="SideStyle">
        <choose>
          <when test="@StatusName = 'Held'">HeldStatusText</when>
          <when test="@StatusName != 'Filled' and (@DirectionCode = 0 or @DirectionCode = 1)">BuyStatusText</when>
          <when test="@StatusName != 'Filled' and (@DirectionCode = 2 or @DirectionCode = 3)">SellStatusText</when>
          <otherwise>FilledStatusText</otherwise>
        </choose>
      </variable>
      <variable name="BucketStyle">
        <choose>
          <when test="@VolumeCategoryMnemonic='High'">HighBucket</when>
          <when test="@VolumeCategoryMnemonic='Mid'">MidBucket</when>
          <otherwise>LowBucket</otherwise>
        </choose>
      </variable>
      <if test="@AverageDailyVolume">
        <ss:Cell mts:ColumnId="AverageDailyVolume">
          <attribute name="ss:StyleID">Quantity</attribute>
          <ss:Data ss:Type="decimal">
            <value-of select="@AverageDailyVolume" />
          </ss:Data>
        </ss:Cell>
      </if>
      <if test="@SecurityImage">
        <ss:Cell mts:ColumnId="SecurityImage">
          <ss:Data ss:Type="image">
            <value-of select="@SecurityImage" />
          </ss:Data>
        </ss:Cell>
      </if>
      <if test="@MatchId">
        <ss:Cell mts:ColumnId="MatchId">
          <attribute name="ss:StyleID">RightText</attribute>
          <ss:Data ss:Type="int">
            <value-of select="@MatchId" />
          </ss:Data>
        </ss:Cell>
      </if>
      <if test="@MarketCapitalization">
        <ss:Cell mts:ColumnId="MarketCapitalization">
          <attribute name="ss:StyleID">Quantity</attribute>
          <ss:Data ss:Type="decimal">
            <value-of select="@MarketCapitalization" />
          </ss:Data>
        </ss:Cell>
      </if>
      <if test="@OrderTypeCode">
        <ss:Cell mts:ColumnId="OrderTypeImage">
          <choose>
            <when test="@OrderTypeCode=0">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAACoAAAAZCAIAAADITysPAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAZpJREFUSEvNVkFqwzAQ1Afzjzwihz4gL8gDmnvoVdfmUEoCPflSCD2UQKE+FHJ0x5lmu92VbRFEVGMSW6w0OzuzkkPoQpU77j677oyNvxtfAI1PdeH/F/twF5L37H62el4d2kNBgfrie/btqZUMCNZ8NPOHOQfxXCqDQe0NPPDAm4PIoyR8UnsP37fHRZeS8Ennj7BfPi4Jry2SHDEeQkx8jXrltPae6P59D+ux8sevo7D3WeoR6MVZ2jGwDga5yLT2mgGwje/G4QGwfdsyZv2yZtIoAO6faiWd79kDdREXXAgPmewZJgVAQ/GVD1nstcvQ94bKJHut96bZgAYW+c0+kz0n+P0gB14KwI0LNvoDn+l84+3kq1eNYRCeiSIDXc5c53MOEjfFF11H3KDLJgYc014XWdpMek9bT2gxDFvC0NbESHNkJNgnzxvxPOyjq6cLC12RhN+LGM9Tw8y93XmPDUDafdr5Js3rXlEqFIOdpiWbdv51eLa2lyMKknvqfZsM9X0ReHGrd0zC+TW/dPHFGXenKr9nFape33NPTKfHMjI/AAAAAElFTkSuQmCC</ss:Data>
            </when>
            <otherwise>
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAACoAAAAbCAIAAACFh4oEAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAV1JREFUSEvtlrERwjAMRWFA9mAIRmACFkhPT01LS0lJxwbmhR8UoRgucMahwOfjnKDoS19fSuYppdmEC/BpNnmTeYtdf91B//D1ubeKD8g/n9Nm04mRw/GYLpex+vAqvnVUv0OK+dqDvVi0zzRNaw/watW5GEmRQco+XJqTPPx6nQFbLsdmP8R7D96sT6c+28OhFryYZ3OgEAOu+jtUR8YQFizl4RPy9/u+Wngn7+ySIDBGmIoVlWTx3iMfFz4CHkb8Ye12D/oQwHZbCF6CNw3iGul5ekMvCJ6bZbK3XFGfNK8NzwEgvKsKwOPRqih3NoII5bWaysBTWr808ryYjRJfEd8jnyt/2HI2SUyDtJwALFDKhFbKZK8IrOUE5scAfKjj+dV0ArtM40li+LKpAgbnIIjwWrIqBDFmtWkk3Q7/z4041ypc318Kv0D+a8l86d9OehWofg4xxUe+i+YKG4pTPaKQXHUAAAAASUVORK5CYII=</ss:Data>
            </otherwise>
          </choose>
        </ss:Cell>
      </if>
      <if test="@OrderTypeMnemonic">
        <ss:Cell mts:ColumnId="OrderTypeMnemonic">
          <attribute name="ss:StyleID">LeftText</attribute>
          <ss:Data ss:Type="string">
            <value-of select="@OrderTypeMnemonic" />
          </ss:Data>
        </ss:Cell>
      </if>
      <if test="@SecurityId">
        <ss:Cell mts:ColumnId="SecurityId">
          <attribute name="ss:StyleID">RightText</attribute>
          <ss:Data ss:Type="int">
            <value-of select="@SecurityId" />
          </ss:Data>
        </ss:Cell>
      </if>
      <if test="@SecuritySymbol">
        <ss:Cell mts:ColumnId="SecuritySymbol">
          <attribute name="ss:StyleID">Status</attribute>
          <ss:Data ss:Type="string">
            <value-of select="@SecuritySymbol" />
          </ss:Data>
        </ss:Cell>
      </if>
      <if test="@StatusCode">
        <ss:Cell mts:ColumnId="StatusCode">
          <attribute name="ss:StyleID">Status</attribute>
          <ss:Data ss:Type="string">
            <value-of select="@StatusCode" />
          </ss:Data>
        </ss:Cell>
      </if>
      <if test="@StatusName">
        <ss:Cell mts:ColumnId="StatusName">
          <attribute name="ss:StyleID">Status</attribute>
          <ss:Data ss:Type="string">
            <value-of select="@StatusName" />
          </ss:Data>
        </ss:Cell>
      </if>



      <if test="@InterpolatedVolume">
        <ss:Cell mts:ColumnId="RunningVolume">
          <choose>

            <when test="@InterpolatedVolume&lt;=-80">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAD4AAAAUCAIAAABalBlDAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAANVJREFUSEvtV8ENgzAMdMcrU3WOihlAqpQ+6TjwbyX4pQ4uiTGItB9cJEd+oFyQzs7FuZy89zCN4TU0j6bt2qqu3N3F+T/9QOpx9M/e3Vx5LYtzwecFdQ7xzGnZfmiGOvgLQAxJTgsdSc+KtFJ1xLGQY2ACS+o6qFHX2BOrulVdNNbt9mCCMcGYYJiHWTsu6U7VQjPHNNCiSGomMxM8lhbKLNemh2G8uY35/K6FTux/tl8zT7sQzE7oV32dzGXUiTDjimje9BLXoHb5hkhZKKGAb6KDxhsxMfwY9q9VVwAAAABJRU5ErkJggg==</ss:Data>
            </when>
            <when test="@InterpolatedVolume&lt;=-60and@InterpolatedVolume&gt;-80">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAD4AAAAUCAIAAABalBlDAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAOJJREFUSEvtl1EKwyAMhrPjrafaOUbP0MLAPXbH6d43aN+cNZvaWNRRAxtE8lD8JXymSdSD1ho+Y37Ow20Y72PXd+qq3PyPfhh0N6bHpC6qPbfNsQnnCXoohTvHZRVV4op6zqCDPgE4o3B8apraqqsgbUTd6CaQ1swGYnQuVdAl6qSI06koCSMJIwnjqkD6evUDi6nD+DN1q8HVUXehL1hovsPgZWa5Y/GpBdC4JJnrAXd4jXk751PL6L8u05XbKGGqqQX0OXT8MS5PYo98ao6+AN3Sx28I75lPTdKDeRP9qb0AwHMIniwqovUAAAAASUVORK5CYII=</ss:Data>
            </when>
            <when test="@InterpolatedVolume&lt;=-40and@InterpolatedVolume&gt;-60">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAD4AAAAUCAIAAABalBlDAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAOpJREFUSEvtl1EOgyAMhtnx5ql2jsUzaLKEPbrj6PtM9I3V1QECU0u3ZEtK+qD8xH6UUuFgjFGvNvZjc2varq3qSl+17f/RB0C3bbgP+qLLc1kcC78/QPclf+Y4jKQGg0mvC08JdGVOSlkL4TgqCTM1eBsd8gkNJhCj56uCPkcgmTD5cYUVXVkxibpEPSiOkjDvNoUUx4yqLxWGVmHcPzW1EfeqX4z6hIXmDgJ4mJnOWByVDY0fWN2mHrd/jJldc9RP0JMrzMJplDAElU2/hY4LY/Mk9sdRefQ70J/08R3C+eWoDHoFd6I/tQfFxRUV5CvXnwAAAABJRU5ErkJggg==</ss:Data>
            </when>
            <when test="@InterpolatedVolume&lt;=-20and@InterpolatedVolume&gt;-40">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAD4AAAAUCAIAAABalBlDAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAOBJREFUSEvtl8EOgjAMhuvjyVP5HIZnwMRkHvFx8K4J3GahupVtFD3USNKlB9hPtm/j39btvPfwLsNjaK9td+uaU+MuLtT/6QOih9Lfe3d29bGu9hWvT9C5xEdOnyWq3uuspwI6+ANAiBROVvWop5bX0dFPFDiAHF1SDX1pBmzWi3Yyw5hhkp0bV4qw/5hhzDBbN0w8U0te/1kms3wkjVgUMRGgZGbMsWRVeYFS8+Jpyrh5GvMCk1V9+q8TgRlSZhh94NjDGjr9mOCTHE1WNYfyAfpEL90hZFWNHvBOtNF4AkEnIYwc261MAAAAAElFTkSuQmCC</ss:Data>
            </when>
            <when test="@InterpolatedVolume&lt;=0and@InterpolatedVolume&gt;-20">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAD4AAAAUCAIAAABalBlDAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAMdJREFUSEtj/P//PwMM/PjyY9/+fc9fPF+3ft3WbVvh4oOUAXQ6HHz//H3rpq1zZs3x8vRCFkdzOrLUALIZCDid4X8DAwMcQfwwgM5FCVCCTgemJwgCemDU6dSJNsIJZjTUqRPSo2md+uFImomjaZ208KKO6tFQp044kmYKOaE+SOpU3E4HykARoiEAacwMkmYM3lAHOR3qbuRmDGnxSjPVJCcYmrmEZIMJOR3UEwGF/eBJJ3AvEuF0sOsHSfpGacMA+0RDFAEAMoouAwH0jFAAAAAASUVORK5CYII=</ss:Data>
            </when>
            <when test="@InterpolatedVolume&lt;=20and@InterpolatedVolume&gt;0">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAD4AAAAUCAIAAABalBlDAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAALtJREFUSEvtl10KgCAMgO14darOEZ2hILDHOk69F9SbLS1J6G9RuGCyBxkSH58zZ6CUEusY+7Gqq6ZtsjyTpbR5ohNAt2PoBlnINEmjMNrmhYqBfpuhMHeAdtFnbtiYGZ4W/QX6wg3o9OjP0B1uevSH6JrbhFZurJugUTnogrF/G+8n9Qm6d2gDwOg+NoKts3WUAS4YlK6XFrP1l0SiPnPDum1ddPdCpIG5vk2XK9d9JqHcfLdYwJvopzEB6/kklok8abwAAAAASUVORK5CYII=</ss:Data>
            </when>
            <when test="@InterpolatedVolume&lt;=40and@InterpolatedVolume&gt;20">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAD4AAAAUCAIAAABalBlDAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAOpJREFUSEvtl1EOwiAMhvF47lSew+wMMzHBx3mc+a7J9oaFOlImFmFiNOnCw9J/sI/yj5WNMUbN13Sb+nM/XIbu0OmT9vEfvQF0f43XUR91u2+bbUPjyuyAnkbeV1/1Wh8PgKLolhsWxsJH6Hl1PR8zQgL9QQboMXpercptfc4YJiB7oufV2twcuiPD5lKOWcdmrcOpX+BOZN3JITrZa5Jq7Qlke51+rNGJ1Sb24wv6vP8w2//HV0OyLlnPMpUYRgwjhnEZsP9LX7pg9UJKX17NSmHBw+l6nR6jFuj4vsUZqgCirIuCM9GftjvO6Q6yGK4cuAAAAABJRU5ErkJggg==</ss:Data>
            </when>
            <when test="@InterpolatedVolume&lt;=60and@InterpolatedVolume&gt;40">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAD4AAAAUCAIAAABalBlDAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAPNJREFUSEvtl8EOwiAMhufjuafyOcyeYSYmeJyPM++abDcEKgQYa3HdQZMSDkt/Ah9dW+CgtW58m1/zcB/Gx9hfenVTwf6jHwY9tOk5qavqzl17bGN7o0+GPrbspa7NWWNPgIroltv8GAtfoOeoNXzIGAL9Q2bQS/Qclclt4xwJmIRsQc9R+dwYuiOD7lwOXoduQ2e7ugs34XUnp+hRrWGq/A18HetxshY3FphwVdC9B5bFkeNX8fpqZEms+wOrPoklTSVN4bySuk7nQkWFCVcXuL1kft2q0mjUCPq+Hj+jMnSYPHtDZSviKoWH6Y15E/1pfwNCvvi/9qTmmgAAAABJRU5ErkJggg==</ss:Data>
            </when>
            <when test="@InterpolatedVolume&lt;=80and@InterpolatedVolume&gt;60">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAD4AAAAUCAIAAABalBlDAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAOtJREFUSEvtl00OwiAQhevx7Kk8h+kZamKCy3qcutek3eHAFAJIO0QZo8kQFs28lH5MHz+z01o3rs2PebgO423sT726KB//0QdA9226T+qsumPX7tsw3ugD0IeR76hrX8R4BJRFN9zwYwx8hp5P3eam0RcyQM/R86kkN4Eekb3Q86kl3Fvolgy7TTlmHbuxDpdayP2OYfxuY1+OJxbsRB+qJRMglmkWzo/Lpwp66HX0uWuSdbf010+9rH/E6+7AqmUnWaayTPG8KtydxDD+6oK3lyRzPGqFrC+X+rhMSsZNaqiKKlFqQE30p/0JR2ni2/Qp1U4AAAAASUVORK5CYII=</ss:Data>
            </when>
            <when test="@InterpolatedVolume&gt;80">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAD4AAAAUCAIAAABalBlDAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAOFJREFUSEvtl8EKgzAMht3jzafacwyfwcGgO7rH0fsGeuuSRktaa/WwGQcpPUh+wc/0T2hO1tpiWsN7aJ5N27X1rTYP4+MHfQB0v/pXb+6mulblueTxwl6AnkeOoa6hIzccDMIn6KVUNEs+6yMZoKfopVRizqEHZDN6KdXnehHdkdF2Kaes00bryKhhjWW9PreE7zbuyMIfY53op+q6YZY+z44sgb6Dqui8EqgKppW001dUzbpmnZrTRjupYdQwahh2X8fe7K8udHuJiklC3VSm40vhmBTNHNEMtZ8KM9Gf7g/c6sz3i9hjiwAAAABJRU5ErkJggg==</ss:Data>
            </when>

            <otherwise>
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAD4AAAAUCAIAAABalBlDAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAHtJREFUSEtj/P//PwMM/PjyY9/+fc9fPF+3ft3WbVvh4oOUAXQ6HHz//H3rpq1zZs3x8vRCFh+cbIZRpw9AxIyG+gAE+v/RUB8NdZJCYDTBkBRcVFI8GupUCkiSjBkNdZKCi0qKR0OdSgFJkjGjoU5ScFFJMQOwTzREEQCZ7jp6SeXViwAAAABJRU5ErkJgggAAAA==</ss:Data>
            </otherwise>
          </choose>
        </ss:Cell>
      </if>



      <if test="@VolumeCategoryMnemonic">
        <ss:Cell mts:ColumnId="VolumeCategoryMnemonic">
          <attribute name="ss:StyleID">
            <value-of select="$BucketStyle"/>
          </attribute>
          <choose>
            <when test="@VolumeCategoryMnemonic='High'">
              <ss:Data ss:Type="string">
                <value-of select="@VolumeCategoryMnemonic" />
              </ss:Data>
            </when>
            <when test="@VolumeCategoryMnemonic='Mid'">
              <ss:Data ss:Type="string">
                <value-of select="@VolumeCategoryMnemonic" />
              </ss:Data>
            </when>
            <otherwise>
              <ss:Data ss:Type="string">
                <value-of select="@VolumeCategoryMnemonic" />
              </ss:Data>
            </otherwise>
          </choose>
        </ss:Cell>
      </if>
      
      

      <if test="@TimeLeft">
        <ss:Cell mts:ColumnId="TimeLeft">
          <choose>
            <when test="@SecondsLeft='0'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAdlJREFUaEPt28dKA1EUxvEbn0OfQHThWvABdOvavS8grly6iAULFuxiVyxYsItdsWDBLnbFggW7WMb/RAISLLiJg36LH0whBO7hZOaenONyG2O5jDHvBfzw/LvP+963z/Udb2vuuw7GDkgSkpGCVKQhHRnIRBaykYNc5CEfBShEEYpRglKUoRwVqEQVqlGDWtShHg1oRBOa0YJWtKEdHehEF7rRg170oR8DGMQQhjGCUYxhHBOYxBSmMYNZzGEeC1jEEpaxglWsYR0b2MQWtrGDXexhHwc4xBGOcYJTnOEcF7jEFa5xg1vc4R4PeMQTnvECBYRFUEBYBGWIMkQ/WXqG6Bmih7resvSWpdde7UO0D9HGUDt17dRVOvlLpZNIE20FmjiPKI5VyyK6v1VcDDGxljFuK9EEe9jHoVxTcZGg+HsfEm/CPAGwM8Nb7bWP7WsJ3FO1l6D4s/webmI+DUgE9xQQPwfEmw0fZUgQmaKAKCD/+w8qZQgZ4KTSiQLisIDooe6wgOi112EBsbtOtDFkEZzWBqTSCUFRX5b6stQop85FdS6qlVStpOrtVbO1mq3V/a5xBLJA4wgsguZDWAQN7GhgRxNUGmnTSJtmDDX0qRlDTeF+MYX7CpTV1z14cdKoAAAAAElFTkSuQmCC</ss:Data>
            </when>
            <when test="@SecondsLeft='1'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAdRJREFUaEPt28dKA1EUxvE7eQd3unIp+hIuXfsKLnwBn0FMDFGxYO8VCxbsYlcsWLCLXdGEqNjFxfU/CiIuFFeJ8i1+hJkQAvdwZubcOcfJMsY6xpjPPL88/un3X793j/Uf72vurkMg6DWO4xiPhyM3IF74kA0/AshBLvKQjwIUogjFKEEpylCOClSiCjWoRR3q0YBGNKEZLWhFG9rRgU50oRs96EUf+jGAQQxhGCMYxRjGMYFJTGEaM5jFHOaxgEUsYRkrWMUa1rGBTWxhGzvYxR72cYBDHOEYJzjFGc5xgSBCCOMSV7jGDW5xh3s84BFPeMYLvEGv9YV81h/2WwWEBVFAWARliDJElyzdQ3QP0U1dT1l6ytJjr+oQ1SEqDFWpq1LX1sl/3DpJM8k2yaRrLyuSm4vVJsammFQbazIs26Nvn9pcJN0itdubSEa43GAoIAQiWrbflSEEI5rehyggCoheUH33xlAZogxRhihDyIK/0uSgS1YUXbICJv6jMHRrkUyToK6TSBWGbmZ8Fcc5l9qAyBr1ZakvS41y6lxU56JaSdXbq95eNVur2Vrd7xpH0DiC5kM0sKOBHU1QaaSNLNBIG4ugGUMWQUOfGvrUFC5TuK93x1YdFEhoCQAAAABJRU5ErkJggg==</ss:Data>
            </when>
            <when test="@SecondsLeft='2'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAlJJREFUaEPt28tLVGEYx/HnzB/QzlX6J9TCZZsW7RSXrVu4a9GiFlHhelBnmrLIysqMtFJR04ruYXalEi+olVF2mTIHS9R0+fZ9xRPOIIQQ54j9Fh+GwyDC/HjOe32CWjMXmNlKiTU+/+3vC7/3z//7/ziSTVoikbAgCPI+zQeSQhqHkcFR1OEYjuME6nESp9CAMziLc2jEeTThAi6iGS24hMu4gla0oR0d6EQXrqIH13AdN3ATt3Abd3AX93AfD9CLh+jDIzzGEzzFMzzHC7zEK/RjAIMYwjBGMIoxvMYbvMU43uE9PmACH/EJn/EFWXzFJL5jCjlM4wd+YgazmMM8fmEBi0hmk676W7WrmaxxqamUS+fSLjOdcQpEgahCVCFUgV5Z/AgaQzSGaFDXLEuzLE17tQ7ROkQLQ63UtVLX1slG2Tops52u2PYv2Wq7XZWVai8rrs3FLQQQhmFW60J7bZs2F6Pe7d3Dj+4DabdNS7u95VRKGEgJ1aLdXn6UKBeG9VbM9n5R3jrEV4sPRYEQRtQVstp5SBjIdtulCok7kGaqxVeHD6Wb15heWRG/sgorJBxDNMsiiLhmWeER7iGmur4yfBg6wo05kIPLYTQwyOtMnTDivORwgDD81Pc0Yay85ODHkAqmwbrkQEBR3TqptB1/BvFwcRh++mnvPtYpCiTCQDYvb5cUhuGftQ5ZJ+sQjSExjyG6KEcAOjHUiaFODHViqBNDnRhulBND3X5nYFc7gtoR1B+ihh017KiDSh1UamlTj6F6DNX0qS7cf9SF+xvg5smHwpia3AAAAABJRU5ErkJggg==</ss:Data>
            </when>
            <when test="@SecondsLeft='3'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAlVJREFUaEPt27lrVFEUx/Hz3h9gZ2f+AtHCUpQUdmZaGxs7AzZiZaNYzJBhNoeouOAe3A0xoglxScQkbrigBhM31DGJxoRoUPvr9wZuGGIhNrlBf8WHxzAMA+9w3r3nvnOSkplLzKxe+pef//T7+d/7z//jf1RrWUvSxNIktSThmv5+NR+QMirYjSpasQd7sQ/7cQAHcQiHcQRHcQzHcQIn0YZTOI0zOItzOI8LuIh2dOASOnEZV3AVXehGD67hOm7gJnrRh1u4jX4MYBB3cBf3cB8P8BCP8BhP8BTP8BxDeIFhjOAlXuE13uAt3uE9PqCGjxjFGMbxCRP4gklMYRpf8Q0z+I4f+IlsLetyoznXMtbi8uN5V/hccMWJoitPll1lquKq01WngCggyhBlCFmgRxY3QWuI1hAt6tplaZelba/qENUhKgxVqatS19HJv3J00m5L3Frb5JbZ9lkZ26CzrFiHi222dC4QZiUXNBIgHS5SjS90HbKGG7/VVs+e9u6wVXMBaSBTFJAIAZl//O4fWT5Lmm2dAhLzfUgH60gTa4cPyE4yRe9DyI6YAQmZ4bNjpW1xBVuuDIkZkJAh9Qv7LjJFbwzJlJivcP2WNwTFL+wKSOSAdLKWhMeXAhJhl7WC9WI9WVHf5BACstEyypCFrkPCzfc7LN91so2aJCzsqkMiZEiO3ZTPknBs4ndYm6lBtO2NvO1VXxYBUKOcGuXUuahWUrWSqrdXvb1qtlb3u7rfNY6g+RDNh2hgRxNUmqDSSJtmDMkCzRhyEzT0qaHPRTeF+wu8UTlMX7ZCGgAAAABJRU5ErkJggg==</ss:Data>
            </when>
            <when test="@SecondsLeft='4'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAhdJREFUaEPt28lOFFEUxvFTteEl8BFgwZKFD0DcumDlng1LNiZNgECAbjsCQUURJYATYYhgbJmCs1GIQ1Ac0syKENQ3OP4LAnbYGDdWBb7FL013h3RSX27duveeE7SaeWBmhcJ/fP+3/z/8ffT+uPxGNp+yIAgsDMLd1yD883cY7n1W+GpRIGlkcA5ZnEcb2tGBTlzARVzCZVxBN66iB9dwHb3oQz8GcAM3cQu3cQeDGMIwRjCKuxjDOO7hPnJ4gAlMYgrTmMEsHuIRHuMJnuIZnuMFXuIV5jCP13iDt3iHBbzHByziIz7hM74gjyUsYwWrWMM6NvAVm/iOLWxjBz/wE7+Qyqe8dqnW65brvH6l3hvWGrxxvdGbNpq8+Vuzt2y2eHor7ZntjGd3sq5AFIhGiEaIblmaQzSHMAo0qXMR9JSlpyw99modonWIFoZaqWulrq2To7p1ctLO+Amr0WNvEjYXK+2Um7UqkCTs9p61st0wFAgLwrgD6bJiL7UqL+ZWpUASEEgJYeSsSIEk4YAqCiMaIdEBlUYIFyHOE8MKO+3VVn5wYqhAYg4kCqDQ/qS+P4/oCJeA/ueZugLhgid5c1G3rJhvWYerThRIAgOJQtHWCcEkYetEdVkJGyEKRIGoclGlpIwC1faqtlfF1iq2VvW72hHUjqD+EDXsqGFHHVSxn6mrpY1hqB5D9Riq6VNduPF24f4GKzKlVk54hjUAAAAASUVORK5CYII=</ss:Data>
            </when>
            <when test="@SecondsLeft='5'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAmFJREFUaEPt28tLlFEYx/HnfTft2rmzlev6A6Jo0TK3FbVpGbRqFwyKoqJoMw2VVGJXu9mF7kVpF7oXXehC1qR4SS1T/BuevsdmyMYi2nRi+C0+6Ds6DMzDOc855zlPst3MEzObL/3L5z+9v/zv4bmSPyNXyFiappYkyY+fyffnJOW14u8L/of3WAhIFjnsQB47sQu70Yk92It96EI39uMADuIQDuMIenAMx3ECJ9GLUziNMziLcziPC7iIy7iCq7iG67iBPvTjJm7hNu7gLu7hPh7gIR7hMZ7gKZ7hOV7gJV7hNd7gLd5hAO/xAQV8xCCGMIwRjGIMnzCOCUziM6bwFdOYwSwyhYzXDdZ5/VC9Nww3eONIozeNNnnzWLO3jLd460Srt022efuXdu+Y6vDsdNZzMznPz+ZdAVFANEI0QjRlKYcohzAKlNT5ErTK0ipLy17tQ7QP0cZQO3Xt1HV0UilHJ0etyqtt20+W8LzKNuksK8ayd73V/jIgnVajgMQIyFLbwolylU57/4fj9x4CQdFgboQsIzC1tpYyQY2O32PVQ9YxXYWAlNvI6zo6iXB0EqarUkIvD8pmW60cEiOHlCqG3VbtG4oJPgQnrLRUMWSUxC7h9tuiuXyigESYsn5XU79kixWQGJccVrD5Czlkqy1fsOzVxjDCCAnBKCXzNSx5+5iuwq2TsMrqZUmsHPKPc0jYh6wsjpL5e5EwZWnZG2GE6F4WX7oKVCpQqUClApUKVCpQVUqBSpetSey6/a7b72pHUH+I+kPUsKOGHXVQqaVNLW3qMVTTp5o+K7ML9xsmWg01rYNLhwAAAABJRU5ErkJggg==</ss:Data>
            </when>
            <when test="@SecondsLeft='6'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAm9JREFUaEPt28tLlGEUx/Hzvpt27drZqnUtWga1addl2bpVLVoWzKA4KSqKOtOQhRV2tTLL6EIXsivdiy5UMqYpXlLLiv6G0/eZfEFEiDbzwvBbfBhnHBHew/Oe5zznPVGnmUdmtlj8n+//9fdLfx/eV9v/KJQyFkWRxXG8/Gv09/Mo5jsLPy/3XQsByaOAAyjiILpwCIfRjSM4imPowXGcwEmcwmmcQS/O4Tz6cAH9uIhLGMBlXMFVXMN13MBN3MJt3MEg7uIe7uMBHuIRHuMJnuIZnuMFXuIVXuMN3uId3uMDPuIThlDCMD5jBKP4gjGMYwKTmMI0vmIGs5jDN8zjBzKljGeHs147Uut1o3VeP1bvufGcN0w0eONkozdNNXnzdLO3zLR462yrt821efv3du+Y7/D8z7wXfhW8+LvoCogCohWiFaJblnKIcgirQEmdi6BdlnZZ2vaqDlEdosJQlboqdR2dVOPRSZet8W22w1dbtkxnWWx90zpc3EogzDp9k+30/baeA8qVCkhadcha21MORo5A6LSXVZHm8fuWhZWxyzbr+D3tfkivrSqvjGAdq6SGvBHstQ06OkmjQZWsjhCEQVvhu1klSYD2ERQl9Qon9RCIEIDwGjqGfYtWjHZZKRwuLg1IaOEmnykgCoh66kkOCbetHqsp77KSFbKd3ZdySIVzyADFXxKAUBiGSj0EJ9yu+sknCkiFAxIeAzrLhd9IdZ5seUOlHoKhjmEKOUTPZXHR1aBSg0oNKjWo1KBSg6oaG1R62Jokr6ff9fS7xhE0H6L5EA3saGBHE1QaadNIm2YMNfSpoc/qmcL9A9+Ub4cXhZ2+AAAAAElFTkSuQmCC</ss:Data>
            </when>
            <when test="@SecondsLeft='7'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAkdJREFUaEPt20tLlHEUx/HzPC+gV6Cr9i1aRuSirVujXbsW7RrRQXEYUVG0mYZSSukeZWqRiRrdo3vRhcbSvOEllaLoNZy+T2iIFtFm+CO/xYe5PTowh///ec55zomOmHlkZuvF//n6X3+/8fPk9Vb4jnwxZVEUWRzHvx7XP1977/djtHpMzPGrzzcdw/+xJCA55HEUBRzDcXSiCydwEt3owSmcxhmcxTmcxwVcxCVcRi+uoA/9GMBVXMN1DOIGhjCMEYziJm7hNu7gLu7hPh7gIR7hMZ7gKZ7hOV7gJV7hNd7gLd7hPYoYwweMYwKfMIkpTGMGs5jDPBawiM9YwjJWkCqmvHqs2ms+1njteK2nJ9JeN1nn9VP13jDT4JnZjGfnst443+hNC03evNjsLUst3rrc6m0rbd7+pd07vnZ47lvO89/zXvhRcAVEAdEK0QrRlqVziM4hOqnrKktXWawCXfbyIygPUR6ixFCZujJ1lU62Qumk28q8zNKblPNeYti2qZZVyuLiPqt0SqJ/DEiFHVBxsdTV3t386D2sko3V3kqr8qztVEBKHZC/5SE77JDK76HcD+m07Z6sEN0PIWkMYYXsYRvrIigKSCABSbYr3TEkGCFsWYdtlx+0vQpIKPfUk9UxRO6hFRLAChkkEMn5Q00OBCOEFbKfJDFD7qGABBKQZLtSGxDBUF+W+rLUKKfORXUuqpVUraTq7VWztZqt1f2ucQSNI2g+JIjSiQZ2yNI1QaUJKo20acZQM4Ya+tTQZ5hTuD8B8GHVJk+1N5oAAAAASUVORK5CYII=</ss:Data>
            </when>
            <when test="@SecondsLeft='8'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAqJJREFUaEPtm1tLVFEUx/eZD9AnsKceo4Ieg3rp1R7rtYeKIBBtRIcZFEUnZXQmqaQLlV3onlTShe7RvShLLU1TvHShKPoMq98+nBMHe4heZkr+Dz82+8wcBvZ/1tprrb1X0O2cBc65JKm/nP/p/fmf+/n/+hvdgzUulUq5IAhcKojGaB4+iz+bP0bfDVK/v5d8x3lBilCCndADu2A37IFe2Av7YD8cgINwCA5DHxyBo3AMjsMJOAmn4DScgbNwDs5DP1yAi3AJBuAyXIGrcA2uww24CbfgNtyBu3AP7sMDeAiP4DE8gafwDJ7DC3gJg/AKhmAYRuANjMIYvINxmID3MAlTMA0zMAtz8AFqBmus9nWt1Q3VWXo4bfUj9dbwtsEaRxstM5ax7HjWchM5a5pssuapZmuZbrHWmVZrm22z9rl2y3/MW8enDuv83GmFLwXr+tplxW9FK30vWc+PHpMgEkQWIguRy9Ieoj1Em7qiLEVZWIHCXhZBeYjyECWGytSVqat0slBKJzm30pa5bVblMr/Y6taqllWJ4uImFt657pAmhPHFxbRbFc7XufUqLpa72uutwi++H5PV3sXMPar28g8tZ/k9FsSLsgVr8RYy4BaFYqzAjUmQMguSxU3FLsuPa9xGW44QXpA+VyVByu2yfKa+wy0NXVZSGL+P6ICKxamEIP24qNWRZSRF6XVLZCHlFiQfWUcdFuE39Woiq1gU7SEVsJA4/0ieqcfuS1FWBQSJk0EJwuL/C7dONrjq0EXFLmt7lBT6Zy1EYAp7yxz2+ihrM/lHsmziw94Ce4uirAq4LB1Q6YBKF+V0cxEr0AGVDqh0QLVQDqh02RqXptvvuv2udgT1h6g/RA07athRB5Va2tTSph5DNX2q6XNhduH+BMcFKUsi7phTAAAAAElFTkSuQmCC</ss:Data>
            </when>
            <when test="@SecondsLeft='9'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAptJREFUaEPt28tLVGEYx/H3zKa/oJ2t2he0jGrTMrctWrWqmNQURUUZUVQUp5mGLnQhs7LMbnShC2U3ujeopZamKVpUFEV/w9P3nTyDDEEEMcfkt/jAnDPnMDAP7+153yfY7ZwFzrmFYn95/af3C7/31//TbySzcReLxVwQBDnh51jw695vv5t/vvDZIMbzBe/ln+Ed5wOSQhp7kMFe7MN+HMBBHMJhHMFRdOMYenAcJ3ASvTiNPpxBP87iHM7jAi7iEi7jCq7iGq7jBm7iFm5jAHdwF/dwHw/wEI/wGE/wFM/wHC+QxSCGMIyXGMEoxvAa45jAW0xiCu8wjRnMYg7xbNzKBsusfKjcKoYrrPJVpVWNVFn1aLXVjNVY7Ztaqxuvs/qJemuYbLDGqUZLTCesaabJmmebrWWuxVrft1rbhzZr/9huHZ86rPNzp3V96bLk16SlvqUs/T1tmR8ZU0AUELUQtRB1WRpDNIZoUNcsS7MsWoGmvfwJWodoHaKFoVbqWqkrdbJUUien3HJb77ZaiavPKXWblcuKKrnYSzB8EEiLkgRdaVtcae7zKrdTycUosr3raBk+AD4oPtvb7Upy194Ot1HZ3mKn38PWEQbEp9/DeysIktLv/CHF3A9RQBbZwjAMiO+i+hhP1EIIUJQ7hrvc2vyYsY0xY8Aty3dZqxnY1WUVucvyuaxKghJOeTfMD/K+xdRwXwGJICAL99S300o07c1NN6NPvyfcmvwUuIfprw45EJRiT3t9l9VIIPxKPWwZ/QzuOnUSUQsJx45NpEv8Sl3HgCKeZWmDapGtQxQQBUQnF3WUlFagHUPtGGrHcKnsGOr0O12ayhFUjqD6EBXsqGBHFVSqoFJJm2oMVWOook9V4f6jKtyfsSWB3eQvBQYAAAAASUVORK5CYII=</ss:Data>
            </when>
            <when test="@SecondsLeft='10'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAoZJREFUaEPt29tLVFEUx/F95n/orXrpUYrqLwh87CF66LmnGTQVRdFBURxURhxnHKzoQna/X+hCF7pHlzFFLbU0zehCRVH0N6y+e7DQwTGCYTD7PXyYM2d7HNiLvc/Z66wddDtngXNurtBffv/T9bnt/vtS/41EJuyCIHChUGje57xzwSJtOdeFZv82CPE/c66b+xvOBySJFHqQRi92Yhd2Yw/2Yh/24wD6cBCHcBhHcBTHcAIncQqncQZncQ7ncQEXcQmXcQVXcQ3XcQM3cQu3cQd3cQ/38QAP8QiP8QQZ9OMpBjCIIQxjBM8wijGM4wUmMIlXmMI0XmMG4UzYIv0RKxsos/LBcqsYqrDK4UqrGqmy6ufVVjNaY7VjtVY3Xmf1L+utYaLBopNRa5xqtKbpJmueabaWNy3W+rbVYu9i1va+zdo/tFvHxw6Lf4pb5+dO6/rSZYmvCUt+S1rqe8rSP9KmgCggGiEaIZqydA/RPUQ3dT1l6SlLj71ah2gdwijQwpBO0EpdK3WlTpZj6iTiSm2d25E3l7XFbbPVLpq1lWPlspgOC51cPO5W2GY6dyWd7Fx39nOh5OJ6AuXbe1wJSdOS7PEGzim5SFAKme1dS6d6voPzBSTmNmbb/Mj4le31x/5cnDZlewlKodPvi42QTW573oCU0qaAFDkgq2ZHw0IjxJ9TQBSQ5f+CarEpSyOEEVDsV7gKCJ2+lN6p66b+DwVEj71FnrJ63ZrfC0O/tuhi4ZdbdaKFIUEpRhmQn6py+Zu4l1sGpNQJQVFdluqyVCinykVVLqqUVLW9qu1VsbWq31X9ru0I2o6g/SHasKMNO9pBpS1t2tKmPYba9KlNn/93sfVPNGHJI0QCVR0AAAAASUVORK5CYII=</ss:Data>
            </when>
            <when test="@SecondsLeft='11'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAnNJREFUaEPt28tPE1EUx/E7/R/cqRuXRqN/g0sXrtw3EBqwEAgEGggEAgQstJKqUYnv9/sVH/EtgiAEUEBBjMZH1Gg0/g3H7zXUlKa2MQGD+lt8Mpn2TCfp6e3MvXNO0O2cBc65TKHf3C90fPb7fn8pnmNrX9iFQiEXBMEvt/PeC+bH5jsuNBcbhPjsrOMyz+l8QhJIYht6kMJ27MBO7MJu7EEv9mIf9uMADuIQDuMIjuE4TuAkTuE0zuAszuECLuISLuMKruIaruMGbuIWbuMO7uIe7uMB+jGAhxjEEB5hGCMYxRjG8RgTmMQUnmIaM3iOWYT7wlbUX2TFA8VWMlhikaGIlQ6XWtlImUVHo1Y+Vm4V4xVW+aTSqiaqrHqy2mqmaqz2Wa3VTddZbCZm9bP11vCiwRpfNlrTqyZrft1sLW9arPVtq7W9a7P29+3W8aHDOj92WvxT3Lo+d1niS8KSX5PW863HlBAlRCNEI0R/WbqG6Bqii7rusnSXpdtezUM0D2EUaGLIl6CZumbqWjr5F5dOIm6DrXVbCq5lRYlbT5zWsvg7XOjFxaNumW10m225i5lz3T+2uRYXzxO3ibiVc3F+q4QsQkLW8Ev3fDLyJWQdMV46TgkhGYu5/F5ohKSX3zVCSMSfeB6ihPBFL6UHVEqIEqInhvke4WqEaIRohGiEMAr+hqqTlFv1c2Lo5xlxtzpn1UkvcenbXh+XIk5VJyR5IcuA/LUj2wpe8zLLgPy+T0YuKgMiKarLUl2WCuVUuajKRZWSqrZXtb0qtlb1u6rf1Y6gdgT1h6hhRw076qBSS5ta2tRjqKZPNX3+H1243wGz9hvWZ9hxcQAAAABJRU5ErkJggg==</ss:Data>
            </when>
            <when test="@SecondsLeft='12'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAvBJREFUaEPtm8tPE1EUxu/0D3DHDt24ND7i0qUuXWhM3DWUQGiKQCAgNG0gNKXSFFob1PiILxR8G9T41gIKghBAAcVoND6iRqPxbzh+t3JJmViUZEaMfotfZqa9k0nul3PvOeeeY7UrJZZSKhfPIp9/9b79f/38t3wjnvEqj8ejLMvKYu7zXeeNsX6891vvz461PBhvey/3W0oLkgQpsBukQQfYA/aCfWA/OAAOgkPgMDgCjoJj4DjoBCfASdANToHT4Aw4C86B8+ACuAh6wCVwGVwBV8E1cB3cALfAbXAH3AUZ0Av6QD+4DwbAIHgAhsAweAhGwCgYA+NgAjwCk2AKTIMnYAZ4M14p6isSX79Piu8VS8lAiZQOlkrZUJn4h/0SGAlI+Wi5VIxVSOV4pVRNVEn142qpmayR2qlaqZuuk/qn9dIw0yDBZ0EJPQ9J+EVYGl82StOrJml+3SyRNxGJvo1Ky7sWib2PSeuHVol/jEviU0LaPrdJ8ktSUl9Tkv6WFgpCQWghtBAuWdxDuIdwU6eXRS+Lbi/jEMYhDAwZqTNShxUwdfKPpk78apOsUTt+msvaorbLChXMsg5jdqn1zGW5kVzsUgWyGZNdiIlWqj17tScX10KA5bNi6DGGkNrA5KLT2d7VmGyNmWS7IDsx6VqQm2pZNtu7FeKZsdpamO3FpLiRfs9nIZ2qEGn+gnnpdy2EFoWCQAynLcSchyy0ZNnPQ4wgG5WPFrLUgvTAWox19GIZ45L1h5cs+4mh2UPoZUEIN7ysxSxZUbi6ermK4coj3CUWxIjRjU2eZ+oQw+0ih4U29QgsQgeDXRAjt8hB7yHb4AazyAECOVl10qFWzgWGesNOqFVzVScBRO/6NxMYmmjdXMOIUyiIg4Joy7CjJ1+jy4DMfT5B6GW55GWxLgsTy0I5FsqxcpGlpCwlZW0va3tZbO1oHMLqd3hYbEdgOwL7Q9iww4YddlCxg4otba4UOTCXxVwWmz7/ty7c7wt0ZKaWNHZYAAAAAElFTkSuQmCC</ss:Data>
            </when>
            <when test="@SecondsLeft='13'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAvBJREFUaEPtm8lrU1EUxu/LXneuHDZuBHFciSuHpQsVFFJamtLS0CGhpbWJLS0tTWxImxii4oDzPFQccEoHW21tbelgK60oigMqiuLfcPxu6C0xhGolISLf4kfykvt4cD/OOfecd47VppRYSqlEbPO8/t39yf/r62w+IxCzK8uylM1m++Uz1W9zrrHmcf/MWsuG5ybdl/gMpQUJgTDYDyIgCg6Ag+AQOAyOgKPgGDgOToCT4BQ4Dc6As+AcuAAugkvgMrgCroJroB1cBzfATXAL3AZ3wF1wD9wHMdABOkEX6AYPQQ/oBY9BH+gHT8AAGARPwRAYBiNgFIyBcTABJoE9ZpecjhzJ7cqVvO48ye/JF0evQwoeFUhhX6EU9RdJ8UCxOAedUjJUIqXDpVI+Ui6uUZe4x9xS8axCKicqpWqySqqfV0vNVI14pj3ifeGV2pe1UveqTupf10vDmwZpfNsoTe+apPl9s/g++MT/0S8tn1ok8DkgwS9Baf3aKqFvIQl/D0vkR0QoCAWhhdBC6LIYQxhDGNR5yuIpi8de5iHMQ5gYMlNnpg4rYOnkPy2dONVWWa3KUtayHqiFslk5ZJnyxtmpdrOWlYni4nm1SLZhc5dgk5Vqi38mFxfbsWbpjBB6jWELBGJxES4qndXeVbAIjdnkVIJswsZ71MZ4tden1s+u1ZZCQdIsiCm/z2UhyeV3LYQW0A0XR0GyKEgMcWQH3JsWZB8she9DIEamXlD9iYUYy9DWsQ5uLqpW0kKyKYixkMTAvsFTxjeG6QzqfxNDtNsyoixY7Kcg2RakE7HEuC8KkoWgvgbxYjusIrHJwQiyYtceWki6LSSqls8mhtoVBRGsE7tOdFKof9ei6K6TvchJTGDnsTfNFqJPV8loATSmDSgEgdbCSkzZRH93IQfhsTeDx172ZWFz2SjHRjl2LrKVlK2k7O1lby+brdn9zu53jiNwPoTzIRzY4QQVJ6g40sYZQ84YcuiTQ5//4BTuT1ngqGmer33VAAAAAElFTkSuQmCC</ss:Data>
            </when>
            <when test="@SecondsLeft='14'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAsNJREFUaEPtm9tLVFEUxvc5L73YYy+N9RL0EkX06GuPQQ+JoigqiqKpKIojiqI4MqI5iRVd6J7diy40maZpaZqi0oWKouhCRVH0N3x9e5gZbEBnghmF+B5+bPacfc6B883ae+211nb2GQPHGLMY9x/78e6PvW77K/0OfzDTuK5rHMcxrhNuI/1wG7oW81ts/68xMc9Z9v7wWMdd+v32XcYK0ksCZD/pI/3kADlIDpHD5Ag5So6R4+QEOUlOkdPkDDlLzpHz5AK5SC6Ry+QKuUqukevkBrlJbpHb5A4JkrtkkAyRYXKfjJBR8oCMkXHyiEyQSfKYTJFp8oTMkFkyR+bJAskMZiJrMAvZ97KRM5SD3OFc5I3kIX80HwVjBSgcL0TRwyIUTxSjZLIEpVOlKJsuQ/lMOSpmK1A5V4mq+SpUL1Sj5mkNap/Vou55Hepf1KPhZQO8r7xofN2IpjdNaH7bjJZ3LWh934q2D21o/9iOjk8d8H32ofNLJ/xf/ej61oXu793o+dGD3p+9CPwKoO93HySIBJGFyEI0ZWkN0RqiRV1elrwsub3ah2gfoo2hduraqdMKFDr5T0MnZWYntpm9CcWyPBnNSPP4FMtKdnBxwKzDLpOFdNMIhkdDbbzg4uY9daGxEoTTU7IF2UqLsNgPnIggPrMjOlaCpECQSPg9EQsZMOnYTvHS1vtkIanOhyQiiBVj0qyRICuRoIoniBXDWohNUMlC+BFW00J2c9H3moxoxlCCrLIgG+h5WTaGiTgAtl3r6VQKNxU59eWmLAlCi1jpIod4a8jiIgdNWSmesvrNpujG0E5D3WbLslUnEiSFgljLiCUyRS1VBmQFCaHQSfJ36qrL4r9dhXIqlFPlokpJVUqq2l7V9qrYOun5EHlZ8rJ0HEHnQ2gF8rLkZcnLkpclL0tels4Y6tCnTuEm6RTuHwEB2IdYFRsKAAAAAElFTkSuQmCC</ss:Data>
            </when>
            <when test="@SecondsLeft='15'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAyJJREFUaEPtm9tLlFEUxc/Ma/WkPaQGRlGkpK/RkyjWg5GpKGOKDoqmqWiKmuaoOKJpmlihRdndblpZmmmalqYpFV2oKIouVBRFf8NqnckZ0qaRaEYj9sOP7zrfgVnss8/eZ2/DbqVgUEr9jPEPr2f7/czn+nouxqjpilRGo1EZDAYb9nOj4cc9x7Wzd6buuXxnxnecfXPmmAYjx/3N+PpdpQVpII1kD2kizWQv2Uf2kxbSSg6Qg+QQOUzayBFylBwjx8kJcoq0k9PkDDlLzpHzpIN0kovkEukil0k36SFXSS/pI/3kOhkgg+QGGSLD5BYZIaPkNhkj4+QOmSCTJLIrElFXohDdHY2YnhjE9sYi7locTH0mxPfHI2EgAYmDiUgaSkLycDLMN81IGUlB6mgq0sbSkD6ejoyJDGROZiLrbhay72Uj534Och/kIu9hHvIf5aPgcQEKnxSi6GkRip8Vo+R5CUpflKLsZRksryyoeF2ByjeVqHpbBes7K6rfV6PmQw1qP9ai7lMd6j/Xo+FLAxq/NqLpWxNEEBFELEQsRKYs8SHiQ8SpyypLVlmy7JU4ROIQCQwlUpdIXVIn/2/qJF2FIUhtc5rLCmsxYaGPdTq+Vvit2ym5LHcmF0+qxYhQsfBTxWB61HZ0llxcsTnPqSCh9WkiiDsFWUOL0GgxXAnitdqC8NYtku2dq/S7KwvpoBVpsfSU5RVggf/6QoTsSpX0uyf3Q1wJkqA2OizIbkn6uCqmQPZDPLVB5UqQYE5pC3yqbBbysyD6PMi8XXyIO32IfcdwNqdu3zEMbUrCyuh8hziLfKtFkPkUxL6FG3XBBO/AcoggDAT/BUH0nvqm9kQRZD58SIhKtvmQ4K2Zvyx7JTD0gIU0q+WOwFA76joVOK3qZOlU0Kif+YcXIrIzzlZ1oldZEW1m8SHunLK0M5+JFkBjLwPScciStTsckbp3QDmWbSiyTVlSBuQBC5G6LP6pUignhXJSuSilpFJKKrW9UtsrxdYeidSl+p0rLWlHkHYE6Q+Rhh1p2JEOKumgkpY26TGUHkNp+pQu3L/swv0OTk8SXA/bZ20AAAAASUVORK5CYII=</ss:Data>
            </when>
            <when test="@SecondsLeft='16'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAxxJREFUaEPtm9tLk3EYx9/tVruKLtLO3SV1UUTURRdqSUwUtZmiTlM0z4prDkVRnCmaNuxgSWpnO2hpaaZpWppSVHSgoig6UFEU/Q3fvr+Vw0ltCVtEPRcfXvYe9hvvl2fP4fc8ut2aBp2madPRz/Kzp+dnXleffbXGri6DptPpNL1e73J0OadzvTZ178+em833uH3+x5o6PX/bL9ZXa2lKkAbSSPYQO2kie8k+sp80k4PkEGkhh0kraSPt5Ag5So6R4+QkOUU6yGlyhpwl50gn6SIXSDfpIRdJL+kjl0k/GSCD5CoZIsPkGhkho+QGGSPj5CaZIJPE0GVA+PlwRHRHILInElGXohDdG42YvhgY+42IvRKLuIE4xA/GI2EoAYnDiTCNmJA8moyU6ylIHUtF2nga0ifSkTGZgcxbmci6nYWcOznIvZuLvHt5KLhfgMIHhSh6WATzIzMsjy0oflIM61MrSp6VoPR5KcpelKH8ZTkqXlWg8nUlqt5UwfbWhup31ah5X4PaD7Wo+1iH+k/1aPjcgMYvjbB/tUMEEUHEQsRC5C9LfIj4EHHqEmVJlCVhr+QhkodIYiiZumTqUjr5d0snGVoIVmnZHmtZG2u3Y2lYMfwDbZgTWC21LG8WF09o82DQjFigWcHyqOPorri4ZJPFcV/ghlKss2QhsiNJBPGmICtpEQr1kj0JMndFueOeteYdUu31dfndk4UsDt3pECMoKV/K739iP8SdIMEHtjktSFmJf4DNwersXNkP8dUGlTtBFoWaHYIoESI6jQgy5TsFWpOTJz7Emz5kasfQnSB+86ucgqgdw7CWRKcgEmX5aAt3NoKoLVxlLcpqRBAR5P/YU/8dH6IsIthuckRZUxayLMwqPsTbPqRJW+5MDNVLr9OCXLpONrdvhV/Adz+iEkOVqTucPDN1Q1uKCOJNQZRlzGQhzymmtwGFNMchYH2JM+RVmfqWVpOEvb4Ke6Uvi85ZGuWkUU46F6WVVFpJpbdXenul2Vq636X7XcYRZD5E5kNkYEcmqGSCSkbaZMZQZgxl6FOGPv/CKdxvxO5GArcD53wAAAAASUVORK5CYII=</ss:Data>
            </when>
            <when test="@SecondsLeft='17'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAvJJREFUaEPtm9tLVFEUxs+ZR62HyCAvLyFBWEEEgflQQg+jaV5RtDEVB80rY4pj3nVm0LSZpAtd6GpZXrK0NNM0LaUoKrpQURRdqCiK/oavdaRC07EmZkDqe/gxHPbM7MP5WHuftfa31J2KAlVRlMnoXLz+3e9/Hdeu3TmHrV2vqKo6gU6nm/Fzypj6B9/5i/+ZbX7d9zlVndyjk/m1e1c0QeyCQ9glNAu7hT3CXmGfsF84IBwUDgmHhSPCUeGYcFw4IbQIJ4VW4bRwRmgT2oUOoVM4K3QJ54VuoUe4IPQKfcIloV8YEAaFK8KQMCxcFUaEUeG6MCaMC/p2PcI6whDeGY6IrghEnotEVHcUonuiEXsxFnG9cYjvi0dCfwISLyciaSAJyYPJMAwZkDKcgtSRVKSNpiH9WjoyxjJgHDci80Ymsm5mIftWNnJu5yDvTh7y7+aj4F4BTPdNKHxQiKKHRSh+VIySxyUwPzGj9Gkpyp6Vofx5OSpfVKLqZRVqXtWg9nUtLG8ssL61wvbOhvr39Wj40IDGj41o+tQE+2c7HF8caP7aDApCQRghjBAuWdxDuIdwU+dbFt+y+NrLPIR5CBNDZurM1Fk6+XdLJ1nKBixYVjGtlrXOvhnefhbM87NOxd+K+f42xLSlspblruLiKWURIpQEBCilkPIovBbXTRMkMMY0MTaTIAEhFSwuurPau1LJhYb2wJ0J4hu8HesdhmnV3iV6M4LNuRTEnYL8KL/PFiHOyu8+y6tZfvfUeYirgoTuMEKLEJ6HeOiAylVB/EPKEdpopCBzJUK05Yonhh48wnUlQlZl52JFmomCePJM3RVBfIKqsanVQEHmgiAbW5Lgt7aMJgdPuk52K4E/E0MtF1ldssWp62Rp7DasKd5KQTwliLZUTUbL1L186+Dta5nRBrQwqIo2IPqy6MuiUY7ORToXaSWlt5feXpqt6X6n+53tCGxHYH8IG3bYsMMOKra0saWNPYZs+mTT5//bhfsNo9uLw6c8wWUAAAAASUVORK5CYII=</ss:Data>
            </when>
            <when test="@SecondsLeft='18'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAA09JREFUaEPtm+tLU3EYx8/2Mg26CTl7ERWRiVQSXSAIhjpF09Q0Zd6a9znbUubY0qabKJomZXShi5WX1CwtzTRNoyiKii5UFEUXKoqiv+Hbcw5tbGKraK6I58WHs8PvnJ2xL8/l95znke0WBMgEQXBF/pvnP7t/8rp4/qfPsLcpBblcLshkMufR9bPHNZn7fY5rPd0/5TWTvsfj/d+vlcnp9/7g+eIzBFGQRqKJ2EM0E3uJfUQLsZ84QBwkDhGHiSPEUeIYcZxoJU4QJ4lTRDvRQXQSp4kuopvoIc4QvcQ5oo/oJ84TA8QgcZEYIoaJEeIyMUqMEVeIcWKCULYpEd4RjojOCKi6VIjqjkJ0TzRiemMQezYWcX1xiO+PR8KFBCQOJCJpMAnJQ8lIuZSC1OFUpI2kQT2qRvpYOjLHM5E1kYXsq9nQXNMg53oO8m7kIf9mPgpvFaLodhGK7xRDd1eHknsl0N/Xw/DAgNKHpSh7VAbjYyPKn5TD9NQE8zMzLM8tqHhRgcqXlbC+sqLqdRVsb2ywv7Wj5l0Nat/Xou5DHeo/1qPhUwMaPzei6UsTmr82gwVhQdhC2ELYZXEM4RjCQZ2zLM6yOO3lfQjvQ3hjyDt13qlz6eT/LZ0s2VqA2ct2TlnLCtuRi7nLK+GvsDsJzTJwLcvbxcU2IQCKjXrMmF8NKo9Kx8nFxWC1TloTWV2aLxUXV2mLpfNFKhMXF71Z7Q0VtJi11OL8w6cSxC/QJq2L1uFa7fUPsmNmUA0L4k1BHOV3TxbiEEQUJSRju2Qhm9rVEAUJCLGyIL4WJMyQ67QgURTFejPmheySBIls0bAgvhZEfEG11qKRXJYjlojHMK2Og7q3g/qvuCxREFVrsmQZYqblKoqyIY8txNcWssa8DX4KG1YUFElBfWGk0SkKx5BpeqfuKajPCa6QBHF9p+5wX5xl/QVBxCyLBfFh18kqyxbnxlAK1sYMt66TxZv1kotyuKyVhVqny1pXruUY4s0YskAwSWK4EVgN0Spc24CC03VuZRMx7d1gLeAsa7qyLO7LIpfEjXLcKMedi9xKyq2k3NvLvb3cbM3d79z9zuMIPB/C8yE8sMMTVDxBxSNtPGPIM4Y89MlDn//gFO43zY2XAws9pBYAAAAASUVORK5CYII=</ss:Data>
            </when>
            <when test="@SecondsLeft='19'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAztJREFUaEPtm/1LU2EUx+/2W1lUUKDrhwhCopGQBdUPKS6XiqKo+YY6l+ZUfJ+pbfk6RdE0y8oMK83U1CwtzTSloiiKil6oKIpeqCiK/oZv57m1oaZLwxXk+eHDvXf33j1jh/M855znexR7JQkKSZLGopzh9e/en3hfXP/JGJZmD0mhUEhKpXLS47TvKWb+vnXMcWNM+B574yt/PqtQ0u+fYnwxhiQMUkPUEvuIOuIAUU8cJA4RDcQRopE4SjQRx4jjxAmimWghThKtRBvRTnQQp4lOoovoJs4QPcQ5opfoI84T/cQAcZEYJIaIYeIyMUKMEh7NHvBs8YRXqxc0pzTwbveGtkMLn04f+Hb5wq/bD/49/gg4G4DA3kAE9QUh+EIwQvpDEDoQirDBMIRfCkfkUCSihqMQPRKNmNEY6K7oEHc1DvpresRfj0fCjQQk3kyE4ZYBybeTkXInBal3U5F2Lw3p99OR+SATWQ+zYHxkRM7jHOQ+yUXe0zzkP8uH6bkJ5hdmFLwsQOGrQhS/LkbJmxJY3lpQ9q4M5e/LUfGhApUfK1H1qQrVn6tR86UGtV9rUfetDmwQNgh7CHsIT1m8hvAawos6R1kcZXHYy3kI5yGcGHKmzpk6l07+39LJqogkLFm9Z9Ja1taGSKg2m7BAVSaz0iePa1mOKC5urA6AyjMT851LQeVR+TixuKg5HAEnlUW+v6VCD9cQo3y+VF3ExcXZrvYucjVhsatZ/oOnMojLpt3yPeEZotqr2a+zPe+mz+ZqryPK7/Y8xMnlh3dYDSLK7+JcfLZweTkbhA0yB/ZDpuMhwiO2NUbLG1TsIQ7eMbRnkLWGJNuaoY7NQFBPuM0gy9TFPGX97SlLbOG6GZJtIa8If61BwPrUdDbIvzDI2D11tS6Dw15HixzsTVljRQ4bjAZbxKWt38F5yGznIUJ1ss683ZYYiqnIPTf2F9WJe/ZOOVOXE8I1RfBtiuVM3RGZ+jznEtkY43Aphcg9rDIgcS4y9RXaXXKmzjIg1mWxLouFcqxcZOUiS0nJC1jby9peFluz+p3V79yOwO0I3B/CDTvcsMMdVNzSxi1t3GPITZ/c9Dk3u3C/A3rdx26rWhaNAAAAAElFTkSuQmCC</ss:Data>
            </when>
            <when test="@SecondsLeft='20'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAA0BJREFUaEPtm+1LU3EUx+/dO60oKNCZUEH0YPUmSGyKpigqikNF0RwqboqPaZozzeWcQ1NnkkUZmpqlqVlamilFRVEUFT1QURQ9UFEU/Q3fzr04aWtTg7mgzosP997dJ7YvZ7/fOb/vEZsFAaIgCL+i+MPjue63Py8dz/cdtR0qQRRFQVSIgkJUyPsKhePtvM85ec5s91vfaXON3XNmvX/62rm+hyAJYiFaiINEK3GIaCMOE0eIo8Qxop04TnQQncQJoovoJnqIk0QvcZroI/qJM8QAMUgMEWeJYeI8MUKMEheIMWKcuERMEJPEFKHqUCGwMxBBXUEI7g5GSE8IQntDEXYqDOF94Yjoj0DkQCSiBqMQPRSNmOEYxJ6LRdxIHNSjasRfjEfCWAISxxORNJGE5MvJSJlMQepUKtKupEFzVYP0a+nIuJ6BzBuZyLqZBe0tLbJvZyPnTg5y7+Yi714eCu4XoPBBIYoeFqH4UTFKHpeg9Ekpyp6WofxZOfTP9ah4UYHKl5WoelWF6tfVMLwxoOZtDYzvjDC9N6HuQx3MH82o/1SPhs8NaPzSiKavTbB8s6Dlewtaf7SCBWFBOEI4Qvgvi8cQHkN4UOdZFs+yeNrLeQjnIZwYcqbOmTqXTv6t0okyZBc8vWtllq2vwpaCDKe1rFURe7DYp05mTaSea1muLi4uXVcJD2+jLIYgNM/gl6X7rbi4fKNBPr+9WgeVIVveX7FpPxcXXVXtXatJhyRIYFu0XO312VE8I4instZGkK0lOvmcFBnWaq+0L30WoM/naq8ryu9ba+LgfyDGpvxujRR7QZQBe50K4qvax4K4QhBH6yFWQbz89TYRskhpcirIkpVmFmQhBFFZ1PKPLokS2q5mQf72iqF1DHE0y+IIcfMSrl+uRo6MzfnpDpdwWRA3CmIVw9+c4HRNnQd1NwmyIWenPPXdZoq3MTlIY4hv2O4ZkwNPe90gyOpErTyIWxNDa7YubykP8dPqbFwnnBgusA3Iw8sImelM3V4QRzYgLp2wL4t9WWyUoyhg5yI7F9lKyt5e9vay2Zrd7+x+53YEbkfg/hBu2OGGHe6g4pY2bmnjHkNu+uSmz/+7C/cn89vpBSOvQIAAAAAASUVORK5CYII=</ss:Data>
            </when>
            <when test="@SecondsLeft='21'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAA0tJREFUaEPtm/tLk1EYx9/tN60oMNA0qB+ii9YvwVTUNE1RURwqiqZMcWqa1zQVxbXplqbOJAsvaWqWpmZpaTeKiqIoKrpQURRdqCiK/oZvz958V5vmDLYF8fzw4ezde847ti9n5zzP+T6yFkGATBCE35H/5bWt8db3TdfzfYauUyHI5XJBJpMJctnPVib/9dp8T+oz04r9rd6Tri3uzTzTuu984xfynHnHL/B7CCZBjEQrsZ9oIw4Q7cRB4hDRQXQSXUQ30UP0EkeIPqKfGCCOEoPEcWKIGCZOECPEKDFGnCTGidPEBDFJnCGmiGniHKHoVMCv2w/+h/0R0BOAwN5ABPUFIbg/GCEDIQgdDEXYsTCED4UjYjgCkSORiBqNQvRYNGLGYxB7KhZxE3FQTioRfzYeCVMJSJxORNL5JCRfSEbKxRSkXkpF2uU0pF9Jh+qqChnXMpB5PRNZN7KgvqlGzq0c5N7ORd6dPOTfzUfBvQIU3i9E0YMilDwsQemjUpQ9LkP5k3JUPK1A5bNKVD2vQvWLatS8rEHtq1poXmugfaOF7q0O9e/qoX+vh+GDAQ0fG9D4qRFNn5vQ/KUZxq9GtH5rRdv3NrAgLAjPEJ4h/JfFawivIbyo8y6Ld1m87eU4hOMQDgw5UudInVMn/1fqZEVIMVw96kSWravBpoIMm7ksH1Uxlvvs4VyWvZOLS9dWw8VDJ4ohCC1mvLOyZyUXt3WkYFXEbiz21Iv9TC0nF+2Y7V2TroJJkMD2aDHb67m1xCyI64q6WYK4bdDAzVtj7sOC2Dn9vlkbB999MRbpd2mmzCWIlH7nGeLE8xBJEHffyj+eh7AgThIkwKgU/45MooR2KVmQf31iKK0htnZZPEOcMEO889LFmbFxp8rmES4L4mBBJDF8DQkLOlNnQRwoyPrc7eLWV1Efb2FyMK0hK8N2zWlyYEEcJMjqRLW4iEuBoRStiy3FId7q7FmCbNmbaQ4MTWODtDvYdWIvG5CLuw4iM5G6tSDWNqBFnvWiGBZ46bHEy8A2IPZlsS+LjXLsXGTn4oK2vWwlddAui7299MOy2ZrN1ux+53IELkfg+hCuD+GCHa6g4goqLmnjGkOuMeSiT67CdWIV7g/XAhqrff4JbAAAAABJRU5ErkJggg==</ss:Data>
            </when>
            <when test="@SecondsLeft='22'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAA6ZJREFUaEPtm/tLk2EUx9/ttywoKigtSCK6SP3g8oZatqGZKBMVK3VNcWutpmlecWiuKZU6lbKL3dQudrW0C10oKoqiqOhCRVF0oaIo+hu+nffJd7ilq2D7pc4PH969vM+zsX159jznnO9RtUgSVJIkDUX9l/e/m+/9XL73/gxHR7ikUqkktVrteVX9vFep6dng61/GDM4Zdv5wz0Z4H1/zlc/0GOP1Pj7n/+H3kGRBXEQr0Ua0E1uIrUQHsY3YQewkOoldxB5iL7GP6CK6iR5iP3GAOET0EoeJI8RR4hhxnDhB9BGniH5igDhNhHeEQ7NNg4gdEYjcGYnoXdGI2R2D2D2xiNsbh/iueCzsXoiEngRoD2ihO6hDYm8ikg4nIfloMpYcW4KU4ylI7UtF2sk06Pv1SB9IR8aZDGSezUTWuSxkn8/G0gtLsfzicuRcykHe5TwYrhhgvGpE/rV8FFwvQOGNQphumrDy1kpYbltgvWPF6rurYbtnQ9H9IhQ/KEbJwxKUPipF2eMylD8pR+XTSlQ9q0L182rUvKiB/aUdta9qUfe6DvVv6uF464DznRMN7xvQ+KERGz9uxKZPm9D0uQnNX5rh+upC67dWtH9vBwvCgvAK4RXCf1m8h/Aewps6n7L4lMXHXo5DOA7hwJAjdY7UOXXyb6VOghPWImjyBsG4WXbMs+UPm8ualliBMSENgolh6xFVYeVclr+Ti2Nn1mDUZIcQQ5Ja3IQVmj2Si+Pn1GJ0iFOIMXScZk0RJxf9le2dYTBCFiRua4rI9oYsKnH/2EHBG9yCzDVbIAuS3J0tsr2hSZXucWOmNLAg/hJEU69H1OZUj/S7slKGCrKgOQe67cs80u/KSmFBAlwPUQSZFFXlsx6iCDIl1s4rxF8rxLtAFetKF39FsijazvQRBVncmSfGyaLoew0sSKAEUfaQkU5ZSsVQ2UP4lBXAEm6Y1SBWxtw1Rp8l3Igyi1gZkeWruIQbqJq6IkZUY6bPmrpmnVmIoWs3ck09UCaH2ZZccfSNdGZ4mBzkPWSqbp3b5KApNWPCnDpo21Z4mBzkPWR6cjWbHPzhOgnNMonNWQkMlWhdXCkOCTOZhSCzc21inBIYKtG6uFIcMt9WzIL4Q5BRkxwQDEbq3oIoNqDRwU4IBiN1b0HYBsS+LPZlsVGOnYvsXGQrKXt72dvLZmtaBex+Z/c7tyNwOwL3h3DDDjfscAcVt7RxSxv3GHLTJzd9/j9duD8AYLs2eJNnWy0AAAAASUVORK5CYII=</ss:Data>
            </when>
            <when test="@SecondsLeft='23'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAA7ZJREFUaEPtm/tLk2EUx9/tN60oKjA10CAqzWmKTlPRFE3FUNQsreFKcy3NNC+ribY1xW7aupdZeSlLu1PRhaKiKIqKLlQURRcqiqK/4dvZk+/YZFrB6y9xfvjw7t377N27fTnPec55zlFtkSSoJElyRf2P53/6/ODrjnPHd1jsGkmlUgnUarXno+r3+yo1jRl4PdTY4e7jdm2I+wz7HAPPN9x9lPgdkkOQVqKN2ErYie3EDmInsYvYQ+wl9hHtRAdxgDhIHCI6iS6im+ghjhC9xFHiGNFH9BPHiRPESeI0obFrELotFGHbwxC+MxwRuyIQuScSUXujEN0ejZj9MYjtiEXcgTjEH4pHQmcCErsSkdSThOTDyUjpTUHq0VSk9aUhvT8dGcczkHkyE/NOzUPWmSxkn81Gzrkc5J7PRd6FPORfzMeCSwtQcLkAhVcKsfjqYuiu6VB0vQj6G3osubkExbeKUXK7BKV3SmG4a4DxnhEr7q9A+YNyrHy4EhWPKlD5uBJVT6pQ/bQaNc9qUPe8DqYXJqx5uQbmV2bUv65Hw5sGNL5thOWdBdb3Vtg+2ND0sQnNn5rR8rkFG75swKavm7D522a0fm9F24822H/awYKwIGwhbCE8ZbEPYR/CTp1XWbzK4mUvxyEch3BgyJE6R+qcOvm/Uie+iavgPWm9YNz0emjK9R5zWWmd+fCbbcZovybBlDQT57KUTi6OnWaG1ySrEEOStjgJLl7mllxM3r0Qo/xsQgjXcf6x9ZxcVCrbO1VXBIcgcTsyRLbXb06l88/29l3vJohvzFqEGowi2xtZbXCOG+3fxIIoJUiEJQvajZlu6XfZUgYLMjj9LluKRl/FgigliKf9EFkQH63J435IRvdCBKbWialLW2tkH6K0D3HdoIptzRZTkUOUpH3ZHgVx9SETZ65DvGU5W8hIWYjsQ4ZaZTl2DGULcXXsMaYy3jFUegs32KgTlhFSVvRXW7iBc+ucjn2MfzMLoqQgshja5ty/3lPP7Cl0LoFZEAWLHGYYFomlb5Qtx63IweFDJievdhY5jA9qQEBKrVuRg+xPZsyvZQtRwkIC80rEtCMHhnK0Lo4UhwSXLHMKMsrXJsYGpNaKqpNZxjJx7nDsXHWikIV4+VghGIjUBwviWgakNS/FhKBGZ9pkYvA6hOgredk7kstersviQjkulOPKRS4l5VJSru3l2l6OQ5SIQ7j6ndsRuB2B+0PICrhhhxt2uIOKO6i4pY17DLnHkJs+uQtX4S7cXxiCUFj90o6iAAAAAElFTkSuQmCC</ss:Data>
            </when>
            <when test="@SecondsLeft='24'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAA2BJREFUaEPtm+tLk3EUx5/tnZco8IW3iF6E2UxF8cYUTVFUlA0niuaa4nRoatpERXE5t6HpNpcXvKSpWZpaUVR0oagoiqKiCxVF0YWKouhv+Pbbk89Qc1YwB8V58eHZ3G9j8t15fr9zzveIzBwHEcdxixH/5fPfvX/x6/vMwZxYJOZEIhEnFv+8Ln4s/M1xXVgrErP1y963fO1qn7PkNSefs+r3WOm7rsH/wdkFsTCsjB6GjdHL6GP0MwYYg4whxjBjhDHKGGMcYowzJhiTjMOMKcZRxjRjhnGMMcuYY8wzgs3BkFgkCLGGINQWirADYQjvDUdEfwQiByIRNRiF6KFoxI7EIu5gHKSjUsSPxSNhPAGJE4lImkxC8lQyUo6kIHU6FWkzaUifTUfGXAYy5zORdSIL2SezITslg/y0HDlncqA4q0DuuVzknc9D/oV8FFwsQOGlQhRdLoLyihKqqyoUXytGyfUSlN4ohfqmGuW3yqG5rUHFnQpU3q1E1b0qVN+vRs2DGtQ+rEXdozpoH2tR/6QeDU8b0PisEU3Pm9D8ohktL1vQ+qoVutc6tL1pg/6tHoZ3BhjfG2H6YELHxw50fupE1+cudH/phuWrBdZvVti+20CCkCAUIRQhdMuiPYT2ENrU6ZRFpyw69lIeQnkIJYaUqVOmTqWT/6t04p+0B55+7TwbtrYgtKr4j2pZgdIWeAcaqZblyuLi+qBmePjpeTE4zuxAUlq2anExSKHl15IgLqz2blGqYBckvi+Tr/YG7Kh1COLp3+5UkCitxrGOBHGhIJFtMsTsz1pSfhcixZkgyT274CPRwTvASBHijn6IIIhvTOOKEWIXQ3Y8jwRxR4NKapHzv3q7KMnD8l8E8dmmgz1C7A0qihA3dAyFPWSlU9am1HqEaSocHUMSZI0FkVQo+cjYvlu1YgvXy98ArwADHxmCGMKpbF2giVq4ruypC2LEmBROe+okiJtMDsGanfzRN9qQs8TkYN9DNqbsdWpyoFvWGtyyNueq+U1cSAyFbJ2/sjxEoi4jQdxpA/Lw1YNnIVNfLshqNiBhL6HE0IWJIfmyyChHRjlyLrIoICspWUnJ20veXjJbk/ud3O80jkDzITQfQgM7NEFFE1Q00kYzhjRjSEOfNPT5D0zh/gCjfGcfJB1IWAAAAABJRU5ErkJggg==</ss:Data>
            </when>
            <when test="@SecondsLeft='25'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAA7FJREFUaEPtW+tLU2EcPts3tShKMLUPRsvZTCXxhoaX1VKZOC9kVmOKS9O0vKGlObVNnM2tVZaX8tLNbiRFRReKiqIoKrpQURRdqCiK/oan95w8y6nd4OxD8fvw8O6wc96z8fB7f7fnJ+vkOMg4jhsL+V9e/+55k1XByWQyTi6Xf19lo+votdt34j3j7pXJJz7n2u8P9vnV+8V9fvk7JnuHB/4HxxNiZ3AwbGVwMmxn2MHQxbCToZuhh6GXoY9hD0M/wwDDIMMQw16GfQz7GQ4yDDMcYjjMoLAqMK9jHpQ2JUI6Q6CyqxDqCEWYMwzh28IRsT0CC7sWInJnJKK6oxDdE43YvljE7Y5D/J54JPQnYNHgIiQOJSJpbxJS9qdAfUCNJcNLoDmkQeqRVKQdTUP6sXRoj2uRMZKBzBOZ0J3UIftUNnJO5yD3TC6WnV2GvHN5yD+fjxUXVmDVxVXQX9LDcNmAgisFKLxaiKJrRTBeN6L4RjFKbpag9FYpym6XofxOOSruVmDdvXWovF+JqgdVqHlYg9pHtah7XIf6J/XY8HQDGp41oPF5I5peNMH00oSWVy1ofd0K8xszLG8taHvXhvb37bB+sGLLxy2wfbLB/tkOxxcHnF+dIEKIELIQshA6ssiHkA8hp05RFkVZFPZSHkJ5CCWGlKlTpk6lk/+rdOKftB7eszYLmK5sRFh5waS1rMXd+ZgSYHFHoAWz4zdRLUuq4uK04AZ4zWoVyOC4ThdURasnFBcVWVWTEqK2FRMhUhCi0BvAE5KwI12o9gYkV7oI8fbfPIGQmfNN0PSspGqvp8rvkS2ZiOnQupXfRUsZT4h613KBLP7ImqkyIWhpHZI7jFR+93Q/RCTEL6bezULmZv2wnrFHmzK3lvohnmpQxdt1ghXwpKT06twImTG/CT4BZsFCxhLCfw4vrCYfIoUPGd8xFH3Iz6IssWOodhoQnFPjImdqYBsRIjUhqlK9YBkL1hr+uIWbPZIP39BmECES99RFMmLacv66p545rCdCpPQhISUrhdA32pztJnLgfchsdbVL5OAft1HwIRFryiaEvZQYSmQhQblGwUGLiaGYrQsry0NUxtUuQnz8zS5nHqSpg+54nqA64aMs7UAh+RApfIiXXysEjGbq4wkZKwPi8xDeSsTSia+qGXNS68EfWSQDkshCSJdFQjkSypFykaSkJCUlbS9pe0lsTep3Ur/TOALNh9B8CA3s0AQVTVDRSBvNGNKMIQ190tDnvzGF+w3WKXqJdv3ViwAAAABJRU5ErkJggg==</ss:Data>
            </when>
            <when test="@SecondsLeft='26'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAA8FJREFUaEPtW1lPE1EUnvZN0GjigyxGi4CQIhBQFoGAVLASDA0giFoLgiI7KAKBQFjasBYriyyyiktco1HjEo0ajUajxiVqNBqXqNFo/A2fd67OBJrikhQe9Dx8mbb3zkzbL2fO+c6iaBMEKARBGA/lX763dX61USUoFApBqVTy4/jXSsWPz2yu/dwvrUl7FUq23+o8eY+te1hd51f3/6PvMU2/QxAJMTO0M+xisDB0MHQydDF0M/Qw9DL0MfQzDDAMMgwxDDOMMIwy7GMYYzjAoDKq4GZyg3ujOzyaPODZ7AmvVi94t3lDbVbDp90HvhZf+O32g3+HPwK6AhDYHYhlPcsQ1BuEkP4QhO4NRdhAGMIHwxExHIHIkUhEjUYheiwamv0axByMQeyhWGgPa7H6yGrEHY1D/PF4rDmxBgknE6A7pUPi6UQknUlC8tlkpJxLQer5VKRdSMP6i+ux8dJG6C/rYbhiQPrVdGRcy0Dm9Uxk3cjC1ptbkX0rGzm3c5B7Jxf5d/NRcK8AhfcLUfygGCUPS7Dj0Q6UPi5F2ZMylD8tR8WzClQ+r0TViypUv6xGzasa1L6uRd2bOjS8bYDxnRGm9yY0fmhE08cmtHxqQevnVpi/mNH+tR2WbxYQIUQIWQhZCD2yyIeQDyGnTlEWRVkU9pIOIR1CwpCUOil1Sp38W6kT56giODjVc8zxqoJvfvpvc1mRTZvhpi3HTFcjZrmaKJdlr+Ti7MWVmOFUx8kQhDYZ6swtkyYXVbFlfJ9rWBVCynKhO7SJCLEHIR56A0RCwjvjeLbXZUWxTIiDc71NQuaqa/ieoNJtlO21d/o9sDYBwc3xE9LvkqXYImRhzE5Ohs+mIkq/T1c9RCJkXnD5BAvR7FknW49oJTNdjByBeQVUD5mqAlWYWcf/dJGU6D7dBEIWxJTyNZGEhGMp8DEUyQQtzS8kH2IPH2JdMZR8iK0oy9G5QSZErBhq+/UyIRRlTUEJV52j55axJM9gs4RrTYhYwhWtRbQaIsTOhEhkBJuSJq2pEyHT1OTgnb2Bh75BDYkTmhxEHzJfs11ucpB8iGgRGouBR1mShSzSVpAPsYcPUSVn8UeOJAwltc6PTIeos7bIhKwaXgtHlx9+RBSGolLnTp4p9fihDCLEHoTMmFcHjp9K3ZoQ6zaglT1pcFleKYe8olKPGzRQ2DtVYS/1ZVGjHDXKUecitZJSKyn19lJvL4W99gh7qfudxhFoHIHmQ2hghwZ2aIKKJqiopj5dNXWaMaShTxr6/J+mcL8DZ/qFSSymhjUAAAAASUVORK5CYII=</ss:Data>
            </when>
            <when test="@SecondsLeft='27'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAA45JREFUaEPtW/lLVFEYfTO/aUVBgjqjoqgpY2qKW6O4oahprihujOKoueaGY+6jM7jOZC64pKntC0VR0UJRURRFRQsVRdFCRVH0N5zuPB1pdFyCGYj4fjg85s17982bw73f/c53PkE/x0HAcdyfEP7lZ/29zW1iTigUcgKBYMnR4JzA8Bpj1y+MM3+tQMjGXHTf4metNM5Kz9ePs+LvmH8vc78HpyNEw6Bl2MswwDDIMMQwzDDCMMowxjDOMMEwyTDFcIBhmmGGYZZB3CaGvdIeDh0OcFQ5wkntBOcuZ7h0u8C1xxVufW5w73eHRCOBh9YDngOe8NrnBe9Bb/gM+8B3xBd+o37wH/NH4EQggvYHQTopRfBUMEKmQxA6E4qw2TBEHIpA5OFIRB2NQvSxaMSciEHsyVjEnYpD/Ol4JJxJQOLZRCSdS0LK+RSkXkhF2sU0pF9KR8blDGReyUTW1SzkXMtB7vVcyG7IkHczD/m38lFwuwDyO3IU3S1C8b1ilNwvQemDUpQ/LEfFowpUPq5E1ZMqVD+tRu2zWtQ9r0P9i3ooXirQ8KoBja8b0fSmCS1vW9D6rhXt79uh/KBE58dOqD6poP6sRteXLnR/7Ubvt170fe+D5ocG2p9aDPwaABFChNAMoRlCSxbFEIohFNRpl0W7LNr2Uh5CeQglhpSpU6ZO0sn/JZ3Yhu2GpU0Hj01uTfAsz1uiZYVqsrFO1In1IpUhxCpsEKuRfFxGWpYpxMWNWxphYaPkyeC4/gVICgoNxEXn5Cr+O2OE2EmbSVw0hdrrkiuDjpDgoThe7RWFz/3pOljadhgQYhu0B2HanCVqr1OMAkGKMiLEFIT4ticioCfeQH7Xz5TFhCwnv1t5tJH8bs56iJ4Q6wDFqvWQ8B45dDOE6iFmKlBJNUlzyxWLJxHjSasSIpY2IbxXToSYa4boY4ixXZaxJUu3XFHF0EwlXElJLj8ztpbJ1lTC3VZShq15VUSIOWrqejIC1KlrrqlbSdqw80gOEWJqQtyLs/mtr39nioHJQRdD7CJrjJocdhzMhGh7I5kcTO06cUyT80Fcnxjqs3X+yPIQibzQKCGuKTXwr9tFhJiaEAtrJXjMZ+qLCVnOBrRZ0ko2IPJlkS9r1TyEjHLkXCTnIllJydtL3l4yW5P7ndzv1I5A/SHUH0INO9SwQ2qvqdVeammjHkPqMaSmT+rC/We6cH8DnJml6mgl8lcAAAAASUVORK5CYII=</ss:Data>
            </when>
            <when test="@SecondsLeft='28'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAA8xJREFUaEPtW/tLU2EYPttv3Si6MJ0Vlpa6ZebwloqmVCaWpqJoLq15SedKM1S2tM1taOnMyuimZXSjoigqulBUFEVR0YWKouhCRVH0Nzx958szdkyF4AgR7w8Ph+2cncPOw3t/XlWHIEAlCIIv1H/x2WbTCCqVSlCr1cMeZdeo5NcO+/v+a1Vq9owBvxv4zOHuM9zzpfv8C/9DEAnxMHQybGXoYtjOsIOhm2Enwy6G3Qx7GPYy9DD0Muxn0Ng08NvoB22zFgGbAjDNMQ3TW6Yj0BWIGe4ZCGoNQnBbMGZtnoWQ9hCEdoRC59FB36lHeFc45m6bi4jtEYjsjoRhpwFRu6IQvTsasXtjEbcvDvE98UjoTUDigUQk9SUh+WAyUg6lIPVwKhYeXYhFxxYh7XgalpxYgvST6cg4lYGlp5ci80wmss5mIftcNnLO5yD3Qi7yLuYh/1I+Ci4XoPBKIYquFsF4zYji68UouVGCVTdXwXTLhNLbpSi/U46KuxWovFeJqvtVqH5QDctDC9Y+WouaxzWofVKLuqd12PBsA+qf16PhRQMaXzbC+soK22sbmt40ofltM+zv7HC8d8D5wQnXRxfcn9xo/dyKti9t2PJ1C9q/tcPz3YPOH53o+tkFIoQIIQshCyGXRTGEYggFdcqyKMuitJfqEKpDqDCkSp0qdWqd/F+tE//kdRjt18IxIcSG8OqSQXtZhvVlmKRrxlity4vwklrqZSnZXBw/24pRfg5OhiB0eKEzlcmai2FFFu+5qLoK3lyMNFfz72amNVJzUYlub7CxGCIhCTvSebdXu6DG+9JH+7fICBnj7+TnROvw7faODXBhXICbCFGCEIM9EzGbM2Ttd8lShiJEJEW/ch23kGVHiiASMkVvJ0KUIGSweYhEiCamQWYhhtoymUvTzrdisn4TJ2Rxt4kIGQlC4j1Z/KWLpKTsyfpjQBVrM3GX5RtrDGYLBXUlg7rvxFCKIUNlWWl9eRAtQ8y0fElJbS8nC1HaQnSVRm4Zc8zFg45wY6yrMUbrRMSaKh7UAxfXe0mhGKLwTF0iI8adM+RMfWJYEyfEd6YuuS/KshQkJLRiBU99o53ZMpGDGEOmpq73ihzEtJcIGWHVSWBuKXc7UmEoVev8yOoQXWmZl5Cg5b9rFMllzas0e11WXIOZYogSMWSUxgGO/kp9ICEDZUBhRousbSKmvYn2NZRljVSWRbosEsqRUI6UiyQlJSkpaXtJ20tprxJpL6nfaR2B1hFoP4QWdmhhhzaoaIOKVtpox5B2DGnpk7Zwld/C/QX/2ZBxSJwhKgAAAABJRU5ErkJggg==</ss:Data>
            </when>
            <when test="@SecondsLeft='29'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAA7xJREFUaEPtW+tLU2EcPts3rSjQUmeZoqVsOdG8paW5MhXFoZJp6jS1pXm3nG2lTje8zrxUZhcqiqLCKCq6UFQURVHRhYqi6EJFUfQ3PJ3z6hluzPLDpAu/Dw/n7Fx2tj37nfd5fu9zJD0cBwnHceMh/c1rg86Nk0gknFQqZcvx6+I2+6XNMZLR8yZ1/tixEil/vN15v7zG2GdzeI0J3udv+B6cQIiFRy+P7Tz6eAzwGOSxg8dOHkM8dvMY5rGHh5vODe6N7pijnwMPgwc8t3pC1iSDd7M35hnnwafVB74mX/iZ/eDf7o+AjgAs6FyAwO5ABPUEQW6RQ9GrQHBfMJT9SoQMhCB0RyjCdoYhfCgcEbsjELUnCtF7oxGzLwax+2Ox9MBSxB2MQ/yheCQcToDqiAorj65E4rFEJB1PQvKJZKScTEHqSCrSTqUh/XQ61GfUyDibgcxzmcg6n4XVF1Yj+2I2ci7lIPdyLvKu5CH/aj401zQovF6IohtFKL5ZjJJbJVh/ez20d7Qou1uG8nvlqLhfgcoHlah6WIWaRzWofVyL+if12PR0ExqeNUD3XIfGF43Qv9TD8MqAba+3oelNE1retsD4zoi2920wfTDB/NGM9k/t6Pjcga4vXej+2g3LNwt6v/ei70cfiBAihCqEKoRuWTSG0BhCgzqpLFJZJHvJh5APIWNITp2cOrVO/q/WiVd8NVw9WxlmBRoQXFHosJe1YigHsiV6TJeZGPySdNTLcnZzceZCPVw8jYwMjuuxQl5catNcVO1ag2myNrZ/WXsRFmbWs3V3RTM1F53V7Q3I10AgJHYwhXV7ZctrrIS4erXaEOIVvYXtEypD6Paq+jXWY5VFddTtdUb7PawlHZGdqTbtd7FS7AmZ5jVaHSIhQvtdWBe2zfA2EyHOIMTRfIhIiEekzqZCiJA/MEEVY1Gzf7xASsKw2iEhwv5Vw3lsgooqZIpnDMUxxJHKCtZusI4ZioJqqEeyrYTMVrTQLcvZtyx5WT6rjEUbNRNO4Sq1ZVbJK8hfUZUtrqgiQpxJiEhGpDlz0nPqCk01yd6pCDkEadcy6RvRlmETchDGkLmqOochh/B6rVVxJQ6uIx/iLB/im1XCfljRGIpunS15HyIvKbUhJKyulDl1ZgjlzUjeV0BO3ZlO3cXDCIYxp25PyPgYkCB7Bac+P3Ezc+oUA6JcFuWyKChHyUVKLlKUlLK9lO2lsDWl3yn9To8j0OMI9HzIVPSySGWRyiKVRSqLVBapLFJZpLL+CZX1E8GzlxAzQMeGAAAAAElFTkSuQmCC</ss:Data>
            </when>
            <when test="@SecondsLeft='30'">
              <ss:Data ss:Type="image">iVBORw0KGgoAAAANSUhEUgAAAGQAAAAoCAYAAAAIeF9DAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAA2hJREFUaEPtW+tLU2EcPts3rUzR1M1Lirelqbi8peJUFDVNUdEmXvOGS9fW1HmdTideZ+YFowsVRVFRFBVdKCqKoqjoQkVRdKGiKPobns4OnbGp85OtiN+Hh537YTz8zu99n/d5BOMMAwHDMJYQLrLfoXFihAIhIxAIGKHQ+nexY0teM+85S97/+1qBkH2vjffz71rqOVbn/uH/wZgIMbKYYLGTxSSLKRbTLGZYzLKYY+GkcYJzizNcWl3gqnWFW7sb3Dvd4dHlAc9uT4h1Ynj1esFH7wPffl/4GfzgP+iPgKEABA4HImgkCCFjIZCMSxBqDEXYRBjCJ8MRsSsCkVORiJqJgnRWiui5aMTsjkHcnjjE741Hwr4EJO5PRNKBJCQfTIbskAyph1ORdiQN6UfTkXEsA5nHM5F1IgvZJ7ORcyoHuadzkXcmD/ln81FwrgCF5wtRdKEIxReLUXKpBPLLcpReKUXZ1TKUXytH5fVKVN2oQvXNatTcqkHt7VrU36lHw90GNN5rhOK+Ak0PmtD8sBnKR0qoHqugfqKG5qkGLc9a0Pa8DdoXWrS/bEfnq050ve5Cz5se6N7q0PeuD/r3egx8GIDhowGDnwYx9HkIw1+GMfp1FGPfxmD8bsTEjwlM/pwEEUKEUIVQhdAni3oI9RBq6jTKolEWDXtpHkLzEJoY0kydZuoknfxf0knidDbWRLfC0bOfg3fqDpta1tqMVqwUGzj4Z2pJy1pucTF2JAcOnnqOCIYZN8MjVrtAXHRdp+POb+ypQ4Kuntt2C+slcXE51V63DS0Irqzm1N4wRYWZEEdRvxUhUnUdd85UGbzaa9o2HYvXbiO190/J73ylBMsVVoSI4jtsEuKd0E2ELDchSTObIE5RcZ+u8KaqBT1khWjAJiGrvAaJkOUmxLKHuEi6IW2rsKoQIsTOK4Z8hVg29kjlVvOKIRFiZ0L4JVyvFLW5sZtI4JdwiZC/REjyXK55CGxJCDV1OxGyOrgTItl2K5MD30/8NyvNFULDXjsR4uCh5z5RYpmKc51Iqmu4fVNjn+86oYmhHWxA61VbYKoSXjZxDulCkLyRpBPyZZEvi4xy5Fwk5yJZScnbS95eMluT2Zrc7xRHoDgC5UMosEOBHUpQUaSNIm2UMaTQJ4U+KYVrK4X7C1juj6bMJxXfAAAAAElFTkSuQmCC</ss:Data>
            </when>
          </choose>
        </ss:Cell>
      </if>





      <if test="@SubmittedQuantity">
        <ss:Cell mts:ColumnId="SubmittedQuantity">
          <attribute name="ss:StyleID">Quantity</attribute>
          <ss:Data ss:Type="decimal">
            <value-of select="@SubmittedQuantity" />
          </ss:Data>
        </ss:Cell>
      </if>
      <if test="@LastPrice">
        <ss:Cell mts:ColumnId="LastPrice">
          <attribute name="ss:StyleID">Price</attribute>
          <ss:Data ss:Type="decimal">
            <value-of select="@LastPrice" />
          </ss:Data>
        </ss:Cell>
      </if>
      <if test="@AskPrice">
        <ss:Cell mts:ColumnId="AskPrice">
          <attribute name="ss:StyleID">Price</attribute>
          <ss:Data ss:Type="decimal">
            <value-of select="@AskPrice" />
          </ss:Data>
        </ss:Cell>
      </if>
      <if test="@BidPrice">
        <ss:Cell mts:ColumnId="BidPrice">
          <attribute name="ss:StyleID">Price</attribute>
          <ss:Data ss:Type="decimal">
            <value-of select="@BidPrice" />
          </ss:Data>
        </ss:Cell>
      </if>
      <if test="@WorkingOrderId">
        <ss:Cell mts:ColumnId="WorkingOrderId">
          <attribute name="ss:StyleID">RightText</attribute>
          <ss:Data ss:Type="int">
            <value-of select="@WorkingOrderId" />
          </ss:Data>
        </ss:Cell>
      </if>
      <if test="@ContraOrderId">
        <ss:Cell mts:ColumnId="ContraOrderId">
          <attribute name="ss:StyleID">RightText</attribute>
          <ss:Data ss:Type="int">
            <value-of select="@ContraOrderId" />
          </ss:Data>
        </ss:Cell>
      </if>
      <if test="@OmsFixNote">
        <ss:Cell mts:ColumnId="OmsFixNote">
          <attribute name="ss:StyleID">LeftText</attribute>
          <ss:Data ss:Type="string">
            <value-of select="@OmsFixNote" />
          </ss:Data>
        </ss:Cell>
      </if>
      <if test="@Volume">
        <ss:Cell mts:ColumnId="Volume">
          <attribute name="ss:StyleID">Quantity</attribute>
          <ss:Data ss:Type="decimal">
            <value-of select="@Volume" />
          </ss:Data>
        </ss:Cell>
      </if>

    </ss:Row>

  </template>

</stylesheet>
