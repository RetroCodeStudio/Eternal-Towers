namespace EternalTowers.Systems.Save
{
    public interface IPersistenceService
    {
        void Save<T>(T data);

        T Load<T>(int id);
    }
}