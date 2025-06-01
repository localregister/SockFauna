using ECCLibrary;
using ECCLibrary.Mono;
using Nautilus.Utility;
using Nautilus.Assets;
using SockFauna.Creatures;
using UnityEngine;

namespace SockFauna.Mono;
public class MouthBehaviour : MonoBehaviour
{
    public Creature creature;
    private Animator animator;
    private Rigidbody rb;
    private LastTarget lastTarget;
    private LiveMixin live;

    void Start()
    {
        creature = GetComponent<Creature>();
        animator = gameObject.GetComponentInChildren<Animator>();
        lastTarget = gameObject.GetComponent<LastTarget>();
        live = gameObject.GetComponent<LiveMixin>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var target = GetTarget();
        transform.LookAt(Player.main.transform.position);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    private GameObject GetTarget()
    {
        if (Player.main.GetCurrentSub() != null)
        {
            return Player.main.GetCurrentSub().gameObject;
        }
        return null;
    }
}
