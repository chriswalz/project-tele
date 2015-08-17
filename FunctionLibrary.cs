using UnityEngine;
using System.Collections;

public static class FunctionLibrary : object
{
    public delegate void myDelegate();
    public delegate void myDelegateInt(int i);

    //Moves a menu element by the received ammount in time
    public static IEnumerator MoveElementBy(Transform element, Vector2 ammount, float time)
    {
        float i = 0.0f;
        float rate = 1.0f / time;

        Vector2 startPos = element.position;
        Vector2 endPos = element.position;
        endPos += ammount;

        while (i < 1.0)
        {
            i += Time.deltaTime * rate;
            element.localPosition = Vector3.Lerp(startPos, endPos, i);

            yield return 0;
        }
    }
    //Rescales the given element to the given scale in time
    public static IEnumerator ScaleTo(Transform element, Vector2 endScale, float time)
    {
        float i = 0.0f;
        float rate = 1.0f / time;

        Vector2 startScale = element.localScale;

        while (i < 1.0)
        {
            i += Time.deltaTime * rate;
            element.localScale = Vector3.Lerp(startScale, endScale, i);

            yield return 0;
        }
    }
    //Sets the active state of the go to state, after time
    public static IEnumerator ChangeEnabledState(GameObject go, bool state, float time)
    {
        float i = 0.0f;
        float rate = 1.0f / time;

        while (i < 1.0)
        {
            i += Time.deltaTime * rate;
            yield return 0;
        }

        go.SetActive(state);
    }
    //Calls the passed void function with no arguments after delay
    public static IEnumerator CallWithDelay(myDelegate del, float delay)
    {
        yield return new WaitForSeconds(delay);
        del();
    }
    //Calls the passed void function with no arguments after delay
    public static IEnumerator CallWithDelay(myDelegateInt del, int num, float delay)
    {
        yield return new WaitForSeconds(delay);
        del(num);
    }
    //Fade overlay opacity
    public static IEnumerator FadeScreen(SpriteRenderer overlay, float time, float to)
    {
        //Set the screen fade's color to end in time
        float i = 0.0f;
        float rate = 1.0f / time;

        Color start = overlay.color;
        Color end = new Color(start.r, start.g, start.b, to);

        while (i < 1.0)
        {
            i += Time.deltaTime * rate;
            overlay.color = Color.Lerp(start, end, i);
            yield return 0;
        }
    }
}
