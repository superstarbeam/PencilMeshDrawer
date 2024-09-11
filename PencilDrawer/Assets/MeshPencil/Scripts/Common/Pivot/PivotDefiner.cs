using System;
using System.Collections.Generic;
using UnityEngine;

namespace MeshPencil.Common.Pivot
{    
    //Follow naming order rules for all classes works with pivots !
    //1 = Left,2 = Right,3 = Up,4 = Down  <== Use this order in lists , vectors enums and tuples
    
    public static class PivotDefiner 
    {
        public static PivotsData DefineBorderPoints(byte[,] data)
        {    
            int rowsCount = data.GetLength(1);
            int columnsAmount = data.GetLength(0);
            
            //Leftest
            int leftestPosition = rowsCount;
            //Rightest
            int rightestPosition = 0;
            
            //Upperest
            int upestPosition = 0;
            //Downest
            int downestPosition = rowsCount;


            for (int column = columnsAmount-1; column >= 0; column--)
            {
                for (int row = 0; row < rowsCount; row++)
                {
                    if (data[column, row] == 1)
                    {    
                        //Check horizontal borders point
                        if (row < leftestPosition)
                        {
                            leftestPosition = row;
                        }     
                        if (row > rightestPosition)
                        {
                            rightestPosition = row;
                        }
                        
                        //Check vertical borders point
                        if (column < downestPosition)
                        {
                            downestPosition = column;
                        }     
                        if (column > upestPosition)
                        {
                            upestPosition = column;
                        }
                    }
                }
            }
            
            var lastIndexes = new Vector4(leftestPosition,rightestPosition,upestPosition,downestPosition);

            (List<Vector2> leftestTouchIndex,
                List<Vector2> rightestTouchIndex,
                List<Vector2> upestTouchIndex,
                List<Vector2> downestTouchIndex) 
                = GetHorizontalTouchingIndices(data, lastIndexes);
            
            //Correct offset
            rightestPosition++;
            upestPosition++;
            
            PivotsData resultData = new PivotsData
            {
                LeftestIndex = leftestPosition,
                RightestIndex = rightestPosition,
                UpestIndex = upestPosition,
                DownestIndex = downestPosition,
                LeftestTouchIndex = leftestTouchIndex,
                RightestTouchIndex = rightestTouchIndex,
                UpestTouchIndex = upestTouchIndex,
                DownestTouchIndex = downestTouchIndex,
            };

            return resultData;
        }
        
        //Please use next order of position types : left,right,up,down
        private static Tuple<List<Vector2>,List<Vector2>,List<Vector2>,List<Vector2>> 
            GetHorizontalTouchingIndices(byte[,] data,Vector4 lastIndexes)
        {    
            var LeftestTouchingIndexes = new List<Vector2>();
            var RightestTouchingIndexes = new List<Vector2>();
            var UpestTouchingIndexes = new List<Vector2>();
            var DownestTouchingIndexes = new List<Vector2>();

            int leftestIndex = (int)lastIndexes.x;
            int rightestIndex = (int)lastIndexes.y;
            int upestIndex = (int)lastIndexes.z;
            int downesttIndex = (int)lastIndexes.w;
            
            int rowsCount = data.GetLength(1);
            int columnsAmount = data.GetLength(0);

            for (int column = columnsAmount-1; column >= 0; column--)
            {
                for (int row = 0; row < rowsCount; row++)
                {
                    if (data[column, row] == 1)
                    {    
                        //Check horizontal borders point
                        if (row == leftestIndex)
                        {
                            LeftestTouchingIndexes.Add(new Vector2(row,column));
                        }     
                        if (row == rightestIndex)
                        {
                            RightestTouchingIndexes.Add(new Vector2(row+1,column+1));
                        }
                        
                        //Check vertical borders point
                        if (column == upestIndex)
                        {
                            UpestTouchingIndexes.Add(new Vector2(row+1,column+1));
                        }     
                        if (column == downesttIndex)
                        {
                            DownestTouchingIndexes.Add(new Vector2(row,column));
                        }
                    }
                }
            }
            
            return Tuple.Create(LeftestTouchingIndexes, RightestTouchingIndexes,UpestTouchingIndexes,DownestTouchingIndexes);
        }
            
    }
}
