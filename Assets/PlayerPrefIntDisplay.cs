﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(TextMeshProUGUI))]
public class PlayerPrefIntDisplay : MonoBehaviour
{
    [SerializeField] private bool m_autoUpdate = false;
    [SerializeField] private string m_key = "";
    [SerializeField] private bool m_labelWithKey = false;

    private void Start() {
        UpdateDisplay();
    }

    private void Update() {
        if (m_autoUpdate)
            UpdateDisplay();
    }

    private void UpdateDisplay() {
        var val = PlayerPrefs.GetInt(m_key);
        var label = m_labelWithKey ? $"{m_key}: " : "";
        GetComponent<TextMeshProUGUI>().text = $"{label}{val}";
    }
}
