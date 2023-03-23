public class SessionData
{
    public static Golf2Socket socketManager;
    public static ProceduralMapGenerator.MapData mapData;

    public static void Init()
    {
        Golf2Socket.OnMapSync += UpdateMapData;
    }

    private static void UpdateMapData(ProceduralMapGenerator.MapData data)
    {
        mapData = data;
    }  
}