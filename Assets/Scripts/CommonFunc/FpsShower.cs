/*using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FpsShower : MonoBehaviour
{

   public float updateInterval = 0.2F;
   public GUIText FPS_text;
   //private float accum = 0; // FPS accumulated over the interval
   private int frames = 0; // Frames drawn over the interval
   private float timeleft; // Left time for current interval
   IEnumerator  Start()
   {
       yield return null;
       yield return null;
       yield return null;
       yield return null;
       yield return null;
       if (!FPS_text)
       {
           FPS_text = gameObject.AddComponent<GUIText>();
           gameObject.layer = 5;
           gameObject.transform.position = new Vector3(0, 0, 0);
           FPS_text.pixelOffset = new Vector2(30, Screen .height -10);
       }
       timeleft = updateInterval;
       recorTime = Time.realtimeSinceStartup;
   }

   private float recorTime;
   void Update()
   {
       if (!FPS_text) return;
       timeleft -= Time.deltaTime;
       ++frames;
       if (timeleft <= 0.0)
       {           
           float fps = frames/(Time.realtimeSinceStartup - recorTime);
           string format = System.String.Format("FPS: {0:F2}", fps);
           recorTime = Time.realtimeSinceStartup;
           FPS_text.text = format ;
           if (fps < 20)
               FPS_text.color = Color.yellow;
           else
               if (fps < 10)
                   FPS_text.color = Color.red;
               else
                   FPS_text.color = Color.green;
           timeleft = updateInterval;
           frames = 0;
       }
      
   }


}*/


using UnityEngine;
using System.Collections;

public class ShowFPS_OnGUI : MonoBehaviour
{

    public float fpsMeasuringDelta = 1.0f;

    private float timePassed;
    private int m_FrameCount = 0;
    private float m_FPS = 0.0f;

    private void Start()
    {
        timePassed = 0.0f;
    }

    private void Update()
    {
        m_FrameCount = m_FrameCount + 1;
        timePassed = timePassed + Time.deltaTime;

        if (timePassed > fpsMeasuringDelta)
        {
            m_FPS = m_FrameCount / timePassed;

            timePassed = 0.0f;
            m_FrameCount = 0;
        }
    }

    private void OnGUI()
    {
        GUIStyle bb = new GUIStyle();
        bb.normal.background = null;    //这是设置背景填充的
        bb.normal.textColor = new Color(1.0f, 0.5f, 0.0f);   //设置字体颜色的
        bb.fontSize = 40;       //当然，这是字体大小
        uint fps = (uint)m_FPS;
        //居中显示FPS
        GUI.Label(new Rect((Screen.width / 2) - 40, 0, 200, 200), "FPS: " + fps, bb);
    }
}