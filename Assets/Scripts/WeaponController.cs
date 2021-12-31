using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject parentPlayer;
    public StatsController playerStats;
    public Animator animator;

    public Dictionary<string, float> attackType =
        new Dictionary<string, float>(){{"Light_Attack",1},{"Heavy_Attack",1.5f}};
    
    public float curDmgMultiplier;
    public int knockbackAmount;

    // Start is called before the first frame update
    void Start()
    {
        parentPlayer = gameObject.transform.parent.gameObject;

        playerStats = parentPlayer.GetComponent<StatsController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAttackType(string type) {
        if(attackType.ContainsKey(type))
            curDmgMultiplier = attackType[type];
        else
            curDmgMultiplier = 1;
    }

    void OnTriggerEnter(Collider other) {
        // Can't attack self
        if(other.gameObject != parentPlayer) {
            
            StatsController targetStats = other.gameObject.GetComponent<StatsController>();
            if(targetStats != null) {
                playerStats.curXP += targetStats.TakeDamage((int)(playerStats.atk * curDmgMultiplier),
                                                            knockbackAmount, parentPlayer.transform.position);
            }
        }
    }
}
