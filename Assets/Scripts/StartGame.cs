using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StartGame : MonoBehaviour
{

    // Use this for initialization
    int currentPostion_;
    int targetPosition_;
   // Vector4 turnPosition_;
    float round_ = 0.0f;

    float lowTime_;
    float highTime_;
    float currentTime_;
    float addTime_;
    float currentStepTime_;

    bool isStart_;
    bool isEnd_;
    bool isStop_;
    bool goingtotarget_;

    //float duration_;
    List<GameObject> DisableCoronas_;

    List<GameObject> coronas_;
    float speed_ = 10.0f;

    bool isGaming_ = false;

    GameObject panel_;

    void Start () {
        targetPosition_ = 9;
        currentPostion_ = Random.Range(0,19);
        coronas_ = new List<GameObject>();
        DisableCoronas_ = new List<GameObject>();
        //turnPosition_ = new Vector4(0, 7, 10, 17);

//         for (int i = 0; i < 20; i++)
//         {
//             GameObject corona = GameObject.Find(i.ToString());
//             corona.SetActive(false);
//             coronas_.Add(corona);
//         }

        panel_ = GameObject.Find("panel");
        if(panel_ != null)
            panel_.SetActive(false);

        lowTime_ = 0.6f;
        highTime_ = 0.02f;
        currentTime_ = 0.0f;
        addTime_ = 0.05f;
        currentStepTime_ = lowTime_;

        resetgame();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(isGaming_)
            gaming();
    }

    void pickgame()
    {

    }

    void gaming()
    {
        float time = Time.deltaTime;
        DisableCoronas(time * speed_);
        GameLogic(time);
    }

    void resetgame()
    {
        isStart_ = true;
        isEnd_ = false;
        isStop_ = false;
        goingtotarget_ = false;
    }

    void caculate(float deltatime)
    {
        currentTime_ += deltatime;
        if (currentTime_ >= currentStepTime_)
        {
            currentStepTime_ += addTime_;
            if (currentStepTime_ < highTime_ && addTime_ < 0)
                currentStepTime_ = highTime_;
            if (currentStepTime_ >= lowTime_ && addTime_ > 0)
                currentStepTime_ = lowTime_;

            DisableCoronas_.Add(coronas_[currentPostion_]);
            currentPostion_ += 1;
            round_ += 0.05f;

            if (currentPostion_ > 19)
                currentPostion_ = 0;

            if (round_ >= 4)
            {
                int step = targetPosition_ - currentPostion_;
                if (step < 0)
                    step += 20;
                if (step > 3 && step < 8)
                {
                    goingtotarget_ = true;

                    isEnd_ = true;
                    isStart_ = false;
                }
            }

            GotoTarget();
            coronas_[currentPostion_].SetActive(true);
            resetCorona();

            currentTime_ = 0;
        }
    }

    void DisableCoronas(float deltatime)
    {
        foreach (GameObject corona in DisableCoronas_)
        {
            UnityEngine.UI.Image image = corona.GetComponent<UnityEngine.UI.Image>();
            Color end = new Color(image.color.r, image.color.g, image.color.b, 0.0f);
            image.color = Color.Lerp(image.color, end, deltatime);
        }
    }

    void resetCorona()
    {
        UnityEngine.UI.Image image = coronas_[currentPostion_].GetComponent<UnityEngine.UI.Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1.0f);
        DisableCoronas_.Remove(coronas_[currentPostion_]);
    }

    void GotoTarget()
    {
        if (!goingtotarget_)
            return;

        if (currentPostion_ == targetPosition_)
            isStop_ = true;
    }

    public void TargetPosition( int position )
    {
        targetPosition_ = position;
    }

    public int TargetPosition()
    {
        return targetPosition_;
    }

    void GameLogic( float deltatime )
    {
        if (isStop_)
            return;

        if (isStart_)
        {
            addTime_ = -0.1f;
            caculate(deltatime);
        }

        if( isEnd_ )
        {
            addTime_ = 0.1f;
            caculate(deltatime);
        }
    }
}
