using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using Newtonsoft.Json;
using TMPro;
using Nethereum.Web3;
using Nethereum.Util;
using Nethereum.Signer;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.ABI.Encoders;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3.Accounts;
using Nethereum.Web3.Accounts.Managed;
using Newtonsoft.Json.Linq;

public class Authentication : MonoBehaviour
{   
    List<LoginVerifier> verifierList = new List<LoginVerifier> {
        new LoginVerifier("Google", Provider.GOOGLE),
        new LoginVerifier("Facebook", Provider.FACEBOOK),
        // new LoginVerifier("CUSTOM_VERIFIER", Provider.CUSTOM_VERIFIER),
        new LoginVerifier("Twitch", Provider.TWITCH),
        new LoginVerifier("Discord", Provider.DISCORD),
        new LoginVerifier("Reddit", Provider.REDDIT),
        new LoginVerifier("Apple", Provider.APPLE),
        new LoginVerifier("Github", Provider.GITHUB),
        new LoginVerifier("LinkedIn", Provider.LINKEDIN),
        new LoginVerifier("Twitter", Provider.TWITTER),
        new LoginVerifier("Line", Provider.LINE),
        new LoginVerifier("Email Passwordless", Provider.EMAIL_PASSWORDLESS),
        new LoginVerifier("SMS Passwordless", Provider.SMS_PASSWORDLESS),
        new LoginVerifier("Farcaster", Provider.FARCASTER),
    };

    Web3Auth web3Auth;

    [SerializeField]
    InputField emailAddressField;

    [SerializeField]
    Dropdown verifierDropdown;

    [SerializeField]
    Button loginButton;

    [SerializeField]
    Text loginResponseText;

    [SerializeField]
    Button logoutButton;

    [SerializeField]
    Button mfaSetupButton;

    [SerializeField]
    Button launchWalletServicesButton;

    [SerializeField]
    Button signMessageButton;

    [SerializeField]
    Button signResponseButton;

    [SerializeField]
    GameObject mainMenu;

    const string rpcURL = "https://rpc.WinMe.com/eth";

    void Start()
    {
        var loginConfigItem = new LoginConfigItem()
        {
            verifier = "your_verifierid_from_web3auth_dashboard",
            typeOfLogin = TypeOfLogin.GOOGLE,
            clientId = "your_clientId_from_web3auth_dashboard"
        };

        var clientID = "BKEUunaSgB08d9_NKfEUWpqyJKnHJ20cTqLLF2UUWr8i2l--bcD_qZTto8s6tINL9HDJI6WEtyNiB2YtCU8UbB4";

        web3Auth = GetComponent<Web3Auth>();
        web3Auth.setOptions(new Web3AuthOptions()
        {
            clientId = clientID,
            redirectUrl = new System.Uri("winme://com.web3auth.winme/auth"),
            network = Web3Auth.Network.SAPPHIRE_DEVNET,
            sessionTime = 86400
        });

        web3Auth.onLogin += onLogin;
        web3Auth.onLogout += onLogout;
        web3Auth.onMFASetup += onMFASetup;
        web3Auth.onSignResponse += onSignResponse;

        emailAddressField.gameObject.SetActive(false);
        logoutButton.gameObject.SetActive(false);
        mfaSetupButton.gameObject.SetActive(false);
        launchWalletServicesButton.gameObject.SetActive(false);
        signMessageButton.gameObject.SetActive(false);
        signResponseButton.gameObject.SetActive(false);

        loginButton.onClick.AddListener(login);
        logoutButton.onClick.AddListener(logout);
        mfaSetupButton.onClick.AddListener(enableMFA);
        launchWalletServicesButton.onClick.AddListener(launchWalletServices);
        //signMessageButton.onClick.AddListener(request);

        verifierDropdown.AddOptions(verifierList.Select(x => x.name).ToList());
        verifierDropdown.onValueChanged.AddListener(onVerifierDropDownChange);
    }

    private void onLogin(Web3AuthResponse response)
    {
        loginResponseText.text = JsonConvert.SerializeObject(response, Formatting.Indented);
        var userInfo = JsonConvert.SerializeObject(response.userInfo, Formatting.Indented);
        Debug.Log(userInfo);

        gameObject.SetActive(false);
        mainMenu.SetActive(true);

        loginButton.gameObject.SetActive(false);
        verifierDropdown.gameObject.SetActive(false);
        emailAddressField.gameObject.SetActive(false);
        logoutButton.gameObject.SetActive(true);
        mfaSetupButton.gameObject.SetActive(true);
        launchWalletServicesButton.gameObject.SetActive(true);
        signMessageButton.gameObject.SetActive(true);
        signResponseButton.gameObject.SetActive(true);
    }

