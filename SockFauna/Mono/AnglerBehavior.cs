using ECCLibrary;
using ECCLibrary.Mono;
using Nautilus.Utility;
using UnityEngine;

namespace SockFauna.Mono;
public class AnglerBehaviour : MonoBehaviour
{
    public Creature creature;
    private Animator animator;
    private Rigidbody rb;
    private LastTarget lastTarget;
    private LiveMixin live;

    private bool luring;
    private float showDistance = 56;

    private float timeLureAgain;

    private MesmerizedScreenFXController mesmerFx;

    public FMOD_CustomEmitter grabVehicleEmitter;

    private Vehicle _heldVehicle;
    private VehicleType _heldVehicleType;
    private float _timeVehicleGrabbed;
    private float _timeVehicleReleased;
    private Quaternion _vehicleInitialRotation;
    private Vector3 _vehicleInitialPosition;
    private Transform _vehicleHoldPoint;
    private AnglerMeleeAttack _mouthAttack;
    private CreatureVoice _voice;
    private static readonly FMODAsset _seamothSounds = AudioUtils.GetFmodAsset("AbyssalBlazaSeamothAttack");
    private static readonly FMODAsset _exosuitSounds = AudioUtils.GetFmodAsset("AbyssalBlazaExosuitAttack");
    private static readonly FMODAsset _roarSound = AudioUtils.GetFmodAsset("MultiGargBite");

    private readonly float _damagePerSecond = 10f;

    void Start()
    {
        creature = GetComponent<Creature>();
        _vehicleHoldPoint = gameObject.transform.SearchChild("SeamothCineAttachDisplace").transform;
        _mouthAttack = GetComponent<AnglerMeleeAttack>();
        _voice = GetComponent<CreatureVoice>();

        animator = gameObject.GetComponentInChildren<Animator>();
        lastTarget = gameObject.GetComponent<LastTarget>();
        live = gameObject.GetComponent<LiveMixin>();
        rb = GetComponent<Rigidbody>();
        mesmerFx = gameObject.GetComponent<MesmerizedScreenFXController>();
        SetLureState(false);
    }

    Transform GetHoldPoint()
    {
        return _vehicleHoldPoint;
    }

    public bool IsHoldingVehicle()
    {
        return _heldVehicleType != VehicleType.None;
    }

    /// <summary>
    /// Holding Seamoth or Seatruck.
    /// </summary>
    /// <returns></returns>
    public bool IsHoldingGenericSub()
    {
        return _heldVehicleType == VehicleType.GenericSub;
    }

    public bool IsHoldingExosuit()
    {
        return _heldVehicleType == VehicleType.Exosuit;
    }

    private enum VehicleType
    {
        None = 0,
        Exosuit = 1,
        GenericSub = 2
    }

    public void GrabGenericSub(Vehicle vehicle)
    {
        GrabVehicle(vehicle, VehicleType.GenericSub);
    }

    public void GrabExosuit(Vehicle exosuit)
    {
        GrabVehicle(exosuit, VehicleType.Exosuit);
    }

    public bool GetCanGrabVehicle()
    {
        return _timeVehicleReleased + 10f < Time.time && !IsHoldingVehicle();
    }

