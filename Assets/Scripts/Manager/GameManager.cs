using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [Header("UI Elements")]
    [SerializeField] private Button openCookingButton;
    [SerializeField] private Button closeCookingButton;
    [SerializeField] private Button cookingButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private GameObject cookingPanel;
    [SerializeField] private GameObject dimPanel;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private ResultPanel resultPanel;

    [Header("Content Panels")]
    [SerializeField] private GameObject menuContent;
    [SerializeField] private GameObject ingredientContent;
    public GameObject MenuContent { get => menuContent; set => menuContent = value; }
    public GameObject IngredientContent { get => ingredientContent; set => ingredientContent = value; }

    [Header("Cooking Animation")]
    [SerializeField] private SkeletonGraphic cookingAnimation;

    [Header("Cooking Data")]
    [SerializeField, ReadOnlyInspector] private List<FoodSO> allFoodData;
    [SerializeField, ReadOnlyInspector] private List<IngredientsSO> allIngredientsData;
    [SerializeField, ReadOnlyInspector] private List<FoodSO> currentFoodData;
    [SerializeField, ReadOnlyInspector] private List<IngredientsData> currentIngredientData;
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private GameObject ingredientPrefab;

    public bool IsCooking { get; set; } = false;
    public FoodSO CurrentFood { get; set; }
    public float CookingTimer { get; set; }
    public GameObject DimPanel { get => dimPanel; set => dimPanel = value; }
    public GameObject FoodPrefab { get => foodPrefab; set => foodPrefab = value; }
    public GameObject IngredientPrefab { get => ingredientPrefab; set => ingredientPrefab = value; }
    public List<FoodSO> AllFoodData { get => allFoodData; set => allFoodData = value; }
    public List<IngredientsSO> AllIngredientsData { get => allIngredientsData; set => allIngredientsData = value; }
    public List<FoodSO> CurrentFoodData { get => currentFoodData; set => currentFoodData = value; }
    public List<IngredientsData> CurrentIngredientData { get => currentIngredientData; set => currentIngredientData = value; }

    void Start()
    {
        AllFoodData = new List<FoodSO>(Resources.LoadAll<FoodSO>(""));
        AllIngredientsData = new List<IngredientsSO>(Resources.LoadAll<IngredientsSO>(""));

        openCookingButton.gameObject.SetActive(true);
        cookingPanel.SetActive(false);

        openCookingButton.onClick.AddListener(() =>
        {
            cookingPanel.SetActive(true);
            Initialize();
        });
        closeCookingButton.onClick.AddListener(() =>
        {
            cookingPanel.SetActive(false);
            openCookingButton.gameObject.SetActive(true);

            foreach (Transform item in MenuContent.transform)
            {
                Destroy(item.gameObject);
            }

            SearchManager.Instance.FilterButton.value = 99;
            SearchManager.Instance.FilterButton.RefreshShownValue();
        });
        cookingButton.onClick.AddListener(() => OnCooking());
        leftButton.onClick.AddListener(SearchManager.Instance.GetPreviousPage);
        rightButton.onClick.AddListener(SearchManager.Instance.GetNextPage);
    }

    private void OnApplicationQuit()
    {
        SaveManager.Instance.SaveCooking();
    }

    private void Initialize()
    {
        CurrentFoodData = new List<FoodSO>(AllFoodData);
        if (System.IO.File.Exists(System.IO.Path.Combine(Application.persistentDataPath, "save.json")))
        {
            SaveManager.Instance.LoadIngredientsData();
        }
        else
        {
            CurrentIngredientData = new List<IngredientsData>();
        }
        foreach (var ingredient in AllIngredientsData)
        {
            if (!CurrentIngredientData.Exists(x => x.ingredient == ingredient))
            {
                CurrentIngredientData.Add(new IngredientsData { ingredient = ingredient, quantity = 100 });
            }
        }

        InitializeMenuContent();
    }

    public void InitializeMenuContent()
    {
        foreach (Transform child in MenuContent.transform)
        {
            Destroy(child.gameObject);
        }

        int totalItems = CurrentFoodData.Count;
        int itemsPerPage = SearchManager.Instance.ItemsPerPage;
        int currentPage = SearchManager.Instance.CurrentPage;
        int totalPages = Mathf.CeilToInt((float)totalItems / itemsPerPage);

        int startIdx = currentPage * itemsPerPage;
        int endIdx = Mathf.Min(startIdx + itemsPerPage, totalItems);

        for (int i = startIdx; i < endIdx; i++)
        {
            var item = CurrentFoodData[i];
            var foodRecipe = Instantiate(FoodPrefab, MenuContent.transform);
            foodRecipe.GetComponent<Food>().FoodSO = item;
            foodRecipe.GetComponent<Food>().Initialize();

            var button = foodRecipe.GetComponent<Button>();
            var outline = foodRecipe.GetComponent<Outline>();
            if (button != null && outline != null)
            {
                button.onClick.AddListener(() =>
                {
                    if (IsCooking)
                        return;

                    foreach (Transform child in MenuContent.transform)
                    {
                        var childOutline = child.GetComponent<Outline>();
                        if (childOutline != null)
                            childOutline.enabled = false;
                    }
                    outline.enabled = true;
                });
            }
        }

        PageIndicator.Instance.SetPageCount(totalPages, SearchManager.Instance.CurrentPage);
    }

    public void SetCurrentFood(FoodSO food, bool forceSet = false)
    {
        if (IsCooking && !forceSet)
            return;

        CurrentFood = food;

        foreach (Transform item in IngredientContent.transform)
        {
            Destroy(item.gameObject);
        }

        bool canCook = true;

        foreach (var item in CurrentFood.Ingredients)
        {
            var data = Instantiate(IngredientPrefab, IngredientContent.transform);
            data.GetComponent<Ingredients>().IngredientsSO = item.ingredient;
            var currentAmount = 0;
            var searchData = CurrentIngredientData.Find(x => x.ingredient == item.ingredient);
            if (searchData != null)
                currentAmount = searchData.quantity;
            var amountText = currentAmount == 0
                ? $"<color=red>{currentAmount}</color>/{item.quantity}"
                : $"{currentAmount}/{item.quantity}";
            data.GetComponent<Ingredients>().IngredientNameText.text = amountText;
            data.GetComponent<Ingredients>().Initialize();

            if (currentAmount < item.quantity)
                canCook = false;
        }

        SetCookingButtonInteractable(canCook);
    }

    private void SetCookingButtonInteractable(bool interactable)
    {
        var colors = cookingButton.colors;
        colors.normalColor = interactable ? Color.white : Color.gray;
        colors.disabledColor = Color.gray;
        cookingButton.interactable = interactable;
        cookingButton.colors = colors;
    }

    public void OnCooking()
    {
        if (IsCooking || CurrentFood == null)
            return;

        foreach (var item in CurrentFood.Ingredients)
        {
            var searchData = CurrentIngredientData.Find(x => x.ingredient == item.ingredient);
            int currentAmount = searchData != null ? searchData.quantity : 0;
            if (currentAmount < item.quantity)
                return;
        }

        StaminaManager.Instance.UseStamina(10);

        IsCooking = true;
        timerText.text = $"{CurrentFood.CookingTime}";

        StartCoroutine(CookingCoroutine());
    }

    private IEnumerator CookingCoroutine()
    {
        foreach (var item in CurrentFood.Ingredients)
        {
            var searchData = CurrentIngredientData.Find(x => x.ingredient == item.ingredient);
            if (searchData != null)
            {
                searchData.quantity -= item.quantity;
                if (searchData.quantity < 0)
                    searchData.quantity = 0;
            }
        }

        SetCurrentFood(CurrentFood, true);

        cookingAnimation.AnimationState.SetAnimation(0, "idle-boiled", true);

        CookingTimer = CurrentFood.CookingTime;
        while (CookingTimer > 0)
        {
            int seconds = Mathf.FloorToInt(CookingTimer);
            int milliseconds = Mathf.FloorToInt((CookingTimer - seconds) * 100);
            timerText.text = $"{seconds}:{milliseconds:00}";
            yield return null;
            CookingTimer -= Time.deltaTime;
        }
        timerText.text = "0:00";
        cookingAnimation.AnimationState.SetAnimation(0, "idle", true);

        resultPanel.gameObject.SetActive(true);
        DimPanel.SetActive(true);
        resultPanel.SetResult(CurrentFood.FoodName, CurrentFood.FoodImage, CurrentFood.Rarity);

        SaveManager.Instance.SaveCooking();

        IsCooking = false;
    }
}
