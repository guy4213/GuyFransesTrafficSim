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
            X = x;
            Y = y;
            Lane = lane;
            Direction = dir;
            DesiredSpeed = desiredSpeed;
            ActualSpeed = desiredSpeed;
        }

        public abstract void Draw(Graphics g, bool isNight);
        public abstract void Move(TrafficObjectCollection all);
        public virtual Rectangle GetBounds()
        {
            return new Rectangle(X, Y, Width, Height);
        }

        public virtual float EvaluateSurroundings(TrafficObjectCollection all)
        {
            return ActualSpeed;
        }
    }
}