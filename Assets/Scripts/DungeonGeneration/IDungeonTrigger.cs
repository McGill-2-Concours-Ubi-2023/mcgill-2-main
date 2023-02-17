using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDungeonTrigger : ITrigger
{
    public void ReceiveMessage(string message) { }
}
