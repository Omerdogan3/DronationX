using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace PA_DronePack
{
    public class DroneSelection : MonoBehaviour
    {
        public List<GameObject> selections;
        public Text selectionLabel;
        static int droneIndex = 0;
        static int skinIndex = 0;
        public bool turntable = false;

        void Awake()
        {
            Application.targetFrameRate = 100;
            UpdateDroneSelection();
        }

        void Update()
        {
            if (turntable && Input.GetMouseButton(0))
            {
                foreach (GameObject drone in selections)
                {
                    drone.transform.rotation = Quaternion.Euler(drone.transform.rotation.eulerAngles + new Vector3(0f, 10f * -Input.GetAxis("Mouse X"), 0f));
                }
            }
        }

        public void UpdateDroneSelection()
        {
            if (selections.Count > 0)
            {
                for (int i = 0; i < selections.Count; i++)
                {
                    selections[i].SetActive(false);
                }

                for (int j = 0; j < selections[droneIndex].transform.childCount; j++)
                {
                    selections[droneIndex].transform.GetChild(j).gameObject.SetActive(false);
                }

                selections[droneIndex].transform.GetChild(skinIndex).gameObject.SetActive(true);
                selections[droneIndex].SetActive(true);

                if (selectionLabel) { selectionLabel.text = selections[droneIndex].name; }
            }
        }

        public void NextDrone()
        {
            if (droneIndex < selections.Count - 1) { droneIndex += 1; } else { droneIndex = 0; }
            UpdateDroneSelection();
        }

        public void PrevDrone()
        {
            if (droneIndex > 0) { droneIndex -= 1; } else { droneIndex = selections[droneIndex].transform.childCount - 1; }
            UpdateDroneSelection();
        }

        public void NextSkin()
        {
            if (skinIndex < selections[droneIndex].transform.childCount - 1) { skinIndex += 1; } else { skinIndex = 0; }
            UpdateDroneSelection();
        }

        public void PrevSkin()
        {
            if (skinIndex < 0) { skinIndex += 1; } else { skinIndex = selections[droneIndex].transform.childCount - 1; }
            UpdateDroneSelection();
        }

        public void LoadLevel(string levelName)
        {
            SceneManager.LoadScene(levelName);
        }
    }
}
