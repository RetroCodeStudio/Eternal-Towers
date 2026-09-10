using System;
using UnityEngine;

public class PlayerProfileService : IDisposable
{
    private readonly IPlayerProfileRepository repository;

    private PlayerProfile profile;

    private const string LocalPlayerId = "player-001";

    public PlayerProfileService(IPlayerProfileRepository repository)
    {
        this.repository = repository
            ?? throw new ArgumentNullException(nameof(repository));

        Debug.Log("[PlayerProfileService] Servicio de perfil iniciado.");

        LoadOrCreateProfile();
    }

    public PlayerProfile GetProfile()
    {
        Debug.Log(
            $"[PlayerProfileService] Consultando perfil: " +
            $"{profile?.playerId ?? "NULL"}"
        );

        return profile;
    }

    public void UpdateProfile(
        string playerName,
        string gender,
        int age,
        string password)
    {
        if (profile == null)
        {
            Debug.LogError(
                "[PlayerProfileService] No se puede actualizar. " +
                "No existe un perfil cargado."
            );

            throw new InvalidOperationException(
                "No existe un perfil cargado."
            );
        }

        profile.playerName = playerName;
        profile.gender = gender;
        profile.age = age;
        profile.password = password;

        repository.Save(profile);

        Debug.Log(
            "[PlayerProfileService] Perfil actualizado y guardado. " +
            $"ID: {profile.playerId} | " +
            $"Nombre: {profile.playerName} | " +
            $"Género: {profile.gender} | " +
            $"Edad: {profile.age}"
        );
    }

    private void LoadOrCreateProfile()
    {
        Debug.Log(
            $"[PlayerProfileService] Buscando perfil con ID: {LocalPlayerId}"
        );

        profile = repository.Load(LocalPlayerId);

        if (profile != null)
        {
            Debug.Log(
                "[PlayerProfileService] Perfil encontrado en SQLite. " +
                $"ID: {profile.playerId} | " +
                $"Nombre: {profile.playerName} | " +
                $"Género: {profile.gender} | " +
                $"Edad: {profile.age}"
            );

            return;
        }

        Debug.Log(
            "[PlayerProfileService] No existe un perfil. " +
            "Creando perfil inicial."
        );

        profile = new PlayerProfile
        {
            playerId = LocalPlayerId,
            playerName = "Player",
            gender = string.Empty,
            age = 0,
            password = string.Empty
        };

        repository.Save(profile);

        Debug.Log(
            "[PlayerProfileService] Perfil inicial creado y guardado en SQLite. " +
            $"ID: {profile.playerId} | " +
            $"Nombre: {profile.playerName}"
        );
    }

    public void Dispose()
    {
        if (repository is IDisposable disposableRepository)
        {
            disposableRepository.Dispose();

            Debug.Log(
                "[PlayerProfileService] Repositorio liberado correctamente."
            );
        }
    }
}