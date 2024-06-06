using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Abilities : MonoBehaviour
{
    public PlayerData Data;
    [SerializeField] private Transform player;
    [SerializeField] private Transform camHolder;
    [SerializeField] private CameraMovement cm;
    private Rigidbody rb;
    [Space]

    //punch
    [Header("Punch")]
    [SerializeField] private GameObject punchHUD;
    [SerializeField] private Image chargedPunchImage;
    private Coroutine chargeCoroutine;
    private Coroutine punchCoroutine;
    private bool isChargingPunch;
    [HideInInspector] public bool isPunching;

    //timers

    // Start is called before the first frame update
    void Start()
    {
        punchHUD.SetActive(false);

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        #region Punch

        if (!isChargingPunch && Input.GetKeyDown(Data.punchKey))
        {
            isChargingPunch = true;

            chargeCoroutine = StartCoroutine(ChargePunch());
        }

        #endregion

        #region Lift

        if (Input.GetKeyDown(Data.liftKey))
        {
            StartCoroutine(Lift());
        }

        #endregion

        if (Input.GetKeyDown("e"))
        {
            rb.AddForce(transform.forward * 10, ForceMode.Impulse);
            StartCoroutine(SlipperyEffect(Data.punchSlipperyMultiplier, Data.punchSlipperyRecoveryTime));
        }

        if (Input.GetKeyDown("f"))
        {
            rb.AddForce(-transform.forward * 10, ForceMode.Impulse);
        }
    }

    #region Punch

    private IEnumerator ChargePunch()
    {
        //slows down the player
        Data.movementSpeed *= Data.punchChargeSlowMultiplier;

        //shows the HUD
        chargedPunchImage.GetComponent<Image>().fillAmount = 0;
        punchHUD.SetActive(true);

        float currentPunchDuration = Data.punchMinDuration;
        float currentPunchForce = Data.punchMinForce;

        float t = 0;

        while (t < Data.punchChargeTime)
        {
            t += Time.deltaTime;
            float percent = Mathf.Clamp01(t / Data.punchChargeTime);
            percent = Mathf.SmoothStep(0, 1, percent);

            currentPunchDuration = Mathf.Lerp(Data.punchMinDuration, Data.punchMaxDuration, percent);
            currentPunchForce = Mathf.Lerp(Data.punchMinForce, Data.punchMaxForce, percent);

            chargedPunchImage.GetComponent<Image>().fillAmount = t / Data.punchChargeTime;

            if (isChargingPunch && Input.GetKeyUp(Data.punchKey))
            {
                isChargingPunch = false;
                isPunching = true;

                punchCoroutine = StartCoroutine(Punch(currentPunchForce, currentPunchDuration));

                isChargingPunch = false;
                isPunching = true;

                StopCoroutine(chargeCoroutine);
            }

            yield return null;
        }

        currentPunchDuration = Data.punchMaxDuration;
        currentPunchForce = Data.punchMaxForce;

        t = 0;

        while (t < Data.punchChargeBonusTime)
        {
            t += Time.deltaTime;

            if (isChargingPunch && Input.GetKeyUp(Data.punchKey))
            {
                isChargingPunch = false;
                isPunching = true;

                punchCoroutine = StartCoroutine(Punch(currentPunchForce, currentPunchDuration));

                isChargingPunch = false;
                isPunching = true;

                StopCoroutine(chargeCoroutine);
            }

            yield return null;
        }

        isChargingPunch = false;
        isPunching = true;

        punchCoroutine = StartCoroutine(Punch(currentPunchForce, currentPunchDuration));
    }

    private IEnumerator Punch(float force, float duration)
    {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);

        //remove the HUD
        punchHUD.SetActive(false);
        Data.movementSpeed /= Data.punchChargeSlowMultiplier;

        //lock camera
        cm.lockedCamera = true;

        StartCoroutine(SetGravityScale(Data.gravityScale * Data.punchGravityMultiplier, 0.05f));
        StartCoroutine(SetPlayerVelocity(Vector3.zero, 0.1f));

        float t = 0;

        while (rb.velocity.magnitude < Data.punchVelocityLimit && t < duration)
        {
            t += Time.deltaTime;

            if (Input.GetKeyDown(Data.JumpKey))
            {
                rb.velocity = Vector3.zero;

                t = duration;

                StartCoroutine(PunchBoost(force * Data.punchBoostForceMultiplier, duration * Data.punchBoostDurationMultiplier));

                break;
            }

            rb.AddForce(player.forward.normalized * force * 3000 * Time.deltaTime, ForceMode.Force);

            yield return null;
        }

        while (t < duration)
        {
            t += Time.deltaTime;

            if (Input.GetKeyDown(Data.JumpKey))
            {
                rb.velocity = Vector3.zero;

                StartCoroutine(PunchBoost(force * Data.punchBoostForceMultiplier, duration * Data.punchBoostDurationMultiplier));

                break;
            }

            yield return null;
        }

        StartCoroutine(SetGravityScale(Data.gravityScale, 0.15f));
        StartCoroutine(SlipperyEffect(Data.punchSlipperyMultiplier, Data.punchSlipperyRecoveryTime));

        cm.lockedCamera = false;
        isPunching = false;
    }

    private IEnumerator PunchBoost(float force, float duration)
    {
        Vector3 dir = new Vector3(camHolder.transform.forward.x, 0, camHolder.transform.forward.z);
        Vector3 worldDirection = transform.TransformDirection(dir);

        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;

            rb.AddForce((worldDirection * force + player.up * Data.punchBoostUpForce) * 3000 * Time.deltaTime, ForceMode.Force);

            yield return null;
        }
    }

    #endregion

    #region

    private IEnumerator Lift()
    {
        SetPlayerVelocity(Vector3.zero, 0.05f);

        yield return new WaitForSecondsRealtime(0.1f);

        rb.velocity = Vector3.zero;

        float requiredVelocity = Mathf.Sqrt(2 * Mathf.Abs(Data.gravityScale) * Data.liftDistanceUp);

        rb.velocity = new Vector3(rb.velocity.x, requiredVelocity, rb.velocity.z);

        StartCoroutine(SlipperyEffect(Data.liftSlipperyMultiplier, Data.liftSlipperyRecoveryTime));

        yield return new WaitForSecondsRealtime(Data.liftGravityChangeDelay);

        GetComponent<ConstantForce>().force = new Vector3(GetComponent<ConstantForce>().force.x, GetComponent<ConstantForce>().force.y * Data.liftGravityMultiplier, GetComponent<ConstantForce>().force.z);

        StartCoroutine(SetGravityScale(Data.gravityScale, Data.liftGravityRecoveryTime));
    }

    #endregion

    #region Other Stuff

    private IEnumerator SlipperyEffect(float slipperyMult, float recoveryTime)
    {
        float originalGroundMult = Data.groundedPowerMult;
        float originalAirMult = Data.airedPowerMult;

        float t = 0;

        Data.groundedPowerMult *= slipperyMult;
        Data.airedPowerMult *= slipperyMult;

        float startGroundMult = Data.groundedPowerMult;
        float startAirMult = Data.airedPowerMult;

        while (t < recoveryTime)
        {
            t += Time.deltaTime;
            float percent = Mathf.Clamp01(t / recoveryTime);
            percent = Mathf.SmoothStep(0, 1, percent);

            Data.groundedPowerMult = Mathf.Lerp(startGroundMult, originalGroundMult, percent);
            Data.airedPowerMult = Mathf.Lerp(startAirMult, originalAirMult, percent);

            yield return null;
        }

        Data.groundedPowerMult = originalGroundMult;
        Data.airedPowerMult = originalAirMult;
    }

    private IEnumerator SetGravityScale(float desiredGravity, float time)
    {
        float currentGravity = GetComponent<ConstantForce>().force.y;
        float newGravityScale = currentGravity;

        float t = 0;

        while (t < time)
        {
            t += Time.deltaTime;
            float percent = Mathf.Clamp01(t / time);
            percent = Mathf.SmoothStep(0, 1, percent);

            newGravityScale = Mathf.Lerp(currentGravity, desiredGravity, percent);

            GetComponent<ConstantForce>().force = new Vector3(GetComponent<ConstantForce>().force.x, newGravityScale, GetComponent<ConstantForce>().force.z);

            yield return null;
        }

        GetComponent<ConstantForce>().force = new Vector3(GetComponent<ConstantForce>().force.x, desiredGravity, GetComponent<ConstantForce>().force.z);
    }

    private IEnumerator SetPlayerVelocity(Vector3 desiredVelocity, float time)
    {
        Vector3 currentVelocity = rb.velocity;
        Vector3 newVelocity = rb.velocity;

        float t = 0;

        while (t < time)
        {
            t += Time.deltaTime;
            float percent = Mathf.Clamp01(t / time);
            percent = Mathf.SmoothStep(0, 1, percent);

            newVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, percent);

            rb.velocity = newVelocity;

            yield return null;
        }

        rb.velocity = desiredVelocity;
    }

    #endregion
}
