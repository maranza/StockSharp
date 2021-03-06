#region S# License
/******************************************************************************************
NOTICE!!!  This program and source code is owned and licensed by
StockSharp, LLC, www.stocksharp.com
Viewing or use of this code requires your acceptance of the license
agreement found at https://github.com/StockSharp/StockSharp/blob/master/LICENSE
Removal of this comment is a violation of the license agreement.

Project: StockSharp.MatLab.MatLab
File: MatLabConnector.cs
Created: 2015, 11, 11, 2:32 PM

Copyright 2010 by StockSharp, LLC
*******************************************************************************************/
#endregion S# License
namespace StockSharp.MatLab
{
	using System;
	using System.Collections.Generic;

	using Ecng.Common;

	using StockSharp.BusinessEntities;

	/// <summary>
	/// The interface <see cref="IConnector"/> implementation which provides ability to use from MatLab scripts.
	/// </summary>
	public class MatLabConnector : Disposable
	{
		private readonly bool _ownTrader;

		/// <summary>
		/// Initializes a new instance of the <see cref="MatLabConnector"/>.
		/// </summary>
		/// <param name="realConnector">The connection for market-data and transactions.</param>
		public MatLabConnector(IConnector realConnector)
			: this(realConnector, true)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MatLabConnector"/>.
		/// </summary>
		/// <param name="realConnector">The connection for market-data and transactions.</param>
		/// <param name="ownTrader">Track the connection <paramref name="realConnector" /> lifetime.</param>
		public MatLabConnector(IConnector realConnector, bool ownTrader)
		{
			if (realConnector == null)
				throw new ArgumentNullException(nameof(realConnector));

			RealConnector = realConnector;

			RealConnector.Connected += RealTraderOnConnected;
			RealConnector.ConnectionError += RealTraderOnConnectionError;
			RealConnector.Disconnected += RealTraderOnDisconnected;
			RealConnector.Error += RealTraderOnError;
			RealConnector.MarketTimeChanged += RealTraderOnMarketTimeChanged;
			RealConnector.NewSecurities += RealTraderOnNewSecurities;
			RealConnector.SecuritiesChanged += RealTraderOnSecuritiesChanged;
			RealConnector.NewPortfolios += RealTraderOnNewPortfolios;
			RealConnector.PortfoliosChanged += RealTraderOnPortfoliosChanged;
			RealConnector.NewPositions += RealTraderOnNewPositions;
			RealConnector.PositionsChanged += RealTraderOnPositionsChanged;
			RealConnector.NewTrades += RealTraderOnNewTrades;
			RealConnector.NewMyTrades += RealTraderOnNewMyTrades;
			RealConnector.NewMarketDepths += RealTraderOnNewMarketDepths;
			RealConnector.MarketDepthsChanged += RealTraderOnMarketDepthsChanged;
			RealConnector.NewOrders += RealTraderOnNewOrders;
			RealConnector.OrdersChanged += RealTraderOnOrdersChanged;
			RealConnector.OrdersRegisterFailed += RealTraderOnOrdersRegisterFailed;
			RealConnector.OrdersCancelFailed += RealTraderOnOrdersCancelFailed;
			RealConnector.NewStopOrders += RealTraderOnNewStopOrders;
			RealConnector.StopOrdersChanged += RealTraderOnStopOrdersChanged;
			RealConnector.StopOrdersRegisterFailed += RealTraderOnStopOrdersRegisterFailed;
			RealConnector.StopOrdersCancelFailed += RealTraderOnStopOrdersCancelFailed;
			RealConnector.NewOrderLogItems += RealTraderOnNewOrderLogItems;

			_ownTrader = ownTrader;
		}

		/// <summary>
		/// The connection for market-data and transactions.
		/// </summary>
		public IConnector RealConnector { get; }

		/// <summary>
		/// Connected.
		/// </summary>
		public event EventHandler Connected;

		/// <summary>
		/// Connection error (for example, the connection was aborted by server).
		/// </summary>
		public event EventHandler<ErrorEventArgs> ConnectionError;

		/// <summary>
		/// Disconnected.
		/// </summary>
		public event EventHandler Disconnected;

		/// <summary>
		/// Dats process error.
		/// </summary>
		public event EventHandler<ErrorEventArgs> Error;

		/// <summary>
		/// Server time changed <see cref="IConnector.ExchangeBoards"/>. It passed the time difference since the last call of the event. The first time the event passes the value <see cref="TimeSpan.Zero"/>.
		/// </summary>
		public event EventHandler MarketTimeChanged;

