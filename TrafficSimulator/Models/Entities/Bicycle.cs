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
            g.FillRectangle(bikeBrush, X, Y, Width, Height);
            g.DrawRectangle(Pens.Black, X, Y, Width, Height);

            using (Pen pen = new Pen(Color.White, 2))
            {
                g.DrawLine(pen, X + 5, Y + Height / 2, X + Width - 5, Y + Height / 2);
            }
        }

        public override void Move(TrafficObjectCollection all)
        {
            EvaluateSurroundings(all);
            X += (int)ActualSpeed;

            if (ActualSpeed < DesiredSpeed)
            {
                AttemptLaneChange(all);
            }
        }
    }
}