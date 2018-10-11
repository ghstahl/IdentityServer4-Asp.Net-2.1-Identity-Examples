namespace ScopedHelpers
{
    public interface IProtectedCookieStore
    {
        void Store(string cookieName, string data, int minutes);
        bool TryRead(string cookieName, out string value);
        void Remove(string cookieName);
    }
}