
using System;
using System.Collections.Generic;
using CoinbasePro.WebSocket.Models.Response;
using CoinbasePro.WebSocket.Types;
using CoinbasePro.Services.Orders.Types;

namespace GDAX
{
    class Window
    {
        public Window(string name, TimeSpan duration)
        {
            this.name = name;
            this.duration = duration;
            this.orders = new List<Ticker>();
        }

        public void Add(Ticker order)
        {
            foreach(Ticker element in orders.ToArray())
            {
                if(order.Time - element.Time > duration)
                {
                    orders.Remove(element);

                    if(element.Side == OrderSide.Buy)
                    {
                        volume -= element.LastSize;
                    }
                    else
                    {
                        volume += element.LastSize;
                    }
                }
                else
                {
                    break;
                }
            }

            orders.Add(order);
            if (order.Side == OrderSide.Buy)
            {
                volume += order.LastSize;
            }
            else
            {
                volume -= order.LastSize;
            }

            System.Console.WriteLine("{0} Volume : {1}", name, volume);
        }

        public decimal GetVolume()
        {
            return volume;
        }

        private string name;
        private TimeSpan duration;
        private List<Ticker> orders;
        private decimal volume;
    }
}