    private void GrabVehicle(Vehicle vehicle, VehicleType vehicleType)
    {
        vehicle.GetComponent<Rigidbody>().isKinematic = true;
        vehicle.collisionModel.SetActive(false);
        _heldVehicle = vehicle;
        _heldVehicleType = vehicleType;
        if (_heldVehicleType == VehicleType.Exosuit)
        {
            SafeAnimator.SetBool(vehicle.mainAnimator, "reaper_attack", true);
            Exosuit component = vehicle.GetComponent<Exosuit>();
            if (component != null)
            {
                component.cinematicMode = true;
            }
        }

        _timeVehicleGrabbed = Time.time;
        _vehicleInitialRotation = vehicle.transform.rotation;
        _vehicleInitialPosition = vehicle.transform.position;
        if (_heldVehicleType == VehicleType.GenericSub)
        {
            grabVehicleEmitter.SetAsset(_seamothSounds);
        }
        else if (_heldVehicleType == VehicleType.Exosuit)
        {
            grabVehicleEmitter.SetAsset(_exosuitSounds);
        }
        else
        {
            Plugin.Logger.LogWarning("Unknown Vehicle Type detected");
        }
        SafeAnimator.SetBool(creature.GetAnimator(), "exosuit_attack", true);
        SafeAnimator.SetBool(creature.GetAnimator(), "exosuit_attack", true);

        grabVehicleEmitter.Play();
        InvokeRepeating("DamageVehicle", 1f, 1f);
        float attackLength = 4 + UnityEngine.Random.value;
        Invoke("ReleaseVehicle", attackLength);
        if (Player.main.GetVehicle() == _heldVehicle)
        {
            MainCameraControl.main.ShakeCamera(4f, attackLength, MainCameraControl.ShakeMode.BuildUp, 1.2f);
        }
    }

    private void DamageVehicle()
    {
        if (_heldVehicle != null)
        {
            _heldVehicle.liveMixin.TakeDamage(_damagePerSecond, default, DamageType.Normal, null);
        }
    }

    public void ReleaseVehicle()
    {
        if (_heldVehicle != null)
        {
            if (_heldVehicleType == VehicleType.Exosuit)
            {
                SafeAnimator.SetBool(_heldVehicle.mainAnimator, "reaper_attack", false);
                Exosuit component = _heldVehicle.GetComponent<Exosuit>();
                if (component != null)
                {
                    component.cinematicMode = false;
                }
            }

            _heldVehicle.GetComponent<Rigidbody>().isKinematic = false;
            _heldVehicle.collisionModel.SetActive(true);
            _heldVehicle = null;
            _timeVehicleReleased = Time.time;
        }

        SafeAnimator.SetBool(creature.GetAnimator(), "exosuit_attack", false);
        SafeAnimator.SetBool(creature.GetAnimator(), "exosuit_attack", false);

        _heldVehicleType = VehicleType.None;
        CancelInvoke("DamageVehicle");
        _mouthAttack.OnVehicleReleased();
        MainCameraControl.main.ShakeCamera(0f, 0f);
        Utils.PlayFMODAsset(_roarSound, transform.position);
    }

    public void Update()
    {
        if (_heldVehicleType != VehicleType.None && _heldVehicle == null)
        {
            ReleaseVehicle();
        }
        if (_heldVehicle != null)
        {
            Transform holdPoint = GetHoldPoint();
            float num = Mathf.Clamp01(Time.time - _timeVehicleGrabbed);
            if (num >= 1f)
            {
                _heldVehicle.transform.position = holdPoint.position;
                _heldVehicle.transform.rotation = holdPoint.transform.rotation;
                return;
            }

            _heldVehicle.transform.position =
                (holdPoint.position - this._vehicleInitialPosition) * num + this._vehicleInitialPosition;
            _heldVehicle.transform.rotation = Quaternion.Lerp(this._vehicleInitialRotation, holdPoint.rotation, num);
        }
        if (creature == null || creature.liveMixin == null || !creature.liveMixin.IsAlive())
        {
            SetLureState(false);
            return;
        }
        if (!luring && Time.time > timeLureAgain)
        {
            SetLureState(false);
        }
    }

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if ((damageInfo.type == DamageType.Electrical || damageInfo.type == DamageType.Poison) && _heldVehicle != null)
        {
            ReleaseVehicle();
        }
    }

    void OnDisable()
    {
        if (_heldVehicle != null)
        {
            ReleaseVehicle();
        }
    }
    private void KillPlayer()
    {
        Player.main.liveMixin.Kill();
    }

    private void OnDestroy()
    {
        if (mesmerFx) mesmerFx.StopHypnose();
    }

    public void SetLureState(bool state)
    {
        luring = state;
        animator.SetBool("lure", state);
        animator.Play("None", 4);
        rb.isKinematic = state;
        live.invincible = state;
    }
}
