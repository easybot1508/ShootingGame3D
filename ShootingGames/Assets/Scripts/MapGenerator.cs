using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
   
    public Map[] maps;
    public int mapIndex;

    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform navMeshFloor;
    public Transform mapFloor;
    public Transform navMeshMarkPrefab;
    
    public Vector2 maxMapSize;
  

    [Range(0, 1)]
    public float outlinePercent;
    

    public float tileSize;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;
    Queue<Coord> shuffledOpenTileCoords;

    Map currentMap;

    Transform[,] tileMap;

    void Start()
    {
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber)
    {
        mapIndex = waveNumber - 1;
        GenerateMap();
    }

    public void GenerateMap()
    {
        currentMap = maps[mapIndex];
        tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
        //sap xep chieu` cao cua? cac' chuong' ngai. vat.
        System.Random prng = new System.Random(currentMap.seed);
        

        //tao. toa. do.
        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y)); 
            }
        }
        //xao' tron. toa. do. cua? tat' ca? cac' o
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));
       
        //tao. doi' tuong. giu~ ban? do`
        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        //Spawning tiles
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                //tinh' toan' noi ma` vien gach. dc dat.   
                Vector3 tilePosition = CoordToPosition(x, y);
               
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                //phan biet tung` tile khac nhau,Vector3.one * (1-outLinePercent) : giam? kich' thuoc' cua? tung` o theo duong` vien`,
                //* tileSize nhan them kich thuoc theo ti le do. lon cua? map
                newTile.localScale = Vector3.one * (1 - outlinePercent ) * tileSize;
                //khi nao` muon' tao. o moi' , ta dat. cha cua? o moi = mapHolder
                newTile.parent = mapHolder;
                tileMap[x, y] = newTile;
            }
        }

        //Spawning chuong' ngai. vat.
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

        //ta can` lay' ti? le. % cua? tat ca? cac' o tren map se~ dc chi? dinh. la chuong' ngai. vat.
        //xac dinh. so' luong. chuong' ngai. vat. can` tao. ,mapSize.x * mapSize.y: tong? so gach. * % chuong' ngai. vat. 
        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
        int currentObstacleCount = 0;
        List<Coord> allOpenCoords = new List<Coord>(allTileCoords);

        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;

            if (randomCoord != currentMap.mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight,(float)prng.NextDouble());
                //cap. nhat. vi. tri' cua? chuong' ngai. vat. tren ban? do`
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                //tao. chuong' ngai. vat. moi'
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight/2, Quaternion.identity) as Transform;
                newObstacle.parent = mapHolder;
                //kich thuoc cua? chuong ngai. vat.
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize,obstacleHeight, (1 - outlinePercent) * tileSize);
                //Mau` cua? cac' chuong' ngai. vat.
                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colourPercent = randomCoord.y / (float)currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColour, currentMap.backgroundColour, colourPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                //khi 1 chuong' ngai. vat. dc tao. ra, ta  se~ loai. bo? chuong' ngia. vat. do' khoi? danh sach' toa. do.
                allOpenCoords.Remove(randomCoord);
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }
        shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray(allOpenCoords.ToArray(), currentMap.seed));


        //tao. navMesh
        //tinh' toan' khu vuc. navMesh ma chung toi ko su? dung.
        //ben trai' ban? do`
        Transform markLeft = Instantiate(navMeshMarkPrefab,Vector3.left *((currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize), Quaternion.identity);
        markLeft.parent = mapHolder;
        //kich thuoc' that. su. ghi de` len cac' khu vuc. can` che
        //kich thuoc' cua? x se~ bang` khoang? cach' tu` canh. cua? ban? do` toi' da den' canh. cua? ban? do` thuc. te'
        markLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x)/2f,1, currentMap.mapSize.y) * tileSize;
        //ben phai? ban? do`
        Transform markRight = Instantiate(navMeshMarkPrefab, Vector3.right * ((currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize), Quaternion.identity);
        markRight.parent = mapHolder;
        //kich thuoc' that. su. ghi de` len cac' khu vuc. can` che
        //kich thuoc' cua? x se~ bang` khoang? cach' tu` canh. cua? ban? do` toi' da den' canh. cua? ban? do` thuc. te'
        markRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y )* tileSize;

        //ben tren ban? do`
        Transform markForward = Instantiate(navMeshMarkPrefab, Vector3.forward * ((currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize), Quaternion.identity);
        markForward.parent = mapHolder;
        //kich thuoc' that. su. ghi de` len cac' khu vuc. can` che
        //kich thuoc' cua? x se~ bang` khoang? cach' tu` canh. cua? ban? do` toi' da den' canh. cua? ban? do` thuc. te'
        markForward.localScale = new Vector3(maxMapSize.x,1,(maxMapSize.y - currentMap.mapSize.y) / 2f)  * tileSize;

        //ben duoi' ban? do`
        Transform markBottom = Instantiate(navMeshMarkPrefab, Vector3.back * ((currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize), Quaternion.identity);
        markBottom.parent = mapHolder;
        //kich thuoc' that. su. ghi de` len cac' khu vuc. can` che
        //kich thuoc' cua? x se~ bang` khoang? cach' tu` canh. cua? ban? do` toi' da den' canh. cua? ban? do` thuc. te'
        markBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;



        navMeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
        mapFloor.localScale = new Vector3(currentMap.mapSize.x * tileSize, currentMap.mapSize.y * tileSize);
    }

    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    { 
        //su? dung. thuat. toan FlootFill, cach hoat dong. la` khong co' chuong ngai. vat. o? trung tam ban? do` 
        //khi su? dung. thuat. toan' FlootFill no' se~ danh' dau' cac' o ma` ban. da~ xem , vi` vay. se~ ko thay dc su. lap. di lap. lai cung` 1 o
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(currentMap.mapCenter);
        mapFlags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;

        int accessibleTileCount = 1;

        //FlootFill
        while (queue.Count > 0)
        {
            //lay' muc. dau` tien trong queue, ma` con` loia. bo? no' ra khoi? queue
            Coord tile = queue.Dequeue();
            //lap. qua 8 o lan can. voi' tao. do. tile
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;//dai. dien. cho toa. do x cua? o lan can.
                    int neighbourY = tile.y + y;//dai. dien. cho toa. do y cua? o lan can.
                    //xem xet' cac' o lan can theo chieu` doc va` ngang, khong xet' theo duong` cheo'
                    if (x == 0 || y == 0)
                    {
                        //can` dam? bao? cac'toa. do. ton` tai. trong ban? do`
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            //kiem? tra o nay` da~ dc kiem? tra hay chua
                            //neu' o nay` chua dc kiem? tra va` no ko phai? la` o chuong' ngai. vat.
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                //o nay` da~ dc kiem? tra
                                mapFlags[neighbourX, neighbourY] = true;
                                //xem xet' o lan can tiep' theo
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    Vector3 CoordToPosition(int x, int y)
    {
        //vi` no' dc tao. tu` goc' ben trai' tren cung` cua? map lam` diem? bat' dau`
        //neu thuc hien. -mapSize.x /2, cac' o se~ dc. tao. tu` diem? di chuyen? sang ben trai'
        //bang` 1 nua? chieu` ngang cua map voi' toa. do x0
        //khi 1 o dc tao. tai. vi. tri' do',thi` tam cua? no' nam` o? toa. do. -mapSize.x /2
        //khong muon' dat. goc' tai. tam , de? thay doi? goc' thi` ta +0.5f
        //voi' moi~ vong` lap ta se~ + them gia tri. x de di chuyen qua, gia' tri. z cung~ tuong tu. nhu v
        // *  tileSize tang khoang? cach' giua~ cac vien gach
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) *  tileSize;
    }


    //Lay' vi. tri' nguoi` choi va` chuyen? doi? vi. tri' nay` thanh` toa. do. de? tim` ra o nao` o? do'
    public Transform GetTileFromPosition(Vector3 position)
    {
        //Mathf.RoundToInt:lam` tron` so' tu` 1.9 se~ thanh` 2
        int x =Mathf.RoundToInt((int)(position.x / tileSize + (currentMap.mapSize.x - 1) / 2f));
        int y = Mathf.RoundToInt((int)(position.z / tileSize + (currentMap.mapSize.y - 1) / 2f));
        //neu' ban. co' lay' vi. tri' ben ngoai` pham. vi cua? tileMap ban. se~ gap. loi~,vi` vay. chung' ta can` gioi' han. gia' tri. x,y hop.ly'
        //o? day chung' toi gioi' han. x nam` trong khoang? tu` 0 den' x chieu` dai` cua? tileMap
        x = Mathf.Clamp(x, 0, tileMap.GetLength(0) - 1);
        //o? day chung' toi gioi' han. y nam` trong khoang? tu` 0 den' y chieu` dai` cua? tileMap
        x = Mathf.Clamp(y, 0, tileMap.GetLength(1) - 1);
        return tileMap[x, y]; 
    } 

    //lay muc. tieu tiep theo tu` hang` doi. va` tra? ve` toa. do. ngau~ nhien 
    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    

    public Transform GetRandomOpenTile()
    {
        Coord randomCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(randomCoord);
        return tileMap[randomCoord.x, randomCoord.y];
    }

    [System.Serializable]
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
    }

    [System.Serializable]
    public class Map
    {
        public Coord mapSize;
        [Range(0,1)]
        public float obstaclePercent;
        public int seed;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColour;
        public Color backgroundColour;

        public Coord mapCenter
        {
            get
            {
                return new Coord(mapSize.x / 2, mapSize.y / 2);
            }
        }
    }
}