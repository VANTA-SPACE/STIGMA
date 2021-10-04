using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class FMODTest : MonoBehaviour
{

    bool Paused = false;

    int PauseDelay = 0;

[EventRef] public string TitleEvent = "event:/Title";
    private FMOD.Studio.EventInstance _titleEvent;
    private FMOD.Studio.PARAMETER_ID _titleParamterId;

    private bool _isStarting;
    private Transform _transform;

    private void Start()
    {
        _isStarting = false;
        _transform = GetComponent<Transform>();

        _titleEvent = FMODUnity.RuntimeManager.CreateInstance(TitleEvent);
        _titleEvent.start();

        RuntimeManager.AttachInstanceToGameObject(_titleEvent, GetComponent<Transform>());

        FMOD.Studio.EventDescription _titleEventDescription;
        _titleEvent.getDescription(out _titleEventDescription);
        FMOD.Studio.PARAMETER_DESCRIPTION _titleParameterDescription;
        _titleEventDescription.getParameterDescriptionByName("Transition", out _titleParameterDescription);
        _titleParamterId = _titleParameterDescription.id;
    }

    private void Update()
    {
        if (!Paused && Input.GetKeyDown(KeyCode.Escape) && PauseDelay == 0)
        {
            _titleEvent.setPaused(true);
            Paused = true;
            StartCoroutine(Delay());
        }

        if (Paused && Input.GetKeyDown(KeyCode.Escape) && PauseDelay == 0)
        {
            StartCoroutine(ResumeAfterDelay());
        }
    }

    private IEnumerator Delay()
    {
        PauseDelay = 1;
        yield return new WaitForSeconds(0.25f);
        PauseDelay = 0;
    }

    private IEnumerator ResumeAfterDelay()
    {
        PauseDelay = 1;
        yield return new WaitForSeconds(0.7f);
        _titleEvent.setPaused(false);
        Paused = false;
        PauseDelay = 0;
    }

}
