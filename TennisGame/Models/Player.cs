using System;

namespace TennisGame.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public int TotalWins { get; set; }
        public int TotalMatches { get; set; }
        public double WinRate => TotalMatches > 0 ? (double)TotalWins / TotalMatches * 100 : 0;
    }
} 