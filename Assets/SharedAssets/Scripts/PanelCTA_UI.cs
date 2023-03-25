using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if !UNITY_EDITOR
using Luna.Unity;
#endif
public enum UiCart
    {
     old_ui,
     new_ui

}
public class PanelCTA_UI : MonoBehaviour
{
    [Header("------UI Components----")]
    [SerializeField] private Button btnDownload;
    [SerializeField] private Button btnRetry;


    public void Start()
    {
        btnDownload.onClick.AddListener(() => DownloadCallBack());
        btnRetry.onClick.AddListener(() => RetryCallBack());

        btnDownload.transform.GetChild(0).GetComponent<RectTransform>().DOScale(Vector3.one * .8f, .8f).SetLoops(-1, LoopType.Yoyo);
    }

    private void RetryCallBack()
    {
        SceneManager.LoadScene(0);
    }

    private void DownloadCallBack()
    {
#if !UNITY_EDITOR
        Playable.InstallFullGame();
#endif

    }

    public void DownloadButtonCallBack()
    {
#if !UNITY_EDITOR
        Playable.InstallFullGame();
#endif

    }
}
