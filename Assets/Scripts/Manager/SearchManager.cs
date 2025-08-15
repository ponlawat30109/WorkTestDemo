using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SearchManager : Singleton<SearchManager>
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Dropdown filterButton;
    [SerializeField] private TMP_InputField searchBox;
    [SerializeField, ReadOnlyInspector] private int filterIndex;

    public TMP_Dropdown FilterButton => filterButton;
    public TMP_InputField SearchBox => searchBox;
    public int FilterIndex => filterIndex;

    [Header("Search Text")]
    [SerializeField, ReadOnlyInspector] private string searchText = string.Empty;

    public Action<string> OnSearchChanged;
    public Action<int> OnFilterChanged;

    private int currentPage = 0;
    private const int itemsPerPage = 4;

    public int CurrentPage => currentPage;
    public int ItemsPerPage => itemsPerPage;

    private void Start()
    {
        if (searchBox != null)
            searchBox.onValueChanged.AddListener(OnSearchValueChanged);

        if (filterButton != null)
            filterButton.onValueChanged.AddListener(OnFilterValueChanged);

        OnSearchChanged += FilterFoodDataBySearchBox;
        OnFilterChanged += FilterFoodDataByDropdown;
    }

    private void OnDestroy()
    {
        if (searchBox != null)
            searchBox.onValueChanged.RemoveListener(OnSearchValueChanged);

        if (filterButton != null)
            filterButton.onValueChanged.RemoveListener(OnFilterValueChanged);
    }

    private void OnSearchValueChanged(string value)
    {
        searchText = value;
        OnSearchChanged?.Invoke(value);
    }

    private void OnFilterValueChanged(int index)
    {
        filterIndex = index;
        OnFilterChanged?.Invoke(filterIndex);
    }

    private void FilterFoodDataBySearchBox(string search)
    {
        GameManager.Instance.CurrentFoodData = string.IsNullOrEmpty(search)
            ? new List<FoodSO>(GameManager.Instance.AllFoodData)
            : GameManager.Instance.AllFoodData.FindAll(food => food.FoodName.ToLower().Contains(search.ToLower()));

        currentPage = 0;

        GameManager.Instance.InitializeMenuContent();
    }

    private void FilterFoodDataByDropdown(int index)
    {
        List<FoodSO> filteredList = new();
        filteredList = index switch
        {
            0 => GameManager.Instance.AllFoodData.FindAll(food => food.Rarity == 3),
            1 => GameManager.Instance.AllFoodData.FindAll(food => food.Rarity == 2),
            2 => GameManager.Instance.AllFoodData.FindAll(food => food.Rarity == 1),
            _ => new List<FoodSO>(GameManager.Instance.AllFoodData),
        };
        GameManager.Instance.CurrentFoodData = filteredList;

        currentPage = 0;

        GameManager.Instance.InitializeMenuContent();
    }

    public void GetPreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            GameManager.Instance.InitializeMenuContent();
        }
    }

    public void GetNextPage()
    {
        int totalItems = GameManager.Instance.CurrentFoodData.Count;
        int totalPages = Mathf.CeilToInt((float)totalItems / itemsPerPage);
        if (currentPage < totalPages - 1)
        {
            currentPage++;
            GameManager.Instance.InitializeMenuContent();
        }
    }
}
