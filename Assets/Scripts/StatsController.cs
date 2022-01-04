using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsController : MonoBehaviour
{
    [Header("References")]
    public bool isPlayer;
    public PlayerController playerController;
    public bool isEnemy;
    public BarController hpBar;
    public BarController mpBar;
    public BarController expBar;

    [Header("Stats")]
    public bool isAlive;
    public int maxHP = 50;
    public int curHP = 50;
    public int atk = 7;

    [Header("Level XP")]
    public int level = 1;
    public int curXP;
    public int reqXP = 100;

    public int xpValue = 10; // Used for XP gain, if killed

    // Start is called before the first frame update
    void Start()
    {
        //isAlive = true;
        if(isPlayer)
            playerController = GetComponent<PlayerController>();
        //if(isEnemy)
        //  enemyController

        hpBar.maxValue = maxHP;
        //mpBar.maxValue = maxMP;
        expBar.maxValue = reqXP;
    }


    // Place-holder for Reviving mechanic
    public bool revive;
    void Update() {
        if(revive && !isAlive) {
            Revives();
            revive = false;
        }
    }

    // Adds HP Value to the Current HP
    // Returns false if HP is already at maximum
    // Returns true is HP was added
    public bool Heal(int hpValue) {
        if(curHP == maxHP) {
            // If the current HP is already at maximum, don't returns false
            return false;
        }
        
        string log = "Healed: " + curHP;
        // Else, adds the ammount recieved
        curHP += hpValue;

        // Clamps to Maximum HP, if necessary
        if(curHP > maxHP) {
            curHP = maxHP;
        }

        Debug.Log(log + " -> " + curHP);

        // Updates HP bar
        hpBar.currentValue = curHP;
        
        return true;
    }

    // Subtracts Damage Value recieved from Current HP
    // Sends Knockback back to controller
    // Returns XP ammount if killed, returns 0 if not
    public int TakeDamage(int dmgValue, int kbValue, Vector3 attackerPosition) {
        bool isBlocking = false;
        if(isPlayer)
            isBlocking = playerController.isBlocking;

        // If it's blocking, don't recieve damage
        //      Maybe recieve less damage instead of 0? Open for discussion
        if(isBlocking) {
            if(isPlayer)
                playerController.Knockback(kbValue, attackerPosition);

            return 0;
        }
        
        string log = "Damaged: " + curHP;
            
        curHP -= dmgValue;
        
        Debug.Log(log + " -> " + curHP);

        // Updates HP bar
        hpBar.currentValue = curHP;

        // If dies, return XP Amount and flags death
        if(curHP <= 0) {
            Dies();
            return xpValue;
        } else { // If doesn't die, sends knockback to controller
            if(isPlayer)
                playerController.Knockback(kbValue, attackerPosition);
            //if(isEnemy)
            //  enemyController
        }

        return 0;
    }

    public void GainExp(int xpGain) {
        curXP += xpGain;

        // Updates exp bar
        expBar.currentValue = curXP;
    }

    void Dies() {
        isAlive = false;

        // Place-holder for Dying animation
        transform.eulerAngles = new Vector3(-90,transform.eulerAngles.y,transform.eulerAngles.z);
    }

    void Revives() {
        isAlive = true;

        // Place-holder value until decided differently
        curHP = maxHP/10;

        // Updates HP bar
        hpBar.currentValue = curHP;

        // Place-holder for Reviving animation
        transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,transform.eulerAngles.z);
    }

}
