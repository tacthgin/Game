using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    private List<GameObject> _dialogList = new List<GameObject>();
    private string _dialogRootDir = "Prefabs/Dialogs/";
    private string _dialogBgName = "DialogBg";

    public static string StoreDialog = "ScoreDialog";

    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    GameObject getDialog(string name)
    {
        GameObject dialog = (GameObject)Instantiate(Resources.Load(_dialogRootDir + name));
        if(dialog != null)
        {
            GameObject dialogBg = (GameObject)Instantiate(Resources.Load(_dialogRootDir + _dialogBgName));
            dialogBg.transform.SetParent(GameObject.Find("Canvas").transform, false);
            dialog.transform.SetParent(dialogBg.transform, false);
            return dialogBg;
        }
        return null;
    }

    public void showDialog(string name)
    {
        GameObject dialog = getDialog(name);
        if(dialog != null)
        {
            
        }
    }
}
