using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Firebase.Database;

public class FireBaseManager : MonoBehaviour
{
    //loading screen
    [Header("Loading Script")]
    public ASyncLoader loadingScreen;

    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;
    public Button continueButton;


    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    public static FireBaseManager instance;

   
    private int sceneNum;
    private int totalScore;
    private List<string> Collectables = new();
    private List<string> RareCollectables = new();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // This will ensure the GameObject persists across scenes.
        }
        else
        {
            Destroy(gameObject);
        }

        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void ClearLoginFeilds()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }
    public void ClearRegisterFeilds()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }

    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
        

    }

    //Function for the register button
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    //Function for the sign out button
    public void SignOutButton()
    {
        auth.SignOut();
        UIManager.instance.LoginScreen();
        ClearRegisterFeilds();
        ClearLoginFeilds();
    }
   
    //Function for the save button
    public void ContinueButton()
    {
        StartCoroutine(LoadScoreData());
        StartCoroutine(loadDataCollection());
        StartCoroutine(loadDataRareCollection());
        loadingScreen.LoadLevelbtn(GetSceneNum());
    }

    public void NewGameButton()
    {
        Task DBTask = DBreference.Child("users").Child(User.UserId).Child("sceneNum").SetValueAsync(1);
        Task DBRareCollectables = DBreference.Child("users").Child(User.UserId).Child("Collecables").RemoveValueAsync();
        Task DBTaskCollectables = DBreference.Child("users").Child(User.UserId).Child("Rare Collection").RemoveValueAsync();
        Task DBTaskScore = DBreference.Child("users").Child(User.UserId).Child("TotalScore").SetValueAsync(0);
        loadingScreen.LoadLevelbtn(1);
    }

    


    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = new FirebaseUser(LoginTask.Result.User);
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
            StartCoroutine(LoadUserData());
            

            yield return new WaitForSeconds(2);
            UIManager.instance.UserDataScreen();
            confirmLoginText.text = "";
            if (GetSceneNum() == 0)
            {
                Debug.Log("continue");
                continueButton.interactable = false;
            }
        }
    }
    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                User = new FirebaseUser(RegisterTask.Result.User);

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    Task ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        //Now return to login screen
                        UIManager.instance.LoginScreen();
                        warningRegisterText.text = "";
                    }
                }
            }
        }
    }

 

    public string GetUserID()
    {
        return User.UserId;
    }

    public DatabaseReference databaseReference()
    {
        return DBreference;
    }
    /// <summary>
    /// get and set methods for the scene number 
    /// </summary>
    /// <returns></returns>
    public int GetSceneNum()
    {
        return sceneNum;
    }
    public void SetSceneNum(int sceneNum)
    {
        this.sceneNum = sceneNum;
    }
    /// <summary>
    /// get and set methods for the Score 
    /// </summary>
    /// <returns></returns>
    public int GetTotalScore()
    {
        return totalScore;
    }

    public void SetTotalScore(int Rectotalscore)
    {
        this.totalScore = Rectotalscore;
    }
    /// <summary>
    /// get and set methods for the collection List
    /// </summary>
    /// <returns></returns>
    /// 
    public List<string> GetCollectable()
    {
        return Collectables;
    }

    public void SetCollectables(List<string> ReCollectables)
    {
        this.Collectables = ReCollectables;
    }

    /// <summary>
    /// get and set methods for the Rare collection List
    /// </summary>
    /// <returns></returns>
    /// 
    public List<string> GetRareCollectable()
    {
        return RareCollectables;
    }

    public void SetRareCollectables(List<string> ReRareCollectables)
    {
        this.RareCollectables = ReRareCollectables;
    }
    /// <summary>
    /// //////////
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadUserData()
    {
        //Get the currently logged in user data
        Task<DataSnapshot> DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }

        else if (DBTask.Result.Value == null)
        {
            //No data exists yet
            sceneNum = 0;
            totalScore = 0;
            Collectables = new();
            RareCollectables = new();
        }

        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            
            SetSceneNum(int.Parse(snapshot.Child("sceneNum").Value.ToString()));

            
        }
    }


    private IEnumerator LoadScoreData()
    {
        //Get the currently logged in user data
        Task<DataSnapshot> DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }

        else if (DBTask.Result.Value == null)
        {
            totalScore = 0;
        }

        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;


            if (snapshot.Child("TotalScore").Value == null)
                SetTotalScore(0);

            else if (snapshot.Child("TotalScore").Value != null)
                SetTotalScore(int.Parse(snapshot.Child("TotalScore").Value.ToString()));

        }
    }


    private IEnumerator loadDataCollection()
    {
        //Get the currently logged in user data
        Task<DataSnapshot> DBTaskCollectables = DBreference.Child("users").Child(User.UserId).GetValueAsync();
        List<string> DBCollectiontemp = new();

        yield return new WaitUntil(predicate: () => DBTaskCollectables.IsCompleted);

        if (DBTaskCollectables.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTaskCollectables.Exception}");
        }

        else if (DBTaskCollectables.Result.Value == null)
        {
            Collectables = new();
        }

        else
        {

            DataSnapshot snapshot = DBTaskCollectables.Result.Child("Collecables");
            if (snapshot.Value != null)
            {
                DBCollectiontemp = new();
               
            }
            for (int i = 0; i < snapshot.ChildrenCount; i++)
            {
                string key = i.ToString();
                DBCollectiontemp.Add(snapshot.Child(key).Value.ToString());
                snapshot.Child("Collecables").Reference.RemoveValueAsync();
            }

            SetCollectables(DBCollectiontemp);
        }
    }


    private IEnumerator loadDataRareCollection()
    {
        //Get the currently logged in user data
        Task<DataSnapshot> DBRareCollectables = DBreference.Child("users").Child(User.UserId).GetValueAsync();
        List<string> DBRareCollection = new();

        yield return new WaitUntil(predicate: () => DBRareCollectables.IsCompleted);

        if (DBRareCollectables.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBRareCollectables.Exception}");
        }

        else if (DBRareCollectables.Result.Value == null)
        {
            RareCollectables = new();
        }

        else
        {

            DataSnapshot snapshot = DBRareCollectables.Result.Child("Rare Collection");
            if (snapshot.Value != null)
            {
                DBRareCollection = new();

            }
            for (int i = 0; i < snapshot.ChildrenCount; i++)
            {
                string key = i.ToString();
                DBRareCollection.Add(snapshot.Child(key).Value.ToString());
                snapshot.Child("Rare Collection").Reference.RemoveValueAsync();
            }

            SetRareCollectables(DBRareCollection);
        }
    }




}
