public class PlayerProfileService
{
    private PlayerProfile profile = new PlayerProfile
    {
        playerId = "player-001",
        playerName = "Player"
    };

    public PlayerProfile GetProfile()
    {
        return profile;
    }

    public void UpdateProfile(string playerName)
    {
        profile.playerName = playerName;
    }
}
