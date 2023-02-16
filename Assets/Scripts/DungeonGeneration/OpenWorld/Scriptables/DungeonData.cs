using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New dungeon", menuName = "Dungeon asset")]
public class DungeonData : ScriptableObject
{
    [SerializeField]
    private ScriptableObject activeLayout;
    private GameObject mono; //The monobehaviour using that data, i.e. the dungeon
    [SerializeField]
    RandomWalk randomWalkParameters; //already initialized in inspector
    [SerializeField]
    StandardCorridor corridorsParameters;
    [SerializeField]
    FractalCorridor fractalCorridorParameters;
    private DungeonWallGenerator walls = new DungeonWallGenerator();

    public void GenerateDungeon()
    {       
        ClearDungeon();
        fractalCorridorParameters.CreateFractalCorridor();
        randomWalkParameters.ExecuteRandomWalk();
        corridorsParameters.GenerateCorridors(randomWalkParameters);
        //walls.CreateWalls(corridorsParameters.GetPositionsBuffer());
        //DungeonDrawer.Draw(randomWalkParameters.GetPositionsBuffer(), mono);
        //DungeonLayoutDrawer.DrawWalls(walls.GetPositionsBuffer(), mono);
    }

    public void ClearDungeon() 
    {
        if (randomWalkParameters.GetMonoInstance() == null) 
            randomWalkParameters.SetMonoInstance(mono);
        if (walls.GetMonoInstance() == null)
            walls.SetMonoInstance(mono);
        if (corridorsParameters.GetMonoInstance() == null)
            corridorsParameters.SetMonoInstance(mono);
        if (fractalCorridorParameters.GetMonoInstance() == null)
            fractalCorridorParameters.SetMonoInstance(mono);

        randomWalkParameters.ClearBuffer();
        corridorsParameters.ClearBuffer();
        walls.ClearBuffer();
        DungeonDrawer.EraseDungeon(mono);
    }

    public DungeonWallGenerator GetWallsData() 
    {
        return walls; 
    }

    public void SaveData()
    {
        GetActiveLayout().SaveFloorData(fractalCorridorParameters.GetPositionsBuffer());
        GetActiveLayout().SaveWallsData(walls.GetPositionsBuffer()); //expected not to be null   
    }

    public void LoadData()
    {
        ClearDungeon();
        //DungeonDrawer.Draw(GetActiveLayout().GetFloorData(), mono);
        //DungeonLayoutDrawer.DrawWalls(GetActiveLayout().GetWallsData(), mono);
    }

    public void SetMonoInstance(GameObject mono)
    {
        this.mono = mono;
    }

    public DungeonLayout GetActiveLayout()
    {
        return activeLayout as DungeonLayout;
    }

    public HashSet<Vector2Int> GetFloorLayout()
    {
        HashSet<Vector2Int> floorLayout = new HashSet<Vector2Int>(randomWalkParameters.GetRooms2DLayout());
        floorLayout.UnionWith(corridorsParameters.GetCorridorsLayout());
        return floorLayout;
    }
}
