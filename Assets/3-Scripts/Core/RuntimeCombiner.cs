using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MellowMadness.Core
{

    public class RuntimeCombiner : MonoBehaviour
    {

        public bool combineOnStart = true;
        public Material sharedMaterial;
        public bool useInt32Buffers;

        // Start is called before the first frame update
        void Start()
        {
            if (combineOnStart)
            {
                CombineChildren(gameObject, sharedMaterial, useInt32Buffers);
            }
        }

        public void CombineChildren()
        {
            CombineChildren(gameObject, sharedMaterial, useInt32Buffers);
        }

        public void UseFirstMaterial()
        {
            MeshRenderer meshRen = GetComponentInChildren<MeshRenderer>();
            if (meshRen != null)
                sharedMaterial = meshRen.sharedMaterial;
        }

        public static void CombineChildren(GameObject obj, Material sharedMaterial, bool useInt32Buffers)
        {
            Vector3 worldPosition = obj.transform.position;
            obj.transform.position = Vector3.zero;

            MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];

            for (int i = 0; i < meshFilters.Length; i++)
            {
                combineInstances[i].mesh = meshFilters[i].sharedMesh;
                combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
            }

            MeshFilter targetMeshFilter;
            if (obj.TryGetComponent(out targetMeshFilter) == false)
                targetMeshFilter = obj.AddComponent<MeshFilter>();

            targetMeshFilter.mesh = new Mesh();
            targetMeshFilter.mesh.indexFormat = useInt32Buffers ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16;
            targetMeshFilter.mesh.CombineMeshes(combineInstances, true);

            targetMeshFilter.mesh.Optimize();
            targetMeshFilter.mesh.RecalculateBounds();
            targetMeshFilter.mesh.RecalculateNormals();

            MeshRenderer targetMeshRen;
            if (obj.TryGetComponent(out targetMeshRen) == false)
                targetMeshRen = obj.AddComponent<MeshRenderer>();

            targetMeshRen.sharedMaterial = sharedMaterial;

            for (int i = 0; i < meshFilters.Length; i++)
            {
                if (meshFilters[i].gameObject != obj)
                {
                    Destroy(meshFilters[i].gameObject);
                }
            }

            obj.transform.position = worldPosition;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
        }

    }

}