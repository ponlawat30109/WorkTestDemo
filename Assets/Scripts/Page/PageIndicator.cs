using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageIndicator : Singleton<PageIndicator>
{
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = new Color(1, 1, 1, 0); // transparent
    private List<GameObject> dots = new();

    public void SetPageCount(int pageCount, int currentPage)
    {
        foreach (var dot in dots)
            Destroy(dot);
        dots.Clear();

        for (int i = 0; i < pageCount; i++)
        {
            var dot = Instantiate(dotPrefab, transform);
            var img = dot.GetComponent<Image>();
            img.color = (i == currentPage) ? activeColor : inactiveColor;
            dots.Add(dot);
        }
    }
}