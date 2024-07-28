using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : StatusBar
{
    public TextMeshProUGUI BossName;

    private bool isVisible = true;

    public Transform FollowTarget;

    public float YOffset = 0f;


    public void Update()
    {
        if (FollowTarget != null)
        {
            this.transform.position = FollowTarget.position + (Vector3.up * YOffset);
        }
    }

    public void SetBossName(string name)
    {
        BossName.text = name;
    }

    public void ShowHealthBar()
    {
        if (!isVisible)
        {
            isVisible = true;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(isVisible);
            }
        }
    }

    public void HideHealthBar()
    {
        if (isVisible)
        {
            isVisible = false;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(isVisible);
            }
        }
    }
}
