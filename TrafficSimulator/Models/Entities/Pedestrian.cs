using System;
using System.Drawing;

namespace TrafficSimulator
{
    [Serializable]
    public class Pedestrian : RoadUser
    {
        public bool IsCrossing { get; set; }

        private readonly Direction _conflictA;
        private readonly Direction _conflictB;

        // 'road' is the approach whose crosswalk this pedestrian uses (Right/Down/Left/Up
        // = West/North/East/South) - the pedestrian actually walks perpendicular to it.
        public Pedestrian(int x, int y, int lane, Direction road, float desiredSpeed = 15f)
            : base(x, y, lane, RoadLayout.IsHorizontal(road) ? Direction.Down : Direction.Right, desiredSpeed)
        {
            Width = 14;
            Height = 14;
            IsCrossing = false;

            if (RoadLayout.IsHorizontal(road))
            {
                _conflictA = Direction.Right;
                _conflictB = Direction.Left;
            }
            else
            {
                _conflictA = Direction.Down;
                _conflictB = Direction.Up;
            }
        }

        public override void Draw(Graphics g, bool isNight)
        {
            Brush pedBrush = isNight ? Brushes.LightGreen : Brushes.Green;
            g.FillEllipse(pedBrush, X, Y, Width, Height);
            g.DrawEllipse(Pens.Black, X, Y, Width, Height);
        }

        public override void Move(TrafficObjectCollection all)
        {
            bool safeToCross = all.ActiveGreenDirection != _conflictA && all.ActiveGreenDirection != _conflictB;

            if (!safeToCross || all.HasActiveEmergency)
            {
                ActualSpeed = 0;
                IsCrossing = false;
                return;
            }

            IsCrossing = true;
            RoadLayout.Advance(this, ActualSpeed);
        }
    }
}
