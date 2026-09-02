using System;

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
        private const int SpawnMargin = 60;
        private const int DespawnMargin = 80;
        private const int QueueGap = 55;
        private const int QueueStopBuffer = 30;

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

        public static System.Drawing.Point GetSpawnPosition(Direction dir, int lane, int offset)
        {
            int laneCenter = GetLaneCenter(dir, lane);
            switch (dir)
            {
                case Direction.Right:
                    return new System.Drawing.Point(-SpawnMargin + offset, laneCenter);
                case Direction.Left:
                    return new System.Drawing.Point(CanvasWidth + SpawnMargin - offset, laneCenter);
                case Direction.Down:
                    return new System.Drawing.Point(laneCenter, -SpawnMargin + offset);
                case Direction.Up:
                    return new System.Drawing.Point(laneCenter, CanvasHeight + SpawnMargin - offset);
            }
            return new System.Drawing.Point(0, 0);
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
