using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    private Dictionary<string, GameObject> _dialogMap = new Dictionary<string, GameObject>();
    private List<string> _dialogNameList = new List<string>();
    private string _dialogRootDir = "Prefabs/Dialogs/";
    private string _dialogBgName = "DialogBg";

    public static string StoreDialog = "ScoreDialog";

    void Start()
    {
        Debug.Log("hello");
    }

    GameObject GetDialog(string name)
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

    public void ShowDialog(string name)
    {
        GameObject dialog = GetDialog(name);
        if(dialog != null)
        {
            _dialogMap.Add(name, dialog);
            _dialogNameList.Add(name);
        }
    }

    public void CloseDialog()
    {
        if (_dialogNameList.Count == 0) return;
        int lastIndex = _dialogNameList.Count - 1;
        string lastDialogName = _dialogNameList[lastIndex];
        if(lastDialogName != null)
        {
            _dialogNameList.Remove(lastDialogName);
            GameObject dialog = _dialogMap[lastDialogName];
            Destroy(dialog);
        }
    }
}