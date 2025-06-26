# 🎮 Furkanla Pong

**Furkan'ın özel Pong oyunu** - C# .NET Core ile geliştirilmiş, Dapper ve SQLite kullanan modern Pong oyunu.

## 🌟 Özellikler

### 🎯 Oyun Özellikleri
- **Klasik Pong Oynanışı**: İki kişilik ve AI modu
- **Kırmızı Hızlanma Bölgeleri**: Çubukların üst ve alt kısımlarında kırmızı bölgeler
- **Yıldız Bonus Sistemi**: Toplanabilir yıldızlar ile hız artışı
- **Dinamik Hız Sistemi**: Çarpışmalara göre artan top hızı
- **Gelişmiş AI**: Akıllı rakip yapay zekası

### ⚙️ Oyun Ayarları Sistemi
- **Top Hızı**: 0-10 arası ayarlanabilir (200-600 piksel/saniye)
- **Çubuk Boyutu**: 0-10 arası ayarlanabilir (60-140 piksel yükseklik)
- **Kazanma Skoru**: 0-10 arası ayarlanabilir (3-15 puan)
- **Çubuk Hızı**: 0-10 arası ayarlanabilir (200-600 piksel/saniye)
- **Ayar Profilleri**: Birden fazla ayar profili oluşturma ve yönetme

### 🎨 Görsel Tema Sistemi
1. **Classic (Klasik)**: Siyah-beyaz klasik tema
2. **Dark (Karanlık Mod)**: Koyu gri arka plan, açık gri elementler
3. **Pink (Pembe Mod)**: Hello Kitty temalı pembe renk paleti
   - Pembe arka plan
   - Koyu pembe çubuklar
   - Hot pink top
   - Köşelerde kalp şekilleri
   - Üst/alt kenarlarda dekoratif çizgiler

### 🗄️ Veritabanı Entegrasyonu (Dapper)
- **SQLite Veritabanı**: Hafif ve hızlı veritabanı
- **Dapper ORM**: Hızlı ve güvenli veri erişimi
- **CRUD İşlemleri**: Tam veri yönetimi
- **Oyuncu İstatistikleri**: Detaylı performans takibi
- **Maç Geçmişi**: Tüm oyunların kaydı
- **Ayar Profilleri**: Kalıcı ayar saklama

### 📊 Raporlama Sistemi
- Oyuncu istatistikleri ve sıralamaları
- Son maçlar listesi
- Oyuncu maç geçmişi
- AI maç istatistikleri
- En iyi oyuncular listesi

## 🚀 Kurulum

### Gereksinimler
- .NET 8.0 SDK
- macOS, Windows veya Linux

### Adımlar
```bash
# Projeyi klonlayın
git clone https://github.com/[kullanici-adi]/furkanla-pong.git
cd furkanla-pong/TennisGame

# Bağımlılıkları yükleyin
dotnet restore

# Projeyi derleyin
dotnet build

# Oyunu çalıştırın
dotnet run
```

## 🎮 Kontroller

### İki Kişilik Oyun
- **Oyuncu 1**: W (yukarı), S (aşağı)
- **Oyuncu 2**: Yukarı Ok, Aşağı Ok
- **P**: Oyunu duraklat/devam et
- **ESC**: Oyundan çık

### AI ile Oyun
- **Oyuncu**: W (yukarı), S (aşağı)
- **P**: Oyunu duraklat/devam et
- **ESC**: Oyundan çık

## ⚙️ Oyun Ayarları

### Ayar Menüsü
1. Ana menüden "Oyun Ayarları" seçin
2. Mevcut ayarları görüntüleyin
3. Yeni profil oluşturun veya mevcut profili düzenleyin
4. İstediğiniz değerleri 0-10 arası ayarlayın
5. Tema seçin (Classic/Dark/Pink)
6. Profili aktif yapın

### Ayar Değerleri
- **Top Hızı (0-10)**: Topun hareket hızı
- **Çubuk Boyutu (0-10)**: Çubukların yüksekliği
- **Kazanma Skoru (0-10)**: Kazanmak için gereken puan (3-15)
- **Çubuk Hızı (0-10)**: Çubukların hareket hızı

## 🎨 Görsel Temalar

### Classic (Klasik)
- Siyah arka plan
- Beyaz çubuklar ve top
- Altın sarısı yıldızlar
- Klasik Pong görünümü

### Dark (Karanlık Mod)
- Koyu gri arka plan (20, 20, 20)
- Açık gri çubuklar (100, 100, 100)
- Beyaz top (200, 200, 200)
- Altın sarısı yıldızlar
- Göz yorgunluğunu azaltan tema

