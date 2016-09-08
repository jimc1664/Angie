using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Eco : MonoBehaviour {

    public int Qnty = 20;
    public float SellPrice = 15;
    public bool Buy;
    public float PurchasePrice = 10;
    public bool Sell;

    public float Cash = 0;
    public float LastCashAt20 = 0;


    public float TrackMod = 1.0f / 20.0f;
    public float TrackedPurchasePrice = 10;
    public float TrackedSellPrice = 15;

    public float Mid = 12;

    public float Margin = 1.1f;
    void Update() {

        Mid = Mathf.LerpUnclamped(TrackedSellPrice, TrackedPurchasePrice, 0.5f );
        PurchasePrice = Mid / Margin;
        SellPrice = Mid * Margin;

        if(Buy) {
            if(Qnty == 20) LastCashAt20 = Cash;

            Qnty--;
            Cash += SellPrice;

            TrackedSellPrice = Mathf.LerpUnclamped(TrackedSellPrice, SellPrice, TrackMod);
            Buy = false;

        }
        if(Sell) {
            if(Qnty == 20) LastCashAt20 = Cash;

            Qnty++;
            Cash -= PurchasePrice;
            TrackedPurchasePrice = Mathf.LerpUnclamped(TrackedPurchasePrice, PurchasePrice, TrackMod);
            Sell = false;
        }

        
    }


}
