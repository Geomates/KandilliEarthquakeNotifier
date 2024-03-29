﻿namespace KandilliEarthquakeBot.Enums
{
    public static class BotDialog
    {
        public const string START_SUBSCRIPTION = "Deprem bildirimleri uyeliginiz basladi. Bildirimleri kisisellestirmek icin /siddet veya /konumekle komutlarini kullanabilirsiniz.";
        public const string ASK_LOCATION = "Sadece 200km cevrenizdeki depremlerden haberdar olabilirsiniz, bunun icin deprem bildirimleri almak istediginiz dairesel bolgenin merkez konumunu paylasmaniz gerekiyor. Simdi Telegram'dan bu konumu paylasin.";
        public const string REPLY_LOCATION = "Konum bilgisi kaydedildi, artik bu konumun 200km cevresindeki depremlerden haberdar olacaksiniz.";
        public const string REMOVED_LOCATION = "Konum bilgisi kaldirildi.";
        public const string ASK_MAGNITUDE = "En dusuk hangi siddetteki depremlerden haber almak istersiniz?";
        public const string REPLY_MAGNITUDE = "Bildirimlerin deprem siddeti {0:f1} ve ustu olacak sekilde ayarlandi.";
        public const string ABOUT = @"
Veriler Kandilli Rasathanesi'nden alinmaktadir. Verilerin dogrulugu garanti edilmez. 

Begendiyseniz destek olmak icin; https://www.buymeacoffee.com/mete
        ";
    }
}
