using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Map : MonoBehaviour {
	public int width=10, height=10;					//Ancho y alto de la grilla en celdas (enteros)
	public float startX, startZ;					//Inicio de la grilla en coordenadas de mundo
	public float raycastStartY = 120f;				//Donde iniciar los raycasts que "llueven"
	public float cellSize = 1f;						//Tamaño (ancho y largo) de una celda
	public float maxAngle = 40f;					//Angulo maximo entre un nodo y otro respecto del piso (para conectarlo)
	public Vector3 nodeOffset = Vector3.up;			//Offset para el nodo (cuanto correrlo desde el punto de intersección entre rayo y terreno)
    public LayerMask maskToDetectNode;
    public LayerMask maskNotConnectNode;

	public MapNode[,] grid { get; private set; }	//Grilla [X,Z]
    
    Vector3 positionOfRaycast;
    HashSet<MapNode> visited = new HashSet<MapNode>();
    public static Map instance { get; private set; }

    void Start() {

        if (instance == null)
            instance = this;
        
    }

    public void Build() {
        if (cellSize < 1f) cellSize = 1f;

        grid = new MapNode[width, height];

        RaycastingAndSettingNodesOnTerrainAndGrid();
        ConeccionOfNodes();
        SettingAccessibles(grid[0,0]);

        //Debug.Log(grid.GetLength(0));
        //Debug.Log(grid.Cast<MapNode>().Where(x => x != null).Where(x => x.accessible == true).ToList().Count());
    }

    void RaycastingAndSettingNodesOnTerrainAndGrid() {
        positionOfRaycast = new Vector3(startX, raycastStartY, startZ);

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                RaycastHit rh;
                //la Y del for en las coordenadas del mundo seria Z
                if (Physics.Raycast(positionOfRaycast, Vector3.down, out rh, raycastStartY, maskToDetectNode)) { 
                        var newNode = new MapNode(rh.point + nodeOffset);
                        grid[x, y] = newNode; 
                }
                else grid[x, y] = null;
                //null porque significa que esta fuera del terrain al pasar esto
                //avanzamos 1 pedazo grid en x del world
                positionOfRaycast += new Vector3(width / cellSize, 0f, 0f);
            }
            // avanzamos 1 pedazo de grid en z del world
            positionOfRaycast.x = startX;
            positionOfRaycast += new Vector3(0f, 0f, height / cellSize);
        }
    }

    void ConeccionOfNodes() {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (grid[x, y] == null)
                    continue;

                for (int i = 0; i < 8; i++) {
                    var nextX = x + Utility.xMoveVector8[i];
                    var nextY = y + Utility.yMoveVector8[i];

                    if (
                        0 <= nextX && nextX < width
                        && 0 <= nextY && nextY < height
                        && grid[nextX, nextY] != null
                        && AngleCorrect(grid[x, y], grid[nextX, nextY])
                    ) {
                        var direction = grid[nextX, nextY].position - grid[x, y].position;
                        var dirNormalized = direction.normalized;
                        var dirMag = direction.magnitude;
                        if(!Physics.Raycast(grid[x, y].position, dirNormalized, dirMag,maskNotConnectNode))
                            grid[x,y].adjacent.Add(grid[nextX, nextY]);
                    }
                } 
            }
        }
    }

    void SettingAccessibles(MapNode node) {
        if (node == null || visited.Contains(node))
            return;

        visited.Add(node);
        node.accessible = true;

        foreach (var adj in node.adjacent)
            SettingAccessibles(adj);
    }

    bool AngleCorrect(MapNode current, MapNode next) {
        Vector3 deltaVector = current.position - next.position;
        Vector3 Projection2 = Vector3.ProjectOnPlane(deltaVector, new Vector3(0, 1, 0));
        float angle = Vector3.Angle(deltaVector, Projection2);

        return angle < maxAngle ;
    }

    //IA2-P1
    public MapNode GetRandomMapNodeAccesible() { 
        return grid.Cast<MapNode>()
            .Where(x => x != null)
            .Where(x => x.accessible == true)
            .Skip(UnityEngine.Random.Range(0, grid.Cast<MapNode>().Where(x => x != null).Where(x => x.accessible == true).Count()))
            .First(); 
    }

    //IA2-P1
    public MapNode FindClosestNode(Vector3 point) {
        return grid.Cast<MapNode>().Where(x=> x != null)
                   .Aggregate(Tuple.Create(new MapNode(float.MaxValue, float.MaxValue, float.MaxValue),float.MaxValue), (a,c) => {
                       var distanceFromActualNodeToPoint = Vector3.Distance(c.position, point);
                       if (distanceFromActualNodeToPoint < a.Item2)
                           return Tuple.Create(c, distanceFromActualNodeToPoint);
                       else
                           return a;
                   }).Item1;
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.black;
		Gizmos.DrawWireCube(
			  new Vector3(startX+width/2, raycastStartY/2f, startZ+height/2)
			, new Vector3(width, raycastStartY, height)
		);

		Gizmos.color = Color.green;
        
        if (grid != null) { 
            foreach (var node in grid) {
                if (node == null)
                    continue;

                if (node.usedInPath) {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(node.position, 1f);
                    continue;
                }
                if (node.accessible) { 
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(node.position, 1f);
                    foreach (var nodeAdj in node.adjacent) {
                        if (nodeAdj != null)
                            Gizmos.DrawLine(node.position, nodeAdj.position);
                    }
                }
                else {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(node.position, 1f);
                    foreach (var nodeAdj in node.adjacent) {
                        if (nodeAdj != null)
                            Gizmos.DrawLine(node.position, nodeAdj.position);
                    }
                }
            }
        }
    }
}
