using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _UI {
    public class HiveDialogCoinsAnimator : MonoBehaviour {
        [SerializeField] private RectTransform drop;
        [SerializeField] private RectTransform coinAppear;
        [SerializeField] private RectTransform coinScaleAnim;
        [SerializeField] private Vector2 dropOriginAnchPos;
        [SerializeField] private Vector2 coinAppearAnchPos;

        private Image coinAppearImage;
        private Image coinScaleImage;

        //----------------------------------------------------------------------------

        private void Start() {
        }

        public void StartAnimation() {
            coinAppearImage = coinAppear.GetComponent<Image>();
            coinScaleImage = coinScaleAnim.GetComponent<Image>();

            StartCoroutine(AnimateHoneyProduction());
        }

        private IEnumerator AnimateHoneyProduction() {
            while (true) {
                ResetAnimItemsState();

                //drop anim
                const float dropFillDur = 0.7f;
                drop.gameObject.SetActive(true);
                drop.localScale = new Vector3(0.8f, 0.2f, 1);
                drop.DOScale(Vector3.one, 0.7f).SetEase(Ease.Linear);
                yield return new WaitForSeconds(dropFillDur * 0.9f);

                //move drop
                yield return drop.DOMove(coinAppear.position + Vector3.up * 0.4f, 1).SetEase(Ease.InQuad).WaitForCompletion();
                drop.gameObject.SetActive(false);
                


                //show coin
                var appearDur = 0.5f;
                coinAppear.gameObject.SetActive(true);
                coinAppear.DOScale(Vector3.one, appearDur).SetEase(Ease.OutCirc);
                coinAppear.parent.DOPunchPosition(Vector3.down * 15f, appearDur * 0.9f, 1, 0).SetDelay(appearDur * 0.1f);
                yield return coinAppearImage.DOFade(1, appearDur).SetEase(Ease.OutCirc).WaitForCompletion();
                yield return new WaitForSeconds(0.25f);

                //show add collect animation
                //do not jump

                //shake and move to target
                /*yield return coinAppear.DORotate(Vector3.forward * 7, 0.15f).SetEase(Ease.Linear).SetLoops(7, LoopType.Yoyo).WaitForCompletion();
                yield return coinAppear.DORotate(Vector3.zero, 0.15f).SetEase(Ease.Linear).WaitForCompletion();*/
                //yield return coinAppear.DOMove(coinScaleAnim.position, 1).WaitForCompletion();

                //yield return coinAppear.DOJump(coinScaleAnim.position, 1.7f, 1, 1).SetEase(Ease.InCubic).WaitForCompletion();
                yield return coinAppearImage.DOFade(0, 1.7f).SetEase(Ease.InCubic).WaitForCompletion();

                /*//make scale fx
                const float scaleDur = 1f;
                coinAppear.gameObject.SetActive(false);
                coinScaleAnim.gameObject.SetActive(true);
                coinScaleImage.DOFade(0, scaleDur).SetEase(Ease.OutQuad);
                yield return coinScaleAnim.DOScale(Vector3.one * 2, scaleDur).SetEase(Ease.OutCirc).WaitForCompletion();*/

                yield return new WaitForSeconds(1);
            }
        }

        //----------------------------------------------------------------------------
        private void ResetAnimItemsState() {
            coinAppear.localScale = Vector3.one * 0.4f;
            coinAppearImage.color = new Color(1, 1, 1, 0);
            coinAppear.anchoredPosition = coinAppearAnchPos;
            coinAppear.eulerAngles = Vector3.zero;

            drop.localScale = Vector3.one;
            drop.anchoredPosition = dropOriginAnchPos;

            coinScaleAnim.gameObject.SetActive(false);
            coinScaleAnim.localScale = Vector3.one;
            coinScaleImage.color = new Color(1, 1, 1, 1);
        }
    }
}