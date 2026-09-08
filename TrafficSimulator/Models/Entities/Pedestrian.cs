using System;
using System.Drawing;

namespace TrafficSimulator
{
    [Serializable]
    public class Pedestrian : RoadUser
    {
        public bool IsCrossing { get; set; }
        public bool WalksOnSidewalk { get; private set; }

        private Direction _conflictA;
        private Direction _conflictB;
        private readonly bool _joinsCrosswalk;
        private readonly int _destinationCoordinate;
        private readonly Direction _targetCrosswalkRoad;

        // 'road' is the approach whose crosswalk this pedestrian uses (Right/Down/Left/Up
        // = West/North/East/South) - the pedestrian actually walks perpendicular to it.
        public Pedestrian(int x, int y, int lane, Direction road, float desiredSpeed = 15f)
            : base(x, y, lane, RoadLayout.IsHorizontal(road) ? Direction.Down : Direction.Right, desiredSpeed)
        {
            Width = 14;
            Height = 14;
            IsCrossing = false;
            WalksOnSidewalk = false;
            _joinsCrosswalk = false;
            _destinationCoordinate = 0;
            _targetCrosswalkRoad = road;
            ConfigureCrosswalkConflicts(road);
        }

        private Pedestrian(
            int x,
            int y,
            Direction walkingDirection,
            float desiredSpeed,
            Direction? targetCrosswalkRoad)
            : base(x, y, 0, walkingDirection, desiredSpeed)
        {
            Width = 14;
            Height = 14;
            IsCrossing = false;
            WalksOnSidewalk = true;
            _joinsCrosswalk = targetCrosswalkRoad.HasValue;
            _targetCrosswalkRoad = targetCrosswalkRoad.GetValueOrDefault();

            Point target = targetCrosswalkRoad.HasValue
                ? RoadLayout.GetCrosswalkSpawn(targetCrosswalkRoad.Value)
                : Point.Empty;
            _destinationCoordinate = RoadLayout.IsHorizontal(walkingDirection) ? target.X : target.Y;
            _conflictA = Direction.Up;
            _conflictB = Direction.Down;
        }

        public static Pedestrian CreateSidewalkWalker(
            int x,
            int y,
            Direction walkingDirection,
            float desiredSpeed = 2f)
        {
            return new Pedestrian(x, y, walkingDirection, desiredSpeed, null);
        }

        public static Pedestrian CreateSidewalkWalkerTowardsCrosswalk(
            int x,
            int y,
            Direction walkingDirection,
            Direction crosswalkRoad,
            float desiredSpeed = 2f)
        {
            return new Pedestrian(x, y, walkingDirection, desiredSpeed, crosswalkRoad);
        }

        public override void Draw(Graphics g, bool isNight)
        {
            Brush pedBrush = isNight ? Brushes.LightGreen : Brushes.Green;
            g.FillEllipse(pedBrush, X, Y, Width, Height);
            g.DrawEllipse(Pens.Black, X, Y, Width, Height);
        }

        public override void Move(TrafficObjectCollection all)
        {
            if (WalksOnSidewalk)
            {
                MoveAlongSidewalk();
                return;
            }

            bool safeToCross = all.ActiveGreenDirection != _conflictA && all.ActiveGreenDirection != _conflictB;
            bool emergencyBlocksEntry = !IsCrossing &&
                (all.HasMovingEmergencyVehicle || all.HasActiveEmergency);

            // Never enter in front of a moving emergency vehicle. A pedestrian
            // already on the road keeps moving so the crossing is cleared safely.
            if (!IsCrossing && (!safeToCross || emergencyBlocksEntry))
            {
                ActualSpeed = 0;
                IsCrossing = false;
                return;
            }

            IsCrossing = true;
            ActualSpeed = DesiredSpeed;
            RoadLayout.Advance(this, ActualSpeed);
        }

        private void MoveAlongSidewalk()
        {
            IsCrossing = false;
            ActualSpeed = DesiredSpeed;

            if (_joinsCrosswalk && ReachedDestination())
            {
                JoinCrosswalk();
                return;
            }

            RoadLayout.Advance(this, ActualSpeed);

            if (_joinsCrosswalk && ReachedDestination())
            {
                JoinCrosswalk();
            }
        }

        private void JoinCrosswalk()
        {
            Point crossingStart = RoadLayout.GetCrosswalkSpawn(_targetCrosswalkRoad);
            X = crossingStart.X;
            Y = crossingStart.Y;
            Direction = RoadLayout.IsHorizontal(_targetCrosswalkRoad)
                ? Direction.Down
                : Direction.Right;
            ConfigureCrosswalkConflicts(_targetCrosswalkRoad);
            WalksOnSidewalk = false;
            ActualSpeed = 0;
        }

        private void ConfigureCrosswalkConflicts(Direction road)
        {
            if (RoadLayout.IsHorizontal(road))
            {
                _conflictA = Direction.Right;
                _conflictB = Direction.Left;
            }
            else
            {
                _conflictA = Direction.Down;
                _conflictB = Direction.Up;
            }
        }

        private bool ReachedDestination()
        {
            switch (Direction)
            {
                case Direction.Right: return X >= _destinationCoordinate;
                case Direction.Left: return X <= _destinationCoordinate;
                case Direction.Down: return Y >= _destinationCoordinate;
                case Direction.Up: return Y <= _destinationCoordinate;
                default: return false;
            }
        }
    }
}
