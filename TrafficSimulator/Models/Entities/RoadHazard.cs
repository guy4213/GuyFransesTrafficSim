using System;
using System.Drawing;

namespace TrafficSimulator
{
    [Serializable]
    public class RoadHazard : TrafficObject
    {
        public RoadHazard(int x, int y, int lane, Direction dir)
            : base(x, y, lane, dir, 0)
        {
            Width = 30;
            Height = 30;
        }

        public override void Move(TrafficObjectCollection all)
        {
        }

        public override void Draw(Graphics g, bool isNight)
        {
            Brush hazardBrush = isNight ? Brushes.OrangeRed : Brushes.Orange;
            Point p1 = new Point(X + Width / 2, Y);
            Point p2 = new Point(X, Y + Height);
            Point p3 = new Point(X + Width, Y + Height);

            g.FillPolygon(hazardBrush, new Point[] { p1, p2, p3 });
            g.DrawPolygon(Pens.Black, new Point[] { p1, p2, p3 });
        }
    }
}