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
            var objectsAhead = all.GetObjectsInLane(Direction, Lane);

            foreach (var obj in objectsAhead)
            {
                if (obj is RoadUser vehicle && vehicle != this)
                {
                    int diff = RoadLayout.ForwardDistance(this, vehicle);
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
            Color bodyColor = isNight ? Color.FromArgb(255, 90, 80) : Color.FromArgb(178, 30, 24);
            var state = BeginOrientedDraw(g);

            DrawWheels(g, Width, Height);

            using (Brush body = new SolidBrush(bodyColor))
            using (var path = RoundedRect(0, 0, Width, Height, 6))
            {
                g.FillPath(body, path);
                g.DrawPath(Pens.Black, path);
            }

            using (Brush stripe = new SolidBrush(Color.FromArgb(230, 230, 230)))
            {
                g.FillRectangle(stripe, 0, Height * 0.4f, Width, Height * 0.2f);
            }

            if (SirenOn)
            {
                Brush sirenBrush = (X / 10) % 2 == 0 ? Brushes.Red : Brushes.Blue;
                g.FillRectangle(sirenBrush, (Width / 2) - 6, -6, 12, 6);
            }
            EndOrientedDraw(g, state);
        }

        public override void Move(TrafficObjectCollection all)
        {
            TriggerYieldOnNearbyVehicles(all);
            EvaluateSurroundings(all);
            RoadLayout.Advance(this, ActualSpeed);

            if (ActualSpeed < DesiredSpeed)
            {
                AttemptLaneChange(all);
            }
        }
    }
}