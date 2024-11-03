using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeStage : MonoBehaviour
{
    public void NewGame(string newgame)
    {
        Debug.Log("Switching to new game: " + newgame);
        SceneManager.LoadScene(newgame);
    }

    public void ToSetting(string settingmenu)
    {
        Debug.Log("Switching to setting scene: " + settingmenu);
        SceneManager.LoadScene(settingmenu);
    }

    public void Back(string gobai)
    {
        Debug.Log("Switching back to scene: " + gobai);
        SceneManager.LoadScene(gobai);
    }

    public void ToProps(string propsmenu)
    {
        Debug.Log("Switching to props scene: " + propsmenu);
        SceneManager.LoadScene(propsmenu);
    }

    public void ToAchievement(string achievementmenu)
    {
        Debug.Log("Switching to achievement scene: " + achievementmenu);
        SceneManager.LoadScene(achievementmenu);
    }

    public void ToMap(string mapmenu)
    {
        Debug.Log("Switching to map scene: " + mapmenu);
        SceneManager.LoadScene(mapmenu);
    }
}
