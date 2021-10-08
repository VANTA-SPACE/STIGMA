using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Level;
using Manager;
using System;

public class ProgressBar : MonoBehaviour {
    public RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update() {
        if (!PlayManager.IsPlaying) {
            rectTransform.sizeDelta = Vector2.zero;
            return;
        } 

        var rat = PlayManager.Instance.CurrentMilisec / PlayManager.Instance.LevelData.EndMSAnd4Beats;
        rectTransform.sizeDelta = new Vector2((float) (rat * 1920), 5);
    }
}