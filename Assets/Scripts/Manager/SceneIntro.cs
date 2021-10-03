using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class SceneIntro : MonoBehaviour{
    private void Update() {
        if (Input.anyKeyDown) {
            var transition = (Trans) ((int) Random.Range(1, 0b10000)) + ((int) Random.Range(0b100000, 0b1000000000));
            // transition = Trans.FromRight | Trans.ToLeft 등으로 뱡향 설정
            GameManager.Instance.Transition(transition, () => Debug.Log("asdf"));
        }
    }
}