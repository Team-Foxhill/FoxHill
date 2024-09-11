using FoxHill.Core.Pause;
using UnityEngine;
using UnityEngine.UI;

public class TesterScript : MonoBehaviour
{
    public Button button1;
    public Button button2;

    private void Awake()
    {
        button1.onClick.AddListener(PauseManager.Pause);
        button2.onClick.AddListener(PauseManager.Resume);
    }
}
