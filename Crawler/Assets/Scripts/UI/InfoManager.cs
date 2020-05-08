using System.Collections;
using System.Collections.Generic;
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

    private void Awake() {
        character = GameObject.Find("BaseHealth").GetComponent<Character>();
    }
    private void Start() {

    }

    public void Show(int hero) {
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
        HideTitle();
    }

    public void Hide(int i) {
        ShowTitle();
        HideCard();
    }

    public void ShowCard() {
        LeanTween.cancel(infoCardRectTransform);
        LeanTween.move(infoCardRectTransform, Vector3.zero, .5f).setEaseOutCirc();
    }

    public void HideCard() {
        LeanTween.cancel(infoCardRectTransform);
        LeanTween.move(infoCardRectTransform, Vector3.right * 2500, .5f).setEaseOutCirc();
    }

    public void ShowTitle() {
        LeanTween.cancel(title);
        LeanTween.move(title, Vector3.zero, .5f).setEaseOutCirc();
    }

    public void HideTitle() {
        LeanTween.cancel(title);
        LeanTween.move(title, Vector3.left * 2500, .5f).setEaseOutCirc();
    }

    public void SelectedCharacter() {
        selected = true;
    }
}
