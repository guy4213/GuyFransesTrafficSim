using System;
using System.Drawing;

namespace TrafficSimulator
{
    [Serializable]
    public class Bicycle : RoadUser
    {
        public Bicycle(int x, int y, int lane, Direction dir, float desiredSpeed = 40f)
            : base(x, y, lane, dir, desiredSpeed)
        {
            Width = 30;
            Height = 15;
        }

        public override void Draw(Graphics g, bool isNight)
        {
            Brush bikeBrush = isNight ? Brushes.Magenta : Brushes.Purple;
            var state = BeginOrientedDraw(g);
            g.FillRectangle(bikeBrush, 0, 0, Width, Height);
            g.DrawRectangle(Pens.Black, 0, 0, Width, Height);

            using (Pen pen = new Pen(Color.White, 2))
            {
                g.DrawLine(pen, 5, Height / 2, Width - 5, Height / 2);
            }
            EndOrientedDraw(g, state);
        }

        public override void Move(TrafficObjectCollection all)
        {
            EvaluateSurroundings(all);
            RoadLayout.Advance(this, ActualSpeed);

            if (ActualSpeed < DesiredSpeed)
            {
                AttemptLaneChange(all);
            }
        }
    }
}