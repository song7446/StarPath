using UnityEngine;
using System.Collections;
using System;

namespace BookCurlPro
{
    public class PageFlipper : MonoBehaviour
    {
        public float duration;
        public BookPro book;
        bool isFlipping = false;
        Action finish;
        float elapsedTime = 0;
        float xc, pageWidth, pageHeight;
        FlipMode flipMode;

        // 💡 종이가 들리는 최대 높이 조절 (0.1f = 원래 높이의 10%만 살짝 들림)
        [Range(0.0f, 1.0f)]
        public float liftRatio = 0.15f; 

        public static void FlipPage(BookPro book, float duration, FlipMode mode, Action OnComplete)
        {
            PageFlipper flipper = book.GetComponent<PageFlipper>();
            if (!flipper) flipper = book.gameObject.AddComponent<PageFlipper>();
                
            flipper.enabled = true;
            flipper.book = book;
            flipper.isFlipping = true;
            flipper.duration = duration - Time.deltaTime;
            flipper.finish = OnComplete;
            flipper.xc = (book.EndBottomLeft.x + book.EndBottomRight.x) / 2;
            flipper.pageWidth = (book.EndBottomRight.x - book.EndBottomLeft.x) / 2;
            flipper.pageHeight = Mathf.Abs(book.EndBottomRight.y);
            flipper.flipMode = mode;
            flipper.elapsedTime = 0;
            
            float x;
            if (mode == FlipMode.RightToLeft)
            {
                x = flipper.xc + (flipper.pageWidth * 0.99f);
                book.DragRightPageToPoint(new Vector3(x, flipper.GetY(x), 0));
            }
            else
            {
                x = flipper.xc - (flipper.pageWidth * 0.99f);
                book.DragLeftPageToPoint(new Vector3(x, flipper.GetY(x), 0));
            }
        }

        // 💡 [핵심] Y값(들리는 높이)을 구하는 공식을 완전히 새로 짰습니다.
        private float GetY(float x)
        {
            // 중앙에서 가장 높이 들리되, 그 높이를 liftRatio(15%)로 깎아버립니다.
            float liftHeight = pageHeight * liftRatio; 
            return -pageHeight + liftHeight * (1f - ((x - xc) * (x - xc)) / (pageWidth * pageWidth));
        }

        void Update()
        {
            if (isFlipping)
            {
                elapsedTime += Time.deltaTime;
                if (elapsedTime < duration)
                {
                    if (flipMode == FlipMode.RightToLeft)
                    {
                        float x = xc + (0.5f - elapsedTime / duration) * 2 * (pageWidth);
                        book.UpdateBookRTLToPoint(new Vector3(x, GetY(x), 0));
                    }
                    else
                    {
                        float x = xc - (0.5f - elapsedTime / duration) * 2 * (pageWidth);
                        book.UpdateBookLTRToPoint(new Vector3(x, GetY(x), 0));
                    }
                }
                else
                {
                    book.Flip();
                    isFlipping = false;
                    this.enabled = false;
                    if (finish != null) finish();
                }
            }
        }
    }
}