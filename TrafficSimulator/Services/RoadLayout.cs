using System;
using System.Drawing;

namespace TrafficSimulator
{
    public static class RoadLayout
    {
        public const int CanvasWidth = 760;
        public const int CanvasHeight = 500;
        public const int CenterX = CanvasWidth / 2;
        public const int CenterY = CanvasHeight / 2;
        public const int RoadWidth = 160;
        public const int LanesPerDirection = 2;
        public const int RightLane = 0;
        public const int LaneWidth = RoadWidth / 2 / LanesPerDirection;
        private const int DespawnMargin = 80;
        private const int QueueGap = 90;
        private const int CurbOffset = 26;
        private const int CrosswalkOffset = 40;
        private const int CrosswalkHalfThickness = 9;
        private const int StopLineToCrosswalkGap = 10;
        private const int VehicleStopGap = 8;
        private const int PedestrianSize = 14;

        // Full object bounds, not just its centre, must fit in one sidewalk corner.
        public static Point NearestSidewalkPoint(int x, int y, int width, int height)
        {
            Point best = Point.Empty;
            double bestDistance = double.MaxValue;
            foreach (bool left in new[] { true, false })
            foreach (bool top in new[] { true, false })
            {
                int minX = left ? 0 : CenterX + RoadWidth / 2 + 2;
                int maxX = left ? CenterX - RoadWidth / 2 - width - 2 : CanvasWidth - width;
                int minY = top ? 0 : CenterY + RoadWidth / 2 + 2;
                int maxY = top ? CenterY - RoadWidth / 2 - height - 2 : CanvasHeight - height;
                int px = Math.Clamp(x, minX, maxX);
                int py = Math.Clamp(y, minY, maxY);
                // Candidate edges include the light housing so projection cannot land on it.
                Rectangle light = new Rectangle(left ? CenterX - RoadWidth / 2 - 25 : CenterX + RoadWidth / 2 + 3,
                    top ? CenterY - RoadWidth / 2 - 43 : CenterY + RoadWidth / 2 - 15, 22, 58);
                foreach (int candidateX in new[] { px, Math.Clamp(light.Left - width - 2, minX, maxX), Math.Clamp(light.Right + 2, minX, maxX) })
                foreach (int candidateY in new[] { py, Math.Clamp(light.Top - height - 2, minY, maxY), Math.Clamp(light.Bottom + 2, minY, maxY) })
                {
                    if (new Rectangle(candidateX, candidateY, width, height).IntersectsWith(light)) continue;
                    double distance = (double)(candidateX - x) * (candidateX - x) + (double)(candidateY - y) * (candidateY - y);
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        best = new Point(candidateX, candidateY);
                    }
                }
            }
            return best;
        }

        public static bool IsOnRoad(Rectangle bounds)
        {
            return bounds.IntersectsWith(new Rectangle(0, CenterY - RoadWidth / 2, CanvasWidth, RoadWidth)) ||
                bounds.IntersectsWith(new Rectangle(CenterX - RoadWidth / 2, 0, RoadWidth, CanvasHeight));
        }

        public static bool IsHorizontal(Direction dir)
        {
            return dir == Direction.Right || dir == Direction.Left;
        }

        public static int GetLaneCenter(Direction dir, int lane)
        {
            switch (dir)
            {
                case Direction.Right:
                    // West approach: eastbound traffic uses the lower half.
                    return CenterY + RoadWidth / 2 - LaneWidth * lane - LaneWidth / 2;
                case Direction.Left:
                    // East approach: westbound traffic uses the upper half.
                    return CenterY - RoadWidth / 2 + LaneWidth * lane + LaneWidth / 2;
                case Direction.Down:
                    return CenterX - RoadWidth / 2 + LaneWidth * lane + LaneWidth / 2;
                case Direction.Up:
                    return CenterX + RoadWidth / 2 - LaneWidth * lane - LaneWidth / 2;
            }
            return 0;
        }

