using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MailBee;
using Takkhal_Panel;

public class SteamSettingsHandler
{
    private const ulong SteamBase = 76561197960265728;

    public static string LoadOrCreateSteamId(string gamePath)
    {
        string settingsPath = Path.Combine(gamePath, "steam_settings");
        string steamIdFilePath = Path.Combine(settingsPath, "force_steamid.txt");

        if (!Directory.Exists(settingsPath))
            Directory.CreateDirectory(settingsPath);


        if (File.Exists(steamIdFilePath))
        {
            string existingId = File.ReadAllText(steamIdFilePath).Trim();
            if (!string.IsNullOrEmpty(existingId))
            {
                Takkhal_Panel.Logger.Log("Loggin...");
                return existingId;
            }
        }

        Takkhal_Panel.Logger.Log("Registering...");

        ulong newSteamId = GenerateFakeSteamId();
        string newSteamIdStr = newSteamId.ToString();

        File.WriteAllText(steamIdFilePath, newSteamIdStr);
        return newSteamIdStr;
    }

    private static ulong GenerateFakeSteamId()
    {
     
        string systemId = Environment.MachineName + Environment.UserName;


        byte[] hash;
        using (SHA1 sha1 = SHA1.Create())
        {
            hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(systemId));
        }


        uint accountId = BitConverter.ToUInt32(hash, 0);

        // SteamID64 = Base + AccountId
        return SteamBase + accountId;
    }
}

