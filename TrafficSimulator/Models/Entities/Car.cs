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
            Brush carBrush = isNight ? Brushes.Cyan : Brushes.Blue;
            var state = BeginOrientedDraw(g);
            g.FillRectangle(carBrush, 0, 0, Width, Height);
            EndOrientedDraw(g, state);
        }

        public override void Move(TrafficObjectCollection all)
        {
            EvaluateSurroundings(all);
            RoadLayout.Advance(this, ActualSpeed);
            if(ActualSpeed<DesiredSpeed)
            {
                AttemptLaneChange(all);
            }
        }
    }
}
