using System;
using System.Drawing;

namespace TrafficSimulator
{
    [Serializable]
    public class Bus : RoadUser
    {
        public int PassengerCount;
        public bool IsStoppedAtStation;

        private int _stopTimer = 0;
        private BusStation _lastServicedStation = null;

        public Bus(int x, int y, int lane, Direction dir, float desiredSpeed=50f, int initialPassengers = 0)
            : base(x, y, lane, dir, desiredSpeed)
        {
            Width = 80;
            Height = 35;
            PassengerCount = initialPassengers;
            IsStoppedAtStation = false;
        }

        public void BoardPassenger()
        {
            PassengerCount++;
        }

        public void DisembarkPassenger()
        {
            PassengerCount = Math.Max(0, PassengerCount - 1);
        }

        private BusStation GetNextStationToStop(TrafficObjectCollection all)
        {
            foreach (var obj in all.GetObjectsInLane(Direction, Lane))
            {
                if (obj is BusStation station)
                {
                    int diff = RoadLayout.ForwardDistance(this, station);

                    if (diff <= 0 && _lastServicedStation == station)
                    {
                        _lastServicedStation = null;
                    }

                    if (diff > 0 && diff < 50 && station != _lastServicedStation)
                    {
                        return station;
                    }
                }
            }
            return null;
        }

        public override void Draw(Graphics g, bool isNight)
        {
            Color bodyColor = isNight ? Color.FromArgb(120, 220, 220) : Color.FromArgb(90, 95, 102);
            var state = BeginOrientedDraw(g);

            DrawWheels(g, Width, Height);

            using (Brush body = new SolidBrush(bodyColor))
            using (var path = RoundedRect(0, 0, Width, Height, 5))
            {
                g.FillPath(body, path);
                g.DrawPath(Pens.Black, path);
            }

            using (Brush windowBrush = new SolidBrush(Color.FromArgb(150, 190, 220, 235)))
            {
                float winW = Width * 0.12f;
                float gap = Width * 0.05f;
                for (int i = 0; i < 4; i++)
                {
                    g.FillRectangle(windowBrush, Width * 0.1f + i * (winW + gap), Height * 0.2f, winW, Height * 0.35f);
                }
            }
            EndOrientedDraw(g, state);

            using (Font font = new Font("Arial", 8, FontStyle.Bold))
            {
                Brush textBrush = isNight ? Brushes.White : Brushes.Black;
                g.DrawString($"Bus ({PassengerCount})", font, textBrush, X, Y - 14);
            }
        }

        public override void Move(TrafficObjectCollection all)
        {
            if (IsStoppedAtStation)
            {
                ActualSpeed = 0;
                _stopTimer--;

                if (_stopTimer <= 0)
                {
                    IsStoppedAtStation = false;
                }
                return;
            }

            if (ShouldStopAtIntersection(all))
            {
                ActualSpeed = 0;
                return;
            }

            BusStation targetStation = GetNextStationToStop(all);
            if (targetStation != null)
            {
                IsStoppedAtStation = true;
                _stopTimer = 15;
                _lastServicedStation = targetStation;
                BoardPassenger();
                ActualSpeed = 0;
                return;
            }

            EvaluateSurroundings(all);
            RoadLayout.Advance(this, ActualSpeed);
        }
    }
}