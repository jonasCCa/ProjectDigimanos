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
    
    [Header("Level XP")]
    public int level = 1;
    public int curXP;
    public int reqXP = 100;

    [Header("Stats")]
    public bool isAlive;
    public int maxHP;
    public int curHP;
    public int maxMP;
    public int curMP;
    public int str; // Strength > Força             ( Ataque físico )
    public int wis; // Wisdom > Sabedoria           ( Ataque mágico )
    public int con; // Constitution > Constituição  ( Defesa física )
    public int min; // Mind > Mente                 ( Defesa mágica )

    [Header("Bonus Stats")]
    public List<AttType> curBonuses;
    public int bonusHP, bonHPDuration;
    float lastHPBonus;
    public int bonusMP, bonMPDuration;
    float lastMPBonus;
    public int bonusStr, bonStrDuration;
    float lastStrBonus;
    public int bonusWis, bonWisDuration;
    float lastWisBonus;
    public int bonusCon, bonConDuration;
    float lastConBonus;
    public int bonusMin, bonMinDuration;
    float lastMinBonus;
    
    [Header("Nerf Stats")]
    public List<AttType> curNerfs;
    public int nerfHP, nerfHPDuration;
    float lastHPNerf;
    public int nerfMP, nerfMPDuration;
    float lastMPNerf;
    public int nerfStr, nerfStrDuration;
    float lastStrNerf;
    public int nerfWis, nerfWisDuration;
    float lastWisNerf;
    public int nerfCon, nerfConDuration;
    float lastConNerf;
    public int nerfMin, nerfMinDuration;
    float lastMinNerf;

    [Header("XP Drop")]
    public int xpValue = 10; // Used for XP drop, if killed

    // Start is called before the first frame update
    void Start()
    {
        //isAlive = true;
        if(isPlayer)
            playerController = GetComponent<PlayerController>();
        //if(isEnemy)
        //  enemyController

        hpBar.maxValue = maxHP;
        hpBar.currentValue = curHP;

        mpBar.maxValue = maxMP;
        mpBar.currentValue = curMP;

        if(isPlayer) {
            expBar.maxValue = reqXP;
            expBar.currentValue = curXP;
        }
    }


    void Update() {
        List<int> removeIndexes = new List<int>();
        // Buff checking
        for(int i=0; i<curBonuses.Count; i++) {
            switch(curBonuses[i]) {
                case AttType.HP:
                    if((Time.time - lastHPBonus) > bonHPDuration) {
                        removeIndexes.Add(i);
                        bonusHP = 0;
                    }
                    break;
                case AttType.MP:
                    if((Time.time - lastMPBonus) > bonMPDuration) {
                        removeIndexes.Add(i);
                        bonusMP = 0;
                    }
                    break;
                case AttType.STR:
                    if((Time.time - lastStrBonus) > bonStrDuration) {
                        removeIndexes.Add(i);
                        bonusStr = 0;
                    }
                    break;
                case AttType.WIS:
                    if((Time.time - lastWisBonus) > bonWisDuration) {
                        removeIndexes.Add(i);
                        bonusWis = 0;
                    }
                    break;
                case AttType.CON:
                    if((Time.time - lastConBonus) > bonConDuration) {
                        removeIndexes.Add(i);
                        bonusCon = 0;
                    }
                    break;
                case AttType.MIN:
                    if((Time.time - lastMinBonus) > bonMinDuration) {
                        removeIndexes.Add(i);
                        bonusMin = 0;
                    }
                    break;
            }
        }
        // Buff removing
        for(int i=removeIndexes.Count-1; i>=0; i--) {
            curBonuses.RemoveAt(removeIndexes[i]);
        }

        removeIndexes.Clear();
        // Nerf checking
        for(int i=0; i<curNerfs.Count; i++) {
            switch(curNerfs[i]) {
                case AttType.HP:
                    if((Time.time - lastHPNerf) > nerfHPDuration) {
                        removeIndexes.Add(i);
                        nerfHP = 0;
                    }
                    break;
                case AttType.MP:
                    if((Time.time - lastMPNerf) > nerfMPDuration) {
                        removeIndexes.Add(i);
                        nerfMP = 0;
                    }
                    break;
                case AttType.STR:
                    if((Time.time - lastStrNerf) > nerfStrDuration) {
                        removeIndexes.Add(i);
                        nerfStr = 0;
                    }
                    break;
                case AttType.WIS:
                    if((Time.time - lastWisNerf) > nerfWisDuration) {
                        removeIndexes.Add(i);
                        nerfWis = 0;
                    }
                    break;
                case AttType.CON:
                    if((Time.time - lastConNerf) > nerfConDuration) {
                        removeIndexes.Add(i);
                        nerfCon = 0;
                    }
                    break;
                case AttType.MIN:
                    if((Time.time - lastMinNerf) > nerfMinDuration) {
                        removeIndexes.Add(i);
                        nerfMin = 0;
                    }
                    break;
            }
        }
        // Nerf removing
        for(int i=removeIndexes.Count-1; i>=0; i--) {
            curNerfs.RemoveAt(removeIndexes[i]);
        }
    }

    // Adds HP Value to the Current HP
    // Returns false if HP is already at maximum
    // Returns true is HP was added
    public bool Heal(int hpValue) {
        if(curHP == maxHP) {
            // If the current HP is already at maximum, return false
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

    // UNSURE if this will be useful
    // Subtracts HP Value from the Current HP
    // Returns true if HP reaches 0
    // Returns false otherwise
    public bool UnHeal(int hpValue) {
        string log = "UnHealed: " + curHP;
        // Subtracts HP Value from Current HP
        curHP -= hpValue;

        Debug.Log(log + " -> " + curHP);

        // Updates HP bar
        hpBar.currentValue = curHP;
        
        if(curHP <= 0) {
            Dies();
            return true;
        }

        return false;
    }

    // Adds MP Value to the Current MP
    // Returns false if MP is already at maximum
    // Returns true is MP was added
    public bool RestoreMP(int mpValue) {
        if(curMP == maxMP) {
            // If the current MP is already at maximum, returns false
            return false;
        }
        
        string log = "Restored: " + curMP;
        // Else, adds the ammount recieved
        curMP += mpValue;

        // Clamps to Maximum MP, if necessary
        if(curMP > maxMP) {
            curMP = maxMP;
        }

        Debug.Log(log + " -> " + curMP);

        // Updates MP bar
        mpBar.currentValue = curMP;
        
        return true;
    }

    // Reduces MP Value from the Current MP
    // Returns false if the Current MP is less than necessary
    // Returns true is MP was used
    public bool UseMP(int mpValue) {
        if(curMP < mpValue) {
            // If the Current MP is less than necessary, returns false
            return false;
        }
        
        string log = "Used MP: " + curMP;
        // Else, subtracts the ammount sent
        curMP -= mpValue;

        Debug.Log(log + " -> " + curMP);

        // Updates MP bar
        mpBar.currentValue = curMP;
        
        return true;
    }

    // Recieves Buff of any Attribute Type
    // Returns true if Buff was recieved
    // Returns false if there's already a bigger Buff of that type
    public bool RecieveBuff(AttType att, int amount, int duration) {
        switch(att) {
            case AttType.HP:
                if(bonusHP == 0 || bonusHP < amount) {
                    bonusHP = amount;
                    bonHPDuration = duration;
                    lastHPBonus = Time.time;
                    curBonuses.Add(AttType.HP);
                    return true;
                }
                break;
            case AttType.MP:
                if(bonusMP == 0 || bonusMP < amount) {
                    bonusMP = amount;
                    bonMPDuration = duration;
                    lastMPBonus = Time.time;
                    curBonuses.Add(AttType.MP);
                    return true;
                }
                break;
            case AttType.STR:
                if(bonusStr == 0 || bonusStr < amount) {
                    bonusStr = amount;
                    bonStrDuration = duration;
                    lastStrBonus = Time.time;
                    curBonuses.Add(AttType.STR);
                    return true;
                }
                break;
            case AttType.WIS:
                if(bonusWis == 0 || bonusWis < amount) {
                    bonusWis = amount;
                    bonWisDuration = duration;
                    lastWisBonus = Time.time;
                    curBonuses.Add(AttType.WIS);
                    return true;
                }
                break;
            case AttType.CON:
                if(bonusCon == 0 || bonusCon < amount) {
                    bonusCon = amount;
                    bonConDuration = duration;
                    lastConBonus = Time.time;
                    curBonuses.Add(AttType.CON);
                    return true;
                }
                break;
            case AttType.MIN:
                if(bonusMin == 0 || bonusMin < amount) {
                    bonusMin = amount;
                    bonMinDuration = duration;
                    lastMinBonus = Time.time;
                    curBonuses.Add(AttType.MIN);
                    return true;
                }
                break;
        }
        return false;
    }

    // Recieves Nerf of any Attribute Type
    // Returns true if Nerf was recieved
    // Returns false if there's already a bigger Nerf of that type
    public bool RecieveNerf(AttType att, int amount, int duration) {
        switch(att) {
            case AttType.HP:
                if(nerfHP == 0 || nerfHP < amount) {
                    nerfHP = amount;
                    nerfHPDuration = duration;
                    lastHPNerf = Time.time;
                    curNerfs.Add(AttType.HP);
                    return true;
                }
                break;
            case AttType.MP:
                if(nerfMP == 0 || nerfMP < amount) {
                    nerfMP = amount;
                    nerfMPDuration = duration;
                    lastMPNerf = Time.time;
                    curNerfs.Add(AttType.MP);
                    return true;
                }
                break;
            case AttType.STR:
                if(nerfStr == 0 || nerfStr < amount) {
                    nerfStr = amount;
                    nerfStrDuration = duration;
                    lastStrNerf = Time.time;
                    curNerfs.Add(AttType.STR);
                    return true;
                }
                break;
            case AttType.WIS:
                if(nerfWis == 0 || nerfWis < amount) {
                    nerfWis = amount;
                    nerfWisDuration = duration;
                    lastWisNerf = Time.time;
                    curNerfs.Add(AttType.WIS);
                    return true;
                }
                break;
            case AttType.CON:
                if(nerfCon == 0 || nerfCon < amount) {
                    nerfCon = amount;
                    nerfConDuration = duration;
                    lastConNerf = Time.time;
                    curNerfs.Add(AttType.CON);
                    return true;
                }
                break;
            case AttType.MIN:
                if(nerfMin == 0 || nerfMin < amount) {
                    nerfMin = amount;
                    nerfMinDuration = duration;
                    lastMinNerf = Time.time;
                    curNerfs.Add(AttType.MIN);
                    return true;
                }
                break;
        }
        return false;
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
            if(isAlive) {
                Dies();
                return xpValue;
            }
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

        // Change UI
    }

    [ContextMenu("Revive")]
    void Revives() {
        isAlive = true;

        // Place-holder value until decided differently
        curHP = maxHP/10;

        // Updates HP bar
        hpBar.currentValue = curHP;

        // Place-holder for Reviving animation
        transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,transform.eulerAngles.z);

        // Change UI
    }

}
