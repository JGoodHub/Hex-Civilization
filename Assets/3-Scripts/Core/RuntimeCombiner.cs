using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MellowMadness.Core
{

    public class RuntimeCombiner : MonoBehaviour
    {

        public bool combineOnStart = true;
        public Material sharedMaterial;

        //flags
        public bool useInt32Buffers;
        public bool destroyChildMeshes;
        public bool destroyAllChildren;

        // Start is called before the first frame update
        void Start()
        {
            if (combineOnStart)
                Combine();
        }

        public void UseFirstMaterial()
        {
            MeshRenderer meshRen = GetComponentInChildren<MeshRenderer>();
            if (meshRen != null)
                sharedMaterial = meshRen.sharedMaterial;
        }

        public void Combine()
        {
            Vector3 worldPosition = gameObject.transform.position;
            gameObject.transform.position = Vector3.zero;

            MeshFilter[] meshFilters = gameObject.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combineInstances = new CombineInstance[meshFilters.Length];

            for (int i = 0; i < meshFilters.Length; i++)
            {
                combineInstances[i].mesh = meshFilters[i].sharedMesh;
                combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
            }

            MeshFilter targetMeshFilter;
            if (gameObject.TryGetComponent(out targetMeshFilter) == false)
                targetMeshFilter = gameObject.AddComponent<MeshFilter>();

            targetMeshFilter.mesh = new Mesh();
            targetMeshFilter.mesh.indexFormat = useInt32Buffers ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16;
            targetMeshFilter.mesh.CombineMeshes(combineInstances, true);

            targetMeshFilter.mesh.Optimize();
            targetMeshFilter.mesh.RecalculateBounds();
            targetMeshFilter.mesh.RecalculateNormals();

            MeshRenderer targetMeshRen;
            if (gameObject.TryGetComponent(out targetMeshRen) == false)
                targetMeshRen = gameObject.AddComponent<MeshRenderer>();

            targetMeshRen.sharedMaterial = sharedMaterial;

            if (destroyChildMeshes)
            {
                for (int i = 0; i < meshFilters.Length; i++)
                    if (meshFilters[i].gameObject != gameObject)
                        Destroy(meshFilters[i].gameObject);
            }

            if (destroyAllChildren)
            {
                Transform[] childTransforms = gameObject.GetComponentsInChildren<Transform>();

                for (int i = 0; i < childTransforms.Length; i++)
                    if (childTransforms[i].gameObject != gameObject)
                        Destroy(childTransforms[i].gameObject);
            }

            gameObject.transform.position = worldPosition;
            gameObject.transform.rotation = Quaternion.identity;
            gameObject.transform.localScale = Vector3.one;
        }

    }

}