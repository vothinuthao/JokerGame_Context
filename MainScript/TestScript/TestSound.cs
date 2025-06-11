using System.Collections;
using Frameworks.Scripts;
using UnityEngine;

public class TestSound : MonoBehaviour
{
    public void OnClickSound()
    {
        StartCoroutine(DelayTimeSound());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator DelayTimeSound()
    {
        for (int i = 1; i < 6; i++)
        {
            yield return new WaitForSeconds(300f);
            float pitch = 1f + (i / 10f);
            AudioManager.Instance.PlaySFXWithPitchChanger(AudioName.Chip1, i);
            yield return new WaitForSeconds(300f);
        }
       
        
    }
}
