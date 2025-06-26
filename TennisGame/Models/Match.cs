using System;

namespace TennisGame.Models
{
    public class Match
    {
        public int Id { get; set; }
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public int WinnerId { get; set; }
        public DateTime MatchDate { get; set; }
        public int Duration { get; set; } // saniye cinsinden
        public bool IsAIMode { get; set; }
        
        // Navigation properties
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Player Winner { get; set; }
    }
} 