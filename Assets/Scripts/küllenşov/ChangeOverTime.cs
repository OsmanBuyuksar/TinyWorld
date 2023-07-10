using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace osman
{
    public class ChangeOverTime : MonoBehaviour
    {
        [SerializeField] Cinemachine.CinemachineVirtualCamera camera1;
        [SerializeField] Cinemachine.CinemachineVirtualCamera camera2;

        [SerializeField] MapGenerator mapGen;

        [SerializeField] float cameraTime;
        [SerializeField] float heightChangeSpeed;
        [SerializeField] float heightMax = 16f;


        private float time;
        void Start()
        {
            time = Time.time;
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time - time > cameraTime)
                camera2.gameObject.SetActive(true);

            if (Time.time - time > cameraTime)
            {
                if (Time.time - time > 2 * cameraTime && Time.time - time < 3 * cameraTime)
                {
                    mapGen.drawMode = DrawMode.ColoredMesh;
                    mapGen.GenerateMap();
                    mapGen.IncrementHeight(heightChangeSpeed * Time.deltaTime);
                }
                else if (Time.time - time < 2 * cameraTime)
                {
                    mapGen.IncrementHeight(-heightChangeSpeed * Time.deltaTime);
                }
                else if (Time.time - time > 3 && Time.time - time < 4 * cameraTime)
                {
                    mapGen.IncrementHeight(-heightChangeSpeed * Time.deltaTime);


                }
                else if (Time.time - time > 4 && Time.time - time < 5 * cameraTime)
                {
                    mapGen.drawMode = DrawMode.WaveFunctionMesh;
                    mapGen.IncrementHeight(heightChangeSpeed * Time.deltaTime);
                    mapGen.GenerateMap();
                    //gameObject.GetComponent<ChangeOverTime>().enabled = false;
                }
                else if (Time.time - time > 5 && Time.time - time < 6 * cameraTime)
                {
                    mapGen.drawMode = DrawMode.Combined;
                    mapGen.IncrementHeight(16);
                    mapGen.GenerateMap();
                    gameObject.GetComponent<ChangeOverTime>().enabled = false;
                }
            }
            else
                mapGen.IncrementHeight(heightChangeSpeed * Time.deltaTime);
        }
    }
}
