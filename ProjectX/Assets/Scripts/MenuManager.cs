using UnityEngine;
using PlayFab;
using UnityEngine.UI;
using PlayFab.ClientModels;
using System.Collections;
using System.Text.RegularExpressions;

public class MenuManager : MonoBehaviour {

    //public variables of UIs
    public InputField userField;
    public InputField passField;

    public InputField userRegField;
    public InputField passRegField;
    public InputField emailRegField;

    public GameObject loginPanel;
    public GameObject registerPanel;

    public Text ErrorLoginText;
    public Text ErrorRegText;


    public string emailNotAvailable = "That email address is already taken.";
    public string usernameNotAvailable = "That username is already taken.";
    public string invalidPassword = "Password is invalid (6-24 characters).";
    public string invalidUsername = "Username is invalid (3-24 characters).";

    private string errorLabel = "";


    void Awake()
    {
        PlayFabSettings.TitleId = "95D3";
    }

    // Use this for initialization
    void Start () {
      
    }
	
	// Update is called once per frame
	void Update () {
       
	}

    //LoginBtn pressed behaviour
    public void OnLoginBtnPressed()
    {

    }

    //RegisterBtn pressed behaviour
    public void OnRegisterBtnPressed()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    //BackBtn pressing
    public void OnBackBtnPressed()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    //Actual Registering button pressing on RegisterPanel
    public void OnRegistering()
    {
        if(!ValidateUser() || !ValidatePass() || !ValidateEmail())
        {
            ErrorRegText.text = "<color=red>Insert Valid Informations!</color>";
        }
        else
        {
            Debug.Log(passRegField.text);
            RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest();
            request.TitleId = PlayFabSettings.TitleId;
            request.Username = userRegField.text;
            request.Password = passRegField.text;
            request.Email = emailRegField.text;
            PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterResult, OnPlayFabError);
        }

    }


    //Checks the text if it is valid
    bool ValidateUser()
    {
        if (userRegField.text == "" || userRegField.text.Length < 3)
        {
            return false;
        }
        else return true;
    }
    //Checks the password
    bool ValidatePass()
    {
        if (passRegField.text == "" || passRegField.text.Length < 3)
        {
            return false;
        }
        else return true;
    }
    // Email Validations
    const string pattern = @"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,17})$";
    bool ValidateEmail()
    {
        return Regex.IsMatch(emailRegField.text, pattern);
    }

    public void OnRegisterResult(RegisterPlayFabUserResult result)
    {
        // now need to store a title-specific display name for this game
        // this is the name that will show up in the leaderboard
        UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest();
        request.DisplayName = result.Username;
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, NameUpdated, OnPlayFabError);
    }

    //Now we can go back to our login or swap 
    public void NameUpdated(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Name " + result.DisplayName + " updated");
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }


    void OnPlayFabError(PlayFabError error)
    {
        Debug.Log("Got an error: " + error.Error);
        Debug.Log(error.ErrorDetails + "   " + error.ErrorMessage);
        if ((error.Error == PlayFabErrorCode.InvalidParams && error.ErrorDetails.ContainsKey("Password")) || (error.Error == PlayFabErrorCode.InvalidPassword))
        {
            errorLabel = invalidPassword;
        }
        else if ((error.Error == PlayFabErrorCode.InvalidParams && error.ErrorDetails.ContainsKey("Username")) || (error.Error == PlayFabErrorCode.InvalidUsername))
        {
            errorLabel = invalidUsername;
        }
        else if (error.Error == PlayFabErrorCode.EmailAddressNotAvailable)
        {
            errorLabel = emailNotAvailable;
        }
        else if (error.Error == PlayFabErrorCode.UsernameNotAvailable)
        {
            errorLabel = usernameNotAvailable;
        }
    }
}
