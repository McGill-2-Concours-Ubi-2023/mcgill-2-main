using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public interface IScoringSystemTriggers : ITrigger
{
    public void OnEnemyDeath() { }

    public void OnDamageTaken(float damage) { }

    public void OnCrateCollect() { }

    public void OnCrateSpawn() { }
}

public class ScoringSystem : MonoBehaviour, IScoringSystemTriggers
{
    private const float maxScore = 999999;

    [SerializeField]
    public float currScore = 0;

    [SerializeField]
    public Dictionary<string, ScoringField> scoringFields;

    [SerializeField]
    public TextMeshProUGUI scoreText;

    private float scoreSpent;

    public ScoringField GetScoringField(string name)
    {
        return scoringFields[name];
    }

    public void SetScoringField(string name, float data, float weight)
    {
        scoringFields[name].data = data;
        scoringFields[name].weight = weight;
    }

    public void UpdateScore()
    {
        currScore = ComputeScore();
        if (scoreText)
        {
            scoreText.text = "SCORE\n" + currScore.ToString().PadLeft(6, '0');
        }
    }
    
    // Computes the current score based on the data and weight values of the scoring fields
    public float ComputeScore()
    {
        float score = 0;
        foreach (var field in scoringFields.Values)
        {
            score += field.Value();
        }
        score -= scoreSpent;
        if (score < 0)
        {
            return 0;
        } 
        else if (score > maxScore)
        {
            return maxScore;
        }
        return score;
    }

    // Attempts to resolve a purchase of the specified cost, returning true (and updating the score accordingly) if the purchase could be completed and false otherwise
    public bool TryPurchase(float amount)
    {
        if (currScore < amount)
        {
            return false;
        }
        scoreSpent += amount;
        return true;
    }

    public void OnEnemyDeath()
    {
        scoringFields["EnemiesKilled"].data += 1;
        UpdateScore();
    }

    public void OnDamageTaken(float damage) 
    {
        scoringFields["DamageTaken"].data += damage;
        UpdateScore();
    }

    public void OnCrateCollect()
    {
        scoringFields["CratesDestroyed"].data += 1;
        UpdateScore();
    }

    public void OnCrateSpawn()
    {
        scoringFields["CratesSpawned"].data += 1;
        UpdateScore();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        scoreText = GameObject.Find("Points").GetComponent<TextMeshProUGUI>();
        scoringFields = new Dictionary<string, ScoringField>()
        {
            { "ClearTime", new ScoringField(0, 0) }, // Should be negatively weighted for desired behavior, currently disabled so weight set to 0
            { "EnemiesKilled", new ScoringField(0, 50) },
            { "DamageTaken", new ScoringField(0, 0) }, // Should be negatively weighted for desired behavior, currently disabled so weight set to 0
            { "CratesDestroyed", new ScoringField(0, 25) },
            { "CratesSpawned", new ScoringField(0, 25) }
        };

    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BossScene") {
            scoreText = GameObject.Find("Points").GetComponent<TextMeshProUGUI>();
        }
    }
}