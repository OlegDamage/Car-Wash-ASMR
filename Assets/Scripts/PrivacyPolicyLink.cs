using UnityEngine;

public class PrivacyPolicyLink : MonoBehaviour
{
    public const string PolicyUrl = "https://olegdamage.github.io/Car-Wash-ASMR/";

    public void OpenPolicy()
    {
        Application.OpenURL(PolicyUrl);
    }
}
