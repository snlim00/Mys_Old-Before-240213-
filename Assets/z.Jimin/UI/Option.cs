using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Option : UI_System
{
    [Header("�ؽ�Ʈ �ӵ�")]
    [SerializeField] [Range(0, 10)]
    private int text_Speed = 5;
    [SerializeField] private GameObject textScroll;

    [Header("�ڵ� �ؽ�Ʈ �ӵ�")]
    [SerializeField] [Range(0, 10)]
    private int autoText_Speed = 5;
    [SerializeField] private GameObject autoTextScroll;

    [Header("�ؽ�Ʈ �б�")]
    [SerializeField] private Text readingText;

    [Header("������ ����")]
    [SerializeField] [Range(0, 100)]
    private int master_Volume = 50;
    [SerializeField] private GameObject masterScroll;

    [Header("ȿ���� ����")]
    [SerializeField] [Range(0, 100)] 
    private int effect_Volume = 50;
    [SerializeField] private GameObject effectScroll;

    [Header("����� ����")]
    [SerializeField] [Range(0, 100)]
    private int BGM_Volume = 50;
    [SerializeField] private GameObject BGMScroll;

    [Header("���� ��Ҹ� ����")]
    [SerializeField] [Range(0, 100)] 
    private int yoonHa_Voice = 50;
    [SerializeField] private GameObject yoonHa_Scroll;
    [SerializeField] private Slider yoonha_Slider;
    
    [Header("���� ��Ҹ� ����")]
    [SerializeField]
    [Range(0, 100)]
    private int seEun_Voice = 50;
    [SerializeField] private GameObject seEun_Scroll;

    [Header("���� ��Ҹ� ����")]
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

    #region ��ũ�� ����

    public void ScreenFull()    //��ü ��ũ��
    {
        int width = 1080;
        int height = 1920;
        if (!isScreen)
        {   isScreen = true;
            Screen.SetResolution(width, height, isScreen);
            Debug.Log("ȭ�� ��ȯ \n��ü ȭ�� ���� : " + isScreen);}
    }

    public void ScreenWindow()  //â���
    {
        int width = 1080;
        int height = 1920;
        if (isScreen)
        {   isScreen = false;
            Screen.SetResolution(width, height, isScreen);
            Debug.Log("ȭ�� ��ȯ \n��ü ȭ�� ���� : " + isScreen);
        }
    }
    #endregion

    #region �̸����
    public void SoundListen()
    {
        auidoSo.Play();
    }
    #endregion

    #region ��ũ�� 

    public void EffectScroll()  //ȿ���� ����
    {effect_Volume = (int)(effectScroll.GetComponent<Slider>().value * 100);
   
    }
    public void MasterScroll()
    { master_Volume = (int)(masterScroll.GetComponent<Slider>().value * 100);
      
    }


    public void TextScroll()    //�ؽ�Ʈ �ӵ� ����
    {text_Speed = (int)(textScroll.GetComponent<Slider>().value * 10);
     
    }
    public void AutoTextScroll()
    {   autoText_Speed = (int)(autoTextScroll.GetComponent<Slider>().value * 10);
        readingText.DOText(readingText.text, 0.01f).SetEase(Ease.Linear);
       
    }
    public void BGM_Scroll()    //��� ����
    {BGM_Volume = (int)(BGMScroll.GetComponent<Slider>().value * 100);
        
    }
    public void YoonHaVoice()   //���� ���� ����
    { yoonHa_Voice = (int)(yoonHa_Scroll.GetComponent<Slider>().value * 100);
      
    }
    public void SeEunVoice()   //���� ���� ����
    { seEun_Voice = (int)(seEun_Scroll.GetComponent<Slider>().value * 100);
      
    }
    public void JiHyeVoice()   //���� ���� ����
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


    #region ���

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