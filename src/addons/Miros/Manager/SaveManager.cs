using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Godot;
using FileAccess = Godot.FileAccess;

public partial class SaveManager : Node
{
    private const string SAVE_DIR = "user://saves/";
    private const string SAVE_EXTENSION = ".sav";
    private const string ENCRYPTION_KEY = "YourSecretKey123"; // 建议使用更复杂的密钥
    private const string ENCRYPTION_IV = "YourSecretIV1234"; // 建议使用更复杂的IV
    private GameSaveData _currentSaveData;

    // 加密开关
    private bool _useEncryption;

    public SaveManager()
    {
        Instance = this;
    }

    public static SaveManager Instance { get; private set; }

    public override void _Ready()
    {
        // 确保存档目录存在
        DirAccess.MakeDirRecursiveAbsolute(ProjectSettings.GlobalizePath(SAVE_DIR));
    }

    private EncryptionConfig LoadEncryptionConfig()
    {
        try
        {
            // 从外部配置文件加载加密配置
            var configPath = "res://config/encryption_config.json";
            using var file = FileAccess.Open(configPath, FileAccess.ModeFlags.Read);
            var json = file.GetAsText();
            return JsonSerializer.Deserialize<EncryptionConfig>(json);
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to load encryption config: {e.Message}");
            // 如果配置文件不存在或无法读取，使用默认配置
            // 注意：在实际发布时应确保配置文件存在
            return new EncryptionConfig
            {
                Key = "DefaultKey123!@#$%^&*()", // 这个默认值仅用于开发测试
                IV = "DefaultIV!@#$%^&*" // 这个默认值仅用于开发测试
            };
        }
    }

    /// <summary>
    ///     加密数据
    /// </summary>
    private string EncryptData(string data)
    {
        try
        {
            var config = LoadEncryptionConfig();
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(config.Key).Take(32).ToArray();
                aes.IV = Encoding.UTF8.GetBytes(config.IV).Take(16).ToArray();

                var encryptor = aes.CreateEncryptor();

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(data);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Encryption failed: {e.Message}");
            return data;
        }
    }

    /// <summary>
    ///     解密数据
    /// </summary>
    private string DecryptData(string encryptedData)
    {
        try
        {
            var cipherText = Convert.FromBase64String(encryptedData);

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(ENCRYPTION_KEY).Take(32).ToArray();
                aes.IV = Encoding.UTF8.GetBytes(ENCRYPTION_IV).Take(16).ToArray();

                var decryptor = aes.CreateDecryptor();

                using (var msDecrypt = new MemoryStream(cipherText))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Decryption failed: {e.Message}");
            return encryptedData;
        }
    }

    private string CalculateChecksum(string data)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

    /// <summary>
    ///     设置是否使用加密
    /// </summary>
    public void SetEncryption(bool enable)
    {
        _useEncryption = enable;
        GD.Print($"Save encryption: {(_useEncryption ? "enabled" : "disabled")}");
    }

    /// <summary>
    ///     保存游戏
    /// </summary>
    public void SaveGame(string saveName)
    {
        try
        {
            // 获取旧的存档数据（如果存在）
            string oldChecksum = null;
            var oldSaveCount = 0;
            var saveId = Guid.NewGuid().ToString();

            var existingSave = GetSaveInfo(saveName);
            if (existingSave != null)
            {
                oldChecksum = existingSave.Checksum;
                oldSaveCount = existingSave.SaveCount;
                saveId = existingSave.SaveId; // 保持同一个存档ID
            }

            var saveData = new GameSaveData
            {
                SaveName = saveName,
                SaveTime = DateTime.Now,
                // PlayTime = GameInitializer.Instance.GameTime,
                CurrentScene = SceneManager.Instance.GetCurrentScene().SceneFilePath,
                SceneStates = GetSceneStates(),
                GlobalVariables = GetGlobalVariables(),
                PlayerStats = GetPlayerStats(),

                // 添加安全元数据
                SaveId = saveId,
                CreatedTimestamp = existingSave?.CreatedTimestamp ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                ModifiedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                SaveCount = oldSaveCount + 1,
                PreviousChecksum = oldChecksum
            };

            var json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            // 添加校验和
            saveData.Checksum = CalculateChecksum(json);

            // 根据开关决定是否加密
            var finalData = _useEncryption ? EncryptData(json) : json;

            var savePath = SAVE_DIR + saveName + SAVE_EXTENSION;
            using var file = FileAccess.Open(savePath, FileAccess.ModeFlags.Write);
            file.StoreString(finalData);

            GD.Print($"Game saved {(_useEncryption ? "and encrypted " : "")}successfully to: {savePath}");
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to save game: {e.Message}");
        }
    }

