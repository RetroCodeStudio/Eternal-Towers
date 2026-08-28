using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== PLAYER PROFILE TEST ===");
        Console.WriteLine();

        PlayerProfileService service = new PlayerProfileService();

        PlayerProfile profile = service.GetProfile();

        Console.WriteLine("Perfil obtenido:");
        Console.WriteLine("ID: " + profile.playerId);
        Console.WriteLine("Nombre: " + profile.playerName);

        Console.WriteLine();
        Console.WriteLine("Actualizando perfil...");

        service.UpdateProfile("NuevoJugador");

        profile = service.GetProfile();

        Console.WriteLine();
        Console.WriteLine("Perfil actualizado:");
        Console.WriteLine("ID: " + profile.playerId);
        Console.WriteLine("Nombre: " + profile.playerName);

        Console.WriteLine();
        Console.WriteLine("Prueba completada correctamente.");
    }
}
