using FoxHill.Core.Pause;
using FoxHill.Player;
using UnityEngine;
using UnityEngine.UI;

public class TesterScript : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;

    public PlayerManager player;

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerManager>();

        button1.onClick.AddListener(PauseManager.Pause);
        button2.onClick.AddListener(PauseManager.Resume);
        button3.onClick.AddListener(() => { player.Quest.TryStartQuest(1, 1); });
        //button4.onClick.AddListener(() => { player.TakeDamage(40f); });
    }
}
