using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WorldBoxModLoader
{
    [RequireComponent(typeof(Button))]
    internal class ModPowerButton : MonoBehaviour
    {
        public ModConstants ModConstants;
        public UnityAction<ModConstants> onClick;

        private void OnEnable()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }
        private void OnClick()
        {
            onClick.Invoke(ModConstants);
        }
        private void OnDisable()
        {
            GetComponent<Button>().onClick.RemoveListener(OnClick);
        }
    }
}
