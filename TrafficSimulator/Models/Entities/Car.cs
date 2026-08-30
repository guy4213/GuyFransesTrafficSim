using System;
using System.Drawing;

namespace TrafficSimulator
{
    public class Car : RoadUser
    {
        public CarModel Model;

        public Car(int x, int y, int lane, Direction dir, float desiredSpeed, CarModel model)
            : base(x, y, lane, dir, desiredSpeed)
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