    /// <summary>
    ///     加载游戏
    /// </summary>
    public async void LoadGame(string saveName)
    {
        try
        {
            var savePath = SAVE_DIR + saveName + SAVE_EXTENSION;
            if (!FileAccess.FileExists(savePath))
            {
                GD.PrintErr($"Save file not found: {savePath}");
                return;
            }

            using var file = FileAccess.Open(savePath, FileAccess.ModeFlags.Read);
            var fileContent = file.GetAsText();

            // 尝试解析JSON，如果失败则尝试解密
            string json;
            try
            {
                // 首先尝试直接解析为JSON（未加密情况）
                _currentSaveData = JsonSerializer.Deserialize<GameSaveData>(fileContent);
                json = fileContent;
            }
            catch
            {
                // 如果解析失败，尝试解密
                json = DecryptData(fileContent);
                _currentSaveData = JsonSerializer.Deserialize<GameSaveData>(json);
            }

            // 验证校验和
            var calculatedChecksum = CalculateChecksum(json);
            if (calculatedChecksum != _currentSaveData.Checksum)
                throw new Exception("Save file has been tampered with!");

            // 验证存档完整性
            ValidateSaveData(_currentSaveData);

            await SceneManager.Instance.ChangeScene(_currentSaveData.CurrentScene);
            RestoreSceneStates(_currentSaveData.SceneStates);
            RestoreGlobalVariables(_currentSaveData.GlobalVariables);
            RestorePlayerStats(_currentSaveData.PlayerStats);

            GD.Print($"Game loaded successfully from: {savePath}");
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to load game: {e.Message}");
        }
    }

    /// <summary>
    ///     验证存档数据的完整性
    /// </summary>
    private void ValidateSaveData(GameSaveData saveData)
    {
        // 验证时间戳
        var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (saveData.ModifiedTimestamp > currentTime)
            throw new Exception("Invalid save file: Future timestamp detected");
        if (saveData.CreatedTimestamp > saveData.ModifiedTimestamp)
            throw new Exception("Invalid save file: Creation time is after modification time");

        // 验证存档次数
        if (saveData.SaveCount < 1) throw new Exception("Invalid save file: Invalid save count");

        // 如果有上一个校验和，验证它
        if (!string.IsNullOrEmpty(saveData.PreviousChecksum))
        {
            // 这里可以维护一个校验和历史记录来进行验证
            // 或者将上一个存档文件保存为备份来进行验证
        }
    }

