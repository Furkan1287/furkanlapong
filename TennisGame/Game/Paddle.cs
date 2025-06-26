using SFML.Graphics;
using SFML.System;

namespace TennisGame.Game
{
    public class Paddle : GameObject
    {
        public bool IsAI { get; set; }
        public float Speed { get; set; } = 500f;
        public float RedZoneHeight { get; set; } = 20f; // Kırmızı bölge yüksekliği

        public Paddle(Vector2f position, Vector2f size, Color color, bool isAI = false) 
            : base(position, size, color)
        {
            IsAI = isAI;
        }

        public override void Draw(RenderWindow window)
        {
            // Ana çubuk
            var mainShape = new RectangleShape(Size)
            {
                Position = Position,
                FillColor = Color
            };
            window.Draw(mainShape);

            // Kırmızı köşe bölgeleri
            var redZone = new RectangleShape(new Vector2f(Size.X, RedZoneHeight))
            {
                Position = Position,
                FillColor = Color.Red
            };
            window.Draw(redZone);

            var redZoneBottom = new RectangleShape(new Vector2f(Size.X, RedZoneHeight))
            {
                Position = new Vector2f(Position.X, Position.Y + Size.Y - RedZoneHeight),
                FillColor = Color.Red
            };
            window.Draw(redZoneBottom);
        }

        public bool IsInRedZone(Vector2f ballPosition)
        {
            return ballPosition.Y >= Position.Y && ballPosition.Y <= Position.Y + RedZoneHeight ||
                   ballPosition.Y >= Position.Y + Size.Y - RedZoneHeight && ballPosition.Y <= Position.Y + Size.Y;
        }

        public void MoveUp(float deltaTime)
        {
            if (Position.Y > 0)
            {
                Position = new Vector2f(Position.X, Position.Y - Speed * deltaTime);
            }
        }

        public void MoveDown(float deltaTime, float windowHeight)
        {
            if (Position.Y + Size.Y < windowHeight)
            {
                Position = new Vector2f(Position.X, Position.Y + Speed * deltaTime);
            }
        }

        public void UpdateAI(float deltaTime, Vector2f ballPosition, float windowHeight)
        {
            if (!IsAI) return;

            // Basit AI: Topa doğru hareket et
            float paddleCenter = Position.Y + Size.Y / 2;
            float ballCenter = ballPosition.Y;

            if (ballCenter < paddleCenter - 10)
            {
                MoveUp(deltaTime);
            }
            else if (ballCenter > paddleCenter + 10)
            {
                MoveDown(deltaTime, windowHeight);
            }
        }
    }
} 