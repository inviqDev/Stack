using UnityEngine;

public class PlatformCutter : MonoBehaviour
{
    public static PlatformCutter Instance { get; private set; }

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void TryCutCurrentPlatform()
    {
        if (!Spawner.Instance)
        {
            Debug.Log("Spawner Instance is null");

            Time.timeScale = 0.0f;
            return;
        }

        var currentTowerTopObj = Spawner.Instance.CurrentTowerTopObj;
        var currentTowerSize = Spawner.Instance.CurrentTowerSize;
        var currentPlatformObj = Spawner.Instance.CurrentPlatformObj;
        var currentPlatformSize = Spawner.Instance.CurrentPlatformSize;

        var towerLeftEdgePosX = currentTowerTopObj.position.x - currentTowerSize.x * 0.5f;
        var towerRightEdgePosX = currentTowerTopObj.position.x + currentTowerSize.x * 0.5f;
        var currentPlatformLeftEdgePosX = currentPlatformObj.position.x - currentPlatformSize.x * 0.5f;

        var deltaX = currentTowerTopObj.position.x - currentPlatformObj.position.x;
        if (CheckIfGameIsOver(deltaX, currentPlatformSize))
        {
            GameManager.Instance?.ShowGameOverPrompt();
            return;
        }
        
        var remainPartWidth = currentPlatformSize.x - Mathf.Abs(deltaX);
        var towerTopRemainPart = SpawnTowerTopRemainPart(deltaX, currentPlatformLeftEdgePosX,
            towerLeftEdgePosX, remainPartWidth, currentPlatformObj, currentPlatformSize);

        var chunkPartWidth = currentPlatformSize.x - remainPartWidth;
        var chunkPartObject = SpawnChunkedPart(deltaX, towerRightEdgePosX, currentPlatformLeftEdgePosX,
            chunkPartWidth, currentPlatformObj, currentPlatformSize);

        GameManager.Instance?.ChangeCurrentScore();
        Spawner.Instance?.CreateNewMovingPlatform();
        Destroy(currentPlatformObj.gameObject);
    }

    private bool CheckIfGameIsOver(float deltaX, Vector3 currentPlatformSize)
    {
        return currentPlatformSize.x - Mathf.Abs(deltaX) < Spawner.Instance.MinPlatformSizeX;
    }

    private static GameObject SpawnChunkedPart(float deltaX, float towerRightEdgePosX,
        float currentPlatformLeftEdgePosX,
        float chunkPartWidth, Transform currentPlatformObj, Vector3 currentPlatformSize)
    {
        var chunkPartLeftEdgePosX = deltaX <= 0.0f ? towerRightEdgePosX : currentPlatformLeftEdgePosX;

        var chunkPartRightEdgePosX = chunkPartLeftEdgePosX + chunkPartWidth;
        var chunkPartCenterPosX = (chunkPartLeftEdgePosX + chunkPartRightEdgePosX) * 0.5f;

        var chunkPartSpawnPos =
            new Vector3(chunkPartCenterPosX, currentPlatformObj.position.y, currentPlatformObj.position.z);
        var chunkPartSize = new Vector3(chunkPartWidth, currentPlatformSize.y, currentPlatformSize.z);

        var chunkPartObject = Spawner.Instance.SpawnNewPlatform("Chunked_Part", chunkPartSpawnPos, chunkPartSize);
        chunkPartObject.AddComponent<Rigidbody>();

        return chunkPartObject;
    }

    private static GameObject SpawnTowerTopRemainPart(float deltaX, float currentPlatformLeftEdgePosX,
        float towerLeftEdgePosX, float remainPartWidth, Transform currentPlatformObj, Vector3 currentPlatformSize)
    {
        var remainPartLeftEdgePosX = deltaX <= 0.0f ? currentPlatformLeftEdgePosX : towerLeftEdgePosX;
        var remainPartRightEdgePosX = remainPartLeftEdgePosX + remainPartWidth;
        var remainPartCenterPosX = (remainPartLeftEdgePosX + remainPartRightEdgePosX) * 0.5f;

        var remainPartSpawnPos = new Vector3(remainPartCenterPosX, currentPlatformObj.position.y,
            currentPlatformObj.position.z);
        var remainPartSize = new Vector3(remainPartWidth, currentPlatformSize.y, currentPlatformSize.z);

        var towerTopRemainPart =
            Spawner.Instance.SpawnNewPlatform("Tower_Top_Remain_Part", remainPartSpawnPos, remainPartSize);

        Spawner.Instance.SetNewTowerTopObjTransform(towerTopRemainPart.transform);
        Spawner.Instance.SetCurrentTowerSize(remainPartSize);
        Spawner.Instance.SetCurrentPlatformSize(remainPartSize);

        return towerTopRemainPart;
    }

    private static void SpawnChunkPartObject(float deltaX, float currentPlatformLeftEdgePosX,
        float towerLeftEdgePosX, Vector3 currentPlatformSize, Transform currentPlatformObj)
    {
        var chunkPartLeftEdgePosX = deltaX <= 0.0f ? currentPlatformLeftEdgePosX : towerLeftEdgePosX;
        var chunkPartWidth = Mathf.Abs(deltaX);

        var chunkPartCenterX = (chunkPartLeftEdgePosX + (chunkPartLeftEdgePosX + chunkPartWidth)) * 0.5f;

        var chunkPartSize = new Vector3(chunkPartWidth, currentPlatformSize.y, currentPlatformSize.z);
        var chunkPartSpawnPos =
            new Vector3(chunkPartCenterX, currentPlatformObj.position.y, currentPlatformObj.position.z);

        var chunkPartObject = Spawner.Instance.SpawnNewPlatform("ChunkPart_Obj", chunkPartSpawnPos, chunkPartSize);
        chunkPartObject.AddComponent<Rigidbody>();
    }
}