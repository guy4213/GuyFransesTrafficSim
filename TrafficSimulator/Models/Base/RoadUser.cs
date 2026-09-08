using System;

namespace TrafficSimulator
{
    [Serializable]
    public abstract class RoadUser : TrafficObject
    {
        public bool IsOvertaking;

        public override System.Drawing.Rectangle GetBounds()
        {
            if (this is Pedestrian || RoadLayout.IsHorizontal(Direction)) return base.GetBounds();
            return new System.Drawing.Rectangle(X + (Width - Height) / 2,
                Y + (Height - Width) / 2, Height, Width);
        }

        protected RoadUser(int x, int y, int lane, Direction dir, float desiredSpeed)
            : base(x, y, lane, dir, desiredSpeed)
        {
        }

        protected bool IsLaneClear(TrafficObjectCollection all, int targetLane)
        {
            var current = GetBounds();
            var target = current;
            int shift = RoadLayout.GetLaneCenter(Direction, targetLane) - RoadLayout.GetLaneCenter(Direction, Lane);
            if (RoadLayout.IsHorizontal(Direction)) target.Offset(0, shift);
            else target.Offset(shift, 0);
            var corridor = System.Drawing.Rectangle.Union(current, target);
            corridor.Inflate(8, 8);
            foreach (var obj in all.GetAllObjects())
                if (obj != this && corridor.IntersectsWith(obj.GetBounds())) return false;

            return true;
        }

        public void AttemptLaneChange(TrafficObjectCollection all)
        {
            if (this is Pedestrian) return;
            int targetLane = Lane == 0 ? 1 : 0;

            if (IsLaneClear(all, targetLane))
            {
                RoadLayout.SetLane(this, targetLane);
                IsOvertaking = true;
                EvaluateSurroundings(all);
            }
        }
        protected bool ShouldStopAtIntersection(TrafficObjectCollection all)
        {
            if (!(this is EmergencyVehicle) && all.HasActiveEmergency)
            {
                return true;
            }

            if (RoadLayout.HasCrossedStopLine(this))
            {
                return false;
            }

            if (IsPedestrianOnMyCrosswalk(all))
            {
                return true;
            }

            return all.IsAmber || all.ActiveGreenDirection != Direction;
        }

        // A pedestrian using this direction's crosswalk is walking straight through
        // this lane's path, so no vehicle may enter until they have fully crossed.
        private bool IsPedestrianOnMyCrosswalk(TrafficObjectCollection all)
        {
            foreach (var obj in all.GetAllObjects())
            {
                if (obj is Pedestrian pedestrian && pedestrian.IsCrossing && pedestrian.Direction == Direction)
                    return true;
            }
            return false;
        }

        public override float EvaluateSurroundings(TrafficObjectCollection all)
        {
            ActualSpeed = Math.Max(0, DesiredSpeed);
            var bounds = GetBounds();
            foreach (var obj in all.GetAllObjects())
            {
                if (obj == this) continue;
                var other = obj.GetBounds();
                bool horizontal = RoadLayout.IsHorizontal(Direction);
                bool overlapsAcross = horizontal ? bounds.Top < other.Bottom && bounds.Bottom > other.Top
                    : bounds.Left < other.Right && bounds.Right > other.Left;
                if (!overlapsAcross) continue;
                int gap;
                switch (Direction)
                {
                    case Direction.Right: if (other.Right <= bounds.Left) continue; gap = other.Left - bounds.Right; break;
                    case Direction.Left: if (other.Left >= bounds.Right) continue; gap = bounds.Left - other.Right; break;
                    case Direction.Down: if (other.Bottom <= bounds.Top) continue; gap = other.Top - bounds.Bottom; break;
                    default: if (other.Top >= bounds.Bottom) continue; gap = bounds.Top - other.Bottom; break;
                }
                ActualSpeed = Math.Min(ActualSpeed, Math.Max(0, gap - 8));
            }

            return ActualSpeed;
        }

    }
}