        // Places the front of a road object behind the stop line, leaving a visible gap.
        // Width is the object's travel length even when it is drawn vertically.
        public static void PlaceBeforeCrosswalk(TrafficObject obj, int extraDistance = 0)
        {
            CenterInLane(obj);

            int travelSign = obj.Direction == Direction.Right || obj.Direction == Direction.Down ? 1 : -1;
            int stopLine = GetStopLineCoordinate(obj.Direction);
            int front = stopLine - travelSign * (VehicleStopGap + Math.Max(0, extraDistance));
            int objectCenter = front - travelSign * obj.Width / 2;

            // A long queue can push extraDistance past the canvas edge; clamp so the
            // object stacks at the edge instead of landing off-canvas and out of view.
            if (IsHorizontal(obj.Direction))
                obj.X = Math.Clamp(objectCenter - obj.Width / 2, 0, CanvasWidth - obj.Width);
            else
                obj.Y = Math.Clamp(objectCenter - obj.Height / 2, 0, CanvasHeight - obj.Height);
        }

        // A bus stop sits beside the outgoing road, after the intersection in the
        // direction of travel. This keeps it beyond the traffic light instead of
        // placing it in the queue approaching the light.
        public static System.Drawing.Point GetRoadsideStaticPosition(Direction dir, int lane, int distanceAfterIntersection)
        {
            int available = IsHorizontal(dir) ? CanvasWidth / 2 - RoadWidth / 2 - 40
                : CanvasHeight / 2 - RoadWidth / 2 - 40;
            distanceAfterIntersection = Math.Clamp(distanceAfterIntersection, 60, available);
            switch (dir)
            {
                case Direction.Right:
                    return new System.Drawing.Point(
                        CenterX + RoadWidth / 2 + distanceAfterIntersection,
                        CenterY + RoadWidth / 2 + CurbOffset);
                case Direction.Left:
                    return new System.Drawing.Point(
                        CenterX - RoadWidth / 2 - distanceAfterIntersection,
                        CenterY - RoadWidth / 2 - CurbOffset - 30);
                case Direction.Down:
                    return new System.Drawing.Point(
                        CenterX - RoadWidth / 2 - CurbOffset - 40,
                        CenterY + RoadWidth / 2 + distanceAfterIntersection);
                default: // Up
                    return new System.Drawing.Point(
                        CenterX + RoadWidth / 2 + CurbOffset,
                        CenterY - RoadWidth / 2 - distanceAfterIntersection);
            }
        }

        // where a pedestrian starts crossing: on the sidewalk just outside the given road,
        // walking straight across to the opposite curb.
        public static System.Drawing.Point GetCrosswalkSpawn(Direction road)
        {
            if (IsHorizontal(road))
            {
                int crosswalkX = road == Direction.Right
                    ? CenterX - RoadWidth / 2 - CrosswalkOffset
                    : CenterX + RoadWidth / 2 + CrosswalkOffset;

                // The traffic lights stand beside these crosswalks. Start on the
                // clear side of the stripes so the pedestrian never overlaps them.
                int x = road == Direction.Right
                    ? crosswalkX - PedestrianSize
                    : crosswalkX + PedestrianSize - 2;
                int y = CenterY - RoadWidth / 2 - PedestrianSize;
                return new System.Drawing.Point(x, y);
            }
            else
            {
                int crosswalkY = road == Direction.Down
                    ? CenterY - RoadWidth / 2 - CrosswalkOffset
                    : CenterY + RoadWidth / 2 + CrosswalkOffset;

                // Start exactly at the road edge, beyond the nearby light housing.
                int y = crosswalkY - PedestrianSize / 2;
                int x = CenterX - RoadWidth / 2 - PedestrianSize - 2;
                return new System.Drawing.Point(x, y);
            }
        }

