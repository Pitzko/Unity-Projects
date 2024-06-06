using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstBoss : MonoBehaviour
{
    //data & Commender
    public FirstBossData Data;
    public Commender cmd;

    [Header("First Phase:")]
    [Header("Refrences")]
    public GameObject rightBox;
    [SerializeField] private Transform rightCurvePoint;
    [SerializeField] private Transform rightEndPoint;
    [SerializeField] private Transform rightIdlePoint;
    IEnumerator rightFloatCoroutine;
    [Space]
    public GameObject leftBox;
    [SerializeField] private Transform leftCurvePoint;
    [SerializeField] private Transform leftEndPoint;
    [SerializeField] private Transform leftIdlePoint;
    IEnumerator leftFloatCoroutine;

    //stuff
    float t = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("e"))
        {
            StartCoroutine(StartBoss());
        }
    }

    private IEnumerator StartBoss()
    {
        //starting the fight
        StartCoroutine(cmd.Move(rightBox, rightBox.transform.position, rightCurvePoint.position, rightIdlePoint.position, Data.returnToIdleTime));
        StartCoroutine(RollBoxes(rightBox, Data.boxRolls1, Data.rollDuration1));

        StartCoroutine(cmd.Move(leftBox, leftBox.transform.position, leftCurvePoint.position, leftIdlePoint.position, Data.returnToIdleTime));
        StartCoroutine(RollBoxes(leftBox, Data.boxRolls1, Data.rollDuration1));

        yield return new WaitForSecondsRealtime(Data.returnToIdleTime);

        StartCoroutine(RandomAttack());

        //start floating
        rightFloatCoroutine = cmd.Float(rightBox, rightBox.transform.position);
        leftFloatCoroutine = cmd.Float(leftBox, leftBox.transform.position);

        StartCoroutine(rightFloatCoroutine);
        StartCoroutine(leftFloatCoroutine);
    }

    private IEnumerator RandomAttack()
    {
        string box;
        float timeToWait = Random.Range(Data.timeBetweenAttacks.x, Data.timeBetweenAttacks.y + Mathf.Epsilon);
        int attackIndex = Random.Range(1, Data.numberOfAttacks + 1);

        yield return new WaitForSecondsRealtime(timeToWait);

        switch (attackIndex)
        {
            //drop attack
            case 1:
                box = (Random.Range(1, 3) == 1) ? "Right" : "Left";

                StartCoroutine(FirstAttack(box));
                break;
        }
    }

    #region Attack1

    private IEnumerator FirstAttack(string str)
    {
        if (str == "Right")
        {
            StopCoroutine(rightFloatCoroutine);

            StartCoroutine(cmd.Move(rightBox, rightBox.transform.position, rightCurvePoint.position, rightIdlePoint.position, Data.returnToIdleTime));
            StartCoroutine(RollBoxes(rightBox, Data.boxRolls1, Data.rollDuration1));

            yield return new WaitForSecondsRealtime(Data.delayBeforeDrop + Data.returnToIdleTime);

            StartCoroutine(DropBox(rightBox));
        }

        if (str == "Left")
        {
            StopCoroutine(leftFloatCoroutine);

            StartCoroutine(cmd.Move(leftBox, leftBox.transform.position, leftCurvePoint.position, leftIdlePoint.position, Data.returnToIdleTime));
            StartCoroutine(RollBoxes(leftBox, Data.boxRolls1, Data.rollDuration1));

            yield return new WaitForSecondsRealtime(Data.delayBeforeDrop + Data.returnToIdleTime);

            StartCoroutine(DropBox(leftBox));
        }
    }

    private IEnumerator RollBoxes(GameObject obj, int boxRolls, float RollDuration)
    {
        float t = 0;
        float percent = 0;

        while (percent < 1)
        {
            t += Time.deltaTime;
            percent = Mathf.Clamp01(t / (RollDuration));

            float rotationAngle = Mathf.Lerp(0, boxRolls * 360, Data.rollCurve.Evaluate(percent));

            obj.transform.rotation = Quaternion.Euler(0, 0, rotationAngle);

            yield return null;
        }

        obj.transform.rotation = Quaternion.Euler(0, 0, boxRolls * 360);
    }

    private IEnumerator DropBox(GameObject obj)
    {
        //getting the box to drop on the ground
        Vector2 dropPos = new Vector2(obj.transform.position.x, Data.yGroundLevel);

        float t = 0;
        float percent = 0;
        Vector2 currentPos = obj.transform.position;

        while (percent < 1)
        {
            t += Time.deltaTime;
            percent = Mathf.Clamp01(t / Data.dropDuration);

            Vector2 newPos = Vector2.Lerp(currentPos, dropPos, Data.dropCurve.Evaluate(percent));

            obj.transform.position = newPos;

            yield return null;
        }
    }

    #endregion

    #region Attack2



    #endregion
}