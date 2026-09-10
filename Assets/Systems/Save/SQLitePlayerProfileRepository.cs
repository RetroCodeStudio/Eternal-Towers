using System;
using System.IO;
using SQLite;
using UnityEngine;

public class SQLitePlayerProfileRepository : IPlayerProfileRepository, IDisposable
{
    private const string DatabaseFileName = "eternal_towers.db";

    private readonly SQLiteConnection database;

    public SQLitePlayerProfileRepository()
    {
        string databasePath = Path.Combine(
            Application.persistentDataPath,
            DatabaseFileName
        );

        Debug.Log(
            $"[SQLitePlayerProfileRepository] Inicializando base de datos: {databasePath}"
        );

        database = new SQLiteConnection(databasePath);

        database.CreateTable<PlayerProfileRecord>();

        Debug.Log(
            "[SQLitePlayerProfileRepository] Tabla 'Usuario' verificada correctamente."
        );
    }

    public PlayerProfile Load(string playerId)
    {
        if (string.IsNullOrWhiteSpace(playerId))
        {
            throw new ArgumentException(
                "El ID del jugador no puede estar vacío.",
                nameof(playerId)
            );
        }

        Debug.Log(
            $"[SQLitePlayerProfileRepository] Consultando Usuario con ID: {playerId}"
        );

        PlayerProfileRecord record =
            database.Find<PlayerProfileRecord>(playerId);

        if (record == null)
        {
            Debug.Log(
                $"[SQLitePlayerProfileRepository] No se encontró Usuario con ID: {playerId}"
            );

            return null;
        }

        Debug.Log(
            $"[SQLitePlayerProfileRepository] Usuario encontrado en SQLite. " +
            $"ID: {record.PlayerId} | " +
            $"Nombre: {record.PlayerName} | " +
            $"Género: {record.Gender} | " +
            $"Edad: {record.Age}"
        );

        return new PlayerProfile
        {
            playerId = record.PlayerId,
            playerName = record.PlayerName,
            gender = record.Gender,
            age = record.Age,
            password = record.Password
        };
    }

    public void Save(PlayerProfile profile)
    {
        if (profile == null)
        {
            throw new ArgumentNullException(nameof(profile));
        }

        if (string.IsNullOrWhiteSpace(profile.playerId))
        {
            throw new ArgumentException(
                "El ID del jugador no puede estar vacío.",
                nameof(profile)
            );
        }

        Debug.Log(
            $"[SQLitePlayerProfileRepository] Guardando Usuario en SQLite. " +
            $"ID: {profile.playerId} | " +
            $"Nombre: {profile.playerName} | " +
            $"Género: {profile.gender} | " +
            $"Edad: {profile.age}"
        );

        PlayerProfileRecord record = new PlayerProfileRecord
        {
            PlayerId = profile.playerId,
            PlayerName = profile.playerName,
            Gender = profile.gender,
            Age = profile.age,
            Password = profile.password
        };

        database.InsertOrReplace(record);

        Debug.Log(
            $"[SQLitePlayerProfileRepository] Usuario guardado correctamente. " +
            $"ID: {profile.playerId}"
        );
    }

    public void Dispose()
    {
        database?.Dispose();

        Debug.Log(
            "[SQLitePlayerProfileRepository] Conexión SQLite cerrada correctamente."
        );
    }
}