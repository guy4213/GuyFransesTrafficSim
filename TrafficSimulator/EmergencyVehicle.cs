using System;
using System.Drawing;

namespace TrafficSimulator
{
    public class EmergencyVehicle : RoadUser
    {
        public bool SirenOn;

        public EmergencyVehicle(int x, int y, int lane, Direction dir, float desiredSpeed)
            : base(x, y, lane, dir, desiredSpeed)
        {
            throw new NotImplementedException();
        }

        public void TriggerYieldOnNearbyVehicles(TrafficObjectCollection all)
        {
            throw new NotImplementedException();
        }

        public override void Draw(Graphics g, bool isNight)
        {
            throw new NotImplementedException();
        }

        public override void Move()
        {
            throw new NotImplementedException();
        }
    }
}
