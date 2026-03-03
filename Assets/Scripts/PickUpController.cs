using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickUpController : MonoBehaviour
{
    public ProjectileGun gunScript;
    public WeaponAnim WeaponAnimScript;
    public Rigidbody rb;
    public BoxCollider coll;
    public Transform player, gunContainer, fpsCam;

    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;

    public bool equipped;
    public bool youCanInteract;
    public static bool slotFull;

    public WeaponAnim weaponAnim;

    private void Start()
    {
        //Setup
        if (!equipped)
        {
            gunScript.enabled = false;
            WeaponAnimScript.enabled = false;
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate; 
            coll.isTrigger = false;
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("Default"));
        }
        if (equipped)
        {
            gunScript.enabled = true;
            WeaponAnimScript.enabled=true;
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.interpolation = RigidbodyInterpolation.None;
            coll.isTrigger = true;
            slotFull = true;
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("Weapon"));
        }

        weaponAnim = GetComponentInChildren<WeaponAnim>();
    }

    private void Update()
    {
        //Check if player is in range and "E" is pressed
        Vector3 distanceToPlayer = player.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E) && !slotFull) PickUp();

        //Drop if equipped and "Q" is pressed
        if (equipped && Input.GetKeyDown(KeyCode.Q) && !weaponAnim.isAiming) Drop();

        if (!equipped && distanceToPlayer.magnitude <= pickUpRange && !slotFull)
            youCanInteract = true;
        else 
            youCanInteract = false;

        gunScript.UpdatePrssEToInteractUI(youCanInteract);
    }

    private void PickUp()
    {
        equipped = true;
        slotFull = true;

        //Make weapon a child of the camera and move it to default position
        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        //Make Rigidbody kinematic and BoxCollider a trigger
        rb.isKinematic = true;
        coll.isTrigger = true;
        rb.interpolation = RigidbodyInterpolation.None;
        rb.useGravity = false;

        //Enable scripts
        gunScript.enabled = true;
        WeaponAnimScript.enabled = true;

        gunScript.UpdateBulletsConterUI(equipped);
        gunScript.UpdateCrosshairUI(equipped);
        //gunScript.UpdatePrssEToInteractUI(youCanInteract);

        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Weapon"));
    }

    private void Drop()
    {
        equipped = false;
        slotFull = false;

        // Set parent to null
        transform.SetParent(null);

        // Make Rigidbody not kinematic and BoxCollider normal
        rb.isKinematic = false;
        coll.isTrigger = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.useGravity = true;

        // Gun carries momentum of player
        rb.velocity = player.GetComponent<Rigidbody>().velocity;

        // Add Force
        rb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse);

        // Add random rotation
        float random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);

        // Disable scripts
        gunScript.enabled = false;
        WeaponAnimScript.enabled = false;

        gunScript.UpdateBulletsConterUI(equipped);
        gunScript.UpdateCrosshairUI(equipped);
        //gunScript.UpdatePrssEToInteractUI(youCanInteract);

        SetLayerRecursively(gameObject, LayerMask.NameToLayer("Default"));
    }


    private void SetLayerRecursively(GameObject obj, int layer)
    {
        if (layer == -1) // Check if layer exists
        {
            Debug.LogError("Layer does not exist. Please ensure the layer is created in Tags and Layers.");
            return;
        }

        obj.layer = layer; // Set layer for the current object

        // Iterate through and set the layer for each child
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
