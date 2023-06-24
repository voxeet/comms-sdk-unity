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

namespace DolbyIO.Comms.Unity
{
    [AddComponentMenu("Dolby.io Comms/Devices/Audio Device Dropdown", 250)]
    public class AudioDeviceDropdown : MonoBehaviour
    {
        [Tooltip("The the devices direction to display.")]
        public DeviceDirection _direction = DeviceDirection.Input;

        private DolbyIOSDK _sdk = DolbyIOManager.Sdk;
        private TMP_Dropdown _dropdown;

        private List<AudioDevice> _currentDevices;

        void Awake()
        {
            _dropdown = GetComponent<TMP_Dropdown>();
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

                _currentDevices = await _sdk.MediaDevice.GetAudioDevicesAsync();
                var current = await _sdk.MediaDevice.GetCurrentAudioInputDeviceAsync();

                var names = _currentDevices
                                .FindAll(d => d.Direction == _direction)
                                .ConvertAll(device => device.Name);

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
            try
            {
                var device = _currentDevices[_dropdown.value];
                if (_direction == DeviceDirection.Input)
                {
                    _sdk.MediaDevice.SetPreferredAudioInputDeviceAsync(device);
                }
                else
                {
                    _sdk.MediaDevice.SetPreferredAudioOutputDeviceAsync(device);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to select device." + e.Message);
            }
        }
    }
}
