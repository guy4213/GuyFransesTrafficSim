using System;
using System.Drawing;

namespace TrafficSimulator
{
    [Serializable]
    public class Car : RoadUser
    {
        public CarModel Model;

        public Car(int x, int y, int lane, Direction dir, CarModel model, float desiredSpeed = 90f)
            : base(x, y, lane, dir, desiredSpeed)
        {
            Model = model;
            Width = 50;  
            Height = 25;
        }

        public override void Draw(Graphics g, bool isNight)
        {
            Color bodyColor = isNight ? Color.FromArgb(120, 220, 235) : Color.FromArgb(45, 100, 220);
            var state = BeginOrientedDraw(g);

            DrawWheels(g, Width, Height);

            using (Brush body = new SolidBrush(bodyColor))
            using (var path = RoundedRect(0, 0, Width, Height, 6))
            {
                g.FillPath(body, path);
                g.DrawPath(Pens.Black, path);
            }

            using (Brush windshield = new SolidBrush(Color.FromArgb(110, 20, 25, 30)))
            {
                g.FillRectangle(windshield, Width * 0.58f, Height * 0.15f, Width * 0.28f, Height * 0.7f);
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
            if(ActualSpeed<DesiredSpeed)
            {
                AttemptLaneChange(all);
            }
        }
    }
}
