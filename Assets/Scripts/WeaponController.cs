using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{

    public GameObject parentPlayer;
    public StatsController playerStats;
    public Animator animator;
    //public int weaponDmg;

    // Start is called before the first frame update
    void Start()
    {
        parentPlayer = gameObject.transform.parent.gameObject;

        playerStats = parentPlayer.GetComponent<StatsController>();
    }

    // Update is called once per frame
    void Update()
    {
        //isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Light_Attack");
    }

    void OnTriggerEnter(Collider other) {
        //if(isAttacking)
        if(other.gameObject != parentPlayer)
            Debug.Log("Collided");
        
        StatsController targetStats = other.gameObject.GetComponent<StatsController>();
        if(targetStats != null) {
            playerStats.curXP += targetStats.takeDamage(playerStats.atk);
        }
    }
}
