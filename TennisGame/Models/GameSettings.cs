using System;

namespace TennisGame.Models
{
    public class GameSettings
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BallSpeed { get; set; } = 5; // 0-10
        public int PaddleSize { get; set; } = 5; // 0-10
        public int WinningScore { get; set; } = 5; // 0-10
        public int PaddleSpeed { get; set; } = 5; // 0-10
        public string VisualTheme { get; set; } = "Classic"; // Classic, Dark, Pink
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsActive { get; set; } = true;
    }
} 