using SFML.Graphics;
using SFML.System;

namespace TennisGame.Game
{
    public class Ball : GameObject
    {
        public float InitialSpeed { get; set; } = 400f;
        public float SpeedMultiplier { get; set; } = 1.0f;
        public int SpeedBoostCount { get; set; } = 0;
        public const int MaxSpeedBoosts = 3;

        public Ball(Vector2f position, Vector2f size, Color color) 
            : base(position, size, color)
        {
            Reset();
        }

        public void Reset()
        {
            SpeedMultiplier = 1.0f;
            SpeedBoostCount = 0;
            
            // Rastgele yön
            var random = new System.Random();
            float angle = (float)(random.NextDouble() * 0.5f - 0.25f); // -0.25 to 0.25 radians
            
            Velocity = new Vector2f(
                InitialSpeed * (random.Next(2) == 0 ? 1 : -1),
                InitialSpeed * angle
            );
        }

        public void SetSpeed(float speed)
        {
            InitialSpeed = speed;
        }

        public override void Update(float deltaTime)
        {
            Position += Velocity * SpeedMultiplier * deltaTime;
        }

        public override void Draw(RenderWindow window)
        {
            var shape = new CircleShape(Size.X / 2)
            {
                Position = Position,
                FillColor = Color
            };
            window.Draw(shape);
        }

        public void HandleWallCollision(float windowHeight)
        {
            if (Position.Y <= 0 || Position.Y + Size.Y >= windowHeight)
            {
                Velocity = new Vector2f(Velocity.X, -Velocity.Y);
            }
        }

        public void HandlePaddleCollision(Paddle paddle)
        {
            if (paddle.IsInRedZone(Position))
            {
                // Kırmızı bölgeye çarptı - hızlan
                if (SpeedBoostCount < MaxSpeedBoosts)
                {
                    SpeedBoostCount++;
                    SpeedMultiplier = 1.0f + (SpeedBoostCount * 0.5f); // Her çarpışmada %50 hızlan
                }
            }
            else
            {
                // Normal bölgeye çarptı - hafif hızlan
                SpeedMultiplier += 0.1f;
            }

            // Yön değiştir
            Velocity = new Vector2f(-Velocity.X, Velocity.Y);
        }

        public bool IsOutOfBounds(float windowWidth)
        {
            return Position.X < 0 || Position.X > windowWidth;
        }

        public int GetScoringPlayer(float windowWidth)
        {
            if (Position.X < 0) return 2; // Player 2 scores
            if (Position.X > windowWidth) return 1; // Player 1 scores
            return 0; // No score
        }
    }
} 