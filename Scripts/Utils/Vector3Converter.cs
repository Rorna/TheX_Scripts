using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

public class Vector3Converter : JsonConverter<Vector3>
{
    public override bool CanRead => false;

    // セーブ：x, y, zのみを抽出して書き出す。
    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("x"); writer.WriteValue(value.x);
        writer.WritePropertyName("y"); writer.WriteValue(value.y);
        writer.WritePropertyName("z"); writer.WriteValue(value.z);
        writer.WriteEndObject();
    }

    // ロード: x, y, zの値を読み取り、Vector3として復元する。
    public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        // JSONデータをJObjectとしてパース。
        JObject jo = JObject.Load(reader);

        // x, y, zの値を安全に取得（値が存在しない場合は0fで初期化）。
        float x = jo["x"]?.Value<float>() ?? 0f;
        float y = jo["y"]?.Value<float>() ?? 0f;
        float z = jo["z"]?.Value<float>() ?? 0f;

        return new Vector3(x, y, z);
    }

}
