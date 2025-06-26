using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TennisGame.Services;
using TennisGame.Models;

namespace TennisGame.Game
{
    public class TennisGame
    {
        private RenderWindow window;
        private Clock gameClock;
        private DatabaseService databaseService;
        private SettingsService settingsService;
        
        // Oyun nesneleri
        private Paddle player1Paddle;
        private Paddle player2Paddle;
        private Ball ball;
        private List<Star> stars;
        
        // Oyun durumu
        private int player1Score = 0;
        private int player2Score = 0;
        private bool gameRunning = true;
        private bool gamePaused = false;
        private GameSettings currentSettings;
        
        // Oyuncu bilgileri
        private string player1Name;
        private string player2Name;
        private bool isAIMode = false;
        
        // Oyun zamanı
        private DateTime gameStartTime;
        private int gameDuration = 0;
        
        // Yıldız sistemi
        private float starSpawnTimer = 0f;
        private float starSpawnInterval = 5f; // 5 saniyede bir yıldız
        private Random random;

        private Font? gameFont;

        // Tema renkleri
        private Color backgroundColor;
        private Color paddleColor;
        private Color ballColor;
        private Color starColor;
        private Color textColor;
        private Color centerLineColor;

        public TennisGame(string connectionString, string player1Name, string player2Name, bool aiMode = false)
        {
            this.player1Name = player1Name;
            this.player2Name = player2Name;
            this.isAIMode = aiMode;
            
            databaseService = new DatabaseService(connectionString);
            settingsService = new SettingsService(databaseService);
            currentSettings = settingsService.GetCurrentSettings();
            
            stars = new List<Star>();
            random = new Random();
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Pencere oluştur
            window = new RenderWindow(new VideoMode(1200, 600), "Furkanla Pong");
            window.SetFramerateLimit(60);
            window.Closed += (sender, e) => window.Close();
            window.KeyPressed += OnKeyPressed;
            window.KeyReleased += OnKeyReleased;

            // Ayarları uygula
            ApplySettings();

            // Oyun nesnelerini oluştur
            float paddleWidth = 15f;
            float paddleHeight = GetPaddleHeight();
            float ballSize = 15f;

            player1Paddle = new Paddle(
                new Vector2f(50, 250),
                new Vector2f(paddleWidth, paddleHeight),
                paddleColor
            );
            player1Paddle.Speed = GetPaddleSpeed();

            player2Paddle = new Paddle(
                new Vector2f(1135, 250),
                new Vector2f(paddleWidth, paddleHeight),
                paddleColor,
                isAIMode
            );
            player2Paddle.Speed = GetPaddleSpeed();

            ball = new Ball(
                new Vector2f(600, 300),
                new Vector2f(ballSize, ballSize),
                ballColor
            );

            // Top hızını ayarla
            ball.SetSpeed(GetBallSpeed());

            gameClock = new Clock();
            gameStartTime = DateTime.Now;

            // Font yükleme - birden fazla seçenek dene
            gameFont = LoadFont();
        }

        private Font? LoadFont()
        {
            // Font yükleme seçenekleri (öncelik sırasına göre)
            string[] fontPaths = {
                "/System/Library/Fonts/Helvetica.ttc",       // macOS - mevcut
                "/System/Library/Fonts/ArialHB.ttc",         // macOS - mevcut
                "/System/Library/Fonts/HelveticaNeue.ttc",   // macOS - mevcut
                "/System/Library/Fonts/Arial.ttf",           // macOS
                "/Library/Fonts/Arial.ttf",                  // macOS
                "/Library/Fonts/Helvetica.ttf",              // macOS
                "C:/Windows/Fonts/arial.ttf",                // Windows
                "C:/Windows/Fonts/calibri.ttf",              // Windows
                "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf", // Linux
                "/usr/share/fonts/TTF/arial.ttf",            // Linux
                "arial.ttf",                                 // Mevcut dizin
                "fonts/arial.ttf",                           // Fonts klasörü
                "fonts/DejaVuSans.ttf"                       // Fonts klasörü
            };

            foreach (string fontPath in fontPaths)
            {
                try
                {
                    if (System.IO.File.Exists(fontPath))
                    {
                        Console.WriteLine($"✅ Font yüklendi: {fontPath}");
                        return new Font(fontPath);
                    }
                }
                catch (Exception ex)
                {
                    // Bu font yüklenemezse diğerini dene
                    Console.WriteLine($"❌ Font yüklenemedi: {fontPath} - {ex.Message}");
                    continue;
                }
            }

            // Hiçbir font yüklenemezse null döndür
            Console.WriteLine("⚠️  Hiçbir font yüklenemedi. Metin gösterimi basitleştirilecek.");
            return null;
        }

