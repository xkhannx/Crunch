using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsManager : MonoBehaviour
{
    [SerializeField] GameObject pointPrefab;
    [SerializeField] Text pointText;

    // Game states
    int pointsLeft;

    public int numPoints;
    public void InitPoints(int startPoints)
    {
        numPoints = startPoints;
        pointsLeft = startPoints;
        pointText.text = "Points %: 100";
    }
    
    public void RemovePoint()
    {
        pointsLeft--;
        pointText.text = "Points %: " + ((pointsLeft * 100) / numPoints).ToString();
    }
}
