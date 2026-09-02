using System;

namespace TrafficSimulator
{
    public abstract class RoadUser : TrafficObject
    {
        public bool IsOvertaking;

        protected RoadUser(int x, int y, int lane, Direction dir, float desiredSpeed)
            : base(x, y, lane, dir, desiredSpeed)
        {
        }

        protected bool IsLaneClear(TrafficObjectCollection all, int targetLane)
        {
            var objectsInTargetLane = all.GetObjectsInLane(targetLane);
            int safetyBuffer = 100;

            foreach (var obj in objectsInTargetLane)
            {
                if (obj == this) continue;

                if (Math.Abs(obj.X - this.X) < safetyBuffer)
                {
                    return false;
                }
            }

            return true;
        }

        public void AttemptLaneChange(TrafficObjectCollection all)
        {
            int targetLane = Lane + 1;

            int maxLanes = 3;
            if (targetLane >= maxLanes)
            {
                targetLane = Lane - 1;
                if (targetLane < 0) return;
            }

            if (IsLaneClear(all, targetLane))
            {
                Lane = targetLane;

                Y = targetLane * 50 + 20;

                IsOvertaking = true;
            }
        }
        public override float EvaluateSurroundings(TrafficObjectCollection all)
        {
            TrafficObject closestAhead = null;
            int minDistance = int.MaxValue;
            var objectsInLane = all.GetObjectsInLane(this.Lane);

            foreach (var obj in objectsInLane)
            {
                int diff = obj.X - this.X;
                if (obj != this && diff > 0 && diff < 100 && minDistance > diff)
                {
                    minDistance = diff;
                    closestAhead = obj;
                }
            }

            ActualSpeed = closestAhead != null ? closestAhead.ActualSpeed : DesiredSpeed;

            return ActualSpeed;
        }

    }
}
