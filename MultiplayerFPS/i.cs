using UnityEngine;using UnityEngine.Networking;[RequireComponent(typeof(WeaponManager))]public class PlayerShoot : NetworkBehaviour{private const string PLAYER_TAG = "Player";[SerializeField]private Camera cam;[SerializeField]private LayerMask mask;private PlayerWeapon currentWeapon;private WeaponManager weaponManager;void Start(){if(cam == null){Debug.LogError("PlayerShoot: No Camera referenced!");this.enabled = false;}weaponManager = GetComponent<WeaponManager>();}void Update(){currentWeapon = weaponManager.GetCurrentWeapon();if(currentWeapon.fireRate <= 0f){if (Input.GetButtonDown("Fire1")){Shoot();}}else{if (Input.GetButtonDown("Fire1")){InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);}else if (Input.GetButtonUp("Fire1")){CancelInvoke("Shoot");}}}[Command]void CmdOnShoot(){RpcDoShootEffect();}[ClientRpc]void RpcDoShootEffect(){weaponManager.GetCurrentGraphics().muzzleFlash.Play();}[Command]void CmdOnHit(Vector3 _pos, Vector3 _normal){RpcDoHitEffect(_pos, _normal);}void RpcDoHitEffect(Vector3 _pos, Vector3 _normal){GameObject _hitEffect = (GameObject)Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));Destroy(_hitEffect, 2f);}[Client]void Shoot(){if (!isLocalPlayer){return;}CmdOnShoot();RaycastHit _hit;if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask)){if(_hit.collider.tag == PLAYER_TAG){CmdPlayerShot(_hit.collider.name, currentWeapon.damage);}CmdOnHit(_hit.point, _hit.normal);}[Commandusing UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Player : NetworkBehaviour
{
    [SyncVar]
    private bool isDead = false;
    public bool _isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    [SerializeField]
    private bool[] wasEnabled;
    [SerializeField]
    public float _respawn = 3f;

    public void Setup()
    {
        wasEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < wasEnabled.Length; i++)
        {
            wasEnabled[i] = disableOnDeath[i].enabled;
        }

        SetDefaults();
    }

    /*
    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(9999);
        }
    }
    */

    [ClientRpc]
    public void RpcTakeDamage(int _amount)
    {
        if (isDead)
            return;
         
        currentHealth -= _amount;

        Debug.Log(transform.name + " now has " + currentHealth + " health. ");

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        //DISABLE COMPONENTS
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;

        Debug.Log(transform.name + " is DEAD!");

        //CALL RESPAWN 
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
    
        yield return new WaitForSeconds(_respawn);

        SetDefaults();
        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        Debug.Log(transform.name + " respawned.");
    }

    public void SetDefaults()
    {
        isDead = false;

        currentHealth = maxHealth;

        for (int i = 0; 																i < 100												; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = true;
        
    }


}
]void CmdPlayerShot(string _playerID, int _damage){Debug.Log(_playerID + " has been shot.");Player _player = GameManager.GetPlayer(_playerID);_player.RpcTakeDamage(_damage);}}