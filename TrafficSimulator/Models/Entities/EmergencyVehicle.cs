using System;
using System.Drawing;

namespace TrafficSimulator
{
    [Serializable]
    public class EmergencyVehicle : RoadUser
    {
        public bool SirenOn { get; set; }

        public EmergencyVehicle(int x, int y, int lane, Direction dir, float desiredSpeed = 130f)
            : base(x, y, lane, dir, desiredSpeed)
        {
            Width = 55;
            Height = 25;
            SirenOn = false;
        }

        public void TriggerYieldOnNearbyVehicles(TrafficObjectCollection all)
        {
            bool hasVehicleAhead = false;
            var objectsAhead = all.GetObjectsInLane(Lane);

            foreach (var obj in objectsAhead)
            {
                if (obj is RoadUser vehicle && vehicle != this)
                {
                    int diff = vehicle.X - X;
                    if (diff > 0 && diff < 150)
                    {
                        hasVehicleAhead = true;
                        vehicle.AttemptLaneChange(all);
                    }
                }
            }

            SirenOn = hasVehicleAhead;
        }

        public override void Draw(Graphics g, bool isNight)
        {
            Brush carBrush = isNight ? Brushes.Red : Brushes.DarkRed;
            g.FillRectangle(carBrush, X, Y, Width, Height);
            g.DrawRectangle(Pens.Black, X, Y, Width, Height);

            if (SirenOn)
            {
                Brush sirenBrush = (X / 10) % 2 == 0 ? Brushes.Red : Brushes.Blue;
                g.FillRectangle(sirenBrush, X + (Width / 2) - 5, Y + (Height / 2) - 5, 10, 10);
            }
        }

        public override void Move(TrafficObjectCollection all)
        {
            TriggerYieldOnNearbyVehicles(all);
            EvaluateSurroundings(all);
            X += (int)ActualSpeed;

            if (ActualSpeed < DesiredSpeed)
            {
                AttemptLaneChange(all);
            }
        }
    }
}