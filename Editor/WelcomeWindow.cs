using UnityEngine;
using UnityEditor;
using UnityEditor.VSAttribution.DolbyIO;
using System.Text;
using System.Threading;
using UnityEditor.PackageManager;

namespace DolbyIO.Comms.Unity.Editor
{
    public class WelcomeWindow : EditorWindow
    {
        public string CustomerKey;

        private const string _actionName = "Welcome";
        private Texture2D _logoTex;

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            var listRequest = Client.List(true);
            while (!listRequest.IsCompleted)
                Thread.Sleep(100);

            if (listRequest.Error != null)
            {
                Debug.Log("Error: " + listRequest.Error.message);
                return;
            }

            var packages = listRequest.Result;
            var text = new StringBuilder("Packages:\n");
            foreach (var package in packages)
            {
                if (package.name == "dolbyio.comms.unity" && package.source == PackageSource.Registry)
                {
                    WelcomeWindow.Initialize();
                }
            }

        }

        public static void Initialize()
        {
            GetWindow<WelcomeWindow>("Welcome!");
        }

        void OnEnable()
        {
            _logoTex = (Texture2D) AssetDatabase.LoadAssetAtPath("Packages/dolbyio.comms.unity/Editor/Resources/dolby.io-logo.png", typeof(Texture2D));
            if (!_logoTex)
            {
                Debug.Log("Failed to laod logo texture.");
            }
        }

        void OnGUI()
        {
            //DrawLine(Color.gray);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(_logoTex, GUILayout.Width(256), GUILayout.Height(73));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            DrawHelpBox("<b><size=16>Welcome to the Dolby.io Virtual Worlds plugin</size></b>", 2);

            GUILayout.Space(40);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Insert your App Key to get started:");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            CustomerKey = EditorGUILayout.TextField("", CustomerKey, GUILayout.Width(256));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("How to get an App Key", GUILayout.Height(30)))
            {
                Application.OpenURL("https://docs.dolby.io/communications-apis/docs/guides-app-credentials");
            }

            GUILayout.Space(60);

            if (GUILayout.Button("Begin", GUILayout.Width(50), GUILayout.Height(30)))
            {
                var result = VSAttribution.SendAttributionEvent(_actionName, CustomerKey);

                if (!string.IsNullOrEmpty(CustomerKey))
                {
                    if (result != UnityEngine.Analytics.AnalyticsResult.Ok)
                    {
                        Debug.LogError($"Failed to send attribution event with: {result}");
                    }

                    Close();
                }
                else
                {
                    Debug.LogError("App Key can't be empty.");
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(40);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Getting Started", GUILayout.Height(30)))
            {
                Application.OpenURL("https://api-references.dolby.io/comms-sdk-dotnet/documentation/unity/getting-started/example.html");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Visual Scripting", GUILayout.Height(30)))
            {
                Application.OpenURL("https://api-references.dolby.io/comms-sdk-dotnet/documentation/unity/visualscripting/nodes.html");
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        #region Utilities
        private void DrawHelpBox(string text, int linesCount)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label) { richText = true };
            style.alignment = TextAnchor.MiddleCenter;

            EditorGUILayout.LabelField(text, style, GUILayout.Height(linesCount * 14f));
        }

        private void DrawLine(Color color, int thickness = 2, int padding = 7)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding * 2 + thickness));
            r.height = thickness;
            r.y += padding;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
        #endregion
    }
}