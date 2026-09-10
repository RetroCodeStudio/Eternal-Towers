public interface IPlayerProfileRepository
{
    PlayerProfile Load(string playerId);
    void Save(PlayerProfile profile);
}