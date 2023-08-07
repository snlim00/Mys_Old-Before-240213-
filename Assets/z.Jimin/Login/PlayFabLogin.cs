using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class PlayFabLogin : MonoBehaviour
{

    private void Start()
    {
        if(OnLogin()) //로그인
        {
            //게임 실행
        }
        else
        {
            Instantiate(Resources.Load("Fail_Login"));
            Debug.Log("로그인 실패");
        }
    }

    public bool OnLogin()
    {
        SaveFile data = SaveManager.Load(GameInit.tempSaveNumber);
        if (SteamManager.Initialized)   //스팀이 켜져 있는가?
        {
            if (data == null)   //세이브 파일이 없을 때
            {
                //세이브파일 생성
                var id = SteamUser.GetSteamID().ToString();
                SaveFile file = new SaveFile();
                //file.account = id;
                SaveManager.Save(file, GameInit.tempSaveNumber);
                OnLogin();
                return false;
            }
            else //세이브 파일이 있을 경우
            {
                var name = SteamUser.GetSteamID();

                //if (data.account == name.ToString())  //세이브 파일과 계정이 일치한다면
                if(true)
                {
                    Steamworks.AppId_t appid = new AppId_t();
                    var returnValue = SteamUser.UserHasLicenseForApp(SteamUser.GetSteamID(), appid);

                    if (returnValue == EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense) //라인센스가 있을 때
                    {
                        Debug.Log("로그인성공");
                        return true;
                    }
                    else   //라이센스가 없을 때
                    {
                        Debug.Log("로그인 실패!");
                        return false;
                    }
                }
                else
                {
                    //아이디와 세이브파일이 일치하지 않을경우 새로운 세이브파일 덮어쓰기
                    var id = SteamUser.GetSteamID().ToString();
                    SaveFile file = new SaveFile();
                    //file.account = id;
                    SaveManager.Save(file, GameInit.tempSaveNumber);
                    Debug.Log("계정이 없거나 잘못된 계정입니다.");
                    OnLogin();
                    return false;
                }
            }
        }
        else
        {
            Debug.Log("Steam이 연결되지 않았습니다. \nsteam을 연결해주세요."); 
            return false;
        }

    }




}//end class