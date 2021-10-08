using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Level;
using Manager;
using System;

public class ProgressBar : MonoBehaviour
{
    RectTransform rt;
    private static ProgressBar _instance;
    public static ProgressBar instance => _instance ??= FindObjectOfType<ProgressBar>();
    void Start()
    {
        rt = GetComponent<RectTransform>();
    }
    Vector3 scale = new Vector3();
    float length = 0;
    double songlength = 0;
    public void StartProgress()
    {
        length = 3.5f / ?;
        songlength = PlayManager.Instance.CurrentMilisec;
    }
    public void EndProgress()
    {
        scale.x = 0;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
