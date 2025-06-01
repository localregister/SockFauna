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

    void Start()
    {
        creature = GetComponent<Creature>();
        animator = gameObject.GetComponentInChildren<Animator>();
        lastTarget = gameObject.GetComponent<LastTarget>();
        live = gameObject.GetComponent<LiveMixin>();
        rb = GetComponent<Rigidbody>();
        mesmerFx = gameObject.GetComponent<MesmerizedScreenFXController>();
        SetLureState(false);
    }

    private void Update()
    {
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
