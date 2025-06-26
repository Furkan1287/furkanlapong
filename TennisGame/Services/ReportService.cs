using System;
using System.Collections.Generic;
using System.Linq;
using TennisGame.Models;

namespace TennisGame.Services
{
    public class ReportService
    {
        private readonly DatabaseService _databaseService;
        private readonly SettingsService _settingsService;

        public ReportService(DatabaseService databaseService, SettingsService settingsService)
        {
            _databaseService = databaseService;
            _settingsService = settingsService;
        }

        public void ShowMainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                    FURKANLA PONG MENÜSÜ                    ║");
                Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
                Console.WriteLine("║ 1. Oyuncu İstatistikleri                                   ║");
                Console.WriteLine("║ 2. Son Maçlar                                             ║");
                Console.WriteLine("║ 3. Oyuncu Maç Geçmişi                                     ║");
                Console.WriteLine("║ 4. En İyi Oyuncular                                       ║");
                Console.WriteLine("║ 5. AI Maç İstatistikleri                                  ║");
                Console.WriteLine("║ 6. Oyuncu Yönetimi                                        ║");
                Console.WriteLine("║ 7. Maç Yönetimi                                           ║");
                Console.WriteLine("║ 8. Oyun Ayarları                                          ║");
                Console.WriteLine("║ 0. Ana Menüye Dön                                         ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
                Console.Write("\nSeçiminizi yapın (0-8): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ShowPlayerStats();
                        break;
                    case "2":
                        ShowRecentMatches();
                        break;
                    case "3":
                        ShowPlayerMatchHistory();
                        break;
                    case "4":
                        ShowTopPlayers();
                        break;
                    case "5":
                        ShowAIMatchStats();
                        break;
                    case "6":
                        ShowPlayerManagement();
                        break;
                    case "7":
                        ShowMatchManagement();
                        break;
                    case "8":
                        _settingsService.ShowSettingsMenu();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Geçersiz seçim!");
                        break;
                }

                if (choice != "0" && choice != "8")
                {
                    Console.WriteLine("\nDevam etmek için bir tuşa basın...");
                    Console.ReadKey();
                }
            }
        }

        private void ShowPlayerStats()
        {
            Console.Clear();
            Console.WriteLine("=== OYUNCU İSTATİSTİKLERİ ===\n");

            var players = _databaseService.GetPlayerStats();

            if (!players.Any())
            {
                Console.WriteLine("Henüz oyuncu bulunmuyor.");
                return;
            }

            Console.WriteLine($"{"ID",-5} {"İsim",-20} {"Maç",-8} {"Galibiyet",-10} {"Kazanma %",-12}");
            Console.WriteLine(new string('-', 60));

            foreach (var player in players)
            {
                Console.WriteLine($"{player.Id,-5} {player.Name,-20} {player.TotalMatches,-8} {player.TotalWins,-10} {player.WinRate:F1}%");
            }
        }

        private void ShowRecentMatches()
        {
            Console.Clear();
            Console.WriteLine("=== SON MAÇLAR ===\n");

            Console.Write("Kaç maç gösterilsin? (varsayılan: 10): ");
            string input = Console.ReadLine();
            int count = string.IsNullOrEmpty(input) ? 10 : int.Parse(input);

            var matches = _databaseService.GetRecentMatches(count);

            if (!matches.Any())
            {
                Console.WriteLine("Henüz maç bulunmuyor.");
                return;
            }

            Console.WriteLine($"{"Tarih",-20} {"Oyuncu 1",-15} {"Skor",-10} {"Oyuncu 2",-15} {"Kazanan",-15} {"AI",-5}");
            Console.WriteLine(new string('-', 85));

            foreach (var match in matches)
            {
                string score = $"{match.Player1Score} - {match.Player2Score}";
                string aiMode = match.IsAIMode ? "Evet" : "Hayır";
                Console.WriteLine($"{match.MatchDate:dd.MM.yyyy HH:mm,-20} {match.Player1.Name,-15} {score,-10} {match.Player2.Name,-15} {match.Winner.Name,-15} {aiMode,-5}");
            }
        }