		/// <summary>
		/// Securities received.
		/// </summary>
		public event EventHandler<SecuritiesEventArgs> NewSecurities;

		/// <summary>
		/// Securities changed.
		/// </summary>
		public event EventHandler<SecuritiesEventArgs> SecuritiesChanged;

		/// <summary>
		/// Portfolios received.
		/// </summary>
		public event EventHandler<PortfoliosEventArgs> NewPortfolios;

		/// <summary>
		/// Portfolios changed.
		/// </summary>
		public event EventHandler<PortfoliosEventArgs> PortfoliosChanged;

		/// <summary>
		/// Positions received.
		/// </summary>
		public event EventHandler<PositionsEventArgs> NewPositions;

		/// <summary>
		/// Positions changed.
		/// </summary>
		public event EventHandler<PositionsEventArgs> PositionsChanged;

		/// <summary>
		/// Tick trades received.
		/// </summary>
		public event EventHandler<TradesEventArgs> NewTrades;

		/// <summary>
		/// Own trades received.
		/// </summary>
		public event EventHandler<MyTradesEventArgs> NewMyTrades;

		/// <summary>
		/// Orders received.
		/// </summary>
		public event EventHandler<OrdersEventArgs> NewOrders;

		/// <summary>
		/// Orders changed (cancelled, matched).
		/// </summary>
		public event EventHandler<OrdersEventArgs> OrdersChanged;

		/// <summary>
		/// Order registration errors event.
		/// </summary>
		public event EventHandler<OrderFailsEventArgs> OrdersRegisterFailed;

		/// <summary>
		/// Order cancellation errors event.
		/// </summary>
		public event EventHandler<OrderFailsEventArgs> OrdersCancelFailed;

		/// <summary>
		/// Stop-orders received.
		/// </summary>
		public event EventHandler<OrdersEventArgs> NewStopOrders;

		/// <summary>
		/// Stop orders state change event .
		/// </summary>
		public event EventHandler<OrdersEventArgs> StopOrdersChanged;

		/// <summary>
		/// Stop-order registration errors event.
		/// </summary>
		public event EventHandler<OrderFailsEventArgs> StopOrdersRegisterFailed;

		/// <summary>
		/// Stop-order cancellation errors event.
		/// </summary>
		public event EventHandler<OrderFailsEventArgs> StopOrdersCancelFailed;

		/// <summary>
		/// Order books received.
		/// </summary>
		public event EventHandler<MarketDepthsEventArgs> NewMarketDepths;

		/// <summary>
		/// Order books changed.
		/// </summary>
		public event EventHandler<MarketDepthsEventArgs> MarketDepthsChanged;

		/// <summary>
		/// Order log received.
		/// </summary>
		public event EventHandler<OrderLogItemsEventArg> NewOrderLogItems;

		private void RealTraderOnNewMyTrades(IEnumerable<MyTrade> trades)
		{
			NewMyTrades?.Invoke(this, new MyTradesEventArgs(trades));
		}

		private void RealTraderOnError(Exception exception)
		{
			Error?.Invoke(this, new ErrorEventArgs(exception));
		}

		private void RealTraderOnStopOrdersCancelFailed(IEnumerable<OrderFail> fails)
		{
			StopOrdersCancelFailed?.Invoke(this, new OrderFailsEventArgs(fails));
		}

		private void RealTraderOnStopOrdersRegisterFailed(IEnumerable<OrderFail> fails)
		{
			StopOrdersRegisterFailed?.Invoke(this, new OrderFailsEventArgs(fails));
		}

		private void RealTraderOnStopOrdersChanged(IEnumerable<Order> orders)
		{
			StopOrdersChanged?.Invoke(this, new OrdersEventArgs(orders));
		}

		private void RealTraderOnNewStopOrders(IEnumerable<Order> orders)
		{
			NewStopOrders?.Invoke(this, new OrdersEventArgs(orders));
		}

		private void RealTraderOnOrdersCancelFailed(IEnumerable<OrderFail> fails)
		{
			OrdersCancelFailed?.Invoke(this, new OrderFailsEventArgs(fails));
		}

		private void RealTraderOnOrdersRegisterFailed(IEnumerable<OrderFail> fails)
		{
			OrdersRegisterFailed?.Invoke(this, new OrderFailsEventArgs(fails));
		}

		private void RealTraderOnOrdersChanged(IEnumerable<Order> orders)
		{
			OrdersChanged?.Invoke(this, new OrdersEventArgs(orders));
		}

