using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public static bool isDebugging = true; // set to true to enable Debug.Logs
    public static void Info(string message)
    {
        if (!isDebugging)
            return;
        else
            Debug.Log(message);
    }

    public static void Error(string message)
    {
        if (!isDebugging)
            return;
        else
            Debug.LogError(message);
    }
}
