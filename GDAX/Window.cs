
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
            this.duration = duration.Ticks;
            this.orders = new List<Ticker>();
        }

        public void Add(Ticker order)
        {
            DateTime now = DateTime.UtcNow;

            foreach (Ticker element in orders.ToArray())
            {
                Int64 diff = now.Ticks - element.Time.Ticks;

                if(diff > duration)
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
        }

        public string GetName()
        {
            return name;
        }
        
        public decimal GetVolume()
        {
            return volume;
        }

        private string name;
        private Int64 duration;
        private List<Ticker> orders;
        private decimal volume;
    }
}
