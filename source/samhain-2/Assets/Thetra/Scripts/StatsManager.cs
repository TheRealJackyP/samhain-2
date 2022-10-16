using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatsManager : MonoBehaviour
{
    public static StatsManager instance;
    private void Awake()
    {
        instance = this;
    }



    public StatsStorage stats;
    public TextMeshProUGUI He_txt, Ch_txt;
    public Image He_bar, Ch_bar;
    public int He_HP, Ch_HP;


 
    void Start()
    {
        stats = StatsStorage.instance;
        He_HP = stats.hermesHP;
        Ch_HP = stats.charonHP;
    }

    void Update()
    {
        stats.hermesHP = He_HP;
        stats.charonHP = Ch_HP;


        He_bar.fillAmount = (float)He_HP / 20f;
        Ch_bar.fillAmount = (float)Ch_HP / 30f;
        He_txt.text = He_HP.ToString() + " / " + "20";
        Ch_txt.text = Ch_HP.ToString() + " / " +"30";
    }

    public void TakeDmg(int dmg, int ID) // ID 0 = hermes, 1 = Charon
    {
        if (ID == 0)
            He_HP -= dmg;

        if (He_HP < 0)
            He_HP = 0;

        if (ID == 1)
            Ch_HP -= dmg;

        if (Ch_HP < 0)
            Ch_HP = 0;
    }

    public void Heal(int heal, int ID) // ID 0 = hermes, 1 = Charon
    {
        if (ID == 0)
            He_HP += heal;
        if(He_HP > 20)
            He_HP = 20;

        if (ID == 1)
            Ch_HP += heal;
        if(Ch_HP > 30)
            Ch_HP = 30;
    }




}
