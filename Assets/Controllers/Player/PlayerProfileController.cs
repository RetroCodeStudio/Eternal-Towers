using UnityEngine;

public class PlayerProfileController : MonoBehaviour
{
    private PlayerProfileService playerProfileService;

    private void Awake()
    {
        playerProfileService = new PlayerProfileService();
    }
}
