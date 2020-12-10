using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadEndScreen : MonoBehaviour
{
    private void OnEnable()
    {
        Cursor.visible = true;
        SceneManager.LoadScene("EndScreen");
    }
}
