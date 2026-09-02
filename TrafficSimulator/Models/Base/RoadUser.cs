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
            var objectsInTargetLane = all.GetObjectsInLane(Direction, targetLane);
            int safetyBuffer = 100;

            foreach (var obj in objectsInTargetLane)
            {
                if (obj == this) continue;

                if (Math.Abs(RoadLayout.ForwardDistance(this, obj)) < safetyBuffer)
                {
                    return false;
                }
            }

            return true;
        }

        public void AttemptLaneChange(TrafficObjectCollection all)
        {
            int targetLane = Lane == 0 ? 1 : 0;

            if (IsLaneClear(all, targetLane))
            {
                RoadLayout.SetLane(this, targetLane);
                IsOvertaking = true;
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

            return all.ActiveGreenDirection != Direction;
        }

        public override float EvaluateSurroundings(TrafficObjectCollection all)
        {
            TrafficObject closestAhead = null;
            int minDistance = int.MaxValue;
            var objectsInLane = all.GetObjectsInLane(Direction, this.Lane);

            foreach (var obj in objectsInLane)
            {
                int diff = RoadLayout.ForwardDistance(this, obj);
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
