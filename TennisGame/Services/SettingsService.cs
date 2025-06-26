using System;
using System.Collections.Generic;
using System.Linq;
using TennisGame.Models;

namespace TennisGame.Services
{
    public class SettingsService
    {
        private readonly DatabaseService _databaseService;

        public SettingsService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public void ShowSettingsMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                    FURKANLA PONG AYARLAR                    ║");
                Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
                Console.WriteLine("║ 1. Mevcut Ayarları Görüntüle                                 ║");
                Console.WriteLine("║ 2. Yeni Ayar Profili Oluştur                                 ║");
                Console.WriteLine("║ 3. Ayar Profili Düzenle                                      ║");
                Console.WriteLine("║ 4. Ayar Profili Sil                                          ║");
                Console.WriteLine("║ 5. Ayar Profili Seç                                          ║");
                Console.WriteLine("║ 6. Ana Menüye Dön                                            ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
                Console.Write("\nSeçiminizi yapın (1-6): ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DisplayCurrentSettings();
                        break;
                    case "2":
                        CreateNewSettings();
                        break;
                    case "3":
                        EditSettings();
                        break;
                    case "4":
                        DeleteSettings();
                        break;
                    case "5":
                        SelectSettings();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Geçersiz seçim! Tekrar deneyin.");
                        break;
                }

                if (choice != "6")
                {
                    Console.WriteLine("\nDevam etmek için bir tuşa basın...");
                    Console.ReadKey();
                }
            }
        }

        private void DisplayCurrentSettings()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    MEVCUT AYARLAR                           ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

            var activeSettings = _databaseService.GetActiveGameSettings();
            if (activeSettings != null)
            {
                DisplaySettings(activeSettings);
            }
            else
            {
                Console.WriteLine("Aktif ayar bulunamadı!");
            }

            Console.WriteLine("\n══════════════════════════════════════════════════════════════");
            Console.WriteLine("TÜM AYAR PROFİLLERİ:");
            Console.WriteLine("══════════════════════════════════════════════════════════════");

