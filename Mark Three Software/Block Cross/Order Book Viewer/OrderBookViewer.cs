namespace MarkThree.Guardian.Forms
{

	using AxOWC11;
	using MarkThree.Forms;
	using MarkThree.Guardian;
	using MarkThree.Guardian.Client;
	using OWC11;
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;
	using System.Threading;

	public partial class OrderBookViewer : MarkThree.Forms.ChartViewer
	{

		private bool isTickerRunning;
		private int steps;
		private int matchId;
		Series[] series;
		private static Random random;
		private double lowRange;
		private double highRange;
		private double stepSize;
		private Market market;
		double lastPrice;
		double lastQuantity;
		private delegate void DisplayDelegate(string securityName, decimal price);
		private DisplayDelegate displayDelegate;
		private delegate void SimulateMarketHandler();
		private SimulateMarketHandler tickerHandler;
		private Thread thread;

		public OrderBookViewer()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			random = new Random(15);

			displayDelegate = new DisplayDelegate(DisplayForeground);

			this.tickerHandler = new SimulateMarketHandler(SimulateMarket);

#if DEBUG
			// Disable the background threads when in the design mode.  They will kill the designer.
			if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			{
#endif

				// This is used as a signal to terminate the background thread that does the market simulation.
				this.IsTickerRunning = true;

				this.thread = new Thread(new ThreadStart(Ticker));
				this.thread.Name = "Chart Market Simulation";
				this.thread.IsBackground = true;
				this.thread.Start();

#if DEBUG
			}
#endif

		}

		private bool IsTickerRunning
		{
			get { lock (this) return this.isTickerRunning; }
			set { lock (this) this.isTickerRunning = value; }
		}

		private void Ticker()
		{

			while (this.IsTickerRunning)
			{

				try
				{

					if (this.IsHandleCreated && !this.IsDisposed)
						Invoke(tickerHandler);

				}
				catch (Exception exception)
				{

					EventLog.Error("{0}, {1}", exception.Message, exception.StackTrace);

				}

				Thread.Sleep(1000);

			}

		}

		/// <summary>
		/// Opens the a block order in the execution viewer.
		/// </summary>
		/// <param name="matchId">The primary identifier of the object to open.</param>
		public void OpenMatch(int matchId)
		{

			// Execute a command in the background to open up the document.  Constructing an appraisal will require access
			// to the data model to build a model and select a stylesheet.
			ThreadPool.QueueUserWorkItem(new WaitCallback(OpenMatchCommand), matchId);

		}

		/// <summary>
		/// Closes the a block order in the execution viewer.
		/// </summary>
		/// <param name="matchId">The primary identifier of the object to close.</param>
		public void CloseMatch() { }

		/// <summary>
		/// Opens the document for the given object and it's argument.
		/// </summary>
		/// <param name="matchId">The primary identifier of the object to open.</param>
		/// <param name="argument">Options that can be used to further specify the document's properties.</param>
		public void OpenMatchCommand(object parameter)
		{

			// Extract the command argument.
			int matchId = (int)parameter;

			// This will force an empty placement and placement document to appear until we get some data.  It's the same
			// as forcing a drawing of the headers only.
			this.matchId = matchId;

			try
			{

				// Lock the tables
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);
				ClientMarketData.EquityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.MatchLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.ObjectLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.PriceLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.SecurityLock.AcquireReaderLock(CommonTimeout.LockWait);
				ClientMarketData.WorkingOrderLock.AcquireReaderLock(CommonTimeout.LockWait);

				// Install the event handlers.  The ClientMarketData component will advise us when the data has changed.
				ClientMarketData.MatchRow matchRow = ClientMarketData.Match.FindByMatchId(matchId);
				if (matchRow == null)
					return;
				ClientMarketData.SecurityRow securityRow = matchRow.WorkingOrderRow.SecurityRowBySecurityWorkingOrderSecurityId;
				ClientMarketData.EquityRow equityRow = ClientMarketData.Equity.FindByEquityId(securityRow.SecurityId);
				if (equityRow == null)
					return;
				string securityName = securityRow.ObjectRow.Name;
				ClientMarketData.PriceRow priceRow = ClientMarketData.Price.FindBySecurityId(securityRow.SecurityId);
				decimal price = priceRow.LastPrice;
				Invoke(displayDelegate, new object[] { securityName, price });

			}
			finally
			{

				if (ClientMarketData.EquityLock.IsReaderLockHeld)
					ClientMarketData.EquityLock.ReleaseReaderLock();
				if (ClientMarketData.MatchLock.IsReaderLockHeld)
					ClientMarketData.MatchLock.ReleaseReaderLock();
				if (ClientMarketData.ObjectLock.IsReaderLockHeld)
					ClientMarketData.ObjectLock.ReleaseReaderLock();
				if (ClientMarketData.PriceLock.IsReaderLockHeld)
					ClientMarketData.PriceLock.ReleaseReaderLock();
				if (ClientMarketData.SecurityLock.IsReaderLockHeld)
					ClientMarketData.SecurityLock.ReleaseReaderLock();
				if (ClientMarketData.WorkingOrderLock.IsReaderLockHeld)
					ClientMarketData.WorkingOrderLock.ReleaseReaderLock();
				System.Diagnostics.Debug.Assert(!ClientMarketData.IsLocked);

			}

		}

		public void DisplayForeground(string securityName, decimal startingPrice)
		{

			lock (this)
			{

				try
				{

					lastPrice = 0.0;
					lastQuantity = 0.0;
					steps = 17;
					lowRange = Convert.ToDouble(startingPrice - startingPrice * 0.01M);
					highRange = Convert.ToDouble(startingPrice + startingPrice * 0.01M);
					stepSize = (highRange - lowRange) / steps;

					// Initialize the market
					InitializeMarket(Convert.ToDouble(startingPrice) + stepSize * 2.0);

					// Calculate a random price movement and go back in time building the history for this security.  The
					// price movement will be anywhere between nothing and 10 cents.
					int direction = random.Next(0, 1);
					double priceChange = Math.Round(random.NextDouble() * direction == 0 ? -0.05 : 0.05, 2);

					double[] priceHistory = new Double[4];
					priceHistory[0] = Convert.ToDouble(startingPrice) + (-priceChange * 2);
					priceHistory[1] = priceHistory[0] + priceChange;
					priceHistory[2] = priceHistory[1] + priceChange;
					priceHistory[3] = priceHistory[2] + priceChange;

					series = new Series[4];
					series[0] = GenerateSeries();
					this.MoveMarket(priceHistory[1]);
					series[1] = GenerateSeries();
					this.MoveMarket(priceHistory[2]);
					series[2] = GenerateSeries();
					this.MoveMarket(priceHistory[3]);
					series[3] = GenerateSeries();

					object[] categories = new object[steps];
					for (int step = 0; step < steps; step++)
						categories[step] = Convert.ToDouble(Math.Round(lowRange + step * stepSize, 2));

					double maxQuantity = 0.0;
					for (int seriesIndex = 0; seriesIndex < 4; seriesIndex++)
						for (int step = 0; step < steps; step++)
							if (Convert.ToDouble(series[seriesIndex].values[step]) > maxQuantity)
								maxQuantity = Convert.ToDouble(series[seriesIndex].values[step]);

					// Clear the contents of the chart workspace. This removes
					// any old charts that may already exist and leaves the chart workspace
					// completely empty. One chart object is then added.
					AxChartSpace chartSpace = this.axChartSpace;
					chartSpace.Clear();
					ChChart chartMain = chartSpace.Charts.Add(0);
					chartMain.Type = ChartChartTypeEnum.chChartTypeColumn3D;
					chartMain.PlotArea.BackWall.Interior.SetTextured(ChartPresetTextureEnum.chTextureWhiteMarble, ChartTextureFormatEnum.chTile, 0.0, ChartTexturePlacementEnum.chAllFaces);
					chartMain.PlotArea.SideWall.Interior.SetTextured(ChartPresetTextureEnum.chTextureWhiteMarble, ChartTextureFormatEnum.chTile, 0.0, ChartTexturePlacementEnum.chAllFaces);
					chartMain.PlotArea.Floor.Interior.SetTextured(ChartPresetTextureEnum.chTextureWhiteMarble, ChartTextureFormatEnum.chTile, 0.0, ChartTexturePlacementEnum.chAllFaces);
					chartMain.Overlap = 100;
					chartMain.ChartDepth = 50;
					chartMain.GapWidth = 50;
					chartMain.Perspective = 0;
					chartMain.ProjectionMode = ChartProjectionModeEnum.chProjectionModePerspective;
					chartMain.Inclination = 30.0;
					chartMain.DirectionalLightInclination = 10.0;
					chartMain.DirectionalLightRotation = 60.0;
					chartMain.DirectionalLightIntensity = 0.5;
					chartMain.Rotation = 330.0;
					chartMain.HasTitle = true;
					chartMain.Title.Caption = securityName;
					chartMain.Title.Font.Bold = true;
					chartMain.Title.Font.Name = "Ariel";
					chartMain.Title.Font.Size = 12;

					chartMain.Axes[0].Scaling.Minimum = 1.0;
					chartMain.Axes[0].Scaling.Maximum = 17.0;
					chartMain.Axes[1].Scaling.Minimum = 0.0;
					chartMain.Axes[1].Scaling.Maximum = maxQuantity;
					chartMain.Axes[1].NumberFormat = "#0.00";

					DateTime currentTime = DateTime.Now;

					// Bid Series
					ChSeries chSeries0 = chartMain.SeriesCollection.Add(0);
					chSeries0.Caption = (currentTime - new TimeSpan(0, 15, 0)).ToString("hh:mm");
					chSeries0.Interior.SetSolid(ChRGB.Get(Color.Transparent));
					chSeries0.Border.Color = ChRGB.Get(Color.Transparent);
					chSeries0.SetData(ChartDimensionsEnum.chDimCategories, (int)ChartSpecialDataSourcesEnum.chDataLiteral, categories);
					chSeries0.SetData(ChartDimensionsEnum.chDimValues, (int)ChartSpecialDataSourcesEnum.chDataLiteral, series[0].values);
					chSeries0.SetData(ChartDimensionsEnum.chDimFormatValues, (int)ChartSpecialDataSourcesEnum.chDataLiteral, series[0].format);
					AddBidAskSegment(chSeries0, 3);

					ChSeries chSeries1 = chartMain.SeriesCollection.Add(0);
					chSeries1.Caption = (currentTime - new TimeSpan(0, 10, 0)).ToString("hh:mm");
					chSeries1.Interior.SetSolid(ChRGB.Get(Color.Transparent));
					chSeries1.Border.Color = ChRGB.Get(Color.Transparent);
					chSeries1.SetData(ChartDimensionsEnum.chDimCategories, (int)ChartSpecialDataSourcesEnum.chDataLiteral, categories);
					chSeries1.SetData(ChartDimensionsEnum.chDimValues, (int)ChartSpecialDataSourcesEnum.chDataLiteral, series[1].values);
					chSeries1.SetData(ChartDimensionsEnum.chDimFormatValues, (int)ChartSpecialDataSourcesEnum.chDataLiteral, series[1].format);
					AddBidAskSegment(chSeries1, 2);

					ChSeries chSeries2 = chartMain.SeriesCollection.Add(0);
					chSeries2.Caption = (currentTime - new TimeSpan(0, 5, 0)).ToString("hh:mm");
					chSeries2.Interior.SetSolid(ChRGB.Get(Color.Transparent));
					chSeries2.Border.Color = ChRGB.Get(Color.Transparent);
					chSeries2.SetData(ChartDimensionsEnum.chDimCategories, (int)ChartSpecialDataSourcesEnum.chDataLiteral, categories);
					chSeries2.SetData(ChartDimensionsEnum.chDimValues, (int)ChartSpecialDataSourcesEnum.chDataLiteral, series[2].values);
					chSeries2.SetData(ChartDimensionsEnum.chDimFormatValues, (int)ChartSpecialDataSourcesEnum.chDataLiteral, series[2].format);
					AddBidAskSegment(chSeries2, 1);

					ChSeries chSeries3 = chartMain.SeriesCollection.Add(0);
					chSeries3.Caption = "Now";
					chSeries3.Interior.SetSolid(ChRGB.Get(Color.Transparent));
					chSeries3.Border.Color = ChRGB.Get(Color.Transparent);
					chSeries3.SetData(ChartDimensionsEnum.chDimCategories, (int)ChartSpecialDataSourcesEnum.chDataLiteral, categories);
					chSeries3.SetData(ChartDimensionsEnum.chDimValues, (int)ChartSpecialDataSourcesEnum.chDataLiteral, series[3].values);
					chSeries3.SetData(ChartDimensionsEnum.chDimFormatValues, (int)ChartSpecialDataSourcesEnum.chDataLiteral, series[3].format);
					AddBidAskSegment(chSeries3, 0);

					this.lastPrice = Convert.ToDouble(startingPrice);

				}
				catch (Exception exception)
				{

					Console.WriteLine(exception.Message);

				}

			}

		}

		private void InitializeMarket(double basePrice)
		{

			market = new Market();

			// Calculate the Bid series
			int bids = random.Next(10, 20);
			double bidPrice = basePrice - 0.01 - Math.Round(random.NextDouble() * 0.02, 2);
			for (int bid = 0; bid < bids; bid++)
			{
				int largeBlock = random.Next(0, 3);
				double quantity = Convert.ToDouble(random.Next(1, largeBlock == 0 ? 20 : 5) * 100);
				bidPrice -= 0.01 + Math.Round(random.NextDouble() * 0.02, 2);
				market.Bid.AddBidRow(0, Convert.ToDouble(bidPrice), quantity, DateTime.Now);
			}

			// Calculate the Ask series
			int asks = random.Next(10, 20);
			double askPrice = basePrice + 0.01 + Math.Round(random.NextDouble() * 0.02, 2);
			for (int ask = 0; ask < asks; ask++)
			{
				int largeBlock = random.Next(0, 3);
				double quantity = Convert.ToDouble(random.Next(1, largeBlock == 0 ? 20 : 5) * 100);
				askPrice += 0.01 + Math.Round(random.NextDouble() * 0.02, 2);
				market.Ask.AddAskRow(0, Convert.ToDouble(askPrice), quantity, DateTime.Now);
			}

			// Accept the initial market conditions.
			market.AcceptChanges();

		}

		private void MoveMarket(double targetPrice)
		{

			lock (this)
			{

				// Select four random bids to delete or reduce.
				for (int movement = 0; movement < 2; movement++)
				{
					int range = this.market.Bid.Rows.Count;
					int bidIndex = random.Next(0, range - 1);
					Market.BidRow bidRow = this.market.Bid[bidIndex];

					int deleteBidChance = random.Next(0, 3);
					if (deleteBidChance == 0)
					{

						bidRow = null;
						foreach (Market.BidRow bidLoop in market.Bid)
							if (bidRow == null || bidLoop.Price > bidRow.Price)
								bidRow = bidLoop;

						if (bidRow != null)
							bidRow.Delete();

					}
					else
					{

						bidRow = this.market.Bid[bidIndex];
						int largeBlock = random.Next(0, 3);
						double quantity = Convert.ToDouble(random.Next(1, largeBlock == 0 ? 20 : 5) * 100);
						bidRow.Quantity -= quantity;
						if (bidRow.Quantity <= 0)
							bidRow.Delete();

					}

					bidRow.AcceptChanges();

				}

				for (int movement = 0; movement < 2; movement++)
				{

					Market.AskRow askRow = null;

					int newAskChance = random.Next(0, 3);
					if (newAskChance == 0)
					{

						Market.AskRow lowestAsk = null;
						foreach (Market.AskRow lowestAskLoop in market.Ask)
							if (lowestAsk == null || lowestAskLoop.Price < lowestAsk.Price)
								lowestAsk = lowestAskLoop;

						if (lowestAsk != null)
						{

							int largeBlock = random.Next(0, 3);
							double askPrice = lowestAsk.Price - 0.01 - Math.Round(random.NextDouble() * 0.02, 2);
							double quantity = Convert.ToDouble(random.Next(1, largeBlock == 0 ? 20 : 5) * 100);
							askRow = market.Ask.NewAskRow();
							askRow.Price = Convert.ToDouble(askPrice);
							askRow.Quantity = quantity;
							askRow.Time = DateTime.Now;
							market.Ask.AddAskRow(askRow);

						}

					}
					else
					{

						int range = this.market.Ask.Rows.Count;
						int askIndex = random.Next(0, range - 1);
						askRow = this.market.Ask[askIndex];

						int largeBlock = random.Next(0, 3);
						double quantity = Convert.ToDouble(random.Next(1, largeBlock == 0 ? 20 : 5) * 100);
						askRow.Quantity += quantity;
					}

					askRow.AcceptChanges();
				}

			}

		}

		private Series GenerateSeries()
		{

			Series series = new Series(steps);
			for (int step = 0; step < steps; step++)
			{
				double stepPrice = lowRange + step * stepSize;
				series.values[step] = 0.0;
				series.format[step] = 1.0;
				foreach (Market.BidRow bidRow in market.Bid)
					if (bidRow.Price >= stepPrice)
					{
						series.format[step] = 0.0;
						series.values[step] = Convert.ToDouble(series.values[step]) + Convert.ToDouble(bidRow.Quantity);
					}

				foreach (Market.AskRow askRow in market.Ask)
					if (askRow.Price < stepPrice + stepSize)
					{
						series.format[step] = 3.0;
						series.values[step] = Convert.ToDouble(series.values[step]) + Convert.ToDouble(askRow.Quantity);
					}

				if (stepPrice <= this.lastPrice && this.lastPrice <= stepPrice + stepSize)
				{
					series.format[step] = 2.0;
					series.values[step] = lastQuantity;
				}

			}

			return series;

		}

		public void AddBidAskSegment(ChSeries chSeries, int level)
		{

			int depth = level == 0 ? 0 : 40 + (level * 24);
			Color colorBid = System.Drawing.Color.Lime;
			object RGBBid = (int)ChRGB.Get(colorBid) - (depth << 8);
			Color colorAsk = System.Drawing.Color.Red;
			object RGBAsk = (int)ChRGB.Get(colorAsk) - depth;

			ChSegment segmentAsk = chSeries.FormatMap.Segments.Add();
			segmentAsk.Begin.Border.Color = ChRGB.Get(Color.Black);
			segmentAsk.Begin.Interior.SetSolid(RGBBid);
			segmentAsk.Begin.Value = 0.0;
			segmentAsk.Begin.ValueType = ChartBoundaryValueTypeEnum.chBoundaryValueAbsolute;
			segmentAsk.End.Border.Color = ChRGB.Get(Color.Black);
			segmentAsk.End.Interior.SetSolid(RGBBid);
			segmentAsk.End.Value = 0.0;
			segmentAsk.End.ValueType = ChartBoundaryValueTypeEnum.chBoundaryValueAbsolute;

			ChSegment segmentLast = chSeries.FormatMap.Segments.Add();
			segmentLast.Begin.Border.Color = ChRGB.Get(Color.Black);
			segmentLast.Begin.Interior.SetSolid(ChRGB.Get(Color.Blue));
			segmentLast.Begin.Value = 2.0;
			segmentLast.Begin.ValueType = ChartBoundaryValueTypeEnum.chBoundaryValueAbsolute;
			segmentLast.End.Border.DashStyle = ChartLineDashStyleEnum.chLineLongDash;
			segmentLast.End.Border.Color = ChRGB.Get(Color.Black);
			segmentLast.End.Interior.SetSolid(ChRGB.Get(Color.Blue));
			segmentLast.End.Value = 2.0;
			segmentLast.End.ValueType = ChartBoundaryValueTypeEnum.chBoundaryValueAbsolute;

			ChSegment segmentBid = chSeries.FormatMap.Segments.Add();
			segmentBid.Begin.Border.Color = ChRGB.Get(Color.Black);
			segmentBid.Begin.Interior.SetSolid(RGBAsk);
			segmentBid.Begin.Value = 3.0;
			segmentBid.Begin.ValueType = ChartBoundaryValueTypeEnum.chBoundaryValueAbsolute;
			segmentBid.End.Border.DashStyle = ChartLineDashStyleEnum.chLineLongDash;
			segmentBid.End.Border.Color = ChRGB.Get(Color.Black);
			segmentBid.End.Interior.SetSolid(RGBAsk);
			segmentBid.End.Value = 3.0;
			segmentBid.End.ValueType = ChartBoundaryValueTypeEnum.chBoundaryValueAbsolute;

		}

		public void SimulateMarket()
		{

			try
			{

				AxChartSpace chartSpace = this.axChartSpace;

				// It is possible for this foreground thread to be called after the thread has been terminated.  This will prevent
				// the COM control from being accessed during a shutdown sequence.  Also, if there is nothing to display there's no
				// need to continue trying to paint the chart.
				if (!this.isTickerRunning || chartSpace.Charts.Count == 0)
					return;

				ChChart chartMain = chartSpace.Charts[0];

				int side = random.Next(0, 3);
				if (side == 0)
				{

					int range = this.market.Bid.Rows.Count;
					if (range == 0)
						return;

					int bidIndex = random.Next(0, range - 1);
					Market.BidRow bidRow = this.market.Bid[bidIndex];

					int down = random.Next(0, 2);
					if (down == 0)
					{

						int largeBlock = random.Next(0, 3);
						double quantity = Convert.ToDouble(random.Next(1, largeBlock == 0 ? 20 : 5) * 100);
						bidRow.Quantity -= quantity;
						if (bidRow.Quantity <= 0.0)
							bidRow.Quantity = 0.0;
					}
					else
					{

						int largeBlock = random.Next(0, 3);
						double quantity = Convert.ToDouble(random.Next(1, largeBlock == 0 ? 20 : 5) * 100);
						bidRow.Quantity += quantity;

					}

					bidRow.AcceptChanges();

				}
				else
				{
					if (side == 1)
					{

						int range = this.market.Ask.Rows.Count;
						if (range == 0)
							return;

						int askIndex = random.Next(0, range - 1);
						Market.AskRow askRow = this.market.Ask[askIndex];

						int down = random.Next(0, 2);
						if (down == 0)
						{

							int largeBlock = random.Next(0, 3);
							double quantity = Convert.ToDouble(random.Next(1, largeBlock == 0 ? 20 : 5) * 100);
							askRow.Quantity -= quantity;
							if (askRow.Quantity <= 0.0)
								askRow.Quantity = 0.0;
						}
						else
						{

							int largeBlock = random.Next(0, 3);
							double quantity = Convert.ToDouble(random.Next(1, largeBlock == 0 ? 20 : 5) * 100);
							askRow.Quantity += quantity;

						}

						askRow.AcceptChanges();
					}
					else
					{

						int largeBlock = random.Next(0, 3);
						int direction = random.Next(0, 3);
						double newPrice = this.lastPrice + (direction == 0 ? -0.01 : direction == 1 ? 0.0 : 0.01);

						Market.BidRow highestBid = null;
						foreach (Market.BidRow bidLoop in market.Bid)
							if (highestBid == null || bidLoop.Price > highestBid.Price)
								highestBid = bidLoop;

						Market.AskRow lowestAsk = null;
						foreach (Market.AskRow askLoop in market.Ask)
							if (lowestAsk == null || askLoop.Price < lowestAsk.Price)
								lowestAsk = askLoop;

						if (newPrice < highestBid.Price || lowestAsk.Price < newPrice)
							return;

						this.lastPrice = newPrice;
						this.lastQuantity = Convert.ToDouble(random.Next(1, largeBlock == 0 ? 20 : 5) * 100);

					}

				}

				Series series = GenerateSeries();
				ChSeries chSeries = chartMain.SeriesCollection[0];
				chSeries.SetData(ChartDimensionsEnum.chDimValues, (int)ChartSpecialDataSourcesEnum.chDataLiteral, series.values);
				chSeries.SetData(ChartDimensionsEnum.chDimFormatValues, (int)ChartSpecialDataSourcesEnum.chDataLiteral, series.format);

			}
			catch (Exception exception)
			{

				Console.WriteLine(exception.Message);

			}

		}

	}

}
