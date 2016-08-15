using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class MatchSeams : EditorWindow {
    
    SkinnedMeshRenderer source;
    int numPieces;
    bool saveSource = false;

    
    SkinnedMeshRenderer[] pieces;


    [MenuItem("Tools/Match Seams")]
    private static void DisplayWindow()
    {

        GetWindow<MatchSeams>("Match Seams");

    }

    void OnEnable()
    {
        numPieces = 1;
        pieces = new SkinnedMeshRenderer[numPieces];

    }

    void OnGUI()
    {

        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.Space();
            //This is the armor piece the other pieces will be matching their seams to
            source = (SkinnedMeshRenderer)EditorGUILayout.ObjectField("Source armor piece:", source, typeof(SkinnedMeshRenderer), true);
            saveSource = EditorGUILayout.Toggle("Save source", saveSource);

            EditorGUILayout.Space();

            //The number of armor pieces we will be matching to the source
            numPieces = EditorGUILayout.IntField("Number of armor pieces:", numPieces);
            if (GUI.changed)
            {
                pieces = new SkinnedMeshRenderer[numPieces];
            }
            for (int i = 0; i < pieces.Length; i++)
            {
                pieces[i] = (SkinnedMeshRenderer)EditorGUILayout.ObjectField("Armor piece " + (i + 1), pieces[i], typeof(SkinnedMeshRenderer), true);
            }

            EditorGUILayout.Space();


            //Apply button
            if (GUILayout.Button("Apply"))
            {
                //Store all the vertices that are on the seams to this dictionary
                Dictionary<Vector3, int> sourceEdgeVertices = GetSourceEdgeVertices(source);

                //Loop through each armor piece
                for (int i = 0; i < pieces.Length; i++)
                {
                    //Store all the vertices that are on the seams to this dictionary
                    Dictionary<int, Vector3> edgeVertices = GetEdgeVertices(pieces[i]);

                    //Save the current piece's normals to this list
                    //The normals that border the source armor piece 
                    //will be matched to the sources normals
                    List<Vector3> normals = new List<Vector3>();
                    normals.AddRange(pieces[i].sharedMesh.normals);


                    //Loop through this piece's vertices that lie on the seams/edges
                    foreach (KeyValuePair<int, Vector3> currentVertex in edgeVertices)
                    {
                        int index;

                        //The currentVertex value is its position which will
                        //match the position of the source edge vertex. This
                        //match will give us the source's vertex index
                        if (sourceEdgeVertices.ContainsKey(currentVertex.Value))
                        {
                            index = sourceEdgeVertices[currentVertex.Value];
                        }
                        else
                        {
                            index = -1;
                        }
                       
                        //Assign the vertex normal from the source
                        if(index != -1) normals[currentVertex.Key] = source.sharedMesh.normals[index];

                    }

                    //Now we set all normals for this current piece
                    pieces[i].sharedMesh.SetNormals(normals);


                    
                    //Does the folder exist?
                    if(!AssetDatabase.IsValidFolder("Assets/Armor Meshes"))
                    {
                        AssetDatabase.CreateFolder("Assets", "Armor Meshes");
                    }


                    //Create the new mesh
                    Mesh tempMesh = Instantiate(pieces[i].sharedMesh);
                    AssetDatabase.CreateAsset(tempMesh, "Assets/Armor Meshes/" + pieces[i].sharedMesh.name + ".asset");

                    //Save source mesh
                    if (saveSource)
                    {
                        tempMesh = Instantiate(source.sharedMesh);
                        AssetDatabase.CreateAsset(tempMesh, "Assets/Armor Meshes/" + source.sharedMesh.name + ".asset");
                    }

                }
            }
        }
        EditorGUILayout.EndVertical();

    }



    Dictionary<int, Vector3> GetEdgeVertices(SkinnedMeshRenderer meshRenderer)
    {
        //Get all triangle edges on this armor piece
        Dictionary<Vector3, EdgeOccurrence> edges = GetEdges(meshRenderer);

        //We get all edges that are only in the dictionary once. 
        //This means they are not sharing an edge with another triangle
        //and must be on the edge of the armor piece
        Dictionary<int, Vector3> vertices = new Dictionary<int, Vector3>();
        foreach (KeyValuePair<Vector3, EdgeOccurrence> kvp in edges)
        {
            if (kvp.Value.occurrence == 1)
            {
                Edge edge = kvp.Value.edge;

                //Store the edge's vertices
                try
                {
                    vertices.Add(edge.vertex1.index, edge.vertex1.position);
                }
                catch (ArgumentException)
                {

                }


                try
                {
                    vertices.Add(edge.vertex2.index, edge.vertex2.position);
                }
                catch (ArgumentException)
                {

                }
            }
        }
        return vertices;
    }


    Dictionary<Vector3, int> GetSourceEdgeVertices(SkinnedMeshRenderer meshRenderer)
    {
        //Get all triangle edges on this source armor piece
        Dictionary<Vector3, EdgeOccurrence> edges = GetEdges(meshRenderer);

        //We get all edges that are only in the dictionary once. 
        //This means they are not sharing an edge with another triangle
        //and must be on the edge of the source armor piece
        Dictionary<Vector3, int> vertices = new Dictionary<Vector3, int>();
        foreach (KeyValuePair<Vector3, EdgeOccurrence> kvp in edges)
        {
            if (kvp.Value.occurrence == 1)
            {
                Edge edge = kvp.Value.edge;

                //Store the edge's vertices
                try
                {
                    vertices.Add(edge.vertex1.position, edge.vertex1.index);
                }
                catch (ArgumentException)
                {

                }


                try
                {
                    vertices.Add(edge.vertex2.position, edge.vertex2.index);
                }
                catch (ArgumentException)
                {

                }
            }
        }
        return vertices;
    }






    Edge CreateEdge(int index1, int index2, Mesh mesh)
    {
        //First get the two vertex positions
        Vector3 vert1Pos = new Vector3((float)Math.Round(mesh.vertices[index1].x, 3), (float)Math.Round(mesh.vertices[index1].y, 3), (float)Math.Round(mesh.vertices[index1].z, 3));
        Vector3 vert2Pos = new Vector3((float)Math.Round(mesh.vertices[index2].x, 3), (float)Math.Round(mesh.vertices[index2].y, 3), (float)Math.Round(mesh.vertices[index2].z, 3));

        //Create two vertices with their index and position
        Vertex vertex1 = new Vertex(index1, vert1Pos);
        Vertex vertex2 = new Vertex(index2, vert2Pos);

        //Return the new edge
        return new Edge(vertex1, vertex2);
    }


    void StoreEdges(Edge edge, ref Dictionary<Vector3, EdgeOccurrence> edges)
    {
        //The edge position is the average of its two vertex positions
        Vector3 edgePosition = (edge.vertex1.position + edge.vertex2.position) / 2;

        //Store the edges in the dictionary and mark how many times this edge occurres
        //An occurrence of more than once means this edge is sharing a space with another edge
        //and is not on the seam of the armor piece
        try
        {
            edges.Add(edgePosition, new EdgeOccurrence(edge, 1));
        }
        catch (ArgumentException)
        {
            int occurrence = edges[edgePosition].occurrence + 1;
            edges[edgePosition].occurrence = occurrence;
        }
    }


    Dictionary<Vector3, EdgeOccurrence> GetEdges(SkinnedMeshRenderer meshRenderer)
    {
        Mesh mesh = meshRenderer.sharedMesh;
        Dictionary<Vector3, EdgeOccurrence> edges = new Dictionary<Vector3, EdgeOccurrence>();

        //Loop through all the triangles of this armor piece and get all of their edges
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            int index1 = mesh.triangles[i];
            int index2 = mesh.triangles[i + 1];
            int index3 = mesh.triangles[i + 2];

            StoreEdges(CreateEdge(index1, index2, mesh), ref edges);
            StoreEdges(CreateEdge(index2, index3, mesh), ref edges);
            StoreEdges(CreateEdge(index1, index3, mesh), ref edges);
        }

        return edges;
    }

}


struct Edge
{
    public Vertex vertex1;
    public Vertex vertex2;

    public Edge(Vertex vert1, Vertex vert2)
    {
        vertex1 = vert1;
        vertex2 = vert2;
    }
}


class EdgeOccurrence
{
    public Edge edge;
    public int occurrence;

    public EdgeOccurrence(Edge e, int o)
    {
        edge = e;
        occurrence = o;
    }
}



public struct Vertex
{
    public int index;
    public Vector3 position;

    public Vertex(int index, Vector3 pos)
    {
        this.index = index;
        position = pos;
    }
}
