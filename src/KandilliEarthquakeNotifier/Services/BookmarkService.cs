using System.Threading.Tasks;

namespace KandilliEarthquakeNotifier.Services
{
    public interface IBookmarkService
    {
        Task<string> GetBookmark(string bookmarkName);
        Task<bool> SetBookmark(string bookmarkName, string bookmarkValue);
    }

    public class BookmarkService: IBookmarkService
    {
        private readonly IKeyValueStore _keyValueStore;

        public BookmarkService(IKeyValueStore keyValueStore)
        {
            _keyValueStore = keyValueStore;
        }

        public Task<string> GetBookmark(string bookmarkName) =>  _keyValueStore.Read(bookmarkName);

        public Task<bool> SetBookmark(string bookmarkName, string bookmarkValue) => _keyValueStore.Write(bookmarkName, bookmarkValue);
    }
}