    private void onLogout()
    {
        loginButton.gameObject.SetActive(true);
        verifierDropdown.gameObject.SetActive(true);
        logoutButton.gameObject.SetActive(false);
        mfaSetupButton.gameObject.SetActive(false);
        launchWalletServicesButton.gameObject.SetActive(false);
        signMessageButton.gameObject.SetActive(false);
        signResponseButton.gameObject.SetActive(false);

        loginResponseText.text = "";
    }

    private void onMFASetup(bool response)
    {
        Debug.Log("MFA Setup: " + response);
    }

    private void onSignResponse(SignResponse signResponse)
    {
        Debug.Log("Retrieved SignResponse: " + signResponse);
    }

    private void onVerifierDropDownChange(int selectedIndex)
    {
        if (verifierList[selectedIndex].loginProvider == Provider.EMAIL_PASSWORDLESS)
            emailAddressField.gameObject.SetActive(true);
        else
            emailAddressField.gameObject.SetActive(false);
    }

    private void login()
    {
        var selectedProvider = verifierList[verifierDropdown.value].loginProvider;

        var options = new LoginParams()
        {
            loginProvider = selectedProvider
        };

        if (selectedProvider == Provider.EMAIL_PASSWORDLESS)
        {
            options.extraLoginOptions = new ExtraLoginOptions()
            {
                login_hint = emailAddressField.text
            };
        }
        if (selectedProvider == Provider.SMS_PASSWORDLESS)
        {
            options.extraLoginOptions = new ExtraLoginOptions()
            {
                login_hint = "+XX-XXXXXXXXXX"
            };
        }

        if(web3Auth == null)
        {
            Debug.Log("not initialized");
        }

        web3Auth.login(options);
    }

    private void logout()
    {
        web3Auth.logout();
    }

    private void enableMFA()
    {
        var selectedProvider = verifierList[verifierDropdown.value].loginProvider;

        var options = new LoginParams()
        {
            loginProvider = selectedProvider,
            mfaLevel = MFALevel.MANDATORY
        };

        if (selectedProvider == Provider.EMAIL_PASSWORDLESS)
        {
            options.extraLoginOptions = new ExtraLoginOptions()
            {
                login_hint = emailAddressField.text
            };
        }
        web3Auth.enableMFA(options);
    }

    private void launchWalletServices()
    {
        var selectedProvider = verifierList[verifierDropdown.value].loginProvider;

        var chainConfig = new ChainConfig()
        {
            chainId = "0x1",
            rpcTarget = "https://mainnet.infura.io/v3/daeee53504be4cd3a997d4f2718d33e0",
            ticker = "ETH",
            chainNamespace = Web3Auth.ChainNamespace.EIP155
        };
        web3Auth.launchWalletServices(chainConfig);
    }

    //private void request()
    //{
    //    var selectedProvider = verifierList[verifierDropdown.value].loginProvider;

    //    var chainConfig = new ChainConfig()
    //    {
    //        chainId = "0x89",
    //        rpcTarget = "https://1rpc.io/matic",
    //        chainNamespace = Web3Auth.ChainNamespace.EIP155
    //    };

    //    JArray paramsArray = new JArray
    //    {
    //        "Hello, World!",
    //        getPublicAddressFromPrivateKey(web3Auth.getPrivKey()),
    //        "Android"
    //    };

    //    web3Auth.request(chainConfig, "personal_sign", paramsArray);
    //}

    //public string getPublicAddressFromPrivateKey(string privateKeyHex)
    //{
    //    byte[] privateKeyBytes = Hex.Decode(privateKeyHex);

    //    // Create the EC private key parameters
    //    BigInteger privateKeyInt = new BigInteger(1, privateKeyBytes);
    //    var ecParams = SecNamedCurves.GetByName("secp256k1");
    //    ECPoint q = ecParams.G.Multiply(privateKeyInt);

    //    // Get the public key bytes
    //    byte[] publicKeyBytes = q.GetEncoded(false).Skip(1).ToArray();

    //    // Compute the Keccak-256 hash of the public key
    //    var digest = new KeccakDigest(256);
    //    byte[] hash = new byte[digest.GetDigestSize()];
    //    digest.BlockUpdate(publicKeyBytes, 0, publicKeyBytes.Length);
    //    digest.DoFinal(hash, 0);

    //    // Take the last 20 bytes of the hash as the address
    //    byte[] addressBytes = hash.Skip(12).ToArray();
    //    string publicAddress = "0x" + BitConverter.ToString(addressBytes).Replace("-", "").ToLower();

    //    return publicAddress;
    //}
}