### Pink (Pembe Mod - Hello Kitty)
- Pembe arka plan (255, 192, 203)
- Koyu pembe çubuklar (255, 20, 147)
- Hot pink top (255, 105, 180)
- Sarı yıldızlar
- Köşelerde kalp şekilleri
- Üst/alt kenarlarda dekoratif çizgiler
- Hello Kitty temalı pembe renk paleti

## 🗄️ Veritabanı Yapısı

### Tablolar
- **Players**: Oyuncu bilgileri
- **Matches**: Maç sonuçları
- **GameSettings**: Oyun ayarları

### Dapper Kullanımı
```csharp
// Oyuncu oluşturma
var playerId = databaseService.CreatePlayer("Furkan");

// Maç kaydetme
var match = new Match { /* maç bilgileri */ };
databaseService.CreateMatch(match);

// Ayar profili oluşturma
var settings = new GameSettings { /* ayar bilgileri */ };
databaseService.CreateGameSettings(settings);
```

## 🔧 Teknik Özellikler

### Kullanılan Teknolojiler
- **C# .NET 8.0**: Modern C# özellikleri
- **SFML.NET**: Cross-platform grafik kütüphanesi
- **Dapper**: Hızlı ORM
- **SQLite**: Hafif veritabanı
- **Microsoft.Data.Sqlite**: SQLite bağlantısı

### Mimari
- **MVC Pattern**: Model-View-Controller yapısı
- **Service Layer**: İş mantığı katmanı
- **Repository Pattern**: Veri erişim katmanı
- **Dependency Injection**: Bağımlılık yönetimi

### Dosya Yapısı
```
TennisGame/
├── Game/                 # Oyun mantığı
│   ├── TennisGame.cs    # Ana oyun sınıfı
│   ├── Ball.cs          # Top sınıfı
│   ├── Paddle.cs        # Çubuk sınıfı
│   ├── Star.cs          # Yıldız sınıfı
│   └── GameObject.cs    # Temel oyun nesnesi
├── Models/              # Veri modelleri
│   ├── Player.cs        # Oyuncu modeli
│   ├── Match.cs         # Maç modeli
│   └── GameSettings.cs  # Ayar modeli
├── Services/            # Servis katmanı
│   ├── DatabaseService.cs   # Veritabanı servisi
│   ├── ReportService.cs     # Raporlama servisi
│   └── SettingsService.cs   # Ayar servisi
└── Program.cs           # Ana program
```

## 🐛 Sorun Giderme

### Font Yükleme Sorunu
Oyun artık gelişmiş font yükleme sistemi kullanıyor:
- Birden fazla font yolu deneniyor
- macOS, Windows ve Linux için uygun fontlar
- Font yüklenemezse görsel alternatifler
- Detaylı hata mesajları

### Veritabanı Sorunları
- SQLite dosyası otomatik oluşturuluyor
- Tablolar otomatik oluşturuluyor
- Varsayılan ayarlar otomatik ekleniyor

## 🤝 Katkıda Bulunma

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/yeni-ozellik`)
3. Değişikliklerinizi commit edin (`git commit -am 'Yeni özellik eklendi'`)
4. Branch'inizi push edin (`git push origin feature/yeni-ozellik`)
5. Pull Request oluşturun

## 📝 Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

## 👨‍💻 Geliştirici

**Furkan** tarafından özel olarak geliştirilmiştir.

---

## 🎉 Özel Özellikler

### Furkan'ın Dokunuşları
- **Dinamik Hız Sistemi**: Çarpışmalara göre artan top hızı
- **Yıldız Toplama Bonusu**: Ekstra hız için yıldız toplama
- **Gelişmiş AI**: Akıllı rakip yapay zekası
- **SQLite Veritabanı Entegrasyonu**: Hafif ve hızlı veri saklama
- **Hello Kitty Temalı Pembe Mod**: Sevimli pembe tema
- **Karanlık Mod Desteği**: Göz dostu karanlık tema
- **Özelleştirilebilir Oyun Ayarları**: Tam kontrol
- **Ayar Profilleri**: Birden fazla ayar saklama

### Oyun Kuralları
- İlk belirlenen puana ulaşan oyuncu kazanır (varsayılan: 8)
- Top çubuklara çarptığında yön değiştirir
- Kırmızı bölgeye çarpan top 3 kez hızlanır
- Yıldızları toplayarak ekstra hız kazanın!
- Topu kaçıran rakibe puan kazandırır

**Furkanla Pong** - Klasik Pong'un modern ve özelleştirilebilir versiyonu! 🏓✨ 