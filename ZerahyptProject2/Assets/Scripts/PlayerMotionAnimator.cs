using UnityEngine;
using System.Collections;

public enum pState
{
    Idling = 0,
    WalkForward = 1,
    Sprinting = 2
}

public enum eState
{
    Idle = 0,
    Idle2 = 1,
    Idle3 = 2
}

[System.Serializable]
public partial class PlayerMotionAnimator : MonoBehaviour
{
    public GameObject Faceplant;
    public GameObject HurtNoise;
    public Transform FaceplantArea;
    public Transform KickArea;
    public GameObject KickPrefab;
    public GameObject KickSoundPrefab;
    public Transform CarryPointTF;
    public Transform RayPointTF;
    public Transform thisLevelTransform;
    public Transform steppypoint;
    public Transform thisTransform;
    public Rigidbody thisRigidbody;
    public CapsuleCollider PiriCapCol;
    public BoxCollider PiriBoxCol;
    public Transform pivotTF;
    public Rigidbody pivotRB;
    public HingeJoint pivotHinge;
    public HingeJoint PiriWheel;
    public Rigidbody PiriWheelRB;
    public SphereCollider PiriWheelCol;
    public float RotForce;
    public FixedJoint BrakeJoint;
    public ConfigurableJoint CarryJoint;
    public Animation PiriAni;
    public Transform PiriBaseTF;
    public Transform PiriHeadTF;
    public GameObject Pirizuka;
    public GameObject PiriUBC;
    public GameObject RBosom;
    public GameObject LBosom;
    public ConfigurableJoint RBosomCJ;
    public ConfigurableJoint LBosomCJ;
    public float maxAnimationSpeed;
    public float backwardSpeed;
    public float forwardSpeed;
    public float sprintSpeed;
    public string Idling;
    public string Idling2;
    public string Holding;
    public string forward;
    public string HoldingW;
    public string sprint;
    public string falling;
    public string floating;
    public string floatingF;
    public string HoldingOn;
    public string swimming;
    public string jump;
    public string land;
    public string kick;
    public string GunWalk;
    public string GunStrafe;
    public string GunStill;
    public string RidingMotus;
    public string RidingCepstol;
    private GameObject targetRigidbody;
    public static PlayerMotionAnimator instance;
    public bool inInterface;
    public pState state;
    public eState aState;
    public float Count;
    private Vector3 relative;
    public static float lastVelocity;
    public float acceleration;
    public float Gs;
    public bool Performing;
    public static bool CanCollide;
    public bool CanIdle;
    public bool HasJiggled;
    public bool PiriFloating;
    public bool CloseToGround;
    public float GroundClearance;
    public static bool Landing;
    public static bool PiriStill;
    public bool PiriGrounded;
    public bool CanMove;
    public bool CanFPAnimationaise;
    public static bool UsingMotus;
    public static bool Transit;
    public float JumpForce;
    public float StabilizeForce;
    public float TStabilizeForce;
    public int AngDrag;
    public Transform TC;
    public GameObject AimTarget;
    public GameObject AimSideTarget;
    public bool AimingForward;
    public bool AimingLeft;
    public bool AimingRight;
    public bool AimingBack;
    public bool keyW;
    public bool keyA;
    public bool keyS;
    public bool keyD;
    public bool InWater;
    public bool OnGround;
    public bool onMovingGround;
    public Rigidbody groundRigidbody;
    public bool Moving;
    public bool Jumping;
    public bool Carrying;
    public bool HeavyCarry;
    public bool once;
    public LayerMask targetLayers;
    public LayerMask WtargetLayers;
    public LayerMask CtargetLayers;
    public virtual void Awake()
    {
        instance = this;
    }

    public virtual void Start()
    {
        InvokeRepeating("Tick", 1, 0.2f);
        InvokeRepeating("Counter", 0.33f, 0.5f);
        AimTarget = GameObject.Find("PiriAimFront").gameObject;
        AimSideTarget = GameObject.Find("PiriAimSide").gameObject;
        lastVelocity = 0;
        GroundClearance = 2;
        targetRigidbody = thisTransform.gameObject;
        PiriStill = false;
        Landing = false;
        WorldInformation.UsingVessel = false;
        CanMove = true;
        CanFPAnimationaise = false;
        if (WorldInformation.isWearingBackpack)
        {
            GameObject Prefabionaise0 = ((GameObject)Resources.Load("Objects/" + WorldInformation.whatBackpack, typeof(GameObject))) as GameObject;
            GameObject TheThing0 = Instantiate(Prefabionaise0, PlayerInformation.instance.BackpackPoint.position, PlayerInformation.instance.BackpackPoint.rotation);
            TheThing0.name = WorldInformation.whatBackpack;
            ((BackpackScript)TheThing0.transform.GetComponent(typeof(BackpackScript))).GetWorn();
        }
    }

    public virtual IEnumerator Timer()
    {
        GroundClearance = 0.5f;
        yield return new WaitForSeconds(1);
        Jumping = false;
        GroundClearance = 2;
    }

    public virtual IEnumerator Timer2()
    {
        yield return new WaitForSeconds(0.8f);
        Landing = false;
        PiriAni.CrossFade(Idling, 0.5f);
    }

    public virtual IEnumerator Timer3()
    {

        int _2750 = 2;
        JointSpring _2751 = PiriWheel.spring;
        _2751.damper = _2750;
        PiriWheel.spring = _2751;

        thisRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        yield return new WaitForSeconds(3.3f);

        if (keyW || keyA || keyS || keyD)
        {
            CanMove = true;
            CanFPAnimationaise = false;
            thisRigidbody.constraints = RigidbodyConstraints.None;
        }
        yield return new WaitForSeconds(0.5f);
        CanMove = true;
        CanFPAnimationaise = false;
        thisRigidbody.constraints = RigidbodyConstraints.None;
    }

    public virtual IEnumerator Timer4()
    {
        RaycastHit hit = default(RaycastHit);
        GameObject TheThing = Instantiate(KickSoundPrefab, thisTransform.position, thisTransform.rotation);
        TheThing.transform.parent = thisTransform;
        yield return new WaitForSeconds(0.4f);
        if (Physics.Raycast(KickArea.transform.position + (-KickArea.transform.forward * 0.5f), KickArea.transform.forward, out hit, 1.2f, (int)targetLayers))
        {
            GameObject TheThing2 = Instantiate(KickPrefab, KickArea.transform.position, KickArea.transform.rotation);
            TheThing2.transform.parent = thisTransform;
            if (hit.rigidbody)
            {
                hit.rigidbody.AddForceAtPosition(thisTransform.forward * 4, hit.point);
            }
        }
        yield return new WaitForSeconds(0.5f);
        PiriAni.CrossFade(Idling);
        yield return new WaitForSeconds(0.3f);
        CanMove = true;
    }