		private void RealTraderOnNewOrders(IEnumerable<Order> orders)
		{
			NewOrders?.Invoke(this, new OrdersEventArgs(orders));
		}

		private void RealTraderOnNewMarketDepths(IEnumerable<MarketDepth> depths)
		{
			NewMarketDepths?.Invoke(this, new MarketDepthsEventArgs(depths));
		}

		private void RealTraderOnMarketDepthsChanged(IEnumerable<MarketDepth> depths)
		{
			MarketDepthsChanged?.Invoke(this, new MarketDepthsEventArgs(depths));
		}

		private void RealTraderOnNewTrades(IEnumerable<Trade> trades)
		{
			NewTrades?.Invoke(this, new TradesEventArgs(trades));
		}

		private void RealTraderOnPositionsChanged(IEnumerable<Position> positions)
		{
			PositionsChanged?.Invoke(this, new PositionsEventArgs(positions));
		}

		private void RealTraderOnNewPositions(IEnumerable<Position> positions)
		{
			NewPositions?.Invoke(this, new PositionsEventArgs(positions));
		}

		private void RealTraderOnPortfoliosChanged(IEnumerable<Portfolio> portfolios)
		{
			PortfoliosChanged?.Invoke(this, new PortfoliosEventArgs(portfolios));
		}

		private void RealTraderOnNewPortfolios(IEnumerable<Portfolio> portfolios)
		{
			NewPortfolios?.Invoke(this, new PortfoliosEventArgs(portfolios));
		}

		private void RealTraderOnSecuritiesChanged(IEnumerable<Security> securities)
		{
			SecuritiesChanged?.Invoke(this, new SecuritiesEventArgs(securities));
		}

		private void RealTraderOnNewSecurities(IEnumerable<Security> securities)
		{
			NewSecurities?.Invoke(this, new SecuritiesEventArgs(securities));
		}

		private void RealTraderOnNewOrderLogItems(IEnumerable<OrderLogItem> items)
		{
			NewOrderLogItems?.Invoke(this, new OrderLogItemsEventArg(items));
		}

		private void RealTraderOnMarketTimeChanged(TimeSpan diff)
		{
			MarketTimeChanged?.Invoke(this, EventArgs.Empty);
		}

		private void RealTraderOnDisconnected()
		{
			Disconnected?.Invoke(this, EventArgs.Empty);
		}

		private void RealTraderOnConnectionError(Exception exception)
		{
			ConnectionError?.Invoke(this, new ErrorEventArgs(exception));
		}

		private void RealTraderOnConnected()
		{
			Connected?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Release resources.
		/// </summary>
		protected override void DisposeManaged()
		{
			RealConnector.Connected -= RealTraderOnConnected;
			RealConnector.ConnectionError -= RealTraderOnConnectionError;
			RealConnector.Disconnected -= RealTraderOnDisconnected;
			RealConnector.Error -= RealTraderOnError;
			RealConnector.MarketTimeChanged -= RealTraderOnMarketTimeChanged;
			RealConnector.NewSecurities -= RealTraderOnNewSecurities;
			RealConnector.SecuritiesChanged -= RealTraderOnSecuritiesChanged;
			RealConnector.NewPortfolios -= RealTraderOnNewPortfolios;
			RealConnector.PortfoliosChanged -= RealTraderOnPortfoliosChanged;
			RealConnector.NewPositions -= RealTraderOnNewPositions;
			RealConnector.PositionsChanged -= RealTraderOnPositionsChanged;
			RealConnector.NewTrades -= RealTraderOnNewTrades;
			RealConnector.NewMyTrades -= RealTraderOnNewMyTrades;
			RealConnector.MarketDepthsChanged -= RealTraderOnMarketDepthsChanged;
			RealConnector.NewOrders -= RealTraderOnNewOrders;
			RealConnector.OrdersChanged -= RealTraderOnOrdersChanged;
			RealConnector.OrdersRegisterFailed -= RealTraderOnOrdersRegisterFailed;
			RealConnector.OrdersCancelFailed -= RealTraderOnOrdersCancelFailed;
			RealConnector.NewStopOrders -= RealTraderOnNewStopOrders;
			RealConnector.StopOrdersChanged -= RealTraderOnStopOrdersChanged;
			RealConnector.StopOrdersRegisterFailed -= RealTraderOnStopOrdersRegisterFailed;
			RealConnector.StopOrdersCancelFailed -= RealTraderOnStopOrdersCancelFailed;

			if (_ownTrader)
				RealConnector.Dispose();

			base.DisposeManaged();
		}
	}
}