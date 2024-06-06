using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commender : MonoBehaviour
{
    public FirstBossData FirstBossData;
    private float t = 0;

    public IEnumerator Move(GameObject obj, Vector2 A, Vector2 B, Vector2 C, float desiredTime)
    {
        float t = 0;
        float percent = 0;

        while (percent < 1)
        {
            t += Time.deltaTime;
            percent = Mathf.Clamp01(t / desiredTime);

            float easing = Mathf.SmoothStep(0, 1, percent);

            Vector2 AB = Vector2.Lerp(A, B, easing);
            Vector2 BC = Vector2.Lerp(B, C, easing);

            Vector2 AC = Vector2.Lerp(AB, BC, easing);

            obj.transform.position = AC;

            yield return null;
        }

        obj.transform.position = C;
    }

    public IEnumerator Float(GameObject obj, Vector2 yPos)
    {
        t = 0;

        while (true)
        {
            t += Time.deltaTime;

            obj.transform.position = new Vector2(yPos.x, yPos.y + FirstBossData.distanceMult * Mathf.Sin(FirstBossData.floatSpeed * t));

            yield return null;
        }
    }
}
