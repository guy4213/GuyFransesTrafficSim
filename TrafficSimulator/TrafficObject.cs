using System;
using System.Drawing;

namespace TrafficSimulator
{
    [Serializable]
    public abstract class TrafficObject
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public int Lane;
        public Direction Direction;
        public float DesiredSpeed;
        public float ActualSpeed;

        protected TrafficObject(int x, int y, int lane, Direction dir, float desiredSpeed)
        {
            throw new NotImplementedException();
        }

        public abstract void Draw(Graphics g, bool isNight);

        public abstract void Move();

        public virtual Rectangle GetBounds()
        {
            throw new NotImplementedException();
        }

        public virtual float EvaluateSurroundings(TrafficObjectCollection all)
        {
            throw new NotImplementedException();
        }
    }
}
