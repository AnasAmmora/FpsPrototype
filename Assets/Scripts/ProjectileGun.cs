
using UnityEngine;
using TMPro;
using Cinemachine;
using UnityEngine.UI;
using StarterAssets;

public class ProjectileGun : MonoBehaviour
{
    [SerializeField] private FirstPersonController player;
    //[Header("Shoot Setting")]
    [SerializeField] private float shootForce, upwardForce;
    [SerializeField] private float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    [SerializeField] private int magazineSize, bulletsPerTap;
    [SerializeField] private bool allowButtonHold;
    private bool _allowButtonHold;
    [SerializeField] private int bulletsLeft, bulletsRight;

    [Header("Player Recoil")]
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private float recoilForce;

    [Header("Reference")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private Camera fpsCam;
    [SerializeField] private Transform attackPoint;

    [Header("Graphics & UI")]
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject youCanInteract;

    [Header("VFX")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioClip emptyMagazineSound;


    int bulletsShot;
    //bools
    bool shooting, readyToShoot, reloading;

    //bug fixing :D
    private bool allowInvoke = true;

    private WeaponAnim WeaponAnim;
    private CinemachineImpulseSource recoilShakeImpulseSource;

    private void Awake()
    {
        //make sure magazine is full
        bulletsLeft = magazineSize;
        readyToShoot = true;

        // take allowButtonHold value
        _allowButtonHold = allowButtonHold;
    }
    private void Start()
    {
        WeaponAnim = GetComponentInChildren<WeaponAnim>();
        recoilShakeImpulseSource = GetComponentInChildren<CinemachineImpulseSource>();
    }

    private void Update()
    {
        MyInput();
        UpdateBulletsConterUI(true);
    }
    private void MyInput()
    {
        //Check if allowed to hold down button and take corresponding input
        if (_allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //Reloading 
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();
        //Reload automatically when trying to shoot without ammo
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        //Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            //Set bullets shot to 0
            bulletsShot = 0;

            Shoot();
        }

        
    }

    private void Shoot()
    {
        readyToShoot = false;

        //Find the exact hit position using a raycast
        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); //Just a ray through the middle of your current view
        RaycastHit hit;

        //check if ray hits something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75); //Just a point far away from the player

        //Calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //Calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0); //Just add spread to last direction

        //Instantiate bullet/projectile
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity); //store instantiated bullet in currentBullet  
        //Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        //Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        //Instantiate muzzle flash
        if (muzzleFlash != null)
            Instantiate(muzzleFlash, attackPoint.position, attackPoint.rotation);



        bulletsLeft--;
        bulletsShot++;

        //Invoke resetShot function (if not already invoked), with your timeBetweenShooting
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;

            //Add recoil to player (should only be called once)
            //playerRb.AddForce(-directionWithSpread.normalized * recoilForce, ForceMode.Impulse);
        }

        //if more than one bulletsPerTap make sure to repeat shoot function
        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);

        //weapon recoil
        if(WeaponAnim != null)
            WeaponAnim.isRecoiling = true;


        if(recoilShakeImpulseSource != null)
            recoilShakeImpulseSource.GenerateImpulse();

        PlayFireSound();

    }
    private void ResetShot()
    {
        //Allow shooting and invoking again
        readyToShoot = true;
        allowInvoke = true;
    }
    private void Reload()
    {
        if(!(bulletsRight <= 0 && bulletsLeft <= 0))
        {
            PlayReloadSound();
            reloading = true;
            Invoke("ReloadFinished", reloadTime); //Invoke ReloadFinished function with your reloadTime as delay

            _allowButtonHold = allowButtonHold;
        }
        else
        {
            _allowButtonHold = false;
            PlayEmptyMagazineSound();
        }
    }
    private void ReloadFinished()
    {
        //Fill magazine
        if (bulletsRight >= magazineSize || (bulletsRight + bulletsLeft) >= magazineSize) 
        {
            bulletsRight -= (magazineSize - bulletsLeft);
            bulletsLeft = magazineSize;
        }
        else
        {
            bulletsLeft += bulletsRight;
            bulletsRight = 0;
        }

        reloading = false;
    }
    public void UpdateBulletsConterUI(bool equipped)
    {
        if (equipped)
        {
            if (player.bulletsConter != null)
                player.bulletsConter.SetText(bulletsLeft / bulletsPerTap + " / " + bulletsRight / bulletsPerTap);
        }
        else 
        {
            if (player.bulletsConter != null)
                player.bulletsConter.SetText("");
        }
    }

    public void UpdateCrosshairUI(bool equipped)
    {
        if (equipped) 
        {
            player.crosshair.gameObject.SetActive(true);
        }
        else 
        {
            player.crosshair.gameObject.SetActive(false);
        }
    }
    public void UpdatePrssEToInteractUI(bool IsCanInteract)
    {
        this.youCanInteract.SetActive(IsCanInteract);
    }

    private void PlayFireSound()
    {
        if(fireSound  != null && audioSource != null)
        {
            audioSource.PlayOneShot(fireSound);
        }
    }
    private void PlayReloadSound()
    {
        if (reloadSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
    }
    private void PlayEmptyMagazineSound()
    {
        if (emptyMagazineSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(emptyMagazineSound);
        }
    }
}
