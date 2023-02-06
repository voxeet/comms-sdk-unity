using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DolbyIO.Comms;
using DolbyIO.Comms.Unity;

public class App : MonoBehaviour
{
    private DolbyIOSDK _sdk = DolbyIOManager.Sdk;

    public string Token = "MyToken";
    public GameObject Prefab;

    // Start is called before the first frame update
    async void Start()
    {
        await _sdk.InitAsync(Token, () => {
            return Token;
        });

        await _sdk.Session.OpenAsync(new UserInfo { Name = "Anonymous" });

        _sdk.Conference.ParticipantUpdated = OnParticipantUpdated;

        await _sdk.Conference.DemoAsync(SpatialAudioStyle.Shared);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnParticipantUpdated(Participant p) 
    {
        Debug.Log(p.Info.Name);
        if (p.Status == ParticipantStatus.OnAir)
        {
            switch (p.Info.Name)
            {
                case "Dan":
                    DolbyIOManager.QueueOnMainThread(() => Instantiate(Prefab, new Vector3(-2.5f, 0.0f, 2.5f), Quaternion.identity));
                    break;
                case "Ruth":
                    DolbyIOManager.QueueOnMainThread(() => Instantiate(Prefab, new Vector3(0.0f, 0.0f, 5f), Quaternion.identity));
                    break;
                case "JC":
                    DolbyIOManager.QueueOnMainThread(() => Instantiate(Prefab, new Vector3(2.5f, 0.0f, 2.5f), Quaternion.identity));
                    break;
            }
        }
    }
}
