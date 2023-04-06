using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

[System.Serializable]
public class ScoringField
{
    public float data;
    public float weight;

    public ScoringField(float data, float weight) 
    { 
        this.data = data;
        this.weight = weight; 
    } 
    
    public float Value()
    { 
        return this.data * this.weight;
    }
    
    public override string ToString()
    { 
        return this.data + " " + this.weight; 
    }
}