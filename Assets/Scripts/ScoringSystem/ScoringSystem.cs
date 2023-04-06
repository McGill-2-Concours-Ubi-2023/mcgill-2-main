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
    private const float maxScore = System.Single.MaxValue;

    [SerializeField]
    public float currScore = 0;

    [SerializeField]
    public Dictionary<string, ScoringField> scoringFields;

    [SerializeField]
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        // Initializes dictionary with the given params
        scoringFields = new Dictionary<string, ScoringField>()
        {
            { "MatchTime", new ScoringField(0, -1) },
            { "LevelTime", new ScoringField(0, -1) },
            { "DamageTaken", new ScoringField(0, -10) },
            { "EnemiesKilled", new ScoringField(0, 100) }
        };
    }

    // Note: Should probably be refactored to not recompute the score in Update
    private void Update()
    {
        currScore = ComputeScore();
        UpdateText();
    }

    // Updates the TextMeshProGUI element displaying the score
    private void UpdateText()
    {
        scoreText.text = "SCORE\n" + currScore.ToString().PadLeft(6, '0');
    }

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
    }

    public void OnDamageTaken(float damage) 
    {
        scoringFields["DamageTaken"].data += damage;
    }

    // TEMP: Debugging
    public void Test_ScoringSystem()
    {
        currScore = ComputeScore();
        Debug.Log("\nScore Log\n=========\n" + 
        "Total Score: " + currScore + "\n" +
        "Match Time: " + scoringFields["MatchTime"] + "\n" + 
        "Level Time: " + scoringFields["MatchTime"] + "\n" +
        "Damage Taken: " + scoringFields["DamageTaken"] + "\n" +
        "Enemies Killed: " + scoringFields["EnemiesKilled"]);
    }
}