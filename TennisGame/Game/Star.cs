using SFML.Graphics;
using SFML.System;

namespace TennisGame.Game
{
    public class Star : GameObject
    {
        public float RotationSpeed { get; set; } = 180f; // derece/saniye
        public float LifeTime { get; set; } = 10f; // saniye
        public float CurrentLife { get; set; } = 0f;
        public bool IsActive { get; set; } = true;

        public Star(Vector2f position, Color color) 
            : base(position, new Vector2f(20, 20), color)
        {
            CurrentLife = 0f;
        }

        public override void Update(float deltaTime)
        {
            if (!IsActive) return;

            CurrentLife += deltaTime;
            if (CurrentLife >= LifeTime)
            {
                IsActive = false;
            }
        }

        public override void Draw(RenderWindow window)
        {
            if (!IsActive) return;

            // Yıldız şekli oluştur
            var star = new CircleShape(Size.X / 2, 5) // 5 köşeli yıldız
            {
                Position = Position,
                FillColor = Color,
                OutlineColor = Color.White,
                OutlineThickness = 2f
            };

            // Döndürme efekti
            star.Rotation = CurrentLife * RotationSpeed;

            window.Draw(star);
        }

        public bool IntersectsWithBall(Ball ball)
        {
            if (!IsActive) return false;

            float distance = (float)Math.Sqrt(
                Math.Pow(Position.X + Size.X / 2 - (ball.Position.X + ball.Size.X / 2), 2) +
                Math.Pow(Position.Y + Size.Y / 2 - (ball.Position.Y + ball.Size.Y / 2), 2)
            );

            return distance < (Size.X / 2 + ball.Size.X / 2);
        }
    }
} 