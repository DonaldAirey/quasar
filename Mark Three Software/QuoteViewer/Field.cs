namespace MarkThree.Guardian.Forms
{
    using MarkThree.Forms;
    using System;

    [Flags]
    internal enum Field
    {
        Symbol, SecurityName, Exchange, LastPrice, LastSize, BidPrice, AskPrice, BidSize, AskSize, Volume,
        AverageDailyVolume, DayLowPrice, DayHighPrice, YearLowPrice, YearHighPrice, PreviousClosePrice, OpenPrice 
    };
}
