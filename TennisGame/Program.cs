using System;
using TennisGame.Game;
using TennisGame.Services;

namespace TennisGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== FURKANLA PONG ===");
            Console.WriteLine("Dapper ve .NET Core ile geliştirilmiştir\n");
            Console.WriteLine("Furkan'ın özel Pong oyununa hoş geldiniz!\n");

            // Veritabanı bağlantı stringi
            string connectionString = GetConnectionString();

            try
            {
                var databaseService = new DatabaseService(connectionString);
                var settingsService = new SettingsService(databaseService);
                var reportService = new ReportService(databaseService, settingsService);

                ShowMainMenu(databaseService, reportService, settingsService);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veritabanı bağlantı hatası: {ex.Message}");
                Console.WriteLine("Lütfen SQL Server bağlantı ayarlarını kontrol edin.");
                Console.WriteLine("Devam etmek için bir tuşa basın...");
                Console.ReadKey();
            }
        }

        private static string GetConnectionString()
        {
            Console.WriteLine("SQLite veritabanı dosya adı (varsayılan: tennis.db): ");
            string dbFile = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(dbFile)) dbFile = "tennis.db";
            return $"Data Source={dbFile}";
        }

        private static void ShowMainMenu(DatabaseService databaseService, ReportService reportService, SettingsService settingsService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                    FURKANLA PONG ANA MENÜ                   ║");
                Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
                Console.WriteLine("║ 1. İki Kişilik Oyun                                        ║");
                Console.WriteLine("║ 2. AI ile Oyun                                             ║");
                Console.WriteLine("║ 3. Raporlar ve İstatistikler                               ║");
                Console.WriteLine("║ 4. Oyun Ayarları                                           ║");
                Console.WriteLine("║ 5. Yardım                                                  ║");
                Console.WriteLine("║ 0. Çıkış                                                   ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
                Console.Write("\nSeçiminizi yapın (0-5): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        StartTwoPlayerGame(databaseService);
                        break;
                    case "2":
                        StartAIGame(databaseService);
                        break;
                    case "3":
                        reportService.ShowMainMenu();
                        break;
                    case "4":
                        settingsService.ShowSettingsMenu();
                        break;
                    case "5":
                        ShowHelp();
                        break;
                    case "0":
                        Console.WriteLine("Oyundan çıkılıyor...");
                        return;
                    default:
                        Console.WriteLine("Geçersiz seçim!");
                        break;
                }

                if (choice != "0" && choice != "4")
                {
                    Console.WriteLine("\nAna menüye dönmek için bir tuşa basın...");
                    Console.ReadKey();
                }
            }
        }

        private static void StartTwoPlayerGame(DatabaseService databaseService)
        {
            Console.Clear();
            Console.WriteLine("=== İKİ KİŞİLİK OYUN ===\n");

            Console.Write("1. Oyuncu adı: ");
            string player1Name = Console.ReadLine();
            if (string.IsNullOrEmpty(player1Name)) player1Name = "Oyuncu1";

            Console.Write("2. Oyuncu adı: ");
            string player2Name = Console.ReadLine();
            if (string.IsNullOrEmpty(player2Name)) player2Name = "Oyuncu2";

            Console.WriteLine("\nOyun başlatılıyor...");
            Console.WriteLine("Kontroller:");
            Console.WriteLine("Oyuncu 1: W (yukarı), S (aşağı)");
            Console.WriteLine("Oyuncu 2: Yukarı Ok, Aşağı Ok");
            Console.WriteLine("P: Duraklat/Devam Et");
            Console.WriteLine("ESC: Çıkış");
            Console.WriteLine("\nDevam etmek için bir tuşa basın...");
            Console.ReadKey();

            try
            {
                var game = new Game.TennisGame(databaseService.GetConnectionString(), player1Name, player2Name, false);
                game.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Oyun hatası: {ex.Message}");
            }
        }

        private static void StartAIGame(DatabaseService databaseService)
        {
            Console.Clear();
            Console.WriteLine("=== AI İLE OYUN ===\n");

            Console.Write("Oyuncu adı: ");
            string playerName = Console.ReadLine();
            if (string.IsNullOrEmpty(playerName)) playerName = "Oyuncu";

            Console.WriteLine("\nOyun başlatılıyor...");
            Console.WriteLine("Kontroller:");
            Console.WriteLine("Oyuncu: W (yukarı), S (aşağı)");
            Console.WriteLine("P: Duraklat/Devam Et");
            Console.WriteLine("ESC: Çıkış");
            Console.WriteLine("\nDevam etmek için bir tuşa basın...");
            Console.ReadKey();

            try
            {
                var game = new Game.TennisGame(databaseService.GetConnectionString(), playerName, "AI", true);
                game.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Oyun hatası: {ex.Message}");
            }
        }

        private static void ShowGameSettings()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    OYUN AYARLARI                            ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");
            Console.WriteLine("✅ Mevcut özellikler:");
            Console.WriteLine("• Top hızı ayarı (0-10)");
            Console.WriteLine("• Çubuk boyutu ayarı (0-10)");
            Console.WriteLine("• Kazanma skoru ayarı (0-10)");
            Console.WriteLine("• Çubuk hızı ayarı (0-10)");
            Console.WriteLine("• Görsel temalar:");
            Console.WriteLine("  - Classic (Klasik)");
            Console.WriteLine("  - Dark (Karanlık Mod)");
            Console.WriteLine("  - Pink (Pembe Mod - Hello Kitty)");
            Console.WriteLine("\nAna menüden 'Oyun Ayarları' seçeneğini kullanın!");
        }

        private static void ShowHelp()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    FURKANLA PONG YARDIM                     ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");
            Console.WriteLine("Furkan'ın özel Pong oyununa hoş geldiniz!");
            Console.WriteLine("Bu oyun, klasik Pong'un gelişmiş versiyonudur.\n");
            
            Console.WriteLine("OYUN KURALLARI:");
            Console.WriteLine("- İlk belirlenen puana ulaşan oyuncu kazanır (varsayılan: 8)");
            Console.WriteLine("- Top çubuklara çarptığında yön değiştirir");
            Console.WriteLine("- Kırmızı bölgeye çarpan top 3 kez hızlanır");
            Console.WriteLine("- Yıldızları toplayarak ekstra hız kazanın!");
            Console.WriteLine("- Topu kaçıran rakibe puan kazandırır");
            Console.WriteLine();
            Console.WriteLine("KONTROLLER:");
            Console.WriteLine("Oyuncu 1: W (yukarı), S (aşağı)");
            Console.WriteLine("Oyuncu 2: Yukarı Ok, Aşağı Ok");
            Console.WriteLine("P: Oyunu duraklat/devam et");
            Console.WriteLine("ESC: Oyundan çık");
            Console.WriteLine();
            Console.WriteLine("OYUN AYARLARI:");
            Console.WriteLine("- Top hızı: 0-10 arası ayarlanabilir");
            Console.WriteLine("- Çubuk boyutu: 0-10 arası ayarlanabilir");
            Console.WriteLine("- Kazanma skoru: 0-10 arası ayarlanabilir (3-15 puan)");
            Console.WriteLine("- Çubuk hızı: 0-10 arası ayarlanabilir");
            Console.WriteLine("- Görsel temalar:");
            Console.WriteLine("  • Classic: Klasik siyah-beyaz tema");
            Console.WriteLine("  • Dark: Karanlık mod");
            Console.WriteLine("  • Pink: Pembe tema (Hello Kitty elementleri)");
            Console.WriteLine();
            Console.WriteLine("ÖZELLİKLER:");
            Console.WriteLine("- Yapay zeka modu");
            Console.WriteLine("- Kırmızı hızlanma bölgeleri");
            Console.WriteLine("- Yıldız bonus sistemi");
            Console.WriteLine("- Veritabanı kayıt sistemi (Dapper ile)");
            Console.WriteLine("- Detaylı raporlama");
            Console.WriteLine("- CRUD işlemleri");
            Console.WriteLine("- Ayar profilleri");
            Console.WriteLine("- Görsel tema sistemi");
            Console.WriteLine();
            Console.WriteLine("Furkan'ın özel dokunuşları:");
            Console.WriteLine("- Dinamik hız sistemi");
            Console.WriteLine("- Yıldız toplama bonusu");
            Console.WriteLine("- Gelişmiş AI");
            Console.WriteLine("- SQLite veritabanı entegrasyonu");
            Console.WriteLine("- Hello Kitty temalı pembe mod");
            Console.WriteLine("- Karanlık mod desteği");
            Console.WriteLine("- Özelleştirilebilir oyun ayarları");
        }
    }
}
