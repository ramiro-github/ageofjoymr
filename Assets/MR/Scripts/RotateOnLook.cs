using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVRTouchSample;

public class LookAtCamera : MonoBehaviour
{
   public float rotationSpeed = 5f; // A velocidade de rotação

   void Update()
   {
       // Verifique se o botão analógico direito foi pressionado
       //if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
       //{
           // Obtenha todos os objetos cujo nome começa com "cabinet"
           GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
           List<GameObject> cabinets = new List<GameObject>();
           foreach (GameObject obj in allObjects)
           {
               if (obj.name.StartsWith("cabinet"))
               {
                  cabinets.Add(obj);
               }
           }

           foreach (GameObject cabinet in cabinets)
           {
               // Verifique se a câmera está olhando para este objeto
               if (Vector3.Angle(transform.forward, cabinet.transform.position - transform.position) < 90f)
               {
                  // Se sim, faça este objeto olhar para a câmera
                  Quaternion targetRotation = Quaternion.LookRotation(transform.position - cabinet.transform.position);
                  cabinet.transform.rotation = Quaternion.Slerp(cabinet.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
               }
           }
       //}
   }
}