    public virtual void Update()
    {
        RaycastHit hitC = default(RaycastHit);
        RaycastHit hitC2 = default(RaycastHit);
        float LastCDist = 0.0f;
        float C2Dist = 0.0f;
        bool DidHit = false;
        bool BotObs = false;
        bool IsOk = false;
        if (WorldInformation.PiriIsHurt)
        {
            return;
        }
        inInterface = CameraScript.InInterface;
        keyW = false;
        keyA = false;
        keyS = false;
        keyD = false;
        if (!inInterface)
        {
            if (Input.GetKey("w"))
            {
                keyW = true;
            }
            if (Input.GetKey("a"))
            {
                keyA = true;
            }
            if (Input.GetKey("s"))
            {
                keyS = true;
            }
            if (Input.GetKey("d"))
            {
                keyD = true;
            }
        }
        if (!Jumping)
        {
            if (Physics.Raycast(PiriBaseTF.position + (-PiriBaseTF.forward * 0.1f) + (PiriBaseTF.up * 1), -PiriBaseTF.up, 1.05f, (int)targetLayers) || Physics.Raycast(PiriBaseTF.position + (PiriBaseTF.forward * 0.2f) + (PiriBaseTF.up * 1), -PiriBaseTF.up, 1.05f, (int)targetLayers) || Physics.Raycast((PiriBaseTF.position + (PiriBaseTF.right * 0.15f)) + (PiriBaseTF.up * 1), -PiriBaseTF.up, 1.05f, (int)targetLayers) || Physics.Raycast(PiriBaseTF.position + (-PiriBaseTF.right * 0.15f) + (PiriBaseTF.up * 1), -PiriBaseTF.up, 1.05f, (int)targetLayers))
            {
                OnGround = true;
                PiriGrounded = true;
            }
            else
            {
                OnGround = false;
                PiriGrounded = false;
            }
            if (Physics.Raycast(PiriBaseTF.position + (PiriBaseTF.up * 0.2f), PiriBaseTF.forward, 0.6f, (int)targetLayers) && !Physics.Raycast(PiriBaseTF.position + (PiriBaseTF.up * 1), PiriBaseTF.forward, 1, (int)targetLayers))
            {
                OnGround = true;
                PiriGrounded = true;
            }
        }
        if (Physics.Raycast(PiriHeadTF.position, Vector3.down, 1.5f, (int)WtargetLayers))
        {
            InWater = true;
        }
        else
        {
            InWater = false;
        }
        if (WorldInformation.UsingVessel)
        {
            if (!Input.GetMouseButton(1))
            {
                TStabilizeForce = 1;
                thisRigidbody.angularDrag = 1;
                if (UsingMotus)
                {
                    PiriAni.CrossFade(RidingMotus);
                }
                else
                {
                    PiriAni.CrossFade(RidingCepstol);
                }
            }
            else
            {
                TStabilizeForce = 1;
                thisRigidbody.angularDrag = 32;
                PiriAni.Stop();
            }
        }
        else
        {
            if (Transit)
            {
                Transit = false;
                PiriFloating = false;
                Jumping = false;
                PiriAni.CrossFade(Idling);
            }
            if (WorldInformation.FPMode)
            {
                AimingForward = true;
                thisRigidbody.drag = 0;

                {
                    int _2752 = 0;
                    Vector3 _2753 = pivotTF.localEulerAngles;
                    _2753.y = _2752;
                    pivotTF.localEulerAngles = _2753;
                }
                //pivotHinge.spring.targetPosition = 0;
                if (keyW)
                {
                    if (keyA)
                    {

                        {
                            int _2754 = -45;
                            Vector3 _2755 = pivotTF.localEulerAngles;
                            _2755.y = _2754;
                            pivotTF.localEulerAngles = _2755;
                        }
                    }
                    //pivotHinge.spring.targetPosition = -45;
                    if (keyD)
                    {

                        {
                            int _2756 = 45;
                            Vector3 _2757 = pivotTF.localEulerAngles;
                            _2757.y = _2756;
                            pivotTF.localEulerAngles = _2757;
                        }
                    }
                }
                else
                {
                    //pivotHinge.spring.targetPosition = 45;
                    if (keyS)
                    {

                        {
                            int _2758 = 180;
                            Vector3 _2759 = pivotTF.localEulerAngles;
                            _2759.y = _2758;
                            pivotTF.localEulerAngles = _2759;
                        }
                        //pivotHinge.spring.targetPosition = 180;
                        if (keyA)
                        {

                            {
                                int _2760 = -135;
                                Vector3 _2761 = pivotTF.localEulerAngles;
                                _2761.y = _2760;
                                pivotTF.localEulerAngles = _2761;
                            }
                        }
                        //pivotHinge.spring.targetPosition = -135;
                        if (keyD)
                        {

                            {
                                int _2762 = 135;
                                Vector3 _2763 = pivotTF.localEulerAngles;
                                _2763.y = _2762;
                                pivotTF.localEulerAngles = _2763;
                            }
                        }
                    }
                    else
                    {
                        //pivotHinge.spring.targetPosition = 135;
                        if (keyA)
                        {

                            {
                                int _2764 = -90;
                                Vector3 _2765 = pivotTF.localEulerAngles;
                                _2765.y = _2764;
                                pivotTF.localEulerAngles = _2765;
                            }
                        }
                        //pivotHinge.spring.targetPosition = -90;
                        if (keyD)
                        {

                            {
                                int _2766 = 90;
                                Vector3 _2767 = pivotTF.localEulerAngles;
                                _2767.y = _2766;
                                pivotTF.localEulerAngles = _2767;
                            }
                        }
                    }
                }
            }
            else
            {
                //pivotHinge.spring.targetPosition = 90;
                thisRigidbody.drag = 0.1f;

                {
                    int _2768 = 0;
                    Vector3 _2769 = pivotTF.localEulerAngles;
                    _2769.y = _2768;
                    pivotTF.localEulerAngles = _2769;
                }
                //pivotHinge.spring.targetPosition = 0;
                if (Input.GetMouseButton(1))
                {
                    if (keyW)
                    {
                        if (keyA)
                        {

                            {
                                int _2770 = -45;
                                Vector3 _2771 = pivotTF.localEulerAngles;
                                _2771.y = _2770;
                                pivotTF.localEulerAngles = _2771;
                            }
                        }
                        //pivotHinge.spring.targetPosition = -45;
                        if (keyD)
                        {

                            {
                                int _2772 = 45;
                                Vector3 _2773 = pivotTF.localEulerAngles;
                                _2773.y = _2772;
                                pivotTF.localEulerAngles = _2773;
                            }
                        }
                    }
                    else
                    {
                        //pivotHinge.spring.targetPosition = 45;
                        if (keyS)
                        {

                            {
                                int _2774 = 180;
                                Vector3 _2775 = pivotTF.localEulerAngles;
                                _2775.y = _2774;
                                pivotTF.localEulerAngles = _2775;
                            }
                            //pivotHinge.spring.targetPosition = 180;
                            if (keyA)
                            {

                                {
                                    int _2776 = -135;
                                    Vector3 _2777 = pivotTF.localEulerAngles;
                                    _2777.y = _2776;
                                    pivotTF.localEulerAngles = _2777;
                                }
                            }
                            //pivotHinge.spring.targetPosition = -135;
                            if (keyD)
                            {

                                {
                                    int _2778 = 135;
                                    Vector3 _2779 = pivotTF.localEulerAngles;
                                    _2779.y = _2778;
                                    pivotTF.localEulerAngles = _2779;
                                }
                            }
                        }
                        else
                        {
                            //pivotHinge.spring.targetPosition = 135;
                            if (keyA)
                            {

                                {
                                    int _2780 = -90;
                                    Vector3 _2781 = pivotTF.localEulerAngles;
                                    _2781.y = _2780;
                                    pivotTF.localEulerAngles = _2781;
                                }
                            }
                            //pivotHinge.spring.targetPosition = -90;
                            if (keyD)
                            {

                                {
                                    int _2782 = 90;
                                    Vector3 _2783 = pivotTF.localEulerAngles;
                                    _2783.y = _2782;
                                    pivotTF.localEulerAngles = _2783;
                                }
                            }
                        }
                    }
                }
            }
            //pivotHinge.spring.targetPosition = 90;
            if (!inInterface)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    AimingForward = true;
                    AimingBack = false;
                    AimingLeft = false;
                    AimingRight = false;
                }
                if (Input.GetMouseButtonUp(1))
                {
                    AimingForward = false;
                    AimingBack = false;
                    AimingLeft = false;
                    AimingRight = false;
                }
            }
            if (!inInterface && !Input.GetMouseButton(1) && !WorldInformation.FPMode)
            {
                if (Input.GetKeyDown("w"))
                {
                    TStabilizeForce = 0;
                }
                if (Input.GetKeyDown("s"))
                {
                    TStabilizeForce = 0;
                }
                if (Input.GetKeyDown("a"))
                {
                    TStabilizeForce = 0;
                }
                if (Input.GetKeyDown("d"))
                {
                    TStabilizeForce = 0;
                }
                if (keyW)
                {
                    AimingForward = true;
                }
                if (keyS)
                {
                    if (!Carrying)
                    {
                        AimingBack = true;
                    }
                    else
                    {
                        AimingForward = true;
                    }
                }
                if (keyA)
                {
                    AimingLeft = true;
                }
                if (keyD)
                {
                    AimingRight = true;
                }
                if (Input.GetKeyUp("w"))
                {
                    AimingForward = false;
                    TStabilizeForce = 0;
                }
                if (Input.GetKeyUp("s"))
                {
                    AimingBack = false;
                    TStabilizeForce = 0;
                }
                if (Input.GetKeyUp("a"))
                {
                    AimingLeft = false;
                    TStabilizeForce = 0;
                }
                if (Input.GetKeyUp("d"))
                {
                    AimingRight = false;
                    TStabilizeForce = 0;
                }
            }
            if (OnGround)
            {
                GroundClearance = 2;
            }
            thisRigidbody.angularDrag = 48;
            if (!CanMove)
            {
                return;
            }
            Vector3 _velocity = onMovingGround ? targetRigidbody.GetComponent<Rigidbody>().velocity - groundRigidbody.velocity : targetRigidbody.GetComponent<Rigidbody>().velocity;
            relative = thisTransform.InverseTransformDirection(_velocity) / 2;
            if (Physics.Raycast(PiriBaseTF.position + (PiriBaseTF.up * 1.8f), Vector3.down, GroundClearance, (int)targetLayers))
            {
                CloseToGround = true;
            }
            else
            {
                CloseToGround = false;
            }
            if (Landing && Input.GetMouseButton(1))
            {
                CanFPAnimationaise = false;
            }
            if (WorldInformation.IsNopass)
            {
                CanFPAnimationaise = false;
            }
            if (Input.GetKeyDown(KeyCode.Space) && !Input.GetMouseButton(1))
            {
                if (!inInterface)
                {
                    if (!InWater)
                    {
                        if (!PiriFloating && CanMove && PiriGrounded && OnGround)
                        {
                            targetRigidbody = transform.gameObject;
                            thisRigidbody.AddForce(thisTransform.up * JumpForce);
                            PiriAni.Stop();
                            PiriAni.Play(jump);
                            onMovingGround = false;
                            Jumping = true;
                            OnGround = false;
                            PiriGrounded = false;
                            CanIdle = false;
                            GroundClearance = 2;
                            StopAllCoroutines();
                            StartCoroutine(Pauser());
                            StartCoroutine(Timer());
                        }
                    }
                    else
                    {
                        targetRigidbody = transform.gameObject;
                        thisRigidbody.AddForce(thisTransform.up * JumpForce);
                        PiriAni.Stop();
                        PiriAni.Play(jump);
                        onMovingGround = false;
                        Jumping = true;
                        OnGround = false;
                        PiriGrounded = false;
                        CanIdle = false;
                        GroundClearance = 2;
                        StopAllCoroutines();
                        StartCoroutine(Pauser());
                        StartCoroutine(Timer());
                    }
                    if (Carrying)
                    {
                        if (CarryJoint)
                        {
                            Destroy(CarryJoint);
                        }
                        Carrying = false;
                        WorldInformation.isHolding = false;
                        PiriCapCol.enabled = true;
                        PiriWheelCol.enabled = true;
                    }
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                if (!inInterface)
                {
                    if (Carrying)
                    {
                        if (CarryJoint)
                        {
                            Destroy(CarryJoint);
                        }
                        Carrying = false;
                        WorldInformation.isHolding = false;
                        PiriCapCol.enabled = true;
                        PiriWheelCol.enabled = true;
                        PiriBoxCol.center = new Vector3(0, -0.2f, 0);
                        PiriBoxCol.size = new Vector3(0.4f, 2.1f, 0.4f);
                        PiriAni.CrossFade(Idling);
                    }
                    else
                    {
                        if (!FurtherActionScript.IsActive)
                        {
                            if (!Input.GetMouseButton(1) && !PiriFloating && CanMove && PiriGrounded)
                            {
                                CanMove = false;
                                PiriAni.CrossFade(kick);
                                StartCoroutine(Timer4());
                                return;
                            }
                        }
                    }
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (!Carrying)
                {
                    if (FurtherActionScript.FurtherActionLMB)
                    {
                        Rigidbody CRB = null;

                        {
                            int _2784 = 0;
                            Vector3 _2785 = CarryPointTF.localPosition;
                            _2785.y = _2784;
                            CarryPointTF.localPosition = _2785;
                        }
                        if (Physics.Raycast(CarryPointTF.position + (CarryPointTF.forward * -0.3f), CarryPointTF.forward, out hitC, 1, (int)CtargetLayers))
                        {
                            if (hitC.rigidbody)
                            {
                                if (Physics.Raycast(CarryPointTF.position + (CarryPointTF.forward * 0.7f), -RayPointTF.up, out hitC2, 2, (int)CtargetLayers))
                                {
                                    BotObs = true;
                                    CRB = hitC.rigidbody;
                                    IsOk = true;
                                }
                            }
                        }
                        if (!IsOk)
                        {
                            if (Physics.Raycast(RayPointTF.position + (RayPointTF.forward * 0.1f), RayPointTF.up, out hitC2, 2, (int)CtargetLayers) || Physics.Raycast(RayPointTF.position + (RayPointTF.forward * 0.2f), RayPointTF.up, out hitC2, 2, (int)CtargetLayers) || Physics.Raycast(RayPointTF.position + (RayPointTF.forward * 0.3f), RayPointTF.up, out hitC2, 2, (int)CtargetLayers) || Physics.Raycast(RayPointTF.position + (RayPointTF.forward * 0.4f), RayPointTF.up, out hitC2, 2, (int)CtargetLayers) || Physics.Raycast(RayPointTF.position + (RayPointTF.forward * 0.5f), RayPointTF.up, out hitC2, 2, (int)CtargetLayers) || Physics.Raycast(RayPointTF.position + (RayPointTF.forward * 0.6f), RayPointTF.up, out hitC2, 2, (int)CtargetLayers) || Physics.Raycast(RayPointTF.position + (RayPointTF.forward * 0.7f), RayPointTF.up, out hitC2, 2, (int)CtargetLayers) || Physics.Raycast(RayPointTF.position + (RayPointTF.forward * 0.8f), RayPointTF.up, out hitC2, 2, (int)CtargetLayers) || Physics.Raycast(RayPointTF.position + (RayPointTF.forward * 0.9f), RayPointTF.up, out hitC2, 2, (int)CtargetLayers) || Physics.Raycast(RayPointTF.position + (RayPointTF.forward * 1), RayPointTF.up, out hitC2, 2, (int)CtargetLayers))
                            {
                                if (hitC2.rigidbody)
                                {
                                    C2Dist = hitC2.distance;

                                    {
                                        float _2786 = RayPointTF.localPosition.y;
                                        Vector3 _2787 = CarryPointTF.localPosition;
                                        _2787.y = _2786;
                                        CarryPointTF.localPosition = _2787;
                                    }

                                    {
                                        float _2788 = CarryPointTF.localPosition.y + C2Dist;
                                        Vector3 _2789 = CarryPointTF.localPosition;
                                        _2789.y = _2788;
                                        CarryPointTF.localPosition = _2789;
                                    }
                                }
                            }
                        }
                        if (!IsOk)
                        {
                            if (Physics.Raycast(CarryPointTF.position + (CarryPointTF.forward * -0.3f), CarryPointTF.forward, out hitC, 1, (int)CtargetLayers))
                            {
                                if (hitC.rigidbody)
                                {
                                    LastCDist = hitC.distance;

                                    {
                                        float _2790 = CarryPointTF.localPosition.y + 0.1f;
                                        Vector3 _2791 = CarryPointTF.localPosition;
                                        _2791.y = _2790;
                                        CarryPointTF.localPosition = _2791;
                                    }
                                    CRB = hitC.rigidbody;
                                    DidHit = true;
                                }
                            }
                            if (Physics.Raycast(CarryPointTF.position + (CarryPointTF.forward * -0.3f), CarryPointTF.forward, out hitC, 1, (int)CtargetLayers))
                            {
                                if (hitC.rigidbody)
                                {
                                    if (hitC.distance < LastCDist)
                                    {
                                        LastCDist = hitC.distance;
                                    }
                                    else
                                    {
                                        IsOk = true;
                                    }

                                    {
                                        float _2792 = CarryPointTF.localPosition.y + 0.1f;
                                        Vector3 _2793 = CarryPointTF.localPosition;
                                        _2793.y = _2792;
                                        CarryPointTF.localPosition = _2793;
                                    }
                                }
                            }
                            else
                            {
                                if (DidHit)
                                {
                                    IsOk = true;
                                }
                            }
                        }
                        if (!IsOk)
                        {
                            if (Physics.Raycast(CarryPointTF.position + (CarryPointTF.forward * -0.3f), CarryPointTF.forward, out hitC, 1, (int)CtargetLayers))
                            {
                                if (hitC.rigidbody)
                                {
                                    if (hitC.distance < LastCDist)
                                    {
                                        LastCDist = hitC.distance;
                                    }
                                    else
                                    {
                                        IsOk = true;
                                    }

                                    {
                                        float _2794 = CarryPointTF.localPosition.y + 0.1f;
                                        Vector3 _2795 = CarryPointTF.localPosition;
                                        _2795.y = _2794;
                                        CarryPointTF.localPosition = _2795;
                                    }
                                }
                            }
                        }
                        if (!IsOk)
                        {
                            if (Physics.Raycast(CarryPointTF.position + (CarryPointTF.forward * -0.3f), CarryPointTF.forward, out hitC, 1, (int)CtargetLayers))
                            {
                                if (hitC.rigidbody)
                                {
                                    if (hitC.distance < LastCDist)
                                    {
                                        LastCDist = hitC.distance;
                                    }
                                    else
                                    {
                                        IsOk = true;
                                    }

                                    {
                                        float _2796 = CarryPointTF.localPosition.y + 0.1f;
                                        Vector3 _2797 = CarryPointTF.localPosition;
                                        _2797.y = _2796;
                                        CarryPointTF.localPosition = _2797;
                                    }
                                }
                            }
                        }
                        if (!IsOk)
                        {
                            if (Physics.Raycast(CarryPointTF.position + (CarryPointTF.forward * -0.3f), CarryPointTF.forward, out hitC, 1, (int)CtargetLayers))
                            {
                                if (hitC.rigidbody)
                                {
                                    if (hitC.distance < LastCDist)
                                    {
                                        LastCDist = hitC.distance;
                                    }
                                    else
                                    {
                                        IsOk = true;
                                    }

                                    {
                                        float _2798 = CarryPointTF.localPosition.y + 0.1f;
                                        Vector3 _2799 = CarryPointTF.localPosition;
                                        _2799.y = _2798;
                                        CarryPointTF.localPosition = _2799;
                                    }
                                }
                            }
                        }
                        if (!IsOk)
                        {
                            if (Physics.Raycast(CarryPointTF.position + (CarryPointTF.forward * -0.3f), CarryPointTF.forward, out hitC, 1, (int)CtargetLayers))
                            {
                                if (hitC.rigidbody)
                                {
                                    if (hitC.distance < LastCDist)
                                    {
                                        LastCDist = hitC.distance;
                                    }
                                    else
                                    {
                                        IsOk = true;
                                    }

                                    {
                                        float _2800 = CarryPointTF.localPosition.y + 0.1f;
                                        Vector3 _2801 = CarryPointTF.localPosition;
                                        _2801.y = _2800;
                                        CarryPointTF.localPosition = _2801;
                                    }
                                }
                            }
                        }
                        if (!IsOk)
                        {
                            if (Physics.Raycast(CarryPointTF.position + (CarryPointTF.forward * -0.3f), CarryPointTF.forward, out hitC, 1, (int)CtargetLayers))
                            {
                                if (hitC.rigidbody)
                                {
                                    if (hitC.distance < LastCDist)
                                    {
                                        LastCDist = hitC.distance;
                                    }
                                    else
                                    {
                                        IsOk = true;
                                    }

                                    {
                                        float _2802 = CarryPointTF.localPosition.y + 0.1f;
                                        Vector3 _2803 = CarryPointTF.localPosition;
                                        _2803.y = _2802;
                                        CarryPointTF.localPosition = _2803;
                                    }
                                }
                            }
                        }
                        if (!IsOk)
                        {
                            if (Physics.Raycast(CarryPointTF.position + (CarryPointTF.forward * -0.3f), CarryPointTF.forward, out hitC, 1, (int)CtargetLayers))
                            {
                                if (hitC.rigidbody)
                                {
                                    if (hitC.distance < LastCDist)
                                    {
                                        LastCDist = hitC.distance;
                                    }
                                    else
                                    {
                                        IsOk = true;
                                    }

                                    {
                                        float _2804 = CarryPointTF.localPosition.y + 0.1f;
                                        Vector3 _2805 = CarryPointTF.localPosition;
                                        _2805.y = _2804;
                                        CarryPointTF.localPosition = _2805;
                                    }
                                }
                            }
                        }
                        if (CRB)
                        {
                            if ((CRB.mass > 0.1f) && (-CarryPointTF.localPosition.y > 0.5f))
                            {
                                IsOk = false;
                            }
                        }
                        if (IsOk)
                        {
                            CarryJoint = gameObject.AddComponent<ConfigurableJoint>();
                            CarryJoint.connectedBody = CRB;

                            {
                                JointDriveMode _2806 = JointDriveMode.Position;
                                JointDrive _2807 = CarryJoint.xDrive;
                                _2807.mode = _2806;
                                CarryJoint.xDrive = _2807;
                            }

                            {
                                JointDriveMode _2808 = JointDriveMode.Position;
                                JointDrive _2809 = CarryJoint.yDrive;
                                _2809.mode = _2808;
                                CarryJoint.yDrive = _2809;
                            }

                            {
                                JointDriveMode _2810 = JointDriveMode.Position;
                                JointDrive _2811 = CarryJoint.zDrive;
                                _2811.mode = _2810;
                                CarryJoint.zDrive = _2811;
                            }

                            {
                                JointDriveMode _2812 = JointDriveMode.Position;
                                JointDrive _2813 = CarryJoint.angularXDrive;
                                _2813.mode = _2812;
                                CarryJoint.angularXDrive = _2813;
                            }

                            {
                                JointDriveMode _2814 = JointDriveMode.Position;
                                JointDrive _2815 = CarryJoint.angularYZDrive;
                                _2815.mode = _2814;
                                CarryJoint.angularYZDrive = _2815;
                            }

                            {
                                int _2816 = 1;
                                JointDrive _2817 = CarryJoint.xDrive;
                                _2817.positionSpring = _2816;
                                CarryJoint.xDrive = _2817;
                            }

                            {
                                int _2818 = 1;
                                JointDrive _2819 = CarryJoint.yDrive;
                                _2819.positionSpring = _2818;
                                CarryJoint.yDrive = _2819;
                            }

                            {
                                int _2820 = 1;
                                JointDrive _2821 = CarryJoint.zDrive;
                                _2821.positionSpring = _2820;
                                CarryJoint.zDrive = _2821;
                            }

                            {
                                int _2822 = 1;
                                JointDrive _2823 = CarryJoint.angularXDrive;
                                _2823.positionSpring = _2822;
                                CarryJoint.angularXDrive = _2823;
                            }

                            {
                                int _2824 = 1;
                                JointDrive _2825 = CarryJoint.angularYZDrive;
                                _2825.positionSpring = _2824;
                                CarryJoint.angularYZDrive = _2825;
                            }

                            {
                                float _2826 = 0.05f;
                                JointDrive _2827 = CarryJoint.xDrive;
                                _2827.positionDamper = _2826;
                                CarryJoint.xDrive = _2827;
                            }

                            {
                                float _2828 = 0.1f;
                                JointDrive _2829 = CarryJoint.yDrive;
                                _2829.positionDamper = _2828;
                                CarryJoint.yDrive = _2829;
                            }

                            {
                                float _2830 = 0.05f;
                                JointDrive _2831 = CarryJoint.zDrive;
                                _2831.positionDamper = _2830;
                                CarryJoint.zDrive = _2831;
                            }

                            {
                                float _2832 = 0.05f;
                                JointDrive _2833 = CarryJoint.angularXDrive;
                                _2833.positionDamper = _2832;
                                CarryJoint.angularXDrive = _2833;
                            }

                            {
                                float _2834 = 0.05f;
                                JointDrive _2835 = CarryJoint.angularYZDrive;
                                _2835.positionDamper = _2834;
                                CarryJoint.angularYZDrive = _2835;
                            }

                            {
                                float _2836 = -hitC.distance + 0.3f;
                                Vector3 _2837 = CarryJoint.targetPosition;
                                _2837.z = _2836;
                                CarryJoint.targetPosition = _2837;
                            }
                            Carrying = true;
                            WorldInformation.isHolding = true;
                            DidHit = false;
                            IsOk = false;
                            if (CRB.mass > 0.1f)
                            {
                                HeavyCarry = true;
                                PiriCapCol.enabled = false;
                                PiriWheelCol.enabled = false;
                                PiriAni.CrossFade(HoldingOn, 0.5f);

                                {
                                    float _2838 = -CarryPointTF.localPosition.y;
                                    Vector3 _2839 = CarryJoint.targetPosition;
                                    _2839.y = _2838;
                                    CarryJoint.targetPosition = _2839;
                                }
                                PiriBoxCol.center = new Vector3(0, 0.3f, 0);
                                PiriBoxCol.size = new Vector3(0.4f, 1.1f, 0.4f);
                            }
                            else
                            {
                                HeavyCarry = false;
                                PiriCapCol.enabled = true;
                                PiriWheelCol.enabled = true;
                                PiriAni.CrossFade(Holding);
                                if (!BotObs)
                                {

                                    {
                                        float _2840 = CarryJoint.targetPosition.y + 1.6f;
                                        Vector3 _2841 = CarryJoint.targetPosition;
                                        _2841.y = _2840;
                                        CarryJoint.targetPosition = _2841;
                                    }
                                }

                                {
                                    float _2842 = CarryJoint.targetPosition.y - C2Dist;
                                    Vector3 _2843 = CarryJoint.targetPosition;
                                    _2843.y = _2842;
                                    CarryJoint.targetPosition = _2843;
                                }
                            }
                            CRB = null;
                            BotObs = false;
                            C2Dist = 0;

                            {
                                int _2844 = 0;
                                Vector3 _2845 = CarryPointTF.localPosition;
                                _2845.y = _2844;
                                CarryPointTF.localPosition = _2845;
                            }
                        }
                    }
                }
            }
            if (PiriStill && CanFPAnimationaise)
            {
                if (relative.z > 0.4f)
                {
                    PiriAni.CrossFade(GunWalk);
                    PiriAni[GunWalk].speed = Mathf.Clamp(Mathf.Abs(relative.z) / forwardSpeed, 1, maxAnimationSpeed);
                }
                if (-relative.z > 0.4f)
                {
                    PiriAni.CrossFade(GunWalk);
                    PiriAni[GunWalk].speed = Mathf.Clamp(Mathf.Abs(-relative.z) / -forwardSpeed, -1, -maxAnimationSpeed);
                }
                if ((relative.x > 0.4f) || (-relative.x > 0.4f))
                {
                    PiriAni.CrossFade(GunStrafe);
                    PiriAni[GunStrafe].speed = Mathf.Clamp(Mathf.Abs(relative.z) / forwardSpeed, 1, maxAnimationSpeed);
                }
                if ((relative.z < 0.4f) && (-relative.z < 0.4f) && (relative.x < 0.4f) && (-relative.x < 0.4f))
                {
                    PiriAni.CrossFade(GunStill);
                }
            }
            if (!Jumping)
            {
                if (PiriFloating)
                {
                    if (!CanFPAnimationaise && CanMove)
                    {
                        if (thisRigidbody.velocity.magnitude < 15)
                        {
                            if (relative.z > 0.4f)
                            {
                                if (InWater)
                                {
                                    PiriAni.CrossFade(swimming, 0.8f);
                                }
                                else
                                {
                                    if (!Carrying)
                                    {
                                        PiriAni.CrossFade(floatingF, 1f);
                                    }
                                    else
                                    {
                                        PiriAni.CrossFade(HoldingOn, 0.5f);
                                    }
                                }
                            }
                            if (relative.z < 0.4f)
                            {
                                if (!Carrying)
                                {
                                    PiriAni.CrossFade(floating, 1f);
                                }
                                else
                                {
                                    PiriAni.CrossFade(HoldingOn, 0.5f);
                                }
                            }
                        }
                        else
                        {
                            if (!Carrying)
                            {
                                PiriAni.CrossFade(falling, 1f);
                            }
                            else
                            {
                                PiriAni.CrossFade(HoldingOn, 0.5f);
                            }
                        }
                    }
                    if (PiriGrounded)
                    {
                        PiriFloating = false;
                        Landing = false;
                        if (!Input.GetMouseButton(1))
                        {
                            PiriAni.CrossFade(Idling);
                        }
                    }
                }
                else
                {
                    if (!PiriGrounded)
                    {
                        PiriFloating = true;
                    }
                }
            }
            else
            {
                if (PiriGrounded)
                {
                    PiriAni.CrossFade(land, 0.4f);
                    StartCoroutine(Timer2());
                }
                else
                {
                    PiriFloating = true;
                }
            }
            if (!inInterface)
            {
                if (Input.GetMouseButtonUp(1))
                {
                    if (!PiriFloating)
                    {
                        PiriAni.CrossFade(Idling);
                        CanFPAnimationaise = false;
                    }
                    else
                    {
                        PiriAni.CrossFade(floating);
                        CanFPAnimationaise = false;
                    }
                }
                if (Input.GetMouseButtonDown(1) && !Landing)
                {
                    PiriAni.Stop();
                    CanFPAnimationaise = true;
                    if (Carrying)
                    {
                        if (CarryJoint)
                        {
                            Destroy(CarryJoint);
                        }
                        Carrying = false;
                        WorldInformation.isHolding = false;
                        PiriCapCol.enabled = true;
                        PiriWheelCol.enabled = true;
                        PiriBoxCol.center = new Vector3(0, -0.2f, 0);
                        PiriBoxCol.size = new Vector3(0.4f, 2.1f, 0.4f);
                    }
                }
            }
            if ((Input.GetMouseButton(1) && Input.GetKeyDown(KeyCode.I)) || (Input.GetMouseButton(1) && Input.GetMouseButton(2)))
            {
                if (!Carrying)
                {
                    PiriAni.Play(Idling);
                }
                else
                {
                    if (!HeavyCarry)
                    {
                        PiriAni.Play(Holding);
                    }
                    else
                    {
                        PiriAni.Play(HoldingOn);
                    }
                }
                CanFPAnimationaise = false;
            }
            if (CanFPAnimationaise)
            {
                if (!Input.GetMouseButton(1))
                {
                    CanFPAnimationaise = false;
                    if (!Carrying)
                    {
                        PiriAni.Play(Idling);
                    }
                    else
                    {
                        if (!HeavyCarry)
                        {
                            PiriAni.Play(Holding);
                        }
                        else
                        {
                            PiriAni.Play(HoldingOn);
                        }
                    }
                }
            }
            if (PiriGrounded && !CanFPAnimationaise && !PiriFloating)
            {
                if (Input.GetKeyUp("w") && !keyA && !keyS && !keyD)
                {
                    if (!Carrying)
                    {
                        PiriAni.CrossFade(Idling);
                    }
                    else
                    {
                        if (!HeavyCarry)
                        {
                            PiriAni.CrossFade(Holding);
                        }
                        else
                        {
                            PiriAni.CrossFade(HoldingOn);
                        }
                    }
                }
                if (Input.GetKeyUp("a") && !keyW && !keyS && !keyD)
                {
                    if (!Carrying)
                    {
                        PiriAni.CrossFade(Idling);
                    }
                    else
                    {
                        if (!HeavyCarry)
                        {
                            PiriAni.CrossFade(Holding);
                        }
                        else
                        {
                            PiriAni.CrossFade(HoldingOn);
                        }
                    }
                }
                if (Input.GetKeyUp("d") && !keyW && !keyA && !keyS)
                {
                    if (!Carrying)
                    {
                        PiriAni.CrossFade(Idling);
                    }
                    else
                    {
                        if (!HeavyCarry)
                        {
                            PiriAni.CrossFade(Holding);
                        }
                        else
                        {
                            PiriAni.CrossFade(HoldingOn);
                        }
                    }
                }
                if (Input.GetKeyUp("s") && !keyW && !keyA && !keyD)
                {
                    if (!Carrying)
                    {
                        PiriAni.CrossFade(Idling);
                    }
                    else
                    {
                        if (!HeavyCarry)
                        {
                            PiriAni.CrossFade(Holding);
                        }
                        else
                        {
                            PiriAni.CrossFade(HoldingOn);
                        }
                    }
                }
                if (inInterface)
                {
                    if (!once)
                    {
                        if (!Carrying)
                        {
                            PiriAni.CrossFade(Idling);
                        }
                        else
                        {
                            if (!HeavyCarry)
                            {
                                PiriAni.CrossFade(Holding);
                            }
                            else
                            {
                                PiriAni.CrossFade(HoldingOn);
                            }
                        }
                        keyW = false;
                        keyA = false;
                        keyS = false;
                        keyD = false;
                        once = true;
                    }
                }
                else
                {
                    once = false;
                }
            }
            if (!keyW && !keyA && !keyS && !keyD)
            {
                return;
            }
            if (CanFPAnimationaise || PiriFloating || !PiriGrounded)
            {
                return;
            }
            if ((relative.z > relative.x) && (relative.z < 0.4f))
            {
                CanIdle = true;
            }
            else
            {
                if ((relative.z > relative.x) && (relative.z > 0.4f))
                {
                    CanIdle = false;
                }
            }
            //relative.z > relative.x && 
            if (relative.z > 0.4f)
            {
                if (relative.z < 2.5f)
                {
                    if (state != pState.WalkForward)
                    {
                        if (!Carrying)
                        {
                            PiriAni.CrossFade(forward);
                        }
                        else
                        {
                            if (!HeavyCarry)
                            {
                                PiriAni.CrossFade(HoldingW);
                            }
                        }
                        state = pState.WalkForward;
                    }
                    if (!Carrying)
                    {
                        if (PiriAni.IsPlaying(forward))
                        {
                            PiriAni[forward].speed = Mathf.Clamp(Mathf.Abs(relative.z) / forwardSpeed, 0, maxAnimationSpeed);
                        }
                    }
                    else
                    {
                        if (PiriAni.IsPlaying(HoldingW))
                        {
                            PiriAni[HoldingW].speed = Mathf.Clamp(Mathf.Abs(relative.z) / forwardSpeed, 0, maxAnimationSpeed);
                        }
                    }
                }
                else
                {
                    if (state != pState.Sprinting)
                    {
                        if (!Carrying)
                        {
                            PiriAni.CrossFade(sprint);
                        }
                        else
                        {
                            if (!HeavyCarry)
                            {
                                PiriAni.CrossFade(HoldingW);
                            }
                        }
                        state = pState.Sprinting;
                    }
                    if (!Carrying)
                    {
                        if (PiriAni.IsPlaying(sprint))
                        {
                            PiriAni[sprint].speed = Mathf.Clamp(Mathf.Abs(relative.z) / sprintSpeed, 0, maxAnimationSpeed);
                        }
                    }
                    else
                    {
                        if (PiriAni.IsPlaying(HoldingW))
                        {
                            PiriAni[HoldingW].speed = Mathf.Clamp(Mathf.Abs(relative.z) / sprintSpeed, 0, maxAnimationSpeed);
                        }
                    }
                }
                reset();
                state = pState.Idling;
            }
            if (Carrying)
            {
                if (-relative.z > 0.4f)
                {
                    if (!HeavyCarry)
                    {
                        PiriAni.CrossFade(HoldingW);
                    }
                    PiriAni[HoldingW].speed = -Mathf.Abs(-relative.z) / forwardSpeed;
                }
            }
        }
    }