        private void ApplySettings()
        {
            // Tema renklerini ayarla
            switch (currentSettings.VisualTheme)
            {
                case "Dark":
                    backgroundColor = new Color(20, 20, 20);
                    paddleColor = new Color(100, 100, 100);
                    ballColor = new Color(200, 200, 200);
                    starColor = new Color(255, 215, 0); // Altın sarısı
                    textColor = new Color(255, 255, 255);
                    centerLineColor = new Color(80, 80, 80);
                    break;
                case "Pink":
                    backgroundColor = new Color(255, 192, 203); // Pembe
                    paddleColor = new Color(255, 20, 147); // Koyu pembe
                    ballColor = new Color(255, 105, 180); // Hot pink
                    starColor = new Color(255, 255, 0); // Sarı
                    textColor = new Color(139, 0, 139); // Koyu mor
                    centerLineColor = new Color(255, 182, 193); // Açık pembe
                    break;
                default: // Classic
                    backgroundColor = new Color(0, 0, 0);
                    paddleColor = new Color(255, 255, 255);
                    ballColor = new Color(255, 255, 255);
                    starColor = new Color(255, 215, 0);
                    textColor = new Color(255, 255, 255);
                    centerLineColor = new Color(128, 128, 128);
                    break;
            }
        }

        private float GetPaddleHeight()
        {
            // 0-10 arası değeri 60-140 arası yüksekliğe çevir
            return 60f + (currentSettings.PaddleSize * 8f);
        }

        private float GetBallSpeed()
        {
            // 0-10 arası değeri 200-600 arası hıza çevir
            return 200f + (currentSettings.BallSpeed * 40f);
        }

        private float GetPaddleSpeed()
        {
            // 0-10 arası değeri 200-600 arası hıza çevir
            return 200f + (currentSettings.PaddleSpeed * 40f);
        }

        private int GetWinningScore()
        {
            // 0-10 arası değeri 3-15 arası skora çevir
            return 3 + currentSettings.WinningScore;
        }

        public void Run()
        {
            while (window.IsOpen && gameRunning)
            {
                window.DispatchEvents();
                
                if (!gamePaused)
                {
                    Update();
                }
                
                Draw();
            }

            // Oyun bitti, veritabanına kaydet
            SaveGameResult();
        }

        private void Update()
        {
            float deltaTime = gameClock.Restart().AsSeconds();
            gameDuration = (int)(DateTime.Now - gameStartTime).TotalSeconds;

            UpdateStarSpawn(deltaTime);
            UpdateStars(deltaTime);

            if (isAIMode)
            {
                player2Paddle.UpdateAI(deltaTime, ball.Position, window.Size.Y);
            }

            ball.Update(deltaTime);
            CheckStarCollisions();
            ball.HandleWallCollision(window.Size.Y);

            if (ball.Intersects(player1Paddle))
            {
                ball.HandlePaddleCollision(player1Paddle);
            }
            else if (ball.Intersects(player2Paddle))
            {
                ball.HandlePaddleCollision(player2Paddle);
            }

            // Gol kontrolü - her durumda topu resetle
            if (ball.IsOutOfBounds(window.Size.X))
            {
                int scoringPlayer = ball.GetScoringPlayer(window.Size.X);
                if (scoringPlayer == 1)
                {
                    player1Score++;
                }
                else if (scoringPlayer == 2)
                {
                    player2Score++;
                }
                ball.Position = new SFML.System.Vector2f(window.Size.X / 2, window.Size.Y / 2);
                ball.Reset();
                if (player1Score >= GetWinningScore() || player2Score >= GetWinningScore())
                {
                    gameRunning = false;
                    ShowGameOver();
                }
            }
        }

