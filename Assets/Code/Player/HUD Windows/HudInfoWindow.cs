using Assets.Code.Player;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudInfoWindow : MonoBehaviour {

    private GameController GameController = GameController.GetSharedInstance();

    private bool IsEnabled = true;

    public GameObject GoldValueField;
    public GameObject HealthValueField;

    private TextMeshProUGUI _goldValue;
    private TextMeshProUGUI _healthValue;

    private void Awake()
    {
        _goldValue   = GoldValueField.GetComponent<TextMeshProUGUI>();
        _healthValue = HealthValueField.GetComponent<TextMeshProUGUI>();
        //register subscription to health/gold changed events
        PlayerController.GoldChanged   += UpdateGoldValue;
        PlayerController.HealthChanged += UpdateHealthValue;

    }
    private void Start()
    {
        UpdateHealthValue();
        UpdateGoldValue();
    }



    private void UpdateHealthValue()
    {
        _healthValue.text = GameController.PlayerController.GetHealthValue().ToString();
        
    }
    private void UpdateGoldValue()
    {
        _goldValue.text = GameController.PlayerController.GetGoldValue().ToString();
    }
}
