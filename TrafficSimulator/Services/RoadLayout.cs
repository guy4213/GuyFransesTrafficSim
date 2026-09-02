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
        public const int LaneWidth = RoadWidth / 2 / LanesPerDirection;
        private const int DespawnMargin = 80;
        private const int QueueGap = 55;
        private const int QueueStopBuffer = 30;
        private const int CurbOffset = 26;
        private const int CrosswalkOffset = 26;
        private const int SidewalkOffset = 30;

        public static bool IsHorizontal(Direction dir)
        {
            return dir == Direction.Right || dir == Direction.Left;
        }

        public static int GetLaneCenter(Direction dir, int lane)
        {
            switch (dir)
            {
                case Direction.Right:
                    return CenterY - RoadWidth / 2 + LaneWidth * (lane) + LaneWidth / 2;
                case Direction.Left:
                    return CenterY + RoadWidth / 2 - LaneWidth * (lane) - LaneWidth / 2;
                case Direction.Down:
                    return CenterX - RoadWidth / 2 + LaneWidth * (lane) + LaneWidth / 2;
                case Direction.Up:
                    return CenterX + RoadWidth / 2 - LaneWidth * (lane) - LaneWidth / 2;
            }
            return 0;
        }

        // static object placed in the lane itself (e.g. a road hazard blocking traffic),
        // measured as a distance back from the stop line - always lands on-screen.
        public static System.Drawing.Point GetLaneStaticPosition(Direction dir, int lane, int offsetFromStopLine)
        {
            int laneCenter = GetLaneCenter(dir, lane);
            switch (dir)
            {
                case Direction.Right:
                    return new System.Drawing.Point(CenterX - RoadWidth / 2 - offsetFromStopLine, laneCenter);
                case Direction.Left:
                    return new System.Drawing.Point(CenterX + RoadWidth / 2 + offsetFromStopLine, laneCenter);
                case Direction.Down:
                    return new System.Drawing.Point(laneCenter, CenterY - RoadWidth / 2 - offsetFromStopLine);
                case Direction.Up:
                    return new System.Drawing.Point(laneCenter, CenterY + RoadWidth / 2 + offsetFromStopLine);
            }
            return new System.Drawing.Point(0, 0);
        }

        // static roadside object (bus stop) pushed out of the lane onto the curb,
        // so it doesn't block traffic - measured back from the stop line.
        public static System.Drawing.Point GetRoadsideStaticPosition(Direction dir, int lane, int offsetFromStopLine)
        {
            Point lanePos = GetLaneStaticPosition(dir, lane, offsetFromStopLine);

            // push all the way past the outer edge of the road (not just past the lane
            // center) so the station clears the asphalt regardless of which lane was picked.
            switch (dir)
            {
                case Direction.Right: // West lanes sit in the top half -> push up past the curb
                    return new System.Drawing.Point(lanePos.X, CenterY - RoadWidth / 2 - CurbOffset);
                case Direction.Left: // East lanes sit in the bottom half -> push down past the curb
                    return new System.Drawing.Point(lanePos.X, CenterY + RoadWidth / 2 + CurbOffset);
                case Direction.Down: // North lanes sit in the left half -> push left past the curb
                    return new System.Drawing.Point(CenterX - RoadWidth / 2 - CurbOffset, lanePos.Y);
                default: // Up: South lanes sit in the right half -> push right past the curb
                    return new System.Drawing.Point(CenterX + RoadWidth / 2 + CurbOffset, lanePos.Y);
            }
        }

        // where a pedestrian starts crossing: on the sidewalk just outside the given road,
        // walking straight across to the opposite curb.
        public static System.Drawing.Point GetCrosswalkSpawn(Direction road)
        {
            if (IsHorizontal(road))
            {
                int x = road == Direction.Right ? CenterX - RoadWidth / 2 - CrosswalkOffset : CenterX + RoadWidth / 2 + CrosswalkOffset;
                int y = CenterY - RoadWidth / 2 - SidewalkOffset;
                return new System.Drawing.Point(x, y);
            }
            else
            {
                int y = road == Direction.Down ? CenterY - RoadWidth / 2 - CrosswalkOffset : CenterY + RoadWidth / 2 + CrosswalkOffset;
                int x = CenterX - RoadWidth / 2 - SidewalkOffset;
                return new System.Drawing.Point(x, y);
            }
        }

        public static bool IsPedestrianDoneCrossing(TrafficObject obj)
        {
            int limit = RoadWidth / 2 + SidewalkOffset + 20;
            if (obj.Direction == Direction.Down) return obj.Y > CenterY + limit;
            return obj.X > CenterX + limit; // Direction.Right
        }

        public static System.Drawing.Point GetQueuePosition(Direction dir, int lane, int queueIndex)
        {
            int laneCenter = GetLaneCenter(dir, lane);
            int back = QueueStopBuffer + queueIndex * QueueGap;
            switch (dir)
            {
                case Direction.Right:
                    return new System.Drawing.Point(CenterX - RoadWidth / 2 - back, laneCenter);
                case Direction.Left:
                    return new System.Drawing.Point(CenterX + RoadWidth / 2 + back, laneCenter);
                case Direction.Down:
                    return new System.Drawing.Point(laneCenter, CenterY - RoadWidth / 2 - back);
                case Direction.Up:
                    return new System.Drawing.Point(laneCenter, CenterY + RoadWidth / 2 + back);
            }
            return new System.Drawing.Point(0, 0);
        }

        public static int GetStopLineCoordinate(Direction dir)
        {
            switch (dir)
            {
                case Direction.Right: return CenterX - RoadWidth / 2;
                case Direction.Left: return CenterX + RoadWidth / 2;
                case Direction.Down: return CenterY - RoadWidth / 2;
                case Direction.Up: return CenterY + RoadWidth / 2;
            }
            return 0;
        }

        public static bool HasCrossedStopLine(TrafficObject obj)
        {
            int stopLine = GetStopLineCoordinate(obj.Direction);
            switch (obj.Direction)
            {
                case Direction.Right: return obj.X >= stopLine;
                case Direction.Left: return obj.X <= stopLine;
                case Direction.Down: return obj.Y >= stopLine;
                case Direction.Up: return obj.Y <= stopLine;
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
            int laneCenter = GetLaneCenter(obj.Direction, lane);
            if (IsHorizontal(obj.Direction))
                obj.Y = laneCenter;
            else
                obj.X = laneCenter;
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
