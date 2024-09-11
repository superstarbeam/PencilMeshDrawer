using System;
using MeshPencil.Renderers.VoxelRenderer;
using UnityEngine;

namespace MeshPencil.Collider
{
    public class CubeVoxelColliderCreator : ColliderCreator
    {
        [SerializeField] private GameObject _singleColliderCubePrefab;

        private Action<GameObject> _onCompleteCallback;

        public override void CreateCollider(VoxelData data, float depth, float boxSize,PhysicMaterial physicMaterial = null, Action<GameObject> onCompleteCallback = null)
        {
            _onCompleteCallback = onCompleteCallback;

            for (int y = 0; y < data.Width; y++)
            {   
                //Continue extend current box collider width
                bool isBlockContinue = false;

                //Current box collider width extend times
                int currentBlockLength = 0;

                //Current extending box collider 
                BoxCollider currentBoxCollider = null;

                for (int x = 0; x < data.Depth; x++)
                {   
                    //Cell is not painted , block is not exist in x,y point
                    if (data.GetCell(y, x) == 0)
                    {
                        currentBlockLength = 0;

                        if (currentBoxCollider != null)
                        {
                            float xCenter = (currentBoxCollider.size.x - boxSize) / 2;
                            currentBoxCollider.center = new Vector3(xCenter, 0, 0);
                        }

                        currentBoxCollider = null;

                        isBlockContinue = false;
                        continue;
                    }

                    //Cell is exist after empty existed cell
                    //Creating new collider block
                    if (!isBlockContinue)
                    {
                        isBlockContinue = true;

                        GameObject boxColliderCube = Instantiate(_singleColliderCubePrefab);

                        boxColliderCube.transform.parent = gameObject.transform;
                        boxColliderCube.transform.position = new Vector3(x * boxSize, y * boxSize, 0);

                        currentBoxCollider = boxColliderCube.GetComponent<BoxCollider>();

                        if(physicMaterial !=null)
                        currentBoxCollider.material = physicMaterial;
                    }

                    //Extend current block width
                    currentBlockLength++;

                    if (currentBoxCollider != null)
                    {
                        currentBoxCollider.size = new Vector3(currentBlockLength * boxSize, boxSize, depth);
                    }

                    //Finish current block if is last cell in row
                    if (isBlockContinue && x == data.Depth - 1)
                    {
                        float xCenter = (currentBoxCollider.size.x - boxSize) / 2;
                        currentBoxCollider.center = new Vector3(xCenter, 0, 0);

                        currentBoxCollider = null;

                        isBlockContinue = false;
                        continue;
                    }
                }
            }

            _onCompleteCallback?.Invoke(gameObject);
            _onCompleteCallback = null;
        }
    }
}
