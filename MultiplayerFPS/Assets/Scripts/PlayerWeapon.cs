using UnityEngine;

[System.Serializable]
public class PlayerWeapon : MonoBehaviour
{
    public string name = "Glock";

    public int damage = 10;
    public float range = 100f;

    public float fireRate = 0f;

    public GameObject graphics;

}