    public virtual void reset()
    {
        if ((relative.x < 0.8f) && (relative.z < 0.8f))
        {
            if (!PiriAni.IsPlaying(Idling))
            {
                state = pState.Idling;
            }
        }
    }

    public virtual void FixedUpdate()
    {
        RaycastHit hit = default(RaycastHit);
        Vector3 localAngVel = thisTransform.InverseTransformDirection(thisRigidbody.angularVelocity);
        acceleration = (thisRigidbody.velocity.magnitude - lastVelocity) / Time.deltaTime;
        lastVelocity = thisRigidbody.velocity.magnitude;
        float RPVelClamp = Mathf.Clamp(lastVelocity * 0.035f, 0, 128);
        float RPVC = 0.75f + RPVelClamp;
        Gs = acceleration;
        if (!WorldInformation.UsingVessel)
        {
            if (OnGround)
            {
                if (AimingForward || AimingBack || AimingLeft || AimingRight)
                {
                    if (TStabilizeForce < 8)
                    {
                        TStabilizeForce = TStabilizeForce + 0.5f;
                    }
                }
            }
            else
            {
                TStabilizeForce = 2;
            }
            if (!onMovingGround)
            {
                float VelClamp1 = Mathf.Clamp(lastVelocity, 0, 8);
                thisRigidbody.AddTorque(thisTransform.forward * -localAngVel.y * VelClamp1 * 0.3f);
            }
            else
            {
                float VelClamp2 = Mathf.Clamp(relative.magnitude, 0, 8);
                thisRigidbody.AddTorque(thisTransform.forward * -localAngVel.y * VelClamp2 * 0.3f);
            }
            if (Gs < -2500)
            {
                if (!WorldInformation.PiriIsHurt)
                {
                    CanCollide = false;
                    StartCoroutine(Pauser());
                    CanFPAnimationaise = false;
                    PiriUpperBodyController.Resetting = true;
                    CanMove = false;
                    PiriAni.Stop();
                    PiriAni.CrossFade("PiriArmature|Faceplant");
                    GameObject TheThing4 = Instantiate(Faceplant, FaceplantArea.transform.position, FaceplantArea.transform.rotation);
                    TheThing4.transform.parent = thisTransform;
                    PiriFloating = false;
                    StartCoroutine(Timer3());
                    Hurt2();
                }
            }
            if (CanCollide)
            {
                if (Gs < -500)
                {
                    CanCollide = false;
                    StartCoroutine(Pauser());
                    CanFPAnimationaise = false;
                    PiriUpperBodyController.Resetting = true;
                    CanMove = false;
                    PiriAni.Stop();
                    PiriAni.CrossFade("PiriArmature|Faceplant");
                    GameObject TheThing5 = Instantiate(Faceplant, FaceplantArea.transform.position, FaceplantArea.transform.rotation);
                    TheThing5.transform.parent = thisTransform;
                    PiriFloating = false;
                    StartCoroutine(Timer3());
                }
            }
            if (CanCollide && CanMove && !Input.GetMouseButton(1))
            {
                if (Gs < -200)
                {
                    CanCollide = false;
                    StartCoroutine(Pauser());
                    CanFPAnimationaise = false;
                    PiriUpperBodyController.Resetting = true;
                    PiriAni.Stop();
                    if (!Input.GetMouseButton(1))
                    {
                        PiriAni.Play(land);
                    }
                    Landing = true;
                    PiriGrounded = true;
                    PiriFloating = false;
                    StartCoroutine(Timer2());
                }
            }
        }
        if (Input.GetMouseButton(1) && WorldInformation.UsingVessel && CanMove && !inInterface)
        {
            thisRigidbody.AddForceAtPosition((AimTarget.transform.position - thisTransform.position).normalized * TStabilizeForce, thisTransform.forward * 1);
            thisRigidbody.AddForceAtPosition((AimTarget.transform.position - thisTransform.position).normalized * -TStabilizeForce, -thisTransform.forward * 1);
        }
        if (CloseToGround)
        {
            if (!WorldInformation.UsingVessel)
            {
                thisRigidbody.AddForceAtPosition(Vector3.up * StabilizeForce, thisTransform.up * 2);
                thisRigidbody.AddForceAtPosition(-Vector3.up * StabilizeForce, -thisTransform.up * 2);
            }
        }
        else
        {
            thisRigidbody.AddForceAtPosition(Vector3.up * StabilizeForce, thisTransform.up * 0.2f);
            thisRigidbody.AddForceAtPosition(-Vector3.up * StabilizeForce, -thisTransform.up * 0.2f);
        }
        if (CanMove && !WorldInformation.UsingVessel)
        {
            if (AimingForward && (thisRigidbody.angularVelocity.magnitude < 5))
            {
                thisRigidbody.AddForceAtPosition((AimTarget.transform.position - thisTransform.position).normalized * TStabilizeForce, thisTransform.forward * 1);
                thisRigidbody.AddForceAtPosition((AimTarget.transform.position - thisTransform.position).normalized * -TStabilizeForce, -thisTransform.forward * 1);
            }
            if (AimingBack && (thisRigidbody.angularVelocity.magnitude < 5))
            {
                thisRigidbody.AddForceAtPosition((AimTarget.transform.position - thisTransform.position).normalized * TStabilizeForce, -thisTransform.forward * 1);
                thisRigidbody.AddForceAtPosition((AimTarget.transform.position - thisTransform.position).normalized * -TStabilizeForce, thisTransform.forward * 1);
            }
            if (AimingLeft && (thisRigidbody.angularVelocity.magnitude < 5))
            {
                thisRigidbody.AddForceAtPosition((AimSideTarget.transform.position - thisTransform.position).normalized * TStabilizeForce, -thisTransform.forward * 1);
                thisRigidbody.AddForceAtPosition((AimSideTarget.transform.position - thisTransform.position).normalized * -TStabilizeForce, thisTransform.forward * 1);
            }
            if (AimingRight && (thisRigidbody.angularVelocity.magnitude < 5))
            {
                thisRigidbody.AddForceAtPosition((AimSideTarget.transform.position - thisTransform.position).normalized * TStabilizeForce, thisTransform.forward * 1);
                thisRigidbody.AddForceAtPosition((AimSideTarget.transform.position - thisTransform.position).normalized * -TStabilizeForce, -thisTransform.forward * 1);
            }
            Vector3 newRot = ((-thisLevelTransform.up * 2) + (-thisLevelTransform.forward * 2)).normalized;
            Debug.DrawRay(steppypoint.position + (thisLevelTransform.forward * 0.75f), newRot * 0.5f, Color.white);
            if (OnGround)
            {
                if (keyW || keyA || keyS || keyD)
                {
                    Vector3 _velocity = onMovingGround ? thisRigidbody.velocity - groundRigidbody.velocity : thisRigidbody.velocity;
                    if (!inInterface)
                    {
                        if (WorldInformation.FPMode)
                        {
                            if (Physics.Raycast(steppypoint.position + (thisLevelTransform.forward * 0.75f), newRot, 0.5f, (int)targetLayers))
                            {
                                thisRigidbody.AddForce(thisTransform.up * 1.5f);
                                thisRigidbody.AddForce(thisTransform.forward * 2);
                            }
                        }
                        else
                        {
                            if (_velocity.magnitude < 4)
                            {
                                if (_velocity.magnitude < 2)
                                {
                                    if (Physics.Raycast(steppypoint.position + (thisLevelTransform.forward * 0.75f), newRot, 0.5f, (int)targetLayers))
                                    {
                                        thisRigidbody.AddForce(thisTransform.up * 1.5f);
                                        thisRigidbody.AddForce(thisTransform.forward * 1.5f);
                                    }
                                }
                                else
                                {
                                    if (Physics.Raycast(steppypoint.position + (thisLevelTransform.forward * 0.75f), newRot, 0.5f, (int)targetLayers))
                                    {
                                        thisRigidbody.AddForce(thisTransform.up * 1);
                                        thisRigidbody.AddForce(thisTransform.forward * 1);
                                    }
                                }
                            }
                        }
                        if (PiriWheelRB.angularVelocity.magnitude < 7)
                        {
                            if (Carrying)
                            {
                                if (Input.GetKey("s"))
                                {
                                    if (!WorldInformation.FPMode)
                                    {
                                        if (RotForce > -0.5f)
                                        {
                                            RotForce = RotForce - 0.02f;
                                        }
                                    }
                                    else
                                    {
                                        if (RotForce < 0.5f)
                                        {
                                            RotForce = RotForce + 0.02f;
                                        }
                                    }
                                }
                                else
                                {
                                    if (RotForce < 0.5f)
                                    {
                                        RotForce = RotForce + 0.02f;
                                    }
                                }
                            }
                            else
                            {
                                if (RotForce < 1)
                                {
                                    RotForce = RotForce + 0.02f;
                                }
                            }
                        }
                        else
                        {
                            if (PiriWheelRB.angularVelocity.magnitude < 19)
                            {
                                if (Carrying)
                                {
                                    if (Input.GetKey("s"))
                                    {
                                        if (!WorldInformation.FPMode)
                                        {
                                            if (RotForce < 0)
                                            {
                                                RotForce = RotForce + 0.02f;
                                            }
                                        }
                                        else
                                        {
                                            if (RotForce > 0)
                                            {
                                                RotForce = RotForce - 0.02f;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (RotForce > 0)
                                        {
                                            RotForce = RotForce - 0.02f;
                                        }
                                    }
                                }
                                else
                                {
                                    if (!Input.GetKey(KeyCode.LeftShift))
                                    {
                                        if (RotForce > 0)
                                        {
                                            RotForce = RotForce - 0.02f;
                                        }
                                    }
                                    else
                                    {
                                        if (!Input.GetMouseButton(1))
                                        {
                                            if (RotForce < 0.5f)
                                            {
                                                RotForce = RotForce + 0.02f;
                                            }
                                        }
                                        else
                                        {
                                            if (RotForce > 0)
                                            {
                                                RotForce = RotForce - 0.02f;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (RotForce > 0)
                                {
                                    RotForce = RotForce - 0.02f;
                                }
                            }
                            if (PiriWheel.spring.damper < 0.001f)
                            {

                                {
                                    float _2846 = PiriWheel.spring.damper + 0.0001f;
                                    JointSpring _2847 = PiriWheel.spring;
                                    _2847.damper = _2846;
                                    PiriWheel.spring = _2847;
                                }
                            }
                        }
                        if (PiriWheel.spring.damper > 0.001f)
                        {

                            {
                                float _2848 = 0.001f;
                                JointSpring _2849 = PiriWheel.spring;
                                _2849.damper = _2848;
                                PiriWheel.spring = _2849;
                            }
                        }
                        PiriWheelRB.AddTorque(pivotRB.transform.right * RotForce);
                        if (BrakeJoint)
                        {
                            Destroy(BrakeJoint);
                        }
                        Moving = true;
                    }
                    else
                    {
                        Moving = false;
                        if (CanMove)
                        {

                            {
                                int _2850 = 8;
                                JointSpring _2851 = PiriWheel.spring;
                                _2851.damper = _2850;
                                PiriWheel.spring = _2851;
                            }
                        }
                        RotForce = 0;
                    }
                }
                else
                {
                    if (!keyW && !keyA && !keyS && !keyD)
                    {
                        if (!BrakeJoint)
                        {
                            BrakeJoint = PlayerInformation.instance.PiriWheel.AddComponent<FixedJoint>();
                            BrakeJoint.connectedBody = pivotRB;
                        }
                        Moving = false;
                        if (CanMove)
                        {

                            {
                                int _2852 = 8;
                                JointSpring _2853 = PiriWheel.spring;
                                _2853.damper = _2852;
                                PiriWheel.spring = _2853;
                            }
                        }
                        RotForce = 0;
                    }
                }
            }
            else
            {
                if (WorldInformation.instance.AreaSpace)
                {
                    if (keyW || keyS || keyA || keyD)
                    {
                        thisRigidbody.AddForce(thisTransform.forward * 0.1f);
                    }
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        thisRigidbody.AddForce(thisTransform.up * -0.1f);
                    }
                    if (Input.GetKey(KeyCode.Space))
                    {
                        thisRigidbody.AddForce(thisTransform.up * 0.1f);
                    }
                }
                else
                {
                    if (InWater)
                    {
                        if (keyW || keyS || keyA || keyD)
                        {
                            thisRigidbody.AddForce(thisTransform.forward * 0.5f);
                        }
                    }
                }
            }
            //-------------------------------------------------------------------------------------------------------------------------|
            if (Physics.Raycast(PiriBaseTF.position + (PiriBaseTF.up * 1), -PiriBaseTF.up, out hit, 5, (int)targetLayers) && hit.rigidbody)
            {
                if (hit.rigidbody.velocity.magnitude > 2)
                {
                    onMovingGround = true;
                    groundRigidbody = hit.rigidbody;
                    if (relative.magnitude < 1)
                    {
                        if (StabilizeForce < 50)
                        {
                            StabilizeForce = StabilizeForce + 1;
                        }
                    }
                    else
                    {
                        StabilizeForce = 10;
                    }
                }
                else
                {
                    if (thisRigidbody.velocity.magnitude < 1)
                    {
                        if (StabilizeForce < 50)
                        {
                            StabilizeForce = StabilizeForce + 1;
                        }
                    }
                    else
                    {
                        StabilizeForce = 10;
                    }
                }
            }
            else
            {
                if (thisRigidbody.velocity.magnitude < 1)
                {
                    if (StabilizeForce < 50)
                    {
                        StabilizeForce = StabilizeForce + 1;
                    }
                }
                else
                {
                    StabilizeForce = 10;
                }
                onMovingGround = false;
                groundRigidbody = thisRigidbody;
            }
            if (groundRigidbody == null)
            {
                groundRigidbody = thisRigidbody;
                onMovingGround = false;
            }
        }
        //-------------------------------------------------------------------------------------------------------------------------|
        if (!WorldInformation.PiriIsHurt)
        {
            if (!CanIdle || WorldInformation.UsingVessel || Performing)
            {
                return;
            }
            if (PiriStill || Landing || PiriFloating || !CanMove)
            {
                return;
            }
            if (Count < 2.5f)
            {
                return;
            }
            Count = 0;
            int randomValue = Random.Range(0, 10);
            switch (randomValue)
            {
                case 1:
                    if (!WorldInformation.FPMode && !Carrying)
                    {
                        aState = eState.Idle2;
                        PiriAni.CrossFade(Idling2);
                        StopCoroutine("Jiggle");
                        StartCoroutine(Jiggle());
                    }
                    break;
                case 2:
                    if (!WorldInformation.FPMode && !Carrying)
                    {
                        aState = eState.Idle3;
                        PiriAni.CrossFade(Idling2);
                        StopCoroutine("Jiggle");
                        StartCoroutine(Jiggle());
                    }
                    break;
                default:
                    //When randomValue is not 1 or 2
                    if (!Carrying)
                    {
                        aState = eState.Idle;
                        PiriAni.CrossFade(Idling);
                    }
                    else
                    {
                        if (!HeavyCarry)
                        {
                            PiriAni.CrossFade(Holding);
                        }
                        else
                        {
                            PiriAni.CrossFade(HoldingOn);
                        }
                    }
                    break;
            }
        }
    }

    public virtual IEnumerator Jiggle()
    {
        yield return new WaitForSeconds(1.35f);
        if (PiriAni.IsPlaying(Idling2))
        {
            RBosom.GetComponent<Rigidbody>().AddForce(thisTransform.up * 0.07f);
        }
        if (PiriAni.IsPlaying(Idling2))
        {
            LBosom.GetComponent<Rigidbody>().AddForce(thisTransform.up * 0.07f);
        }
        yield return new WaitForSeconds(0.05f);
        if (PiriAni.IsPlaying(Idling2))
        {
            RBosom.GetComponent<Rigidbody>().AddForce(thisTransform.up * 0.07f);
        }
        if (PiriAni.IsPlaying(Idling2))
        {
            LBosom.GetComponent<Rigidbody>().AddForce(thisTransform.up * 0.07f);
        }
        yield return new WaitForSeconds(0.05f);
        if (PiriAni.IsPlaying(Idling2))
        {
            RBosom.GetComponent<Rigidbody>().AddForce(thisTransform.up * 0.07f);
        }
        if (PiriAni.IsPlaying(Idling2))
        {
            LBosom.GetComponent<Rigidbody>().AddForce(thisTransform.up * 0.07f);
        }
        yield return new WaitForSeconds(0.13f);
        if (PiriAni.IsPlaying(Idling2))
        {
            RBosom.GetComponent<Rigidbody>().AddForce(thisTransform.up * -0.2f);
        }
        if (PiriAni.IsPlaying(Idling2))
        {
            LBosom.GetComponent<Rigidbody>().AddForce(thisTransform.up * -0.2f);
        }
    }

    public virtual void PlayAnimation(string ani)
    {
        if (!PiriAni.IsPlaying(ani))
        {
            PiriAni.CrossFade(ani);
        }
    }

    public virtual IEnumerator Hurt()
    {
        if (!WorldInformation.Godmode)
        {
            WorldInformation.PiriIsHurt = true;
            CanMove = false;
            TC.name = "snyf";
            PiriAni.Stop();
            Instantiate(HurtNoise, thisTransform.position, thisTransform.rotation);
            //WheelBrake
            yield return new WaitForSeconds(0.2f);
            PiriAni.CrossFade("PiriArmature|Faceplant");
            StartCoroutine(WorldInformation.instance.Hurt());
        }
    }

    public virtual void Hurt2()
    {
        if (!WorldInformation.Godmode)
        {
            WorldInformation.PiriIsHurt = true;
            CanMove = false;
            TC.name = "snyf";
            StartCoroutine(WorldInformation.instance.Hurt());
        }
    }

    public virtual void Counter()
    {
        Count = Count + 0.5f;
    }

    public virtual IEnumerator Pauser()
    {
        yield return new WaitForSeconds(0.3f);
        CanCollide = true;
    }

    public virtual void Tick()
    {
        if (Pirizuka.activeSelf)
        {
            StartCoroutine(Pauser());
        }
        if (thisRigidbody.velocity.magnitude < 15)
        {

            {
                int _2854 = 0;
                JointDrive _2855 = RBosomCJ.angularXDrive;
                _2855.positionDamper = _2854;
                RBosomCJ.angularXDrive = _2855;
            }

            {
                int _2856 = 0;
                JointDrive _2857 = RBosomCJ.angularYZDrive;
                _2857.positionDamper = _2856;
                RBosomCJ.angularYZDrive = _2857;
            }

            {
                float _2858 = 0.002f;
                JointDrive _2859 = RBosomCJ.angularXDrive;
                _2859.positionSpring = _2858;
                RBosomCJ.angularXDrive = _2859;
            }

            {
                float _2860 = 0.002f;
                JointDrive _2861 = RBosomCJ.angularYZDrive;
                _2861.positionSpring = _2860;
                RBosomCJ.angularYZDrive = _2861;
            }

            {
                int _2862 = 0;
                JointDrive _2863 = LBosomCJ.angularXDrive;
                _2863.positionDamper = _2862;
                LBosomCJ.angularXDrive = _2863;
            }

            {
                int _2864 = 0;
                JointDrive _2865 = LBosomCJ.angularYZDrive;
                _2865.positionDamper = _2864;
                LBosomCJ.angularYZDrive = _2865;
            }

            {
                float _2866 = 0.002f;
                JointDrive _2867 = LBosomCJ.angularXDrive;
                _2867.positionSpring = _2866;
                LBosomCJ.angularXDrive = _2867;
            }

            {
                float _2868 = 0.002f;
                JointDrive _2869 = LBosomCJ.angularYZDrive;
                _2869.positionSpring = _2868;
                LBosomCJ.angularYZDrive = _2869;
            }
        }
        if (thisRigidbody.velocity.magnitude > 15)
        {

            {
                float _2870 = 0.0002f;
                JointDrive _2871 = RBosomCJ.angularXDrive;
                _2871.positionDamper = _2870;
                RBosomCJ.angularXDrive = _2871;
            }

            {
                float _2872 = 0.0002f;
                JointDrive _2873 = RBosomCJ.angularYZDrive;
                _2873.positionDamper = _2872;
                RBosomCJ.angularYZDrive = _2873;
            }

            {
                float _2874 = 0.006f;
                JointDrive _2875 = RBosomCJ.angularXDrive;
                _2875.positionSpring = _2874;
                RBosomCJ.angularXDrive = _2875;
            }

            {
                float _2876 = 0.006f;
                JointDrive _2877 = RBosomCJ.angularYZDrive;
                _2877.positionSpring = _2876;
                RBosomCJ.angularYZDrive = _2877;
            }

            {
                float _2878 = 0.0002f;
                JointDrive _2879 = LBosomCJ.angularXDrive;
                _2879.positionDamper = _2878;
                LBosomCJ.angularXDrive = _2879;
            }

            {
                float _2880 = 0.0002f;
                JointDrive _2881 = LBosomCJ.angularYZDrive;
                _2881.positionDamper = _2880;
                LBosomCJ.angularYZDrive = _2881;
            }

            {
                float _2882 = 0.006f;
                JointDrive _2883 = LBosomCJ.angularXDrive;
                _2883.positionSpring = _2882;
                LBosomCJ.angularXDrive = _2883;
            }

            {
                float _2884 = 0.006f;
                JointDrive _2885 = LBosomCJ.angularYZDrive;
                _2885.positionSpring = _2884;
                LBosomCJ.angularYZDrive = _2885;
            }
        }
        if (thisRigidbody.velocity.magnitude > 50)
        {

            {
                float _2886 = 0.0004f;
                JointDrive _2887 = RBosomCJ.angularXDrive;
                _2887.positionDamper = _2886;
                RBosomCJ.angularXDrive = _2887;
            }

            {
                float _2888 = 0.0004f;
                JointDrive _2889 = RBosomCJ.angularYZDrive;
                _2889.positionDamper = _2888;
                RBosomCJ.angularYZDrive = _2889;
            }

            {
                float _2890 = 0.01f;
                JointDrive _2891 = RBosomCJ.angularXDrive;
                _2891.positionSpring = _2890;
                RBosomCJ.angularXDrive = _2891;
            }

            {
                float _2892 = 0.01f;
                JointDrive _2893 = RBosomCJ.angularYZDrive;
                _2893.positionSpring = _2892;
                RBosomCJ.angularYZDrive = _2893;
            }

            {
                float _2894 = 0.0004f;
                JointDrive _2895 = LBosomCJ.angularXDrive;
                _2895.positionDamper = _2894;
                LBosomCJ.angularXDrive = _2895;
            }

            {
                float _2896 = 0.0004f;
                JointDrive _2897 = LBosomCJ.angularYZDrive;
                _2897.positionDamper = _2896;
                LBosomCJ.angularYZDrive = _2897;
            }

            {
                float _2898 = 0.01f;
                JointDrive _2899 = LBosomCJ.angularXDrive;
                _2899.positionSpring = _2898;
                LBosomCJ.angularXDrive = _2899;
            }

            {
                float _2900 = 0.01f;
                JointDrive _2901 = LBosomCJ.angularYZDrive;
                _2901.positionSpring = _2900;
                LBosomCJ.angularYZDrive = _2901;
            }
        }
        if (!Input.GetMouseButton(1) && !WorldInformation.FPMode)
        {
            AimingForward = false;
            AimingBack = false;
            AimingLeft = false;
            AimingRight = false;
        }
    }

    public PlayerMotionAnimator()
    {
        maxAnimationSpeed = 2f;
        backwardSpeed = 1f;
        forwardSpeed = 1f;
        sprintSpeed = 1f;
        aState = eState.Idle;
        GroundClearance = 0.5f;
        CanMove = true;
        JumpForce = 100;
        StabilizeForce = 10f;
        TStabilizeForce = 10f;
        AngDrag = 40;
    }

}