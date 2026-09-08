using System;
using System.Drawing;

namespace TrafficSimulator
{
    [Serializable]
    public class Bus : RoadUser
    {
        public int PassengerCount;
        public bool IsStoppedAtStation;

        private const int Capacity = 24;
        private int _stopTimer = 0;
        private int _dwellTicks = 0;
        private BusStation _lastServicedStation = null;
        private static readonly Random _rng = new Random();

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
            if (PassengerCount < Capacity) PassengerCount++;
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

                    if (diff > 0 && diff <= Math.Max(50, ActualSpeed) && station != _lastServicedStation)
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

            Brush pedestrianBrush = isNight ? Brushes.LightGreen : Brushes.Green;
            using (Brush windowBrush = new SolidBrush(Color.FromArgb(150, 190, 220, 235)))
            {
                float winW = Width * 0.12f;
                float gap = Width * 0.05f;
                float winH = Height * 0.35f;
                float winY = Height * 0.2f;
                int headsShown = 0;
                int headsToShow = Math.Min(PassengerCount, 8);
                for (int i = 0; i < 4; i++)
                {
                    float winX = Width * 0.1f + i * (winW + gap);
                    g.FillRectangle(windowBrush, winX, winY, winW, winH);

                    // Riders peek through the windows so the bus visibly fills up
                    // as passengers board, even while it is still driving. Drawn
                    // the same way as a walking Pedestrian (green circle).
                    for (int slot = 0; slot < 2 && headsShown < headsToShow; slot++)
                    {
                        float headSize = winW * 0.4f;
                        float headX = winX + (slot == 0 ? winW * 0.1f : winW * 0.5f);
                        float headY = winY + winH * 0.15f;
                        g.FillEllipse(pedestrianBrush, headX, headY, headSize, headSize);
                        g.DrawEllipse(Pens.Black, headX, headY, headSize, headSize);
                        headsShown++;
                    }
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
                _dwellTicks++;

                // A few riders step off right as the doors open...
                if (_dwellTicks == 1 && PassengerCount > 0)
                {
                    int leaving = _rng.Next(0, Math.Min(PassengerCount, 3) + 1);
                    for (int i = 0; i < leaving; i++) DisembarkPassenger();
                }

                // ...then waiting passengers board one at a time while the doors stay open.
                if (_dwellTicks % 3 == 0 && _lastServicedStation != null && _lastServicedStation.WaitingPassengers > 0)
                {
                    BoardPassenger();
                    _lastServicedStation.WaitingPassengers--;
                }

                _stopTimer--;
                if (_stopTimer <= 0)
                {
                    IsStoppedAtStation = false;
                    _dwellTicks = 0;
                }
                return;
            }

            if (ShouldStopAtIntersection(all))
            {
                ActualSpeed = 0;
                return;
            }

            EvaluateSurroundings(all);

            // Lane 0 is the right-hand lane in every direction. A bus that was
            // placed in the left lane returns right as soon as traffic allows it.
            if (Lane != RoadLayout.RightLane || ActualSpeed < DesiredSpeed)
            {
                AttemptLaneChange(all);
            }

            BusStation targetStation = GetNextStationToStop(all);
            if (targetStation != null)
            {
                IsStoppedAtStation = true;
                _stopTimer = 15;
                _dwellTicks = 0;
                _lastServicedStation = targetStation;
                ActualSpeed = 0;
                return;
            }

            RoadLayout.Advance(this, ActualSpeed);
        }
    }
}
