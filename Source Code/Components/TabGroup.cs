using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    public List<GameObject> tabs;

    Color idleColor = new Color(1,1,1);
    Color hoverColor = new Color(0.8f,0.8f,0.8f);
    Color selectedColor = new Color(0.4f, 0.4f, 0.4f);

    public TabButton selectedTab;

    void Start()
    {
        tabs = new List<GameObject>();  
    }

    public void Subscribe(TabButton tabButton)
    {
        if(tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }

        tabButtons.Add(tabButton);
    }

    public void PopulateTabList(GameObject tab)
    {
        if(tabs == null)
        {
            tabs = new List<GameObject> ();
        }
        tabs.Add(tab);
        tab.gameObject.SetActive(false);
    }

    public void OnTabEnter(TabButton tabButton)
    {
        if (selectedTab == null || tabButton != selectedTab)
        {
            ResetTabs();
            tabButton.image.color = hoverColor;
        }

       
    }

    public void OnTabExit(TabButton tabButton)
    {
        ResetTabs();

    }

    public void OnTabSelected(TabButton tabButton)
    {
        selectedTab = tabButton;
        ResetTabs();
        tabButton.image.color = selectedColor;

        int index = tabButton.transform.GetSiblingIndex();
        for(int i = 0; i < tabs.Count; i++)
        {
            if(i == index)
            {
                tabs[i].SetActive(true);
            }
            else
            {
                tabs[i].SetActive(false);
            }
        }
    }

    public void ResetTabs()
    {
        foreach(TabButton tabButton in tabButtons)
        {
            if(selectedTab != null && tabButton == selectedTab)
            {
                continue;
            }
            tabButton.image.color = idleColor;

        }
        
    }
}





    