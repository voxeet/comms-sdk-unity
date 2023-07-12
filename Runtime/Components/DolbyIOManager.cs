using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;
using DolbyIO.Comms;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace DolbyIO.Comms.Unity
{
    [AddComponentMenu("Dolby.io Comms/Dolby.io Manager", 1)]
    [Inspectable]
    public class DolbyIOManager : MonoBehaviour
    {

        [Inspectable]
        [SerializeReference]
        private static DolbyIOSDK _sdk = new DolbyIOSDK(ComponentName.Unity);

        private static List<Action> _backlog = new List<Action>();

        private static HttpClient _client = new HttpClient();

        private float _timePosition = 0.0f;
        private float _timeDirection = 0.0f;

        [Inspectable]
        public static DolbyIOSDK Sdk { get => _sdk; }

        public static DolbyIOManager Instance {get; private set; }

        [Tooltip("The name of the local player")]
        public string PlayerName = "Player";

        [Tooltip("Indicates if the DolbyIOManager should automatically open the session on awaking.")]
        public bool AutoOpenSession = true;

        [Tooltip("Indicates if the DolbyIOManager should automatically close the session on application quit.")]
        public bool AutoCloseSession = true;

        [Tooltip("Indicates if the DolbyIOManager should automatically leave the current conference on application quit.")]
        public bool AutoLeaveConference = true;
        
        [Tooltip("The elapsed time between two call to set position in s.")]
        public float PositionDuration = 0.3f;

        [Tooltip("The elapsed time between two call to set direction in s.")]
        public float DirectionDuration = 0.05f;

        public AudioListener AudioListener;


        /// <summary>
        /// For convenience during early development and prototyping, a method is provided for you to 
        /// acquire a client access token directly from the application. However, please note Dolby does not recommend 
        /// using this mechanism in the production software for (security best practices)[https://docs.dolby.io/communications-apis/docs/guides-client-authentication] reasons.
        /// </summary>
        /// <param name="key">The App key.</param>
        /// <param name="secret">The App Secret.</param>
        /// <returns>An asynchronous task containing the token.</returns>
        public static async Task<string> GetToken(string key, string secret)
        {
            return await Task.Run(async () =>
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, "https://session.voxeet.com/v1/oauth2/token");
                var auth = $"{Uri.EscapeUriString(key)}:{Uri.EscapeUriString(secret)}";

                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", $"{Convert.ToBase64String(Encoding.UTF8.GetBytes(auth))}");
                request.Content = new FormUrlEncodedContent(new Dictionary<string, string> { { "grant_type", "client_credentials" } });

                using var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);

                string accessToken = jsonString;
                if (!json.TryGetValue("access_token", out accessToken))
                {
                    throw new Exception("Unable to read the json token");
                }
                
                return accessToken;
            })
            .ConfigureAwait(false);
        }

        public static void QueueOnMainThread(Action a)
        {
            lock(_backlog)
            {
                _backlog.Add(a);
            }
        }

        public static void ClearQueue()
        {
            lock(_backlog)
            {
                _backlog.Clear();
            }
        }

        private void OnEnable()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Awake()
        {
            var component = GetComponent<Credentials>();
            if (!component)
            {
                Debug.LogError($"Unable to find component {nameof(Credentials)}, please add one.");
            }
            else
            {
                _sdk.InitAsync(component.GetToken(), component.GetToken).Wait();

                if (AutoOpenSession)
                {
                    OpenSession();
                }
            }
        }

        void Start()
        {
            if (!AudioListener)
            {
                AudioListener = FindObjectOfType<AudioListener>();
            }
        }

        void Update()
        {
            lock(_backlog)
            {
                foreach(var action in _backlog)
                {
                    try
                    {
                        action();
                    }
                    catch(Exception e)
                    {
                        Debug.Log(e);
                    }                    
                }
                _backlog.Clear();
            }

            if (_sdk.IsInitialized && _sdk.Session.IsOpen && _sdk.Conference.IsInConference)
            {
                UpdatePosition();
                UpdateDirection();
            }
        }

        public void OpenSession()
        {
            UserInfo user = new UserInfo();
            user.Name = PlayerName;

            if (!_sdk.Session.IsOpen)
            {
                _sdk.Session.OpenAsync(user).Wait();
            }
        }

        public void CloseSession()
        {
            if (!_sdk.Session.IsOpen)
            {
                _sdk.Session.CloseAsync().Wait();
            }
        }

        private void UpdatePosition()
        {
            if (AudioListener)
            {
                _timePosition += Time.deltaTime;

                if (_timePosition >= PositionDuration)
                {
                    _timePosition = 0.0f;

                    var position = AudioListener.gameObject.transform.position;
                    _sdk.Conference.SetSpatialPositionAsync
                    (
                        _sdk.Session.User.Id,
                        new System.Numerics.Vector3(position.x, position.y, position.z)
                    )
                    .ContinueWith(t =>
                    {
                        Debug.LogError(t.Exception);
                    }, TaskContinuationOptions.OnlyOnFaulted);
                }
            }
        }

        private void UpdateDirection()
        {
            if (AudioListener)
            {
                _timeDirection += Time.deltaTime;

                if (_timeDirection >= DirectionDuration)
                {
                    _timeDirection = 0.0f;

                    var direction = AudioListener.gameObject.transform.rotation.eulerAngles;
                    _sdk.Conference.SetSpatialDirectionAsync
                    (
                        new System.Numerics.Vector3(direction.x, direction.y, direction.z)
                    )
                    .ContinueWith(t =>
                    {
                        Debug.LogError(t.Exception);
                    }, TaskContinuationOptions.OnlyOnFaulted);
                }
            }
        }

        async void OnApplicationQuit()
        {
            try
            {
                if (_sdk.IsInitialized)
                {
                    if (AutoLeaveConference && _sdk.Conference.IsInConference)
                    {
                        await _sdk.Conference.LeaveAsync();
                    }

                    if (AutoCloseSession && _sdk.Session.IsOpen)
                    {
                        await _sdk.Session.CloseAsync();
                    }
                }
            }
            catch (DolbyIOException e)
            {
                Debug.LogError(e);
            }
            finally
            {
                DolbyIOManager.ClearQueue();
                _sdk.Dispose();
            }
        }
    }
}

