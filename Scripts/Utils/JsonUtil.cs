using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class JsonUtil
{
    private static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Converters = new List<JsonConverter>{new Vector3Converter()}
    };

    public static Dictionary<string, string> LoadAllJsonFiles(string path)
    {
        Dictionary<string, string> jsonDic = new Dictionary<string, string>();
        TextAsset[] datas = Resources.LoadAll<TextAsset>(path);

        foreach (var data in datas)
        {
            if (jsonDic.ContainsKey(data.name) == false)
                jsonDic.Add(data.name, data.text);
        }

        return jsonDic;
    }

    public static string LoadJson(string jsonPath)
    {
        TextAsset data = Resources.Load<TextAsset>(jsonPath);
        if (data == null)
            return null;

        return data.text;
    }

    ///<summary>
    ///単一のJSONテキストを指定されたクラスオブジェクトに変換（パース）
    ///</summary>
    public static T ParseJson<T>(string jsonText) where T : class
    {
        return JsonConvert.DeserializeObject<T>(jsonText);
    }

    public static void SaveToJson<T>(T data, string fileName)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, fileName);
        string directory = Path.GetDirectoryName(fullPath);

        if (Directory.Exists(directory) == false)
        {
            Directory.CreateDirectory(directory);
        }

        string jsonData = JsonConvert.SerializeObject(data, JsonSettings);
        File.WriteAllText(fullPath, jsonData);
    }

    public static void DeleteSaveDataJson(string fileName)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(fullPath)) //すでにデータがあれば削除
        {
            File.Delete(fullPath);
        }
    }

    public static T LoadSaveDataFromJson<T>(string dataName) where T: class
    {
        string fullPath = Path.Combine(Application.persistentDataPath, dataName);
        if (File.Exists(fullPath) == false)
        {
            return null;
        }
        
        string jsonData = File.ReadAllText(fullPath);
        return JsonConvert.DeserializeObject<T>(jsonData);
    }
}