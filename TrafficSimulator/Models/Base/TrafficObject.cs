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

        protected static void EndOrientedDraw(Graphics g, GraphicsState state)
        {
            g.Restore(state);
        }

        protected static GraphicsPath RoundedRect(float x, float y, float w, float h, float r)
        {
            var path = new GraphicsPath();
            path.AddArc(x, y, r * 2, r * 2, 180, 90);
            path.AddArc(x + w - r * 2, y, r * 2, r * 2, 270, 90);
            path.AddArc(x + w - r * 2, y + h - r * 2, r * 2, r * 2, 0, 90);
            path.AddArc(x, y + h - r * 2, r * 2, r * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected static void DrawWheels(Graphics g, float width, float height)
        {
            using (Brush wheelBrush = new SolidBrush(Color.FromArgb(28, 28, 28)))
            {
                float wheelLen = Math.Min(9f, width * 0.18f);
                float wheelThick = 3f;
                float frontX = width * 0.22f;
                float backX = width * 0.78f;
                g.FillRectangle(wheelBrush, frontX - wheelLen / 2, -wheelThick, wheelLen, wheelThick);
                g.FillRectangle(wheelBrush, frontX - wheelLen / 2, height, wheelLen, wheelThick);
                g.FillRectangle(wheelBrush, backX - wheelLen / 2, -wheelThick, wheelLen, wheelThick);
                g.FillRectangle(wheelBrush, backX - wheelLen / 2, height, wheelLen, wheelThick);
            }
        }
    }
}