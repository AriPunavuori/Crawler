using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InfoManager : MonoBehaviour {
    public Color[] heroColors;
    public Image frameImage;
    public Sprite heroImage;
    public Sprite[] heroImages = new Sprite[4];
    public RectTransform title;
    public RectTransform infoCardRectTransform;
    public TextMeshProUGUI[] textFields;
    public GameObject heroImageCard;
    Character character;
    bool selected;
    Action complete;
    bool movingOut;
    int queue = -1;

    private void Awake() {
        character = GameObject.Find("BaseHealth").GetComponent<Character>();
        complete = OnComplete;
    }

    private void Update() {
        if(queue > -1)
            Show(queue);
    }

    public void Show(int hero) {
        if(!movingOut) {
            if(!selected) {
                frameImage.color = heroColors[hero];
                heroImage = heroImages[hero];
                Image SetheroImageCard = heroImageCard.GetComponent(typeof(Image)) as Image;
                SetheroImageCard.sprite = heroImage;
                for(int i = 0; i < textFields.Length; i++) {
                    textFields[i].text = character.GetCharacterData(i, hero);
                }
                ShowCard();
            }
        } else
            queue = hero;
        HideTitle();
    }

    public void Hide(int i) {
        queue = -1;
        if(!movingOut) {
            movingOut = true;
            ShowTitle();
            HideCard();
        }
    }

    public void ShowCard() {
        LeanTween.cancel(infoCardRectTransform);
        LeanTween.move(infoCardRectTransform, Vector3.zero, .5f).setEaseOutCubic();
    }

    public void HideCard() {
        LeanTween.cancel(infoCardRectTransform);
        LeanTween.move(infoCardRectTransform, Vector3.right * 2500, .5f).setEaseInCubic().setOnComplete(complete);
    }

    public void ShowTitle() {
        LeanTween.cancel(title);
        LeanTween.move(title, Vector3.zero, .5f).setEaseInCubic();
    }

    public void HideTitle() {
        LeanTween.cancel(title);
        LeanTween.move(title, Vector3.left * 2500, .5f).setEaseOutCubic();
    }

    public void SelectedCharacter() {
        selected = true;
    }

    void OnComplete() {
        movingOut = false;
    }

}
