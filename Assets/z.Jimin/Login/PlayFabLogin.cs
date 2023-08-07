using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class PlayFabLogin : MonoBehaviour
{

    private void Start()
    {
        if(OnLogin()) //�α���
        {
            //���� ����
        }
        else
        {
            Instantiate(Resources.Load("Fail_Login"));
            Debug.Log("�α��� ����");
        }
    }

    public bool OnLogin()
    {
        SaveFile data = SaveManager.Load(GameInit.tempSaveNumber);
        if (SteamManager.Initialized)   //������ ���� �ִ°�?
        {
            if (data == null)   //���̺� ������ ���� ��
            {
                //���̺����� ����
                var id = SteamUser.GetSteamID().ToString();
                SaveFile file = new SaveFile();
                //file.account = id;
                SaveManager.Save(file, GameInit.tempSaveNumber);
                OnLogin();
                return false;
            }
            else //���̺� ������ ���� ���
            {
                var name = SteamUser.GetSteamID();

                //if (data.account == name.ToString())  //���̺� ���ϰ� ������ ��ġ�Ѵٸ�
                if(true)
                {
                    Steamworks.AppId_t appid = new AppId_t();
                    var returnValue = SteamUser.UserHasLicenseForApp(SteamUser.GetSteamID(), appid);

                    if (returnValue == EUserHasLicenseForAppResult.k_EUserHasLicenseResultHasLicense) //���μ����� ���� ��
                    {
                        Debug.Log("�α��μ���");
                        return true;
                    }
                    else   //���̼����� ���� ��
                    {
                        Debug.Log("�α��� ����!");
                        return false;
                    }
                }
                else
                {
                    //���̵�� ���̺������� ��ġ���� ������� ���ο� ���̺����� �����
                    var id = SteamUser.GetSteamID().ToString();
                    SaveFile file = new SaveFile();
                    //file.account = id;
                    SaveManager.Save(file, GameInit.tempSaveNumber);
                    Debug.Log("������ ���ų� �߸��� �����Դϴ�.");
                    OnLogin();
                    return false;
                }
            }
        }
        else
        {
            Debug.Log("Steam�� ������� �ʾҽ��ϴ�. \nsteam�� �������ּ���."); 
            return false;
        }

    }




}//end class