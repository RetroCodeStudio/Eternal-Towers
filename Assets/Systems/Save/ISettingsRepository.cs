public interface ISettingsRepository
{
    void Save(GameSettings settings);
    GameSettings Load();
}