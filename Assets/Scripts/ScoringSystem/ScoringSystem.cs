using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using TMPro;

public interface IScoringSystemTriggers : ITrigger
{
    public void OnDamageTaken(float damage) { }

    public void OnEnemyDeath() { }
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

    // Returns the data value of a scoring field
    public float GetScoringFieldData(string name)
    {
        return scoringFields[name].data;
    }

    // Returns the weight of a scoring field
    public float GetScoringFieldWeight(string name)
    {
        return scoringFields[name].weight;
    }

    // Sets the data and weight values of a scoring field
    public void SetScoringField(string name, float data, float weight)
    {
        scoringFields[name].data = data;
        scoringFields[name].weight = weight;
    }

    // Computes the current score based on the data and weight values of the scoring fields
    public float ComputeScore()
    { 
        float score = 0;
        foreach (var field in this.scoringFields.Values)
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
        else 
        {
            return score;
        }
    }

    // Attempts to make a purchase of the specified cost, returning true (and updating the score accordingly) if the purchase could be made and false otherwise
    public bool TryPurchase(float amount)
    {
        if (currScore < amount)
        {
            return false;
        }
        scoreSpent += amount;
        return true;
    }

    public void UpdateMatchTime()
    {
        // TODO: Pending level system completion
    }

    public void UpdateLevelTime()
    {
        // TODO: Pending level system completion
    }

    // TODO: Implement enum parameter in enemy script and corresponding checks in this function to allow for enemy type discrimination in scoring
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

    private void Start()
    {
        // Initializes dictionary with the given params
        scoringFields = new Dictionary<string, ScoringField>()
        {
            { "MatchTime", new ScoringField(0, -1) },
            { "LevelTime", new ScoringField(0, -1) },
            { "DamageTaken", new ScoringField(0, -10) },
            { "EnemiesKilled", new ScoringField(0, 100) },
        };
    }


    // Recomputes the score whenever a field is updated and updates the TextMeshProGUI element displaying the score
    public void UpdateScore()
    {
        currScore = ComputeScore();
        scoreText.text = "SCORE\n" + currScore.ToString().PadLeft(6, '0');
    }
}