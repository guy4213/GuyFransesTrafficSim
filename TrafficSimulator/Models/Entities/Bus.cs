using System;
using System.Drawing;

namespace TrafficSimulator
{
    public class Bus : RoadUser
    {
        public int PassengerCount;
        public bool IsStoppedAtStation;

        public Bus(int x, int y, int lane, Direction dir, float desiredSpeed)
            : base(x, y, lane, dir, desiredSpeed)
        {
            throw new NotImplementedException();
        }

        public void BoardPassenger()
        {
            throw new NotImplementedException();
        }

        public void DisembarkPassenger()
        {
            throw new NotImplementedException();
        }

        public bool CheckNearbyStation(TrafficObjectCollection all)
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
