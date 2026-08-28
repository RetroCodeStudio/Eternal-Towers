using System;
using System.IO;
using SQLite;
using UnityEngine;

/// <summary>
/// Guarda y recupera la configuración global desde una base de datos SQLite local.
/// </summary>
public class SQLiteSettingsRepository : ISettingsRepository, IDisposable
{
    private const string DatabaseFileName = "eternal_towers.db";
    private const int GlobalSettingsId = 1;

    private readonly SQLiteConnection database;

    public SQLiteSettingsRepository()
    {
        string databasePath = Path.Combine(
            Application.persistentDataPath,
            DatabaseFileName
        );

        database = new SQLiteConnection(databasePath);
        database.CreateTable<SettingsRecord>();
    }

    public void Save(GameSettings settings)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        SettingsRecord record = new SettingsRecord
        {
            IdConfig = GlobalSettingsId,
            Graficos = (int)settings.graficos,
            Volumen = Mathf.Clamp01(settings.volumen)
        };

        database.InsertOrReplace(record);
    }

    public GameSettings Load()
    {
        SettingsRecord record = database.Find<SettingsRecord>(GlobalSettingsId);

        if (record == null)
        {
            return new GameSettings();
        }

        return new GameSettings
        {
            id_config = record.IdConfig,
            graficos = GetGraphicsQuality(record.Graficos),
            volumen = Mathf.Clamp01(record.Volumen)
        };
    }

    public void Dispose()
    {
        database?.Dispose();
    }

    private GraphicsQuality GetGraphicsQuality(int value)
    {
        return Enum.IsDefined(typeof(GraphicsQuality), value)
            ? (GraphicsQuality)value
            : GraphicsQuality.Medio;
    }
}
