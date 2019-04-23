using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [SerializeField]
    private Text rateOfFireDisplay;
    [SerializeField]
    private Text coinDisplay;
    [SerializeField]
    private GameObject startUI, endUI;

    private PlayerController player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    // Use this for initialization
    void Start () {

        player.eCoinPickedUp += CoinPickedUp;

        rateOfFireDisplay.text = PlayerController.rateOfFire.ToString();

        DisplayCoins();
        DisplayRate();
    }

    private void CoinPickedUp()
    {
        DisplayCoins();
    }

    // Update is called once per frame
    void Update () {
		
	}
    void DisplayCoins()
    {
        coinDisplay.text = SaveData.LoadCoins().ToString();
    }
    void DisplayRate()
    {
        rateOfFireDisplay.text = PlayerController.rateOfFire.ToString();
    }
    public void UpdateRate(int increment)
    {
        if (SaveData.LoadCoins() > 20)
        {
            SaveData.SaveCoins(SaveData.LoadCoins() - 20);

            DisplayCoins();

            PlayerController.rateOfFire += increment;
            SaveData.SaveFireRate(SaveData.LoadFireRate()+ increment);
        }
        DisplayRate();

    }
    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(0);
    }
    public void ShowStartUI(bool show)
    {
        startUI.SetActive(show);
    }

    public void ShowEndUI(bool show)
    {
        endUI.SetActive(show);
    }
    public void OnRestart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
