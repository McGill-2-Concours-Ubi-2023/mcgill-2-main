using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public interface IScoringSystemTriggers : ITrigger
{
    public void OnDamageTaken(float damage) { }

    public void OnEnemyDeath() { }
}

public class ScoringSystem : MonoBehaviour, IScoringSystemTriggers
{
    private const float maxScore = System.Single.MaxValue;

    [SerializeField]
    public Dictionary<string, ScoringField> scoringFields;

    // TEMP: DEBUGGING
    [SerializeField]
    public float currScore = 0;

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

    // Returns the data value of a scoring field
    public float GetScoringFieldData(string name)
    {
        return this.scoringFields[name].data;
    }

    // Returns the weight of a scoring field
    public float GetScoringFieldWeight(string name)
    {
        return this.scoringFields[name].weight;
    }

    // Sets the data and weight values of a scoring field
    public void SetScoringField(string name, float data, float weight)
    {
        this.scoringFields[name].data = data;
        this.scoringFields[name].weight = weight;
    }

    // Computes the current score based on the data and weight values of the scoring fields
    public float ComputeScore()
    { 
        float score = 0;
        foreach (var field in this.scoringFields.Values)
        {
            score += field.Value();
        }
        return score > maxScore ? maxScore : score; 
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
        this.scoringFields["EnemiesKilled"].data += 1;

        // TEMP: Debugging
        Test_ScoringSystem();
    }

    public void OnDamageTaken(float damage) 
    {
        this.scoringFields["DamageTaken"].data += damage;
        
        // TEMP: Debugging
        Test_ScoringSystem();
    }

    // TEMP: Debugging
    public void Test_ScoringSystem()
    {
        currScore = ComputeScore();
        Debug.Log("\nScore Log\n=========\n" + 
        "Total Score: " + currScore + "\n" +
        "Match Time: " + this.scoringFields["MatchTime"] + "\n" + 
        "Level Time: " + this.scoringFields["MatchTime"] + "\n" +
        "Damage Taken: " + this.scoringFields["DamageTaken"] + "\n" +
        "Enemies Killed: " + this.scoringFields["EnemiesKilled"]);
    }
}