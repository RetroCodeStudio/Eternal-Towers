using UnityEngine;

public class PlayerProfileController : MonoBehaviour
{
    private PlayerProfileService playerProfileService;

    private void Awake()
    {
        playerProfileService = new PlayerProfileService();
    }

    public PlayerProfile GetProfile()
    {
        return playerProfileService.GetProfile();
    }

    public void UpdateProfile(string playerName)
    {
        playerProfileService.UpdateProfile(playerName);
    }
}
