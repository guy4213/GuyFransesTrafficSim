using System;

namespace TrafficSimulator
{
    public abstract class RoadUser : TrafficObject
    {
        public bool IsOvertaking;

        protected RoadUser(int x, int y, int lane, Direction dir, float desiredSpeed)
            : base(x, y, lane, dir, desiredSpeed)
        {
            throw new NotImplementedException();
        }

        protected bool IsLaneClear(TrafficObjectCollection all, int targetLane)
        {
            throw new NotImplementedException();
        }

        protected void AttemptLaneChange(TrafficObjectCollection all)
        {
            throw new NotImplementedException();
        }

        public override float EvaluateSurroundings(TrafficObjectCollection all)
        {
            throw new NotImplementedException();
        }
    }
}
