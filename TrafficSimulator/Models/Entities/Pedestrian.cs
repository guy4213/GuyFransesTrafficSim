using System;
using System.Drawing;

namespace TrafficSimulator
{
    [Serializable]
    public class Pedestrian : RoadUser
    {
        public bool IsCrossing { get; set; }

        public Pedestrian(int x, int y, int lane, Direction dir, float desiredSpeed = 15f)
            : base(x, y, lane, dir, desiredSpeed)
        {
            Width = 15;
            Height = 15;
            IsCrossing = false;
        }

        public override void Draw(Graphics g, bool isNight)
        {
            Brush pedBrush = isNight ? Brushes.LightGreen : Brushes.Green;
            g.FillEllipse(pedBrush, X, Y, Width, Height);
            g.DrawEllipse(Pens.Black, X, Y, Width, Height);
        }

        public override void Move(TrafficObjectCollection all)
        {
            EvaluateSurroundings(all);
            RoadLayout.Advance(this, ActualSpeed);
        }
    }
}