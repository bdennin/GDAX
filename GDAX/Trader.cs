
using CoinbasePro;
using CoinbasePro.Network.Authentication;
using CoinbasePro.Shared.Types;
using CoinbasePro.WebSocket.Models.Response;
using CoinbasePro.WebSocket.Types;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using CoinbasePro.Services.Orders.Types;

namespace GDAX
{
	class Trade
	{
		int id;
		DateTime time;

	}

	class Trader
	{
		public Trader()
		{
			path = System.IO.Directory.GetCurrentDirectory() + "\\database.db";
			
			tickers = new List<Ticker>();
			client = new CoinbaseProClient();

			currencies = new List<ProductType>();
			currencies.Add(ProductType.BtcUsd);

			channels = new List<ChannelType>();

			anchorPoint = DateTime.UtcNow;
			
			windows = new List<Window>();
			windows.Add(new Window("100ms", new TimeSpan(100 * TimeSpan.TicksPerMillisecond)));
			windows.Add(new Window("500ms", new TimeSpan(500 * TimeSpan.TicksPerMillisecond)));
			windows.Add(new Window("1s", new TimeSpan(0, 0, 1)));
			windows.Add(new Window("2s", new TimeSpan(0, 0, 2)));
			windows.Add(new Window("5s", new TimeSpan(0, 0, 5)));
			windows.Add(new Window("10s", new TimeSpan(0, 0, 10)));
			windows.Add(new Window("20s", new TimeSpan(0, 0, 20)));
			windows.Add(new Window("30s", new TimeSpan(0, 0, 30)));
			windows.Add(new Window("1m", new TimeSpan(0, 1, 0)));
			windows.Add(new Window("2m", new TimeSpan(0, 2, 0)));
			windows.Add(new Window("5m", new TimeSpan(0, 5, 0)));
			windows.Add(new Window("10m", new TimeSpan(0, 10, 0)));
			windows.Add(new Window("20m", new TimeSpan(0, 20, 0)));
			windows.Add(new Window("30m", new TimeSpan(0, 30, 0)));
			windows.Add(new Window("1h", new TimeSpan(1, 0, 0)));
			windows.Add(new Window("2h", new TimeSpan(2, 0, 0)));
			windows.Add(new Window("3h", new TimeSpan(3, 0, 0)));
			windows.Add(new Window("6h", new TimeSpan(6, 0, 0)));
			windows.Add(new Window("12h", new TimeSpan(12, 0, 0)));
			windows.Add(new Window("1d", new TimeSpan(24, 0, 0)));

			ReadPoints();

			//client.WebSocket.OnHeartbeatReceived += WebSocket_OnHeartbeatReceived;
			client.WebSocket.OnTickerReceived += WebSocket_OnTickerReceived;

			isRunning = true;
			thread = new Thread(new ThreadStart(ExecThread));
			thread.Start();

			client.WebSocket.Start(currencies, channels, 12000);
		}

		//private static void WebSocket_OnHeartbeatReceived(object sender, WebfeedEventArgs<Heartbeat> args)
		//{
		//    Heartbeat heartbeat = args.LastOrder;
		//    //System.Console.WriteLine("{0} | {1} | {2} | {3} |\n", heartbeat.ProductId, heartbeat.LastTradeId, heartbeat.Sequence, heartbeat.Time);
		//}

		private void WebSocket_OnTickerReceived(object sender, WebfeedEventArgs<Ticker> args)
		{
			Ticker ticker = args.LastOrder;

			AddPoint(ticker);
			WritePoint(ticker);
		}

		private void AddPoint(Ticker ticker)
		{
			foreach (Window window in windows)
			{
				window.Add(ticker);
			}
		}

		private void WritePoint(Ticker ticker)
		{
			StreamWriter writer = new StreamWriter(path, true);

			writer.WriteLine("{0}|{1}|{2}|{3}|{4}|{5}|{6}",
				ticker.Time,
				ticker.Sequence,
				ticker.ProductId,
				ticker.Price,
				ticker.LastSize,
				ticker.Side,
				ticker.Type);

			writer.Flush();
			writer.Close();
		}

		private void ReadPoints()
		{
			Ticker ticker = new Ticker();
			StreamReader reader = null;
			try
			{
				reader = new StreamReader(path);
				while (!reader.EndOfStream)
				{
					string[] args = reader.ReadLine().Split('|');

					if (args.Length >= 7)
					{
						ticker.Time = Convert.ToDateTime(args[0]);
						ticker.Sequence = Convert.ToInt64(args[1]);
						ticker.ProductId = (ProductType)Enum.Parse(typeof(ProductType), args[2]);
						ticker.Price = Convert.ToDecimal(args[3]);
						ticker.LastSize = Convert.ToDecimal(args[4]);
						ticker.Side = (OrderSide)Enum.Parse(typeof(OrderSide), args[5]);
						ticker.Type = (ResponseType)Enum.Parse(typeof(ResponseType), args[6]);

						AddPoint(ticker);
					}
					else
					{
						Console.WriteLine("Invalid argument while reading database file.");
					}
				}

				reader.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception caught while attempting to read the database.");
				if(reader != null)
				{
					reader.Close();
				}
			}
		}

		private void ExecThread()
		{
			while (isRunning)
			{
				Thread.Sleep(1000);
				string execData = string.Empty;

				foreach (Window window in windows)
				{
					execData += window.GetName() + ": " + window.GetVolume() + "\n";
				}

				Console.Write(execData);
			}
		}

		private List<Ticker> tickers;
		private List<Window> windows;
		private bool isRunning;


		private CoinbaseProClient client;
		private Thread thread;
		private DateTime anchorPoint;
		private string path;
		private List<ProductType> currencies;
		private List<ChannelType> channels;
	}
}
