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
            Brush frameBrush = isNight ? Brushes.Magenta : Brushes.Purple;
            var state = BeginOrientedDraw(g);

            using (Brush wheelBrush = new SolidBrush(Color.FromArgb(28, 28, 28)))
            {
                float wheelR = Height * 0.9f;
                g.FillEllipse(wheelBrush, Width * 0.08f - wheelR / 2, Height / 2f - wheelR / 2, wheelR, wheelR);
                g.FillEllipse(wheelBrush, Width * 0.92f - wheelR / 2, Height / 2f - wheelR / 2, wheelR, wheelR);
            }

            using (Pen framePen = new Pen(frameBrush, 3))
            {
                g.DrawLine(framePen, Width * 0.08f, Height / 2f, Width * 0.92f, Height / 2f);
            }

            EndOrientedDraw(g, state);
        }

        public override void Move(TrafficObjectCollection all)
        {
            if (ShouldStopAtIntersection(all))
            {
                ActualSpeed = 0;
                return;
            }

            EvaluateSurroundings(all);
            RoadLayout.Advance(this, ActualSpeed);

            if (ActualSpeed < DesiredSpeed)
            {
                AttemptLaneChange(all);
            }
        }
    }
}