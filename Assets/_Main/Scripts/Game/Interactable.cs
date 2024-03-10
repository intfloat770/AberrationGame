using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    public bool IsTargetInTrigger();
    public void MoveTargetToPosition();
}
