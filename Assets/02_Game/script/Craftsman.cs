using System;
using System.Collections;
using System.Collections.Generic;
using BaseSystem.Utility;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class Craftsman : MonoBehaviour, IPointerClickHandler
{
    public int posX;
    public int posY;

    public bool isTouch = false;

    public enum State
    {
        Stay = 0,
        Move,
        Build,
        Crash
    }

    private State mState = State.Stay;

    public bool Stay() { return true; }

    public bool Move()
    {
        return true;
    }

    public bool Build()
    {
        return true;
    }

    public bool Crash()
    {
        return true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        DebugEx.Log("clicled!");
        isTouch = true;
    }

    public void Update()
    {
        DebugEx.Log(isTouch);
    }
}
