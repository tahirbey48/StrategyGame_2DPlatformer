using UnityEngine;
using StrategyGame_2DPlatformer.GameManagement;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

namespace StrategyGame_2DPlatformer
{
    public class HighligtBuildingsAtMousePosition : MonoBehaviour
    {

        private Color highlightColor; // Is determined on each frame
        public Color unavalaibleColor;
        public Color availaibleColor;
        private Color originalColor; // If a tile is to be de-emphasized, it turns back to its original color.
        private List<Vector3Int> currentTilePositions; // Will depend on mouse position and building size when the class will be over
        private List<Vector3Int> previousPositions; // To store difference between two sets, observer which tiles are changed on mouse drag
        private int sizeX;
        private int sizeY;

        private void Start()
        {
            sizeX = 3;
            sizeY = 3;
            unavalaibleColor = Color.red;
            availaibleColor = Color.green;
            highlightColor = availaibleColor;
            originalColor = GameData.instance.Tilemap.GetColor(new Vector3Int(1, 1, 1));
            currentTilePositions = new List<Vector3Int>();
            previousPositions = new List<Vector3Int>();
        }

        void Update()
        {
            currentTilePositions.Clear();
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int tilePosition = GameData.instance.Tilemap.WorldToCell(mousePosition);

            for (int x = tilePosition.x - sizeX / 2; x <= tilePosition.x + sizeX / 2; x++)
            {
                for (int y = tilePosition.y - sizeY / 2; y <= tilePosition.y + sizeY / 2; y++)
                {
                    Vector3Int position = new Vector3Int(x, y, tilePosition.z);
                    currentTilePositions.Add(position);
                }
            }

            int occupiedCount = 0;
            if (currentTilePositions.Count != 0)
            {
                foreach (var item in currentTilePositions)
                {
                    bool occupied = false;
                    if (GameData.instance.Graph.GetNodeAtPosition(item) != null)
                    {
                        occupied = GameData.instance.Graph.GetNodeAtPosition(item).isOccupied;
                    }
                    if (occupied) { occupiedCount++; }
                }
            }
            // If any tile under the building is occupied, we count this position to be unavailaible
            if (occupiedCount != 0)
            {
                highlightColor = unavalaibleColor;
            }
            else
            {
                highlightColor = availaibleColor;
            }

            // Highlight the current tiles
            ChangeTileColors(currentTilePositions, highlightColor);
            // Unhighlight the previous tiles that are not in the current tiles
            var positionsToUnhighlight = previousPositions.Except(currentTilePositions);
            foreach (var pos in positionsToUnhighlight)
            {
                GameData.instance.Tilemap.SetTileFlags(pos, TileFlags.None);
                GameData.instance.Tilemap.SetColor(pos, originalColor);
            }
            // Save the current positions as previous positions for the next frame
            previousPositions.Clear();
            previousPositions = currentTilePositions;
        }

        private void ChangeTileColors(List<Vector3Int> positions, Color color)
        {
            //This function is used to change colors of tiles that are accessed through a list of positions.
            foreach (var pos in positions)
            {
                GameData.instance.Tilemap.SetTileFlags(pos, TileFlags.None);
                GameData.instance.Tilemap.SetColor(pos, color);
            }

        }
    }
}
