using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunDelegates : MonoBehaviour
{
    public delegate void ShotFiredDelegate();
    public ShotFiredDelegate ShotFiredFunctions;

    public delegate void TriggerReleasedDelegate();
    public TriggerReleasedDelegate TriggerReleasedFunctions;

    public delegate void TriggerPressedDelegate();
    public TriggerPressedDelegate TriggerPressedFunctions;
}
