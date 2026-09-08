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
            // not just a number - it shrinks as a bus boards them.
            const int maxShown = 6;
            int shown = Math.Min(WaitingPassengers, maxShown);
            using (Brush shirtBrush = new SolidBrush(Color.FromArgb(60, 90, 160)))
            using (Brush headBrush = new SolidBrush(Color.FromArgb(240, 200, 160)))
            {
                for (int i = 0; i < shown; i++)
                {
                    int col = i % 3;
                    int row = i / 3;
                    int px = X + 2 + col * 12;
                    int py = Y + Height + 3 + row * 13;
                    g.FillRectangle(shirtBrush, px, py + 4, 7, 8);
                    g.FillEllipse(headBrush, px + 1, py, 5, 5);
                }
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