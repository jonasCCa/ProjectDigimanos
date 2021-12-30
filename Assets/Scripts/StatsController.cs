using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsController : MonoBehaviour
{
    public bool isAlive;
    public int maxHP = 50;
    public int curHP = 50;
    public int atk = 7;

    public int curXP;

    public int xpValue = 10; // Used for XP gain, if killed

    // Start is called before the first frame update
    void Start()
    {
        //isAlive = true;
    }

    // Subtracts Damage Value recieved from Current HP
    // Returns XP ammount if killed, returns 0 if not
    public int takeDamage(int value) {
        string log = "Damaged: " + curHP;
        
        curHP -= value;
        
        Debug.Log(log + " -> " + curHP);

        if(curHP <= 0) {
            isAlive = false;
            return xpValue;
        }

        return 0;
    }

}
