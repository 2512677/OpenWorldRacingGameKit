using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RGSK
{
    public class RacingLineMesh : MonoBehaviour
    {
        public RacingLine racingLine;
        public GameObject meshPrefab;
        public float spacing = 1;
        public float groundOffset = 0.02f;

        public void GenerateRaceLine()
        {
            if (racingLine == null || meshPrefab == null) return;

            GameObject raceLineMesh = new GameObject("RaceLineMesh");
            raceLineMesh.transform.parent = transform;
            RaycastHit hit;

            for (float i = 0; i < (int)racingLine.length; i += spacing)
            {
                GameObject line = Instantiate(meshPrefab);
                Vector3 position = racingLine.GetRoutePoint(i).position;
                Quaternion rotation = Quaternion.LookRotation(racingLine.GetRoutePoint(i).direction);

                line.transform.position = position;
                line.transform.rotation = rotation;
                line.transform.parent = raceLineMesh.transform;

                if (Physics.Raycast(new Ray(line.transform.position, -line.transform.up), out hit))
                {
                    line.transform.position = hit.point + new Vector3(0, groundOffset, 0);
                }
            }
        }


        public void DeleteRaceLine()
        {
            if(transform.Find("RaceLineMesh"))
            {
                DestroyImmediate(transform.Find("RaceLineMesh").gameObject);
            }
        }


        public void CombineMeshes()
        {
            MeshFilter[] meshFilters = transform.Find("RaceLineMesh").GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            while (i < meshFilters.Length)
            {
                if (meshFilters[i] != null)
                {
                    combine[i].mesh = meshFilters[i].sharedMesh;
                    combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                    meshFilters[i].gameObject.SetActive(false);
                    i++;
                }
            }

            MeshFilter racingLineMeshCombined = new GameObject("Racing Line Mesh").AddComponent<MeshFilter>();
            racingLineMeshCombined.mesh = new Mesh();
            racingLineMeshCombined.mesh.CombineMeshes(combine);
        }
    }
}