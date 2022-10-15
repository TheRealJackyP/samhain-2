using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialog : MonoBehaviour
{
    public GameObject StoryEventOB;
    public TextMeshProUGUI StoryTextBox, TitleBox;
    public Image EventImageBox;
    public string EventTitle;
    public string EventText;
    string curText;
    public Sprite EventImage;
    public string[] ChoiceText;
    public Sprite[] ChoiceImage;
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
            EndButton.SetActive(true);
    }

    public void OnSkipClick()
    {
        StopAllCoroutines();
        StoryTextBox.text = "";
        StoryTextBox.text = curText;

        textFin = true;

        if (didChoose)
            EndButton.SetActive(true);
    }

    public void OnEventChoice(int choiceID)
    {
        ChoiceButtons.SetActive(false);
        didChoose = true;
        StoryTextBox.text = "";
        StartCoroutine(Type(ChoiceText[choiceID]));
        EventImageBox.sprite = ChoiceImage[choiceID];
        Destroy(ChoiceButtons);
    }

    public void OnEndButton()
    {
        Destroy(StoryEventOB,0.2f);
    }

}
