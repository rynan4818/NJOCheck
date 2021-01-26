﻿using BeatSaberMarkupLanguage;
using HMUI;
using IPA.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUIControls;
using Zenject;

namespace NJOCheck
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
    public class NJOCheckController : MonoBehaviour
    {
        // These methods are automatically called by Unity, you should remove any you aren't using.
        [Inject]
        void Constractor(DiContainer container)
        {
            this.gameplaySetupViewController = container.TryResolve<GameplaySetupViewController>();
            this.playerSettingsPanelController = this.gameplaySetupViewController.GetField<PlayerSettingsPanelController, GameplaySetupViewController>("_playerSettingsPanelController");
            Plugin.Log.Debug($"{this.playerSettingsPanelController}");
        }

        #region Monobehaviour Messages
        /// <summary>
        /// Only ever called once, mainly used to initialize variables.
        /// </summary>
        private void Awake()
        {
            // For this particular MonoBehaviour, we only want one instance to exist at any time, so store a reference to it in a static property
            //   and destroy any that are created while one already exists.
            Plugin.Log?.Debug($"{name}: Awake()");
        }
        /// <summary>
        /// Only ever called once on the first frame the script is Enabled. Start is called after any other script's Awake() and before Update().
        /// </summary>
        private void Start()
        {
            try {
                this.screen = new GameObject("NotificationText", typeof(CanvasScaler), typeof(RectMask2D), typeof(VRGraphicRaycaster), typeof(CurvedCanvasSettings));
                this.screen.GetComponent<VRGraphicRaycaster>().SetField("_physicsRaycaster", BeatSaberUI.PhysicsRaycasterWithCache);
                this.screen.transform.localScale = new Vector3(1f, 1f, 1f);
                this.notificationText = BeatSaberUI.CreateText(screen.gameObject.transform as RectTransform, "DEFAULT", Vector2.zero);
                this.notificationText.alignment = TextAlignmentOptions.Center;
                this.notificationText.autoSizeTextContainer = false;
                this.noteJumpStartBeatOffsetDropdown = this.playerSettingsPanelController.GetField<NoteJumpStartBeatOffsetDropdown, PlayerSettingsPanelController>("_noteJumpStartBeatOffsetDropdown");
                this.noteJumpStartBeatOffsetDropdown.didSelectCellWithIdxEvent += this.NoteJumpStartBeatOffsetDropdown_didSelectCellWithIdxEvent;
                this.NoteJumpStartBeatOffsetDropdown_didSelectCellWithIdxEvent(0);
            }
            catch (Exception e) {
                Plugin.Log.Error(e);
            }
        }
        /// <summary>
        /// Called when the script is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Plugin.Log?.Debug($"{name}: OnDestroy()");
            Destroy(this.notificationText.gameObject);
        }
        #endregion

        private void NoteJumpStartBeatOffsetDropdown_didSelectCellWithIdxEvent(int obj)
        {
            if (textParameters.Length < (uint)obj) {
                return;
            }

            notificationText.text = this.textParameters[obj].Text;
            this.screen.transform.localScale = this.textParameters[obj].Scale;
            this.screen.transform.localPosition = this.textParameters[obj].Position;
            notificationText.color = this.textParameters[obj].TextColor;
        }

        GameObject screen;
        GameplaySetupViewController gameplaySetupViewController;
        PlayerSettingsPanelController playerSettingsPanelController;
        NoteJumpStartBeatOffsetDropdown noteJumpStartBeatOffsetDropdown;
        TextMeshProUGUI notificationText;

        TextParameter[] textParameters = new TextParameter[5]
        {
            new TextParameter()
            {
                Text = "CLOSE",
                Scale = new Vector3(0.3f, 0.3f, 0.3f),
                Position = new Vector3(0f, 1.5f, 10f),
                TextColor = Color.red
            },
            new TextParameter()
            {
                Text = "CLOSER",
                Scale = new Vector3(0.7f, 0.7f, 0.7f),
                Position = new Vector3(0f, 1.5f, 20f),
                TextColor = Color.yellow
            },
            new TextParameter()
            {
                Text = "DEFAULT",
                Scale = new Vector3(1f, 1f, 1f),
                Position = new Vector3(0f, 1.5f, 30f),
                TextColor = Color.white
            },
            new TextParameter()
            {
                Text = "FURTHER",
                Scale = new Vector3(1.5f, 1.5f, 1.5f),
                Position = new Vector3(0f, 3f, 40f),
                TextColor = Color.blue
            },
            new TextParameter()
            {
                Text = "FAR",
                Scale = new Vector3(3f, 3f, 3f),
                Position = new Vector3(0f, 4f, 50f),
                TextColor = Color.green
            }
        };
        public struct TextParameter
        {
            public string Text;
            public Vector3 Scale;
            public Vector3 Position;
            public Color TextColor;
        }
    }
}