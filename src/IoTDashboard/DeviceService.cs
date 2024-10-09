using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;

public class DeviceService
{
    private readonly string _connectionString = "Data Source=iot_devices.db";

    public DeviceService()
    {
        // Initialiser la base de données et créer la table si elle n'existe pas
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var tableCmd = connection.CreateCommand();
        tableCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS DeviceData (
                Id TEXT PRIMARY KEY,
                Temperature REAL,
                Humidity INTEGER,
                LastUpdate DATETIME
            )";
        tableCmd.ExecuteNonQuery();
    }

    public bool AddDevice(Device device)
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = @"
                INSERT INTO DeviceData (Id, Temperature, Humidity, LastUpdate)
                VALUES ($id, $temperature, $humidity, $lastUpdate)";
            insertCmd.Parameters.AddWithValue("$id", device.Id);
            insertCmd.Parameters.AddWithValue("$temperature", device.Temperature);
            insertCmd.Parameters.AddWithValue("$humidity", device.Humidity);
            insertCmd.Parameters.AddWithValue("$lastUpdate", device.LastUpdate);

            insertCmd.ExecuteNonQuery();
            return true;
        }
        catch (SqliteException ex)
        {
            // Gérer l'erreur si l'appareil existe déjà
            if (ex.SqliteErrorCode == 19) // Violation de contrainte UNIQUE
            {
                return false;
            }
            throw;
        }
    }

    public IEnumerable<Device> GetDeviceData()
    {
        var devices = new List<Device>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var selectCmd = connection.CreateCommand();
        selectCmd.CommandText = "SELECT * FROM DeviceData";

        using var reader = selectCmd.ExecuteReader();
        while (reader.Read())
        {
            var device = new Device
            {
                Id = reader.GetString(0),
                Temperature = reader.GetDouble(1),
                Humidity = reader.GetInt32(2),
                LastUpdate = reader.GetDateTime(3)
            };
            devices.Add(device);
        }

        return devices;
    }

    public void UpdateDeviceData(string deviceId, double? temperature, int? humidity)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var updateCmd = connection.CreateCommand();
        updateCmd.CommandText = @"
            UPDATE DeviceData
            SET Temperature = COALESCE($temperature, Temperature),
                Humidity = COALESCE($humidity, Humidity),
                LastUpdate = $lastUpdate
            WHERE Id = $id";
        updateCmd.Parameters.AddWithValue("$id", deviceId);
        updateCmd.Parameters.AddWithValue("$temperature", (object)temperature ?? DBNull.Value);
        updateCmd.Parameters.AddWithValue("$humidity", (object)humidity ?? DBNull.Value);
        updateCmd.Parameters.AddWithValue("$lastUpdate", DateTime.Now);

        int rowsAffected = updateCmd.ExecuteNonQuery();
        if (rowsAffected == 0)
        {
            throw new DeviceNotFoundException($"Device '{deviceId}' not found.");
        }
    }

    public void RemoveDevice(string deviceId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = "DELETE FROM DeviceData WHERE Id = $id";
        deleteCmd.Parameters.AddWithValue("$id", deviceId);

        deleteCmd.ExecuteNonQuery();
    }

    public void ResetAllDevices()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var resetCmd = connection.CreateCommand();
        resetCmd.CommandText = @"
            UPDATE DeviceData
            SET Temperature = 0,
                Humidity = 0,
                LastUpdate = $lastUpdate";
        resetCmd.Parameters.AddWithValue("$lastUpdate", DateTime.Now);

        resetCmd.ExecuteNonQuery();
    }

    public bool CheckIfDeviceDisconnected(string deviceId)
    {
        var device = GetDeviceById(deviceId);
        if (device != null)
        {
            var timeSinceLastUpdate = DateTime.Now - device.LastUpdate;
            return timeSinceLastUpdate.TotalSeconds > 10;
        }
        return false;
    }

    public void CheckAndExpireInactiveDevices()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var expireCmd = connection.CreateCommand();
        expireCmd.CommandText = @"
            DELETE FROM DeviceData
            WHERE LastUpdate <= $expiryTime";
        expireCmd.Parameters.AddWithValue("$expiryTime", DateTime.Now.AddMinutes(-1));

        expireCmd.ExecuteNonQuery();
    }

    private Device GetDeviceById(string deviceId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var selectCmd = connection.CreateCommand();
        selectCmd.CommandText = "SELECT * FROM DeviceData WHERE Id = $id";
        selectCmd.Parameters.AddWithValue("$id", deviceId);

        using var reader = selectCmd.ExecuteReader();
        if (reader.Read())
        {
            return new Device
            {
                Id = reader.GetString(0),
                Temperature = reader.GetDouble(1),
                Humidity = reader.GetInt32(2),
                LastUpdate = reader.GetDateTime(3)
            };
        }
        return null;
    }
}

// Exception personnalisée
public class DeviceNotFoundException : Exception
{
    public DeviceNotFoundException(string message) : base(message)
    {
    }
}
