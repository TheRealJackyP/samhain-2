using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialog : MonoBehaviour
{
    public GameObject StoryEventOB;
    public TextMeshProUGUI StoryTextBox, TitleBox;
    public Image EventImageBox;
    public Image EventImageBackBox;
    public string EventTitle;
    [TextArea]
    public string EventText;
    string curText;
    public Sprite EventImage;
    public Sprite EventImageBack;
    public string[] ChoiceText;
    public Sprite[] ChoiceImage;
    public Sprite[] ChoiceBackImage;
    [Range(1,10)]
    public float typingSpeed = 5;
    private float typeSpeed;
    private bool textFin, didChoose;
    public GameObject ChoiceButtons, EndButton, SkipButton;

    [Header("Sound")]
    public AudioClip textTypeSFX;


    void Start()
    {
        StoryTextBox.text = "";

        EndButton.SetActive(false);

        typeSpeed = 0.5f / typingSpeed;
        StartCoroutine(Type(EventText));
        TitleBox.text = EventTitle;
        EventImageBox.sprite = EventImage;
        if (EventImageBackBox != null && EventImageBack != null)
        {
            EventImageBackBox.sprite = EventImageBack;
        }
        else
        {
            EventImageBackBox.enabled = false;
        }
    }
    private void Update()
    {
        if(ChoiceButtons != null)
            ChoiceButtons.SetActive(textFin);
        
        SkipButton.SetActive(!textFin);
    }

    IEnumerator Type(string ShowText)
    {
        curText = ShowText;
        textFin = false;
        foreach (char letter in ShowText.ToCharArray())
        {
            if(letter != ' ')
                AudioManager.Instance.PlaySFX(textTypeSFX);
            StoryTextBox.text += letter;
            yield return new WaitForSeconds(typeSpeed);
           
        }
        textFin = true;

        if (didChoose)
        {
            yield return new WaitForSeconds(0.5f);
            EndButton.SetActive(true);
        }
    }

    public void OnSkipClick()
    {
        StopAllCoroutines();
        StartCoroutine(OnSkipClickCo());
    }
    public IEnumerator OnSkipClickCo()
    {
       
        StoryTextBox.text = "";
        StoryTextBox.text = curText;

        textFin = true;

        if (didChoose)
        {
            yield return new WaitForSeconds(0.5f);
            EndButton.SetActive(true);
        }
    }

    public void OnEventChoice(int choiceID)
    {
        ChoiceButtons.SetActive(false);
        didChoose = true;
        StoryTextBox.text = "";
        StartCoroutine(Type(ChoiceText[choiceID]));
        EventImageBox.sprite = ChoiceImage[choiceID];
        if (EventImageBackBox != null && choiceID < ChoiceImage.Count())
        {
            EventImageBackBox.sprite = ChoiceBackImage[choiceID];
        }
        else
        {
            EventImageBackBox = null;
        }
        Destroy(ChoiceButtons);
    }

    public void OnEndButton()
    {
        Destroy(StoryEventOB,0.2f);
    }

    public void HermesDamage(int dmg)
    {
        StatsManager.instance.TakeDmg(dmg, 0);
    }

    public void hermesHeal(int heal)
    {
        StatsManager.instance.Heal(heal, 0);
    }
    public void CharonDamage(int dmg)
    {
        StatsManager.instance.TakeDmg(dmg, 1);
    }

    public void CharonHeal(int heal)
    {
        StatsManager.instance.Heal(heal, 1);
    }

    public void EndGame()
    {
        SceneManger.instance.ChangeScene(3);
    }

}
