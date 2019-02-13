namespace QGM.FlyThrougCamera
{
    public interface IBaseNode
    {
        void DrawNodes();
        void DrawWindow();
        void LinkConnection(ConnectionIO connection, BaseNode id);
        void UnlinkConnection(ConnectionIO connection);
    }
}