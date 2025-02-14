using UnityEngine;
using System.Collections;

public class TrackNode : MonoBehaviour
{
    public float leftWidth = 5;
    public float rightWidth = 5;
    public float distanceAtNode { get; set; }

    void OnDrawGizmos()
    {
        TrackLayout parent = GetComponentInParent<TrackLayout>();
        if (parent != null)
        {
            Gizmos.color = parent.color;

            if (parent.showSegments)
            {
                Gizmos.DrawLine(transform.position, transform.position + (transform.right * rightWidth));
                Gizmos.DrawLine(transform.position, transform.position + (-transform.right * leftWidth));

                Gizmos.DrawWireSphere(transform.position + (transform.right * rightWidth), 0.25f);
                Gizmos.DrawWireSphere(transform.position + (-transform.right * leftWidth), 0.25f);
            }
        }
    }
}
