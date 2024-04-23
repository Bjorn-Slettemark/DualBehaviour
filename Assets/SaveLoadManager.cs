using UnityEngine;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;

[Icon("Assets/Editor/Icons/SaveLoadIcon.png")]
public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;
    private const string SaveFileName = "GameData.dat";
    private readonly byte[] aesKey = Encoding.UTF8.GetBytes("1234567890123456"); // 16 characters for 128 bits
    private readonly byte[] aesIV = Encoding.UTF8.GetBytes("1234567890123456"); // 16 characters for 128 bits

    public List<SaveBlueprintSO> gameSaveBlueprintSO; // List to hold SaveConfigSO ScriptableObjects

    [SerializeField]
    GameEventChannelSO saveEventChannel;
    [SerializeField]
    GameEventChannelSO loadEventChannel;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        EventChannelManager.Instance.RegisterChannel(this.gameObject, saveEventChannel, SaveEvent);
        EventChannelManager.Instance.RegisterChannel(this.gameObject, loadEventChannel, LoadEvent);


    }

    public void SaveEvent(string eventName)
    {
        GameState previousGameState = GameStateManager.Instance.CurrentState.gameState;
        if (eventName.ToLower() == "Save".ToLower())
        {
            GameStateManager.Instance.ChangeState(GameState.Saving);
            SaveBlueprint(gameSaveBlueprintSO[0]);
        }
        if (eventName.ToLower() == "Done")
        {
            GameStateManager.Instance.ChangeState(previousGameState);
        }
    }
    public void LoadEvent(string eventName)
    {
        GameState previousGameState = GameStateManager.Instance.CurrentState.gameState;
        GameState nextGameState = GameState.Default;

        if (eventName.ToLower() == "Load".ToLower())
        {
            GameStateManager.Instance.ChangeState(GameState.Loading);
            LoadBlueprint(gameSaveBlueprintSO[0]);
            nextGameState = gameSaveBlueprintSO[0].postLoadGameState;

        }
        if (eventName.ToLower() == "Done")
        {

            GameStateManager.Instance.ChangeState(nextGameState);

        }
    }
    // Save all game data using the SaveConfigSOs
    public void SaveBlueprint(SaveBlueprintSO saveConfig)
    {
            //GameStateManager.Instance.CurrentState =
            saveConfig.Save();
 
    }

    // Load all game data using the SaveConfigSOs
    public void LoadBlueprint(SaveBlueprintSO saveConfig)
    {

            saveConfig.Load();
       
    }
    public void SaveGame(GameDataSO gameData)
    {
        SaveGameData(gameData);
    }

    public void LoadGame(GameDataSO gameData)
    {
        LoadGameData(gameData);
    }


    private void SaveGameData(GameDataSO gameData)
    {
        string filePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        string json = JsonUtility.ToJson(gameData);

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = aesKey;
            aesAlg.IV = aesIV;
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(json);
                    }
                }
                File.WriteAllBytes(filePath, msEncrypt.ToArray());
            }
        }
    }

    private void LoadGameData(GameDataSO gameData)
    {
        string filePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        if (File.Exists(filePath))
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            string json = DecryptData(bytes);

            JsonUtility.FromJsonOverwrite(json, gameData);
            Debug.Log($"Loaded game data: {json}");
        }
        else
        {
            Debug.LogWarning("Save file not found.");
        }
    }

    private string DecryptData(byte[] encryptedData)
    {
        string plaintext = null;

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = aesKey;
            aesAlg.IV = aesIV;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }

        return plaintext;
    }
}
