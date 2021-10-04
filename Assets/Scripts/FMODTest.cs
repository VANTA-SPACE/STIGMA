using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class FMODTest : MonoBehaviour
{
    [FMODUnity.EventRef] public string TitleEvent = "event:/Title";
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

        FMODUnity.RuntimeManager.AttachInstanceToGameObject(_titleEvent, GetComponent<Transform>());

        FMOD.Studio.EventDescription _titleEventDescription;
        _titleEvent.getDescription(out _titleEventDescription);
        FMOD.Studio.PARAMETER_DESCRIPTION _titleParameterDescription;
        _titleEventDescription.getParameterDescriptionByName("Transition", out _titleParameterDescription);
        _titleParamterId = _titleParameterDescription.id;
    }

    private void Update()
    {
    }
}
