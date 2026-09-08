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

            // Waiting riders stand beside the shelter so the queue is visible,
            // not just a number - it shrinks as a bus boards them. Drawn the
            // same way as a walking Pedestrian (green circle) for consistency.
            const int maxShown = 6;
            const int dotSize = 10;
            int shown = Math.Min(WaitingPassengers, maxShown);
            Brush pedestrianBrush = isNight ? Brushes.LightGreen : Brushes.Green;
            for (int i = 0; i < shown; i++)
            {
                int col = i % 3;
                int row = i / 3;
                int px = X + 2 + col * 13;
                int py = Y + Height + 3 + row * 13;
                g.FillEllipse(pedestrianBrush, px, py, dotSize, dotSize);
                g.DrawEllipse(Pens.Black, px, py, dotSize, dotSize);
            }

            // הצגת כמות הנוסעים המחכים מעל התחנה
            using (Font font = new Font("Arial", 8, FontStyle.Bold))
            {
                Brush textBrush = isNight ? Brushes.White : Brushes.Black;
                string label = WaitingPassengers > maxShown ? $"Stop (+{WaitingPassengers})" : $"Stop ({WaitingPassengers})";
                g.DrawString(label, font, textBrush, X, Y - 14);
            }
        }
    }
}