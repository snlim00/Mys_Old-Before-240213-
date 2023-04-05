using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Option : UI_System
{
    [Header("텍스트 속도")]
    [SerializeField] [Range(0, 10)]
    private int text_Speed = 5;
    [SerializeField] private GameObject textScroll;

    [Header("자동 텍스트 속도")]
    [SerializeField] [Range(0, 10)]
    private int autoText_Speed = 5;
    [SerializeField] private GameObject autoTextScroll;

    [Header("텍스트 읽기")]
    [SerializeField] private Text readingText;

    [Header("마스터 볼륨")]
    [SerializeField] [Range(0, 100)]
    private int master_Volume = 50;
    [SerializeField] private GameObject masterScroll;

    [Header("효과음 음량")]
    [SerializeField] [Range(0, 100)] 
    private int effect_Volume = 50;
    [SerializeField] private GameObject effectScroll;

    [Header("배경음 음량")]
    [SerializeField] [Range(0, 100)]
    private int BGM_Volume = 50;
    [SerializeField] private GameObject BGMScroll;

    [Header("윤하 목소리 음량")]
    [SerializeField] [Range(0, 100)] 
    private int yoonHa_Voice = 50;
    [SerializeField] private GameObject yoonHa_Scroll;
    [SerializeField] private Slider yoonha_Slider;
    
    [Header("세은 목소리 음량")]
    [SerializeField]
    [Range(0, 100)]
    private int seEun_Voice = 50;
    [SerializeField] private GameObject seEun_Scroll;

    [Header("지혜 목소리 음량")]
    [SerializeField]
    [Range(0, 100)]
    private int jiHye_Voice = 50;
    [SerializeField] private GameObject jiHye_Scroll;

    AudioSource auidoSo;

    bool isScreen = true;


    private void Start()
    {
        SetOption();
        auidoSo = this.gameObject.GetComponent<AudioSource>();
    }

    #region 스크린 조절

    public void ScreenFull()    //전체 스크린
    {
        int width = 1080;
        int height = 1920;
        if (!isScreen)
        {   isScreen = true;
            Screen.SetResolution(width, height, isScreen);
            Debug.Log("화면 전환 \n전체 화면 상태 : " + isScreen);}
    }

    public void ScreenWindow()  //창모드
    {
        int width = 1080;
        int height = 1920;
        if (isScreen)
        {   isScreen = false;
            Screen.SetResolution(width, height, isScreen);
            Debug.Log("화면 전환 \n전체 화면 상태 : " + isScreen);
        }
    }
    #endregion

    #region 미리듣기
    public void SoundListen()
    {
        auidoSo.Play();
    }
    #endregion

    #region 스크롤 

    public void EffectScroll()  //효과음 볼륨
    {effect_Volume = (int)(effectScroll.GetComponent<Slider>().value * 100);
   
    }
    public void MasterScroll()
    { master_Volume = (int)(masterScroll.GetComponent<Slider>().value * 100);
      
    }


    public void TextScroll()    //텍스트 속도 볼륨
    {text_Speed = (int)(textScroll.GetComponent<Slider>().value * 10);
     
    }
    public void AutoTextScroll()
    {   autoText_Speed = (int)(autoTextScroll.GetComponent<Slider>().value * 10);
        readingText.DOText(readingText.text, 0.01f).SetEase(Ease.Linear);
       
    }
    public void BGM_Scroll()    //브금 볼륨
    {BGM_Volume = (int)(BGMScroll.GetComponent<Slider>().value * 100);
        
    }
    public void YoonHaVoice()   //윤하 음성 볼륨
    { yoonHa_Voice = (int)(yoonHa_Scroll.GetComponent<Slider>().value * 100);
      
    }
    public void SeEunVoice()   //윤하 음성 볼륨
    { seEun_Voice = (int)(seEun_Scroll.GetComponent<Slider>().value * 100);
      
    }
    public void JiHyeVoice()   //윤하 음성 볼륨
    { jiHye_Voice = (int)(jiHye_Scroll.GetComponent<Slider>().value * 100);
    }


    void Scrollset()
    {
        textScroll.GetComponent<Slider>().value = text_Speed * 0.1f;
        autoTextScroll.GetComponent<Slider>().value = autoText_Speed * 0.1f;
        masterScroll.GetComponent<Slider>().value = master_Volume * 0.01f;
        effectScroll.GetComponent<Slider>().value = effect_Volume * 0.01f;
        BGMScroll.GetComponent<Slider>().value = BGM_Volume * 0.01f;
        yoonHa_Scroll.GetComponent<Slider>().value = yoonHa_Voice * 0.01f;
        seEun_Scroll.GetComponent<Slider>().value = seEun_Voice * 0.01f;
        jiHye_Scroll.GetComponent<Slider>().value = jiHye_Voice * 0.01f;
    }

    void SetOption()
    {
        OptionData data = SaveOption.Load("OptionData");
        if(data !=null)
        {
            text_Speed = data.textSpeed;
            autoText_Speed = data.autoTextSpeed;
            master_Volume = data.masterVolume;
            effect_Volume = data.effectVolume;
            BGM_Volume = data.bgmVolume;
            yoonHa_Voice = data.yoonHaVoice;
            seEun_Voice = data.seEunVoice;
            jiHye_Voice = data.jiHyeVoice;
        }
        else
        {
            ResetOption();
        }
        Scrollset();
    }

    public void ResetOption()
    {
        text_Speed = 5;
        autoText_Speed = 5;
        master_Volume = 50;
        effect_Volume = 50;
        BGM_Volume = 50;
        yoonHa_Voice = 50;
        seEun_Voice = 50;
        jiHye_Voice = 50;
        Scrollset();
    }

    #endregion


    #region Json

    void SetJson()
    {
        OptionData optionData = new OptionData();
        optionData.textSpeed = text_Speed;
        optionData.autoTextSpeed = autoText_Speed;
        optionData.masterVolume = master_Volume;
        optionData.effectVolume = effect_Volume;
        optionData.bgmVolume = BGM_Volume;
        optionData.yoonHaVoice = yoonHa_Voice;
        optionData.seEunVoice = seEun_Voice;
        optionData.jiHyeVoice = jiHye_Voice;

        SaveOption.Save(optionData, "OptionData");
    }

    #endregion


    #region 상속

    private void OnEnable()
    {
        PushUI();
    }

    protected override void PushUI()
    {
        base.PushUI();
    }

    private void Update()
    {
        DeleteUI();
    }

    protected override void DeleteUI()
    {
        base.DeleteUI();
    }
    private void OnDisable()
    {
        SetJson();
    }
    #endregion

}//end class