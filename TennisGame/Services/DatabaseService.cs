using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Microsoft.Data.Sqlite;
using TennisGame.Models;

namespace TennisGame.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(string connectionString)
        {
            _connectionString = connectionString;
            InitializeDatabase();
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Players tablosu
            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS Players (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP
                )");

            // Matches tablosu
            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS Matches (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Player1Id INTEGER,
                    Player2Id INTEGER,
                    Player1Score INTEGER NOT NULL,
                    Player2Score INTEGER NOT NULL,
                    WinnerId INTEGER,
                    MatchDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    Duration INTEGER DEFAULT 0,
                    IsAIMode INTEGER DEFAULT 0,
                    FOREIGN KEY(Player1Id) REFERENCES Players(Id),
                    FOREIGN KEY(Player2Id) REFERENCES Players(Id),
                    FOREIGN KEY(WinnerId) REFERENCES Players(Id)
                )
            ");

            // GameSettings tablosu
            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS GameSettings (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    BallSpeed INTEGER DEFAULT 5,
                    PaddleSize INTEGER DEFAULT 5,
                    WinningScore INTEGER DEFAULT 5,
                    PaddleSpeed INTEGER DEFAULT 5,
                    VisualTheme TEXT DEFAULT 'Classic',
                    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                    LastModified DATETIME DEFAULT CURRENT_TIMESTAMP,
                    IsActive INTEGER DEFAULT 1
                )
            ");

            // Varsayılan ayarları ekle
            var defaultSettings = connection.QueryFirstOrDefault<GameSettings>(
                "SELECT * FROM GameSettings WHERE Name = 'Default'");
            
            if (defaultSettings == null)
            {
                connection.Execute(@"
                    INSERT INTO GameSettings (Name, BallSpeed, PaddleSize, WinningScore, PaddleSpeed, VisualTheme)
                    VALUES ('Default', 5, 5, 5, 5, 'Classic')
                ");
            }
        }

        // CRUD Operations for Players
        public int CreatePlayer(string name)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute("INSERT INTO Players (Name) VALUES (@Name)", new { Name = name });
            return connection.ExecuteScalar<int>("SELECT last_insert_rowid()", null);
        }

        public Player GetPlayer(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            return connection.QueryFirstOrDefault<Player>(
                "SELECT * FROM Players WHERE Id = @Id",
                new { Id = id });
        }

        public Player GetPlayerByName(string name)
        {
            using var connection = new SqliteConnection(_connectionString);
            return connection.QueryFirstOrDefault<Player>(
                "SELECT * FROM Players WHERE Name = @Name",
                new { Name = name });
        }

        public List<Player> GetAllPlayers()
        {
            using var connection = new SqliteConnection(_connectionString);
            return connection.Query<Player>("SELECT * FROM Players ORDER BY Name").ToList();
        }

        public void UpdatePlayer(Player player)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute(
                "UPDATE Players SET Name = @Name WHERE Id = @Id",
                player);
        }

        public void DeletePlayer(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute("DELETE FROM Players WHERE Id = @Id", new { Id = id });
        }

        // CRUD Operations for Matches
        public int CreateMatch(Match match)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute(@"
                INSERT INTO Matches (Player1Id, Player2Id, Player1Score, Player2Score, WinnerId, Duration, IsAIMode)
                VALUES (@Player1Id, @Player2Id, @Player1Score, @Player2Score, @WinnerId, @Duration, @IsAIMode)",
                match);
            return connection.ExecuteScalar<int>("SELECT last_insert_rowid()", null);
        }

        public Match GetMatch(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            var match = connection.QueryFirstOrDefault<Match>(
                "SELECT * FROM Matches WHERE Id = @Id",
                new { Id = id });

            if (match != null)
            {
                match.Player1 = GetPlayer(match.Player1Id);
                match.Player2 = GetPlayer(match.Player2Id);
                match.Winner = GetPlayer(match.WinnerId);
            }

            return match;
        }

        public List<Match> GetAllMatches()
        {
            using var connection = new SqliteConnection(_connectionString);
            var matches = connection.Query<Match>("SELECT * FROM Matches ORDER BY MatchDate DESC").ToList();

            foreach (var match in matches)
            {
                match.Player1 = GetPlayer(match.Player1Id);
                match.Player2 = GetPlayer(match.Player2Id);
                match.Winner = GetPlayer(match.WinnerId);
            }

            return matches;
        }

        public void DeleteMatch(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute("DELETE FROM Matches WHERE Id = @Id", new { Id = id });
        }

        // Reporting Methods
        public List<Player> GetPlayerStats()
        {
            using var connection = new SqliteConnection(_connectionString);
            return connection.Query<Player>(@"
                SELECT 
                    p.Id,
                    p.Name,
                    p.CreatedDate,
                    COUNT(CASE WHEN m.WinnerId = p.Id THEN 1 END) as TotalWins,
                    COUNT(m.Id) as TotalMatches
                FROM Players p
                LEFT JOIN Matches m ON (p.Id = m.Player1Id OR p.Id = m.Player2Id)
                GROUP BY p.Id, p.Name, p.CreatedDate
                ORDER BY TotalWins DESC, TotalMatches DESC").ToList();
        }

        public List<Match> GetRecentMatches(int count = 10)
        {
            using var connection = new SqliteConnection(_connectionString);
            var matches = connection.Query<Match>(
                "SELECT * FROM Matches ORDER BY MatchDate DESC LIMIT @Count",
                new { Count = count }).ToList();

            foreach (var match in matches)
            {
                match.Player1 = GetPlayer(match.Player1Id);
                match.Player2 = GetPlayer(match.Player2Id);
                match.Winner = GetPlayer(match.WinnerId);
            }

            return matches;
        }

        public List<Match> GetMatchesByPlayer(int playerId)
        {
            using var connection = new SqliteConnection(_connectionString);
            var matches = connection.Query<Match>(
                "SELECT * FROM Matches WHERE Player1Id = @PlayerId OR Player2Id = @PlayerId ORDER BY MatchDate DESC",
                new { PlayerId = playerId }).ToList();

            foreach (var match in matches)
            {
                match.Player1 = GetPlayer(match.Player1Id);
                match.Player2 = GetPlayer(match.Player2Id);
                match.Winner = GetPlayer(match.WinnerId);
            }

            return matches;
        }

        public int GetOrCreatePlayer(string name)
        {
            var player = GetPlayerByName(name);
            if (player != null) return player.Id;

            return CreatePlayer(name);
        }

        // GameSettings CRUD Operations
        public int CreateGameSettings(GameSettings settings)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute(@"
                INSERT INTO GameSettings (Name, BallSpeed, PaddleSize, WinningScore, PaddleSpeed, VisualTheme, CreatedDate, LastModified, IsActive)
                VALUES (@Name, @BallSpeed, @PaddleSize, @WinningScore, @PaddleSpeed, @VisualTheme, @CreatedDate, @LastModified, @IsActive)",
                settings);
            return connection.ExecuteScalar<int>("SELECT last_insert_rowid()", null);
        }

        public GameSettings GetGameSettings(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            return connection.QueryFirstOrDefault<GameSettings>(
                "SELECT * FROM GameSettings WHERE Id = @Id",
                new { Id = id });
        }

        public GameSettings GetActiveGameSettings()
        {
            using var connection = new SqliteConnection(_connectionString);
            return connection.QueryFirstOrDefault<GameSettings>(
                "SELECT * FROM GameSettings WHERE IsActive = 1 ORDER BY LastModified DESC LIMIT 1");
        }

        public List<GameSettings> GetAllGameSettings()
        {
            using var connection = new SqliteConnection(_connectionString);
            return connection.Query<GameSettings>("SELECT * FROM GameSettings ORDER BY Name").ToList();
        }

        public void UpdateGameSettings(GameSettings settings)
        {
            using var connection = new SqliteConnection(_connectionString);
            settings.LastModified = DateTime.Now;
            connection.Execute(@"
                UPDATE GameSettings 
                SET Name = @Name, BallSpeed = @BallSpeed, PaddleSize = @PaddleSize, 
                    WinningScore = @WinningScore, PaddleSpeed = @PaddleSpeed, 
                    VisualTheme = @VisualTheme, LastModified = @LastModified, IsActive = @IsActive 
                WHERE Id = @Id",
                settings);
        }

        public void DeleteGameSettings(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute("DELETE FROM GameSettings WHERE Id = @Id", new { Id = id });
        }

        public void SetActiveGameSettings(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            // Tüm ayarları pasif yap
            connection.Execute("UPDATE GameSettings SET IsActive = 0");
            // Seçilen ayarı aktif yap
            connection.Execute("UPDATE GameSettings SET IsActive = 1, LastModified = @LastModified WHERE Id = @Id", 
                new { Id = id, LastModified = DateTime.Now });
        }
    }
} 