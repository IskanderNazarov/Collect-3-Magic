using System;
using _game;
using UnityEngine;
using Zenject;

public class BgScaler : MonoBehaviour {
    [SerializeField] private Camera cam;
    [SerializeField] private float lowerPos = 0.1f;
    [SerializeField] private float topPos = 0.9f;
    [SerializeField, Header("w/h of scale SR")]private float bgAspect = 148f / 256f;

    private SpriteRenderer sr;
    private Vector2 camSize;
    private Vector2 prevCamSize;
    private float prevLowerPos;
    private float prevTopPos;
    private float prevBgAspect;
    private Transform tr;
    private float lowerPosX;


    private void Start() {
        
        sr = GetComponent<SpriteRenderer>();
        camSize = GetCamSize();
        prevCamSize = GetCamSize();
        tr = transform;

        prevLowerPos = lowerPos;
        prevTopPos = topPos;

        Scale();
    }

    private void Update() {
        if (prevCamSize.x == GetCamSize().x && prevLowerPos == lowerPos && prevTopPos == topPos && bgAspect == prevBgAspect) return;
        
        prevLowerPos = lowerPos;
        prevTopPos = topPos;
        camSize = GetCamSize();
        prevCamSize = camSize;
        prevBgAspect = bgAspect;
        Scale();
    }

    private void Scale() {
        
        var height = camSize.y * (topPos - lowerPos);
        var width = height * bgAspect; //a = x/y, x = a * y

        if (width > camSize.x) {
            width = camSize.x;
            height = width / bgAspect;
        }

        tr.localScale = Vector3.one;
        var bgWidth = sr.bounds.size.x;//b*x=w, xw / bgw
        var scale = width / bgWidth;
        //tr.localScale *= scale;
        var s = Vector3.one * scale;
        s.z = 1;
        tr.localScale = s;
        lowerPosX = -camSize.y / 2 + camSize.y * lowerPos;
        tr.position = new Vector3(0, lowerPosX, 0);
    }

    private Vector2 GetCamSize() {
        var y = cam.orthographicSize * 2;
        var x = cam.aspect * y;
        return new Vector2(x, y);
    }
}