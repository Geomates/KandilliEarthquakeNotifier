namespace KandilliEarthquakeBot.Enums
{
    public static class BotDialog
    {
        public const string START_SUBSCRIPTION = "Deprem bildirimleri uyeliginiz basladi. Bildirimleri kisisellestirmek icin /siddet veya /konum komutlarini kullanabilirsiniz.";
        public const string REMOVE_SUBSCRIPTION = "Deprem bildirimleri uyeliginiz silindi. Yeniden uye olmak icin /basla komutunu kullanabilirsiniz.";
        public const string ASK_LOCATION = "Sadece 100km cevrenizdeki depremlerden haberdar olabilirsiniz, bunun icin deprem bildirimleri almak istediginiz dairesel bolgenin merkez konumunu paylasmaniz gerekiyor. Simdi Telegram'dan bu konumu paylasin.";
        public const string REPLY_LOCATION = "Konum bilgisi kaydedildi, artik bu konumun 100km cevresindeki depremlerden haberdar olacaksiniz.";        
        public const string ASK_MAGNITUDE = "En dusuk hangi siddetteki depremlerden haber almak istersiniz?";
        public const string REPLY_MAGNITUDE = "Bildirimlerin deprem siddeti {0:f1} ve ustu olacak sekilde ayarlandi.";
    }
}
