#if UNITY_EDITOR
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public partial class SheetParsing : EditorWindow
{
    // ** 배포 받은 url 링크 **
    string sheetAPIurl = "https://script.google.com/macros/s/AKfycbzr278PgAi_BP2yTebXnpQALCOoOrDadvTjxPDgJQeAmoKsTxnXVVsqbsE2j-X1zBVtPw/exec";
    // ** 구글스프레드시트 링크 **
    string sheeturl = "https://docs.google.com/spreadsheets/d/1gA04LWKg9Xt_wUbLD8vX2N26SmjYbbUTBajAlXur5No/edit?gid=0#gid=0";

    private List<SheetData> sheets = new List<SheetData>();
    private int selectedSheetIndex = 0;
    private bool isFetching = false;

    [MenuItem("Tools/Google Sheet Parsing Tool")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow(typeof(SheetParsing));
        window.titleContent = new GUIContent("Google Sheet Parser");
        window.maxSize = new Vector2(600, 400);
        window.minSize = new Vector2(600, 400);
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        if (isFetching)
        {
            EditorGUILayout.LabelField("Fetching data...");
        }
        else
        {
            if (sheets.Count > 0)
            {
                string[] sheetNames = sheets.Select(s => s.sheetName).ToArray();
                selectedSheetIndex = EditorGUILayout.Popup("Select Sheet", selectedSheetIndex, sheetNames);
            }
            else
            {
                EditorGUILayout.LabelField("No sheets found.");
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Fetch Sheets Data", GUILayout.Height(40)))
            {
                EditorCoroutineUtility.StartCoroutine(FetchSheetsData(), this);
            }
        }

        GUILayout.Space(30);
        if (GUILayout.Button("Parse Selected Sheet and Create Class", GUILayout.Height(40)))
        {
            if (sheets.Count > 0)
            {
                ParseSelectedSheet();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please fetch sheet names and select a sheet.", "OK");
            }
        }
    }

    private IEnumerator FetchSheetsData()
    {
        isFetching = true;
        UnityWebRequest request = UnityWebRequest.Get(sheetAPIurl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ProcessSheetsData(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error fetching data: " + request.error);
        }

        isFetching = false;
        Repaint();
    }

    private void ProcessSheetsData(string json)
    {
        var sheetsData = JsonUtility.FromJson<SheetDataList>(json);
        sheets.Clear();
        sheets.AddRange(sheetsData.sheetData);

        if (sheets.Count > 0)
        {
            selectedSheetIndex = 0;
        }
    }

    private void ParseSelectedSheet()
    {
        var selectedSheet = sheets[selectedSheetIndex];
        string jsonFileName = RemoveSpecialCharacters(selectedSheet.sheetName);
        Debug.Log($"Selected Sheet: {selectedSheet.sheetName}, Sheet ID: {selectedSheet.sheetId}");

        EditorCoroutineUtility.StartCoroutine(ParseGoogleSheet(jsonFileName, selectedSheet.sheetId.ToString()), this);
    }

    private string RemoveSpecialCharacters(string sheetName)
    {
        return Regex.Replace(sheetName, @"[^a-zA-Z0-9\s]", "").Replace(" ", "_");
    }

    private IEnumerator ParseGoogleSheet(string jsonFileName, string gid, bool notice = true)
    {
        string sheetUrl = $"{sheeturl}/export?format=tsv&gid={gid}";

        UnityWebRequest request = UnityWebRequest.Get(sheetUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            EditorUtility.DisplayDialog("Fail", "GoogleConnect Fail!", "OK");
            yield break;
        }

        string data = request.downloadHandler.text;
        List<string> rows = ParseTSVData(data);

        if (rows == null || rows.Count < 4)
        {
            Debug.LogError("Not enough data rows to parse.");
            yield break;
        }

        HashSet<int> dbIgnoreColumns = GetDBIgnoreColumns(rows[0]);
        var keys = rows[1].Split('\t').ToList();
        var types = rows[2].Split('\t').ToList();

        JArray jArray = new JArray();
        for (int i = 3; i < rows.Count; i++)
        {
            var rowData = rows[i].Split('\t').ToList();

            // 첫 열이 DB_IGNORE라면 행 제외
            if (rowData[0].Equals("DB_IGNORE", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"Row {i + 1} ignored due to DB_IGNORE");
                continue;
            }

            var rowObject = ParseRow(keys, types, rowData, dbIgnoreColumns);
            if (rowObject != null)
            {
                jArray.Add(rowObject);
            }
        }

        SaveJsonToFile(jsonFileName, jArray);
        string className = CreateDataClass(jsonFileName, keys, types, dbIgnoreColumns);  // C# 클래스 생성

        if (notice)
        {
            EditorUtility.DisplayDialog("Success", "Sheet parsed and saved as JSON successfully!", "OK");
            AssetDatabase.Refresh();
        }
    }

    // TSV 데이터 파싱
    private List<string> ParseTSVData(string data)
    {
        return data.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    // DB_IGNORE 열 필터링
    private HashSet<int> GetDBIgnoreColumns(string headerRow)
    {
        var dbIgnoreColumns = new HashSet<int>();
        var firstRow = headerRow.Split('\t').ToList();

        for (int i = 0; i < firstRow.Count; i++)
        {
            if (firstRow[i].Equals("DB_IGNORE", StringComparison.OrdinalIgnoreCase))
            {
                dbIgnoreColumns.Add(i);
                Debug.Log($"Column {i + 1} ignored due to DB_IGNORE");
            }
        }

        return dbIgnoreColumns;
    }

    // 개별 행 파싱
    private JObject ParseRow(List<string> keys, List<string> types, List<string> rowData, HashSet<int> dbIgnoreColumns)
    {
        var rowObject = new JObject();

        for (int j = 0; j < keys.Count && j < rowData.Count; j++)
        {
            if (dbIgnoreColumns.Contains(j)) continue;

            string key = keys[j];
            string type = types[j];
            string value = rowData[j].Trim();

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value)) continue;

            rowObject[key] = ConvertValue(value, type);
        }

        return rowObject.HasValues ? rowObject : null;
    }

    // 값을 적절한 형식으로 변환하는 메서드
    private JToken ConvertValue(string value, string type)
    {
        switch (type.Trim()) // 불필요한 공백 제거
        {
            case "int":         return int.TryParse(value, out int intValue) ? intValue : 0;
            case "long":        return long.TryParse(value, out long longValue) ? longValue : 0L;
            case "float":       return float.TryParse(value, out float floatValue) ? floatValue : 0.0f;
            case "double":      return double.TryParse(value, out double doubleValue) ? doubleValue : 0.0d;
            case "bool":        return bool.TryParse(value, out bool boolValue) ? boolValue : false;
            case "byte":        return byte.TryParse(value, out byte byteValue) ? byteValue : (byte)0;
            case "int[]":       return JArray.FromObject(value.Split(',').Select(v => int.TryParse(v.Trim(), out int tempInt) ? tempInt : 0));
            case "float[]":     return JArray.FromObject(value.Split(',').Select(v => float.TryParse(v.Trim(), out float tempFloat) ? tempFloat : 0.0f));
            case "string[]":    return JArray.FromObject(value.Split(',').Select(v => v.Trim()));
            case "DateTime":    return DateTime.TryParse(value, out DateTime dateTimeValue) ? dateTimeValue : DateTime.MinValue; // DateTime 변환
            case "TimeSpan":    return TimeSpan.TryParse(value, out TimeSpan timeSpanValue) ? timeSpanValue : TimeSpan.Zero;
            case "Guid":        return Guid.TryParse(value, out Guid guidValue) ? guidValue.ToString() : Guid.Empty.ToString();
            default:            return value; // 기본적으로 문자열로 반환
        }
    }

    // JSON 파일 저장 메서드
    private void SaveJsonToFile(string jsonFileName, JArray jArray)
    {
        string directoryPath = Path.Combine(Application.dataPath, "Resources", "JsonFiles");

        // 폴더가 존재하지 않으면 생성
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string jsonFilePath = Path.Combine(directoryPath, $"{jsonFileName}.json");

        File.WriteAllText(jsonFilePath, jArray.ToString());
        Debug.Log($"Saved JSON to: {jsonFilePath}");
    }

    // C# 클래스 생성 메서드
    private string CreateDataClass(string fileName, List<string> keys, List<string> types, HashSet<int> dbIgnoreColumns)
    {
        string className = fileName; // 파일 이름을 클래스 이름으로 사용
        string directoryPath = Path.Combine(Application.dataPath, "Resources/DataClass");

        // 폴더가 존재하지 않으면 생성
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string dataClassPath = Path.Combine(directoryPath, $"{className}.cs");

        using (StreamWriter writer = new StreamWriter(dataClassPath))
        {
            writer.WriteLine("using System.Collections.Generic;");
            writer.WriteLine("[System.Serializable]");
            writer.WriteLine($"public class {className}");
            writer.WriteLine("{");

            // 클래스 필드 생성
            for (int i = 0; i < keys.Count; i++)
            {
                if (dbIgnoreColumns.Contains(i)) continue; // DB_IGNORE가 설정된 컬럼 건너뜀

                string fieldType = ConvertTypeToCSharp(types[i]);
                string fieldName = keys[i];

                // 필드명이 비어있지 않은지 확인
                if (!string.IsNullOrEmpty(fieldName))
                {
                    writer.WriteLine($"\tpublic {fieldType} {fieldName};");
                }
            }

            writer.WriteLine();

            // Dictionary 생성
            string keyType = ConvertTypeToCSharp(types[1]); // 첫 번째 컬럼을 Dictionary 키로 사용
            writer.WriteLine($"\tpublic static Dictionary<{keyType}, {className}> tableDic = new Dictionary<{keyType}, {className}>();");

            writer.WriteLine("}");
        }

        Debug.Log($"Saved C# class to: {dataClassPath}");
        AssetDatabase.Refresh(); // 새로 생성된 클래스를 에디터에서 인식하도록 리프레시

        return className; // 생성된 클래스 이름을 반환
    }

    private string ConvertTypeToCSharp(string type)
    {
        switch (type.Trim()) // 불필요한 공백 제거
        {
            case "int":         return "int";
            case "long":        return "long";
            case "float":       return "float";
            case "double":      return "double";
            case "bool":        return "bool";
            case "byte":        return "byte";
            case "int[]":       return "int[]";
            case "float[]":     return "float[]";
            case "string[]":    return "string[]";
            case "DateTime":    return "System.DateTime"; // DateTime에 대한 올바른 반환값
            case "TimeSpan":    return "System.TimeSpan";
            case "Guid":        return "System.Guid";
            default:            return "string"; // 기본적으로 string으로 처리
        }
    }
}

// SheetData 클래스
[System.Serializable]
public class SheetData
{
    public string sheetName;
    public int sheetId;
}

// SheetDataList 클래스
[System.Serializable]
public class SheetDataList
{
    public SheetData[] sheetData;
}
#endif