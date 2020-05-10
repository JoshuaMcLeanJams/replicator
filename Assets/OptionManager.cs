﻿using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class OptionManager : MonoBehaviour, Controls.IAimActions, Controls.IFireActions
{
    [SerializeField] private TextMeshProUGUI m_optionLabel = null;
    [SerializeField] private TextMeshProUGUI m_optionValue = null;

    private enum Option
    {
        InvertY,
        AimSensitivityX,
        AimSensitivityY,
        PieceCount,
        ResetHighScore
    }

    private Option CurOption => (Option)m_curOptionIndex;
    private string Key => SpaceCapitals(CurOption.ToString());

    private int Value {
        get { return m_value; }
        set {
            m_value = Mathf.Clamp(value, m_valueMin, m_valueMax);
            PlayerPrefs.SetInt(Key, m_value);
        }
    }

    private int m_curOptionIndex = 0;
    private int m_value = 0;
    private int m_valueMin = 0;
    private int m_valueMax = 100;
    private int m_valueInc = 1;

    private int CurOptionIndex {
        get { return m_curOptionIndex; }
        set {
            m_curOptionIndex = value;
            if (m_curOptionIndex < 0)
                m_curOptionIndex = (int)Option.ResetHighScore;
            else if (m_curOptionIndex > (int)Option.ResetHighScore)
                m_curOptionIndex = 0;
            m_valueInc = 1;
                m_valueMin = 0;
            switch (CurOption) {
                case Option.InvertY:
                    LoadOption(1);
                    m_valueMax = 1;
                    break;
                case Option.AimSensitivityX:
                    LoadOption(1);
                    m_valueMax = 100;
                    break;
                case Option.AimSensitivityY:
                    LoadOption(1);
                    m_valueMax = 100;
                    break;
                case Option.PieceCount:
                    LoadOption(30);
                    m_valueMin = 30;
                    m_valueMax = 1000;
                    m_valueInc = 30;
                    break;
                case Option.ResetHighScore:
                    m_optionLabel.text = "Reset High Score";
                    m_valueMin = m_valueMax = 0;
                    break;
            }
        }
    }

    public void OnAim(InputAction.CallbackContext context) {
        if (context.started == false)
            return;

        var move = context.ReadValue<Vector2>();
        if (Mathf.Abs(move.x) > Mathf.Abs(move.y)) {
            // left
            if (move.x < 0) {
                --CurOptionIndex;
                return;
            }

            // right
            ++CurOptionIndex;
            return;
        }

        // down
        if (move.y < 0) {
            Value -= m_valueInc;
            return;
        }

        // up
        Value += m_valueInc;
    }

    public void OnFire(InputAction.CallbackContext context) {
        if (context.started == false)
            return;

        if(CurOption == Option.ResetHighScore)
            PlayerPrefs.SetInt("High Score", 0);

        var controller = GetComponent<Controller>();
        controller.Controls.Aim.SetCallbacks(null);
        controller.Controls.Fire.SetCallbacks(null);
        SceneManager.LoadScene("main");
        Debug.Log("Start game");
    }

    private void Start() {
        var controller = GetComponent<Controller>();
        if (controller == null) {
            Destroy(this);
            return;
        }
        controller.Controls.Aim.SetCallbacks(this);
        controller.Controls.Fire.SetCallbacks(this);

        CurOptionIndex = 0;
    }

    void Update()
    {
        m_optionValue.text = $"{m_value}";
        switch (CurOption) {
            case Option.AimSensitivityX:
                break;
            case Option.AimSensitivityY:
                break;
            case Option.PieceCount:
                break;
            case Option.InvertY:
                m_optionValue.text = m_value == 0 ? "UP IS UP" : "UP IS DOWN";
                break;
            case Option.ResetHighScore:
                m_optionValue.text = "Press FIRE to RESET high score AND start new game";
                break;
            default:
                break;
        }
    }

    private void LoadOption(int a_default) {
        if (PlayerPrefs.HasKey(Key) == false)
            PlayerPrefs.SetInt(Key, a_default);
        m_value = PlayerPrefs.GetInt(Key);
        m_optionLabel.text = Key;
    }

    string SpaceCapitals(string a_str) {
        return string.Join(" ", Regex.Split(a_str, @"(?<!^)(?=[A-Z])"));
    }
}