        private void ShowPlayerMatchHistory()
        {
            Console.Clear();
            Console.WriteLine("=== OYUNCU MAÇ GEÇMİŞİ ===\n");

            var players = _databaseService.GetAllPlayers();
            if (!players.Any())
            {
                Console.WriteLine("Henüz oyuncu bulunmuyor.");
                return;
            }

            Console.WriteLine("Mevcut oyuncular:");
            foreach (var player in players)
            {
                Console.WriteLine($"{player.Id}. {player.Name}");
            }

            Console.Write("\nOyuncu ID'si girin: ");
            if (!int.TryParse(Console.ReadLine(), out int playerId))
            {
                Console.WriteLine("Geçersiz ID!");
                return;
            }

            var matches = _databaseService.GetMatchesByPlayer(playerId);
            var selectedPlayer = players.FirstOrDefault(p => p.Id == playerId);

            if (selectedPlayer == null)
            {
                Console.WriteLine("Oyuncu bulunamadı!");
                return;
            }

            Console.WriteLine($"\n=== {selectedPlayer.Name} MAÇ GEÇMİŞİ ===\n");

            if (!matches.Any())
            {
                Console.WriteLine("Bu oyuncunun henüz maçı bulunmuyor.");
                return;
            }

            Console.WriteLine($"{"Tarih",-20} {"Rakip",-15} {"Skor",-10} {"Sonuç",-10} {"Süre",-8}");
            Console.WriteLine(new string('-', 70));

            foreach (var match in matches)
            {
                string opponent = match.Player1Id == playerId ? match.Player2.Name : match.Player1.Name;
                string score = match.Player1Id == playerId ? $"{match.Player1Score} - {match.Player2Score}" : $"{match.Player2Score} - {match.Player1Score}";
                string result = match.WinnerId == playerId ? "Kazandı" : "Kaybetti";
                string duration = $"{match.Duration}s";

                Console.WriteLine($"{match.MatchDate:dd.MM.yyyy HH:mm,-20} {opponent,-15} {score,-10} {result,-10} {duration,-8}");
            }
        }

        private void ShowTopPlayers()
        {
            Console.Clear();
            Console.WriteLine("=== EN İYİ OYUNCULAR ===\n");

            var players = _databaseService.GetPlayerStats()
                .Where(p => p.TotalMatches > 0)
                .OrderByDescending(p => p.WinRate)
                .ThenByDescending(p => p.TotalWins)
                .Take(10)
                .ToList();

            if (!players.Any())
            {
                Console.WriteLine("Henüz oyuncu bulunmuyor.");
                return;
            }

            Console.WriteLine($"{"Sıra",-5} {"İsim",-20} {"Kazanma %",-12} {"Galibiyet",-10} {"Toplam Maç",-12}");
            Console.WriteLine(new string('-', 65));

            for (int i = 0; i < players.Count; i++)
            {
                var player = players[i];
                Console.WriteLine($"{i + 1,-5} {player.Name,-20} {player.WinRate:F1}%{-8} {player.TotalWins,-10} {player.TotalMatches,-12}");
            }
        }

        private void ShowAIMatchStats()
        {
            Console.Clear();
            Console.WriteLine("=== AI MAÇ İSTATİSTİKLERİ ===\n");

            var matches = _databaseService.GetAllMatches().Where(m => m.IsAIMode).ToList();

            if (!matches.Any())
            {
                Console.WriteLine("Henüz AI maçı bulunmuyor.");
                return;
            }

            int totalAIMatches = matches.Count;
            int aiWins = matches.Count(m => m.WinnerId == m.Player2Id); // AI her zaman Player2
            double aiWinRate = (double)aiWins / totalAIMatches * 100;

            Console.WriteLine($"Toplam AI Maçı: {totalAIMatches}");
            Console.WriteLine($"AI Galibiyetleri: {aiWins}");
            Console.WriteLine($"AI Kazanma Oranı: {aiWinRate:F1}%");

            Console.WriteLine("\nSon 10 AI Maçı:");
            Console.WriteLine($"{"Tarih",-20} {"İnsan Oyuncu",-15} {"Skor",-10} {"Sonuç",-10}");
            Console.WriteLine(new string('-', 60));

            foreach (var match in matches.Take(10))
            {
                string result = match.WinnerId == match.Player2Id ? "AI Kazandı" : "İnsan Kazandı";
                Console.WriteLine($"{match.MatchDate:dd.MM.yyyy HH:mm,-20} {match.Player1.Name,-15} {match.Player1Score}-{match.Player2Score,-6} {result,-10}");
            }
        }

        private void ShowPlayerManagement()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== OYUNCU YÖNETİMİ ===\n");
                Console.WriteLine("1. Tüm Oyuncuları Listele");
                Console.WriteLine("2. Oyuncu Ekle");
                Console.WriteLine("3. Oyuncu Güncelle");
                Console.WriteLine("4. Oyuncu Sil");
                Console.WriteLine("0. Geri Dön");

                Console.Write("Seçiminiz: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ListAllPlayers();
                        break;
                    case "2":
                        AddPlayer();
                        break;
                    case "3":
                        UpdatePlayer();
                        break;
                    case "4":
                        DeletePlayer();
                        break;
                    case "0":
                        return;
                }

