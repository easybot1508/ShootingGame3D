using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool devMode;
    public Wave[] waves;
    public Enemy enemy;

    Wave currentWave;
    int currentWaveNumber;
    int enemiesRemainingAlive;

    int enemiesRemainingToSpawn;
    //thoi` gian sinh san? tiep theo
    float nextSpawnTime;
    MapGenerator map;

    float timeBetweenCampingChecks  = 2;
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;
    bool isDisabled;

    public event System.Action<int> OnNewWave;

    //kiem? tra nguoi` choi co' di chuyen? hay ko , con` song hay da~ chet' ta can` tham chieu den LivingEntity
    LivingEntity playerEntity;
    //theo doi~ vi. tri' cua? nguoi` choi
    Transform playerT;

    private void Start()
    {
        
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;
        playerEntity.OnDeath += OnPlayerDeath;
        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;
        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }



    private void Update()
    {
        if (!isDisabled)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;
                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
                campPositionOld = playerT.position;
            }
            if ((enemiesRemainingToSpawn > 0 || currentWave.infinite)&& Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
                StartCoroutine("SpawnEnemy");
            }
        }
        if (devMode)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StopCoroutine("SpawnEnemy");
                //pha' huy? cac' doi' tuong. con` lai. trong wave do'
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }
                NextWave();
            }
        }
       
    }
    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;//nhap nhay' bao nhieu giay truoc' khi spawn ke? thu`
        float tileFlashSpeed = 4;//so lan` nhap nhay' moi~ giay
        Transform spawnTile = map.GetRandomOpenTile();
        if (isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerT.position);
        }
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        //mau` ban dau` cua? o tile
        Color initialColour = Color.white;
        Color flashColor = Color.red;
        float spawnTime = 0;//thoi gian spawn
        while(spawnTime < spawnDelay)
        {
            //lam` cho khoi hinh` nhap' nhay' mau`
            tileMat.color = Color.Lerp(initialColour, flashColor, Mathf.PingPong(spawnTime * tileFlashSpeed,1));
            spawnTime += Time.deltaTime;
            yield return null;
        }
        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
        spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);
       
    }

   

    void OnPlayerDeath()
    {
        isDisabled = true;
    }
    //nhan. thong bao ke? thu` da~ chet thong qua OnEnemyDeath() duoc goi. boi? OnDeath
    void OnEnemyDeath()
    {
        enemiesRemainingAlive--;
        if(enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }

    void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }
    void NextWave()
    {
        currentWaveNumber++;
        if(currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];
            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;
        }
        if(OnNewWave != null)
        {
            OnNewWave(currentWaveNumber);
        }
        ResetPlayerPosition();
    }

    [System.Serializable]
   public class Wave
    {
        public bool infinite;
        public int enemyCount;
        public float timeBetweenSpawns;

        public float moveSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public Color skinColor;
    }
}
