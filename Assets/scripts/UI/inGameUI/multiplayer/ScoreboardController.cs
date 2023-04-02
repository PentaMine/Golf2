using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardController : MonoBehaviour
{
    private GameManager gameManager = GameManager.instance;
    private TextMeshProUGUI textComponent;

    private void Awake()
    {
        Golf2Socket.OnGameEnd += HandleGameEnd;
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    private void HandleGameEnd(List<Golf2Socket.PlayerScore> scores)
    {
        string text = "";
        int place = 1;
        foreach (Golf2Socket.PlayerScore score in scores)
        {
            text += $"{place}: {score.name}: {score.score:0.00}\n";
            place++;
        }

        textComponent.text = text;
    }
}