using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Linq;

public class ProceduralMapGenerator : MonoBehaviour
{
    public GameObject pathPrefab;
    public GameObject holePrefab;
    public GameObject playerPrefab;
    public GameObject boostPadPrefab;
    public GameObject wallPrefab;
    public bool isOnline;
    
    public class MapData
    {
        public List<Vector2> path;
        public List<Vector2> boostPads;
        public Vector2 start;
        public Vector2 end;
        
        public MapData(List<Vector2> path, List<Vector2> boostPads)
        {
            this.path = path;
            this.boostPads = boostPads;
            this.start = path.Last();
            this.end = path[0];
        }
    }

    void Awake()
    {
        MapData mapData;
        mapData = isOnline ? SessionData.mapData : GetMapData();

        Vector3 endV3 = new Vector3(mapData.end.x, 0, mapData.end.y);
        // set what neighbours to check when creating a edge path
        List<Vector2> top = new List<Vector2> { new(1, 1), new(-1, 1) };
        List<Vector2> bottom = new List<Vector2> { new(1, -1), new(-1, -1) };
        List<Vector2> left = new List<Vector2> { new(-1, 1), new(-1, -1) };
        List<Vector2> right = new List<Vector2> { new(1, 1), new(1, -1) };
        List<List<Vector2>> sides = new List<List<Vector2>> { bottom, left, top, right };

        // create the hole
        Instantiate(holePrefab, Vector3.zero, Quaternion.Euler(Vector3.zero));
        // check all relevant neighbours of nodes on the path and create edge paths where needed
        foreach (Vector2 pathVec in mapData.path)
        {
            for (int i = 0; i < sides.Count; i++)
            {
                Vector2 point = VectorUtil2D.getPointInBetween(sides[i][0], sides[i][1]);
                if (!mapData.path.Contains(point + pathVec))
                {
                    Instantiate(wallPrefab, (new Vector3(pathVec.x, 0, pathVec.y) - endV3) * 10, Quaternion.Euler(new Vector3(0, i * 90, 0)));
                }
            }

            // continue if we are at the end node so we dont cover the hole
            if (pathVec == mapData.end)
            {
                continue;
            }

            // create path
            Instantiate(pathPrefab, (new Vector3(pathVec.x, 0, pathVec.y) - endV3) * 10, Quaternion.Euler(Vector3.zero));
        }

        // create boost pads
        foreach (Vector2 boostPadPos in mapData.boostPads)
        {
            if (boostPadPos == mapData.end || boostPadPos == mapData.start)
            {
                continue;
            }

            Vector2 delta = boostPadPos - mapData.end;

            // create boost pad and rotate it to face the hole, but round the rotation the the nearest multiple of 45
            Instantiate(boostPadPrefab, (new Vector3(boostPadPos.x, 0, boostPadPos.y) - endV3) * 10, Quaternion.Euler(new Vector3(0,
                MathUtil.RoundToMultiple(MathUtil.radiansToDegrees(Mathf.Atan2(delta.x, delta.y)), 45), 0)));
        }
        // create the player
        Instantiate(playerPrefab, (new Vector3(mapData.start.x, 0, mapData.start.y) - endV3) * 10, Quaternion.Euler(new Vector3()));
    }

    public static List<Vector2> GetVectorsFromCsv(List<string> contents)
    {
        List<Vector2> path = new List<Vector2>();
        // remove coulomb ids
        contents.RemoveAt(0);
        foreach (string line in contents)
        {
            // separate two values 
            string[] xy = line.Split(",");
            path.Add(new Vector2(Int16.Parse(xy[0]), Int16.Parse(xy[1])));
        }

        return path;
    }

    public static void RunPathGeneration()
    {
        string command = "python.exe ../procGen/";
        string arguments = "";
        
        Process process = new Process();

        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = $"/C {command} {arguments}";
        process.StartInfo.WorkingDirectory = "./venv";
        process.StartInfo.UseShellExecute = false;
        //process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        //Debug.Log(process.StandardOutput.ReadToEnd());
        process.WaitForExit();
    }

    public static MapData GetMapData()
    {
        RunPathGeneration();
        string pathCsv = "./path.csv";
        string boostCsv = "./boost.csv";
        // get coords of every node along the path
        List<string> csvPath = new List<string>(File.ReadAllText(pathCsv).Split("\n"));
        List<Vector2> path = GetVectorsFromCsv(csvPath);
        // get boost pad positions
        List<string> csvBoost = new List<string>(File.ReadAllText(boostCsv).Split("\n"));
        List<Vector2> boostPads = GetVectorsFromCsv(csvBoost);

        return new MapData(path, boostPads);
    }
}