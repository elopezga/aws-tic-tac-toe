using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitMatchConfirmation : MonoBehaviour
{
    public GameObject target;
    public GameObject source;

    void Start()
    {
        Button btn = source.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        target.SetActive(!target.activeSelf);
    }
}
