using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControl : MonoBehaviour
{
    Animator animator;
    GunMotion gunMotion;
    GunDelegates gunDelegates;
    GridManager grid;

    void Start()
    {
        grid = FindObjectOfType<GridManager>();
        animator = GetComponent<Animator>();
        gunMotion = GetComponent<GunMotion>();
        gunDelegates = GetComponent<GunDelegates>();

        gun = GunState.Off;
        aimRadius = GetComponent<CircleCollider2D>().radius;
    }

    bool shotStarted = false;
    bool preFire = false;
    void Update()
    {
        switch (gun)
        {
            case GunState.Off:
                if (!gunMotion.WithinBorder()) { preFire = false; return; }
                if (shotStarted) return;

                if (Input.GetMouseButton(0) || preFire)
                {
                    shotStarted = true;

                    animator.SetTrigger("windup");
                    gun = GunState.Held;
                    gunDelegates.TriggerPressedFunctions();
                }
                break;
            case GunState.Held:
                if (!Input.GetMouseButton(0))
                {
                    if (HitOrMiss())
                    {
                        animator.SetTrigger("hit");
                        AnimShotFired();
                        return;
                    }
                    else
                    {
                        animator.SetTrigger("miss");
                        
                        hitCell = grid.NodeFromWorldPoint(transform.position);
                        if (hitCell != null)
                        {
                            if (hitCell.walkable)
                            {
                                StartCoroutine(BlinkGraphic());
                            }
                        }
                    }

                    preFire = false;

                    gun = GunState.Released;
                    gunDelegates.TriggerReleasedFunctions();
                }
                break;
        }
    }

    public LayerMask enemyLayer;
    float aimRadius;
    private bool HitOrMiss()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, aimRadius, enemyLayer);
        if (colliders.Length > 0)
        {
            colliders[0].GetComponent<Enemy>().graphic.Death();
            colliders[0].GetComponent<Enemy>().dead = true;
            return true;
        }
        return false;
    }

    // Animation events
    public void AnimShotFired()
    {
        gunDelegates.ShotFiredFunctions();
        
        gun = GunState.Off;
        shotStarted = false;
        preFire = Input.GetMouseButton(0);
    }

    public enum GunState { Off, Held, Released}
    public GunState gun;

    // Blink Cell
    Node hitCell;
    IEnumerator BlinkGraphic()
    {
        SpriteRenderer graphic = hitCell.floorPrefab.GetComponentInChildren<SpriteRenderer>();
        Color cellColor = graphic.color;
        cellColor.a = 0.7f;
        graphic.color = cellColor;

        float t = 0;

        while (t < 0.5f)
        {
            t += Time.deltaTime;

            cellColor.a = 0.7f - 0.4f * t / 0.5f;
            graphic.color = cellColor;

            yield return null;
        }
        cellColor.a = 0.3f;
        graphic.color = cellColor;
    }
}
