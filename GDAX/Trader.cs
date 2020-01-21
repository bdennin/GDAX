
using CoinbasePro;
using CoinbasePro.Network.Authentication;
using CoinbasePro.Shared.Types;
using CoinbasePro.WebSocket.Models.Response;
using CoinbasePro.WebSocket.Types;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

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
            file = new StreamWriter(path);
            
            tickers = new List<Ticker>();
            client = new CoinbaseProClient();

            currencies = new List<ProductType>();
            currencies.Add(ProductType.BtcUsd);

            channels = new List<ChannelType>();

            client.WebSocket.Start(currencies, channels, 12000);

            isRunning = true;
            thread = new Thread(new ThreadStart(ParseThread));

            //client.WebSocket.OnHeartbeatReceived += WebSocket_OnHeartbeatReceived;
            client.WebSocket.OnTickerReceived += WebSocket_OnTickerReceived;
        }

        //private static void WebSocket_OnHeartbeatReceived(object sender, WebfeedEventArgs<Heartbeat> args)
        //{
        //    Heartbeat heartbeat = args.LastOrder;
        //    //System.Console.WriteLine("{0} | {1} | {2} | {3} |\n", heartbeat.ProductId, heartbeat.LastTradeId, heartbeat.Sequence, heartbeat.Time);
        //}

        private static void WebSocket_OnTickerReceived(object sender, WebfeedEventArgs<Ticker> args)
        {
            Ticker ticker = args.LastOrder;

            tickers.Add(ticker);

            file.WriteLine("{0} {1} {2} {3} {4} {5} {6}",
                ticker.Time,
                ticker.Sequence,
                ticker.ProductId,
                ticker.Price,
                ticker.LastSize, 
                ticker.Side, 
                ticker.Type);

            Console.WriteLine("{0} {1} {2} {3} {4} {5} {6}", 
                ticker.Time,
                ticker.Sequence,
                ticker.ProductId,
                ticker.Price,
                ticker.LastSize,
                ticker.Side,
                ticker.Type);
        }

        private static void ParseThread()
        { 
            while(isRunning)
            {
                Thread.Sleep(100);

                DateTime dateTime = DateTime.Now;
                
                mutex.WaitOne();
                foreach(Ticker ticker in tickers)
                {

                }
                mutex.ReleaseMutex();
            }
        }

        private static Mutex mutex;
        private static List<Ticker> tickers;
        private static string path;
        private static StreamWriter file;
        private static Thread thread;
        private static bool isRunning;

        private CoinbaseProClient client;
        private List<ProductType> currencies;
        private List<ChannelType> channels;
    }
}
