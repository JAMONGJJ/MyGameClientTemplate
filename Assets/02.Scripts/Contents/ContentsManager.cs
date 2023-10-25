

namespace ClientTemplate
{
    using LobbyRegion;
    
    public static class Contents
    {
        public static ContentsManager Manager { get; } = new ContentsManager();
    }
    
    public class ContentsManager
    {
        public LobbyManager Lobby { get; } = new LobbyManager();
    }
}