        private void UpdateStarSpawn(float deltaTime)
        {
            starSpawnTimer += deltaTime;
            if (starSpawnTimer >= starSpawnInterval)
            {
                SpawnStar();
                starSpawnTimer = 0f;
            }
        }

        private void SpawnStar()
        {
            // Rastgele pozisyon (çubukların arasında)
            float x = random.Next(200, 1000);
            float y = random.Next(50, (int)window.Size.Y - 50);
            
            stars.Add(new Star(new Vector2f(x, y), starColor));
        }

        private void UpdateStars(float deltaTime)
        {
            try
            {
                for (int i = stars.Count - 1; i >= 0; i--)
                {
                    stars[i].Update(deltaTime);
                    if (!stars[i].IsActive)
                    {
                        stars.RemoveAt(i);
                    }
                }
            }
            catch { /* Donma koruması */ }
        }

        private void CheckStarCollisions()
        {
            try
            {
                for (int i = stars.Count - 1; i >= 0; i--)
                {
                    if (stars[i].IntersectsWithBall(ball))
                    {
                        stars.RemoveAt(i);
                        ball.SpeedMultiplier += 0.5f;
                        ball.Velocity = new SFML.System.Vector2f(
                            ball.Velocity.X * 1.2f,
                            ball.Velocity.Y * 1.2f
                        );
                    }
                }
            }
            catch { /* Donma koruması */ }
        }

        private void Draw()
        {
            window.Clear(backgroundColor);

            // Tema özel çizimler
            if (currentSettings.VisualTheme == "Pink")
            {
                DrawHelloKittyElements();
            }

            // Orta çizgi
            var centerLine = new RectangleShape(new Vector2f(2, window.Size.Y))
            {
                Position = new Vector2f(window.Size.X / 2 - 1, 0),
                FillColor = centerLineColor
            };
            window.Draw(centerLine);

            // Oyun nesnelerini çiz
            player1Paddle.Draw(window);
            player2Paddle.Draw(window);
            ball.Draw(window);

            // Yıldızları çiz
            foreach (var star in stars)
            {
                star.Draw(window);
            }

            // Skorları çiz
            DrawScores();

            // Hız göstergesi
            DrawSpeedIndicator();

            window.Display();
        }

        private void DrawScores()
        {
            if (gameFont != null)
            {
                var player1Text = new Text($"{player1Name}: {player1Score}", gameFont, 24)
                {
                    Position = new Vector2f(100, 20),
                    FillColor = textColor
                };
                var player2Text = new Text($"{player2Name}: {player2Score}", gameFont, 24)
                {
                    Position = new Vector2f(900, 20),
                    FillColor = textColor
                };
                window.Draw(player1Text);
                window.Draw(player2Text);
            }
            else
            {
                // Font yoksa, görsel skor kutuları çiz
                DrawScoreBox($"{player1Name}: {player1Score}", new Vector2f(100, 20), textColor);
                DrawScoreBox($"{player2Name}: {player2Score}", new Vector2f(900, 20), textColor);
            }
        }

        private void DrawSpeedIndicator()
        {
            if (gameFont != null)
            {
                var speedText = new Text($"Hız: {ball.SpeedMultiplier:F1}x", gameFont, 18)
                {
                    Position = new Vector2f(550, 20),
                    FillColor = Color.Yellow
                };
                window.Draw(speedText);
            }
            else
            {
                DrawScoreBox($"Hız: {ball.SpeedMultiplier:F1}x", new Vector2f(550, 20), Color.Yellow);
            }
        }

        private void DrawScoreBox(string text, Vector2f position, Color color)
        {
            // Arka plan kutusu
            var backgroundRect = new RectangleShape(new Vector2f(150, 30))
            {
                Position = position,
                FillColor = new Color(0, 0, 0, 180),
                OutlineColor = color,
                OutlineThickness = 2f
            };
            window.Draw(backgroundRect);

            // Metin yerine renkli çizgiler (basit görsel efekt)
            int charCount = Math.Min(text.Length, 15); // Maksimum 15 karakter
            float charWidth = 8f;
            float startX = position.X + 10;
            float startY = position.Y + 15;

            for (int i = 0; i < charCount; i++)
            {
                // Her karakter için basit çizgi
                var charLine = new RectangleShape(new Vector2f(charWidth, 2))
                {
                    Position = new Vector2f(startX + (i * charWidth), startY),
                    FillColor = color
                };
                window.Draw(charLine);
            }
        }

