using System;
using TMPro;
using UnityEngine;

public class DifficultyDisplay : MonoBehaviour
{
    private TextMeshProUGUI m_Text;
    
    private void Start()
    {
        m_Text = GetComponent<TextMeshProUGUI>();
        Debug.Assert(m_Text);
        GameManager.Instance.onDifficultyChanged += OnDifficultyChanged;
        OnDifficultyChanged(GameManager.Instance.assistLevel);
    }

    private void OnDifficultyChanged(GameAssistLevel obj)
    {
        m_Text.text = obj.ToString();
    }
}
