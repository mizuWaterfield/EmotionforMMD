//
//AutoBlink.cs改変
//オート目パチ・表情変化スクリプト
//2014/06/23 N.Kobayashi
//2018/02/10 Nonomura


using UnityEngine;
using System.Collections;
using System.Security.Policy;

namespace UnityChan
{
    public class EmotionforMMD : MonoBehaviour
    {

        public bool isActive = true;				//オート目パチ有効
        public SkinnedMeshRenderer ref_SMR_EYE_DEF;	
        public SkinnedMeshRenderer ref_SMR_EL_DEF;	
        public float ratio_Close = 55.0f;			//閉じ目ブレンドシェイプ比率
        public float ratio_HalfClose = 20.0f;		//半閉じ目ブレンドシェイプ比率
        public int closeID; //閉じ目シェイプ番号
        [HideInInspector]
        public float
            ratio_Open = 0.0f;
        private bool timerStarted = false;			//タイマースタート管理用
        private bool isBlink = false;				//目パチ管理用

        public float timeBlink = 0.4f;				//目パチの時間
        private float timeRemining = 0.0f;			//タイマー残り時間

        public float threshold = 0.3f;				// ランダム判定の閾値
        public float interval = 3.0f;				// ランダム判定のインターバル

        public int angryID, angryRatio;　//怒
        public int sadID, sadRatio;　//哀
        public int straightMouthID, straightMouthRatio;　//口角まっすぐ
        public int smileID, smileRatio;　//喜

        private bool isEmotional=false;


        enum Status
        {
            Close,
            HalfClose,
            Open	//目パチの状態
        }


        private Status eyeStatus;	//現在の目パチステータス

        void Awake()
        {
            //ref_SMR_EYE_DEF = GameObject.Find("EYE_DEF").GetComponent<SkinnedMeshRenderer>();
            //ref_SMR_EL_DEF = GameObject.Find("EL_DEF").GetComponent<SkinnedMeshRenderer>();

        }



        // Use this for initialization
        void Start()
        {
            ResetTimer();
            // ランダム判定用関数をスタートする
            StartCoroutine("RandomChange");
        }

        //タイマーリセット
        void ResetTimer()
        {
            timeRemining = timeBlink;
            timerStarted = false;
        }

        // Update is called once per frame
        void Update()
        {

            //////////////表情遷移////////////////////
            if (OVRInput.GetDown(OVRInput.RawButton.A)&&!isEmotional)
            {
              //Debug.Log("Aボタンを押した");
                isEmotional = true;
                ref_SMR_EYE_DEF.SetBlendShapeWeight(angryID, angryRatio);
                ref_SMR_EYE_DEF.SetBlendShapeWeight(straightMouthID,straightMouthRatio);

            }
            else if (OVRInput.GetDown(OVRInput.RawButton.B) && !isEmotional) {
                //Debug.Log("Bボタンを押した");
                isEmotional = true;
                ref_SMR_EYE_DEF.SetBlendShapeWeight(sadID, sadRatio);
                ref_SMR_EYE_DEF.SetBlendShapeWeight(straightMouthID, straightMouthRatio);
            
            }
            else if (OVRInput.GetDown(OVRInput.RawButton.X) && !isEmotional && !isBlink) {
                //Debug.Log("Xボタンを押した");
                isEmotional = true;
                isActive = false;
                ref_SMR_EYE_DEF.SetBlendShapeWeight(smileID, smileRatio);



            }
            else if (OVRInput.GetDown(OVRInput.RawButton.Y))
            {
                //Debug.Log("Yボタンを押した");
                isEmotional = false;
                isActive = true;

                //reset
                ref_SMR_EYE_DEF.SetBlendShapeWeight(angryID, 0);
                ref_SMR_EYE_DEF.SetBlendShapeWeight(sadID, 0);
                ref_SMR_EYE_DEF.SetBlendShapeWeight(straightMouthID, 0);
                ref_SMR_EYE_DEF.SetBlendShapeWeight(smileID, 0);



            }

            
            if (OVRInput.GetUp(OVRInput.RawButton.A) && isEmotional)
            {
                //Debug.Log("Aボタンをはなした");
                isEmotional = false;
                ref_SMR_EYE_DEF.SetBlendShapeWeight(angryID, 0);
                ref_SMR_EYE_DEF.SetBlendShapeWeight(straightMouthID, 0);

            }
            else if (OVRInput.GetUp(OVRInput.RawButton.B) && isEmotional)
            {
                //Debug.Log("Bボタンをはなした");
                isEmotional = false;
                ref_SMR_EYE_DEF.SetBlendShapeWeight(sadID, 0);
                ref_SMR_EYE_DEF.SetBlendShapeWeight(straightMouthID, 0);

            }
            else if (OVRInput.GetUp(OVRInput.RawButton.X) && isEmotional)
            {
                //Debug.Log("Xボタンをはなした");
                isEmotional = false;
                isActive = true;
                ref_SMR_EYE_DEF.SetBlendShapeWeight(smileID, 0);



            }











            
            if (!timerStarted)
            {
                eyeStatus = Status.Close;
                timerStarted = true;
            }
            if (timerStarted)
            {
                timeRemining -= Time.deltaTime;
                if (timeRemining <= 0.0f)
                {
                    eyeStatus = Status.Open;
                    ResetTimer();
                }
                else if (timeRemining <= timeBlink * 0.3f)
                {
                    eyeStatus = Status.HalfClose;
                }
            }
        }

        void LateUpdate()
        {
            if (isActive)
            {
                if (isBlink)
                {
                    switch (eyeStatus)
                    {
                        case Status.Close:
                            SetCloseEyes();
                            break;
                        case Status.HalfClose:
                            SetHalfCloseEyes();
                            break;
                        case Status.Open:
                            SetOpenEyes();
                            isBlink = false;
                            break;
                    }
                    //Debug.Log(eyeStatus);
                }
            }
        }

        void SetCloseEyes()
        {
            ref_SMR_EYE_DEF.SetBlendShapeWeight(closeID, ratio_Close);
            //ref_SMR_EL_DEF.SetBlendShapeWeight(47, ratio_CloseU);
        }

        void SetHalfCloseEyes()
        {
            ref_SMR_EYE_DEF.SetBlendShapeWeight(closeID, ratio_HalfClose);
            //ref_SMR_EL_DEF.SetBlendShapeWeight(47, ratio_HalfCloseU);
        }

        void SetOpenEyes()
        {
            ref_SMR_EYE_DEF.SetBlendShapeWeight(closeID, ratio_Open);
           // ref_SMR_EL_DEF.SetBlendShapeWeight(47, ratio_Open);
        }

        // ランダム判定用関数
        IEnumerator RandomChange()
        {
            // 無限ループ開始
            while (true)
            {
                //ランダム判定用シード発生
                float _seed = Random.Range(0.0f, 1.0f);
                if (!isBlink)
                {
                    if (_seed > threshold)
                    {
                        isBlink = true;
                    }
                }
                // 次の判定までインターバルを置く
                yield return new WaitForSeconds(interval);
            }
        }
    }
}