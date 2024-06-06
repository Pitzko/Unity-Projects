using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class PufferFish : NetworkBehaviour
{
    [SerializeField] private float scaleMult;
    [SerializeField] private float scaleAnimationMult;
    [SerializeField] private float scaleDuration;
    [SerializeField] private int totalScaleTimes;
    private int scaleTimes = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (Input.GetKeyDown("b"))
        {
            StartCoroutine(Scale());
        }
    }

    private IEnumerator Scale()
    {
        Vector2 currentScale = transform.localScale;
        Vector2 desiredAnimationScale = currentScale * scaleAnimationMult;
        Vector2 desiredScale = currentScale * scaleMult;

        float t = 0;

        while (t < scaleDuration)
        {
            t += Time.deltaTime;
            float percent = Mathf.Clamp01(t / scaleDuration);
            percent = Mathf.SmoothStep(0, 1, percent);

            transform.localScale = Vector2.Lerp(currentScale, desiredAnimationScale, percent);

            yield return null;
        }

        t = 0;

        while (t < scaleDuration / 2)
        {
            t += Time.deltaTime;
            float percent = Mathf.Clamp01(t / (scaleDuration / 2));
            percent = Mathf.SmoothStep(0, 1, percent);

            transform.localScale = Vector2.Lerp(desiredAnimationScale, desiredScale, percent);

            yield return null;
        }

        scaleTimes++;
    }
}
