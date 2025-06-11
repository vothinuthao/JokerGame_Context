using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AnimLogoShop : MonoBehaviour
{
    [SerializeField] private Image[] sprites;
    public float delay = 0.1f;

    private void Start()
    {
        StartCoroutine(PlayAnimationLoop()); // Bắt đầu coroutine
    }

    private IEnumerator PlayAnimationLoop()
    {
        while (true) // Vòng lặp vô tận
        {
            foreach (var sprite in sprites)
            {
                sprite.enabled = false;
            }

            Sequence mySequence = DOTween.Sequence();

            for (int i = 0; i < sprites.Length; i++)
            {
                int index = i; // Lưu lại index để sử dụng trong lambda expression
                mySequence.AppendCallback(() => sprites[index].enabled = true)
                    .AppendInterval(delay);
            }

            yield return mySequence.WaitForCompletion(); // Đợi animation kết thúc
        }
    }
}