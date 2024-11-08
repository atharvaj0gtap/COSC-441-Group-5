using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVManager : MonoBehaviour
{
    // File path to the CSV
    private static string filePath;

    // Method to set the file path and create directories if they don't exist
    public static void SetFilePath(string folderPath, string cursorType)
    {
        // Ensure the folder exists
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("Directory created at: " + folderPath);
        }

        // Set the CSV file path
        filePath = Path.Combine(folderPath, $"data_{cursorType}.csv");
    }

    // Method to write data to a CSV file
    public static void WriteToCSV(List<string[]> data)
    {
        using (StreamWriter sw = new StreamWriter(filePath))
        {
            foreach (string[] line in data)
            {
                sw.WriteLine(string.Join(",", line));
            }
        }
    }

    public static void AppendToCSV(string[] data)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("File path is null or empty. Cannot append to CSV.");
            return;
        }

        // Use StreamWriter with append set to true
        using (StreamWriter sw = new StreamWriter(filePath, true))
        {
            sw.WriteLine(string.Join(",", data));
        }

        Debug.Log($"Data successfully appended to {filePath}");
    }

    // Method to read data from a CSV file
    public List<string[]> ReadFromCSV(string path)
    {
        List<string[]> data = new List<string[]>();

        if (File.Exists(path))
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] values = line.Split(',');
                    data.Add(values);
                }
            }

            Debug.Log($"Data successfully read from {path}");
        }
        else
        {
            Debug.LogError($"File not found: {path}");
        }

        return data;
    }
}