            var allSettings = _databaseService.GetAllGameSettings();
            foreach (var settings in allSettings)
            {
                Console.WriteLine($"\nID: {settings.Id} - {settings.Name} {(settings.IsActive ? "(AKTİF)" : "")}");
                Console.WriteLine($"  Top Hızı: {settings.BallSpeed}/10, Çubuk Boyutu: {settings.PaddleSize}/10");
                Console.WriteLine($"  Kazanma Skoru: {settings.WinningScore}/10, Çubuk Hızı: {settings.PaddleSpeed}/10");
                Console.WriteLine($"  Tema: {settings.VisualTheme}");
            }
        }

        private void DisplaySettings(GameSettings settings)
        {
            Console.WriteLine($"Profil Adı: {settings.Name}");
            Console.WriteLine($"Top Hızı: {settings.BallSpeed}/10");
            Console.WriteLine($"Çubuk Boyutu: {settings.PaddleSize}/10");
            Console.WriteLine($"Kazanma Skoru: {settings.WinningScore}/10");
            Console.WriteLine($"Çubuk Hızı: {settings.PaddleSpeed}/10");
            Console.WriteLine($"Görsel Tema: {settings.VisualTheme}");
            Console.WriteLine($"Oluşturulma: {settings.CreatedDate:dd.MM.yyyy HH:mm}");
            Console.WriteLine($"Son Güncelleme: {settings.LastModified:dd.MM.yyyy HH:mm}");
        }

        private void CreateNewSettings()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    YENİ AYAR PROFİLİ                        ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

            var settings = new GameSettings
            {
                CreatedDate = DateTime.Now,
                LastModified = DateTime.Now
            };

            Console.Write("Profil Adı: ");
            settings.Name = Console.ReadLine() ?? "Yeni Profil";

            settings.BallSpeed = GetIntInput("Top Hızı (0-10)", 5);
            settings.PaddleSize = GetIntInput("Çubuk Boyutu (0-10)", 5);
            settings.WinningScore = GetIntInput("Kazanma Skoru (0-10)", 5);
            settings.PaddleSpeed = GetIntInput("Çubuk Hızı (0-10)", 5);
            settings.VisualTheme = GetThemeChoice();

            _databaseService.CreateGameSettings(settings);
            Console.WriteLine("\n✅ Ayar profili başarıyla oluşturuldu!");
        }

        private void EditSettings()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    AYAR PROFİLİ DÜZENLE                     ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

            var allSettings = _databaseService.GetAllGameSettings();
            if (!allSettings.Any())
            {
                Console.WriteLine("Düzenlenecek ayar profili bulunamadı!");
                return;
            }

            Console.WriteLine("Mevcut Profiller:");
            foreach (var settings in allSettings)
            {
                Console.WriteLine($"{settings.Id}. {settings.Name} {(settings.IsActive ? "(AKTİF)" : "")}");
            }

            Console.Write("\nDüzenlenecek profil ID'si: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Geçersiz ID!");
                return;
            }

            var selectedSettings = _databaseService.GetGameSettings(id);
            if (selectedSettings == null)
            {
                Console.WriteLine("Profil bulunamadı!");
                return;
            }

            Console.WriteLine($"\nDüzenlenen Profil: {selectedSettings.Name}");
            Console.WriteLine("Yeni değerleri girin (boş bırakırsanız mevcut değer korunur):\n");

            Console.Write($"Profil Adı ({selectedSettings.Name}): ");
            var newName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newName))
                selectedSettings.Name = newName;

            selectedSettings.BallSpeed = GetIntInput($"Top Hızı (0-10) [{selectedSettings.BallSpeed}]: ", selectedSettings.BallSpeed);
            selectedSettings.PaddleSize = GetIntInput($"Çubuk Boyutu (0-10) [{selectedSettings.PaddleSize}]: ", selectedSettings.PaddleSize);
            selectedSettings.WinningScore = GetIntInput($"Kazanma Skoru (0-10) [{selectedSettings.WinningScore}]: ", selectedSettings.WinningScore);
            selectedSettings.PaddleSpeed = GetIntInput($"Çubuk Hızı (0-10) [{selectedSettings.PaddleSpeed}]: ", selectedSettings.PaddleSpeed);
            selectedSettings.VisualTheme = GetThemeChoice(selectedSettings.VisualTheme);

            _databaseService.UpdateGameSettings(selectedSettings);
            Console.WriteLine("\n✅ Ayar profili başarıyla güncellendi!");
        }

        private void DeleteSettings()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    AYAR PROFİLİ SİL                         ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

            var allSettings = _databaseService.GetAllGameSettings();
            if (!allSettings.Any())
            {
                Console.WriteLine("Silinecek ayar profili bulunamadı!");
                return;
            }

            Console.WriteLine("Mevcut Profiller:");
            foreach (var settings in allSettings)
            {
                Console.WriteLine($"{settings.Id}. {settings.Name} {(settings.IsActive ? "(AKTİF)" : "")}");
            }

            Console.Write("\nSilinecek profil ID'si: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Geçersiz ID!");
                return;
            }

            var selectedSettings = _databaseService.GetGameSettings(id);
            if (selectedSettings == null)
            {
                Console.WriteLine("Profil bulunamadı!");
                return;
            }

            Console.Write($"\n'{selectedSettings.Name}' profilini silmek istediğinizden emin misiniz? (E/H): ");
            var confirm = Console.ReadLine()?.ToUpper();
            if (confirm == "E" || confirm == "EVET")
            {
                _databaseService.DeleteGameSettings(id);
                Console.WriteLine("✅ Ayar profili başarıyla silindi!");
            }
            else
            {
                Console.WriteLine("Silme işlemi iptal edildi.");
            }
        }

        private void SelectSettings()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    AYAR PROFİLİ SEÇ                         ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

            var allSettings = _databaseService.GetAllGameSettings();
            if (!allSettings.Any())
            {
                Console.WriteLine("Seçilecek ayar profili bulunamadı!");
                return;
            }

            Console.WriteLine("Mevcut Profiller:");
            foreach (var settings in allSettings)
            {
                Console.WriteLine($"{settings.Id}. {settings.Name} {(settings.IsActive ? "(AKTİF)" : "")}");
            }

            Console.Write("\nSeçilecek profil ID'si: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Geçersiz ID!");
                return;
            }

            var selectedSettings = _databaseService.GetGameSettings(id);
            if (selectedSettings == null)
            {
                Console.WriteLine("Profil bulunamadı!");
                return;
            }

            _databaseService.SetActiveGameSettings(id);
            Console.WriteLine($"✅ '{selectedSettings.Name}' profili aktif olarak seçildi!");
        }

        private int GetIntInput(string prompt, int defaultValue)
        {
            while (true)
            {
                Console.Write($"{prompt}: ");
                var input = Console.ReadLine();
                
                if (string.IsNullOrWhiteSpace(input))
                    return defaultValue;

                if (int.TryParse(input, out int value) && value >= 0 && value <= 10)
                    return value;

                Console.WriteLine("Lütfen 0-10 arasında bir değer girin!");
            }
        }

        private string GetThemeChoice(string currentTheme = "Classic")
        {
            Console.WriteLine("\nGörsel Tema Seçenekleri:");
            Console.WriteLine("1. Classic (Klasik)");
            Console.WriteLine("2. Dark (Karanlık Mod)");
            Console.WriteLine("3. Pink (Pembe Mod - Hello Kitty)");
            
            Console.Write($"Tema seçin (1-3) [{currentTheme}]: ");
            var choice = Console.ReadLine();

            return choice switch
            {
                "1" => "Classic",
                "2" => "Dark",
                "3" => "Pink",
                _ => currentTheme
            };
        }

        public GameSettings GetCurrentSettings()
        {
            return _databaseService.GetActiveGameSettings() ?? new GameSettings
            {
                BallSpeed = 5,
                PaddleSize = 5,
                WinningScore = 5,
                PaddleSpeed = 5,
                VisualTheme = "Classic"
            };
        }
    }
} 