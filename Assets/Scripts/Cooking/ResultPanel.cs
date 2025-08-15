using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] private Image resultImage;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private List<GameObject> rarityIcons;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnResultButtonClick);
    }

    public void SetResult(string result, Sprite image, int rarity)
    {
        resultText.text = result;
        resultImage.sprite = image;

        foreach (GameObject icon in rarityIcons)
        {
            icon.SetActive(false);
        }

        switch (rarity)
        {
            case 1:
                rarityIcons[0].SetActive(true);
                break;
            case 2:
                rarityIcons[1].SetActive(true);
                break;
            case 3:
                rarityIcons[2].SetActive(true);
                break;
        }
    }

    public void OnResultButtonClick()
    {
        gameObject.SetActive(false);
        GameManager.Instance.DimPanel.SetActive(false);
        resultText.text = "";
    }
}