                Console.WriteLine("\nDevam etmek için bir tuşa basın...");
                Console.ReadKey();
            }
        }

        private void ShowMatchManagement()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== MAÇ YÖNETİMİ ===\n");
                Console.WriteLine("1. Tüm Maçları Listele");
                Console.WriteLine("2. Maç Sil");
                Console.WriteLine("0. Geri Dön");

                Console.Write("Seçiminiz: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ListAllMatches();
                        break;
                    case "2":
                        DeleteMatch();
                        break;
                    case "0":
                        return;
                }

                Console.WriteLine("\nDevam etmek için bir tuşa basın...");
                Console.ReadKey();
            }
        }

        private void ListAllPlayers()
        {
            Console.Clear();
            Console.WriteLine("=== TÜM OYUNCULAR ===\n");

            var players = _databaseService.GetAllPlayers();
            if (!players.Any())
            {
                Console.WriteLine("Henüz oyuncu bulunmuyor.");
                return;
            }

            Console.WriteLine($"{"ID",-5} {"İsim",-20} {"Kayıt Tarihi",-15}");
            Console.WriteLine(new string('-', 45));

            foreach (var player in players)
            {
                Console.WriteLine($"{player.Id,-5} {player.Name,-20} {player.CreatedDate:dd.MM.yyyy,-15}");
            }
        }

        private void AddPlayer()
        {
            Console.Clear();
            Console.WriteLine("=== OYUNCU EKLE ===\n");

            Console.Write("Oyuncu adı: ");
            string name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Oyuncu adı boş olamaz!");
                return;
            }

            try
            {
                int id = _databaseService.CreatePlayer(name);
                Console.WriteLine($"Oyuncu başarıyla eklendi. ID: {id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
            }
        }

        private void UpdatePlayer()
        {
            Console.Clear();
            Console.WriteLine("=== OYUNCU GÜNCELLE ===\n");

            Console.Write("Oyuncu ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Geçersiz ID!");
                return;
            }

            var player = _databaseService.GetPlayer(id);
            if (player == null)
            {
                Console.WriteLine("Oyuncu bulunamadı!");
                return;
            }

            Console.WriteLine($"Mevcut ad: {player.Name}");
            Console.Write("Yeni ad: ");
            string newName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(newName))
            {
                Console.WriteLine("Ad boş olamaz!");
                return;
            }

            try
            {
                player.Name = newName;
                _databaseService.UpdatePlayer(player);
                Console.WriteLine("Oyuncu başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
            }
        }

        private void DeletePlayer()
        {
            Console.Clear();
            Console.WriteLine("=== OYUNCU SİL ===\n");

            Console.Write("Oyuncu ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Geçersiz ID!");
                return;
            }

            var player = _databaseService.GetPlayer(id);
            if (player == null)
            {
                Console.WriteLine("Oyuncu bulunamadı!");
                return;
            }

            Console.WriteLine($"Oyuncu: {player.Name}");
            Console.Write("Silmek istediğinizden emin misiniz? (E/H): ");
            string confirm = Console.ReadLine();

            if (confirm.ToUpper() == "E")
            {
                try
                {
                    _databaseService.DeletePlayer(id);
                    Console.WriteLine("Oyuncu başarıyla silindi.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Hata: {ex.Message}");
                }
            }
        }

        private void ListAllMatches()
        {
            Console.Clear();
            Console.WriteLine("=== TÜM MAÇLAR ===\n");

            var matches = _databaseService.GetAllMatches();
            if (!matches.Any())
            {
                Console.WriteLine("Henüz maç bulunmuyor.");
                return;
            }

            Console.WriteLine($"{"ID",-5} {"Tarih",-20} {"Oyuncu 1",-15} {"Skor",-10} {"Oyuncu 2",-15} {"Kazanan",-15}");
            Console.WriteLine(new string('-', 85));

            foreach (var match in matches)
            {
                string score = $"{match.Player1Score} - {match.Player2Score}";
                Console.WriteLine($"{match.Id,-5} {match.MatchDate:dd.MM.yyyy HH:mm,-20} {match.Player1.Name,-15} {score,-10} {match.Player2.Name,-15} {match.Winner.Name,-15}");
            }
        }

        private void DeleteMatch()
        {
            Console.Clear();
            Console.WriteLine("=== MAÇ SİL ===\n");

            Console.Write("Maç ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Geçersiz ID!");
                return;
            }

            var match = _databaseService.GetMatch(id);
            if (match == null)
            {
                Console.WriteLine("Maç bulunamadı!");
                return;
            }

            Console.WriteLine($"Maç: {match.Player1.Name} vs {match.Player2.Name} ({match.Player1Score}-{match.Player2Score})");
            Console.Write("Silmek istediğinizden emin misiniz? (E/H): ");
            string confirm = Console.ReadLine();

            if (confirm.ToUpper() == "E")
            {
                try
                {
                    _databaseService.DeleteMatch(id);
                    Console.WriteLine("Maç başarıyla silindi.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Hata: {ex.Message}");
                }
            }
        }
    }
} 