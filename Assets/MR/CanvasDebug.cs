using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasDebug : MonoBehaviour
{
    public TextMeshProUGUI errorText;
    void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if ((type == LogType.Error || type == LogType.Assert || type == LogType.Exception) || logString.StartsWith("[DEBUG]"))
        {
            errorText.SetText(errorText.text + "\n" + $"{logString}");
        }
    }
}