        private void DrawHelloKittyElements()
        {
            // Hello Kitty temalı dekoratif elementler
            // Köşelerde küçük kalp şekilleri
            var heart1 = new CircleShape(8f)
            {
                Position = new Vector2f(20, 20),
                FillColor = new Color(255, 182, 193)
            };
            window.Draw(heart1);

            var heart2 = new CircleShape(8f)
            {
                Position = new Vector2f(window.Size.X - 36, 20),
                FillColor = new Color(255, 182, 193)
            };
            window.Draw(heart2);

            var heart3 = new CircleShape(8f)
            {
                Position = new Vector2f(20, window.Size.Y - 36),
                FillColor = new Color(255, 182, 193)
            };
            window.Draw(heart3);

            var heart4 = new CircleShape(8f)
            {
                Position = new Vector2f(window.Size.X - 36, window.Size.Y - 36),
                FillColor = new Color(255, 182, 193)
            };
            window.Draw(heart4);

            // Üst ve alt kenarlarda dekoratif çizgiler
            var topLine = new RectangleShape(new Vector2f(window.Size.X, 3))
            {
                Position = new Vector2f(0, 0),
                FillColor = new Color(255, 20, 147)
            };
            window.Draw(topLine);

            var bottomLine = new RectangleShape(new Vector2f(window.Size.X, 3))
            {
                Position = new Vector2f(0, window.Size.Y - 3),
                FillColor = new Color(255, 20, 147)
            };
            window.Draw(bottomLine);
        }

        private void OnKeyPressed(object sender, KeyEventArgs e)
        {
            float deltaTime = 1f / 60f; // Sabit delta time

            switch (e.Code)
            {
                case Keyboard.Key.W:
                    if (!gamePaused)
                        player1Paddle.MoveUp(deltaTime);
                    break;
                case Keyboard.Key.S:
                    if (!gamePaused)
                        player1Paddle.MoveDown(deltaTime, window.Size.Y);
                    break;
                case Keyboard.Key.Up:
                    if (!isAIMode && !gamePaused)
                        player2Paddle.MoveUp(deltaTime);
                    break;
                case Keyboard.Key.Down:
                    if (!isAIMode && !gamePaused)
                        player2Paddle.MoveDown(deltaTime, window.Size.Y);
                    break;
                case Keyboard.Key.P:
                    gamePaused = !gamePaused;
                    break;
                case Keyboard.Key.Escape:
                    window.Close();
                    break;
            }
        }

        private void OnKeyReleased(object sender, KeyEventArgs e)
        {
            // Gerekirse key release işlemleri
        }

        private void ShowGameOver()
        {
            string winner = player1Score > player2Score ? player1Name : player2Name;
            Console.WriteLine($"\n=== OYUN BİTTİ ===");
            Console.WriteLine($"Kazanan: {winner}");
            Console.WriteLine($"Final Skor: {player1Name} {player1Score} - {player2Name} {player2Score}");
            Console.WriteLine($"Oyun Süresi: {gameDuration} saniye");
            Console.WriteLine("==================\n");
        }

        private void SaveGameResult()
        {
            try
            {
                var match = new Match
                {
                    Player1Id = databaseService.GetOrCreatePlayer(player1Name),
                    Player2Id = databaseService.GetOrCreatePlayer(player2Name),
                    Player1Score = player1Score,
                    Player2Score = player2Score,
                    WinnerId = player1Score > player2Score 
                        ? databaseService.GetOrCreatePlayer(player1Name)
                        : databaseService.GetOrCreatePlayer(player2Name),
                    Duration = gameDuration,
                    IsAIMode = isAIMode,
                    MatchDate = DateTime.Now
                };

                databaseService.CreateMatch(match);
                Console.WriteLine("Oyun sonucu veritabanına kaydedildi.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Veritabanı kayıt hatası: {ex.Message}");
            }
        }
    }
} 