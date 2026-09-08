using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace TrafficSimulator
{
    [Serializable]
    public class Pedestrian : RoadUser
    {
        public bool IsCrossing { get; set; }
        public bool WalksOnSidewalk { get; private set; }
        [OptionalField] private Point _crossingStart;
        [OptionalField] private Direction _crossingDirection;
        [OptionalField] private bool _approaching;
        [OptionalField] private bool _departing;
        [OptionalField] private int _routeVersion = 1;

        [OnDeserialized]
        private void RestoreLegacyRoute(StreamingContext context)
        {
            if (_routeVersion == 0)
            {
                PlaceOnSidewalk();
                _routeVersion = 1;
            }
        }

        public Pedestrian(int x, int y, int lane, Direction road, float desiredSpeed = 3f)
            : base(x, y, lane, road, desiredSpeed)
        {
            Width = Height = 14;
            PlaceOnSidewalk();
        }

        public static Pedestrian CreateSidewalkWalker(int x, int y, Direction direction, float desiredSpeed = 2f)
        {
            var pedestrian = new Pedestrian(x, y, 0, direction, desiredSpeed);
            pedestrian.Direction = direction;
            pedestrian._approaching = false;
            pedestrian._departing = true;
            return pedestrian;
        }

        public static Pedestrian CreateSidewalkWalkerTowardsCrosswalk(
            int x, int y, Direction walkingDirection, Direction crosswalkRoad, float desiredSpeed = 2f)
        {
            return new Pedestrian(x, y, 0, crosswalkRoad, desiredSpeed);
        }

        // Editing resets the route, never the position during a valid crossing.
        public void PlaceOnSidewalk()
        {
            Point position = RoadLayout.NearestSidewalkPoint(X, Y, Width, Height);
            X = position.X;
            Y = position.Y;
            IsCrossing = false;
            WalksOnSidewalk = true;
            _departing = false;
            _approaching = true;

            bool left = X + Width <= RoadLayout.CenterX - RoadLayout.RoadWidth / 2;
            bool top = Y + Height <= RoadLayout.CenterY - RoadLayout.RoadWidth / 2;
            int crossX = RoadLayout.GetCrosswalkCenterCoordinate(left ? Direction.Right : Direction.Left) - Width / 2;
            int crossY = RoadLayout.GetCrosswalkCenterCoordinate(top ? Direction.Down : Direction.Up) - Height / 2;
            Point verticalStart = new Point(crossX, top
                ? RoadLayout.CenterY - RoadLayout.RoadWidth / 2 - Height - 2
                : RoadLayout.CenterY + RoadLayout.RoadWidth / 2 + 2);
            Point horizontalStart = new Point(left
                ? RoadLayout.CenterX - RoadLayout.RoadWidth / 2 - Width - 2
                : RoadLayout.CenterX + RoadLayout.RoadWidth / 2 + 2, crossY);
            bool vertical = DistanceSquared(verticalStart) <= DistanceSquared(horizontalStart);
            _crossingStart = vertical ? verticalStart : horizontalStart;
            _crossingDirection = vertical ? (top ? Direction.Down : Direction.Up)
                : (left ? Direction.Right : Direction.Left);
        }

        private double DistanceSquared(Point p) => (double)(p.X - X) * (p.X - X) + (double)(p.Y - Y) * (p.Y - Y);

        public override void Draw(Graphics g, bool isNight)
        {
            g.FillEllipse(isNight ? Brushes.LightGreen : Brushes.Green, X, Y, Width, Height);
            g.DrawEllipse(Pens.Black, X, Y, Width, Height);
        }

        public override void Move(TrafficObjectCollection all)
        {
            int step = Math.Max(1, (int)DesiredSpeed);
            ActualSpeed = step;
            if (_approaching)
            {
                // Both legs stay inside the same sidewalk quadrant.
                if (X != _crossingStart.X)
                    X += Math.Sign(_crossingStart.X - X) * Math.Min(step, Math.Abs(_crossingStart.X - X));
                else if (Y != _crossingStart.Y)
                    Y += Math.Sign(_crossingStart.Y - Y) * Math.Min(step, Math.Abs(_crossingStart.Y - Y));
                else
                {
                    _approaching = false;
                    Direction = _crossingDirection;
                }
                return;
            }

            if (_departing)
            {
                int previousX = X, previousY = Y;
                RoadLayout.Advance(this, step);
                if (RoadLayout.IsOnRoad(GetBounds()))
                {
                    X = previousX;
                    Y = previousY;
                    PlaceOnSidewalk();
                }
                return;
            }

            bool conflict = RoadLayout.IsHorizontal(Direction) != RoadLayout.IsHorizontal(all.ActiveGreenDirection);
            if (!IsCrossing && (conflict || all.HasMovingEmergencyVehicle || all.HasActiveEmergency))
            {
                ActualSpeed = 0;
                return;
            }

            IsCrossing = true;
            WalksOnSidewalk = false;
            RoadLayout.Advance(this, step);
            if (RoadLayout.IsPedestrianDoneCrossing(this))
            {
                IsCrossing = false;
                WalksOnSidewalk = true;
                _departing = true;
            }
        }
    }
}
