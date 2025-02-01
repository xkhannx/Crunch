using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMotion : MonoBehaviour
{
    [SerializeField] Transform sensorMin, sensorMax;
    [SerializeField] Transform fieldMin, fieldMax;
    [SerializeField] Transform fingerHalo;

    Camera cam;
    GunDelegates gunDelegates;

    void Start()
    {
        cam = Camera.main;
        gunDelegates = GetComponent<GunDelegates>();

        gunDelegates.ShotFiredFunctions += ShotFired;
        gunDelegates.TriggerReleasedFunctions += TriggerReleased;
        gunDelegates.TriggerPressedFunctions += TriggerPressed;

        SetUpField();
    }

    bool canMove = true;
    bool touchStarted = false;
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (touchStarted || WithinBorder())
            {
                touchStarted = true;
                Vector3 haloPos = cam.ScreenToWorldPoint(Input.mousePosition);

                haloPos.z = 0f;
                haloPos.x = Mathf.Clamp(haloPos.x, sensorMin.position.x, sensorMax.position.x);
                haloPos.y = Mathf.Clamp(haloPos.y, sensorMin.position.y, sensorMax.position.y);

                fingerHalo.position = haloPos;
            }
        }
        else
        {
            fingerHalo.position = Vector3.right * 500;
            touchStarted = false;
        }
        if (!canMove) return;
        MouseToSensor();
    }

    private void MouseToSensor()
    {
        Vector3 mousePos = Input.mousePosition;

        Vector2 trigger = Vector2.zero;
        
        trigger.x = mousePos.x; trigger.y = mousePos.y;
        trigger -= sensor.minBorder;
        trigger /= sensor.maxBorder - sensor.minBorder;
        
        Vector3 gunPos = field.minBorder + trigger * (field.maxBorder - field.minBorder);
        gunPos.z = 0f;
        gunPos.x = Mathf.Clamp(gunPos.x, field.minBorder.x, field.maxBorder.x);
        gunPos.y = Mathf.Clamp(gunPos.y, field.minBorder.y, field.maxBorder.y);

        transform.position = gunPos;
    }

    public bool WithinBorder()
    {
        Vector3 mousePos = Input.mousePosition;

        return mousePos.x >= sensor.minBorder.x && mousePos.x <= sensor.maxBorder.x
            && mousePos.y >= sensor.minBorder.y && mousePos.y <= sensor.maxBorder.y;
    }

    Borders sensor;
    Borders field;
    private void SetUpField()
    {
        Vector3 v = cam.WorldToScreenPoint(sensorMin.position);
        sensor.minBorder = new Vector2(v.x, v.y);
        
        v = cam.WorldToScreenPoint(sensorMax.position);
        sensor.maxBorder = new Vector2(v.x, v.y);

        field.minBorder = new Vector2(fieldMin.position.x, fieldMin.position.y);
        field.maxBorder = new Vector2(fieldMax.position.x, fieldMax.position.y);
    }
    struct Borders
    {
        public Vector2 minBorder, maxBorder;
    }

    
    void TriggerPressed()
    {
        
    }
    void TriggerReleased()
    {
        canMove = false;
    }
    void ShotFired()
    {
        canMove = true;
    }
}
