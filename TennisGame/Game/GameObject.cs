using SFML.Graphics;
using SFML.System;

namespace TennisGame.Game
{
    public abstract class GameObject
    {
        public Vector2f Position { get; set; }
        public Vector2f Size { get; set; }
        public Vector2f Velocity { get; set; }
        public Color Color { get; set; }
        public bool IsActive { get; set; } = true;

        protected GameObject(Vector2f position, Vector2f size, Color color)
        {
            Position = position;
            Size = size;
            Color = color;
            Velocity = new Vector2f(0, 0);
        }

        public virtual void Update(float deltaTime)
        {
            Position += Velocity * deltaTime;
        }

        public virtual void Draw(RenderWindow window)
        {
            var shape = new RectangleShape(Size)
            {
                Position = Position,
                FillColor = Color
            };
            window.Draw(shape);
        }

        public bool Intersects(GameObject other)
        {
            return Position.X < other.Position.X + other.Size.X &&
                   Position.X + Size.X > other.Position.X &&
                   Position.Y < other.Position.Y + other.Size.Y &&
                   Position.Y + Size.Y > other.Position.Y;
        }

        public FloatRect GetBounds()
        {
            return new FloatRect(Position, Size);
        }
    }
} 