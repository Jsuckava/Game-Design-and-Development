using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor.U2D.Animation;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.HID.HID;

namespace Assets.Scripts
{
    [System.Serializable]
    public class CharacterSelection : MonoBehaviour
    {
        public List<CharacterData> characters;
        public Image characterImage;
        public TextMeshProUGUI characterNameText;
        public Button NextButton;
        public Button PrevButton;
        public Button SelectButton;
        private int currentIndex = 0;

        public void ExitButton() {

        }
        public void ConfirmSelection() { 
        
        }
        
        }
    }
