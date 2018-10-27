<?xml version="1.0" encoding="utf-8" standalone="yes"?>
<stylesheet version="1.0" xmlns="http://www.w3.org/1999/XSL/Transform"
	xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet"
	xmlns:x="urn:schemas-microsoft-com:office:excel"
	xmlns:c="urn:schemas-microsoft-com:office:component:spreadsheet"
	xmlns:mts="urn:schemas-markthreesoftware-com:stylesheet">

  <!-- These parameters are used by the loader to specify the attributes of the stylesheet. -->
  <mts:stylesheetId>MATCH HISTORY VIEWER</mts:stylesheetId>
  <mts:stylesheetTypeCode>MATCH HISTORY</mts:stylesheetTypeCode>
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
        
        <ss:Style ss:ID="GreenDelta" ss:Parent="Default">
          <mts:Display>
            <ss:NumberFormat ss:Format="#,##0.00;-#,##0.00;"/>
            <ss:Font ss:Color="#00FF00" ss:FontName="Times New Roman" ss:Size="10" ss:Bold="1" />
            <ss:Alignment ss:Horizontal="Right" />
          </mts:Display>
        </ss:Style>
        <ss:Style ss:ID="RedDelta" ss:Parent="Default">
          <mts:Display>
            <ss:NumberFormat ss:Format="#,##0.00;-#,##0.00;"/>
            <ss:Font ss:Color="#FF0000" ss:FontName="Times New Roman" ss:Size="10" ss:Bold="1" />
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
          <ss:Font ss:FontName="Times New Roman" ss:Size="10" ss:Bold="1" ss:Horizontal="Left"/>
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
        
        <ss:Style ss:ID="BuyText" ss:Parent="CenterText">
          <ss:Font ss:Color="#00FF00" ss:FontName="Times New Roman" ss:Size="10" ss:Bold="1" ss:Horizontal="Center"/>
        </ss:Style>
        <ss:Style ss:ID="SellText" ss:Parent="CenterText">
          <ss:Font ss:Color="#FF0000" ss:FontName="Times New Roman" ss:Size="10" ss:Bold="1" ss:Horizontal="Center"/>
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

        <ss:Table mts:HeaderHeight="13">

          <ss:Columns>
            <ss:Column mts:ColumnId="SecuritySymbol" ss:Width="50" ss:StyleID="TopLeftText" mts:PrintWidth="50" mts:Description="Symbol" mts:Image="iVBORw0KGgoAAAANSUhEUgAAAEIAAAAPCAYAAABQkhlaAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAbBJREFUWEflV71KA0EQHt/HwnQGbLzOvIEBC1ksJJUECwlp5LCQI0W4NvgESSFcCuF8AYu8hJBXsBt3zttz3OxPds8icBOGJDvzzX7z3eT2coTSQNriZQGbjw1sP7fwJV+ds+VqiaPbEWbPGWazDPN53kkHal5cCZw8TNr7VNYg99XiOQqzD85XV4+b+Fg4grgWOLgY4PhuvOv3cs3lEqOM8GRVHQ/GuFeNawpSLd/+dfwPxtSHrzcZBxIhOUuMm/qurCJAefyzD+eLx9SKwXAe0D/tY++kh8PL4c5Ip48pulxt7ssLjcfUjcFwXkDTQEIoMWgc06kUgPzJ7Xwk6bPK5+u05jIer+4v0m119VqcXyOEh7OtJxA3ohFCCRLyrpNWWLVO38ls6yqm54esqz34PiE9UC7QcRkKMuXzRjgxnVybhm3YfxGieC2ihXARs5E7WCHK9xLpCI2ZCtP48jq2aXCN/b7imn5u+lSG9ATlW1k9VYaAfLm8UV/uocSBSJMYyfnv6dGWXJsr03bvWPyPEOsS81l90zyWR2kHvRJCWbEusFgV1Z8vegyme0dX/BuXY4N947UMMAAAAABJRU5ErkJggg=="/>
            <ss:Column mts:ColumnId="OrderTypeDescription" ss:Width="34" ss:StyleID="TopLeftText" mts:PrintWidth="34" mts:Description="Side" mts:Image="iVBORw0KGgoAAAANSUhEUgAAAC0AAAAPCAYAAABwfkanAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAOBJREFUSEvVlFEKwjAMhuN9fHDXGHgI2Qmk+CDDFykeQHbf6D+MhNLaNMOHBn62tl+Sv13ZbjyOTL3F8ly4N9F8m3m+ViSMhUUtcBa2hVEshXPgcPkhrGsplj+BdYRw3/kMu3KYl5qO3lQ95cJXEGPI1++5sfRIOW9vivfIHomBllxPTq4+xUdc7x+eVoFPQ3L1vNS2spb+6E3DYWCvUjNSR+Yxbn23eNlkOmey1ajeuMUwGLfp0gm2mrYa1dwm06lBPc5tKj3V0vWqbcRtulb4n+udmt6//x6diabTxL3pBVpHq0rqmCyWAAAAAElFTkSuQmCC"/>
            <ss:Column mts:ColumnId="StatusName" ss:Width="42" ss:StyleID="TopLeftText" mts:PrintWidth="42" mts:Description="Status" mts:Image="iVBORw0KGgoAAAANSUhEUgAAADgAAAAPCAYAAACx+QwLAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAShJREFUSEvlVFEKwjAMrffxw4GnEDyCH9ITyPBDhj8yPICUfe2ucZFlxNh06RwIsxDWJu+leUu21W6/A7fkFR4BFmVNgLZpXxa6vasuFUTtqvgJPxZHXA5mDBuLkw+f/b6+1cDNlecSPuzU+dBEDPqFflxRLnGUHG8cwlixslYDX++g6CyJw27zPXUffeo0sFxWnCWXBeNkS7UziUrFLbkwjwU3B8YfPLj63s2sYjjXFBva1m9Sfo6lHJLPz5hrmIr+Thmn+3hNqbpRXLEtwBWbAqwmiyQe+fGMK+anmBaXOQjH8dY6OS5LYG7hvOgcgTHsFHHIMQvU3rAsxoob480lMkugdikfXY5J+eU0aN/o1M4Rzyzw24t+xf8DgevuL7pgc/7oYcn2BBXoEW7L33oRAAAAAElFTkSuQmCC"/>
            <ss:Column mts:ColumnId="ExecutedQuantity" ss:Width="68" ss:StyleID="TopLeftText" mts:PrintWidth="68" mts:Description="Executed" mts:Image="iVBORw0KGgoAAAANSUhEUgAAAFoAAAAPCAYAAABk69hGAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAfdJREFUWEftVj1rwzAQVaGDhw4eMnjoEujYoYYMNXQKdAl0aMZAhuJfUEKGYrIUkylTMZnyQzMUrn5KVJSrZMlfnSw4bOnu3t09n2RdzV/nJMpx/D7iMYweGAiuAyFAdL7JqfgqBumKg31Bh/1BSlG+y2aevcwo/8wp+8jssil1kCob6HxsXBg2fZc5qDx98nXZ6Fjnd/CpCzgWyVNyInqd0Wq9Mst7uQ6x6bFeYUOWcYHH/dWcPxvm8BurAk9PU9rXqbvCFhyLZHImummnefqpItSuwNy5QzyxXTh1YvE8Xdg+enAsxndj2dGL5eKi3Xn7t52rAoCD0Ravjn+deHqedWLYbNNlSuBYRKOI8m1O8SSm9C2V5yzmNmmq56cH8PWhz006lY/NR5KpYXIMrldz2KEmU34qpqtmE1fwAcngFRyLMAxpt91R/BD3KqoQxMFQ8fQC+ZqyVfYuDG7v8rfh6vl1wQs4FsFNcOrofySax9IJ1MnhH4HbuT6MjegqXFuMNvyA4xPR5bnZBsjH10Smrat9CNVj8k437RhTt/t8KJ/aXDaS6HAUyr+/y7iN/s8ByBZ4F5u6uuqo8dkF/Jjw3UVt6la+4FhEt5G8L3YBOGCY/3PgWN6jp8/Tgege/1HyHp08JvIKEt8P0hcH4Fjg7jxI/xz8AOaSMqERksrrAAAAAElFTkSuQmCC"/>
            <ss:Column mts:ColumnId="AveragePrice" ss:Width="37" ss:StyleID="TopLeftText" mts:PrintWidth="37" mts:Description="Price" mts:Image="iVBORw0KGgoAAAANSUhEUgAAADEAAAAPCAYAAABN7CfBAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAARlJREFUSEvVVc0KwjAMju/jwYJPMPAB3HHgQXwCGTvI2EXKTp5kD7uDEJfNSAn9VQ+28NGl+emXJmtX5b7E8TFC1kN3Gof7kDVAXzW2l9aNbtIRfDak+5WNbZ8AhyWJpsW6qe04T+sEoUfH8MVhF6cN78F7ytnBMVwJTwWYFFdJyrJ6IX2w2g4ucxLVoUKaU8Gk2E/KqfE+tQfda1Rbhafjae5rkn0wbd6kXz6uFpPrHN9c962ZfGwc4dbfUG3UR7CR5lisI5kGz65vnz7Eb6nEl0nY/M0kXIlxQlJvHk4Mt/mfiDGMJWojnJpEKh+gGyHVySy978RlS8kTtp24bNEYbkBvQIzhP9tAsSvyT4KuV7XOG0DvQ+54AuQDLo2QpC9JAAAAAElFTkSuQmCC"/>
            <ss:Column mts:ColumnId="VWAP" ss:Width="37" ss:StyleID="TopLeftText" mts:PrintWidth="40" mts:Description="VWAP" mts:Image="iVBORw0KGgoAAAANSUhEUgAAADUAAAAPCAYAAABEB4e7AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAARFJREFUSEvdVFsKwkAMjPfxw16j4CGkJ5Dih4g/UjyA9L5xUzY1hGQfUhB3IXS3mUxmzNZdf+wRWlvza8bWAm7XGy5xj08+W0/G1GA9vi04pG7BB+NlxPEcg/ZOoFqE8xbx5fBLz9iLsbK3yy31sW6l/zOp1IRijhut0w3vafFZ53N4r66GT2rhPUyPCUuDRUo8veNzSd7qJaeiuXP8Fh9Mz2AqBn1X8mztV+GhZmko6mmv8yV4XcecstbD6P7kAbpDhzXBjaiGlq7V+VK8xDGn9V2VaN3cFJuVhi3B1o+icV5dzli1KUt0alopvJ7035uSV0sat66cdc1T0/pqUrnx/zrfqKl9+PdrLGA4DdhavAG0BcpKRnczSAAAAABJRU5ErkJggg=="/>
            <ss:Column mts:ColumnId="DeltaArrow" ss:Width="15" ss:StyleID="TopLeftText" mts:PrintWidth="15" mts:Description="DeltaArrow" mts:Image="iVBORw0KGgoAAAANSUhEUgAAACkAAAAPCAIAAAD293GKAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAHZJREFUSEtj9PL1YhgoMGXSlIFCDNWV1SioFsyFkJgIlzgeLXApDJMZikuLEaiwuBiOkMXhbKAsLnH8UpgmFxZj+Burd2kjyNDS1DJQiKGlrQUXAsY6VilSxXGZz2BoYDhQaNRu+of8yA1zHUPDAUIMyYnJA4UA8DtFoGMeM4EAAAAASUVORK5CYIIAAAAAAAAAAA=="/>
            <ss:Column mts:ColumnId="Delta" ss:Width="30" ss:StyleID="TopLeftText" mts:PrintWidth="41" mts:Description="Delta" mts:Image="iVBORw0KGgoAAAANSUhEUgAAADYAAAAPCAYAAACvMDy4AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAVJJREFUSEvdVb0KwjAQjuDQwaGDQwcXwdHBgIMFn6DgoLuD9AlEHERcRHwAKU591zNXjJzpJaZVKDbwQXP57ufrXdNOskhAtGwF3UCI7JbB3+OeQX7PC2TqebVcgTgejlDCibGZPB8O+iDPh1uVQ+JezhegKKZwt99BCduyDSyL9UX/J6gbmwvzaz5Xi7aRmC8+UyfmiOexpWNcF5VNL91hc892n/FDHi4b/1t7PFXCzDa69loI5XA2M4bNr0puX266TmE4GiphVzWfFuC3Qc9eBT75eE5tdOw4P8r38XPVZp5hbBQlpxKifgRCTiT4QhdD+eanh2cmz7XHMx2Pi+9bG+WFYfh7YVSorWCb0F8JC3rBd8JoIa6iPnVMd60RYZbb/m2MXWPJvQRTENftWqPYrziKdZI04RMNKl4eTRRZJ2fxH5NjdSu2DPFMCUs3KbQRD02exxLR2PP5AAAAAElFTkSuQmCC"/>
            <ss:Column mts:ColumnId="Volume" ss:Width="57" ss:StyleID="TopLeftText" mts:PrintWidth="57" mts:Description="Volume" mts:Image="iVBORw0KGgoAAAANSUhEUgAAAEsAAAAPCAYAAACshzKQAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAVlJREFUWEftVsGKAjEMrf+zBwv7BQP7AXoU9rDMFyziQQYvUjzNafFjPSxkm0qWGNqmWpcdtIUwmNf30mSSOrPlYgmn75Npq6ACbufg+HVsVlAD4/YOhu2Qtp3H0LQ9ORwx0rinlnYmLRY/l6bl8XOxNgOsN+u4fXo/msBBLMT5uthPGv6Z3EP6kVhRLXleFiOcN3HuIi2uzbT0zspUnBLnXYe+bBd6vRhP40wBD8Vava8An9caJc156NN0YjyNMwXcuIMD+2qh/+jDvYK/r7HfLvHcUCjG5yMX86NP7pF6ctw5pyQW7tHy0nCKY8bDCHZubzZKBjVwkZb0pzDiEc55OSylL/k1uUnuubMepFi8C2tySnHDnVUrHHub/9VZtbnk+Ab/ZWoDpFo/9qblHSTHVuLauMnx/dMxxO+R2mI9C990b10rVuGdbfCzwb40K6mBwe+rZmU1+AEoHGYGV4gJHAAAAABJRU5ErkJggg=="/>
            <ss:Column mts:ColumnId="AverageDailyVolume" ss:Width="60" ss:StyleID="TopLeftText" mts:PrintWidth="60" mts:Description="ADV" mts:Image="iVBORw0KGgoAAAANSUhEUgAAAFAAAAAPCAYAAABzyUiPAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAUxJREFUWEftVc0KwjAMju/jwYJPIfgIHmRPIMODDC8yfAAZO+1dYzMWiaE/++kOYgsfbdPkI/mWdpvD8YCQx3wFmleDGRM0aBvs2q5HY9dQ3SusbhY809qHMb4xntj5mFzGcHAN5Cv9x8bqmGFfP2qUgPJSYnm14JnWLtC5RMAHhyF52KZn9nHZpe0rp1iunJvOmfe+GqXdFeuIA2+3hToxcsaFa25tj+0pnsYaOabiBN2SKfaycyQf230233mKnNbgKE4FQv20d3oAvRVyH1u7/PuOsTwfMQS/tHGs9vt05cARymFqvpqrfxsn1kwcFEPimb1BMDuDKaHfMsnNZyGb9KF1ytzW4EoqoC5YC5YFjHTrVAFdglKX+OxrdNBSzmQdKK+uFEFf6dAV52L+UsClX/JX45N14K8KsDRvMFv7F86YrQEU5wIz5mvwBjz4ea8ctR4qAAAAAElFTkSuQmCC"/>
            <ss:Column mts:ColumnId="CreatedTime" ss:Width="69" ss:StyleID="TopLeftText" mts:PrintWidth="69" mts:Description="Time" mts:Image="iVBORw0KGgoAAAANSUhEUgAAAFwAAAAPCAYAAABp9agBAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAg5JREFUWEftWDFLw0AUbrcODhk6dOgScHTwoIIBFwMuBQe7GXCQ0Kl0kNBBQhcJHSR0kNBB8iuEdIu/q4Nw3rvm5Hq5S3KtYpQ8eFxy73vvXr97fbmkPboZ4RaRzccGhkYOZMDsmy10hlruvUsjtYnshATCg3mAo5eo0X04WBHeiMarmI7Aoz/38WQ8wSC5/RteD3HwFGD/0VcrCQBBCjGif1WffeLCWmV+ZfYqMXgMiyfEhWLdUeCSYLwHD4fLME+4dWFtCZ8R0MyTK3GGAGBXCdiZUCznUyWuEsPnxMfN8imMrfo9MK+Tn2pdcZ6/n06wc+dICB9khBdVOGcDUqHSmbBrcU7r31Bx7TrG3CkuVgQwTj1sX9p5ws1jk1Y47AaMVZURXhX/X3G0CCTty7l1MDpFecJ73R4OFgFGA4TJk5U6w32ZfhGeYflWA74y4ef5+KJv2dq8vSzfQ+2luZAeTtfIejm0OEa2tMINw8DhIqS7oaOMJN5HnOPvda91cqkr1h27+QrvHHW2Ff7LhPNVrptLXfHxa6wgnPRu3aR/osJ1c6g7PnlL8oQbXYOeOnSSF/sz+BYJbxexMl+dXOqKhRef9D2VPDT7PXomrWvifzWv8DmUv2la5BxuX9kN4ZrPsKJCgNNJuk4VhJ9b9EiIThr9Fg7IxkXLSE04nL0bPYwD9t0kWScYlBfx49UnihKFZGgwskwAAAAASUVORK5CYII="/>


            <ss:Column mts:ColumnId="MatchId" ss:Hidden="1" ss:Width="30" ss:StyleID="TopRightText" mts:PrintWidth="20" mts:Description="Match Id"/>
            <ss:Column mts:ColumnId="OrderTypeMnemonic" ss:Hidden="1" ss:Width="23" ss:StyleID="TopLeftText" mts:PrintWidth="45" mts:Description="Side" />
            <ss:Column mts:ColumnId="StatusCode" ss:Hidden="1" ss:Width="15" ss:StyleID="TopCenterText" mts:PrintWidth="20" mts:Description="Status"/>
            <ss:Column mts:ColumnId="WorkingOrderId" ss:Hidden="1" ss:Width="30" ss:StyleID="TopRightText" mts:PrintWidth="24" mts:Description="Order Id" />
            <ss:Column mts:ColumnId="ContraOrderId" ss:Hidden="1" ss:Width="30" ss:StyleID="TopRightText" mts:PrintWidth="24" mts:Description="Contra" />
            <ss:Column mts:ColumnId="SecurityId" ss:Hidden="1" ss:Width="60" ss:StyleID="TopRightText" mts:Printed="false" mts:Description="Security" />
            <ss:Column mts:ColumnId="SubmittedQuantity" ss:Hidden="1" ss:Width="37" ss:StyleID="TopRightText" mts:PrintWidth="37" mts:Description="Submitted"/>
            <ss:Column mts:ColumnId="MarketCapitalization" ss:Hidden="1"  ss:Width="50" ss:StyleID="TopRightText" mts:PrintWidth="37" mts:Description="Mkt. Cap."/>
            <ss:Column mts:ColumnId="LimitPrice" ss:Hidden="1" ss:Width="30" ss:StyleID="TopRightText" mts:PrintWidth="37" mts:Description="Limit&#10;Price"/>
            <ss:Column mts:ColumnId="LimitTypeMnemonic" ss:Hidden="1" ss:Width="50" ss:StyleID="TopLeftText" mts:Description="Limit&#10;Type"/>
            <ss:Column mts:ColumnId="EmptySpace" ss:Hidden="1" ss:Width="240" ss:StyleID="TopLeftText" mts:Printed="false" mts:Description="" />
            <ss:Column mts:ColumnId="EmptySpace2" ss:Hidden="1" ss:Width="24" ss:StyleID="TopLeftText" mts:Printed="false" mts:Description="" />
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

    <ss:Row>

      <variable name="DeltaStyle">
        <choose>
          <when test="number(@AveragePrice - @VolumeWeightedAveragePrice)&gt;=0 and @OrderTypeCode=0">RedDelta</when>
          <when test="number(@AveragePrice - @VolumeWeightedAveragePrice)&lt;0  and @OrderTypeCode=0">GreenDelta</when>
          <when test="number(@AveragePrice - @VolumeWeightedAveragePrice)&gt;=0 and @OrderTypeCode!=0">GreenDelta</when>
          <when test="number(@AveragePrice - @VolumeWeightedAveragePrice)&lt;0  and @OrderTypeCode!=0">RedDelta</when>
        </choose>
      </variable>

      <variable name="ArrowString">
        <choose>
          <when test="number(@AveragePrice - @VolumeWeightedAveragePrice)&gt;=0 and @OrderTypeCode=0">iVBORw0KGgoAAAANSUhEUgAAAA0AAAAUCAIAAADZUCB4AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAGRJREFUOE/tUjESwCAIS3/en1MwJ0aPgdGhXBY1CJfkMTN0ynlaR0s+YZFgL5BgQ83zDQhn/7ypkYt1jS7pynAwNiOWbzQ6LZ5OC0ktbuVgy9uevhit46r/eBcbSihF3CPR9fEDG8DYwQY/O+IAAAAASUVORK5CYIIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==</when>
          <when test="number(@AveragePrice - @VolumeWeightedAveragePrice)&lt;0  and @OrderTypeCode=0">iVBORw0KGgoAAAANSUhEUgAAAA0AAAAUCAIAAADZUCB4AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAGhJREFUOE/tklEOwCAIQ/Xm3hyLbFqBbEv2K+FH8qxCqSJSvgQ4jiINt1xRtTwEdSU9us6qBMgyoBe3QRmq3IAsSQ+SpPr07hzD1kf8H3fzojencbjl7//5mae3s7mejd6tduJH3GGudJj/z1ROhqZvAAAAAElFTkSuQmCCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==</when>
          <when test="number(@AveragePrice - @VolumeWeightedAveragePrice)&gt;=0 and @OrderTypeCode!=0">iVBORw0KGgoAAAANSUhEUgAAAA0AAAAUCAIAAADZUCB4AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAGRJREFUOE/tUssOwCAIq3/OnzMEh8Vo4nGHEQ4SyyNtm6riJgzHsbTkF+glEMz0jgPOToi0hh8XJEFlkPIBXlKVrqBfFkm6hdavxEPpAiKJr3xQ/FYLX03rNvMmi2S7I26xd5YPJL3PVNatykgAAAAASUVORK5CYIIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==</when>
          <when test="number(@AveragePrice - @VolumeWeightedAveragePrice)&lt;0  and @OrderTypeCode!=0">iVBORw0KGgoAAAANSUhEUgAAAA0AAAAUCAIAAADZUCB4AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAGhJREFUOE/tksEOwCAIQ92f++eswsQKLNnF2wgneBRDvUSkfQlwW7QxFovQChCorvJp3Atooa2Z0TlHUIkqNyDLpQdJVqV3pL1+BpOaUXHc/Lnnwuzy0fuZp+7sy171JXztyrf8ialyA0a32MHMYh1IAAAAAElFTkSuQmCCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==</when>
        </choose>
      </variable>
      
      <variable name="SideStyle">
        <choose>
          <when test="@StatusName = 'Held'">HeldStatusText</when>
          <when test="@StatusName != 'Filled' and (@DirectionCode = 0 or @DirectionCode = 1)">BuyStatusText</when>
          <when test="@StatusName != 'Filled' and (@DirectionCode = 2 or @DirectionCode = 3)">SellStatusText</when>
          <otherwise>FilledStatusText</otherwise>
        </choose>
      </variable>
      <variable name="TimeStyle">
        <choose>
          <when test="@StatusName != 'Filled'">Time</when>
          <otherwise>FilledTime</otherwise>
        </choose>
      </variable>
      <variable name="CenterTextStyle">
        <choose>
          <when test="@StatusName != 'Filled'">CenterText</when>
          <otherwise>FilledCenterText</otherwise>
        </choose>
      </variable>
      <variable name="LeftTextStyle">
        <choose>
          <when test="@StatusName != 'Filled'">LeftText</when>
          <otherwise>FilledLeftText</otherwise>
        </choose>
      </variable>
      <variable name="RightTextStyle">
        <choose>
          <when test="@StatusName != 'Filled'">RightText</when>
          <otherwise>FilledRightText</otherwise>
        </choose>
      </variable>
      <variable name="QuantityStyle">
        <choose>
          <when test="@StatusName != 'Filled'">Quantity</when>
          <otherwise>ExecutedQuantity</otherwise>
        </choose>
      </variable>
      <variable name="PriceStyle">
        <choose>
          <when test="@StatusName != 'Filled'">Price</when>
          <otherwise>FilledPrice</otherwise>
        </choose>
      </variable>
      <variable name="PercentStyle">
        <choose>
          <when test="@StatusName != 'Filled'">Percent</when>
          <otherwise>FilledPercent</otherwise>
        </choose>
      </variable>
      <variable name="SoftAskPriceStyle">
        <choose>
          <when test="@StatusName != 'Filled'">SoftAskPrice</when>
          <otherwise>FilledSoftAskPrice</otherwise>
        </choose>
      </variable>
      <variable name="SoftBidPriceStyle">
        <choose>
          <when test="@StatusName != 'Filled'">SoftBidPrice</when>
          <otherwise>FilledSoftBidPrice</otherwise>
        </choose>
      </variable>
      <variable name="MarketValueStyle">
        <choose>
          <when test="@StatusName != 'Filled'">MarketValue</when>
          <otherwise>FilledMarketValue</otherwise>
        </choose>
      </variable>
      <variable name="CommissionStyle">
        <choose>
          <when test="@StatusName != 'Filled'">Commission</when>
          <otherwise>FilledCommission</otherwise>
        </choose>
      </variable>
      <variable name="DateTimeStyle">
        <choose>
          <when test="@StatusName != 'Filled'">DateTime</when>
          <otherwise>FilledDateTime</otherwise>
        </choose>
      </variable>
      <variable name="ShortDateStyle">
        <choose>
          <when test="@StatusName != 'Filled'">ShortDate</when>
          <otherwise>FilledShortDate</otherwise>
        </choose>
      </variable>
      <variable name="OrderTypeStyle">
        <choose>
          <when test="@StatusCode != 6 and (@OrderTypeCode = 0 or @OrderTypeCode = 2)">BuyText</when>
          <when test="@StatusCode != 6 and (@OrderTypeCode = 1 or @OrderTypeCode = 3)">SellText</when>
          <otherwise>FilledCenterText</otherwise>
        </choose>
      </variable>
      
      <if test="@Volume">
        <ss:Cell mts:ColumnId="Volume">
          <attribute name="ss:StyleID">
            <value-of select="$QuantityStyle"/>
          </attribute>
          <ss:Data ss:Type="decimal">
            <value-of select="@Volume" />
          </ss:Data>
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
      <if test="@MatchId">
        <ss:Cell mts:ColumnId="MatchId">
          <attribute name="ss:StyleID">
            <value-of select="$RightTextStyle"/>
          </attribute>
          <ss:Data ss:Type="int">
            <value-of select="@MatchId" />
          </ss:Data>
        </ss:Cell>
      </if>
      <if test="@StatusCode">
        <ss:Cell mts:ColumnId="StatusCode">
          <attribute name="ss:StyleID">
            <value-of select="$SideStyle"/>
          </attribute>
          <ss:Data ss:Type="string">
            <value-of select="@StatusCode" />
          </ss:Data>
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
      <if test="@StatusName">
        <ss:Cell mts:ColumnId="StatusName">
          <attribute name="ss:StyleID">
            <value-of select="$SideStyle"/>
          </attribute>
          <ss:Data ss:Type="string">
            <value-of select="@StatusName" />
          </ss:Data>
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
      <if test="@OrderTypeMnemonic">
        <ss:Cell mts:ColumnId="OrderTypeMnemonic">
          <attribute name="ss:StyleID">
            <value-of select="$LeftTextStyle"/>
          </attribute>
          <ss:Data ss:Type="string">
            <value-of select="@OrderTypeMnemonic" />
          </ss:Data>
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
      <if test="@SecurityId">
        <ss:Cell mts:ColumnId="SecurityId">
          <attribute name="ss:StyleID">
            <value-of select="$RightTextStyle"/>
          </attribute>
          <ss:Data ss:Type="int">
            <value-of select="@SecurityId" />
          </ss:Data>
        </ss:Cell>
      </if>
      <if test="@SecuritySymbol">
        <ss:Cell mts:ColumnId="SecuritySymbol">
          <attribute name="ss:StyleID">
            <value-of select="$LeftTextStyle"/>
          </attribute>
          <ss:Data ss:Type="string">
            <value-of select="concat('     ', @SecuritySymbol)" />
          </ss:Data>
        </ss:Cell>
      </if>

      <if test="@AveragePrice">
        <ss:Cell mts:ColumnId="AveragePrice">
          <attribute name="ss:StyleID">
            <value-of select="$DeltaStyle"/>
          </attribute>
          <ss:Data ss:Type="decimal">
            <value-of select="@AveragePrice" />
          </ss:Data>
        </ss:Cell>
        <ss:Cell mts:ColumnId="VWAP">
          <attribute name="ss:StyleID">
            <value-of select="$DeltaStyle"/>
          </attribute>
          <ss:Data ss:Type="decimal">
            <value-of select="@VolumeWeightedAveragePrice" />
          </ss:Data>
        </ss:Cell>
        <ss:Cell mts:ColumnId="DeltaArrow">
          <ss:Data ss:Type="image">
            <value-of select="$ArrowString"/>
          </ss:Data>
        </ss:Cell>
        <ss:Cell mts:ColumnId="Delta">
          <attribute name="ss:StyleID">
            <value-of select="$DeltaStyle"/>
          </attribute>
          <ss:Data ss:Type="decimal">
            <value-of select="@AveragePrice - @VolumeWeightedAveragePrice" />
          </ss:Data>
        </ss:Cell>
      </if>
      
      <if test="@ExecutedQuantity">
        <ss:Cell mts:ColumnId="ExecutedQuantity">
          <attribute name="ss:StyleID">
            <value-of select="$QuantityStyle"/>
          </attribute>
          <ss:Data ss:Type="decimal">
            <value-of select="@ExecutedQuantity" />
          </ss:Data>
        </ss:Cell>
      </if>
      <if test="@SubmittedQuantity">
        <ss:Cell mts:ColumnId="SubmittedQuantity">
          <attribute name="ss:StyleID">
            <value-of select="$QuantityStyle"/>
          </attribute>
          <ss:Data ss:Type="decimal">
            <value-of select="@SubmittedQuantity" />
          </ss:Data>
        </ss:Cell>
      </if>
      
      <if test="@WorkingOrderId">
        <ss:Cell mts:ColumnId="WorkingOrderId">
          <attribute name="ss:StyleID">
            <value-of select="$RightTextStyle"/>
          </attribute>
          <ss:Data ss:Type="int">
            <value-of select="@WorkingOrderId" />
          </ss:Data>
        </ss:Cell>
      </if>
      <if test="@ContraOrderId">
        <ss:Cell mts:ColumnId="ContraOrderId">
          <attribute name="ss:StyleID">
            <value-of select="$RightTextStyle"/>
          </attribute>
          <ss:Data ss:Type="int">
            <value-of select="@ContraOrderId" />
          </ss:Data>
        </ss:Cell>
      </if>

    </ss:Row>

  </template>

</stylesheet>
