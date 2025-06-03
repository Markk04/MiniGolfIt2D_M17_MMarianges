using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform platform;    
    public Transform startPoint;  
    public Transform endPoint;    
    public float speed = 2f;      

    int direction = 1; // 1 per moure's cap al endPoint, -1 per moure's cap al startPoint   

    private void Update()
    {
        // Calcula la posici� objectiu actual
        Vector2 targetPosition = currentMovementTarget();

        // Mou la plataforma cap a la posici� objectiu
        platform.position = Vector2.MoveTowards(platform.position, targetPosition, speed * Time.deltaTime);

        // Calcula la dist�ncia fins a l'objectiu
        float distance = (targetPosition - (Vector2)platform.position).magnitude;

        // Si est� a prop de l'objectiu, canvia de direcci�
        if (distance < 0.1f)
        {
            direction *= -1;
        }
    }

    // Retorna la posici� objectiu segons la direcci� actual
    Vector2 currentMovementTarget()
    {
        if (direction == 1)
        {
            return startPoint.position;
        }
        else
        {
            return endPoint.position;
        }
    }

    // Dibuixa l�nies a l'editor per visualitzar el recorregut
    private void OnDrawGizmos()
    {
        if (platform != null && startPoint != null && endPoint != null)
        {
            Gizmos.DrawLine(platform.transform.position, startPoint.transform.position);
            Gizmos.DrawLine(platform.transform.position, endPoint.transform.position);
        }
    }
}