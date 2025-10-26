using Unity.VisualScripting;
using UnityEngine;

public class NPC : MonoBehaviour
{

    private CameraTrocar Camera_Trocar_Objeto = null;

        void Start()
    {
        Camera_Trocar_Objeto = GameObject.Find("Camera").GetComponent<CameraTrocar>();
    }


    void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "Player" )
        {
            if(Camera_Trocar_Objeto)
            {
                Camera_Trocar_Objeto.TrocarCamera(1);
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
         if(collider.tag == "Player" )
        {
            if(Camera_Trocar_Objeto)
            {
                Camera_Trocar_Objeto.TrocarCamera(0);
            }
        }
    }
}
