using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DolbyIO.Comms;
using DolbyIO.Comms.Unity;

[AddComponentMenu("Dolby.io Comms/Devices/Video Device Dropdown", 251)]
public class VideoDeviceDropdown : MonoBehaviour
{
	private DolbyIOSDK _sdk = DolbyIOManager.Sdk;
	private TMP_Dropdown _dropdown;

    private List<VideoDevice> _currentDevices;

    public VideoDevice CurrentDevice;

	void Awake()
	{
		_dropdown = GetComponent<TMP_Dropdown> ();
        _dropdown.onValueChanged.AddListener(delegate
        {
            OnDropdownSelect(_dropdown);
        });
    }

	// Use this for initialization
	void Start()
	{
        LoadDevices().ContinueWith(t => Debug.LogError(t.Exception.Message), TaskContinuationOptions.OnlyOnFaulted);
	}

	public async Task LoadDevices()
	{
        try
        {
            _dropdown.ClearOptions();

            _currentDevices = await _sdk.MediaDevice.GetVideoDevicesAsync();
            var current = await _sdk.MediaDevice.GetCurrentVideoDeviceAsync();

            var names = _currentDevices.ConvertAll(device => device.Name);

            _dropdown.AddOptions(names);

            var index = names.FindIndex(d => d.Equals(current.Name));
            _dropdown.value = index;
        }
        catch (DolbyIOException e)
        {
            Debug.LogError(e.Message);
        }
    }

    private void OnDropdownSelect(TMP_Dropdown value)
    {
        CurrentDevice = _currentDevices[_dropdown.value];
    }
}

