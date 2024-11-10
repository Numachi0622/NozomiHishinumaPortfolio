using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class FishPresenter : MonoBehaviour
{
    private FishMove fishMove;
    private FishView fishView;

    private void Start()
    {
        fishMove = GetComponent<FishMove>();
        fishView = GetComponent<FishView>();
        fishMove.Direction.Subscribe(dir =>
        {
            fishView.Flip(dir);
        })
        .AddTo(this);
    }
}
