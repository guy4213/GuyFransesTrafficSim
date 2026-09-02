using System;
using System.Drawing;
using System.Drawing.Drawing2D;

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

        protected GraphicsState BeginOrientedDraw(Graphics g)
        {
            GraphicsState state = g.Save();
            g.TranslateTransform(X + Width / 2f, Y + Height / 2f);
            if (Direction == Direction.Up || Direction == Direction.Down)
            {
                g.RotateTransform(90);
            }
            g.TranslateTransform(-Width / 2f, -Height / 2f);
            return state;
        }

        protected void EndOrientedDraw(Graphics g, GraphicsState state)
        {
            g.Restore(state);
        }
    }
}