using System;
using System.Drawing;

namespace TrafficSimulator
{
    [Serializable]
    public class BusStation : TrafficObject
    {
        public StationType Type { get; set; }
        public int WaitingPassengers { get; set; }

        public BusStation(int x, int y, int lane, Direction dir, StationType type = StationType.Regular, int waitingPassengers = 5)
            : base(x, y, lane, dir, 0)
        {
            Type = type;
            WaitingPassengers = waitingPassengers;
            Width = 40;
            Height = 30;
        }

        public override void Move(TrafficObjectCollection all)
        {
            
        }

        public override void Draw(Graphics g, bool isNight)
        {
            // צבע התחנה לפי סוגה
            Brush stationBrush = Type switch
            {
                StationType.Central => Brushes.DarkOrange,
                StationType.Express => Brushes.Purple,
                _ => isNight ? Brushes.Gold : Brushes.Yellow
            };

            // ציור התחנה
            g.FillRectangle(stationBrush, X, Y, Width, Height);
            g.DrawRectangle(Pens.Black, X, Y, Width, Height);

            // הצגת כמות הנוסעים המחכים מעל התחנה
            using (Font font = new Font("Arial", 8, FontStyle.Bold))
            {
                Brush textBrush = isNight ? Brushes.White : Brushes.Black;
                g.DrawString($"Stop ({WaitingPassengers})", font, textBrush, X, Y - 14);
            }
        }
    }
}