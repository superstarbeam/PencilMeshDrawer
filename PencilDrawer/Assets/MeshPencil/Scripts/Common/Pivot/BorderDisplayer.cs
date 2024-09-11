using UnityEngine;
using Random = UnityEngine.Random;

namespace MeshPencil.Common.Pivot
{
   public class BorderDisplayer : MonoBehaviour
   {   
      [Tooltip("Pivot object will be spawned as a parent to cteated mesh\n" +
               "Could be empty")]
      [SerializeField] private GameObject _displayBorderPrefab;
      
      /// <summary>
      /// Create a pivot GameObject and set result mesh as a child
      /// </summary>
      /// <param name="data"></param>
      /// <returns>Created pivot parent</returns>
      public Transform SetPivot(BorderDisplayerData data)
      {
         var pivotObject = new GameObject();

         if (_displayBorderPrefab!= null)
         {
            pivotObject = Instantiate(_displayBorderPrefab);
         }
         else
         {
            pivotObject.name = "Created Mesh";
         }

         pivotObject.transform.position = data.SpawnedMesh.transform.position;
         pivotObject.transform.rotation = data.SpawnedMesh.transform.rotation;
         
         ApplyPivotPosition(pivotObject,data);
         
         data.SpawnedMesh.transform.parent = pivotObject.transform;

         return pivotObject.transform;
      }

      private void ApplyPivotPosition(GameObject pivotObject,BorderDisplayerData data)
      {
         Vector3 pivotOffsetedPosition = GetOffsetedPosition(data);

         if (data.PivotPosition == PivotPosition.StartDrawPoint || 
             data.PivotPosition == PivotPosition.FinishDrawPoint)
         {
            var targetPoint = Vector3.zero;
            
            switch (data.PivotPosition)
            {
               case PivotPosition.StartDrawPoint:
                  targetPoint = data.PivotsData.StartDrawPoint;
                  break;
               case PivotPosition.FinishDrawPoint:
                  targetPoint = data.PivotsData.FinishDrawPoint;
                  break;
            }
            
            pivotObject.transform.position = new Vector3(targetPoint.x,targetPoint.y,pivotObject.transform.position.z);
         }
         else
         {
            pivotObject.transform.position += pivotOffsetedPosition;
         }
      }

      private Vector3 GetOffsetedPosition(BorderDisplayerData data)
      {
         float centerOffset = data.PixelScale / 2;

         int horizontalIndex = 0;
         int verticalIndex = 0;

         Vector2 randomTouchingArrayElement;
         
         switch (data.PivotPosition)
         {
            case PivotPosition.UpperLeft:
               horizontalIndex = data.PivotsData.LeftestIndex;
               verticalIndex = data.PivotsData.UpestIndex;
               break;
            case PivotPosition.UpperRight:
               horizontalIndex = data.PivotsData.RightestIndex;
               verticalIndex = data.PivotsData.UpestIndex;
               break;
            case PivotPosition.LowerLeft:
               horizontalIndex = data.PivotsData.LeftestIndex;
               verticalIndex = data.PivotsData.DownestIndex;
               break;
            case PivotPosition.LowerRight:
               horizontalIndex = data.PivotsData.RightestIndex;
               verticalIndex = data.PivotsData.DownestIndex;
               break;
            //Touching
            case PivotPosition.TouchingLeft:
               randomTouchingArrayElement = data.PivotsData.LeftestTouchIndex[Random.Range(0, data.PivotsData.LeftestTouchIndex.Count)];
               horizontalIndex = (int)randomTouchingArrayElement.x;
               verticalIndex = (int)randomTouchingArrayElement.y;
               break;
            case PivotPosition.TouchingRight:
               randomTouchingArrayElement = data.PivotsData.RightestTouchIndex[Random.Range(0, data.PivotsData.RightestTouchIndex.Count)];
               horizontalIndex = (int)randomTouchingArrayElement.x;
               verticalIndex = (int)randomTouchingArrayElement.y;
               break;
            case PivotPosition.TouchingUp:
               randomTouchingArrayElement = data.PivotsData.UpestTouchIndex[Random.Range(0, data.PivotsData.UpestTouchIndex.Count)];
               horizontalIndex = (int)randomTouchingArrayElement.x;
               verticalIndex = (int)randomTouchingArrayElement.y;
               break;
            case PivotPosition.TouchingDown:
               randomTouchingArrayElement = data.PivotsData.DownestTouchIndex[Random.Range(0, data.PivotsData.DownestTouchIndex.Count)];
               horizontalIndex = (int)randomTouchingArrayElement.x;
               verticalIndex = (int)randomTouchingArrayElement.y;
               break;
         }

         Vector3 offsetedPosition;
         
         offsetedPosition = new Vector3((horizontalIndex * data.PixelScale) - centerOffset,
            (verticalIndex * data.PixelScale) - centerOffset, 0); 
         
         return offsetedPosition;
      }
   }
}