        public static bool IsPedestrianDoneCrossing(TrafficObject obj)
        {
            switch (obj.Direction)
            {
                case Direction.Right: return obj.X >= CenterX + RoadWidth / 2;
                case Direction.Left: return obj.X + obj.Width <= CenterX - RoadWidth / 2;
                case Direction.Down: return obj.Y >= CenterY + RoadWidth / 2;
                case Direction.Up: return obj.Y + obj.Height <= CenterY - RoadWidth / 2;
                default: return false;
            }
        }

        public static void PlaceInQueue(TrafficObject obj, int queueIndex)
        {
            PlaceBeforeCrosswalk(obj, Math.Max(0, queueIndex) * QueueGap);
        }

        public static int GetStopLineCoordinate(Direction dir)
        {
            int travelSign = dir == Direction.Right || dir == Direction.Down ? 1 : -1;
            int crosswalkCenter = GetCrosswalkCenterCoordinate(dir);
            return crosswalkCenter - travelSign *
                (CrosswalkHalfThickness + StopLineToCrosswalkGap);
        }

        public static int GetCrosswalkCenterCoordinate(Direction dir)
        {
            switch (dir)
            {
                case Direction.Right: return CenterX - RoadWidth / 2 - CrosswalkOffset;
                case Direction.Left: return CenterX + RoadWidth / 2 + CrosswalkOffset;
                case Direction.Down: return CenterY - RoadWidth / 2 - CrosswalkOffset;
                case Direction.Up: return CenterY + RoadWidth / 2 + CrosswalkOffset;
                default: return 0;
            }
        }

        public static bool HasCrossedStopLine(TrafficObject obj)
        {
            int stopLine = GetStopLineCoordinate(obj.Direction);
            switch (obj.Direction)
            {
                case Direction.Right: return obj.X + obj.Width >= stopLine;
                case Direction.Left: return obj.X <= stopLine;
                case Direction.Down: return obj.Y + obj.Height / 2 + obj.Width / 2 >= stopLine;
                case Direction.Up: return obj.Y + obj.Height / 2 - obj.Width / 2 <= stopLine;
            }
            return false;
        }

        public static void Advance(TrafficObject obj, float amount)
        {
            switch (obj.Direction)
            {
                case Direction.Right: obj.X += (int)amount; break;
                case Direction.Left: obj.X -= (int)amount; break;
                case Direction.Down: obj.Y += (int)amount; break;
                case Direction.Up: obj.Y -= (int)amount; break;
            }
        }

        public static void SetLane(TrafficObject obj, int lane)
        {
            obj.Lane = lane;
            CenterInLane(obj);
        }

        // X/Y are the top-left drawing coordinates, while GetLaneCenter returns
        // the geometric centre line. Account for the object's dimensions so its
        // visual centre, rather than its top-left corner, lies on that line.
        public static void CenterInLane(TrafficObject obj)
        {
            int laneCenter = GetLaneCenter(obj.Direction, obj.Lane);
            if (IsHorizontal(obj.Direction))
                obj.Y = laneCenter - obj.Height / 2;
            else
                obj.X = laneCenter - obj.Width / 2;
        }

        public static int ForwardDistance(TrafficObject self, TrafficObject other)
        {
            switch (self.Direction)
            {
                case Direction.Right: return other.X - self.X;
                case Direction.Left: return self.X - other.X;
                case Direction.Down: return other.Y - self.Y;
                case Direction.Up: return self.Y - other.Y;
            }
            return 0;
        }

        public static bool IsOutOfBounds(TrafficObject obj)
        {
            if (obj is Pedestrian)
            {
                Rectangle bounds = obj.GetBounds();
                return bounds.Right <= 0 || bounds.Left >= CanvasWidth ||
                    bounds.Bottom <= 0 || bounds.Top >= CanvasHeight;
            }
            switch (obj.Direction)
            {
                case Direction.Right: return obj.X > CanvasWidth + DespawnMargin;
                case Direction.Left: return obj.X < -DespawnMargin;
                case Direction.Down: return obj.Y > CanvasHeight + DespawnMargin;
                case Direction.Up: return obj.Y < -DespawnMargin;
            }
            return false;
        }
    }
}
