using System;
using System.Drawing;

namespace TrafficSimulator
{
    public class RoadHazard : TrafficObject
    {
        public RoadHazard(int x, int y, int lane, Direction dir, float desiredSpeed)
            : base(x, y, lane, dir, desiredSpeed)
        {
            throw new NotImplementedException();
        }

        public override float EvaluateSurroundings(TrafficObjectCollection all)
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
