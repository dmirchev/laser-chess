using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LaserChess
{
    public class PlayerTurnIndicatorUI : MonoBehaviour
    {
        [SerializeField] private LayoutElement imageLayoutElement;
        [SerializeField] private LayoutElement spaceLayoutElement;

        [SerializeField] private float defaultImageRatio = 0.8f;
        [SerializeField] private float defaultSpaceRatio = 0.2f;

        [SerializeField] private float animationSpeed = 3.0f;
        [SerializeField] private float animationDelay = 0.5f;

        void Awake()
        {
            SetImageLayoutElementFlexibleWidth(defaultImageRatio);
            SetSpaceLayoutElementFlexibleWidth(defaultSpaceRatio);
        }

        public void StartAnimation(bool state)
        {
            float currentImageWidth = GetImageLayoutElementFlexibleWidth();
            float currentSpaceWidth = GetSpaceLayoutElementFlexibleWidth();
            StartCoroutine(state ? Show(currentImageWidth, currentSpaceWidth) : Hide(currentImageWidth, currentSpaceWidth));
        }

        private IEnumerator Show(float lastImageWidth, float lastSpaceWidth)
        {
            // Wait For Other Hide Animation
            yield return new WaitForSeconds(animationDelay);
            
            float increment = 0;

            while (increment < 1.0f)
            {
                increment += animationSpeed * Time.deltaTime;

                SetImageLayoutElementFlexibleWidth(Mathf.Lerp(lastImageWidth, 1.0f, increment));
                SetSpaceLayoutElementFlexibleWidth(Mathf.Lerp(lastSpaceWidth, 0.0f, increment));

                yield return null;
            }
        }

        private IEnumerator Hide(float lastImageWidth, float lastSpaceWidth)
        {
            float increment = 0;

            while (increment < 1.0f)
            {
                increment += animationSpeed * Time.deltaTime;

                SetImageLayoutElementFlexibleWidth(Mathf.Lerp(lastImageWidth, defaultImageRatio, increment));
                SetSpaceLayoutElementFlexibleWidth(Mathf.Lerp(lastSpaceWidth, defaultSpaceRatio, increment));

                yield return null;
            }
        }

        private void SetImageLayoutElementFlexibleWidth(float flexibleWidth)
        {
            imageLayoutElement.flexibleWidth = flexibleWidth;
        }

        private void SetSpaceLayoutElementFlexibleWidth(float flexibleWidth)
        {
            spaceLayoutElement.flexibleWidth = flexibleWidth;
        }

        private float GetImageLayoutElementFlexibleWidth()
        {
            return imageLayoutElement.flexibleWidth;
        }

        private float GetSpaceLayoutElementFlexibleWidth()
        {
            return spaceLayoutElement.flexibleWidth;
        }
    }
}