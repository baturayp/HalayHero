using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class IntersititialAd : MonoBehaviour
{
    private InterstitialAd interstitial;

    //show ads event
    public delegate void AdsShownAction();
    public static event AdsShownAction AdsShownEvent;

    public void Start()
    {
        PlayingUIController.ShowAdsEvent += ShowAds;
        RequestInterstitial();
    }

    public void OnDestroy()
    {
        interstitial.Destroy();
        PlayingUIController.ShowAdsEvent -= ShowAds;
        interstitial.OnAdClosed -= HandleOnAdClosed;
    }

    private void RequestInterstitial()
    {
#if UNITY_ANDROID
        //test ads
        //string adUnitId = "ca-app-pub-3940256099942544/1033173712";
        
        //my id
        string adUnitId = "ca-app-pub-6334699688332612/7041194148";
#elif UNITY_IPHONE
        //test id
        //string adUnitId = "ca-app-pub-3940256099942544/4411468910";

        //my id
        string adUnitId = "ca-app-pub-6334699688332612/3632254561";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);
        
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        AdsShownEvent?.Invoke();
    }


    public void ShowAds()
    {
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }
        else
        {
            AdsShownEvent?.Invoke();
        }
    }
}
