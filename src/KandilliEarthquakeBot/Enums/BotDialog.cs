namespace KandilliEarthquakeBot.Enums
{
    public static class BotDialog
    {
        public const string START_SUBSCRIPTION = "Deprem bildirimleri uyeliginiz basladi. Bildirimleri kisisellestirmek icin /siddet komutunu kullanabilirsiniz.";
        public const string ASK_LOCATION = "Konum bilginizi paylasarak sadece cevrenizdeki depremlerden haberdar olabilirsiniz [Test Asamasinda]. Konum bilginiz paylasmak icin asagidaki butona basin.";
        public const string REPLY_LOCATION = "Konum bilginiz kaydedildi.";        
        public const string ASK_MAGNITUDE = "En dusuk hangi siddetteki depremlerden haber almak istersiniz?";
        public const string REPLY_MAGNITUDE = "Bildirimlerin deprem siddeti {0} ve ustu olacak sekilde ayarlandi.";
    }
}