    /// <summary>
    ///     获取存档信息（不加载游戏）
    /// </summary>
    public GameSaveData GetSaveInfo(string saveName)
    {
        try
        {
            var savePath = SAVE_DIR + saveName + SAVE_EXTENSION;
            if (!FileAccess.FileExists(savePath)) return null;

            using var file = FileAccess.Open(savePath, FileAccess.ModeFlags.Read);
            var fileContent = file.GetAsText();

            try
            {
                // 首先尝试直接解析为JSON（未加密情况）
                return JsonSerializer.Deserialize<GameSaveData>(fileContent);
            }
            catch
            {
                // 如果解析失败，尝试解密
                var json = DecryptData(fileContent);
                return JsonSerializer.Deserialize<GameSaveData>(json);
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to get save info: {e.Message}");
            return null;
        }
    }

    /// <summary>
    ///     获取所有存档
    /// </summary>
    public List<string> GetAllSaves()
    {
        var saves = new List<string>();
        var dir = DirAccess.Open(SAVE_DIR);
        if (dir != null)
        {
            dir.ListDirBegin();
            var fileName = dir.GetNext();
            while (!string.IsNullOrEmpty(fileName))
            {
                if (!dir.CurrentIsDir() && fileName.EndsWith(SAVE_EXTENSION))
                    saves.Add(fileName.Replace(SAVE_EXTENSION, ""));
                fileName = dir.GetNext();
            }

            dir.ListDirEnd();
        }

        return saves;
    }

    /// <summary>
    ///     删除存档
    /// </summary>
    public void DeleteSave(string saveName)
    {
        try
        {
            var savePath = SAVE_DIR + saveName + SAVE_EXTENSION;
            if (FileAccess.FileExists(savePath))
            {
                DirAccess.RemoveAbsolute(savePath);
                GD.Print($"Save file deleted: {savePath}");
            }
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to delete save: {e.Message}");
        }
    }

    private Dictionary<string, Dictionary<string, object>> GetSceneStates()
    {
        // 从SceneManager获取场景状态
        // 这里需要实现具体的逻辑
        return new Dictionary<string, Dictionary<string, object>>();
    }

    private Dictionary<string, object> GetGlobalVariables()
    {
        // 获取需要保存的全局变量
        return new Dictionary<string, object>
        {
            // { "GameScore", GameInitializer.Instance.GameScore },
            // { "GameTime", GameInitializer.Instance.GameTime }
            // 添加其他需要保存的全局变量
        };
    }

    private Dictionary<string, object> GetPlayerStats()
    {
        // 获取玩家状态
        // 这里需要实现具体的逻辑
        return new Dictionary<string, object>();
    }

    private void RestoreSceneStates(Dictionary<string, Dictionary<string, object>> states)
    {
        // 恢复场景状态
        // 这里需要实现具体的逻辑
    }

    private void RestoreGlobalVariables(Dictionary<string, object> variables)
    {
        foreach (var variable in variables)
            switch (variable.Key)
            {
                // case "GameScore":
                //     GameInitializer.Instance.GameScore = Convert.ToInt32(variable.Value);
                //     break;
                // case "GameTime":
                //     GameInitializer.Instance.GameTime = Convert.ToSingle(variable.Value);
                //     break;
                // 处理其他全局变量
            }
    }

    private void RestorePlayerStats(Dictionary<string, object> stats)
    {
        // 恢复玩家状态
        // 这里需要实现具体的逻辑
    }

    // 存档数据结构
    public class GameSaveData
    {
        public string SaveName { get; set; }
        public DateTime SaveTime { get; set; }
        public float PlayTime { get; set; }
        public string CurrentScene { get; set; }
        public Dictionary<string, Dictionary<string, object>> SceneStates { get; set; }
        public Dictionary<string, object> GlobalVariables { get; set; }
        public Vector2 PlayerPosition { get; set; }
        public Dictionary<string, object> PlayerStats { get; set; }
        public string Checksum { get; set; }

        // 添加安全相关的元数据
        public string SaveId { get; set; } // 每个存档的唯一标识符
        public long CreatedTimestamp { get; set; } // 存档创建时间戳
        public long ModifiedTimestamp { get; set; } // 存档最后修改时间戳
        public int SaveCount { get; set; } // 存档被保存的次数
        public string PreviousChecksum { get; set; } // 上一次存档的校验和
    }

    private class EncryptionConfig
    {
        public string Key { get; set; }
        public string IV { get; set; }
    }
}