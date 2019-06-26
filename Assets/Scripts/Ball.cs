using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ball : MonoBehaviour
{
    private static Ball _instance;
    public static Ball Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.Find("Ball").GetComponent<Ball>();
            }
            return _instance;
        }
    }

    public class OnSetHexEvent : UnityEvent<Hex> { }
    public OnSetHexEvent SetHexEvent = new OnSetHexEvent();

    public Hex hex;
    Animator anim;

    Transform holder;

    // Start is called before the first frame update
    void Start()
    {
        GetHex();

        holder = GetComponentInParent<Transform>();
        anim = GetComponentInChildren<Animator>();
    }

    public void GetHex() {
        RaycastHit hitInfo;

        if (Physics.Raycast(new Ray(transform.position + transform.up, -transform.up), out hitInfo, 3f, Mouse.Instance.HexMask)) {
            var h = hitInfo.transform.GetComponentInParent<Hex>();
            if(h != hex) {
                SetHex(h);
            }
        }
    }

    public void Teleport(Hex h) {
        transform.position = h.transform.position;
        GetHex();
    }

    public void SetHex(Hex h) {
        hex = h;
        SetHexEvent.Invoke(hex);
        hex.SetBall(this);
    }

    /*
    public void Shoot(Hex h) {
        anim.SetTrigger("Idle");

        var v = h.transform.position;

        Debug.Log("shoot");
        col.enabled = false;
        Invoke("EnableCol", .2f);
        var dist = Vector3.Distance(transform.position, v);
        var angle = Mathf.Clamp(dist * 10f, 0f, 45f);
        rb.useGravity = true;
        rb.velocity = BallisticVel(v, angle, dist);
        beingShot = true;
    }*/

    bool beingShot;
    Team shotBy;
    public Team BeingShotBy() {
        if (beingShot) {
            return shotBy;
        }
        else return null;
    }

    public void Shoot(Hex h) {
        anim.SetTrigger("Idle");

        Debug.Log("shoot");
        col.enabled = false;
        Invoke("EnableCol", .2f);
        var dist = Vector3.Distance(transform.position, h.transform.position);
        var angle = Mathf.Clamp(dist * 10f, 0f, 45f);
        rb.useGravity = true;
        rb.velocity = BallisticVel(h.transform.position, angle, dist);

        if (h.flags.Contains(Hex.Flag.basket)) {
            beingShot = true;
            shotBy = GameController.Instance.teamWithPossession;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    


    public float damage;

    Rigidbody rb;
    Collider col;
    bool stuck = false;

    AudioSource audioSource;
    public List<AudioClip> HitSFX = new List<AudioClip>();
    public List<AudioClip> HitGroundSFX = new List<AudioClip>();

    public void Awake() {
        audioSource = gameObject.AddComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        col = GetComponentInChildren<Collider>();
    }

    // Use this for initialization
    public void Init(Transform t, string s) {
        col.enabled = false;
    }

    public void Init(Vector3 p, string s) {

    }

    void EnableCol() {
        col.enabled = true;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (stuck) return;
        Vector3 v = rb.velocity.normalized;
        if (v.magnitude > 0) {
            Quaternion wanted_rotation = Quaternion.LookRotation(v);//create the rotation
            transform.rotation = wanted_rotation;
        }
    }

    public string targetTag;

    private void OnTriggerEnter(Collider collision) {
        
        //GetComponentInChildren<Collider>().enabled = false;

        if (collision.transform.parent.name.Contains("Hex")) {
            SetHex(collision.GetComponentInParent<Hex>());
            rb.useGravity = false;
            stuck = true;
            rb.velocity = Vector3.zero;
        }
    }

    public Transform target;
    public GameObject projectile;
    public float angle;

    public float accuracy;

    Vector3 BallisticVel(Transform target, float angle, float _dist) {
        var v = target.GetComponent<Rigidbody>().velocity;
        var adj = target.position + (v) + Random.insideUnitSphere * accuracy * Mathf.Log((Vector3.Distance(target.position, transform.position)) + 1f);
        var dir = adj - transform.position;  // get target direction
        var h = dir.y;  // get height difference
        dir.y = 0;  // retain only the horizontal direction
        var dist = dir.magnitude;  // get horizontal distance
        var a = angle * Mathf.Deg2Rad;  // convert angle to radians
        dir.y = dist * Mathf.Tan(a);  // set dir to the elevation angle
        dist += h / Mathf.Tan(a);  // correct for small height differences
                                   // calculate the velocity magnitude
        var vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return vel * dir.normalized;
    }

    Vector3 BallisticVel(Vector3 target, float angle, float _dist) {
        var t = target + Random.insideUnitSphere * accuracy * Mathf.Log((Vector3.Distance(target, transform.position)) + 1f);
        var dir = t - transform.position;  // get target direction
        var h = dir.y;  // get height difference
        dir.y = 0;  // retain only the horizontal direction
        var dist = dir.magnitude;  // get horizontal distance
        var a = angle * Mathf.Deg2Rad;  // convert angle to radians
        dir.y = dist * Mathf.Tan(a);  // set dir to the elevation angle
        dist += h / Mathf.Tan(a);  // correct for small height differences
                                   // calculate the velocity magnitude
        var vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return vel * dir.normalized;
    }
}
