using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DolbyIO.Comms;
using DolbyIO.Comms.Unity;
using TMPro;
using System.Threading.Tasks;

[AddComponentMenu("Dolby.io Comms/Devices/Screen Share Source Dropdown", 252)]
public class ScreenShareSourceDropdown : MonoBehaviour
{
    private DolbyIOSDK _sdk = DolbyIOManager.Sdk;
    private TMP_Dropdown _dropdown;

    private List<ScreenShareSource> _currentSources;

    public ScreenShareSource CurrentSource;

    // Use this for initialization
    void Awake()
	{
        _dropdown = GetComponent<TMP_Dropdown>();
        _dropdown.onValueChanged.AddListener(delegate
        {
            OnDropdownSelect(_dropdown);
        });
    }

    void Start()
    {
        LoadDevices().ContinueWith(t => Debug.LogError(t.Exception.Message), TaskContinuationOptions.OnlyOnFaulted);
    }


    public async Task LoadDevices()
    {
        try
        {
            _dropdown.ClearOptions();

            _currentSources = await _sdk.MediaDevice.GetScreenShareSourcesAsync();

            var titles = _currentSources.ConvertAll(device => device.Title);

            _dropdown.AddOptions(titles);
        }
        catch (DolbyIOException e)
        {
            Debug.LogError(e.Message);
        }
    }

    private void OnDropdownSelect(TMP_Dropdown value)
    {
        CurrentSource = _currentSources[_dropdown.value];
    }
}

