using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class RacingLine : TrackSpline
    {
        private RacingLineNode[] racingLineNodes;

        //Target projection
        public float minTargetDistance = 10;
        public float maxTargetDistance = 50;

        //Automatic node speed values
        public float minSpeed = 50;
        public float maxSpeed = 100;
        public float cautionAngle = 50;


        void Start()
        {
            racingLineNodes = GetRaceLineNodes().ToArray();
        }


        public float GetSpeedAtNode(int index)
        {
            return racingLineNodes[index].targetSpeed;
        }


        public void CalculateNodeSpeeds()
        {
            AdjustNodeRotation();

            racingLineNodes = GetRaceLineNodes().ToArray();

            for (int i = 0; i < racingLineNodes.Length; i++)
            {
                if(i > 0)
                {
                    //Compare the angle between the previous to the next node 
                    Vector3 direction = racingLineNodes[i].transform.position 
                                        - racingLineNodes[i - 1].transform.position;

                    float angle = Vector3.Angle(direction, racingLineNodes[i].transform.forward);
                    float ratio = Mathf.InverseLerp(0, cautionAngle, angle);
                    float nodeSpeed = Mathf.Lerp(maxSpeed, minSpeed, ratio);
                    racingLineNodes[i - 1].targetSpeed = nodeSpeed;
                }
            }

            racingLineNodes[racingLineNodes.Length - 1].targetSpeed = racingLineNodes[racingLineNodes.Length - 2].targetSpeed;
        }


        public override void DrawGizmos()
        {
            base.DrawGizmos();

            //Visualize the first node
            if (nodes.Count == 1)
            {
                Gizmos.color = color;
                Gizmos.DrawWireSphere(transform.GetChild(0).position, 0.5f);
            }
        }
    }
}
